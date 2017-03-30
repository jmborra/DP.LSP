using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

using DP.LSP.Tools.DiskMon.Core;

namespace DP.LSP.Tools.DiskMon.Core
{
    internal sealed class LogHelper
    {
        private static ILog _instance = LogManager.GetLogger("DP.LSP.Tools.DiskMon");
        public static ILog Instance
        {
            get { return _instance; }
        }
    }
}
