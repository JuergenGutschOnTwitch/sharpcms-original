// sharpcms is licensed under the open source license GPL - GNU General Public License.

namespace Sharpcms.PluginInterface
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
			foreach (AvailablePlugin pluginOn in this.List)
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
	}

	/// <summary>
	/// Data Class for Available Plugin. Holds an instance of the loaded Plugin, as well as the Plugin's Assembly Path
	/// </summary>
	public class AvailablePlugin
	{
		//This is the actual AvailablePlugin object.. 
		//Holds an instance of the plugin to access
		//ALso holds assembly path... not really necessary
	    private string _assemblyPath = "";

	    public AvailablePlugin()
	    {
	        Instance = null;
	    }

	    public IPlugin Instance { get; set; }

	    public string AssemblyPath
		{
			get { return _assemblyPath; }
			set { _assemblyPath = value; }
		}
	}
}
