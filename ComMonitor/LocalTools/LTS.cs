using HexMessageViewerControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor.LocalTools
{
    /// <summary>
    /// class LTS
    /// Local Static Tools
    /// </summary>
    class LTS
    {
        static Random _random = new Random();

        /// <summary>
        /// AddARandomTestLine
        /// </summary>
        /// <returns></returns>
        static public byte[] AddARandomTestLine()
        {
            byte[] barray = new byte[(int)RandomDouble(1, 100, 0)];

            _random.NextBytes(barray);

            // Just for testing with a fix and defined stream
            //barray = new byte[]{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x66 };

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
    }
}
