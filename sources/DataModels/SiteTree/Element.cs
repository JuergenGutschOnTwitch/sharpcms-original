using System;
using System.Collections.Generic;
using System.Text;
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
            get
            {
				return Attributes["type"];
            }
            set
            {
				Attributes["type"] = value;
            }
        }
    }
}
