// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Sharpcms.Data.SiteTree;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;
using Sharpcms.Library.Users;

namespace Sharpcms.Providers.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class ProviderPage : BasePlugin2, IPlugin2
    {
        private SiteTree _siteTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderPage"/> class.
        /// </summary>
        public ProviderPage()
        {
            Process = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderPage"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public ProviderPage(Process process) : this()
        {
            Process = process;
        }

        /// <summary>
        /// Gets the tree.
        /// </summary>
        /// <value>The tree.</value>
        private SiteTree Tree
        {
            get
            {
                SiteTree siteTree = _siteTree ?? (_siteTree = new SiteTree(Process));

                return siteTree;
            }
        }

        #region IPlugin2 Members

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public new string Name
        {
            get
            {
                return "Page";
            }
        }

        /// <summary>
        /// Handles the specified main event.
        /// </summary>
        /// <param name="mainEvent">The main event.</param>
        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "remove":
                    HandleRemoveElement();
                    break;
                case "save":
                    HandleSave();
                    break;
                case "moveup":
                    HandleMoveElementUp();
                    break;
                case "copy":
                    HandleCopyElement();
                    break;
                case "movetop":
                    HandleMoveElementTop();
                    break;
                case "movedown":
                    HandleMoveElementDown();
                    break;
                case "removepage":
                    HandleRemovePage();
                    break;
                case "addpage":
                    HandleAddPage();
                    break;
                case "addelement":
                    HandleAddElement();
                    break;
                case "pagemoveup":
                    HandlePageMoveUp();
                    break;
                case "pagemovebottom":
                    HandlePageMoveBottom();
                    break;
                case "pagemovetop":
                    HandlePageMoveTop();
                    break;
                case "pagemovedown":
                    HandlePageMoveDown();
                    break;
                case "pagecreatcontainer":
                    HandlePageCreateContainer();
                    break;
                case "pageremovecontainer":
                    HandlePageRemoveContainer();
                    break;
                case "pagecopyto":
                    HandlePageCopy();
                    break;
                case "pagemove":
                    HandlePageMove();
                    break;
                case "setstandardpage":
                    HandleSetStandardPage();
                    break;
            }
        }

        /// <summary>
        /// Loads the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "tree":
                    LoadTree(control, value, pathTrail);
                    break;
                case "page":
                    LoadPage(control, value, pathTrail);
                    break;
                case "elementlist":
                    LoadElementList(control);
                    break;
                case "pagestatus":
                    LoadPageStatus(control);
                    break;
                case "security":
                    LoadPageSecurity(control);
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Handles the set standard page.
        /// </summary>
        private void HandleSetStandardPage()
        {
            string newDefault = Process.QueryData["pageidentifier"].Trim();
            if (newDefault.Length > 0)
            {
                Process.Settings["sitetree/stdpage"] = newDefault;
            }
        }

        /// <summary>
        /// Handles the page remove container.
        /// </summary>
        private void HandlePageRemoveContainer()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string mainValueQueryEvent = Process.QueryEvents["mainvalue"];
            
            currentPage.Containers.Remove(int.Parse(mainValueQueryEvent) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the page create container.
        /// </summary>
        private void HandlePageCreateContainer()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            
            string query = Process.QueryEvents["mainvalue"];
            query = Common.CleanToSafeString(query).ToLower();

            Container container = currentPage.Containers[query];
            currentPage.Save();
        }

        /// <summary>
        /// Handles the page move up.
        /// </summary>
        private void HandlePageMoveUp()
        {
            new SiteTree(Process).MoveUp(Process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page move down.
        /// </summary>
        private void HandlePageMoveDown()
        {
            new SiteTree(Process).MoveDown(Process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page move top.
        /// </summary>
        private void HandlePageMoveTop()
        {
            new SiteTree(Process).MoveTop(Process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page move bottom.
        /// </summary>
        private void HandlePageMoveBottom()
        {
            new SiteTree(Process).MoveBottom(Process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page copy.
        /// </summary>
        private void HandlePageCopy()
        {
            string mainValueQueryEvent = Process.QueryEvents["mainvalue"];
            
            SiteTree siteTree = new SiteTree(Process);
            siteTree.CopyTo(mainValueQueryEvent);
        }

        /// <summary>
        /// Handles the page move.
        /// </summary>
        private void HandlePageMove()
        {
            string newParentPath = Process.QueryEvents["mainvalue"];
            string path = Process.QueryData["pageidentifier"];
            
            SiteTree siteTree = new SiteTree(Process);
            siteTree.Move(path, newParentPath);
        }

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string GetRedirectUrl(string path)
        {
            return Process.GetUrl(String.Format("admin/page/edit/{0}/", path));
        }
        
        /// <summary>
        /// Handles the add element.
        /// </summary>
        private void HandleAddElement()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string query = Process.QueryEvents["mainvalue"];
            string[] elementParts = query.Split('_');
            string elementType = elementParts[0];
            int containerId = int.Parse(elementParts[1]);

            currentPage.Containers[containerId - 1].Elements.Create(elementType, String.Empty, false);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the add page.
        /// </summary>
        private void HandleAddPage()
        {
            string mainvalue = Process.QueryEvents["mainvalue"];
            string[] pathSplit = mainvalue.Split('*');
            string path = pathSplit[0];
            string pageName = pathSplit[1];
            
            SiteTree siteTree = new SiteTree(Process);
            if (!siteTree.Exists(path + "/" + pageName))
            {
                siteTree.Create(path, pageName, pageName);
            }
            else
            {
                Process.AddMessage("A page with that name already exists.");
            }
        }

        /// <summary>
        /// Handles the remove page.
        /// </summary>
        private void HandleRemovePage()
        {
            string path = Process.QueryEvents["mainvalue"];

            new SiteTree(Process).Delete(path);
        }

        /// <summary>
        /// Handles the save.
        /// </summary>
        private void HandleSave()
        {
            string elementName = String.Empty;
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);

            for (int i = 0; i < Process.QueryData.Count; i++)
            {
                Query query = Process.QueryData[i];
                string[] queryParts = query.Name.Split('_');

                if (queryParts.Length > 1)
                {
                    switch (queryParts[0])
                    {
                        case "attribute":
                            if (queryParts[1].EndsWith("-list"))
                            {
                                XmlNode xmlNode = currentPage.GetAttribute(queryParts[1]);
                                xmlNode.InnerText = string.Empty;
                                foreach (string tmpstring in query.Value.Split('\n'))
                                {
                                    CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew).InnerText = tmpstring;
                                }
                            }
                            else
                            {
                                currentPage[queryParts[1]] = query.Value;
                            }
                            break;
                        case "element":
                            Container container = currentPage.Containers[int.Parse(queryParts[1]) - 1];

                            if (queryParts[3].EndsWith("-list"))
                            {
                                XmlNode xmlNode = CommonXml.GetNode(container.Elements[int.Parse(queryParts[2]) - 1].Node, queryParts[3]);
                                xmlNode.InnerText = String.Empty;
                                foreach (string tmpstring in query.Value.Split('\n'))
                                {
                                    if (tmpstring != String.Empty)
                                    {
                                        XmlNode tmpNode = CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew);
                                        tmpNode.InnerText = tmpstring;
                                        CommonXml.AppendAttribute(tmpNode, "id", Common.CleanToSafeString(tmpstring));
                                    }
                                }
                            }
                            else
                            {
                                if (elementName != query.Name.Substring(0, query.Name.LastIndexOf('_')))
                                {
                                    elementName = query.Name.Substring(0, query.Name.LastIndexOf('_'));
                                    container.Elements[int.Parse(queryParts[2]) - 1].Publish = false.ToString(CultureInfo.InvariantCulture).ToLower();
                                }

                                if (query.Name.EndsWith("elementtitle"))
                                {
                                    container.Elements[int.Parse(queryParts[2]) - 1].Name = query.Value;
                                }
                                else if (query.Name.EndsWith("elementpublish"))
                                {
                                    container.Elements[int.Parse(queryParts[2]) - 1].Publish = query.Value.ToLower() == "publish" 
                                        ? true.ToString(CultureInfo.InvariantCulture).ToLower()
                                        : false.ToString(CultureInfo.InvariantCulture).ToLower();
                                }
                                else
                                {
                                    container.Elements[int.Parse(queryParts[2]) - 1][queryParts[3]] = query.Value;
                                }
                            }
                            break;
                    }
                }
            }

            currentPage.Save();
        }

        /// <summary>
        /// Handles the remove.
        /// </summary>
        private void HandleRemoveElement()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string element = Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');

            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Remove(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the copy.
        /// </summary>
        private void HandleCopyElement()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string element = Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');

            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Copy(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the move top.
        /// </summary>
        private void HandleMoveElementTop()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string element = Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');

            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveTop(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the move up.
        /// </summary>
        private void HandleMoveElementUp()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string element = Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');

            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveUp(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the move down.
        /// </summary>
        private void HandleMoveElementDown()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string element = Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');

            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveDown(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Loads the element list.
        /// </summary>
        /// <param name="control">The control.</param>
        private void LoadElementList(ControlList control)
        {
            control["elementlist"] = Process.Settings.GetAsNode("sitetree/elementlist");
        }

        /// <summary>
        /// Loads the page status.
        /// </summary>
        /// <param name="control">The control.</param>
        private void LoadPageStatus(ControlList control)
        {
            control["pagestatus"] = Process.Settings.GetAsNode("sitetree/pagestatus");
        }

        /// <summary>
        /// Loads the page security.
        /// </summary>
        /// <param name="control">The control.</param>
        private void LoadPageSecurity(ControlList control)
        {
            Users users = new Users(Process);
            List<User> userList = new List<User>();
            List<Group> groupList = new List<Group>();
            for (int i = 0; i < users.UserList.Count; i++)
            {
                User user = users.UserList[i];
                if (!userList.Contains(user))
                {
                    userList.Add(user);
                }
            }

            for (int i = 0; i < users.GroupList.Count; i++)
            {
                Group group = users.GroupList[i];
                if (!groupList.Contains(group))
                {
                    groupList.Add(group);
                }
            }
            
            XmlNode elementSecurity = Process.XmlData.CreateElement("security");

            XmlNode elementUsers = Process.XmlData.CreateElement("users");
            foreach (User user in userList)
            {
                XmlNode xuser = Process.XmlData.CreateElement("user");
                xuser.AppendChild(Process.XmlData.CreateTextNode(user.Login));
                elementUsers.AppendChild(xuser);
            }

            XmlNode elementGroups = Process.XmlData.CreateElement("groups");
            foreach (Group group in groupList)
            {
                XmlNode elementGroup = Process.XmlData.CreateElement("group");
                elementGroup.AppendChild(Process.XmlData.CreateTextNode(group.Name));
                elementGroups.AppendChild(elementGroup);
            }

            elementSecurity.AppendChild(elementUsers);
            elementSecurity.AppendChild(elementGroups);

            control["security"] = elementSecurity;
        }

        /// <summary>
        /// Loads the page.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
        private void LoadPage(ControlList control, string value, string pathTrail)
        {
            LoadDay(Process.Content.GetSubControl("basedata")); //ToDo: quick hack not nice

            string pagePath = GetCurrentPage(GetFullPath(value, pathTrail));
            Page page = Tree.GetPage(pagePath);

            if (page == null) return;

            Process.Attributes["pageroot"] = pagePath.Split('/')[0];
            Process.Attributes["pagepath"] = pagePath;
            
            Plugins(page);

            control["page"] = page.Node;

            Process.Content["templates"] = Process.Settings.GetAsNode("templates");

            if (page["template"] != string.Empty && Process.CurrentProcess.Split('/')[0].ToLower() != "admin")
            {
                Process.MainTemplate = Process.Settings["templates/" + page["template"]];
            }
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
        /// <returns></returns>
        private static string GetFullPath(string value, string pathTrail)
        {
            string fullPath;
            if (value != string.Empty)
            {
                if (value.StartsWith("!"))
                {
                    fullPath = value.Substring(1);
                }
                else
                {
                    fullPath = value;
                    if (pathTrail.Trim() != string.Empty)
                    {
                        fullPath += "/" + pathTrail;
                    }
                }
            }
            else
                fullPath = pathTrail;

            return fullPath;
        }

        /// <summary>
        /// Pluginses the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        private void Plugins(Page page)
        {
            for (int i = 0; i < page.Containers.Count; i++)
            {
                for (int b = 0; b < page.Containers[i].Elements.Count; b++)
                {
                    XmlNode xmlElementNode = page.Containers[i].Elements[b].Node;
                    string plugin = CommonXml.GetNode(xmlElementNode, "plugin").InnerText;
                    string action = CommonXml.GetNode(xmlElementNode, "action").InnerText;
                    if (plugin != string.Empty & action != string.Empty)
                    {
                        string pathTrail = CommonXml.GetNode(xmlElementNode, "value").InnerText;
                        AvailablePlugin availablePlugin = Process.Plugins.AvailablePlugins.Find(plugin);
                        if (availablePlugin != null)
                        {
                            IPlugin2 plugin2 = availablePlugin.Instance as IPlugin2;
                            if (plugin2 != null)
                            {
                                IPlugin2 iPlugin = availablePlugin.Instance as IPlugin2;

                                iPlugin.Load(new ControlList(xmlElementNode), action, string.Empty, pathTrail);
                            }
                            else
                            {
                                IPlugin iPlugin = availablePlugin.Instance;

                                iPlugin.Load(new ControlList(xmlElementNode), action, pathTrail);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the day.
        /// </summary>
        /// <param name="control">The control.</param>
        private static void LoadDay(ControlList control)
        {
            control["now/day"].InnerText = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);
            control["now/month"].InnerText = DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);
            control["now/year"].InnerText = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string GetCurrentPage(string value)
        {
            string pagePath = string.IsNullOrEmpty(value) ? Process.Settings["sitetree/stdpage"] : value;
            string[] args = pagePath.Split('/');

            while (args != null && !Tree.Exists(pagePath))
            {
                args = Common.RemoveOneLast(args);
                if (args != null)
                {
                    pagePath = string.Join("/", args);
                }
            }

            return pagePath;
        }

        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="pathArray">The path.</param>
        private static void SetCurrentPage(XmlNode xmlNode, string[] pathArray)
        {
            try
            {
                XmlNode currentNode = xmlNode;
                for (int i = 0; i < pathArray.Length; i++)
                {
                    string path = pathArray[i];
                    currentNode = CommonXml.GetNode(currentNode, path, EmptyNodeHandling.Ignore);
                    CommonXml.SetAttributeValue(currentNode, "inpath", "true");
                    if (i + 1 == pathArray.Length)
                    {
                        CommonXml.SetAttributeValue(currentNode, "currentpage", "true");
                    }
                }
            }
            catch
            {
                // Ignore
            }
        }

        /// <summary>
        /// Loads the tree.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
        private void LoadTree(ControlList control, string value, string pathTrail)
        {
            XmlNode xmlNode = Tree.TreeDocument.DocumentElement;

            if (value != string.Empty)
            {
                HandleDifferentRoot(control, ref value, xmlNode);
            }
            else
            {
                control["sitetree"] = xmlNode;
            }

            string pagePath = GetCurrentPage(GetFullPath(value, pathTrail).Replace("edit/", string.Empty));

            // ToDo: a very dirty hack
            if (pagePath != string.Empty)
            {
                SetCurrentPage(control["sitetree"], pagePath.Split('/'));
            }
        }

        /// <summary>
        /// Handles the different root.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        /// <param name="xmlNode">The XML node.</param>
        private static void HandleDifferentRoot(ControlList control, ref string value, XmlNode xmlNode)
        {
            string realName;
            string treeRootName;
            if (value.IndexOf("|", StringComparison.Ordinal) > 0)
            {
                treeRootName = value.Substring(0, value.IndexOf("|", StringComparison.Ordinal));
                value = value.Substring(value.IndexOf("|", StringComparison.Ordinal) + 1);
                realName = GetTreeRootName(value);
            }
            else
            {
                treeRootName = GetTreeRootName(value);
                realName = treeRootName;
            }

            XmlDocument ownerDocument = control["sitetree"].OwnerDocument;
            if (ownerDocument != null)
            {
                XmlNode treeNode = ownerDocument.CreateElement(treeRootName);
                XmlNode copyNode = xmlNode.SelectSingleNode(value);
                if (copyNode != null)
                {
                    treeNode.InnerXml = copyNode.InnerXml;
                }

                CommonXml.AppendAttribute(treeNode, "realname", realName);
                control["sitetree"].AppendChild(treeNode);
            }
        }

        /// <summary>
        /// Gets the name of the tree root.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GetTreeRootName(string value)
        {
            string treeRootName = value.Substring(value.LastIndexOf("/", StringComparison.Ordinal) + 1);

            return treeRootName;
        }
    }
}