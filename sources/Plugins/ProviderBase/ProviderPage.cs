//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Xml;
using InventIt.SiteSystem.Data.SiteTree;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Plugin.Types;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderPage : BasePlugin2, IPlugin2
    {
        private SiteTree _siteTree;

        public ProviderPage()
        {
        }

        public ProviderPage(Process process)
        {
            _process = process;
        }

        private SiteTree Tree
        {
            get
            {
                if (_siteTree == null)
                    _siteTree = new SiteTree(_process);

                return _siteTree;
            }
        }


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

        public new string Name
        {
            get { return "Page"; }
        }

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

        private void HandleSetStandardPage()
        {
            string newDefault = _process.QueryData["pageidentifier"].Trim();
            if (newDefault.Length > 0)
                _process.Settings["sitetree/stdpage"] = newDefault;
        }

        private void HandlePageRemoveContainer()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string query = _process.QueryEvents["mainvalue"];

            currentPage.Containers.Remove(int.Parse(query) - 1);
            currentPage.Save();
        }

        private void HandlePageCreateContainer()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string query = _process.QueryEvents["mainvalue"];
            query = Common.CleanToSafeString(query).ToLower();

            Container container = currentPage.Containers[query]; //ToDo: ??? (T.Huber 18.06.2009)
            currentPage.Save();
        }

        private void HandlePageMoveUp()
        {
            new SiteTree(_process).MoveUp(_process.QueryData["pageidentifier"]);
        }

        private void HandlePageMoveDown()
        {
            new SiteTree(_process).MoveDown(_process.QueryData["pageidentifier"]);
        }

        private void HandlePageMoveTop()
        {
            new SiteTree(_process).MoveTop(_process.QueryData["pageidentifier"]);
        }

        private void HandlePageMoveBottom()
        {
            new SiteTree(_process).MoveBottom(_process.QueryData["pageidentifier"]);
        }


        private void HandlePageCopy()
        {
            new SiteTree(_process).CopyTo(_process.QueryEvents["mainvalue"]);
        }

        private void HandlePageMove()
        {
            new SiteTree(_process).Move(_process.QueryData["pageidentifier"], _process.QueryEvents["mainvalue"]);
        }

        private void HandleAddElement()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('_');
            string elementname = _process.QueryData["container_" + elementParts[1]];
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Create(elementname);
            currentPage.Save();
        }

        private void HandleAddPage()
        {
            string path = _process.QueryEvents["mainvalue"];
            string[] pathSplit = path.Split('*');
            new SiteTree(_process).Create(pathSplit[0], pathSplit[1], pathSplit[1]);
        }

        private void HandleRemovePage()
        {
            string path = _process.QueryEvents["mainvalue"];
            new SiteTree(_process).Delete(path);
        }

        private void HandleSave()
        {
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
                                xmlNode.InnerText = "";
                                foreach (string tmpstring in query.Value.Split('\n'))
                                {
                                    if (tmpstring != "")
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
                                currentPage.Containers[int.Parse(queryParts[1]) - 1].Elements[
                                    int.Parse(queryParts[2]) - 1][queryParts[3]] = query.Value;
                            }
                            break;
                    }
                }
            }
            currentPage.Save();
        }

        private void HandleRemove()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            if (currentPage == null) throw new NotImplementedException();
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Remove(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        private void HandleCopy()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Copy(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        private void HandleMoveTop()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveTop(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        private void HandleMoveUp()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveUp(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        private void HandleMoveDown()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            currentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveDown(int.Parse(elementParts[2]) - 1);
            currentPage.Save();
        }

        private void LoadElementList(ControlList control)
        {
            control["elementlist"] = _process.Settings.GetAsNode("sitetree/elementlist");
        }

        private void LoadPageStatus(ControlList control)
        {
            XmlNode xmlNode = _process.Settings.GetAsNode("sitetree/pagestatus");
            control["pagestatus"] = xmlNode;
        }

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
                                //m_Process.AddMessage("IPlugin 2");
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

        private static void LoadDay(ControlList control)
        {
            control["now/day"].InnerText = DateTime.Now.Day.ToString();
            control["now/month"].InnerText = DateTime.Now.Month.ToString();
            control["now/year"].InnerText = DateTime.Now.Year.ToString();
        }

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

        private static string GetTreeRootName(string value)
        {
            return value.Substring(value.LastIndexOf("/") + 1);
        }
    }
}