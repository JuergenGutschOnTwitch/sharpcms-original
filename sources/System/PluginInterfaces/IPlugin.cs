using System.Xml;
using Sharpcms.Library.Process;

namespace Sharpcms.PluginInterface
{
    public interface IPlugin
    {
        string Name { get; }
        Process Process { set; }
        IPluginHost Host { set; }

        void Initialize();
        void Dispose();
        void Handle(string mainEvent);
        void Load(string value, string[] args, XmlNode node);
    }
}