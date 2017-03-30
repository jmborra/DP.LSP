using System.Configuration;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public class RecipientElement : ConfigurationElement
    {
        private const string EMAIL_KEY = "email";

        [ConfigurationProperty(EMAIL_KEY, IsRequired = true)]
        public string Email
        {
            get { return (string)this[EMAIL_KEY]; }
            set { this[EMAIL_KEY] = value; }
        }
    }
}
