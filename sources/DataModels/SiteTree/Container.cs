//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.SiteTree
{
    public class Container : DataElement
    {
        public ElementList Elements;
        
		public Container(XmlNode node)
			: base(node)
		{
            XmlNode elementNode = CommonXml.GetNode(Node,"elements",EmptyNodeHandling.CreateNew);
            Elements = new ElementList(elementNode);
		}
    }
}
