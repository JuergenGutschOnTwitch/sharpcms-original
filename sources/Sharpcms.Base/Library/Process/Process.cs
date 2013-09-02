// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;

namespace Sharpcms.Base.Library.Process
{
    public class Process
    {
        private const String CookieSeparator = "cookieseparator";
        private readonly String _basePath;
        private Cache _cache;
        private String _currentProcess;
        private Settings _settings;
        private Dictionary<String, String> _variables;
        public String MainTemplate; //ToDo: this should be more logical
        public bool OutputHandledByModule;
        public readonly Page HttpPage;
        public readonly PluginServices Plugins;
        public readonly ControlList Content;
        public readonly XmlItemList Attributes;
        public readonly XmlItemList QueryData;
        public readonly XmlItemList QueryEvents;
        public readonly XmlItemList QueryOther;

        public Process(Page httpPage, PluginServices pluginServices)
        {
            _currentProcess = String.Empty;

            Plugins = pluginServices;
            HttpPage = httpPage;
            XmlData = new XmlDocument();

            Plugins.FindPlugins(this, Common.Common.CombinePaths(Root, "Bin"));

            XmlNode xmlNode = XmlData.CreateElement("data");
            XmlData.AppendChild(xmlNode);

            Content = new ControlList(xmlNode);

            _basePath = GetBasePath(httpPage);
            Content["basepath"].InnerText = _basePath;

            String referrer = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["HTTP_REFERER"]);
            Content["referrer"].InnerText = referrer ?? String.Empty;

            String domain = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["SERVER_NAME"]);
            Content["domain"].InnerText = domain ?? String.Empty;

