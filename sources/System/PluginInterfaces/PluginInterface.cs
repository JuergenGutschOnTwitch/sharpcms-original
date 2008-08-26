using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem.Plugin.Types;

namespace InventIt.SiteSystem.Plugin
{
	public interface IPlugin
	{
		string Name { get; }
		Process Process { get; set; }

		IPluginHost Host { get; set; }

		void Initialize();
		void Dispose();

		void Handle(string mainEvent);
		void Load(string value, string[] args,XmlNode node);
	}

	public interface IPluginHost
	{
		AvailablePlugins AvailablePlugins { get; set; }
	}
}