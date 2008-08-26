//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Plugin.Types;


namespace InventIt.SiteSystem.Plugin
{
	/// <summary>
	/// Summary description for PluginServices.
	/// </summary>
	public class PluginServices : IPluginHost
	{
        public IPlugin this[string pluginNameOrPath]
        {
            get
            {
                AvailablePlugin plugin = AvailablePlugins.Find(pluginNameOrPath);
                if (plugin == null)
                {
                    return null;
                }

                if (plugin.Instance == null)
                {
                    return null;
                }

                return plugin.Instance;
            }
        }

		/// <summary>
		/// Constructor of the Class
		/// </summary>
		public PluginServices()
		{
		}

		private AvailablePlugins colAvailablePlugins = new AvailablePlugins();

		/// <summary>
		/// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
		/// </summary>
		public AvailablePlugins AvailablePlugins
		{
			get { return colAvailablePlugins; }
			set { colAvailablePlugins = value; }
		}

        /// <summary>
        /// Invokes the action on all plug-ins implementing the specified API.
        /// </summary>
        /// <param name="api">API</param>
        /// <param name="action">Action to invoke</param>
        /// <param name="args">Arguments, are passed on to the Plugins</param>
        /// <returns>An array of results from the Plugins (only non-null values are included)</returns>
        public object[] InvokeAll(string api, string action, params object[] args)
        {
            List<object> results = new List<object>();

            List<AvailablePlugin> plugins = AvailablePlugins.FindImplementations(api);
            foreach (AvailablePlugin plugin in plugins)
            {
                IPlugin2 invokablePlugin = plugin.Instance as IPlugin2;
                if (invokablePlugin == null) continue;
                object result = invokablePlugin.Invoke(api, action, args);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results.ToArray();
        }

        public object[] Flatten(object[] results)
        {
            List<object> flattened = new List<object>();
            try
            {
                foreach (object result in results)
                {
                    if (result != null)
                    {
                        object[] partResults = result as object[];
                        foreach (object partResult in partResults)
                        {
                            flattened.Add(partResult);
                        }
                    }
                }
            }
            catch
            {
                // Just ignore...
            }

            return flattened.ToArray();
        }

		/// <summary>
		/// Searches the Application's Startup Directory for Plugins
		/// </summary>
		public void FindPlugins(Process process)
		{
			FindPlugins(process, AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		/// Searches the passed Path for Plugins
		/// </summary>
		/// <param name="Path">Directory to search for Plugins in</param>
		public void FindPlugins(Process process, string Path)
		{
			//First empty the collection, we're reloading them all
			colAvailablePlugins.Clear();

			//Go through all the files in the plugin directory
			foreach (string fileOn in Directory.GetFiles(Path))
			{
				FileInfo file = new FileInfo(fileOn);

				// Preliminary check, must be .dll
				if (file.Extension.Equals(".dll"))
				{
					//Add the 'plugin'
					this.AddPlugin(fileOn, process);
				}
			}
		}

		/// <summary>
		/// Unloads and Closes all AvailablePlugins
		/// </summary>
		public void ClosePlugins()
		{
			foreach (AvailablePlugin pluginOn in colAvailablePlugins)
			{
				//Close all plugin instances
				//We call the plugins Dispose sub first incase it has to do 
				//Its own cleanup stuff
				pluginOn.Instance.Dispose();

				//After we give the plugin a chance to tidy up, get rid of it
				pluginOn.Instance = null;
			}

			//Finally, clear our collection of available plugins
			colAvailablePlugins.Clear();
		}

		private void AddPlugin(string FileName, Process process)
		{
			//Create a new assembly from the plugin file we're adding..
			Assembly pluginAssembly = Assembly.LoadFrom(FileName);

			//Next we'll loop through all the Types found in the assembly
			foreach (Type pluginType in pluginAssembly.GetTypes())
			{
				if (pluginType.IsPublic) //Only look at public types
				{
					if (!pluginType.IsAbstract)  //Only look at non-abstract types
					{
						//Gets a type object of the interface we need the plugins to match
						Type typeInterface = pluginType.GetInterface("InventIt.SiteSystem.Plugin.IPlugin", true);

						//Make sure the interface we want to use actually exists
						if (typeInterface != null)
						{
							//Create a new available plugin since the type implements the IPlugin interface
							AvailablePlugin newPlugin = new AvailablePlugin();

							//Set the filename where we found it
							newPlugin.AssemblyPath = FileName;

							//Create a new instance and store the instance in the collection for later use
							//We could change this later on to not load an instance.. we have 2 options
							//1- Make one instance, and use it whenever we need it.. it's always there
							//2- Don't make an instance, and instead make an instance whenever we use it, then close it
							//For now we'll just make an instance of all the plugins
							newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));

							//Set the Plugin's host to this class which inherited IPluginHost
							newPlugin.Instance.Host = this;

							//Set the Plugin's process
							newPlugin.Instance.Process = process;

							//Call the initialization sub of the plugin
							newPlugin.Instance.Initialize();

                            // Do not add the BasePlugin
                            if (!newPlugin.Instance.Name.StartsWith("BasePlugin"))
                            {
                                //Add the new plugin to our collection here
                                this.colAvailablePlugins.Add(newPlugin);
                            }

							//cleanup a bit
							newPlugin = null;
						}

						typeInterface = null; //Mr. Clean			
					}
				}
			}

			pluginAssembly = null; //more cleanup
		}
	}
}
