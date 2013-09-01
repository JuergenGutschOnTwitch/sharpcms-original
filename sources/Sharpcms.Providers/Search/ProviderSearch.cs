// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections;
using System.IO;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Providers.Search
{
    public class ProviderSearch : BasePlugin2, IPlugin2
    {
        public ProviderSearch()
        {
            Process = null;
        }

        public ProviderSearch(Process process) : this()
        {
            Process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get
            {
                return "search";
            }
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
            string query = Process.QueryData["query"];
            if (!string.IsNullOrEmpty(query))
            {
                Search search = new Search(Process);
                if (startAt > 0)
                {
                    search.StartAt = startAt;
                }

                search.HandleSearch(query);
            }
        }

        private void HandleIndex()
        {
            string rootPath = Process.Root;
            string[] s = Process.CurrentProcess.Split('/');
            string baseDir = Process.Settings["search/index"];
            string rules = rootPath + @"\Custom\App_Data\rules.xml";
            string filePath = rootPath + @"\Custom\App_Data\database";

            //jig: index only one section
            if (s.Length >= 2)
            {
                filePath = Path.Combine(Path.Combine(filePath, "site"), s[1]);
                baseDir = Path.Combine(baseDir, s[1]);
            }

            string procMessage;

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
                Process.AddMessage(procMessage);
            }
        }
    }
}