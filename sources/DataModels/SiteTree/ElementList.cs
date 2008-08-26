//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
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

		public Element Create(string type)
		{
			XmlNode node = Document.CreateElement("element");
			ParentNode.AppendChild(node);
			Element element = new Element(node);
			element.Type = type;

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

		public Element this[int index]
		{
			get
			{
				string xPath = string.Format("element[{0}]", index + 1);
				XmlNode node = ParentNode.SelectSingleNode(xPath);

				if (node != null)
				{
					Element element = new Element(node);
					return element;
				}
				return null;
			}
		}
	}
}
