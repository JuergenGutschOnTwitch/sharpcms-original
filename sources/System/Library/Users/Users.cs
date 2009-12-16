//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.Users
{
    public class Users
    {
        private readonly GroupList groupList;
        private readonly XmlDocument userDocument;
        private readonly string userFileName;
        private readonly UserList userList;

        public Users(Process process)
        {
            userFileName = process.Settings["users/filename"];
            userDocument = new XmlDocument();
            userDocument.Load(userFileName);
            userList = new UserList(CommonXml.GetNode(userDocument.DocumentElement, "users"));
            groupList = new GroupList(CommonXml.GetNode(userDocument.DocumentElement, "groups"));
        }

        public UserList UserList
        {
            get { return userList; }
        }

        public GroupList GroupList
        {
            get { return groupList; }
        }

        public void Save()
        {
            userDocument.Save(userFileName);
        }
    }
}