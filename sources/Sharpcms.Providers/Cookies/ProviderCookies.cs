// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Web;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Providers.Cookies
{
    public class ProviderCookies : BasePlugin2, IPlugin2
    {
        public new String Name
        {
            get
            {
                return "Cookies";
            }
        }

        public ProviderCookies()
        {
        }

        public ProviderCookies(Process process)
        {
            Process = process;
        }

        public new void Initialize()
        {
            // Do nothing
        }

        public new void Dispose()
        {
            // Do nothing
        }

        public new void Handle(String mainEvent)
        {
            switch (mainEvent)
            {
                case "cookie":
                    HandleCookies();
                    break;
            }
        }

        public new void Load(ControlList control, String action, String value, String pathTrail)
        {
            switch (action)
            {
                case "cookie":
                    LoadCookies(control);
                    break;
            }
        }

        private void HandleCookies()
        {
            foreach (String key in Process.HttpPage.Request.QueryString.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ","))
                {
                    HttpCookie cookie = new HttpCookie(key, Process.HttpPage.Request.QueryString[key]) { Expires = DateTime.Now.AddDays(1) };
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        private void LoadCookies(ControlList control)
        {
            XmlItemList cookieData = new XmlItemList(CommonXml.GetNode(control.ParentNode, "items", EmptyNodeHandling.CreateNew));

            foreach (String key in Process.HttpPage.Response.Cookies.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ","))
                {
                    HttpCookie httpCookie = Process.HttpPage.Response.Cookies[key];
                    if (httpCookie != null)
                    {
                        cookieData[key.Replace(".", String.Empty)] = HttpUtility.UrlEncode(httpCookie.Value);
                    }
                }
            }

            foreach (String key in Process.HttpPage.Request.Cookies.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ",") && String.IsNullOrEmpty(cookieData[key.Replace(".", String.Empty)]))
                {
                    HttpCookie httpCookie = Process.HttpPage.Request.Cookies[key];
                    if (httpCookie != null)
                    {
                        cookieData[key.Replace(".", String.Empty)] = HttpUtility.UrlEncode(httpCookie.Value);
                    }
                }
            }
        }
    }
}
