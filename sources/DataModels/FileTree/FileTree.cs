//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.FileTree
{
    public class FileTree
    {
        private readonly Process process;
        private readonly string rootFilesPath;

        public FileTree(Process process)
        {
            this.process = process;
            rootFilesPath = Path.Combine(this.process.Root, this.process.Settings["filetree/filesPath"]);
            RootFolder = new FolderElement(rootFilesPath);
        }

        public FolderElement RootFolder { get; private set; }

        public bool FolderExists(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(rootFilesPath, path);
            return Directory.Exists(combinedPath);
        }

        public bool FileExists(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(rootFilesPath, path);
            return File.Exists(combinedPath);
        }

        public void CreateFolder(string path, string name)
        {
            name = Common.CleanToSafeString(name);
            string combinedPath = Common.CheckedCombinePaths(rootFilesPath, path, name);
            Directory.CreateDirectory(combinedPath);
        }

        public void DeleteFolder(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(rootFilesPath, path);
            Common.DeleteDirectory(combinedPath);
        }

        public void DeleteFile(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(rootFilesPath, path);
            File.Delete(combinedPath);
        }

        public FolderElement GetFolder(string folder)
        {
            string path = Path.Combine(rootFilesPath, folder);
            return Directory.Exists(path)
                ? new FolderElement(path)
                : null;
        }

        public string[] SaveUploadedFiles(string path)
        {
            int fileCount = process.HttpPage.Request.Files.Count;
            string[] files = new string[fileCount];

            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
            {
                HttpPostedFile file = process.HttpPage.Request.Files[fileIndex];

                if (path != null && file.ContentLength > 0)
                {
                    string prepend = process.QueryOther["file_prepend"];
                    if (!string.IsNullOrEmpty(prepend))
                    {
                        prepend = prepend + "_";
                    }
                    else
                    {
                        prepend = string.Empty;
                    }

                    string filename = string.Join("_", Common.CleanToSafeString(Path.GetFileName(file.FileName)).Split(' '));
                    string fullName = Common.CombinePaths(rootFilesPath, path, prepend + filename);
                    if (Common.PathIsInSite(fullName) && filename != "")
                    {
                        file.SaveAs(fullName);
                        files[fileIndex] = fullName;
                    }
                }
                else if (path != null && file.ContentLength == 0 && file.FileName != string.Empty)
                {
                    process.AddMessage(
                        string.Format("The file \"{0}\" was ignored because it was empty.", file.FileName),
                        MessageType.Error);
                }
            }

            return files;
        }

        public void SendToBrowser(string filename)
        {
            HttpResponse response = process.HttpPage.Response;

            FileInfo fileInfo = new FileInfo(Path.Combine(rootFilesPath, filename));
            if (fileInfo.FullName.StartsWith(rootFilesPath))
            {
                if (fileInfo.Exists)
                {
                    response.Clear();
                    string currentFile = fileInfo.FullName;

                    int width;
                    int.TryParse(process.QueryOther["width"], out width);

                    int height;
                    int.TryParse(process.QueryOther["height"], out height);

                    int radius;
                    int.TryParse(process.QueryOther["radius"], out radius);

                    int borderwidth;
                    int.TryParse(process.QueryOther["borderwidth"], out borderwidth);

                    string bordercolor = ("#" + process.QueryOther["bordercolor"]);

                    bool isFixed = process.QueryOther["fixed"] == "true";
                    if (width > 0 || height > 0 || radius >= 0)
                    {
                        Bitmap bitmap = null;
                        string newFilename = string.Format("{0}_{1}x{2}r{3}",
                                                           fileInfo.Name.TrimEnd(fileInfo.Extension.ToCharArray()),
                                                           width, height, radius);

                        if (borderwidth > 0 && Regex.IsMatch(bordercolor, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
                        {
                            newFilename += ("bw" + borderwidth + "bc" + bordercolor);
                        }

                        newFilename += ("_" + isFixed + ".png");

                        if (fileInfo.Directory != null)
                        {
                            string thumbnailFile = Common.CombinePaths(fileInfo.Directory.FullName, "thumbs",
                                                                       newFilename);

                            DateTime rerender = new DateTime(2006, 9, 25);
                            FileInfo thumbInfo = null;
                            if (File.Exists(thumbnailFile))
                                thumbInfo = new FileInfo(thumbnailFile);

                            if (thumbInfo == null || thumbInfo.LastWriteTime < fileInfo.LastWriteTime || rerender > thumbInfo.LastWriteTime)
                            {
                                bitmap = new Bitmap(fileInfo.FullName);
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
                                bitmap = RoundImageVertex(bitmap, radius, borderwidth, bordercolor);
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
                                eps.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                                ImageCodecInfo ici = GetEncoderInfo("image/png");
                                bitmap.Save(thumbnailFile, ici, eps);
                                bitmap.Dispose();
                                currentFile = thumbnailFile;
                            }
                        }
                    }

                    // Send image to browser
                    FileInfo file = new FileInfo(currentFile);
                    if (process.QueryOther["download"] == "true")
                    {
                        response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    }
                    else
                    {
                        response.AddHeader("Content-Disposition", "filename=" + file.Name);
                    }
                    response.AddHeader("Content-Length", file.Length.ToString());
                    response.AddHeader("Content-Type", Common.GetMimeType(file.Extension));

                    response.Cache.SetExpires(DateTime.Now + new TimeSpan(7, 0, 0, 0));
                    response.Cache.SetCacheability(HttpCacheability.Public);

                    response.WriteFile(currentFile);
                    response.Flush();
                    response.End();
                }
            }
        }

        private static Bitmap ResizeOrCropImage(Image bitmap, int width, int height, bool isFixed)
        {
            if (width >= bitmap.Width && height >= bitmap.Height && !isFixed)
            {
                return null;
            }

            Image tmpImage = bitmap;

            // Crop
            if (width > 0 && height > 0)
            {
                tmpImage = isFixed
                               ? ImageResize.FixedSize(tmpImage, width, height, Color.White)
                               : ImageResize.Crop(tmpImage, width, height, ImageResize.AnchorPosition.Center);
            }

            // Resize
            if (width > 0 && width < bitmap.Width)
            {
                tmpImage = ImageResize.ConstrainProportions(tmpImage, width, ImageResize.Dimensions.Width);
            }
            else if (height > 0 && height < bitmap.Height)
            {
                tmpImage = ImageResize.ConstrainProportions(tmpImage, height, ImageResize.Dimensions.Height);
            }

            return (Bitmap)tmpImage;
        }

        private static Bitmap RoundImageVertex(Bitmap bitmap, int radius, int borderWidth, string borderColor)
        {
            if (radius <= 0 || (radius * 2) > bitmap.Height || (radius * 2) > bitmap.Width)
            {
                return bitmap;
            }

            Image tmpImage = bitmap;

            if (!Regex.IsMatch(borderColor, @"[#]([0-9]|[a-f]|[A-F]){6}\b") || borderWidth < 1)
            {
                tmpImage = ImageVertexRounding.RoundedRectangle(tmpImage, radius);
            }
            else
            {
                tmpImage = ImageVertexRounding.RoundedRectangle(tmpImage, radius, borderWidth, borderColor);
            }

            return (Bitmap)tmpImage;
        }

        public void MoveFolder(string folder, string newContainingDirectory)
        {
            Common.MoveDirectory(Common.CombinePaths(rootFilesPath, folder),
                                 Common.CombinePaths(rootFilesPath, newContainingDirectory));
        }

        public void MoveFile(string file, string newContainingDiretory)
        {
            Common.MoveFile(Common.CombinePaths(rootFilesPath, file),
                            Common.CombinePaths(rootFilesPath, newContainingDiretory));
        }

        public void RenameFile(string file, string newFileName)
        {
            string filePath = Common.CheckedCombinePaths(rootFilesPath, file);
            FileInfo fileInfo = new FileInfo(filePath);
            string filePathNew = Common.CheckedCombinePaths(fileInfo.DirectoryName, newFileName);

            fileInfo.MoveTo(filePathNew);
        }

        public void RenameFolder(string folder, string newFolderName)
        {
            string folderPath = Common.CheckedCombinePaths(rootFilesPath, folder);

            DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
            if (folderInfo.Parent != null)
            {
                string folderPathNew = Common.CheckedCombinePaths(folderInfo.Parent.FullName, newFolderName);

                folderInfo.MoveTo(folderPathNew);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }

            return null;
        }
    }

    public class FolderElement
    {
        private readonly DirectoryInfo directoryInfo;

        public FolderElement(string path)
        {
            directoryInfo = new DirectoryInfo(path);
        }

        private string Name
        {
            get { return directoryInfo.Name; }
        }

        private static bool Filter(string name)
        {
            const string illegal = "._";
            return !illegal.Contains(name[0].ToString());
        }

        public void GetXml(XmlNode xmlNode, SubFolder subFolder)
        {
            XmlNode folderNode = CommonXml.GetNode(xmlNode, "folder", EmptyNodeHandling.ForceCreateNew);
            CommonXml.SetAttributeValue(folderNode, "name", Name);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                XmlNode fileNode = CommonXml.GetNode(folderNode, "file", EmptyNodeHandling.ForceCreateNew);
                CommonXml.SetAttributeValue(fileNode, "name", file.Name);
                CommonXml.SetAttributeValue(fileNode, "extension", file.Extension);
                GetFileAttributes(fileNode, file.Name); 
            }

            if (subFolder == SubFolder.IncludeSubfolders)
            {
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    if (Filter(dir.Name))
                    {
                        FolderElement folderElement = new FolderElement(dir.FullName);
                        folderElement.GetXml(folderNode, SubFolder.IncludeSubfolders);
                    }
                }
            }
        }

        private void GetFileAttributes(XmlNode xmlNode, String fileName) 
        {
            DataRow[] dr;
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(directoryInfo.FullName + "\\data\\gallery.xml");
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