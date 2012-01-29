// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Web;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Cookies
{
    public class ProviderCookies : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get { return "Cookies"; }
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

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "cookie":
                    HandleCookies();
                    break;
            }
        }

        private void HandleCookies()
        {
            foreach (string key in Process.HttpPage.Request.QueryString.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ","))
                {
                    var cookie = new HttpCookie(key, Process.HttpPage.Request.QueryString[key]) { Expires = DateTime.Now.AddDays(1) };
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "cookie":
                    LoadCookies(control);
                    break;
            }
        }


        private void LoadCookies(ControlList control)
        {
            {
                var cookieData = new XmlItemList(CommonXml.GetNode(control.ParentNode, "items", EmptyNodeHandling.CreateNew));
                foreach (string key in Process.HttpPage.Response.Cookies.Keys)
                {
                    if (Process.Settings["general/cookies"].Contains("," + key + ","))
                    {
                        HttpCookie httpCookie = Process.HttpPage.Response.Cookies[key];
                        if (httpCookie != null)
                        {
                            cookieData[key.Replace(".", "")] = HttpUtility.UrlEncode(httpCookie.Value);
                        }
                    }
                }

                foreach (string key in Process.HttpPage.Request.Cookies.Keys)
                {
                    if (Process.Settings["general/cookies"].Contains("," + key + ",") && string.IsNullOrEmpty(cookieData[key.Replace(".", "")]))
                    {
                        HttpCookie httpCookie = Process.HttpPage.Request.Cookies[key];
                        if (httpCookie != null)
                        {
                            cookieData[key.Replace(".", "")] = HttpUtility.UrlEncode(httpCookie.Value);
                        }
                    }
                }
            }
        }
    }
}
