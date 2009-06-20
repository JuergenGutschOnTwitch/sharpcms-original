//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public class DataElement
    {
        private readonly AttributeList _attributes;
        private readonly XmlNode _xmlNode;

        protected DataElement(XmlNode node)
        {
            _xmlNode = node;
            _attributes = new AttributeList(node);
        }

        public XmlNode Node
        {
            get { return _xmlNode; }
        }

        public XmlDocument Document //ToDo: Is a unused Property (T.Huber 18.06.2009)
        {
            get { return _xmlNode != null ? _xmlNode.OwnerDocument : null; }
        }

        protected AttributeList Attributes
        {
            get { return _attributes; }
        }

        protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
        {
            return CommonXml.GetNode(_xmlNode, cleanPath, emptyNode);
        }

        private XmlNode SelectNode(string xPath)
        {
            return _xmlNode.SelectSingleNode(xPath);
        }

        protected string GetNodeValue(string name)
        {
            if (_xmlNode == null)
                return string.Empty;

            XmlNode node = SelectNode(name);
            return node != null ? node.InnerText : string.Empty;
        }
    }
}