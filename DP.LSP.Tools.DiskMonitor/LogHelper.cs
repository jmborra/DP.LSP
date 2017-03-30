
using log4net;

namespace DP.LSP.Tools.DiskMon
{
    internal sealed class LogHelper
    {
        private static readonly ILog logger = LogManager.GetLogger("DP.LSP.Tools.DiskMon");
        public static ILog Logger
        {
            get { return logger; }
        }
    }
}
