//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public class DataElementList
    {
        private readonly XmlNode parentNode;

        protected DataElementList(XmlNode parentNode)
        {
            this.parentNode = parentNode;
        }

        public XmlNode ParentNode
        {
            get { return parentNode; }
        }

        protected XmlDocument Document
        {
            get { return parentNode != null ? parentNode.OwnerDocument : null; }
        }

        public int Count
        {
            get
            {
                if (parentNode == null)
                {
                    return 0;
                }

                XmlNodeList xmlNodeList = parentNode.SelectNodes("*");
                return xmlNodeList == null ? 0 : xmlNodeList.Count;
            }
        }

        protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
        {
            return CommonXml.GetNode(parentNode, cleanPath, emptyNode);
        }
    }
}