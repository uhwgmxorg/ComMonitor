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
    }
}
