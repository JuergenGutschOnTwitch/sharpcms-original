//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Xml;
using InventIt.SiteSystem.Data.SiteTree;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Plugin.Types;

namespace InventIt.SiteSystem.Providers
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderPage"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public ProviderPage(Process process)
        {
            _process = process;
        }

        /// <summary>
        /// Gets the tree.
        /// </summary>
        /// <value>The tree.</value>
        private SiteTree Tree
        {
            get
            {
                if (_siteTree == null)
                    _siteTree = new SiteTree(_process);

                return _siteTree;
            }
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public string CurrentPage //ToDo: Is a unused Property (T.Huber 18.06.2009)
        {
            get
            {
                if (_process.QueryOther["page"] == "")
                    _process.QueryOther["page"] = _process.Settings["sitetree/stdpage"];
                return _process.QueryOther["page"];
            }
        }

        #region IPlugin2 Members

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public new string Name
        {
            get { return "Page"; }
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
                    HandleRemove();
                    break;
                case "save":
                    HandleSave();
                    break;
                case "moveup":
                    HandleMoveUp();
                    break;
                case "copy":
                    HandleCopy();
                    break;
                case "movetop":
                    HandleMoveTop();
                    break;
                case "movedown":
                    HandleMoveDown();
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
            }
        }

        #endregion

        /// <summary>
        /// Handles the set standard page.
        /// </summary>
        private void HandleSetStandardPage()
        {
            string newDefault = _process.QueryData["pageidentifier"].Trim();
            if (newDefault.Length > 0)
                _process.Settings["sitetree/stdpage"] = newDefault;
        }

        /// <summary>
        /// Handles the page remove container.
        /// </summary>
        private void HandlePageRemoveContainer()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string query = _process.QueryEvents["mainvalue"];

            currentPage.Containers.Remove(int.Parse(query) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the page create container.
        /// </summary>
        private void HandlePageCreateContainer()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string query = _process.QueryEvents["mainvalue"];
            query = Common.CleanToSafeString(query).ToLower();

            Container container = currentPage.Containers[query]; //ToDo: ??? (T.Huber 18.06.2009)
            currentPage.Save();
        }

        /// <summary>
        /// Handles the page move up.
        /// </summary>
        private void HandlePageMoveUp()
        {
            new SiteTree(_process).MoveUp(_process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page move down.
        /// </summary>
        private void HandlePageMoveDown()
        {
            new SiteTree(_process).MoveDown(_process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page move top.
        /// </summary>
        private void HandlePageMoveTop()
        {
            new SiteTree(_process).MoveTop(_process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page move bottom.
        /// </summary>
        private void HandlePageMoveBottom()
        {
            new SiteTree(_process).MoveBottom(_process.QueryData["pageidentifier"]);
        }

        /// <summary>
        /// Handles the page copy.
        /// </summary>
        private void HandlePageCopy()
        {
            new SiteTree(_process).CopyTo(_process.QueryEvents["mainvalue"]);
        }

        /// <summary>
        /// Handles the page move.
        /// </summary>
        private void HandlePageMove()
        {
            new SiteTree(_process).Move(_process.QueryData["pageidentifier"], _process.QueryEvents["mainvalue"]);
        }

        /// <summary>
        /// Handles the add element.
        /// </summary>
        private void HandleAddElement()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('_');
            string elementType = _process.QueryData["container_" + elementParts[1]];
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Create(elementType, String.Empty, true);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the add page.
        /// </summary>
        private void HandleAddPage()
        {
            string path = _process.QueryEvents["mainvalue"];
            string[] pathSplit = path.Split('*');
            new SiteTree(_process).Create(pathSplit[0], pathSplit[1], pathSplit[1]);
        }

        /// <summary>
        /// Handles the remove page.
        /// </summary>
        private void HandleRemovePage()
        {
            string path = _process.QueryEvents["mainvalue"];
            new SiteTree(_process).Delete(path);
        }

        /// <summary>
        /// Handles the save.
        /// </summary>
        private void HandleSave()
        {
            bool isPublishReset = false;
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);

            for (int i = 0; i < _process.QueryData.Count; i++)
            {
                Query query = _process.QueryData[i];
                string[] queryParts = query.Name.Split('_');

                if (queryParts.Length > 1)
                {
                    switch (queryParts[0])
                    {
                        case "attribute":
                            if (queryParts[1].EndsWith("-list"))
                            {
                                XmlNode xmlNode = currentPage.GetAttribute(queryParts[1]);
                                xmlNode.InnerText = "";
                                foreach (string tmpstring in query.Value.Split('\n'))
                                    CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew).InnerText =
                                        tmpstring;
                            }
                            else
                                currentPage[queryParts[1]] = query.Value;
                            break;
                        case "element":
                            if (queryParts[3].EndsWith("-list"))
                            {
                                XmlNode xmlNode =
                                    CommonXml.GetNode(
                                        currentPage.Containers[int.Parse(queryParts[1]) - 1].Elements[
                                            int.Parse(queryParts[2]) - 1].Node, queryParts[3]);
                                xmlNode.InnerText = String.Empty;
                                foreach (string tmpstring in query.Value.Split('\n'))
                                {
                                    if (tmpstring != String.Empty)
                                    {
                                        XmlNode tmpNode = CommonXml.GetNode(xmlNode, "item",
                                                                            EmptyNodeHandling.ForceCreateNew);
                                        tmpNode.InnerText = tmpstring;
                                        CommonXml.AppendAttribute(tmpNode, "id", Common.CleanToSafeString(tmpstring));
                                    }
                                }
                            }
                            else
                            {
                                Container container = currentPage.Containers[int.Parse(queryParts[1]) - 1];

                                if (!isPublishReset)
                                {
                                    container.Elements[int.Parse(queryParts[2]) - 1].Publish =
                                        false.ToString().ToLower();
                                    isPublishReset = true;
                                }

                                if (query.Name.EndsWith("elementtitle"))
                                    container.Elements[int.Parse(queryParts[2]) - 1].Name = query.Value;
                                else if (query.Name.EndsWith("elementpublish"))
                                    container.Elements[int.Parse(queryParts[2]) - 1].Publish = query.Value.ToLower() ==
                                                                                               "publish"
                                                                                                   ? true.ToString().
                                                                                                         ToLower()
                                                                                                   : false.ToString().
                                                                                                         ToLower();
                                else
                                    container.Elements[int.Parse(queryParts[2]) - 1][queryParts[3]] = query.Value;
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
        private void HandleRemove()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            if (currentPage == null) throw new NotImplementedException();
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Remove(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the copy.
        /// </summary>
        private void HandleCopy()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Copy(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the move top.
        /// </summary>
        private void HandleMoveTop()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveTop(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the move up.
        /// </summary>
        private void HandleMoveUp()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveUp(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        /// <summary>
        /// Handles the move down.
        /// </summary>
        private void HandleMoveDown()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
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
            control["elementlist"] = _process.Settings.GetAsNode("sitetree/elementlist");
        }

        /// <summary>
        /// Loads the page status.
        /// </summary>
        /// <param name="control">The control.</param>
        private void LoadPageStatus(ControlList control)
        {
            XmlNode xmlNode = _process.Settings.GetAsNode("sitetree/pagestatus");
            control["pagestatus"] = xmlNode;
        }

        /// <summary>
        /// Loads the page.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
        private void LoadPage(ControlList control, string value, string pathTrail)
        {
            LoadDay(_process.Content.GetSubControl("basedata")); //ToDo: quick hack not nice (old)

            string pagePath = GetCurrentPage(GetFullPath(value, pathTrail));

            Page page = Tree.GetPage(pagePath);

            if (page == null) return;

            _process.Attributes["pageroot"] = pagePath.Split('/')[0];
            Plugins(page);
            control["page"] = page.Node;
            _process.Content["templates"] = _process.Settings.GetAsNode("templates");
            if (page["template"] != "" && _process.CurrentProcess.Split('/')[0].ToLower() != "admin")
                _process.MainTemplate = _process.Settings["templates/" + page["template"]];
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
                    fullPath = value.Substring(1);
                else
                {
                    fullPath = value;
                    if (pathTrail.Trim() != string.Empty)
                        fullPath += "/" + pathTrail;
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
                    //  m_Process.Settings["sitetree/elementlist/

                    XmlNode xmlElementNode = page.Containers[i].Elements[b].Node;
                    string plugin = CommonXml.GetNode(xmlElementNode, "plugin").InnerText;
                    string action = CommonXml.GetNode(xmlElementNode, "action").InnerText;
                    if (plugin != "" & action != "")
                    {
                        string pathTrail = CommonXml.GetNode(xmlElementNode, "value").InnerText;
                        AvailablePlugin availablePlugin = _process.Plugins.AvailablePlugins.Find(plugin);
                        if (availablePlugin != null)
                        {
                            var plugin2 = availablePlugin.Instance as IPlugin2;
                            if (plugin2 != null)
                            {
                                var iPlugin = availablePlugin.Instance as IPlugin2;
                                //_process.AddMessage("IPlugin 2");
                                if (iPlugin != null)
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
            control["now/day"].InnerText = DateTime.Now.Day.ToString();
            control["now/month"].InnerText = DateTime.Now.Month.ToString();
            control["now/year"].InnerText = DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string GetCurrentPage(string value)
        {
            string pagePath = string.IsNullOrEmpty(value) ? _process.Settings["sitetree/stdpage"] : value;
            string[] args = pagePath.Split('/');

            while (args != null && !Tree.Exists(pagePath))
            {
                args = Common.RemoveOneLast(args);
                if (args != null)
                    pagePath = string.Join("/", args);
            }
            return pagePath;
        }

        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="path">The path.</param>
        private static void SetCurrentPage(XmlNode xmlNode, string[] path)
        {
            try
            {
                XmlNode currentNode = xmlNode;
                for (int i = 0; i < path.Length; i++)
                {
                    string str = path[i];
                    currentNode = CommonXml.GetNode(currentNode, str, EmptyNodeHandling.Ignore);
                    CommonXml.SetAttributeValue(currentNode, "inpath", "true");
                    if (i + 1 == path.Length)
                        CommonXml.SetAttributeValue(currentNode, "currentpage", "true");
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
                HandleDifferentRoot(control, ref value, xmlNode);
            else
                control["sitetree"] = xmlNode;

            string pagePath = GetCurrentPage(GetFullPath(value, pathTrail).Replace("edit/", ""));

            // ToDo: a very dirty hack (old)
            if (pagePath != string.Empty)
                SetCurrentPage(control["sitetree"], pagePath.Split('/'));
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
            if (value.IndexOf("|") > 0)
            {
                treeRootName = value.Substring(0, value.IndexOf("|"));
                value = value.Substring(value.IndexOf("|") + 1);
                realName = GetTreeRootName(value);
            }
            else
            {
                treeRootName = GetTreeRootName(value);
                realName = treeRootName;
            }

            XmlNode treeNode = control["sitetree"].OwnerDocument.CreateElement(treeRootName);
            XmlNode copyNode = xmlNode.SelectSingleNode(value);
            if (copyNode != null)
                treeNode.InnerXml = copyNode.InnerXml;

            CommonXml.AppendAttribute(treeNode, "realname", realName);
            control["sitetree"].AppendChild(treeNode);
        }

        /// <summary>
        /// Gets the name of the tree root.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GetTreeRootName(string value)
        {
            return value.Substring(value.LastIndexOf("/") + 1);
        }
    }
}