using System.IO;

namespace DP.LSP.Tools.DiskMon.Core
{
    internal class Drive
    {
        #region Fields
        private DriveInfo _cache = null;
        #endregion

        #region Propeties
        public long FreeSpace
        {
            get { return IsReady ? Info.AvailableFreeSpace : 0; }
        }

        private DriveInfo Info
        {
            get { return _cache = _cache ?? new DriveInfo(Name); }
        }

        public bool IsLow
        {
            get { return IsReady ? (((double)FreeSpace / TotalSize * 100)) <= Threshold : false; }
        }

        public bool IsReady
        {
            get { return Info.IsReady; }
        }

        public string Name { get; private set; }

        internal long UsedSpace
        {
            get { return IsReady ? TotalSize - FreeSpace : 0; }
        }

        public int Threshold { get; private set; }

        public long TotalSize
        {
            get { return IsReady ? Info.TotalSize : 0; }
        }
        #endregion

        #region Ctor
        public Drive(string name, int threshold)
        {
            Name = name;
            Threshold = threshold;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Format("{0} {1} free of {2}", Name, SizeConverter.Convert(FreeSpace), SizeConverter.Convert(TotalSize));
        } 
        #endregion
    }
}
