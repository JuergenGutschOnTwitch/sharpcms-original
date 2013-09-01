// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Collections;
using System.Collections.Generic;

namespace Sharpcms.Base.Library.Plugin
{
    /// <summary>
    /// Collection for AvailablePlugin Type
    /// </summary>
    public class AvailablePlugins : CollectionBase
    {
        //A Simple Home-brew class to hold some info about our Available Plugins

        /// <summary>
        /// Add a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToAdd">The Plugin to Add</param>
        public void Add(AvailablePlugin pluginToAdd)
        {
            List.Add(pluginToAdd);
        }

        /// <summary>
        /// Remove a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToRemove">The Plugin to Remove</param>
        public void Remove(AvailablePlugin pluginToRemove)
        {
            List.Remove(pluginToRemove);
        }

        /// <summary>
        /// Finds a plugin in the available Plugins
        /// </summary>
        /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
        /// <returns>Available Plugin, or null if the plugin is not found</returns>
        public AvailablePlugin Find(string pluginNameOrPath)
        {
            AvailablePlugin toReturn = null;

            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                {
                    toReturn = pluginOn;
                    break;
                }
            }

            return toReturn;
        }

        public IEnumerable<AvailablePlugin> FindImplementations(string api)
        {
            List<AvailablePlugin> plugins = new List<AvailablePlugin>();

            foreach (AvailablePlugin plugin in List)
            {
                IPlugin2 instance = plugin.Instance as IPlugin2;
                if (instance != null && instance.Implements != null)
                {
                    List<string> implements = new List<string>(instance.Implements);
                    if (implements.Contains(api))
                    {
                        plugins.Add(plugin);
                    }
                }
            }

            return plugins;
        }
    }

    /// <summary>
    /// Data Class for Available Plugin. Holds an instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// </summary>
    public class AvailablePlugin
    {
        //This is the actual AvailablePlugin object.. 
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private string _myAssemblyPath = string.Empty;

        public IPlugin Instance { get; set; }

        public string AssemblyPath
        {
            get { return _myAssemblyPath; }
            set { _myAssemblyPath = value; }
        }
    }
}