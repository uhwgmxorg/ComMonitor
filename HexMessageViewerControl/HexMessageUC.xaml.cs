using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexMessageViewerControl
{
    public enum Direction
    {
        In,
        Out
    }

    /// <summary>
    /// Interaktionslogik für HexMessageUC.xaml
    /// </summary>
    public partial class HexMessageUC : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region INotify Propertie Changed
        private String internByteStream;
        public String InternByteStream
        {
            get { return internByteStream; }
            set
            {
                if (value != InternByteStream)
                {
                    internByteStream = value;
                    OnPropertyChanged("InternByteStream");
                }
            }
        }
        #endregion

        #region Dependencie Propertys
        public byte[] HexContentByte
        {
            get { return (byte[])GetValue(HexContentByteProperty); }
            set
            {
                SetValue(HexContentByteProperty, value);
            }
        }
        public static readonly DependencyProperty HexContentByteProperty =
            DependencyProperty.Register("HexContentByte", typeof(byte[]), typeof(HexMessageUC), new PropertyMetadata(null));
        #endregion

        private Direction messageDirection;
        public Direction MessageDirection
        {
            get
            {
                return messageDirection;
            }
            set
            {
                messageDirection = value;
                AddMessage(HexContentByte);
            }
        }

        private bool isSecected;
        public bool IsSecected
        {
            get
            {
                return isSecected;
            }
            set
            {
                isSecected = value;
                if (IsSecected)
                {
                    lableByteStream.Foreground = new SolidColorBrush(Colors.White);
                    lableByteStream.Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    lableByteStream.Foreground = new SolidColorBrush(Colors.Black);
                    lableByteStream.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public HexMessageUC()
        {
            InitializeComponent();
            DataContext = this;

            IsSecected = false;
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Grid_MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                IsSecected = !IsSecected;
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// AddMessage
        /// </summary>
        /// <param name="massage"></param>
        private void AddMessage(byte[] massage)
        {
            string inputString;

            inputString = PrepareStringForOutPut(massage);

            InternByteStream = inputString;
        }

        /// <summary>
        /// PrepareStringForOutPut
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private string PrepareStringForOutPut(byte[] inputStream)
        {
            string outputString = "";
            byte[][] outputByteLines = null;

            outputByteLines = SplitByteArray(inputStream);

            foreach (var b in outputByteLines)
                if (outputByteLines.First() == b)
                    if(MessageDirection == Direction.In)
                        outputString += String.Format("IN  -> {0,-48} |{1,-16}|\n", ByteArrayToHexString(b), ByteArrayToAsciiString(b));
                    else
                        outputString += String.Format("OUT <- {0,-48} |{1,-16}|\n", ByteArrayToHexString(b), ByteArrayToAsciiString(b));
                else
                    outputString += String.Format("       {0,-48} |{1,-16}|\n", ByteArrayToHexString(b), ByteArrayToAsciiString(b));

            outputString = outputString.Remove(outputString.Length - 1);
            return outputString;
        }
        private byte[][] SplitByteArray(byte[] input)
        {
            byte[][] output = null;
            int i = 0;

            var b = input.Split(16);
            output = new byte[b.ToArray().Length][];
            foreach (var c in b)
            {
                var d = c;
                output[i++] = d.ToArray();
            }

            return output;
        }

        /// <summary>
        /// ByteArrayToHexString
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private string ByteArrayToHexString(byte[] buf)
        {
            System.Text.StringBuilder hex = new System.Text.StringBuilder(buf.Length * 2);
            foreach (byte b in buf)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }

        /// <summary>
        /// ByteArrayToAsciiString
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private string ByteArrayToAsciiString(byte[] buf)
        {
            char[] carray = new char[buf.Length];
            char c;

            for (int i = 0; i < buf.Length; i++)
            {
                if (33 <= buf[i] && buf[i] <= 126)
                    c = (char)buf[i];
                else
                    c = '.';
                carray[i] = c;
                if (buf[i] == 0x5f) // '_' invisible ??
                    carray[i] = '.';
            }

            return new String(carray);
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }

    /// <summary>
    /// class ExtentionClass
    /// extension for split the byte array
    /// </summary>
    static class ExtentionClass
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
