using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem.Library;
using System.Xml;

namespace InventIt.SiteSystem.Data.Users
{
    public class UserList : DataElementList
    {
        public UserList(XmlNode parentNode)
			: base(parentNode)
		{
		}

		public User Create(string login)
		{
			XmlNode node = Document.CreateElement("user");
			ParentNode.AppendChild(node);
			User user = new User(node);
            user.Login = login;

            return user;
		}
        
        public void Remove(int index)
        {
            User user = this[index];
            ParentNode.RemoveChild(user.Node);
        }
        public void Remove(string name)
        {
            User user = this[name];
            ParentNode.RemoveChild(user.Node);
        }

        public User this[int index]
		{
			get
			{
				string xPath = string.Format("user[{0}]", index + 1);
				XmlNode node = ParentNode.SelectSingleNode(xPath);

				if (node != null)
				{
                    User user = new User(node);
                    return user;
				}
				return null;
			}
		}

        public User this[string name]
        {
            get
            {//todo this is not implementet yet
                string xPath = string.Format("user[login='{0}']",Common.CleanToSafeString(name) );
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    User user = new User(node);
                    return user;
                }
                return null;
            }
        }
    }
}
