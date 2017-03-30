using System;

namespace DP.LSP.Tools.DiskMon.Core
{
    internal sealed class SizeConverter
    {
        private static readonly string[] SUFFIXES = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string Convert(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + Convert(-value); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SUFFIXES[i]);
        }
    }
}
