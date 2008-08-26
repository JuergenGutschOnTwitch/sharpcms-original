using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.Users
{
    public class User : DataElement
    {
        private GroupList m_GroupList;
        public GroupList GroupList
        {
            get
            {
                return m_GroupList;
            }
        }

        public string Login
        {
            get
            {
                return CommonXml.GetNode(Node, "login").InnerText;
            }
            set 
            {
                CommonXml.GetNode(Node, "login").InnerText = Common.CleanToSafeString(value);
            }
        }

        public String Password
        {
            set
            {
                CommonXml.GetNode(Node, "password").InnerText =Common.CleanToSafeString(value).GetHashCode().ToString();
            }
            
        }

        public bool CheckPassword(string password)
        {
            return CommonXml.GetNode(Node, "password").InnerText == Common.CleanToSafeString(password).GetHashCode().ToString();
        }
        
        public User(XmlNode node)
			: base(node)
        {
            m_GroupList = new GroupList(CommonXml.GetNode(node, "groups"));
		}
    }
}
