using System;
using System.Web;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Library;

namespace Cookies
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
            _process = process;
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
                    HttpCookie cookie = new HttpCookie(key, Process.HttpPage.Request.QueryString[key])
                                            {
                                                Expires = DateTime.Now.AddDays(1)
                                            };
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "cookie":
                    LoadCookies(value, control);
                    break;
            }
        }


        private void LoadCookies(string value, ControlList control)
        {
            {
                XmlItemList cookieData = new XmlItemList(CommonXml.GetNode(control.ParentNode, "items", EmptyNodeHandling.CreateNew));
                foreach (string key in Process.HttpPage.Response.Cookies.Keys)
                {
                    if (Process.Settings["general/cookies"].Contains("," + key + ","))
                    {
                        cookieData[key.Replace(".", "")] = HttpUtility.UrlEncode(Process.HttpPage.Response.Cookies[key].Value);
                    }
                }
                foreach (string key in Process.HttpPage.Request.Cookies.Keys)
                {
                    if (Process.Settings["general/cookies"].Contains("," + key + ",") && string.IsNullOrEmpty(cookieData[key.Replace(".", "")]))
                    {
                        cookieData[key.Replace(".", "")] = HttpUtility.UrlEncode(Process.HttpPage.Request.Cookies[key].Value);
                    }
                }
            }
        }
    }
}
