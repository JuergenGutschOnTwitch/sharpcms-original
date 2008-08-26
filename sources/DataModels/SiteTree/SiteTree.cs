//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using InventIt.SiteSystem.Data.SiteTree;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Library;
using System.IO;

namespace InventIt.SiteSystem.Data.SiteTree
{
	public class SiteTree
	{
		private Process m_Process;
		private XmlDocument m_TreeDocument;
		private string m_TreeFilename;
		private string m_ContentRoot;
		private string m_ContentFilenameFormat;

		public XmlDocument TreeDocument
		{
			get
			{
				return m_TreeDocument;
			}
		}

		public SiteTree(Process process)
		{
			m_Process = process;

			m_ContentRoot = process.Settings["sitetree/contentRoot"];
            m_ContentFilenameFormat = process.Settings["sitetree/contentFilenameFormat"];
            m_TreeFilename = process.Settings["sitetree/treeFilename"];

			m_TreeDocument = new XmlDocument();
			m_TreeDocument.Load(m_TreeFilename);
		}

		public Page GetPage(string path)
		{
			XmlNode pageNode = GetPageNode(path);
			return GetPage(pageNode);
		}

		public Page GetPage(XmlNode pageNode)
		{
			if (pageNode == null)
			{
				return null;
			}
			string path = CommonXml.GetXPath(pageNode);

			string fileName = Path.Combine(m_ContentRoot, string.Format(m_ContentFilenameFormat, path));
			if (!File.Exists(fileName))
			{
				PagePath pagePath = new PagePath(path);
				CreateFile(pagePath.Path, pagePath.Name, CommonXml.GetAttributeValue(pageNode, "menuname"));
			}

			XmlDocument document = new XmlDocument();
			document.Load(fileName);

			Page page = new Page(document.SelectSingleNode("//page"), GetPageNode(path), this);

			return page;
		}

		private XmlNode GetPageNode(string path)
		{
			string xPath = NormalizePath(path);
			XmlNode pageNode = CommonXml.GetNode(m_TreeDocument, path, EmptyNodeHandling.Ignore);
			return pageNode;
		}

