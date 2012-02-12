// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Collections.Generic;
using System.Xml;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;
using Sharpcms.Library.Users;

namespace Sharpcms.Providers.Base
{
    public class ProviderUser : BasePlugin2, IPlugin2
    {
        private Users _users;

        public ProviderUser()
        {
        }

        public ProviderUser(Process process)
        {
            Process = process;
        }

        private Users Users
        {
            get { return _users ?? (_users = new Users(Process)); }
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "User"; }
        }

        public new string[] Implements
        {
            get { return new[] {"users"}; }
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "saveuser":
                    HandleSaveUser();
                    break;
                case "deleteuser":
                    HandleDeleteUser();
                    break;
                case "adduser":
                    HandleAddUser();
                    break;
                case "addgroup":
                    HandleAddGroup();
                    break;
                case "deletegroup":
                    HandleDeleteGroup();
                    break;
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "users":
                    LoadUsers(control);
                    break;
                case "groups":
                    LoadGroups(control);
                    break;
                case "user":
                    LoadUser(control, pathTrail);
                    break;
                case "frontpage":
                    FrontPage();
                    break;
            }
        }

        public new object Invoke(string api, string action, params object[] args)
        {
            switch (api)
            {
                case "users":
                    return InvokeUsers(action, args);
            }

            return null;
        }

        #endregion

        private void HandleDeleteGroup()
        {
            var users = new Users(Process);
            users.GroupList.Remove(Process.QueryEvents["mainvalue"]);
            users.Save();
        }

        private void HandleAddGroup()
        {
            var users = new Users(Process);

            if (users.GroupList[Process.QueryEvents["mainvalue"]] != null)
            {
                return;
            }

            Group group = users.GroupList.Create(Process.QueryEvents["mainvalue"]);

            if (group.Name.Length > 0)
            {
                users.Save();
            }
        }

        private void HandleAddUser()
        {
            var users = new Users(Process);

            if (users.UserList[Process.QueryEvents["mainvalue"]] != null) return;

            User user = users.UserList.Create(Process.QueryEvents["mainvalue"]);

            if (user.Login.Length > 0)
            {
                users.Save();
            }
        }

        private void HandleSaveUser()
        {
            var users = new Users(Process);

            User user = users.UserList[Process.QueryEvents["mainvalue"]];
            user.Login = Process.QueryData["user_login"];
            if ("emptystring" != Process.QueryData["user_password"])
            {
                user.Password = Process.QueryData["user_password"];
            }

            user.GroupList.Clear();

            string groups = Process.QueryData["user_groups"];
            foreach (string groupname in groups.Split(','))
            {
                user.GroupList.Create(groupname);
            }

            users.Save();
        }

        private void HandleDeleteUser()
        {
            var users = new Users(Process);
            users.UserList.Remove(Process.QueryEvents["mainvalue"]);
            users.Save();
        }

        private void FrontPage()
        {
            bool redirected = false;
            object[] results = Process.Plugins.InvokeAll("users", "list_groups", Process.CurrentUser);
            var userGroups = new List<string>(Common.FlattenToStrings(results));

            foreach (string group in userGroups)
            {
                string xPath = string.Format("groups/item[@name='{0}']", group);
                XmlNode node = null;
                try
                {
                    node = Process.Settings.GetAsNode(xPath);
                }
                catch
                {
                    // Ignore
                }

                if (node != null)
                {
                    string frontPage = CommonXml.GetAttributeValue(node, "frontpage");
                    if (!string.IsNullOrEmpty(frontPage))
                    {
                        redirected = true;
                        Process.HttpPage.Response.Redirect(frontPage + "/");
                    }
                }
            }

            if (!redirected)
            {
                string defaultFrontPage = Process.Settings["groups/defaultfrontpage"];
            }
        }

        private void LoadGroups(ControlList control)
        {
            control["groups"].InnerXml = Users.GroupList.ParentNode.InnerXml;
        }

        private void LoadUser(ControlList control, string value)
        {
            var users = new Users(Process);
            if (value != null && users.UserList[value] != null)
            {
                control["user"].InnerXml = users.UserList[value].Node.InnerXml;
            }
        }

        private void LoadUsers(ControlList control)
        {
            var users = new Users(Process);
            control["users"].InnerXml = users.UserList.ParentNode.InnerXml;
        }

        private object InvokeUsers(string action, object[] args)
        {
            switch (action)
            {
                case "verify":
                    return UsersVerify(args);
                case "list_groups":
                    return UsersGroups(args);
            }

            return null;
        }

        private object UsersVerify(object[] args)
        {
            if (args == null || args.Length < 2)
            {
                return null;
            }

            string username = args[0].ToString();
            string password = args[1].ToString();

            User user = Users.UserList[username];
            return user != null && user.CheckPassword(password);
        }

        private object UsersGroups(object[] args)
        {
            if (args == null || args.Length < 1)
            {
                return null;
            }

            string username = args[0].ToString();
            var groups = new List<string>();

            User user = Users.UserList[username];
            if (user != null)
            {
                int groupCount = user.GroupList.Count;
                for (int i = 0; i < groupCount; i++)
                {
                    groups.Add(user.GroupList[i].Name);
                }
            }

            return groups.Count > 0 ? groups.ToArray() : null;
        }
    }
}