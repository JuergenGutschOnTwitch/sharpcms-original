// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Library.Process;

namespace Sharpcms.PluginInterface
{
	public interface IPlugin
	{
		string Name { get; }
		Process Process { get; set; }
        IPluginHost Host { get; set; }

		void Initialize();
		void Dispose();
        void Handle(string mainEvent);
		void Load(string value, string[] args, XmlNode node);
	}

	public interface IPluginHost
	{
		AvailablePlugins AvailablePlugins { get; set; }
	}
}