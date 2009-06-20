//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderPlugin : BasePlugin2, IPlugin2
    {
        public ProviderPlugin()
        {
        }

        public ProviderPlugin(Process process)
        {
            _process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "Plugin"; }
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

        #endregion

        private static void Loadlist()
        {
        }
    }
}