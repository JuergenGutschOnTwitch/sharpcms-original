// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.IO;
using System.Xml;
using Sharpcms.Data.FileTree;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderFile : BasePlugin2, IPlugin2
    {
        private FileTree _fileTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFile"/> class.
        /// </summary>
        public ProviderFile() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFile"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public ProviderFile(Process process)
        {
            Process = process;
        }

        /// <summary>
        /// Gets the tree.
        /// </summary>
        /// <value>The tree.</value>
        private FileTree Tree
        {
            get
            {
                FileTree fileTree = _fileTree ?? (_fileTree = new FileTree(Process));

                return fileTree;
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
            if (string.IsNullOrEmpty(value)) return;

            Process.OutputHandledByModule = true;
            Tree.SendToBrowser(value);
        }

        /// <summary>
        /// Handles the move folder.
        /// </summary>
        private void HandleMoveFolder()
        {
            string[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.MoveFolder(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the move file.
        /// </summary>
        private void HandleMoveFile()
        {
            string[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.MoveFile(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the rename folder.
        /// </summary>
        private void HandleRenameFolder()
        {
            string[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.RenameFolder(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the rename file.
        /// </summary>
        private void HandleRenameFile()
        {
            string[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.RenameFile(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the remove folder.
        /// </summary>
        private void HandleRemoveFolder()
        {
            string path = Process.QueryEvents["mainvalue"];

            FileTree filetree = new FileTree(Process);
            filetree.DeleteFolder(path);
        }

        /// <summary>
        /// Handles the remove file.
        /// </summary>
        private void HandleRemoveFile()
        {
            string path = Process.QueryEvents["mainvalue"];

            FileTree filetree = new FileTree(Process);
            filetree.DeleteFile(path);
        }

        /// <summary>
        /// Handles the add folder.
        /// </summary>
        private void HandleAddFolder()
        {
            string query = Process.QueryEvents["mainvalue"];
            string[] list = query.Split('*');

            FileTree filetree = new FileTree(Process);
            filetree.CreateFolder(list[0], list[1]);
        }

        /// <summary>
        /// Handles the upload.
        /// </summary>
        private void HandleUpload()
        {
            string query = Process.QueryEvents["mainvalue"];

            FileTree fileTree = new FileTree(Process);
            fileTree.SaveUploadedFiles(query);
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