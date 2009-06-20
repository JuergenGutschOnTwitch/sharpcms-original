//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public class DataElementList
    {
        private readonly XmlNode _parentNode;

        protected DataElementList(XmlNode parentNode)
        {
            _parentNode = parentNode;
        }

        public XmlNode ParentNode
        {
            get { return _parentNode; }
        }

        protected XmlDocument Document
        {
            get { return _parentNode != null ? _parentNode.OwnerDocument : null; }
        }

        public int Count
        {
            get
            {
                if (_parentNode == null)
                    return 0;

                XmlNodeList xmlNodeList = _parentNode.SelectNodes("*");
                return xmlNodeList == null ? 0 : xmlNodeList.Count;
            }
        }

        protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
        {
            return CommonXml.GetNode(_parentNode, cleanPath, emptyNode);
        }
    }
}