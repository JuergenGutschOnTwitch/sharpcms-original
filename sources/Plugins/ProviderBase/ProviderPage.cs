//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using InventIt.SiteSystem.Data.SiteTree;
using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem;
using System.Xml;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderPage : BasePlugin2, IPlugin2
    {
        private SiteTree m_SiteTree;

        public SiteTree Tree
        {
            get
            {
                if (m_SiteTree == null)
                {
                    m_SiteTree = new SiteTree(m_Process);
                }
                return m_SiteTree;
            }
        }

        public new string Name
        {
            get
            {
                return "Page";
            }
        }

        public ProviderPage()
        {
        }

        public ProviderPage(Process process)
        {
            m_Process = process;
        }

        public string CurrentPage
        {
            get
            {
                if (m_Process.QueryOther["page"] == "")
                    m_Process.QueryOther["page"] = m_Process.Settings["sitetree/stdpage"];
                return m_Process.QueryOther["page"];
            }
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

        private void HandleSetStandardPage()
        {
            string newDefault = m_Process.QueryData["pageidentifier"].Trim();
            if (newDefault.Length > 0)
            {
                m_Process.Settings["sitetree/stdpage"] = newDefault;
            }
        }

        private void HandlePageRemoveContainer()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string query = m_Process.QueryEvents["mainvalue"];

            CurrentPage.Containers.Remove(int.Parse(query) - 1);
            CurrentPage.Save();
        }

        private void HandlePageCreateContainer()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string query = m_Process.QueryEvents["mainvalue"];
            query = Common.CleanToSafeString(query).ToLower();

            Container container = CurrentPage.Containers[query];
            CurrentPage.Save();
        }

        private void HandlePageMoveUp()
        {
            new SiteTree(m_Process).MoveUp(m_Process.QueryData["pageidentifier"]);
        }

        private void HandlePageMoveDown()
        {
            new SiteTree(m_Process).MoveDown(m_Process.QueryData["pageidentifier"]);
        }


        private void HandlePageCopy()
        {
            new SiteTree(m_Process).CopyTo(m_Process.QueryEvents["mainvalue"]);
        }

        private void HandlePageMove()
        {
            new SiteTree(m_Process).Move(m_Process.QueryData["pageidentifier"], m_Process.QueryEvents["mainvalue"]);
        }

        private void HandleAddElement()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] a_list = element.Split('_');
            string elementname = m_Process.QueryData["container_" + a_list[1]];
            CurrentPage.Containers[int.Parse(a_list[1]) - 1].Elements.Create(elementname);
            CurrentPage.Save();
        }

        private void HandleAddPage()
        {
            string path = m_Process.QueryEvents["mainvalue"];
            string[] pathSplit = path.Split('*');
            new SiteTree(m_Process).Create(pathSplit[0], pathSplit[1], pathSplit[1]);
        }

        private void HandleRemovePage()
        {
            string path = m_Process.QueryEvents["mainvalue"];
            new SiteTree(m_Process).Delete(path);
        }

        private void HandleSave()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            for (int i = 0; i < m_Process.QueryData.Count; i++)
            {
                Query query = m_Process.QueryData[i];
                string[] queryParts = query.Name.Split('_');

                if (queryParts.Length > 1)
                {
                    switch (queryParts[0])
                    {
                        case "attribute":
                            if (queryParts[1].EndsWith("-list"))
                            {
                                XmlNode xmlNode = CurrentPage.getAttribute(queryParts[1]);
                                xmlNode.InnerText = "";
                                foreach (string tmpstring in query.Value.Split('\n'))
                                {
                                    CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew).InnerText = tmpstring;
                                }
                            }
                            else
                            {
                                CurrentPage[queryParts[1]] = query.Value;
                            }
                            break;

                        case "element":
                            if (queryParts[3].EndsWith("-list"))
                            {
                                XmlNode xmlNode = CommonXml.GetNode(CurrentPage.Containers[int.Parse(queryParts[1]) - 1].Elements[int.Parse(queryParts[2]) - 1].Node, queryParts[3]);
                                xmlNode.InnerText = "";
                                foreach (string tmpstring in query.Value.Split('\n'))
                                {
                                    if (tmpstring != "")
                                    {
                                        XmlNode tmpNode = CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew);
                                        tmpNode.InnerText = tmpstring;
                                        CommonXml.AppendAttribute(tmpNode, "id", Common.CleanToSafeString(tmpstring));
                                    }
                                }
                            }
                            else
                            {
                                CurrentPage.Containers[int.Parse(queryParts[1]) - 1].
                                    Elements[int.Parse(queryParts[2]) - 1][queryParts[3]] = query.Value;
                            }
                            break;
                    }
                }
            }
            CurrentPage.Save();
        }

        private void HandleRemove()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] a_list = element.Split('-');
            CurrentPage.Containers[int.Parse(a_list[1]) - 1].Elements.Remove(int.Parse(a_list[2]) - 1);
            CurrentPage.Save();
        }

        private void HandleCopy()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            CurrentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.Copy(int.Parse(elementParts[2]) - 1);
            CurrentPage.Save();
        }

        private void HandleMoveTop()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] elementParts = element.Split('-');
            CurrentPage.Containers[int.Parse(elementParts[1]) - 1].Elements.MoveTop(int.Parse(elementParts[2]) - 1);
            CurrentPage.Save();
        }

        private void HandleMoveUp()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] a_list = element.Split('-');
            CurrentPage.Containers[int.Parse(a_list[1]) - 1].Elements.MoveUp(int.Parse(a_list[2]) - 1);
            CurrentPage.Save();
        }

        private void HandleMoveDown()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] a_list = element.Split('-');
            CurrentPage.Containers[int.Parse(a_list[1]) - 1].Elements.MoveDown(int.Parse(a_list[2]) - 1);
            CurrentPage.Save();
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

        private void LoadElementList(ControlList control)
        {
            control["elementlist"] = m_Process.Settings.GetAsNode("sitetree/elementlist");
        }

        private void LoadPageStatus(ControlList control)
        {
            XmlNode xmlNode = m_Process.Settings.GetAsNode("sitetree/pagestatus");
            control["pagestatus"] = xmlNode;
        }

        private void LoadPage(ControlList control, string value, string pathTrail)
        {
            LoadDay(m_Process.Content.GetSubControl("basedata"));//todo:quick hack not nice

            string pagePath = GetCurrentPage(GetFullPath(value, pathTrail));

            Page page = Tree.GetPage(pagePath);

            if (page != null)
            {
                m_Process.Attributes["pageroot"] = pagePath.Split('/')[0];
                Plugins(page);
                control["page"] = page.Node;
            }
        }

        private string GetFullPath(string value, string pathTrail)
        {
            string fullPath = string.Empty;
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
            {
                fullPath = pathTrail;
            }
            return fullPath;
        }

        private void Plugins(Page page)
        {
            for (int i = 0; i < page.Containers.Count; i++)
            {
                for (int b = 0; b < page.Containers[i].Elements.Count; b++)
                {
                    //  m_Process.Settings["sitetree/elementlist/

                    XmlNode XmlElementNode = page.Containers[i].Elements[b].Node;
                    string plugin = CommonXml.GetNode(XmlElementNode, "plugin").InnerText;
                    string action = CommonXml.GetNode(XmlElementNode, "action").InnerText;
                    if (plugin != "" & action != "")
                    {
                        string pathTrail = CommonXml.GetNode(XmlElementNode, "value").InnerText;
                        InventIt.SiteSystem.Plugin.Types.AvailablePlugin availablePlugin = m_Process.Plugins.AvailablePlugins.Find(plugin);
                        if (availablePlugin != null)
                        {
                            IPlugin2 plugin2 = availablePlugin.Instance as IPlugin2;
                            if (plugin2 != null)
                            {
                                IPlugin2 iPlugin = availablePlugin.Instance as IPlugin2;
                                //m_Process.AddMessage("IPlugin 2");
                                iPlugin.Load(new ControlList(XmlElementNode), action, string.Empty, pathTrail);
                            }
                            else
                            {
                                IPlugin iPlugin = availablePlugin.Instance;
                                iPlugin.Load(new ControlList(XmlElementNode), action, pathTrail);
                            }
                        }
                    }
                }
            }
        }

        private void LoadDay(ControlList control)
        {
            control["now/day"].InnerText = System.DateTime.Now.Day.ToString();
            control["now/month"].InnerText = System.DateTime.Now.Month.ToString();
            control["now/year"].InnerText = System.DateTime.Now.Year.ToString();
        }

        private string GetCurrentPage(string value)
        {
            string pagePath;
            if (value == null || value == "")
            {
                pagePath = m_Process.Settings["sitetree/stdpage"];
            }
            else
            {
                pagePath = value;
            }
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

        private void SetCurrentPage(XmlNode xmlNode, string[] path)
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

            string pagePath = GetCurrentPage(GetFullPath(value, pathTrail).Replace("edit/", "")); // TODO: a very dirty hack
            if (pagePath != string.Empty)
            {
                SetCurrentPage(control["sitetree"], pagePath.Split('/'));
            }
        }

        private void HandleDifferentRoot(ControlList control, ref string value, XmlNode xmlNode)
        {
            string realName = string.Empty;
            string treeRootName = string.Empty;
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
            {
                treeNode.InnerXml = copyNode.InnerXml;
            }
            CommonXml.AppendAttribute(treeNode, "realname", realName);
            control["sitetree"].AppendChild(treeNode);
        }

        private string GetTreeRootName(string value)
        {
            return value.Substring(value.LastIndexOf("/") + 1);
        }
    }
}