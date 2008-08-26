//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Library
{
    public class Settings
    {
        private string m_CustomPath = @"Custom\App_Data\CustomSettings.xml";
        private string m_CustomFullPath;
        private XmlDocument m_CombinedSettings;
        private XmlDocument m_CustomSettings = new XmlDocument();
		private string m_RootPath;
		private static Settings m_DefaultInstance;

        public string CustomFullPath
        {
            get
            {
                return m_CustomFullPath;
            }
        }
		public string RootPath
		{
			get
			{
				return m_RootPath;
			}
		}

      
		public static Settings DefaultInstance
		{
			get
			{
				return m_DefaultInstance;
			}
		}

		public string this[string path]
		{
			get
			{
				return this[path, RelativePathHandling.ConvertToAbsolute];
			}
			set
			{
				this[path, RelativePathHandling.ConvertToAbsolute] = value;
			}
		}

		public string this[string path, RelativePathHandling relativePathHandling]
		{
			get
			{
				XmlNode settingsNode = CommonXml.GetNode(m_CombinedSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew);

                string value = settingsNode.InnerText;

				if (relativePathHandling == RelativePathHandling.ConvertToAbsolute)
				{
					return ConvertPath(value);
				}
				else
				{
					return value;
				}
			}
			set
			{
                CommonXml.GetNode(m_CustomSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew).InnerText = value;
                CommonXml.GetNode(m_CombinedSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew).InnerText = value;
                Save();
			}
		}

		public Settings(Process process, string rootPath)
        {
			m_RootPath = rootPath;
            m_CustomFullPath = Path.Combine(rootPath, m_CustomPath);
            m_CustomSettings.Load(m_CustomFullPath);
            m_CombinedSettings = process.Cache["settings"] as XmlDocument;
			m_DefaultInstance = this;
        }

		public XmlNode GetAsNode(string path)
		{
			return CommonXml.GetNode(m_CombinedSettings.SelectSingleNode("settings"), path);
		}

		private string ConvertPath(string relativePath)
		{
			if (!relativePath.StartsWith("~/"))
			{
				return relativePath;
			}

			relativePath = relativePath.Substring(2);
			return Common.CombinePaths(RootPath, relativePath.Replace('/', '\\'));
		}

		private void Save()
        { //TODO: should work but has not been testet yet
            m_CustomSettings.Save(m_CustomFullPath);
        }
    }
}