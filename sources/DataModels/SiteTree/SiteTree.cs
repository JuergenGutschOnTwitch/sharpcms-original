// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.IO;
using System.Xml;
using Sharpcms.Library.Common;
using Sharpcms.Library.Process;

namespace Sharpcms.Data.SiteTree
{
    public class SiteTree
    {
        private readonly string _contentFilenameFormat;
        private readonly string _contentRoot;
        private readonly Process _process;
        private readonly XmlDocument _treeDocument;
        private readonly string _treeFilename;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteTree"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public SiteTree(Process process)
        {
            _process = process;
            _contentRoot = process.Settings["sitetree/contentRoot"];
            _contentFilenameFormat = process.Settings["sitetree/contentFilenameFormat"];
            _treeFilename = process.Settings["sitetree/treeFilename"];
            _treeDocument = new XmlDocument();
            _treeDocument.Load(_treeFilename);
        }

        /// <summary>
        /// Gets the tree document.
        /// </summary>
        /// <value>The tree document.</value>
        public XmlDocument TreeDocument
        {
            get { return _treeDocument; }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Page GetPage(string path)
        {
            XmlNode pageNode = GetPageNode(path);

            return GetPage(pageNode);
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="pageNode">The page node.</param>
        /// <returns></returns>
        private Page GetPage(XmlNode pageNode)
        {
            if (pageNode == null)
            {
                return null;
            }

            string path = CommonXml.GetXPath(pageNode);

            string fileName = Path.Combine(_contentRoot, string.Format(_contentFilenameFormat, path));
            if (!File.Exists(fileName))
            {
                var pagePath = new PagePath(path);
                CreateFile(pagePath.Path, pagePath.Name, CommonXml.GetAttributeValue(pageNode, "menuname"));
            }

            var document = new XmlDocument();
            document.Load(fileName);

            var page = new Page(document.SelectSingleNode("//page"), GetPageNode(path), this);

            return page;
        }

        /// <summary>
        /// Gets the page node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private XmlNode GetPageNode(string path)
        {
            //string xPath = NormalizePath(path);
            XmlNode pageNode = CommonXml.GetNode(_treeDocument, path, EmptyNodeHandling.Ignore);

            return pageNode;
        }

        /// <summary>
        /// Existses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            XmlNode pageNode = GetPageNode(path);
            return pageNode != null;
        }

        /// <summary>
        /// Saves a page.
        /// </summary>
        /// <param name="page">The page to save.</param>
        public void SavePage(Page page)
        {
            SavePage(page, true);
        }

        /// <summary>
        /// Saves the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="saveTree">if set to <c>true</c> [save tree].</param>
        private void SavePage(Page page, bool saveTree)
        {
            if (page == null) return;

            string filename = GetDocumentFilename(page.TreeNode);
            CommonXml.SaveXmlDocument(filename, page.Node.OwnerDocument);

            if (saveTree)
            {
                Save();
            }
        }

        /// <summary>
        /// Saves the tree.
        /// </summary>
        public void Save()
        {
            Rebuild();
            CommonXml.SaveXmlDocument(_treeFilename, _treeDocument);
        }

        /// <summary>
        /// Rebuilds this instance.
        /// </summary>
        private void Rebuild()
        {
            XmlNodeList xmlNodeList = TreeDocument.SelectNodes("//*[@pageidentifier and not(@pageidentifier = '')]");

            if (xmlNodeList == null) return;

            foreach (XmlNode pageNode in xmlNodeList)
            {
                string path = CommonXml.GetXPath(pageNode);
                Page page = GetPage(path);
                page["pageidentifier"] = path;
                SavePage(page, false);
                CommonXml.SetAttributeValue(pageNode, "pageidentifier", path);
            }
        }

        #region Copy Page

        public void CopyTo(string mainValue)
        {
            string[] parts = mainValue.Split('¤');
            string copyFromPath = parts[0];
            string copyToPath = parts[1];
            string name = parts[2];

            if (copyFromPath.Trim() == string.Empty || copyToPath.Trim() == string.Empty)
            {
                _process.AddMessage("An error occured while copying the page.");
                return;
            }

            Page copyFrom = GetPage(copyFromPath);
            Page copyTo = GetPage(copyToPath);

            if (name.Trim() == string.Empty)
            {
                name = string.Format("Copy of {0}", copyFrom.Name);
            }

            Page newPage = Create(copyTo.PageIdentifier, name, name);

            newPage.Containers.ParentNode.InnerXml = copyFrom.Containers.ParentNode.InnerXml;
            newPage.Save();

            Save();
        }

        #endregion

        #region Create Page

        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        public Page Create(string path, string name, string menuName)
        {
            name = Common.CleanToSafeString(name);

            // Only lowercase URLs are accepted
            path = path.ToLower();
            name = name.ToLower();

            if (GetPageNode(path + "/" + name) != null)
            {
                return GetPage(path + "/" + name); // The requested page already exists.
            }

            // Update filesystem
            string pathfile = path;
            if (pathfile == ".")
            {
                pathfile = string.Empty;
            }

            CreateFile(pathfile, name, menuName);

            // Update Xml
            XmlNode pageNode = CreateInTree(path, name, menuName);

            Save();

            Page page = GetPage(pageNode);
            InitializeCreatedPage(page);
            return page;
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="menuName">Name of the menu.</param>
        private void CreateFile(string path, string name, string menuName)
        {
            string fileDirectory = GetDocumentDirectory(path);
            string filename = GetDocumentFilename(path, name);
            Directory.CreateDirectory(fileDirectory);

            var document = new XmlDocument();
            CommonXml.GetNode(document, "page/attributes/pagename", EmptyNodeHandling.CreateNew).InnerText = menuName;
            CommonXml.SaveXmlDocument(filename, document);
        }

        /// <summary>
        /// Creates the in tree.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        private XmlNode CreateInTree(string path, string name, string menuName)
        {
            XmlNode parentNode = GetPageNode(path);
            XmlNode pageNode = _treeDocument.CreateElement(name);
            parentNode.AppendChild(pageNode);

            CommonXml.AppendAttribute(pageNode, "menuname", menuName);
            return pageNode;
        }

        /// <summary>
        /// Initializes the created page.
        /// </summary>
        /// <param name="page">The page.</param>
        private void InitializeCreatedPage(Page page)
        {
            XmlNode xmlNodeContainer = _process.Settings.GetAsNode("sitetree/containers");
            if (xmlNodeContainer.ChildNodes.Count > 0)
            {
                foreach (XmlNode xmlNode in xmlNodeContainer.ChildNodes)
                {
                    Container container = page.Containers[xmlNode.InnerText];
                    // ToDo: hack to secure content container
                }                
            }
            else
            {
                Container container = page.Containers["content"];
                // ToDo: secures older websites - goes obsoletet
            }

            page["status"] = "hide";
            page.Save();
        }

        #endregion

        #region Delete Page

        public void Delete(string path)
        {
            Delete(GetPageNode(path));
        }

        private void Delete(XmlNode pageNode)
        {
            if (pageNode == null) return;

            // Update filesystem
            DeleteFile(pageNode);

            // Update Xml
            DeleteInTree(pageNode);

            Save();
        }

        private void DeleteFile(XmlNode pageNode)
        {
            string path = CommonXml.GetXPath(pageNode);
            var pagePath = new PagePath(path);

            Common.DeleteFile(GetDocumentFilename(pagePath.Path, pagePath.Name));
            Common.DeleteDirectory(GetDocumentContainerDirectory(pagePath.Path, pagePath.Name));
        }

        private static void DeleteInTree(XmlNode pageNode)
        {
            if (pageNode == null) return;
            if (pageNode.ParentNode == null)  return;
            
            pageNode.ParentNode.RemoveChild(pageNode);
        }

        #endregion

        #region Rename page

        public void Rename(Page page, string renameTo)
        {
            renameTo = Common.CleanToSafeString(renameTo);
            string oldName = page.TreeNode.Name;

            if (oldName == renameTo) return;

            // Update filesystem
            RenameFile(page, renameTo);

            // Update Xml
            RenameInTree(page, renameTo);

            Save();
        }

        private void RenameFile(Page page, string renameTo)
        {
            string path = CommonXml.GetXPath(page.TreeNode);
            var pagePath = new PagePath(path);

            // Rename file
            string oldFilename = GetDocumentFilename(pagePath.Path, pagePath.Name);
            string newFilename = GetDocumentFilename(pagePath.Path, renameTo);
            File.Move(oldFilename, newFilename);

            // Rename directory
            string oldDirectory = GetDocumentContainerDirectory(pagePath.Path, pagePath.Name);
            if (Directory.Exists(oldDirectory))
            {
                string newDirectory = GetDocumentContainerDirectory(pagePath.Path, renameTo);
                Directory.Move(oldDirectory, newDirectory);
            }
        }

        private void RenameInTree(Page page, string renameTo)
        {
            renameTo = Common.CleanToSafeString(renameTo);

            XmlNode newTreeNode = _treeDocument.CreateElement(renameTo);

            // Copy children and attributes
            newTreeNode.InnerXml = page.TreeNode.InnerXml;
            CommonXml.CopyAttributes(page.TreeNode, newTreeNode);

            // Replace old node with new
            XmlNode parentNode = page.TreeNode.ParentNode;
            if (parentNode != null)
            {
                parentNode.ReplaceChild(newTreeNode, page.TreeNode);
            }
        }

        #endregion

        #region Move up / down

        public void MoveUp(string path)
        {
            Page page = GetPage(path);
            MoveUp(page);
        }

        private void MoveUp(Page page)
        {
            CommonXml.MoveUp(page.TreeNode);
            Save();
        }

        public void MoveDown(string path)
        {
            Page page = GetPage(path);
            MoveDown(page);
        }

        private void MoveDown(Page page)
        {
            CommonXml.MoveDown(page.TreeNode);
            Save();
        }

        private void MoveTop(Page page)
        {
            CommonXml.MoveTop(page.TreeNode);
            Save();
        }

        public void MoveTop(string path)
        {
            Page page = GetPage(path);
            MoveTop(page);
        }

        private void MoveBottom(Page page)
        {
            CommonXml.MoveBottom(page.TreeNode);
            Save();
        }

        public void MoveBottom(string path)
        {
            Page page = GetPage(path);
            MoveBottom(page);
        }

        #endregion

        #region Move

        public void Move(string path, string newParentPath)
        {
            Page page = GetPage(path);
            Page newParent = GetPage(newParentPath);
            Move(page, newParent);
        }

        private void Move(Page page, Page newParent)
        {
            if (page.Node != newParent.Node && !newParent.PageIdentifier.StartsWith(page.PageIdentifier))
            {
                string sourcePath = page.PageIdentifier;
                string newParentPath = newParent.PageIdentifier;

                CommonXml.Move(page.TreeNode, newParent.TreeNode);
                Save();

                string newContainerDirectory = GetDocumentDirectory(newParentPath.Replace("/", @"\"));

                string sourceDirectory = GetDocumentDirectory(sourcePath.Replace("/", @"\"));
                string sourceFile = GetDocumentFilename(sourcePath.Replace("/", @"\"));

                // Ensure that the container directory exists
                Directory.CreateDirectory(newContainerDirectory);

                // Move file
                Common.MoveFile(sourceFile, newContainerDirectory);

                // Copy directory
                if (Directory.Exists(sourceDirectory))
                {
                    Common.CopyDirectory(sourceDirectory, Path.Combine(newContainerDirectory, page.Name), true);
                }

                // (Try to) Remove original directory
                try
                {
                    Common.DeleteDirectory(sourceDirectory);
                }
                catch
                {
                    /* Ignore errors */
                }

                Save();
            }
            else
            {
                throw new Exception("Cannot move page to child");
            }
        }

        #endregion

        #region Directory and filename handling

        private static string GetDocumentContainerDirectory(string path, string name)
        {
            return Path.Combine(path, name);
        }

        private string GetDocumentDirectory(string path)
        {
            string directory = Path.Combine(_contentRoot, path);

            return directory;
        }

        private string GetDocumentFilename(XmlNode treeNode)
        {
            string path = CommonXml.GetXPath(treeNode);

            return GetDocumentFilename(path);
        }

        private string GetDocumentFilename(string fullPath)
        {
            var pagePath = new PagePath(fullPath);

            return GetDocumentFilename(pagePath.Path, pagePath.Name);
        }

        private string GetDocumentFilename(string path, string name)
        {
            string directory = GetDocumentDirectory(path);

            return Path.Combine(directory, string.Format(_contentFilenameFormat, name));
        }

        #endregion
    }
}