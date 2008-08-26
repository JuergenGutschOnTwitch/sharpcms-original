using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.SiteTree
{
	public class ContainerList : DataElementList
	{
		public ContainerList(XmlNode parentNode)
			: base(parentNode)
		{
		}

        public void Remove(int index)
        {
            string xPath = string.Format("container[{0}]", index + 1);
			XmlNode node = ParentNode.SelectSingleNode(xPath);
            ParentNode.RemoveChild(node);
        }

		public Container this[int index]
		{
			get
			{
				string xPath = string.Format("container[{0}]", index + 1);
				XmlNode node = ParentNode.SelectSingleNode(xPath);
				if (node != null)
				{
					return new Container(node);
				}
				return null;
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
				node.Attributes.Append(typeAttribute);

				ParentNode.AppendChild(node);
				return new Container(node);
			}
		}
	}
}
