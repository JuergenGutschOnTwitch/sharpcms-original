using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.FileTree
{
    public class FileTree
    {
        private Process m_Process;
        private string m_RootFilesPath;
        private FolderElement m_RootFolder;

        public FolderElement RootFolder
        {
            get
            {
                return m_RootFolder;
            }
            set
            {
                m_RootFolder = value;
            }
        }

        public bool FolderExists(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(m_RootFilesPath, path);
            return Directory.Exists(combinedPath);
        }

        public bool FileExists(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(m_RootFilesPath, path);
            return File.Exists(combinedPath);
        }

        public void CreateFolder(string path, string name)
        {
            name = Common.CleanToSafeString(name);
            string combinedPath = Common.CheckedCombinePaths(m_RootFilesPath, path, name);
            Directory.CreateDirectory(combinedPath);
        }

        public void DeleteFolder(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(m_RootFilesPath, path);
            Common.DeleteDirectory(combinedPath);
        }

        public void DeleteFile(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(m_RootFilesPath, path);
            File.Delete(combinedPath);
        }

        public FileTree(Process process)
        {
            m_Process = process;
            m_RootFilesPath = Path.Combine(m_Process.Root, m_Process.Settings["filetree/filesPath"]);
            m_RootFolder = new FolderElement(m_RootFilesPath);
        }

        public FolderElement GetFolder(string folder)
        {
            string path = Path.Combine(m_RootFilesPath, folder);
            if (Directory.Exists(path))
            {
                return new FolderElement(path);
            }
            else
            {
                return null;
            }
        }

        public string[] SaveUploadedFiles(string path)
        {
            int fileCount = m_Process.HttpPage.Request.Files.Count;
            string[] files = new string[fileCount];

            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
            {
                HttpPostedFile file = m_Process.HttpPage.Request.Files[fileIndex];

                if (path != null && file.ContentLength > 0)
                {
                    string prepend = m_Process.QueryOther["file_prepend"];
                    if (prepend != null && prepend != string.Empty)
                    {
                        prepend = prepend + "_";
                    }
                    else
                    {
                        prepend = string.Empty;
                    }

                    string filename = string.Join("_", Common.CleanToSafeString(Path.GetFileName(file.FileName)).Split(' '));
                    string fullName = Common.CombinePaths(m_RootFilesPath, path, prepend + filename);
                    if (Common.PathIsInSite(fullName) && filename != "")
                    {
                        file.SaveAs(fullName);
                        files[fileIndex] = fullName;
                    }
                }
                else if (path != null && file.ContentLength == 0 && file.FileName != string.Empty)
                {
                    m_Process.AddMessage(string.Format("The file \"{0}\" was ignored because it was empty.", file.FileName), MessageType.Error);
                }
            }

            return files;
        }

        public void SendToBrowser(string filename)
        {
            HttpResponse response = m_Process.HttpPage.Response;

            FileInfo fileInfo = new FileInfo(Path.Combine(m_RootFilesPath, filename));
            if (fileInfo.FullName.StartsWith(m_RootFilesPath))
            {
                if (fileInfo.Exists)
                {
                    response.Clear();
                    string currentFile = fileInfo.FullName;
                    string thumbnailFile;
                    int width = 0;
                    int.TryParse(m_Process.QueryOther["width"], out width);

                    int height = 0;
                    int.TryParse(m_Process.QueryOther["height"], out height);
                    bool isFixed = m_Process.QueryOther["fixed"] == "true";
                    if (width > 0 || height > 0)
                    {
                        System.Drawing.Bitmap bitmap = null;

                        string newFilename = string.Format("{0}_{1}x{2}_{3}.jpg",
                            fileInfo.Name.TrimEnd(fileInfo.Extension.ToCharArray()), width, height, isFixed);

                        thumbnailFile = Common.CombinePaths(fileInfo.Directory.FullName, "thumbs", newFilename);

                        // We changed the cropping and resizing code on April 28 2006
                        // - rerender all thumbnails before that
                        // - and changed it again in september
                        DateTime rerender = new DateTime(2006, 9, 25);
                        FileInfo thumbInfo = null;
                        if (File.Exists(thumbnailFile))
                        {
                            thumbInfo = new FileInfo(thumbnailFile);
                        }

                        if (thumbInfo == null || thumbInfo.LastWriteTime < fileInfo.LastWriteTime || rerender > thumbInfo.LastWriteTime)
                        {
                            bitmap = new System.Drawing.Bitmap(fileInfo.FullName);
                            if (fileInfo.Extension.ToLower() == ".gif")
                            {
                                Bitmap realBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                                for (int x = 0; x < bitmap.Width; x++)
                                {
                                    for (int y = 0; y < bitmap.Height; y++)
                                    {
                                        realBitmap.SetPixel(x, y, bitmap.GetPixel(x, y));
                                    }
                                }
                                bitmap.Dispose();
                                bitmap = realBitmap;
                            }

                            bitmap = ResizeOrCropImage(bitmap, width, height, isFixed);
                        }
                        else
                        {
                            currentFile = thumbnailFile;
                        }

                        if (bitmap != null)
                        {
                            if (!Directory.Exists(fileInfo.Directory + @"\thumbs"))
                            {
                                Directory.CreateDirectory(fileInfo.Directory + @"\thumbs");
                            }

                            EncoderParameters eps = new EncoderParameters(1);
                            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 95L);
                            ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
                            bitmap.Save(thumbnailFile, ici, eps);
                            bitmap.Dispose();
                            currentFile = thumbnailFile;
                        }
                    }

                    // Send image to browser
                    FileInfo file = new FileInfo(currentFile);
                    if (m_Process.QueryOther["download"] == "true")
                        response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    else
                        response.AddHeader("Content-Disposition", "filename=" + file.Name);
                    //response.ContentType = Common.GetMimeType(file.Extension);
                    response.AddHeader("Content-Length", file.Length.ToString());
                    response.AddHeader("Content-Type", Common.GetMimeType(file.Extension));

                    response.Cache.SetExpires(DateTime.Now + new TimeSpan(7, 0, 0, 0));
                    response.Cache.SetCacheability(HttpCacheability.Public);

                    //response.TransmitFile(currentFile);   //jig: com 2007-0-17
                    response.WriteFile(currentFile);        //jig: add 2007-0-17
                    response.Flush();
                    response.End();
                    //response.Close();
                }
            }
        }

        public Bitmap ResizeOrCropImage(Bitmap bitmap, int width, int height, bool isFixed)
        {
            if (width >= bitmap.Width && height >= bitmap.Height && !isFixed)
            {
                return null;
            }

            Image tmpImage = (Image)bitmap;
            // Crop
            if (width > 0 && height > 0)
            {
                if (isFixed)
                {
                    tmpImage = InventIt.SiteSystem.Library.ImageResize.FixedSize(tmpImage, width, height, Color.White);
                }
                else
                {
                    tmpImage = InventIt.SiteSystem.Library.ImageResize.Crop(tmpImage, width, height, ImageResize.AnchorPosition.Center);
                }
            }

            // Resize
            if (width > 0 && width < bitmap.Width)
            {
                tmpImage = InventIt.SiteSystem.Library.ImageResize.ConstrainProportions(tmpImage, width, ImageResize.Dimensions.Width);
          
            }
            else if (height > 0 && height < bitmap.Height)
            {
                tmpImage =InventIt.SiteSystem.Library.ImageResize.ConstrainProportions(tmpImage, height, ImageResize.Dimensions.Height);
            }
            return (Bitmap)tmpImage;
        }

        public void MoveFolder(string folder, string newContainingDirectory)
        {
            Common.MoveDirectory(Common.CombinePaths(m_RootFilesPath, folder), Common.CombinePaths(m_RootFilesPath, newContainingDirectory));
        }

        public void MoveFile(string file, string newContainingDiretory)
        {
            Common.MoveFile(Common.CombinePaths(m_RootFilesPath, file), Common.CombinePaths(m_RootFilesPath, newContainingDiretory));
        }

        public void RenameFile(string file, string newFileName)
        {
            string filePath = Common.CheckedCombinePaths(m_RootFilesPath, file);

            FileInfo fileInfo = new FileInfo(filePath);
            string filePathNew = Common.CheckedCombinePaths(fileInfo.DirectoryName, newFileName);

            fileInfo.MoveTo(filePathNew);
        }

        public void RenameFolder(string folder, string newFolderName)
        {
            string folderPath = Common.CheckedCombinePaths(m_RootFilesPath, folder);

            DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
            string folderPathNew = Common.CheckedCombinePaths(folderInfo.Parent.FullName, newFolderName);

            folderInfo.MoveTo(folderPathNew);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }

    public class FolderElement
    {
        private DirectoryInfo m_DirectoryInfo;

        public string Name
        {
            get
            {
                return m_DirectoryInfo.Name;
            }
        }

        public FolderElement(string path)
        {
            m_DirectoryInfo = new DirectoryInfo(path);
        }

        private bool Filter(string name)
        {
            string illegal = "._";
            if (illegal.Contains(name[0].ToString()))
            {
                return false;
            }
            return true;
        }

        public void GetXml(XmlNode xmlNode, SubFolder subFolder)
        {
            XmlNode folderNode = CommonXml.GetNode(xmlNode, "folder", EmptyNodeHandling.ForceCreateNew);
            CommonXml.SetAttributeValue(folderNode, "name", Name);

            foreach (FileInfo file in m_DirectoryInfo.GetFiles())
            {
                XmlNode fileNode = CommonXml.GetNode(folderNode, "file", EmptyNodeHandling.ForceCreateNew);
                CommonXml.SetAttributeValue(fileNode, "name", file.Name);
                CommonXml.SetAttributeValue(fileNode, "extension", file.Extension);
                GetFileAttributes(fileNode, file.Name); // jig: add 2007-09-17
            }

            if (subFolder == SubFolder.IncludeSubfolders)
            {
                foreach (DirectoryInfo dir in m_DirectoryInfo.GetDirectories())
                {
                    if (Filter(dir.Name))
                    {
                        FolderElement folderElement = new FolderElement(dir.FullName);
                        folderElement.GetXml(folderNode, SubFolder.IncludeSubfolders);
                    }
                }
            }
        }

        private void GetFileAttributes(XmlNode xmlNode, String fileName) // jig: add 2007-09-17
        {
            System.Data.DataSet ds;
            System.Data.DataRow[] dr;
            ds = new System.Data.DataSet();
            try
            {
                ds.ReadXml(m_DirectoryInfo.FullName + "\\data\\gallery.xml");
                if (ds.Tables["file"] == null) return;

                dr = ds.Tables["file"].Select("name='" + fileName + "'");
                if (dr.Length == 0) return;

                CommonXml.SetAttributeValue(xmlNode, "title", dr[0]["title"].ToString());
                CommonXml.SetAttributeValue(xmlNode, "description", dr[0]["description"].ToString());
            }
            catch 
            {
            }
            finally
            {
                ds.Dispose();
            }
        }
    }

    public enum SubFolder
    {
        IncludeSubfolders,
        OnlyThisFolder
    }
}