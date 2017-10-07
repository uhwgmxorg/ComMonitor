﻿//////////////////////////////////////////////
// Apache 2.0  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfHexaEditor.Core.Converters
{
    /// <summary>
    /// Used to convert long value to hexadecimal string like this 0xFFFFFFFF.
    /// </summary>
    public sealed class LongToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string defaultRtn = "0x00000000";

            return value != null
                ? (long.TryParse(value.ToString(), out var longValue)
                    ? (longValue > -1
                        ? "0x" + longValue
                              .ToString(ConstantReadOnly.HexLineInfoStringFormat, CultureInfo.InvariantCulture)
                              .ToUpper()
                        : defaultRtn)
                    : defaultRtn)
                : defaultRtn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}