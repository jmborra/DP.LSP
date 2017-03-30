using System.Configuration;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public class SettingsSection : ConfigurationSection
    {
        private const string DRIVES_KEY = "drives";
        private const string RECIPIENTS_KEY = "recipients";

        [ConfigurationProperty(DRIVES_KEY)]
        [ConfigurationCollection(typeof(DriveElementCollection), AddItemName = "add")]
        public DriveElementCollection Drives
        {
            get { return (DriveElementCollection)base[DRIVES_KEY]; }
        }

        [ConfigurationProperty(RECIPIENTS_KEY)]
        [ConfigurationCollection(typeof(DriveElementCollection), AddItemName = "add")]
        public RecipientElementCollection Recipients
        {
            get { return (RecipientElementCollection)base[RECIPIENTS_KEY]; }
        }
    }
}
