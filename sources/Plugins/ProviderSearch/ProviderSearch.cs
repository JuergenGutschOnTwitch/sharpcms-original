//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections;
using System.IO;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderSearch : BasePlugin2, IPlugin2
    {
        public ProviderSearch()
        {
        }

        public ProviderSearch(Process process)
        {
            _process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "search"; }
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
                    int startAt;
                    if (int.TryParse(mainEvent, out startAt))
                        HandleSearch(startAt);
                    break;
            }
        }

        #endregion

        private void HandleSearch(int startAt)
        {
            // no query test don't process
            string query = _process.QueryData["query"];
            if (string.IsNullOrEmpty(query)) return;

            var search = new Search(_process);
            if (startAt > 0)
                search.StartAt = startAt;

            search.HandleSearch(query);
        }

        private void HandleIndex()
        {
            string rootPath = _process.Root;
            string[] s = _process.CurrentProcess.Split('/');

            string baseDir = _process.Settings["search/index"];
            string rules = rootPath + @"\Custom\App_Data\rules.xml";
            string filePath = rootPath + @"\Custom\App_Data\database";

            //jig: index only one section
            if (s.Length >= 2)
            {
                filePath = Path.Combine(Path.Combine(filePath, "site"), s[1]);
                baseDir = Path.Combine(baseDir, s[1]);
            }

            string procMessage;

            var indexer = new Indexer(baseDir);
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
                _process.AddMessage(procMessage);
        }
    }
}