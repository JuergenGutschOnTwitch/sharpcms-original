//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Library;
using System.Xml;
using System.Net.Mail;

namespace InventIt.SiteSystem.Providers
{
    public class ProvicerReferrer : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "Referrer";
            }
        }

        public ProvicerReferrer()
        {
        }

        public ProvicerReferrer(Process process)
        {
            m_Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }
        public void HandleLog()
        {
            m_Process.Content["referrer"].InnerText = m_Process.HttpPage.Request.UrlReferrer.ToString();
        }


        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }
    }
}