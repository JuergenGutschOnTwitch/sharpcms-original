// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;

namespace Sharpcms.Library.Process
{
    public class Process
    {
        private const string CookieSeparator = "cookieseparator";
        private readonly string _basePath;
        public readonly Page HttpPage;
        public readonly PluginServices Plugins;
        public readonly ControlList Content;
        public readonly XmlItemList Attributes;
        public readonly XmlItemList QueryData;
        public readonly XmlItemList QueryEvents;
        public readonly XmlItemList QueryOther;
        private Cache _cache;
        private string _currentProcess = string.Empty;
        private Settings _settings;
        private Dictionary<string, string> _variables;
        public string MainTemplate; //ToDo: this should be more logical
        public bool OutputHandledByModule;

        public Process(Page httpPage, PluginServices pluginServices)
        {
            Plugins = pluginServices;
            HttpPage = httpPage;
            XmlData = new XmlDocument();

            Plugins.FindPlugins(this, Common.Common.CombinePaths(Root, "Bin"));

            XmlNode xmlNode = XmlData.CreateElement("data");
            XmlData.AppendChild(xmlNode);

            Content = new ControlList(xmlNode);

            if (httpPage.Request.ApplicationPath != null)
            {
                if (httpPage.Request.ServerVariables["SERVER_PORT"] == "80")
                {
                    _basePath = httpPage.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower() + "://" +
                                    httpPage.Request.ServerVariables["SERVER_NAME"] +
                                    httpPage.Request.ApplicationPath.TrimEnd('/') + string.Empty;
                }
                else
                {
                    _basePath = httpPage.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower() + "://" +
                                httpPage.Request.ServerVariables["SERVER_NAME"] + ":" +
                                httpPage.Request.ServerVariables["SERVER_PORT"] +
                                httpPage.Request.ApplicationPath.TrimEnd('/') + string.Empty;
                }
            }

            Content["basepath"].InnerText = _basePath;
            Content["referrer"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["HTTP_REFERER"]);
            Content["domain"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["SERVER_NAME"]);
            Content["useragent"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["HTTP_USER_AGENT"]);
            Content["sessionid"].InnerText = httpPage.Server.UrlEncode(httpPage.Session.LCID.ToString(CultureInfo.InvariantCulture));
            Content["ip"].InnerText = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["REMOTE_ADDR"]);

            Attributes = new XmlItemList(CommonXml.GetNode(xmlNode, "attributes", EmptyNodeHandling.CreateNew));
            QueryData = new XmlItemList(CommonXml.GetNode(xmlNode, "query/data", EmptyNodeHandling.CreateNew));
            QueryEvents = new XmlItemList(CommonXml.GetNode(xmlNode, "query/events", EmptyNodeHandling.CreateNew));
            QueryOther = new XmlItemList(CommonXml.GetNode(xmlNode, "query/other", EmptyNodeHandling.CreateNew));

            ProcessQueries();
            ConfigureDebugging();
            LoginByCookie();
            
            if (QueryEvents["main"] == "login")
            {
                if (!Login(QueryData["login"], QueryData["password"]))
                {
                    if (_settings != null && _settings["messages/loginerror"] != string.Empty)
                    {
                        httpPage.Response.Redirect(GetErrorUrl(httpPage.Server.UrlEncode(_settings["messages/loginerror"])));
                    }
                    else
                    {
                        httpPage.Response.Redirect(GetRedirectUrl());
                    }
                }
            }
            else if (QueryEvents["main"] == "logout")
            {
                Logout();
                if (QueryEvents["mainValue"] != string.Empty)
                {
                    HttpPage.Response.Redirect(QueryEvents["mainValue"]);
                }
            }
            UpdateCookieTimeout();

            LoadBaseData();
            // loads new user...
        }

        public Dictionary<string, string> Variables
        {
            get
            {
                return _variables ?? (_variables = new Dictionary<string, string>());
            }
        }

        public string RedirectUrl { get; set; }

        private string GetRedirectUrl()
        {
            string redirectUrl = QueryOther["process"];

            if (redirectUrl.Trim().EndsWith("/"))
            {
                redirectUrl = redirectUrl.Remove(redirectUrl.Length - 1, 1);
            }

            return string.Format("{0}login/?redirect={1}", BasePath, redirectUrl);
        }

