using System.Configuration;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public class DriveElement : ConfigurationElement
    {
        private const string NAME_KEY = "name";
        private const string THRESHOLD_KEY = "threshold";

        [ConfigurationProperty(NAME_KEY, IsRequired = true)]
        public string Name
        {
            get { return (string)this[NAME_KEY]; }
            set { this[NAME_KEY] = value; }
        }

        [ConfigurationProperty(THRESHOLD_KEY, IsRequired = true)]
        public int Threshold
        {
            get { return (int)this[THRESHOLD_KEY]; }
            set { this[THRESHOLD_KEY] = value; }
        }
    }
}
