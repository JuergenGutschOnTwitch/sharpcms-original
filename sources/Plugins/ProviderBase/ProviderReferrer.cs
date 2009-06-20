//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProvicerReferrer : BasePlugin2, IPlugin2
    {
        public ProvicerReferrer()
        {
        }

        public ProvicerReferrer(Process process)
        {
            _process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "Referrer"; }
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

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }

        #endregion

        private void HandleLog()
        {
            _process.Content["referrer"].InnerText = _process.HttpPage.Request.UrlReferrer.ToString();
        }
    }
}