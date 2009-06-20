//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Collections.Generic;
using System.Xml;
using InventIt.SiteSystem.Data.Users;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderUser : BasePlugin2, IPlugin2
    {
        private Users _users;

        public ProviderUser()
        {
        }

        public ProviderUser(Process process)
        {
            _process = process;
        }

        private Users Users
        {
            get
            {
                if (_users == null)
                    _users = new Users(_process);

                return _users;
            }
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
            var mUsers = new Users(_process);
            mUsers.GroupList.Remove(_process.QueryEvents["mainvalue"]);
            mUsers.Save();
        }

        private void HandleAddGroup()
        {
            var mUsers = new Users(_process);

            if (mUsers.GroupList[_process.QueryEvents["mainvalue"]] != null) return;

            Group group = mUsers.GroupList.Create(_process.QueryEvents["mainvalue"]);

            if (group.Name.Length > 0)
                mUsers.Save();
        }

        private void HandleAddUser()
        {
            var mUsers = new Users(_process);

            if (mUsers.UserList[_process.QueryEvents["mainvalue"]] != null) return;

            User user = mUsers.UserList.Create(_process.QueryEvents["mainvalue"]);

            if (user.Login.Length > 0)
                mUsers.Save();
        }

        private void HandleSaveUser()
        {
            var mUsers = new Users(_process);

            User user = mUsers.UserList[_process.QueryEvents["mainvalue"]];
            user.Login = _process.QueryData["user_login"];
            if ("emptystring" != _process.QueryData["user_password"])
                user.Password = _process.QueryData["user_password"];

            user.GroupList.Clear();

            string groups = _process.QueryData["user_groups"];
            foreach (string groupname in groups.Split(','))
                user.GroupList.Create(groupname);

            mUsers.Save();
        }

        private void HandleDeleteUser()
        {
            var mUsers = new Users(_process);
            mUsers.UserList.Remove(_process.QueryEvents["mainvalue"]);
            mUsers.Save();
        }

        private void FrontPage()
        {
            bool redirected = false;
            object[] results = _process.Plugins.InvokeAll("users", "list_groups", _process.CurrentUser);
            var userGroups = new List<string>(Common.FlattenToStrings(results));

            foreach (string group in userGroups)
            {
                string xPath = string.Format("groups/item[@name='{0}']", group);
                XmlNode node = null;
                try
                {
                    node = _process.Settings.GetAsNode(xPath);
                }
                catch
                {
                    // Ignore
                }

                if (node == null) continue;

                string frontPage = CommonXml.GetAttributeValue(node, "frontpage");

                if (string.IsNullOrEmpty(frontPage)) continue;

                redirected = true;
                _process.HttpPage.Response.Redirect(frontPage + ".aspx");
            }

            if (!redirected)
            {
                string defaultFrontPage = _process.Settings["groups/defaultfrontpage"]; //ToDo: ??? (T.Huber 18.06.2009)
            }
        }

        private void LoadGroups(ControlList control)
        {
            var mUsers = new Users(_process);
            control["groups"].InnerXml = mUsers.GroupList.ParentNode.InnerXml;
        }

        private void LoadUser(ControlList control, string value)
        {
            var mUsers = new Users(_process);
            if (value != null && mUsers.UserList[value] != null)
                control["user"].InnerXml = mUsers.UserList[value].Node.InnerXml;
        }

        private void LoadUsers(ControlList control)
        {
            var mUsers = new Users(_process);
            control["users"].InnerXml = mUsers.UserList.ParentNode.InnerXml;
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
                return null;

            string username = args[0].ToString();
            string password = args[1].ToString();

            User user = Users.UserList[username];
            return user != null && user.CheckPassword(password);
        }

        private object UsersGroups(object[] args)
        {
            if (args == null || args.Length < 1)
                return null;

            string username = args[0].ToString();
            var groups = new List<string>();

            User user = Users.UserList[username];
            if (user != null)
            {
                int groupCount = user.GroupList.Count;
                for (int i = 0; i < groupCount; i++)
                    groups.Add(user.GroupList[i].Name);
            }

            return groups.Count > 0 ? groups.ToArray() : null;
        }
    }
}