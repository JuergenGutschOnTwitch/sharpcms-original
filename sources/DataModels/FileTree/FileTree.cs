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
        private readonly Process _process;
        private readonly string _rootFilesPath;

        public FileTree(Process process)
        {
            _process = process;
            _rootFilesPath = Path.Combine(_process.Root, _process.Settings["filetree/filesPath"]);
            RootFolder = new FolderElement(_rootFilesPath);
        }

        public FolderElement RootFolder { get; private set; }

        public bool FolderExists(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            return Directory.Exists(combinedPath);
        }

        public bool FileExists(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            return File.Exists(combinedPath);
        }

        public void CreateFolder(string path, string name)
        {
            name = Common.CleanToSafeString(name);
            string combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path, name);
            Directory.CreateDirectory(combinedPath);
        }

        public void DeleteFolder(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            Common.DeleteDirectory(combinedPath);
        }

        public void DeleteFile(string path)
        {
            string combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            File.Delete(combinedPath);
        }

        public FolderElement GetFolder(string folder)
        {
            string path = Path.Combine(_rootFilesPath, folder);
            if (Directory.Exists(path))
                return new FolderElement(path);

            return null;
        }

        public string[] SaveUploadedFiles(string path)
        {
            int fileCount = _process.HttpPage.Request.Files.Count;
            var files = new string[fileCount];

            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
            {
                HttpPostedFile file = _process.HttpPage.Request.Files[fileIndex];

                if (path != null && file.ContentLength > 0)
                {
                    string prepend = _process.QueryOther["file_prepend"];
                    if (!string.IsNullOrEmpty(prepend))
                        prepend = prepend + "_";
                    else
                        prepend = string.Empty;

                    string filename = string.Join("_",
                                                  Common.CleanToSafeString(Path.GetFileName(file.FileName)).Split(' '));
                    string fullName = Common.CombinePaths(_rootFilesPath, path, prepend + filename);
                    if (Common.PathIsInSite(fullName) && filename != "")
                    {
                        file.SaveAs(fullName);
                        files[fileIndex] = fullName;
                    }
                }
                else if (path != null && file.ContentLength == 0 && file.FileName != string.Empty)
                {
                    _process.AddMessage(
                        string.Format("The file \"{0}\" was ignored because it was empty.", file.FileName),
                        MessageType.Error);
                }
            }

            return files;
        }

        public void SendToBrowser(string filename)
        {
            HttpResponse response = _process.HttpPage.Response;

            var fileInfo = new FileInfo(Path.Combine(_rootFilesPath, filename));
            if (fileInfo.FullName.StartsWith(_rootFilesPath))
            {
                if (fileInfo.Exists)
                {
                    response.Clear();
                    string currentFile = fileInfo.FullName;

                    int width;
                    int.TryParse(_process.QueryOther["width"], out width);

                    int height;
                    int.TryParse(_process.QueryOther["height"], out height);

                    int radius;
                    int.TryParse(_process.QueryOther["radius"], out radius);

                    int borderwidth;
                    int.TryParse(_process.QueryOther["borderwidth"], out borderwidth);

                    string bordercolor = ("#" + _process.QueryOther["bordercolor"]);

                    bool isFixed = _process.QueryOther["fixed"] == "true";
                    if (width > 0 || height > 0 || radius >= 0)
                    {
                        Bitmap bitmap = null;
                        string newFilename = string.Format("{0}_{1}x{2}r{3}",
                                                           fileInfo.Name.TrimEnd(fileInfo.Extension.ToCharArray()),
                                                           width, height, radius);

                        if (borderwidth > 0 && Regex.IsMatch(bordercolor, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
                            newFilename += ("bw" + borderwidth + "bc" + bordercolor);

                        newFilename += ("_" + isFixed + ".png");

                        if (fileInfo.Directory != null)
                        {
                            string thumbnailFile = Common.CombinePaths(fileInfo.Directory.FullName, "thumbs",
                                                                       newFilename);

                            // We changed the cropping and resizing code on April 28 2006
                            // - rerender all thumbnails before that
                            // - and changed it again in September
                            var rerender = new DateTime(2006, 9, 25);
                            FileInfo thumbInfo = null;
                            if (File.Exists(thumbnailFile))
                                thumbInfo = new FileInfo(thumbnailFile);

                            if (thumbInfo == null || thumbInfo.LastWriteTime < fileInfo.LastWriteTime ||
                                rerender > thumbInfo.LastWriteTime)
                            {
                                bitmap = new Bitmap(fileInfo.FullName);
                                if (fileInfo.Extension.ToLower() == ".gif")
                                {
                                    var realBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                                    for (int x = 0; x < bitmap.Width; x++)
                                        for (int y = 0; y < bitmap.Height; y++)
                                            realBitmap.SetPixel(x, y, bitmap.GetPixel(x, y));

                                    bitmap.Dispose();
                                    bitmap = realBitmap;
                                }

                                bitmap = ResizeOrCropImage(bitmap, width, height, isFixed);
                                bitmap = RoundImageVertex(bitmap, radius, borderwidth, bordercolor);
                            }
                            else
                                currentFile = thumbnailFile;

                            if (bitmap != null)
                            {
                                if (!Directory.Exists(fileInfo.Directory + @"\thumbs"))
                                    Directory.CreateDirectory(fileInfo.Directory + @"\thumbs");

                                var eps = new EncoderParameters(1);
                                eps.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                                ImageCodecInfo ici = GetEncoderInfo("image/png");
                                bitmap.Save(thumbnailFile, ici, eps);
                                bitmap.Dispose();
                                currentFile = thumbnailFile;
                            }
                        }
                    }

                    // Send image to browser
                    var file = new FileInfo(currentFile);
                    if (_process.QueryOther["download"] == "true")
                        response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    else
                        response.AddHeader("Content-Disposition", "filename=" + file.Name);
                    //response.ContentType = Common.GetMimeType(file.Extension);
                    response.AddHeader("Content-Length", file.Length.ToString());
                    response.AddHeader("Content-Type", Common.GetMimeType(file.Extension));

                    response.Cache.SetExpires(DateTime.Now + new TimeSpan(7, 0, 0, 0));
                    response.Cache.SetCacheability(HttpCacheability.Public);

                    //response.TransmitFile(currentFile);   //jig: com 2007-0-17
                    response.WriteFile(currentFile); //jig: add 2007-0-17
                    response.Flush();
                    response.End();
                    //response.Close();
                }
            }
        }

        private static Bitmap ResizeOrCropImage(Image bitmap, int width, int height, bool isFixed)
        {
            if (width >= bitmap.Width && height >= bitmap.Height && !isFixed)
                return null;

            Image tmpImage = bitmap;

            // Crop
            if (width > 0 && height > 0)
                tmpImage = isFixed
                               ? ImageResize.FixedSize(tmpImage, width, height, Color.White)
                               : ImageResize.Crop(tmpImage, width, height, ImageResize.AnchorPosition.Center);

            // Resize
            if (width > 0 && width < bitmap.Width)
                tmpImage = ImageResize.ConstrainProportions(tmpImage, width, ImageResize.Dimensions.Width);
            else if (height > 0 && height < bitmap.Height)
                tmpImage = ImageResize.ConstrainProportions(tmpImage, height, ImageResize.Dimensions.Height);

            return (Bitmap) tmpImage;
        }

        private static Bitmap RoundImageVertex(Bitmap bitmap, int radius, int borderWidth, string borderColor)
        {
            if (radius <= 0 || (radius*2) > bitmap.Height || (radius*2) > bitmap.Width)
                return bitmap;

            Image tmpImage = bitmap;

            if (!Regex.IsMatch(borderColor, @"[#]([0-9]|[a-f]|[A-F]){6}\b") || borderWidth < 1)
                tmpImage = ImageVertexRounding.RoundedRectangle(tmpImage, radius);
            else
                tmpImage = ImageVertexRounding.RoundedRectangle(tmpImage, radius, borderWidth, borderColor);

            return (Bitmap) tmpImage;
        }

        public void MoveFolder(string folder, string newContainingDirectory)
        {
            Common.MoveDirectory(Common.CombinePaths(_rootFilesPath, folder),
                                 Common.CombinePaths(_rootFilesPath, newContainingDirectory));
        }

        public void MoveFile(string file, string newContainingDiretory)
        {
            Common.MoveFile(Common.CombinePaths(_rootFilesPath, file),
                            Common.CombinePaths(_rootFilesPath, newContainingDiretory));
        }

        public void RenameFile(string file, string newFileName)
        {
            string filePath = Common.CheckedCombinePaths(_rootFilesPath, file);
            var fileInfo = new FileInfo(filePath);
            string filePathNew = Common.CheckedCombinePaths(fileInfo.DirectoryName, newFileName);

            fileInfo.MoveTo(filePathNew);
        }

        public void RenameFolder(string folder, string newFolderName)
        {
            string folderPath = Common.CheckedCombinePaths(_rootFilesPath, folder);

            var folderInfo = new DirectoryInfo(folderPath);
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
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];

            return null;
        }
    }

    public class FolderElement
    {
        private readonly DirectoryInfo _directoryInfo;

        public FolderElement(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        private string Name
        {
            get { return _directoryInfo.Name; }
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

            foreach (FileInfo file in _directoryInfo.GetFiles())
            {
                XmlNode fileNode = CommonXml.GetNode(folderNode, "file", EmptyNodeHandling.ForceCreateNew);
                CommonXml.SetAttributeValue(fileNode, "name", file.Name);
                CommonXml.SetAttributeValue(fileNode, "extension", file.Extension);
                GetFileAttributes(fileNode, file.Name); // jig: add 2007-09-17
            }

            if (subFolder == SubFolder.IncludeSubfolders)
            {
                foreach (DirectoryInfo dir in _directoryInfo.GetDirectories())
                {
                    if (Filter(dir.Name))
                    {
                        var folderElement = new FolderElement(dir.FullName);
                        folderElement.GetXml(folderNode, SubFolder.IncludeSubfolders);
                    }
                }
            }
        }

        private void GetFileAttributes(XmlNode xmlNode, String fileName) // jig: add 2007-09-17
        {
            DataRow[] dr;
            var ds = new DataSet();
            try
            {
                ds.ReadXml(_directoryInfo.FullName + "\\data\\gallery.xml");
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