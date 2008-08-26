//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace InventIt.SiteSystem.Library
{
	public class DataElementList
	{
		private XmlNode m_ParentNode;

		public XmlNode ParentNode
		{
			get
			{
				return m_ParentNode;
			}
		}

		protected XmlDocument Document
		{
			get
			{
				if (m_ParentNode != null)
				{
					return m_ParentNode.OwnerDocument;
				}
				return null;
			}
		}

		public DataElementList(XmlNode parentNode)
		{
			m_ParentNode = parentNode;
		}

        protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
        {
			return CommonXml.GetNode(m_ParentNode, cleanPath, emptyNode);
        }

		public int Count
		{
			get
			{
				if (m_ParentNode == null)
				{
					return 0;
				}
				else
				{
					XmlNodeList xmlNodeList = m_ParentNode.SelectNodes("*");
					if (xmlNodeList == null)
					{
						return 0;
					}
					else
					{
						return xmlNodeList.Count;
					}
				}
			}
		}
	}
}
