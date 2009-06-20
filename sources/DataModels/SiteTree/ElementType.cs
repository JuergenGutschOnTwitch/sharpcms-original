//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.SiteTree
{
    public class ElementType : DataElement
    {
        public ElementType(XmlNode node)
            : base(node)
        {
        }

        public string FriendlyName
        {
            get { return GetNodeValue("friendlyname"); }
        }

        public string Description
        {
            get { return GetNodeValue("description"); }
        }
    }
}