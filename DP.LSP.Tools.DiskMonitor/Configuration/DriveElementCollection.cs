using System.Collections.Generic;
using System.Configuration;

namespace DP.LSP.Tools.DiskMon.Configuration
{
    public class DriveElementCollection : ConfigurationElementCollection, IEnumerable<DriveElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DriveElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DriveElement)element).Name;
        }

        IEnumerator<DriveElement> IEnumerable<DriveElement>.GetEnumerator()
        {
            var count = base.Count;
            for (var i = 0; i < count; i++)
                yield return base.BaseGet(i) as DriveElement;
        }
    }
}
