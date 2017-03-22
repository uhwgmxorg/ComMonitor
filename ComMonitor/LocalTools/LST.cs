using System;

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
            string Location = AppDomain.CurrentDomain.BaseDirectory;
            string Filter = mask;
            var dialog = new Microsoft.Win32.SaveFileDialog { FileName = initialFileName, InitialDirectory = Location, Filter = Filter };
            dialog.ShowDialog();

            return dialog.FileName;
        }

    }
}
