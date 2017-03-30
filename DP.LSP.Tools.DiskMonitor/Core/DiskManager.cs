using System;
using System.Collections.Generic;
using System.Linq;

using DP.LSP.Tools.DiskMon.Configuration;

namespace DP.LSP.Tools.DiskMon.Core
{
    interface IDiskManager
    {
        string MachineName { get; }
        IEnumerable<Drive> GetDrives(Func<Drive, bool> predicate);
        IEnumerable<Drive> GetLowDrives();
    }

    internal class DiskManager : IDiskManager
    {
        public string MachineName
        {
            get { return System.Environment.MachineName; }
        }

        public IEnumerable<Drive> GetDrives(Func<Drive, bool> predicate)
        {
            return ServiceLocator.Instance.GetService<IConfigurationHelper>().Drives.Where(predicate);
        }   

        public IEnumerable<Drive> GetLowDrives()
        {
            return GetDrives(d => d.IsLow);
        }       
    }
}
