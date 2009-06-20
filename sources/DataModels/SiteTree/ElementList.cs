//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.SiteTree
{
    public class ElementList : DataElementList
    {
        public ElementList(XmlNode parentNode)
            : base(parentNode)
        {
        }

        public Element this[int index]
        {
            get
            {
                string xPath = string.Format("element[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    var element = new Element(node);
                    return element;
                }
                return null;
            }
        }

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Element Create(string type)
        {
            XmlNode node = Document.CreateElement("element");
            ParentNode.AppendChild(node);
            var element = new Element(node) {Type = type};

            return element;
        }

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Element Create(string type, string name)
        {
            XmlNode node = Document.CreateElement("element");
            ParentNode.AppendChild(node);
            var element = new Element(node) {Type = type, Name = name, Publish = true.ToString()};

            return element;
        }

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns></returns>
        public Element Create(string type, string name, bool publish)
        {
            XmlNode node = Document.CreateElement("element");
            ParentNode.AppendChild(node);
            var element = new Element(node) {Type = type, Name = name, Publish = publish.ToString().ToLower()};

            return element;
        }

        public void Remove(int index)
        {
            Element element = this[index];
            ParentNode.RemoveChild(element.Node);
        }

        public void MoveUp(int index)
        {
            Element element = this[index];
            CommonXml.MoveUp(element.Node);
        }

        public void Copy(int index)
        {
            Element element = this[index];
            CommonXml.Copy(element.Node, element.Node.ParentNode);
        }

        public void MoveDown(int index)
        {
            Element element = this[index];
            CommonXml.MoveDown(element.Node);
        }

        public void MoveTop(int index)
        {
            Element element = this[index];
            CommonXml.MoveTop(element.Node);
        }
    }
}