            String useragent = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["HTTP_USER_AGENT"]);
            Content["useragent"].InnerText = useragent ?? String.Empty;

            String sessionid = httpPage.Server.UrlEncode(httpPage.Session.LCID.ToString(CultureInfo.InvariantCulture));
            Content["sessionid"].InnerText = sessionid ?? String.Empty;

            String ip = httpPage.Server.UrlEncode(httpPage.Request.ServerVariables["REMOTE_ADDR"]);
            Content["ip"].InnerText = ip ?? String.Empty;
            
            Attributes = new XmlItemList(CommonXml.GetNode(xmlNode, "attributes", EmptyNodeHandling.CreateNew));
            QueryData = new XmlItemList(CommonXml.GetNode(xmlNode, "query/data", EmptyNodeHandling.CreateNew));
            QueryEvents = new XmlItemList(CommonXml.GetNode(xmlNode, "query/events", EmptyNodeHandling.CreateNew));
            QueryOther = new XmlItemList(CommonXml.GetNode(xmlNode, "query/other", EmptyNodeHandling.CreateNew));

            ProcessQueries();
            ConfigureDebugging();
            LoginByCookie();

            String mainEvent = QueryEvents["main"];
            String mainEventValue = QueryEvents["mainValue"];

            if (mainEvent == "login")
            {
                if (!Login(QueryData["login"], QueryData["password"]))
                {
                    if (_settings != null && _settings["messages/loginerror"] != String.Empty)
                    {
                        httpPage.Response.Redirect(GetErrorUrl(httpPage.Server.UrlEncode(_settings["messages/loginerror"])));
                    }
                    else
                    {
                        httpPage.Response.Redirect(GetRedirectUrl());
                    }
                }
            }
            else if (mainEvent == "logout")
            {
                Logout();
                if (mainEventValue != String.Empty)
                {
                    HttpPage.Response.Redirect(mainEventValue);
                }
            }
            else if (mainEvent == String.Empty)
            {
                if (mainEventValue != String.Empty)
                {
                    HttpPage.Response.Redirect("/");
                }
            }

            UpdateCookieTimeout();

            LoadBaseData();
            // loads new user...
        }

        public Dictionary<String, String> Variables
        {
            get
            {
                return _variables ?? (_variables = new Dictionary<String, String>());
            }
        }

        public String RedirectUrl { get; set; }

        private String GetRedirectUrl()
        {
            String process = QueryOther["process"];

            if (process.Trim().EndsWith("/"))
            {
                process = process.Remove(process.Length - 1, 1);
            }

            String redirectUrl = String.Format("{0}login/?redirect={1}", BasePath, process);

            return redirectUrl;
        }

        private String GetErrorUrl(String loginError)
        {
            String process = QueryOther["process"];

            if (process.Trim().EndsWith("/"))
            {
                process = process.Remove(process.Length - 1, 1);
            }

            String errorUrl = String.Format("{0}login/?error={1}&redirect={2}", BasePath, loginError, process);

            return errorUrl;
        }

        private String GetBasePath(Page httpPage)
        {
            String basePath = String.Empty;

            if (httpPage.Request.ApplicationPath != null)
            {
                String serverProtocol = httpPage.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower();
                String serverName = httpPage.Request.ServerVariables["SERVER_NAME"];
                String serverPort = httpPage.Request.ServerVariables["SERVER_PORT"];
                String applicationPath = httpPage.Request.ApplicationPath.TrimEnd('/');

                basePath = String.Format("{0}://{1}", serverProtocol, serverName);

                if (!String.IsNullOrEmpty(serverPort) && serverPort != "80")
                {
                    basePath += String.Format(":{0}", serverPort);
                }

                Uri baseUri = new Uri(basePath);
                Uri applicationBaseUri = new Uri(baseUri, applicationPath);

                basePath = applicationBaseUri.AbsoluteUri;
            }

            return basePath;
        }

        private bool DebugEnabled
        {
            get
            {
                bool debugEnabled = (HttpPage.Session["enabledebug"] != null && HttpPage.Session["enabledebug"].ToString() == "true");

                return debugEnabled;
            }
            set
            {
                HttpPage.Session["enabledebug"] = value 
                    ? "true" 
                    : "false";
            }
        }

        private String BasePath
        {
            get
            {
                return _basePath;
            }
        }

        public Cache Cache
        {
            get
            {
                return _cache ?? (_cache = new Cache(HttpPage.Application));
            }
        }

        public XmlDocument XmlData { get; private set; }

        public String CurrentProcess
        {
            get
            {
                if (_currentProcess == String.Empty)
                {
                    String process = QueryOther["process"];

                    _currentProcess = process != String.Empty 
                        ? process 
                        : Settings["general/stdprocess"];
                }

                return _currentProcess;
            }
        }

        public String Root
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

        public Object SearchContext
        {
            set
            {
                HttpPage.Session["SearchContext"] = value;
            }
        }

        public String CurrentUser
        {
            get
            {
                if (HttpPage.Session["current_username"] == null)
                {
                    Logout();
                }

                return HttpPage.Session["current_username"] != null 
                    ? HttpPage.Session["current_username"].ToString() 
                    : String.Empty;
            }
        }

        private void AddMessage(String message, MessageType messageType, String type)
        {
            XmlNode xmlNode = CommonXml.GetNode(XmlData.DocumentElement, "messages", EmptyNodeHandling.CreateNew);
            xmlNode = CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew);
            xmlNode.InnerText = message;

            CommonXml.SetAttributeValue(xmlNode, "messagetype", messageType.ToString());
            CommonXml.SetAttributeValue(xmlNode, "type", type);

            IPlugin plugin = Plugins["ErrorLog"];
            if (plugin != null)
            {
                plugin.Handle("log");
            }
        }

        public void AddMessage(String message, MessageType messageType = MessageType.Message)
        {
            AddMessage(message, messageType, String.Empty);
        }

        public void AddMessage(Exception exception)
        {
            AddMessage(exception.Message, MessageType.Error, exception.GetType().ToString());
        }

        public void DebugMessage(Object message)
        {
            DebugMessage(message.ToString());
        }

        private void DebugMessage(String message)
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
            Object[] resultsGroups = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            List<String> userGroups = new List<String>(Common.Common.FlattenToStrings(resultsGroups));

            foreach (String group in userGroups)
            {
                CommonXml.GetNode(groupNode, "group", EmptyNodeHandling.ForceCreateNew).InnerText = group;
            }

            ControlList baseData = Content.GetSubControl("basedata");

            baseData["pageviewcount"].InnerText = PageViewCount().ToString(CultureInfo.InvariantCulture);
            baseData["defaultpage"].InnerText = Settings["sitetree/stdpage"];

            foreach (String pageInHistory in History())
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
            if (HttpPage.Request.ServerVariables["REMOTE_ADDR"] != "127.0.0.1")
            {
                return;
            }

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
            List<String> keys = HttpPage.Request.Form.Cast<String>().ToList();
            
            keys.AddRange(HttpPage.Request.QueryString.Cast<String>());

            foreach (String key in keys)
            {
                if (key != null)
                {
                    String[] keyParts = key.Split('_');
                    String value = HttpPage.Request[key];

                    switch (keyParts[0])
                    {
                        case "data":
                            QueryData[String.Join("_", Common.Common.RemoveOne(keyParts))] = value;
                            break;
                        case "event":
                            QueryEvents[String.Join("_", Common.Common.RemoveOne(keyParts))] = value;
                            break;
                        default:
                            QueryOther[key] = value;
                            break;
                    }
                }
            }
        }

        public String GetUrl(String process, String querystring = null)
        {
            Uri baseUri = new Uri(BasePath);
            Uri uri = new Uri(baseUri, String.Format("{0}{1}", process, querystring));

            return uri.AbsoluteUri;
        }

        private IEnumerable<String> History()
        {
            List<String> history = HttpPage.Session["history"] != null 
                ? (List<String>) HttpPage.Session["history"]
                : new List<String>();

            history.Add(CurrentProcess);
            HttpPage.Session["history"] = history;

            return history;
        }

        private int PageViewCount()
        {
            Dictionary<String, int> pageViewCounts;

            if (HttpPage.Session["pageviews"] != null)
            {
                pageViewCounts = (Dictionary<String, int>) HttpPage.Session["pageviews"];

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
                pageViewCounts = new Dictionary<String, int>();
                pageViewCounts[CurrentProcess] = 1;
            }

            HttpPage.Session["pageviews"] = pageViewCounts;

            return pageViewCounts[CurrentProcess];
        }

        private bool Login(String username, String password)
        {
            bool success = false;

            Logout();
            Object[] results = Plugins.InvokeAll("users", "verify", username, password);
            
            if (results.Length > 0)
            {
                bool verified = false;

                foreach (Object result in results)
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
                        httpCookie.Value = String.Format("{0}{1}{2}", username, CookieSeparator, password);
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
                    String value = httpCookie.Value;
                    
                    if (value != null && value.Contains(CookieSeparator))
                    {
                        String[] valueParts = Common.Common.SplitByString(value, CookieSeparator);

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

        public bool CheckGroups(String groups)
        {
            bool valid = true;

            Object[] results = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            List<String> userGroups = new List<String>(Common.Common.FlattenToStrings(results));

            if (groups != String.Empty)
            {
                String[] groupList = groups.Split(',');
                
                valid = groupList.Any(userGroups.Contains);
            }

            return valid;
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
}