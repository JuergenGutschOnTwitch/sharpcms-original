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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFile"/> class.
        /// </summary>
        public ProviderFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFile"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public ProviderFile(Process process)
        {
            _process = process;
        }

        /// <summary>
        /// Gets the tree.
        /// </summary>
        /// <value>The tree.</value>
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

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public new string Name
        {
            get { return "File"; }
        }

        /// <summary>
        /// Handles the specified main event.
        /// </summary>
        /// <param name="mainEvent">The main event.</param>
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

        /// <summary>
        /// Loads the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
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

        /// <summary>
        /// Loads the download.
        /// </summary>
        /// <param name="value">The value.</param>
        private void LoadDownload(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _process.OutputHandledByModule = true;
                Tree.SendToBrowser(value);
            }
        }

        /// <summary>
        /// Handles the move folder.
        /// </summary>
        private void HandleMoveFolder()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.MoveFolder(par[0], par[1]);
            }
        }

        /// <summary>
        /// Handles the move file.
        /// </summary>
        private void HandleMoveFile()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.MoveFile(par[0], par[1]);
            }
        }

        /// <summary>
        /// Handles the rename folder.
        /// </summary>
        private void HandleRenameFolder()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.RenameFolder(par[0], par[1]);
            }
        }

        /// <summary>
        /// Handles the rename file.
        /// </summary>
        private void HandleRenameFile()
        {
            string[] par = _process.QueryEvents["mainvalue"].Split('*');
            if (par.Length == 2 && par[1].Length > 0 && par[0].Length > 0)
            {
                var filetree = new FileTree(_process);
                filetree.RenameFile(par[0], par[1]);
            }
        }

        /// <summary>
        /// Handles the remove folder.
        /// </summary>
        private void HandleRemoveFolder()
        {
            string path = _process.QueryEvents["mainvalue"];

            var filetree = new FileTree(_process);
            filetree.DeleteFolder(path);
        }

        /// <summary>
        /// Handles the remove file.
        /// </summary>
        private void HandleRemoveFile()
        {
            string path = _process.QueryEvents["mainvalue"];

            var filetree = new FileTree(_process);
            filetree.DeleteFile(path);
        }

        /// <summary>
        /// Handles the add folder.
        /// </summary>
        private void HandleAddFolder()
        {
            string query = _process.QueryEvents["mainvalue"];
            string[] list = query.Split('*');
            var filetree = new FileTree(_process);
            filetree.CreateFolder(list[0], list[1]);
        }

        /// <summary>
        /// Handles the upload.
        /// </summary>
        private void HandleUpload()
        {
            string query = _process.QueryEvents["mainvalue"];
            var fileTree = new FileTree(_process);
            string[] files = fileTree.SaveUploadedFiles(query); //ToDo: ??? (T.Huber 18.06.2009)
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="control">The control.</param>
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

        /// <summary>
        /// Loads the folder.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="control">The control.</param>
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

        /// <summary>
        /// Loads the tree.
        /// </summary>
        /// <param name="control">The control.</param>
        private void LoadTree(ControlList control)
        {
            Tree.RootFolder.GetXml(control["filetree"], SubFolder.IncludeSubfolders);
        }
    }
}