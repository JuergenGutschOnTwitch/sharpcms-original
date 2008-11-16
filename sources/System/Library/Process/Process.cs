//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Web.UI;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Data.Users;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem
{
    public class Process
    {
        public const string COOKIE_SEPARATOR = "cookieseparator";

        public System.Web.UI.Page HttpPage;

        //xml data
        public PluginServices Plugins;
        public ControlList Content;
        public XmlItemList QueryData;
        public XmlItemList Attributes;
        public XmlItemList QueryEvents;
        public XmlItemList QueryOther;

        public bool OutputHandledByModule = false;
        public string mainTemplate; //TODO: this should be more logical 

        private Dictionary<string, string> m_Variables;
        private Cache m_Cache;
        private XmlDocument m_XmlData;
        private Settings m_Settings;
        private string m_BasePath;

        private string m_currentProcess = string.Empty;
        private string m_redirectUrl = null;

        public Dictionary<string, string> Variables
        {
            get
            {
                if (m_Variables == null)
                    m_Variables = new Dictionary<string, string>();
                return m_Variables;
            }
        }

        public string RedirectUrl
        {
            get { return m_redirectUrl; }
            set { m_redirectUrl = value; }
        }


        public bool DebugEnabled
        {
            get { return (HttpPage.Session["enabledebug"] != null && HttpPage.Session["enabledebug"].ToString() == "true"); }
            set { HttpPage.Session["enabledebug"] = value ? "true" : "false"; }
        }

        public string BasePath
        {
            get { return m_BasePath; }
        }

        public Cache Cache
        {
            get
            {
                if (m_Cache == null)
                    m_Cache = new Cache(HttpPage.Application);
                return m_Cache;
            }
        }

        public XmlDocument XmlData
        {
            get { return m_XmlData; }
            set { m_XmlData = value; }
        }

        public string CurrentProcess
        {
            get
            {
                if (m_currentProcess == string.Empty)
                {
                    string tmpProcess = QueryOther["process"];
                    if (tmpProcess != string.Empty)
                        m_currentProcess = tmpProcess;
                    else
                        m_currentProcess = Settings["general/stdprocess"];
                }
                return m_currentProcess;
            }
            set { m_currentProcess = value; }
        }

        public string Root
        {
            get { return HttpPage.Server.MapPath("."); }
        }

        public Settings Settings
        {
            get
            {
                if (m_Settings == null)
                    m_Settings = new Settings(this, Root);
                return m_Settings;
            }
        }

        // >>>>> Search Mod by Kiho Chang 2008-10-05
        public object SearchContext
        {
            get { return HttpPage.Session["SearchContext"]; }
            set { HttpPage.Session["SearchContext"] = value; }
        }
        // <<<<< Search Mod by Kiho Chang 2008-10-05

        public void AddMessage(string message, MessageType messageType, string type)
        {
            XmlNode xmlNode = CommonXml.GetNode(XmlData.DocumentElement, "messages", EmptyNodeHandling.CreateNew);
            xmlNode = CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew);
            xmlNode.InnerText = message;

            CommonXml.SetAttributeValue(xmlNode, "messagetype", messageType.ToString());
            CommonXml.SetAttributeValue(xmlNode, "type", type);
        }

        public void AddMessage(string message)
        {
            AddMessage(message, MessageType.Message);
        }

        public void AddMessage(string message, MessageType messageType)
        {
            AddMessage(message, messageType, "");
        }

        public void AddMessage(Exception e)
        {
            AddMessage(e.Message, MessageType.Error, e.GetType().ToString());
            IPlugin plugin = Plugins["ErrorLog"];
            if (plugin != null)
            {
                plugin.Handle("log");
            }
        }

        public void DebugMessage(object message)
        {
            DebugMessage(message.ToString());
        }

        public void DebugMessage(string message)
        {
            if (DebugEnabled)
                AddMessage(message, MessageType.Debug);
        }

        public Process(System.Web.UI.Page httpPage, PluginServices pluginServices)
        {
            Plugins = pluginServices;
            HttpPage = httpPage;
            XmlData = new XmlDocument();

            Plugins.FindPlugins(this, Common.CombinePaths(Root, "Bin"));

            XmlNode xmlNode = XmlData.CreateElement("data");
            XmlData.AppendChild(xmlNode);

            Content = new ControlList(xmlNode);

            if (httpPage.Request.ServerVariables["SERVER_PORT"] == "80")
                m_BasePath = httpPage.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower() + "://" + httpPage.Request.ServerVariables["SERVER_NAME"] + httpPage.Request.ApplicationPath.TrimEnd('/') + "";
            else
                m_BasePath = httpPage.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower() + "://" + httpPage.Request.ServerVariables["SERVER_NAME"] + ":" + httpPage.Request.ServerVariables["SERVER_PORT"] + httpPage.Request.ApplicationPath.TrimEnd('/') + "";

            Content["basepath"].InnerText = m_BasePath;
            Content["referrer"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["HTTP_REFERER"]);
            Content["domain"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["SERVER_NAME"]);
            Content["useragent"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["HTTP_USER_AGENT"]);
            Content["sessionid"].InnerText = httpPage.Server.UrlEncode(httpPage.Session.LCID.ToString());
            Content["ip"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["REMOTE_ADDR"]);

            QueryData = new XmlItemList(CommonXml.GetNode(xmlNode, "query/data", EmptyNodeHandling.CreateNew));
            Attributes = new XmlItemList(CommonXml.GetNode(xmlNode, "attributes", EmptyNodeHandling.CreateNew));
            QueryEvents = new XmlItemList(CommonXml.GetNode(xmlNode, "query/events", EmptyNodeHandling.CreateNew));
            QueryOther = new XmlItemList(CommonXml.GetNode(xmlNode, "query/other", EmptyNodeHandling.CreateNew));

            processQueries();

            ConfigureDebugging();
            LoginByCookie();
            if (this.QueryEvents["main"] == "login")
            {
                if (!Login(QueryData["login"], QueryData["password"]))
                {
                    if (m_Settings["messages/loginerror"] != string.Empty)
                        httpPage.Response.Redirect(BasePath + "/login.aspx?error=" + httpPage.Server.UrlEncode(m_Settings["messages/loginerror"]) + "&redirect=" + QueryOther["process"]);
                    else
                        httpPage.Response.Redirect(BasePath + "/login.aspx?" + "redirect=" + QueryOther["process"]);
                }
            }
            else if (this.QueryEvents["main"] == "logout")
            {
                Logout();
                if (this.QueryEvents["mainValue"] != string.Empty)
                    HttpPage.Response.Redirect(this.QueryEvents["mainValue"]);
            }
            UpdateCookieTimeout();

            LoadBaseData();
            // loads new user...
        }

        private void LoadBaseData()
        {
            XmlNode userNode = this.Content.GetSubControl("basedata")["currentuser"];
            CommonXml.GetNode(userNode, "username").InnerText = this.CurrentUser;
            XmlNode groupNode = CommonXml.GetNode(userNode, "groups");
            object[] resultsGroups = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            List<string> userGroups = new List<string>(Common.FlattenToStrings(this, resultsGroups));

            foreach (string group in userGroups)
                CommonXml.GetNode(groupNode, "group", EmptyNodeHandling.ForceCreateNew).InnerText = group;

            ControlList baseData = this.Content.GetSubControl("basedata");

            baseData["pageviewcount"].InnerText = PageViewCount().ToString();

            foreach (string pageInHistory in History())
            {
                XmlNode historyNode = baseData["history"].OwnerDocument.CreateElement("item");
                historyNode.InnerText = pageInHistory;
                baseData["history"].AppendChild(historyNode);
            }
        }

        private void ConfigureDebugging()
        {
            if (HttpPage.Request.ServerVariables["REMOTE_ADDR"] == "127.0.0.1")
            {
                if (HttpPage.Session["enabledebug"] == null)
                    DebugEnabled = true;

                if (QueryOther["enabledebug"] == "true" || QueryOther["enabledebug"] == "false")
                    DebugEnabled = (QueryOther["enabledebug"] == "true");

            }
        }

        private void processQueries()
        {
            List<string> keys = new List<string>();
            foreach (string key in HttpPage.Request.Form)
                keys.Add(key);

            foreach (string key in HttpPage.Request.QueryString)
                keys.Add(key);

            foreach (string key in keys)
            {
                if (key != null)
                {
                    string[] keyParts = key.Split('_');
                    string value = HttpPage.Request[key];

                    switch (keyParts[0])
                    {
                        case "data":
                            QueryData[string.Join("_", Common.RemoveOne(keyParts))] = value;
                            break;

                        case "event":
                            QueryEvents[string.Join("_", Common.RemoveOne(keyParts))] = value;
                            break;

                        default:
                            QueryOther[key] = value;
                            break;
                    }
                }
            }
        }

        public string GetUrl(string process)
        {
            string url = string.Format("{0}/{1}.aspx", BasePath, process);
            return url;
        }

        public List<string> History()
        {
            List<string> history = null;

            if (HttpPage.Session["history"] != null)
                history = (List<string>)HttpPage.Session["history"];
            else
                history = new List<string>();

            history.Add(CurrentProcess);

            HttpPage.Session["history"] = history;

            return history;
        }

        public int PageViewCount()
        {
            Dictionary<string, int> pageViewCounts = null;

            if (HttpPage.Session["pageviews"] != null)
            {
                pageViewCounts = (Dictionary<string, int>)HttpPage.Session["pageviews"];
                if (pageViewCounts.ContainsKey(CurrentProcess))
                    pageViewCounts[CurrentProcess] += 1;
                else
                    pageViewCounts[CurrentProcess] = 1;

            }
            else
            {
                pageViewCounts = new Dictionary<string, int>();
                pageViewCounts[CurrentProcess] = 1;
            }

            HttpPage.Session["pageviews"] = pageViewCounts;
            return pageViewCounts[CurrentProcess];
        }

        // user login and rights part
        public bool Login(string username, string password)
        {
            Logout();
            object[] results = Plugins.InvokeAll("users", "verify", username, password);
            if (results.Length > 0)
            {
                bool verified = false;
                foreach (object result in results)
                    if ((bool)result)
                        verified = true;

                if (verified)
                {
                    HttpPage.Response.Cookies["login_cookie"].Value = string.Format("{0}{1}{2}", username, COOKIE_SEPARATOR, password);
                    HttpPage.Response.Cookies["login_cookie"].Expires = DateTime.Now.AddDays(1);
                    this.HttpPage.Session["current_username"] = username;

                    return true;
                }
            }
            return false;
        }

        public bool LoginByCookie()
        {
            if (CurrentUser == "anonymous")
            {
                if (HttpPage.Request.Cookies["login_cookie"] != null)
                {
                    string value = HttpPage.Request.Cookies["login_cookie"].Value;
                    if (value == null || !value.Contains(COOKIE_SEPARATOR))
                        return false;

                    string[] valueParts = Common.SplitByString(value, COOKIE_SEPARATOR);

                    if (Login(valueParts[0], valueParts[1]))
                        return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private void UpdateCookieTimeout()
        {
            /*    if (HttpPage.Response.Cookies["login_cookie"] != null)
                {
                    HttpPage.Request.Cookies["login_cookie"].Value = HttpPage.Request.Cookies["login_cookie"].Value;
                    HttpPage.Response.Cookies["login_cookie"].Expires = DateTime.Now.AddDays(1);
                }*/
        }

        public string CurrentUser
        {
            get
            {
                if (HttpPage.Session["current_username"] == null)
                    Logout();

                return HttpPage.Session["current_username"].ToString();
            }
            set { HttpPage.Session["current_username"] = value; }
        }

        public bool CheckGroups(string groups)
        {
            object[] results = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            List<string> userGroups = new List<string>(Common.FlattenToStrings(this, results));

            if (groups != "")
            {
                string[] groupList = groups.Split(',');
                foreach (string checkRight in groupList)
                    if (userGroups.Contains(checkRight))
                        return true;
            }
            else
            {
                return true;
            }
            return false;
        }

        public void Logout()
        {
            HttpPage.Session.Clear();
            HttpPage.Session["current_username"] = "anonymous";
            HttpPage.Response.Cookies["login_cookie"].Expires = DateTime.Now.AddDays(-1);
        }
    }

    public class ControlList : InventIt.SiteSystem.Library.DataElementList
    {
        public ControlList(XmlNode parentNode)
            : base(parentNode)
        {
        }

        public XmlNode this[int index]
        {
            get
            {
                string xPath = string.Format("*[{0}]", index + 1);
                return GetNode(xPath, EmptyNodeHandling.CreateNew);
            }
        }

        public XmlNode this[string name]
        {
            get { return GetControlNode(name); }
            set { GetControlNode(name).InnerXml = value.InnerXml; }
        }

        public ControlList GetSubControl(string name)
        {
            if (name != "")
                return new ControlList(GetControlNode(name));
            return null;
        }

        private XmlNode GetControlNode(string name)
        {
            string xPath = string.Format("{0}", name);

            XmlNode node = CommonXml.GetNode(ParentNode, xPath);
            return node;
        }
    }

    public class XmlItemList : DataElementList
    {
        public XmlItemList(XmlNode parentNode)
            : base(parentNode)
        {
        }

        public Query this[int index]
        {
            get
            {
                string xPath = string.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.CreateNew);
                return new Query(xmlNode.Name, xmlNode.InnerText);
            }
            set
            {
                string xPath = string.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.Ignore);
                if (xmlNode == null)
                    return;
                xmlNode.InnerText = value.Value;
            }
        }

        public string this[string name]
        {
            get
            {
                string xPath = string.Format("{0}", name);
                return GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText;
            }
            set
            {
                string xPath = string.Format("{0}", name);
                GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText = value;
            }
        }
    }

    public class Query
    {
        public string Name;
        public string Value;

        public Query(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public enum MessageType
    {
        Error,
        Status,
        Event,
        Warning,
        Message,
        Debug
    }
}