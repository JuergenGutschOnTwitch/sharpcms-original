//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.IO;
using System.Xml;
using InventIt.SiteSystem.Data.FileTree;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderFile : BasePlugin2, IPlugin2
    {
        private FileTree _siteTree;

        public ProviderFile()
        {
        }

        public ProviderFile(Process process)
        {
            _process = process;
        }

        private FileTree Tree
        {
            get
            {
                if (_siteTree == null)
                    _siteTree = new FileTree(_process);

                return _siteTree;
            }
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "File"; }
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

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "tree":
                    LoadTree(control);
                    break;
                case "folder":
                    LoadFolder(pathTrail, control);
                    break;
                case "file":
                    LoadFile(pathTrail, control);
                    break;
                case "download":
                    LoadDownload(pathTrail);
                    break;
            }
        }

        #endregion

        private void LoadDownload(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _process.OutputHandledByModule = true;
                Tree.SendToBrowser(value);
            }
        }

        private void HandleMoveFolder()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.MoveFolder(par[0], par[1]);
            }
        }

        private void HandleMoveFile()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.MoveFile(par[0], par[1]);
            }
        }

        private void HandleRenameFolder()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.RenameFolder(par[0], par[1]);
            }
        }

        private void HandleRenameFile()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.RenameFile(par[0], par[1]);
            }
        }

        private void HandleRemoveFolder()
        {
            string path = _process.QueryEvents["mainvalue"];

            var filetree = new FileTree(_process);
            filetree.DeleteFolder(path);
        }

        private void HandleRemoveFile()
        {
            string path = _process.QueryEvents["mainvalue"];

            var filetree = new FileTree(_process);
            filetree.DeleteFile(path);
        }

        private void HandleAddFolder()
        {
            string query = _process.QueryEvents["mainvalue"];
            string[] list = query.Split('*');
            var filetree = new FileTree(_process);
            filetree.CreateFolder(list[0], list[1]);
        }

        private void HandleUpload()
        {
            string query = _process.QueryEvents["mainvalue"];
            var fileTree = new FileTree(_process);
            string[] files = fileTree.SaveUploadedFiles(query); //ToDo: ??? (T.Huber 18.06.2009)
        }

        private void LoadFile(string value, ControlList control)
        {
            if (!string.IsNullOrEmpty(value))
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
            if (!string.IsNullOrEmpty(value))
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

        private void LoadTree(ControlList control)
        {
            Tree.RootFolder.GetXml(control["filetree"], SubFolder.IncludeSubfolders);
        }
    }
}