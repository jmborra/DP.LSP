using System;
using System.Linq;

using log4net;

using DP.LSP.Tools.DiskMon.Configuration;
using DP.LSP.Tools.DiskMon.Core;

namespace DP.LSP.Tools.DiskMon
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.Instance.Info("Disk Monitor started");
            RegisterServices();

            var config = ServiceLocator.Instance.GetService<IConfigurationHelper>();
            config.Load();

            var diskManager = ServiceLocator.Instance.GetService<IDiskManager>();
            var email = ServiceLocator.Instance.GetService<IEmailHelper>();    

            var lowDrives = diskManager.GetLowDrives();
            if (lowDrives.Any())
            {
                lowDrives.ToList().ForEach(LogHelper.Instance.Info);
                email.SendAlert(lowDrives, diskManager.GetDrives(d => d.IsReady && !d.IsLow));
            }
            else
                email.SendReport(diskManager.GetDrives(d => d.IsReady));

            //Console.WriteLine("Press any key to continue...");
            //Console.Read();
        }

        static void RegisterServices()
        {
            ServiceLocator.Instance.Register<IConfigurationHelper>(new ConfigurationHelper());
            ServiceLocator.Instance.Register<IDiskManager>(new DiskManager());
            ServiceLocator.Instance.Register<IEmailHelper>(new EmailHelper());
        }
    }
}
