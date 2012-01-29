// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderPlugin : BasePlugin2, IPlugin2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderPlugin"/> class.
        /// </summary>
        public ProviderPlugin()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderPlugin"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public ProviderPlugin(Process process)
        {
            Process = process;
        }

        #region IPlugin2 Members

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public new string Name
        {
            get { return "Plugin"; }
        }

        /// <summary>
        /// Loads the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
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

        /// <summary>
        /// Loadlists this instance.
        /// </summary>
        private static void Loadlist()
        {
        }
    }
}