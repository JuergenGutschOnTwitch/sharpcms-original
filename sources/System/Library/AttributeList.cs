//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public class AttributeList
    {
        private readonly XmlNode xmlNode;

        public AttributeList(XmlNode xmlNode)
        {
            this.xmlNode = xmlNode;
        }

        public string this[string name]
        {
            get { return CommonXml.GetAttributeValue(xmlNode, name); }
            set { CommonXml.SetAttributeValue(xmlNode, name, value); }
        }
    }
}