//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.Users
{
    public class User : DataElement
    {
        private readonly GroupList groupList;

        public User(XmlNode node)
            : base(node)
        {
            groupList = new GroupList(CommonXml.GetNode(node, "groups"));
        }

        public GroupList GroupList
        {
            get { return groupList; }
        }

        public string Login
        {
            get { return CommonXml.GetNode(Node, "login").InnerText; }
            set { CommonXml.GetNode(Node, "login").InnerText = Common.CleanToSafeString(value); }
        }

        public string Password
        {
            set { CommonXml.GetNode(Node, "password").InnerText = Common.CleanToSafeString(value).GetHashCode().ToString(); }
        }

        public bool CheckPassword(string password)
        {
            return CommonXml.GetNode(Node, "password").InnerText ==
                   Common.CleanToSafeString(password).GetHashCode().ToString();
        }
    }
}