using System;
using System.Linq;

using DP.LSP.Tools.DiskMon.Configuration;
using DP.LSP.Tools.DiskMon.Core;

namespace DP.LSP.Tools.DiskMon
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.Logger.Info("Disk Monitor started");        
            ConfigurationHelper.Load();

            var lowDrives = DiskManager.GetLowDrives();
            if (lowDrives.Any())
            {
                lowDrives.ToList().ForEach(LogHelper.Logger.Info);
                EmailHelper.Send(lowDrives, DiskManager.GetDrives(d => d.IsReady && !d.IsLow));
            }
            else
                EmailHelper.SendReport(DiskManager.GetDrives(d => d.IsReady));

            //Console.WriteLine("Press any key to continue...");
            //Console.Read();
        }
    }
}