		public bool Exists(string path)
		{
			XmlNode pageNode = GetPageNode(path);
			if (pageNode == null)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Saves a page.
		/// </summary>
		/// <param name="page">The page to save.</param>
        public void SavePage(Page page)
        {
            SavePage(page, true);
        }

        public void SavePage(Page page, bool saveTree)
        {
            if (page == null)
            {
                return;
            }

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
			CommonXml.SaveXmlDocument(m_TreeFilename, m_TreeDocument);
		}

        public void Rebuild()
        {
            foreach (XmlNode pageNode in TreeDocument.SelectNodes("//*[@pageidentifier and not(@pageidentifier = '')]"))
            {
                string path = CommonXml.GetXPath(pageNode);
                Page page = GetPage(path);
                page["pageidentifier"] = path;
                SavePage(page, false);

                CommonXml.SetAttributeValue(pageNode, "pageidentifier", path);
            }
        }

		private static string NormalizePath(string path)
		{
			if (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}
			return path;
        }

        #region Copy Page
        public void CopyTo(string main_value)
        {
            string[] parts = main_value.Split('¤');
            string copyFromPath = parts[0];
            string copyToPath = parts[1];
            string name = parts[2];

            if (copyFromPath.Trim() == string.Empty || copyToPath.Trim() == string.Empty)
            {
                m_Process.AddMessage("An error occured while copying the page.");
                return;
            }

            Page copyFrom = GetPage(copyFromPath);
            Page copyTo = GetPage(copyToPath);

            if (name.Trim() == string.Empty) {
                name = string.Format("Copy of {0}", copyFrom.Name);
            }

            Page newPage = Create(copyTo.PageIdentifier, name, name);

            newPage.Containers.ParentNode.InnerXml = copyFrom.Containers.ParentNode.InnerXml;
            newPage.Save();

            Save();
        }
        #endregion

        #region Create Page
        public Page Create(string path, string name, string menuName)
		{
            name = Common.CleanToSafeString(name);

			// Only lowercase URLs are accepted
			path = path.ToLower();
			name = name.ToLower();

			if (GetPageNode(path + "/" + name) != null)
			{
				// The requested page already exists.
				return GetPage(path + "/" + name);
			}

			// Update filesystem
            string pathfile = path;
            if (pathfile == ".")
            {
                pathfile = "";
            }

            CreateFile(pathfile, name, menuName);

			// Update Xml
			XmlNode pageNode = CreateInTree(path, name, menuName);

			Save();

			Page page = GetPage(pageNode);
			InitializeCreatedPage(page);
            return page;
		}

		private void CreateFile(string path, string name, string menuName)
		{
			string fileDirectory = GetDocumentDirectory(path);
			string filename = GetDocumentFilename(path, name);
			Directory.CreateDirectory(fileDirectory);

			XmlDocument document = new XmlDocument();
			CommonXml.GetNode(document, "page/attributes/pagename", EmptyNodeHandling.CreateNew).InnerText = menuName;
			CommonXml.SaveXmlDocument(filename, document);
		}

		private XmlNode CreateInTree(string path, string name, string menuName)
		{
			XmlNode parentNode = GetPageNode(path);

			XmlNode pageNode = m_TreeDocument.CreateElement(name);
			parentNode.AppendChild(pageNode);

			CommonXml.AppendAttribute(pageNode, "menuname", menuName);
			return pageNode;
		}

		private void InitializeCreatedPage(Page page)
		{
            XmlNode xmlNodeContainer = m_Process.Settings.GetAsNode("sitetree/containers");
            if (xmlNodeContainer.ChildNodes.Count >0 )
            {
                foreach (XmlNode xmlNode in xmlNodeContainer.ChildNodes)
                {
                    Container container = page.Containers[xmlNode.InnerText]; // todo: hack to secure content container.
                }
            }
            else
            {
                Container container = page.Containers["content"]; // todo: secures older websites - goes obsoletet
            }
			
            page["status"] = "open";
			page.Save();
		}
		#endregion

		#region Delete Page
		public void Delete(string path)
		{
			Delete(GetPageNode(path));
		}

		public void Delete(XmlNode pageNode)
		{
			if (pageNode == null)
			{
				return;
			}

			// Update filesystem
			DeleteFile(pageNode);

			// Update Xml
			DeleteInTree(pageNode);

			Save();
		}

		private void DeleteFile(XmlNode pageNode)
		{
			string path = CommonXml.GetXPath(pageNode);
			PagePath pagePath = new PagePath(path);

			Common.DeleteFile(GetDocumentFilename(pagePath.Path, pagePath.Name));
			Common.DeleteDirectory(GetDocumentContainerDirectory(pagePath.Path, pagePath.Name));
		}

		private static void DeleteInTree(XmlNode pageNode)
		{
			if (pageNode == null)
			{
				return;
			}
			pageNode.ParentNode.RemoveChild(pageNode);
		}
		#endregion

		#region Rename page
		public void Rename(Page page, string renameTo)
		{
            renameTo = Common.CleanToSafeString(renameTo);
			string oldName = page.TreeNode.Name;

			if (oldName == renameTo)
			{
				return;
			}

			// Update filesystem
			RenameFile(page, renameTo);

			// Update Xml
			RenameInTree(page, renameTo);

			Save();
		}

		private void RenameFile(Page page, string renameTo)
		{
			string path = CommonXml.GetXPath(page.TreeNode);
			PagePath pagePath = new PagePath(path);

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

			XmlNode newTreeNode = m_TreeDocument.CreateElement(renameTo);

			// Copy children and attributes
			newTreeNode.InnerXml = page.TreeNode.InnerXml;
			CommonXml.CopyAttributes(page.TreeNode, newTreeNode);

			// Replace old node with new
			XmlNode parentNode = page.TreeNode.ParentNode;
			parentNode.ReplaceChild(newTreeNode, page.TreeNode);
		}
		#endregion

		#region Move up / down
		public void MoveUp(string path)
        {
            Page page = GetPage(path);
            MoveUp(page);
        }
		
		public void MoveUp(Page page)
		{
			CommonXml.MoveUp(page.TreeNode);
			Save();
		}

		public void MoveDown(string path)
        {
            Page page = GetPage(path);
            MoveDown(page);
        }
        
		public void MoveDown(Page page)
		{
			CommonXml.MoveDown(page.TreeNode);
			Save();
		}
		#endregion

		#region Move
		public void Move(string path, string newParentPath)
		{
			Page page = GetPage(path);
            Page newParent = GetPage(newParentPath);
            Move(page, newParent);
		}

		public void Move(Page page, Page newParent)
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
            catch { /* Ignore errors */ }

            Save();
		}
		#endregion

		#region Directory and filename handling
		private string GetDocumentContainerDirectory(string path, string name)
		{
			return Path.Combine(path, name);
		}

		private string GetDocumentDirectory(string path)
		{
			string directory = Path.Combine(m_ContentRoot, path);
			return directory;
		}

		private string GetDocumentFilename(XmlNode treeNode)
		{
			string path = CommonXml.GetXPath(treeNode);
			return GetDocumentFilename(path);
		}

		private string GetDocumentFilename(string fullPath)
		{
			PagePath pagePath = new PagePath(fullPath);
			return GetDocumentFilename(pagePath.Path, pagePath.Name);
		}

		private string GetDocumentFilename(string path, string name)
		{
			string directory = GetDocumentDirectory(path);
			string filename = Path.Combine(directory, string.Format(m_ContentFilenameFormat, name));
			return filename;
		}

        private void RemoveSvnDirectories(string root)
        {
            string fullPath = Common.CheckedCombinePaths(root, ".svn");
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fullPath);
                if ((dirInfo.Attributes & FileAttributes.ReadOnly) != 0)
                {
                    dirInfo.Attributes = FileAttributes.Directory;
                }

                try
                {
                    Directory.Delete(fullPath, true);
                }
                catch
                {
                }
            }

            DirectoryInfo dir = new DirectoryInfo(root);
            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                if (directory.Name != ".svn")
                {
                    RemoveSvnDirectories(directory.FullName);
                }
            }
        }
		#endregion

	//	private static string 
	}
}