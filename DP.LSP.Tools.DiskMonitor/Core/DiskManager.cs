using System;
using System.Collections.Generic;
using System.Linq;

using DP.LSP.Tools.DiskMon.Configuration;

namespace DP.LSP.Tools.DiskMon.Core
{
    internal sealed class DiskManager
    {
        public static string MachineName
        {
            get { return System.Environment.MachineName; }
        }

        public static IEnumerable<Drive> GetDrives(Func<Drive, bool> predicate)
        {
            return ConfigurationHelper.Drives.Where(predicate);
        }   

        public static IEnumerable<Drive> GetLowDrives()
        {
            return GetDrives(d => d.IsLow);
        }       
    }
}
