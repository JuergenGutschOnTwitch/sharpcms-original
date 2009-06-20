//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public class AttributeList
    {
        private readonly XmlNode _xmlNode;

        public AttributeList(XmlNode xmlNode)
        {
            _xmlNode = xmlNode;
        }

        public string this[string name]
        {
            get { return CommonXml.GetAttributeValue(_xmlNode, name); }
            set { CommonXml.SetAttributeValue(_xmlNode, name, value); }
        }
    }
}