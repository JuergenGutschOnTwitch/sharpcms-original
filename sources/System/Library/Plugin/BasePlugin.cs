using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Plugin
{
    /// <summary>
    /// Implements most of the IPlugin interface. By inherting from this class,
    /// you won't have to implement the methods and properties you don't need.
    /// </summary>
    public class BasePlugin : IPlugin
    {
        protected Process m_Process;
        protected IPluginHost m_Host;

        public string Name
        {
            get
            {
                return "BasePlugin";
            }
        }

        public Process Process
        {
            get { return m_Process; }
            set { m_Process = value; }
        }

        public IPluginHost Host
        {
            get { return m_Host; }
            set { m_Host = value; }
        }

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
    }

    public class BasePlugin2 : BasePlugin, IPlugin2
    {
        public string[] Implements
        {
            get
            {
                return null;
            }
        }

        public object Invoke(string api, string action, params object[] args)
        {
            return null;
        }

        public void Load(ControlList control, string action, string value, string pathTrail)
        {
            Load(control, action, null, pathTrail);
        }
    }
}