using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ComMonitor.LocalTools
{
    /// <summary>
    /// class LST
    /// Local Static Tools
    /// </summary>
    class LST
    {
        static Random _random = new Random();

        /// <summary>
        /// RandomByteArray
        /// </summary>
        /// <returns></returns>
        static public byte[] RandomByteArray()
        {
            byte[] barray = new byte[(int)RandomDouble(1, 100, 0)];

            _random.NextBytes(barray);

            return barray;
        }

        /// <summary>
        /// RandomDouble
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="deci"></param>
        /// <returns></returns>
        public static double RandomDouble(double min, double max, int deci)
        {
            double d;
            d = _random.NextDouble() * (max - min) + min;
            return Math.Round(d, deci);
        }

        /// <summary>
        /// StreamToByteArray
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// GetOpenFileName
        /// </summary>
        /// <returns></returns>
        public static string OpenFileDialog(string mask = "Xml Datein (*.xml)|*.xml;|Alle Dateien (*.*)|*.*\"")
        {
            string Location = AppDomain.CurrentDomain.BaseDirectory;
            string Filter = mask;
            var dialog = new Microsoft.Win32.OpenFileDialog { InitialDirectory = Location, Filter = Filter };
            dialog.ShowDialog();

            return dialog.FileName;
        }

        /// <summary>
        /// GetSaveSettingsFileName
        /// </summary>
        /// <returns></returns>
        public static string SaveFileDialog(string mask = "Xml Datein (*.xml)|*.xml;|Alle Dateien (*.*)|*.*\"", string initialFileName = "NewConnection")
        {
            string resultFileName;
            string Location = AppDomain.CurrentDomain.BaseDirectory;
            string Filter = mask;
            var dialog = new Microsoft.Win32.SaveFileDialog { FileName = initialFileName, InitialDirectory = Location, Filter = Filter };
            var result = dialog.ShowDialog();

            resultFileName = dialog.FileName;

            // detect Cancel
            if (result == false)
                resultFileName = "";

            return resultFileName;
        }

        /// <summary>
        /// LoadList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static public List<T> LoadList<T>(string file)
        {
            List<T> list = null;

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                using (StreamReader rd = new StreamReader(file))
                {
                    list = xs.Deserialize(rd) as List<T>;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                list = new List<T>();
            }
            return list;
        }

        /// <summary>
        /// SaveList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        static public void SaveList<T>(List<T> list, string file)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                using (StreamWriter wr = new StreamWriter(file))
                {
                    xs.Serialize(wr, list);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// SaveClass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        static public void SaveClass<T>(T obj, string file)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (StreamWriter wr = new StreamWriter(file))
                {
                    xs.Serialize(wr, obj);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// LoadClass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        static public bool LoadClass<T>(ref T obj, string file)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (StreamReader rd = new StreamReader(file))
                {
                    var Obj = serializer.Deserialize(rd);
                    obj = (T)Obj;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// GetCurrentMethod
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}
