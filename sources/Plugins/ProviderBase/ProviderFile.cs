//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using InventIt.SiteSystem.Data.FileTree;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;


namespace InventIt.SiteSystem.Providers
{
    public class ProviderFile : BasePlugin2, IPlugin2
    {
        private FileTree m_SiteTree;

        public FileTree Tree
        {
            get
            {
                if (m_SiteTree == null)
                {
                    m_SiteTree = new FileTree(m_Process);
                }
                return m_SiteTree;
            }
        }

        public new string Name
        {
            get { return "File"; }
        }

        public ProviderFile()
        {
        }

        public ProviderFile(Process process)
        {
            m_Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "addfolder":
                    HandleAddFolder();
                    break;
                case "removefolder":
                    HandleRemoveFolder();
                    break;
                case "removefile":
                    HandleRemoveFile();
                    break;
                case "uploadfile":
                    HandleUpload();
                    break;
                case "renamefile":
                    HandleRenameFile();
                    break;
                case "renamefolder":
                    HandleRenameFolder();
                    break;
                case "movefolder":
                    HandleMoveFolder();
                    break;
                case "movefile":
                    HandleMoveFile();
                    break;
            }
        }

        private void HandleMoveFolder()
        {
            string[] par = m_Process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                FileTree filetree = new FileTree(m_Process);
                filetree.MoveFolder(par[0], par[1]);
            }
        }

        private void HandleMoveFile()
        {
            string[] par = m_Process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                FileTree filetree = new FileTree(m_Process);
                filetree.MoveFile(par[0], par[1]);
            }
        }

        private void HandleRenameFolder()
        {
            string[] par = m_Process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                FileTree filetree = new FileTree(m_Process);
                filetree.RenameFolder(par[0], par[1]);
            }
        }

        private void HandleRenameFile()
        {
            string[] par = m_Process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                FileTree filetree = new FileTree(m_Process);
                filetree.RenameFile(par[0], par[1]);
            }
        }

        private void HandleRemoveFolder()
        {
            string path = m_Process.QueryEvents["mainvalue"];

            FileTree filetree = new FileTree(m_Process);
            filetree.DeleteFolder(path);
        }

        private void HandleRemoveFile()
        {
            string path = m_Process.QueryEvents["mainvalue"];

            FileTree filetree = new FileTree(m_Process);
            filetree.DeleteFile(path);
        }

        private void HandleAddFolder()
        {
            string query = m_Process.QueryEvents["mainvalue"];
            string[] a_list = query.Split('*');
            FileTree filetree = new FileTree(m_Process);
            filetree.CreateFolder(a_list[0], a_list[1]);
        }

        private void HandleUpload()
        {
            string query = m_Process.QueryEvents["mainvalue"];
            FileTree fileTree = new FileTree(m_Process);
            string[] files = fileTree.SaveUploadedFiles(query);
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "tree":
                    LoadTree(pathTrail, control);
                    break;
                case "folder":
                    LoadFolder(pathTrail, control);
                    break;
                case "file":
                    LoadFile(pathTrail, control);
                    break;
                case "download":
                    LoadDownload(pathTrail, control);
                    break;
            }
        }

        private void LoadFile(string value, ControlList control)
        {
            if (value != null && value != "")
            {
                string path = value;
                if (Tree.FileExists(path))
                {
                    XmlNode xmlNode = control["file"];

                    string extension = Path.GetExtension(path);
                    CommonXml.SetAttributeValue(xmlNode, "extension", extension);

                    string mainMimeType = Common.GetMainMimeType(extension);
                    CommonXml.SetAttributeValue(xmlNode, "mainmimetype", mainMimeType);

                    CommonXml.SetAttributeValue(xmlNode, "path", path);
                }
            }
        }

        private void LoadFolder(string value, ControlList control)
        {
            if (value != null && value != "")
            {
                string path = value;
                if (Tree.FolderExists(path))
                {
                    FolderElement folderElement = Tree.GetFolder(path);
                    XmlNode xmlNode = control["folder"];
                    folderElement.GetXml(xmlNode, SubFolder.OnlyThisFolder);
                    CommonXml.SetAttributeValue(xmlNode, "path", path);
                }
            }
        }

        private void LoadTree(string pathTrail, ControlList control)
        {
            Tree.RootFolder.GetXml(control["filetree"], SubFolder.IncludeSubfolders);
        }

        private void LoadDownload(string value, ControlList control)
        {
            if (value != null && value.Length > 0)
            {
                m_Process.OutputHandledByModule = true;
                Tree.SendToBrowser(value);
            }
        }

    }

}