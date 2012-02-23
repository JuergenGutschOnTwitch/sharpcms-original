// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Library;

namespace Sharpcms.Data.SiteTree
{
    public class ContainerList : DataElementList
    {
        public ContainerList(XmlNode parentNode) : base(parentNode)
        {
        }

        public Container this[int index]
        {
            get
            {
                string xPath = string.Format("container[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                return node != null ? new Container(node) : null;
            }
        }

        public Container this[string name]
        {
            get
            {
                string xPath = string.Format("container[@name='{0}']", name);
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                if (node != null)
                {
                    return new Container(node);
                }

                node = Document.CreateElement("container");
                XmlAttribute typeAttribute = Document.CreateAttribute("name");
                typeAttribute.Value = name;
                if (node.Attributes != null)
                {
                    node.Attributes.Append(typeAttribute);
                }

                ParentNode.AppendChild(node);
                return new Container(node);
            }
        }

        public void Remove(int index)
        {
            string xPath = string.Format("container[{0}]", index + 1);
            XmlNode node = ParentNode.SelectSingleNode(xPath);
            if(node != null)
            {
                ParentNode.RemoveChild(node);
            }
        }

        public int Index(string containerName)
        {
            string xPath = string.Format("container");
            XmlNodeList nodes = ParentNode.SelectNodes(xPath);
            int i = 0;
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    i++;
                    if (node.Attributes != null && node.Attributes["name"].Value == containerName)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }
    }
}