using System;
using System.Collections;
//using System.Text;
using System.Xml;

using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Data.SiteTree;
using System.IO;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderSearch : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get { return "search"; }
        }

        public ProviderSearch()
        {
        }

        public ProviderSearch(InventIt.SiteSystem.Process process)
        {
            m_Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "index":
                    HandleIndex();
                    break;
                case "search":
                    HandleSearch(0);
                    break;
                default:
                    int startAt = 0;
                    if (int.TryParse(mainEvent, out startAt))
                        HandleSearch(startAt);
                    break;

            }
        }

        private void HandleSearch(int startAt)
        {
            // no query test don't process
            string query = m_Process.QueryData["query"];
            if (string.IsNullOrEmpty(query)) return;

            Search search = new Search(m_Process);
            if (startAt > 0)
                search.StartAt = startAt;

            search.HandleSearch(query);
        }

        private void HandleIndex()
        {
            string rootPath = m_Process.Root;
            string[] s = m_Process.CurrentProcess.Split('/');

            string baseDir = m_Process.Settings["search/index"];
            string rules = rootPath + @"\Custom\App_Data\rules.xml";
            string filePath = rootPath + @"\Custom\App_Data\database";

            //jig: index only one section
            if (s.Length >= 2)
            {
                filePath = Path.Combine(Path.Combine(filePath, "site"), s[1]);
                baseDir = Path.Combine(baseDir, s[1]);
            }

            string procMessage = string.Empty;

            Indexer indexer = new Indexer(baseDir);
            indexer.LoadRules(rules);
            try
            {
                indexer.AddDirectory(filePath, "*.xml");
                ArrayList fileList = indexer.IndexDocuments();
                fileList.Clear();
                procMessage = indexer.ProcMessage;
                procMessage += "Indexed OK.";
            }
            catch (Exception)
            {
                procMessage = string.Format("Failed to index documents in '{0}'", filePath);
            }

            if (procMessage != string.Empty)
            {
                m_Process.AddMessage(procMessage);
            }
        }
    }
}
