using System.Configuration;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public class RecipientElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RecipientElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RecipientElement)element).Email;
        }
    }
}
