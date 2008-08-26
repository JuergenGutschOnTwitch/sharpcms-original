//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using System.Xml;


namespace InventIt.SiteSystem.Providers
{
    public class ProviderPlugin : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "Plugin";
            }
        }

        public ProviderPlugin()
        {
        }

        public ProviderPlugin(Process process)
        {
            m_Process = process;
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "list":
                    Loadlist();
                    break;
            }
        }
        
        private void Loadlist()
        {
           
        }
    }
}
