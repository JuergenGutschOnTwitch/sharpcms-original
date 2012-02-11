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
                var dirInfo = new DirectoryInfo(dir);
                foreach (DirectoryInfo subdirinfo in dirInfo.GetDirectories())
                {
                    if (Directory.Exists(Common.CombinePaths(subdirinfo.FullName, "Plugins")))
                    {
                        var pluginDirInfo = new DirectoryInfo(Common.CombinePaths(subdirinfo.FullName, "Plugins"));
                        foreach (FileInfo fileinfo in pluginDirInfo.GetFiles())
                        {
                            if (fileinfo.Extension == ".dll" || fileinfo.Extension == ".pdb")
                            {
                                string destination = Common.CombinePaths(Process.Root, "Bin", fileinfo.Name);
                                if (!File.Exists(destination) || fileinfo.LastWriteTime != File.GetLastWriteTime(destination))
                                {
                                    File.Copy(fileinfo.FullName, Common.CombinePaths(Process.Root, "Bin", fileinfo.Name), true);
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

            var stringB = new StringBuilder();
            stringB.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringB.AppendLine("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            foreach (string dir in paths)
            {
                var dirinfo = new DirectoryInfo(dir);

                foreach (DirectoryInfo subdirinfo in dirinfo.GetDirectories())
                {
                    if (subdirinfo.Name != ".svn")
                    {
                        if (Directory.Exists(Common.CombinePaths(subdirinfo.FullName, "Xsl")))
                        {
                            var xslDirInfo = new DirectoryInfo(Common.CombinePaths(subdirinfo.FullName, "Xsl"));
                            foreach (FileInfo fileinfo in xslDirInfo.GetFiles())
                            {
                                if ((fileinfo.Extension == ".xslt" || fileinfo.Extension == ".xsl") && fileinfo.Name[0] == '_')
                                {
                                    if (dirinfo.Parent != null)
                                    {
                                        stringB.AppendLine("<xsl:include href=\"..\\..\\" + dirinfo.Parent.Name + "\\" + dirinfo.Name + "\\" + subdirinfo.Name + "\\" + xslDirInfo.Name + "\\" + fileinfo.Name + "\"/>");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            stringB.AppendLine("</xsl:stylesheet>");
            File.WriteAllText(pathsnippets, stringB.ToString(), Encoding.UTF8);
        }

        private void LoadMenu(ControlList control)
        {
            control["adminmenu"] = Process.Settings.GetAsNode("admin/menu");
        }
    }
}