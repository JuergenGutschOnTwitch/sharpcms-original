//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Data.Users;
using System.Xml;


namespace InventIt.SiteSystem.Providers
{
    public class ProviderUser : BasePlugin2, IPlugin2
    {
        private Users m_Users;
        private Users users
        {
            get
            {
                if (m_Users == null)
                {
                    m_Users = new Users(m_Process);
                }
                return m_Users;
            }
        }

        public new string Name
        {
            get
            {
                return "User";
            }
        }

        public ProviderUser()
        {
        }

        public new string[] Implements
        {
            get
            {
                return new string[] { "users" };
            }
        }

        public ProviderUser(Process process)
        {
            m_Process = process;
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
  
        public void HandleDeleteGroup()
        {
            Users users = new Users(m_Process);
            users.GroupList.Remove(m_Process.QueryEvents["mainvalue"]);
            users.Save();
        }

        public void HandleAddGroup()
        {
            Users users = new Users(m_Process);
            if (users.GroupList[m_Process.QueryEvents["mainvalue"]] == null)
            {
                Group group = users.GroupList.Create(m_Process.QueryEvents["mainvalue"]);
                users.Save();
            }
        }

        public void HandleAddUser()
        {
            Users users = new Users(m_Process);
            if (users.UserList[m_Process.QueryEvents["mainvalue"]] == null)
            {
                User user = users.UserList.Create(m_Process.QueryEvents["mainvalue"]);
                users.Save();
            }
        }

        public void HandleSaveUser()
        {
            Users users = new Users(m_Process);
            
            User user = users.UserList[m_Process.QueryEvents["mainvalue"]];
            user.Login = m_Process.QueryData["user_login"];
            if ("emptystring" != m_Process.QueryData["user_password"])
            {
                user.Password = m_Process.QueryData["user_password"];
            }
            user.GroupList.Clear();


            string groups = m_Process.QueryData["user_groups"];
            foreach (string groupname in groups.Split(','))
            {
                user.GroupList.Create(groupname);
            }
            users.Save();
        }
        
        public void HandleDeleteUser()
        {
            Users users = new Users(m_Process);
            users.UserList.Remove(m_Process.QueryEvents["mainvalue"]);
            users.Save();
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "users":
                    LoadUsers(control, pathTrail);
                    break;
                case "groups":
                    LoadGroups(control, pathTrail);
                    break;
                case "user":
                    LoadUser(control, pathTrail);
                    break;
                case "frontpage":
                    FrontPage();
                    break;
            }
        }

        public void FrontPage()
        {
            bool redirected = false;
            object[] results = m_Process.Plugins.InvokeAll("users", "list_groups", m_Process.CurrentUser);
            List<string> userGroups = new List<string>(Common.FlattenToStrings(m_Process,results));

            foreach (string group in userGroups)
            {
                string xPath = string.Format("groups/item[@name='{0}']", group);
                XmlNode node = null;
                try
                {
                    node = m_Process.Settings.GetAsNode(xPath);
                }
                catch
                {
                    // Ignore
                }
                if (node != null)
                {
                    string frontPage = CommonXml.GetAttributeValue(node, "frontpage");
                    if (frontPage != null && frontPage != string.Empty)
                    {
                        redirected = true;
                        m_Process.HttpPage.Response.Redirect(frontPage + ".aspx");
                    }
                }
            }

            if (!redirected)
            {
                string defaultFrontPage = m_Process.Settings["groups/defaultfrontpage"];
            }
        }

        public void LoadGroups(ControlList control, string value)
        {
            InventIt.SiteSystem.Data.Users.Users users = new InventIt.SiteSystem.Data.Users.Users(m_Process);
            control["groups"].InnerXml = users.GroupList.ParentNode.InnerXml;
        }
        
        public void LoadUser(ControlList control, string value)
        {
            InventIt.SiteSystem.Data.Users.Users users = new InventIt.SiteSystem.Data.Users.Users(m_Process);
            if (value != null && users.UserList[value] != null)
            {

                control["user"].InnerXml = users.UserList[value].Node.InnerXml;
            }
        }

        public void LoadUsers(ControlList control, string value)
        {
            InventIt.SiteSystem.Data.Users.Users users = new InventIt.SiteSystem.Data.Users.Users(m_Process);
            control["users"].InnerXml = users.UserList.ParentNode.InnerXml;
        }

        public new object Invoke(string api, string action, params object[] args)
        {
            switch (api)
            {
                case "users":
                    return Users(action, args);
            }

            return null;
        }

        private object Users(string action, object[] args)
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
            if (args == null || args.Length < 2) { return null; }

            string username = args[0].ToString();
            string password = args[1].ToString();

            User user = users.UserList[username];
            if (user != null && user.CheckPassword(password))
            {
                return true;
            }
            return false;
        }

        private object UsersGroups(object[] args)
        {
            if (args == null || args.Length < 1) { return null; }

            string username = args[0].ToString();
            List<string> groups = new List<string>();

            User user = users.UserList[username];
            if (user != null) 
            {
                int groupCount = user.GroupList.Count;
                for (int i = 0; i < groupCount; i++)
                {
                    groups.Add(user.GroupList[i].Name);
                }
            }

            if (groups.Count > 0)
            {
                return groups.ToArray();
            }
            return null;
        }
    }
}
