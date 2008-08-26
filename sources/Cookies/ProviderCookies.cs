using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Library;
using System.Xml;

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
            m_Process = process;
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
            foreach (string key in base.Process.HttpPage.Request.QueryString.Keys)
            {
                if (base.Process.Settings["general/cookies"].Contains("," + key + ","))
                {
                    System.Web.HttpCookie cookie = new System.Web.HttpCookie(key, base.Process.HttpPage.Request.QueryString[key]);
                    cookie.Expires = DateTime.Now.AddDays(1);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
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
                XmlItemList CookieData = new XmlItemList(CommonXml.GetNode(control.ParentNode, "items", EmptyNodeHandling.CreateNew));
                List<string> keys = new List<string>();
                foreach (string key in base.Process.HttpPage.Response.Cookies.Keys)
                {
                    if (base.Process.Settings["general/cookies"].Contains("," + key + ","))
                        CookieData[key.Replace(".", "")] = System.Web.HttpUtility.UrlEncode(base.Process.HttpPage.Response.Cookies[key].Value);
                }
                foreach (string key in base.Process.HttpPage.Request.Cookies.Keys)
                {
                    if (base.Process.Settings["general/cookies"].Contains("," + key + ",") && string.IsNullOrEmpty(CookieData[key.Replace(".", "")]))
                        CookieData[key.Replace(".", "")] = System.Web.HttpUtility.UrlEncode(base.Process.HttpPage.Request.Cookies[key].Value);
                }
            }
        }
    }
}
