// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Globalization;
using System.Xml;
using Sharpcms.Library.Common;

namespace Sharpcms.Library.Users
{
    public class User : DataElement
    {
        private readonly GroupList _groupList;

        public User(XmlNode node) : base(node)
        {
            _groupList = new GroupList(CommonXml.GetNode(node, "groups"));
        }

        public GroupList GroupList
        {
            get { return _groupList; }
        }

        public string Login
        {
            get { return CommonXml.GetNode(Node, "login").InnerText; }
            set { CommonXml.GetNode(Node, "login").InnerText = Common.Common.CleanToSafeString(value); }
        }

        public string Password
        {
            set { CommonXml.GetNode(Node, "password").InnerText = Common.Common.CleanToSafeString(value).GetHashCode().ToString(CultureInfo.InvariantCulture); }
        }

        public bool CheckPassword(string password)
        {
            return CommonXml.GetNode(Node, "password").InnerText == Common.Common.CleanToSafeString(password).GetHashCode().ToString(CultureInfo.InvariantCulture);
        }
    }
}