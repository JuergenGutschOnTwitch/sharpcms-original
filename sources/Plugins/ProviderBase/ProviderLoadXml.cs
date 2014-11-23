// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderLoadXml : BasePlugin2, IPlugin2
    {
        public ProviderLoadXml() { }

        public ProviderLoadXml(Process process)
        {
            Process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get
            {
                return "LoadXml";
            }
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "submitform":

                    break;
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Process.Settings["loadxml/" + action]);

            if (doc.DocumentElement != null)
            {
                control[action].InnerXml = doc.DocumentElement.InnerXml;
            }
        }

        #endregion

        public void HandleSubmitForm()
        {

        }
    }
}