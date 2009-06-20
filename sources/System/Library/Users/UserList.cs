//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.Users
{
    public class UserList : DataElementList
    {
        public UserList(XmlNode parentNode)
            : base(parentNode)
        {
        }

        private User this[int index]
        {
            get
            {
                string xPath = string.Format("user[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    var user = new User(node);
                    return user;
                }
                return null;
            }
        }

        public User this[string name]
        {
            get
            {
                //ToDo this is not implementet yet (old)
                string xPath = string.Format("user[login='{0}']", Common.CleanToSafeString(name));
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    var user = new User(node);
                    return user;
                }
                return null;
            }
        }

        public User Create(string login)
        {
            XmlNode node = Document.CreateElement("user");
            ParentNode.AppendChild(node);
            var user = new User(node) {Login = login};

            return user;
        }

        public void Remove(int index) //ToDo Is a unused Method (T.Huber 18.06.2009)
        {
            User user = this[index];
            ParentNode.RemoveChild(user.Node);
        }

        public void Remove(string name)
        {
            User user = this[name];
            ParentNode.RemoveChild(user.Node);
        }
    }
}