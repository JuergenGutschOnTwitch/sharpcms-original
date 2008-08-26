//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;

namespace InventIt.SiteSystem.Plugin.Types
{
	/// <summary>
	/// Collection for AvailablePlugin Type
	/// </summary>
	public class AvailablePlugins : System.Collections.CollectionBase
	{
		//A Simple Home-brew class to hold some info about our Available Plugins

		/// <summary>
		/// Add a Plugin to the collection of Available plugins
		/// </summary>
		/// <param name="pluginToAdd">The Plugin to Add</param>
		public void Add(Types.AvailablePlugin pluginToAdd)
		{
			this.List.Add(pluginToAdd);
		}

		/// <summary>
		/// Remove a Plugin to the collection of Available plugins
		/// </summary>
		/// <param name="pluginToRemove">The Plugin to Remove</param>
		public void Remove(Types.AvailablePlugin pluginToRemove)
		{
			this.List.Remove(pluginToRemove);
		}

		/// <summary>
		/// Finds a plugin in the available Plugins
		/// </summary>
		/// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
		/// <returns>Available Plugin, or null if the plugin is not found</returns>
		public Types.AvailablePlugin Find(string pluginNameOrPath)
		{
			Types.AvailablePlugin toReturn = null;

			//Loop through all the plugins
			foreach (Types.AvailablePlugin pluginOn in this.List)
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

        public List<AvailablePlugin> FindImplementations(string api)
        {
            List<AvailablePlugin> plugins = new List<AvailablePlugin>();

            foreach (AvailablePlugin plugin in this.List)
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
		private IPlugin myInstance = null;
		private string myAssemblyPath = "";

		public IPlugin Instance
		{
			get { return myInstance; }
			set { myInstance = value; }
		}
		public string AssemblyPath
		{
			get { return myAssemblyPath; }
			set { myAssemblyPath = value; }
		}
	}
}