        private string GetErrorUrl(string loginError)
        {
            string redirectUrl = QueryOther["process"];

            if (redirectUrl.Trim().EndsWith("/"))
            {
                redirectUrl = redirectUrl.Remove(redirectUrl.Length - 1, 1);
            }

            return string.Format("{0}login/?error={1}&redirect={2}", BasePath, loginError, redirectUrl);
        }

        private bool DebugEnabled
        {
            get
            {
                bool debugEnabled = (HttpPage.Session["enabledebug"] != null &&
                                     HttpPage.Session["enabledebug"].ToString() == "true");

                return debugEnabled;
            }
            set
            {
                HttpPage.Session["enabledebug"] = value 
                    ? "true" 
                    : "false";
            }
        }

        private string BasePath
        {
            get { return _basePath; }
        }

        public Cache Cache
        {
            get { return _cache ?? (_cache = new Cache(HttpPage.Application)); }
        }

        public XmlDocument XmlData { get; set; }

        public string CurrentProcess
        {
            get
            {
                if (_currentProcess == string.Empty)
                {
                    string tmpProcess = QueryOther["process"];
                    _currentProcess = tmpProcess != string.Empty ? tmpProcess : Settings["general/stdprocess"];
                }

                return _currentProcess;
            }
        }

        public string Root
        {
            get
            {
                return HttpPage.Server.MapPath(".");
            }
        }

        public Settings Settings
        {
            get
            {
                return _settings ?? (_settings = new Settings(this, Root));
            }
        }

        // >>>>> Search Mod by Kiho Chang 2008-10-05
        public object SearchContext
        {
            set
            {
                HttpPage.Session["SearchContext"] = value;
            }
        }

        public string CurrentUser
        {
            get
            {
                if (HttpPage.Session["current_username"] == null)
                {
                    Logout();
                }

                return HttpPage.Session["current_username"].ToString();
            }
        }
        // <<<<< Search Mod by Kiho Chang 2008-10-05

        private void AddMessage(string message, MessageType messageType, string type)
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
            AddMessage(message, messageType, string.Empty);
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

        private void DebugMessage(string message)
        {
            if (DebugEnabled)
            {
                AddMessage(message, MessageType.Debug);
            }
        }

        private void LoadBaseData()
        {
            XmlNode userNode = Content.GetSubControl("basedata")["currentuser"];
            CommonXml.GetNode(userNode, "username").InnerText = CurrentUser;
            XmlNode groupNode = CommonXml.GetNode(userNode, "groups");
            object[] resultsGroups = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            List<string> userGroups = new List<string>(Common.Common.FlattenToStrings(resultsGroups));

            foreach (string group in userGroups)
            {
                CommonXml.GetNode(groupNode, "group", EmptyNodeHandling.ForceCreateNew).InnerText = group;
            }

            ControlList baseData = Content.GetSubControl("basedata");

            baseData["pageviewcount"].InnerText = PageViewCount().ToString(CultureInfo.InvariantCulture);
            baseData["defaultpage"].InnerText = Settings["sitetree/stdpage"];

            foreach (string pageInHistory in History())
            {
                XmlDocument ownerDocument = baseData["history"].OwnerDocument;
                if (ownerDocument != null)
                {
                    XmlNode historyNode = ownerDocument.CreateElement("item");
                    historyNode.InnerText = pageInHistory;
                    baseData["history"].AppendChild(historyNode);
                }
            }
        }

        private void ConfigureDebugging()
        {
            if (HttpPage.Request.ServerVariables["REMOTE_ADDR"] != "127.0.0.1") return;

            if (HttpPage.Session["enabledebug"] == null)
            {
                DebugEnabled = true;
            }

            if (QueryOther["enabledebug"] == "true" || QueryOther["enabledebug"] == "false")
            {
                DebugEnabled = (QueryOther["enabledebug"] == "true");
            }
        }

