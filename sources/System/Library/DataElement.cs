//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace InventIt.SiteSystem.Library
{
	public class DataElement
	{
		private XmlNode m_XmlNode;
		private AttributeList m_Attributes;

		public XmlNode Node
        {
            get
            {
                return m_XmlNode;
            }
        }
               
        public XmlDocument Document
        {
            get
            {
                if (m_XmlNode != null)
                {
                    return m_XmlNode.OwnerDocument;
                }
                return null;
            }
        }

		protected AttributeList Attributes
		{
			get
			{
				return m_Attributes;
			}
		}

		public DataElement(XmlNode node)
		{
			m_XmlNode = node;
			m_Attributes = new AttributeList(node);
		}

		protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
		{
			return CommonXml.GetNode(m_XmlNode, cleanPath, emptyNode);
		}

        protected XmlNode SelectNode(string xPath)
        {
            return m_XmlNode.SelectSingleNode(xPath);
        }

        protected string GetNodeValue(string name)
        {
            if (m_XmlNode == null)
            {
                return string.Empty;
            }

            XmlNode node = SelectNode(name);
            if (node != null)
            {
                return node.InnerText;
            }

            return string.Empty;
        }
	}
}