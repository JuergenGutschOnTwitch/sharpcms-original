// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Library.Common;

namespace Sharpcms.Library
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
            get
            {
                string attribute = CommonXml.GetAttributeValue(_xmlNode, name);

                return attribute;
            }
            set
            {
                CommonXml.SetAttributeValue(_xmlNode, name, value);
            }
        }
    }
}