//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Collections.Generic;
using System.IO;
using System.Text;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderAdmin : BasePlugin2, IPlugin2
    {
        public ProviderAdmin()
        {
        }

        public ProviderAdmin(Process process)
        {
            _process = process;
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
                    _process.Cache.Clean();
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
            paths[0] = _process.Settings["general/customrootcomponents"];
            paths[1] = _process.Settings["general/systemrootcomponents"];

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
                                string destination = Common.CombinePaths(_process.Root, "Bin", fileinfo.Name);
                                if (!File.Exists(destination) ||
                                    fileinfo.LastWriteTime != File.GetLastWriteTime(destination))
                                    File.Copy(fileinfo.FullName,
                                              Common.CombinePaths(_process.Root, "Bin", fileinfo.Name), true);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSnippets(IEnumerable<string> paths)
        {
            string pathsnippets = _process.Settings["general/customrootcomponents"] + "\\snippets.xslt";
            // ToDo: should be more generic (old)

            var stringB = new StringBuilder();
            stringB.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringB.AppendLine("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            foreach (string dir in paths)
            {
                var dirinfo = new DirectoryInfo(dir);

                foreach (DirectoryInfo subdirinfo in dirinfo.GetDirectories())
                {
                    if (!(subdirinfo.Name == ".svn"))
                    {
                        if (Directory.Exists(Common.CombinePaths(subdirinfo.FullName, "Xsl")))
                        {
                            var xslDirInfo = new DirectoryInfo(Common.CombinePaths(subdirinfo.FullName, "Xsl"));
                            foreach (FileInfo fileinfo in xslDirInfo.GetFiles())
                                if ((fileinfo.Extension == ".xslt" || fileinfo.Extension == ".xsl") &&
                                    fileinfo.Name[0] == '_')
                                    if (dirinfo.Parent != null)
                                        stringB.AppendLine("<xsl:include href=\"..\\..\\" + dirinfo.Parent.Name + "\\" +
                                                           dirinfo.Name + "\\" + subdirinfo.Name + "\\" +
                                                           xslDirInfo.Name + "\\" + fileinfo.Name + "\"/>");
                        }
                    }
                }
            }
            stringB.AppendLine("</xsl:stylesheet>");
            File.WriteAllText(pathsnippets, stringB.ToString(), Encoding.UTF8);
        }

        private void LoadMenu(ControlList control)
        {
            control["adminmenu"] = _process.Settings.GetAsNode("admin/menu");
        }
    }
}