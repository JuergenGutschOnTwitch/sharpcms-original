//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.Users
{
    public class Group : DataElement
    {
        public Group(XmlNode node)
            : base(node)
        {
        }

        public string Name
        {
            get { return CommonXml.GetAttributeValue(Node, "name"); }
            set { CommonXml.SetAttributeValue(Node, "name", Common.CleanToSafeString(value)); }
        }
    }
}