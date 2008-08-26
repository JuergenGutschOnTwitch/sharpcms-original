//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace InventIt.SiteSystem.Library
{
	public class AttributeList
	{
		private XmlNode m_XmlNode;

		public AttributeList(XmlNode xmlNode)
		{
			m_XmlNode = xmlNode;
		}

		public string this[string name]
		{
			get
			{
				return CommonXml.GetAttributeValue(m_XmlNode, name);
			}
			set
			{
				CommonXml.SetAttributeValue(m_XmlNode, name, value);
			}
		}
	}
}
