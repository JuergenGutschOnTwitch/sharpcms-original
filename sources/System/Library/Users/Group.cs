// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Library.Common;

namespace Sharpcms.Library.Users
{
    public class Group : DataElement
    {
        public Group(XmlNode node) : base(node) { }

        public string Name
        {
            get { return CommonXml.GetAttributeValue(Node, "name"); }
            set { CommonXml.SetAttributeValue(Node, "name", Common.Common.CleanToSafeString(value)); }
        }
    }
}