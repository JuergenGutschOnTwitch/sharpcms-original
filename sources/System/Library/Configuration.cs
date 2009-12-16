//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public static class Configuration
    {
        private static string[] GetConfigFileNames(string fileName, IEnumerable<string> paths)
        {
            List<string> processFiles = new List<string>();

            foreach (string path in paths)
            {
                if (path.EndsWith(".xml"))
                {
                    processFiles.Add(path);
                }
                else
                {
                    string[] directories = Directory.GetDirectories(path);

                    foreach (string directory in directories)
                    {
                        string fullName = Common.CombinePaths(path, directory, fileName);
                        if (File.Exists(fullName))
                        {
                            processFiles.Add(fullName);
                        }
                    }
                }
            }

            return processFiles.ToArray();
        }

        private static bool FilesChanged(IEnumerable<string> fileNames, Cache cache)
        {
            foreach (string fileName in fileNames)
            {
                if (cache["changed_" + fileName] == null)
                {
                    return true;
                }

                DateTime cacheChanged = (DateTime) cache["changed_" + fileName];
                if (cacheChanged != File.GetLastWriteTime(fileName))
                {
                    return true;
                }
            }

            return false;
        }

        public static void CombineProcessTree(string[] paths, Cache cache)
        {
            string[] fileNames = GetConfigFileNames("Process.xml", paths);
            if (FilesChanged(fileNames, cache))
            {
                XmlDocument combinedProcess = new XmlDocument();
                combinedProcess.AppendChild(combinedProcess.CreateElement("process"));

                foreach (string fileName in fileNames)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);
                    CommonXml.MergeXml(combinedProcess, xmlDocument, "load", "handle");
                    cache["changed_" + fileName] = File.GetLastWriteTime(fileName);
                }

                cache["process"] = combinedProcess;
            }
        }

        public static void CombineSettings(string[] paths, Cache cache)
        {
            string[] fileNames = GetConfigFileNames("Settings.xml", paths);
            if (FilesChanged(fileNames, cache))
            {
                XmlDocument combinedSettings = new XmlDocument();
                combinedSettings.AppendChild(combinedSettings.CreateElement("settings"));

                foreach (string fileName in fileNames)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);
                    CommonXml.MergeXml(combinedSettings, xmlDocument, "item");
                    cache["changed_" + fileName] = File.GetLastWriteTime(fileName);
                }

                cache["settings"] = combinedSettings;
            }
        }
    }
}