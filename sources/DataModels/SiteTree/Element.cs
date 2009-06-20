//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.SiteTree
{
    public class Element : DataElement
    {
        public Element(XmlNode node)
            : base(node)
        {
        }

        public string this[string name]
        {
            get
            {
                string xPath = string.Format("{0}", name);
                XmlNode node = GetNode(xPath, EmptyNodeHandling.CreateNew);
                return node.InnerText;
            }
            set
            {
                string xPath = string.Format("{0}", name);
                XmlNode node = GetNode(xPath, EmptyNodeHandling.CreateNew);
                node.InnerText = value;
            }
        }

        public string Type
        {
            get { return Attributes["type"]; }
            set { Attributes["type"] = value; }
        }
    }
}