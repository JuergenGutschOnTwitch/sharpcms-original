//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Library;
using System.Xml;
using System.IO;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderAdmin : BasePlugin2, IPlugin2
    {
        public new string Name
        {
			get
			{
				return "Admin";
			}
        }

		public ProviderAdmin()
		{
		}

        public ProviderAdmin(Process process)
        {
           m_Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "update":
                    HandleUpdate();
                    break;
                case "clear":
                    m_Process.Cache.Clean(); 
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

        private void HandleUpdate()
        {
            string[] paths = new string[2] ;
            paths[0] = m_Process.Settings["general/customrootcomponents"];
            paths[1] = m_Process.Settings["general/systemrootcomponents"];

            UpdateSnippets(paths);
            UpdateDlls(paths);
        }

        private void UpdateDlls(string[] paths)
        {
            foreach (string dir in paths)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                foreach (DirectoryInfo subdirinfo in dirInfo.GetDirectories())
                {
                    if (Directory.Exists(Common.CombinePaths(subdirinfo.FullName, "Plugins")))
                    {
                        DirectoryInfo pluginDirInfo = new DirectoryInfo(Common.CombinePaths(subdirinfo.FullName, "Plugins"));

                        foreach (FileInfo fileinfo in pluginDirInfo.GetFiles())
                        {
                            if (fileinfo.Extension == ".dll" || fileinfo.Extension == ".pdb")
                            {
                                string destination = Common.CombinePaths(m_Process.Root, "Bin", fileinfo.Name);
                                if (!File.Exists(destination) || fileinfo.LastWriteTime != File.GetLastWriteTime(destination))
                                {
                                    File.Copy(fileinfo.FullName, Common.CombinePaths(m_Process.Root, "Bin", fileinfo.Name), true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSnippets(string[] Paths)
        {

            string pathsnippets = m_Process.Settings["general/customrootcomponents"] +"\\snippets.xslt"; // TODO: should be more generic

            StringBuilder stringB = new StringBuilder();
            stringB.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringB.AppendLine("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            foreach (string dir in Paths)
            {
                DirectoryInfo dirinfo = new DirectoryInfo(dir);

            
                foreach (DirectoryInfo subdirinfo in dirinfo.GetDirectories())
                {
                    if (!(subdirinfo.Name == ".svn"))
                    {
                        if (Directory.Exists(Common.CombinePaths(subdirinfo.FullName, "Xsl")))
                        {
                            DirectoryInfo xslDirInfo = new DirectoryInfo(Common.CombinePaths(subdirinfo.FullName, "Xsl"));

                            foreach (FileInfo fileinfo in xslDirInfo.GetFiles())
                            {
                                if ((fileinfo.Extension == ".xslt" || fileinfo.Extension == ".xsl") && fileinfo.Name[0] == '_')
                                {
                                    stringB.AppendLine("<xsl:include href=\"..\\..\\" + dirinfo.Parent.Name + "\\" + dirinfo.Name+ "\\" + subdirinfo.Name + "\\" + xslDirInfo.Name + "\\" + fileinfo.Name + "\"/>");
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
            control["adminmenu"] = m_Process.Settings.GetAsNode("admin/menu");
        }
    }
}
