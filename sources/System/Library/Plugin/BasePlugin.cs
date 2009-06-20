//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

namespace InventIt.SiteSystem.Plugin
{
    /// <summary>
    /// Implements most of the IPlugin interface. By inherting from this class,
    /// you won't have to implement the methods and properties you don't need.
    /// </summary>
    public class BasePlugin : IPlugin
    {
        protected Process _process;

        #region IPlugin Members

        public string Name
        {
            get { return "BasePlugin"; }
        }

        public Process Process
        {
            get { return _process; }
            set { _process = value; }
        }

        public IPluginHost Host { get; set; }

        public void Initialize()
        {
            // Do nothing
        }

        public void Dispose()
        {
            // Do nothing
        }

        public void Handle(string mainEvent)
        {
            // Do nothing
        }

        public void Load(ControlList control, string action, string pathTrail)
        {
            // Do nothing
        }

        #endregion
    }

    public class BasePlugin2 : BasePlugin, IPlugin2
    {
        #region IPlugin2 Members

        public string[] Implements
        {
            get { return null; }
        }

        public object Invoke(string api, string action, params object[] args)
        {
            return null;
        }

        public void Load(ControlList control, string action, string value, string pathTrail)
        {
            Load(control, action, null, pathTrail);
        }

        #endregion
    }
}