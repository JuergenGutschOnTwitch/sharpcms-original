// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProvicerReferrer : BasePlugin2, IPlugin2
    {
        public ProvicerReferrer()
        {
        }

        public ProvicerReferrer(Process process)
        {
            Process = process;
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
            if (Process.HttpPage.Request.UrlReferrer != null)
            {
                Process.Content["referrer"].InnerText = Process.HttpPage.Request.UrlReferrer.ToString();
            }
        }
    }
}