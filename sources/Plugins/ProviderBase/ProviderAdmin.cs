// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Collections.Generic;
using System.IO;
using System.Text;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderAdmin : BasePlugin2, IPlugin2
    {
        public ProviderAdmin()
        {
            Process = null;
        }

        public ProviderAdmin(Process process) : this()
        {
            Process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "Admin"; }
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "update":
                    HandleUpdate();
                    break;
                case "clear":
                    Process.Cache.Clean();
                    break;
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "adminmenu":
                    LoadMenu(control);
                    break;
            }
        }

        #endregion

        private void HandleUpdate()
        {
            var paths = new string[2];
            paths[0] = Process.Settings["general/customrootcomponents"];
            paths[1] = Process.Settings["general/systemrootcomponents"];

            UpdateSnippets(paths);
            UpdateDlls(paths);
        }

        private void UpdateDlls(IEnumerable<string> paths)
        {
            foreach (string dir in paths)
            {
                var directoryInfo = new DirectoryInfo(dir);
                foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    if (Directory.Exists(Common.CombinePaths(subDirectoryInfo.FullName, "Plugins")))
                    {
                        var pluginDirectoryInfo = new DirectoryInfo(Common.CombinePaths(subDirectoryInfo.FullName, "Plugins"));
                        foreach (FileInfo fileInfo in pluginDirectoryInfo.GetFiles())
                        {
                            if (fileInfo.Extension == ".dll" || fileInfo.Extension == ".pdb")
                            {
                                string destination = Common.CombinePaths(Process.Root, "Bin", fileInfo.Name);
                                if (!File.Exists(destination) || fileInfo.LastWriteTime != File.GetLastWriteTime(destination))
                                {
                                    File.Copy(fileInfo.FullName, Common.CombinePaths(Process.Root, "Bin", fileInfo.Name), true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSnippets(IEnumerable<string> paths)
        {
            string pathsnippets = Process.Settings["general/customrootcomponents"] + "\\snippets.xslt";
            // ToDo: should be more generic

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("<xsl:stylesheet");
            stringBuilder.AppendLine("    version=\"1.0\"");
            stringBuilder.AppendLine("    xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"");
            stringBuilder.AppendLine("    xmlns=\"http://www.w3.org/1999/xhtml\">");

            foreach (string dir in paths)
            {
                var directoryInfo = new DirectoryInfo(dir);

                foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    if (subDirectoryInfo.Name != ".svn")
                    {
                        if (Directory.Exists(Common.CombinePaths(subDirectoryInfo.FullName, "Xsl")))
                        {
                            var xslDirectoryInfo = new DirectoryInfo(Common.CombinePaths(subDirectoryInfo.FullName, "Xsl"));
                            foreach (FileInfo fileInfo in xslDirectoryInfo.GetFiles())
                            {
                                if ((fileInfo.Extension == ".xslt" || fileInfo.Extension == ".xsl") && fileInfo.Name[0] == '_')
                                {
                                    if (directoryInfo.Parent != null)
                                    {
                                        stringBuilder.AppendLine("<xsl:include href=\"..\\..\\" + directoryInfo.Parent.Name + "\\" + directoryInfo.Name + "\\" + subDirectoryInfo.Name + "\\" + xslDirectoryInfo.Name + "\\" + fileInfo.Name + "\"/>");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            stringBuilder.AppendLine("</xsl:stylesheet>");

            File.WriteAllText(pathsnippets, stringBuilder.ToString(), Encoding.UTF8);
        }

        private void LoadMenu(ControlList control)
        {
            control["adminmenu"] = Process.Settings.GetAsNode("admin/menu");
        }
    }
}