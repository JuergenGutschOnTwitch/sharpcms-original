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
    public class ProviderLoadXml : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "LoadXml";
            }
        }

        public ProviderLoadXml()
        {

        }

        public ProviderLoadXml(Process process)
        {
            m_Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "submitform":
                    
                    break;
            }
        }

        public void HandleSubmitForm()
        {
		
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_Process.Settings["loadxml/" + action]);

            control[action].InnerXml = doc.DocumentElement.InnerXml;
        }
    }
}