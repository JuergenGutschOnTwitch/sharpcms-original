// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.IO;
using System.Xml;
using Sharpcms.Library.Common;

namespace Sharpcms.Library
{
    public class Settings
    {
        private const string CustomPath = @"Custom\App_Data\CustomSettings.xml";
        private static Settings _defaultInstance;
        private readonly XmlDocument _combinedSettings;
        private readonly string _customFullPath;
        private readonly XmlDocument _customSettings = new XmlDocument();
        private readonly string _rootPath;

        public Settings(Process.Process process, string rootPath)
        {
            _rootPath = rootPath;
            _customFullPath = Path.Combine(rootPath, CustomPath);
            _customSettings.Load(_customFullPath);
            _combinedSettings = process.Cache["settings"] as XmlDocument;
            _defaultInstance = this;
        }

        public string CustomFullPath //ToDo: is a unused Property (T.Huber 18.06.2009)
        {
            get { return _customFullPath; }
        }

        public string RootPath
        {
            get { return _rootPath; }
        }

        public static Settings DefaultInstance
        {
            get { return _defaultInstance; }
        }

        public string this[string path]
        {
            get { return this[path, RelativePathHandling.ConvertToAbsolute]; }
            set { this[path, RelativePathHandling.ConvertToAbsolute] = value; }
        }

        private string this[string path, RelativePathHandling relativePathHandling]
        {
            get
            {
                XmlNode settingsNode = CommonXml.GetNode(_combinedSettings.SelectSingleNode("settings"), path,
                                                         EmptyNodeHandling.CreateNew);

                string value = settingsNode.InnerText;

                if (relativePathHandling == RelativePathHandling.ConvertToAbsolute)
                {
                    return ConvertPath(value);
                }

                return value;
            }
            set
            {
                CommonXml.GetNode(_customSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew).InnerText = value;
                CommonXml.GetNode(_combinedSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew).InnerText = value;

                Save();
            }
        }

        public XmlNode GetAsNode(string path)
        {
            return CommonXml.GetNode(_combinedSettings.SelectSingleNode("settings"), path);
        }

        private string ConvertPath(string relativePath)
        {
            if (!relativePath.StartsWith("~/"))
                return relativePath;

            relativePath = relativePath.Substring(2);
            return Common.Common.CombinePaths(RootPath, relativePath.Replace('/', '\\'));
        }

        private void Save()
        {
            //ToDo: should work but has not been testet yet (old)
            _customSettings.Save(_customFullPath);
        }
    }
}