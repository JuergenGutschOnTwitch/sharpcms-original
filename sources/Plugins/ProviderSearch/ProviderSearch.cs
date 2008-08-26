using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.providers
{
    class ProviderSearch : BasePlugin2, IPlugin2
    {
        private IndexWriter m_Writer;

        private IndexWriter Writer
        {
            get
            {
                if (m_Writer != null)
                {
                    return m_Writer;
                }

                string indexPath = Process.HttpPage.Server.MapPath("/Custom/App_Data/SearchIndex.idx");
                bool createNewIndex = !File.Exists(indexPath);
                m_Writer = new IndexWriter(indexPath, new StandardAnalyzer(), createNewIndex);
                return m_Writer;
            }
        }

        public new string Name
        {
            get
            {
                return "Search";
            }
        }

        public new string[] Implements
        {
            get
            {
                return new string[] { "pages" };
            }
        }

        public ProviderSearch()
        {
        }

        public ProviderSearch(Process process)
        {
            m_Process = process;
        }
    }
}