        private void ProcessQueries()
        {
            List<string> keys = HttpPage.Request.Form.Cast<string>().ToList();
            
            keys.AddRange(HttpPage.Request.QueryString.Cast<string>());

            foreach (string key in keys)
            {
                if (key != null)
                {
                    string[] keyParts = key.Split('_');
                    string value = HttpPage.Request[key];

                    switch (keyParts[0])
                    {
                        case "data":
                            QueryData[string.Join("_", Common.Common.RemoveOne(keyParts))] = value;
                            break;
                        case "event":
                            QueryEvents[string.Join("_", Common.Common.RemoveOne(keyParts))] = value;
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
            string url = string.Format("{0}/{1}", BasePath, process);

            return url;
        }

        public string GetUrl(string process, string querystring)
        {
            string url = string.Format("{0}/{1}{2}", BasePath, process, querystring);

            return url;
        }

        private IEnumerable<string> History()
        {
            List<string> history = HttpPage.Session["history"] != null 
                ? (List<string>) HttpPage.Session["history"]
                : new List<string>();

            history.Add(CurrentProcess);

            HttpPage.Session["history"] = history;

            return history;
        }

        private int PageViewCount()
        {
            Dictionary<string, int> pageViewCounts;

            if (HttpPage.Session["pageviews"] != null)
            {
                pageViewCounts = (Dictionary<string, int>) HttpPage.Session["pageviews"];
                if (pageViewCounts.ContainsKey(CurrentProcess))
                {
                    pageViewCounts[CurrentProcess] += 1;
                }
                else
                {
                    pageViewCounts[CurrentProcess] = 1;
                }
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
        private bool Login(string username, string password)
        {
            bool success = false;

            Logout();
            object[] results = Plugins.InvokeAll("users", "verify", username, password);
            if (results.Length > 0)
            {
                bool verified = false;
                foreach (object result in results)
                {
                    if ((bool)result)
                    {
                        verified = true;
                    }
                }

                if (verified)
                {
                    HttpCookie httpCookie = HttpPage.Response.Cookies["login_cookie"];
                    if (httpCookie != null)
                    {
                        httpCookie.Value = string.Format("{0}{1}{2}", username, CookieSeparator, password);
                        httpCookie.Expires = DateTime.Now.AddDays(1);
                    }

                    HttpPage.Session["current_username"] = username;

                    success = true;
                }
            }

            return success;
        }

        private void LoginByCookie()
        {
            if (CurrentUser == "anonymous")
            {
                HttpCookie httpCookie = HttpPage.Request.Cookies["login_cookie"];
                if (httpCookie != null)
                {
                    string value = httpCookie.Value;
                    if (value != null && value.Contains(CookieSeparator))
                    {
                        string[] valueParts = Common.Common.SplitByString(value, CookieSeparator);

                        Login(valueParts[0], valueParts[1]);
                    }
                }
            }
        }

        private void UpdateCookieTimeout()
        {
            /*    if (HttpPage.Response.Cookies["login_cookie"] != null)
                {
                    HttpPage.Request.Cookies["login_cookie"].Value = HttpPage.Request.Cookies["login_cookie"].Value;
                    HttpPage.Response.Cookies["login_cookie"].Expires = DateTime.Now.AddDays(1);
                }*/
        }

        public bool CheckGroups(string groups)
        {
            object[] results = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            List<string> userGroups = new List<string>(Common.Common.FlattenToStrings(results));

            if (groups != string.Empty)
            {
                string[] groupList = groups.Split(',');
                return groupList.Any(userGroups.Contains);
            }
            
            return true;
        }

        private void Logout()
        {
            HttpPage.Session.Clear();
            HttpPage.Session["current_username"] = "anonymous";
            HttpCookie httpCookie = HttpPage.Response.Cookies["login_cookie"];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1);
            }
        }
    }

    public class ControlList : DataElementList
    {
        public ControlList(XmlNode parentNode) : base(parentNode) { }

        public XmlNode this[int index]
        {
            get
            {
                string xPath = string.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.CreateNew);

                return xmlNode;
            }
        }

        public XmlNode this[string name]
        {
            get
            {
                XmlNode xmlNode = GetControlNode(name);

                return xmlNode;
            }
            set
            {
                GetControlNode(name).InnerXml = value.InnerXml;
            }
        }

        public ControlList GetSubControl(string name)
        {
            ControlList subControl = name != string.Empty ? new ControlList(GetControlNode(name)) : null;

            return subControl;
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
        public XmlItemList(XmlNode parentNode) : base(parentNode) { }

        public Query this[int index]
        {
            get
            {
                string xPath = string.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.CreateNew);
                Query query = new Query(xmlNode.Name, xmlNode.InnerText);

                return query;
            }
            set
            {
                string xPath = string.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.Ignore);

                if (xmlNode == null) return;

                xmlNode.InnerText = value.Value;
            }
        }

        public string this[string name]
        {
            get
            {
                string xPath = string.Format("{0}", name);
                string innerText = GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText;

                return innerText;
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
        public readonly string Name;
        public readonly string Value;

        public Query(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public enum MessageType
    {
        Error = 0, 
        Status = 1, 
        Event = 2, 
        Warning = 3, 
        Message = 4, 
        Debug = 5
    }
}