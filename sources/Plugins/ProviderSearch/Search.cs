//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using InventIt.SiteSystem.Data.SiteTree;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;

namespace InventIt.SiteSystem.Providers
{
    public class Search
    {
        /// <summary>
        /// How many items can be showed on one page.
        /// </summary>
        private const int MaxResults = 5;

        private readonly Page currentPage;
        private readonly string indexDir;
        private readonly Process process;
        private readonly string searchPage;

        /// <summary>
        /// Time it took to make the search.
        /// </summary>
        private TimeSpan duration;

        /// <summary>
        /// First item on page (user format).
        /// </summary>
        private int fromItem;

        private string query;

        private int startAt;

        /// <summary>
        /// First item on page (index format).
        /// </summary>
        private int startFirstAt;

        /// <summary>
        /// Last item on page (user format).
        /// </summary>
        private int toItem;

        /// <summary>
        /// Total items returned by search.
        /// </summary>
        private int total;

        public Search(Process process)
        {
            this.process = process;

            string mainVal = this.process.QueryData["mainvalue"];
            if (string.IsNullOrEmpty(mainVal))
            {
                string mStartAt = this.process.QueryData["start"];
                startAt = string.IsNullOrEmpty(mStartAt) ? 0 : int.Parse(mStartAt);
            }
            else
            {
                startAt = int.Parse(mainVal);
            }

            indexDir = this.process.Settings["search/index"];

            //jig: search only one section
            string[] s = this.process.CurrentProcess.Split('/');
            if (s.Length >= 2)
            {
                indexDir = Path.Combine(indexDir, s[1]);
            }

            searchPage = "show/";

            string pageID = this.process.QueryData["pageidentifier"];
            currentPage = new SiteTree(this.process).GetPage(pageID);
        }

        public int StartAt
        {
            set { startAt = value; }
        }

        private string Query
        {
            get { return query; }
        }

        /// <summary>
        /// Prepares the string with seach summary information.
        /// </summary>
        private string Summary
        {
            get
            {
                if (total > 0)
                {
                    return string.Format("Results <b>{0} - {1}</b> of <b>{2}</b> for <b>{3}</b>. ({4} mili seconds)", fromItem, toItem, total, Query, duration.TotalMilliseconds);
                }
                return "No results found";
            }
        }

        /// <summary>
        /// How many pages are there in the results.
        /// </summary>
        private int PageCount
        {
            get { return (total - 1) / MaxResults; } // floor 
        }

        /// <summary>
        /// First item of the last page
        /// </summary>
        private int LastPageStartsAt
        {
            get { return PageCount * MaxResults; }
        }

        /// <summary>
        /// Does the search and stores the information about the results.
        /// </summary>
        public void HandleSearch(string q)
        {
            DateTime start = DateTime.Now;
            this.query = q;

            // create the searcher
            // index is placed in "index" subdirectory
            var searcher = new IndexSearcher(indexDir);
            Analyzer analyzer = new StandardAnalyzer();

            // parse the query, "text" is the default field to search
            Lucene.Net.Search.Query query = QueryParser.Parse(this.query, "text", analyzer);

            const string containername = "content";
            Container container = currentPage.Containers[containername];
            const string elementResult = "result";
            const string elementPaging = "paging";
            const string elementSummary = "summary";
            const string elementAll = elementResult + elementPaging + elementSummary;
            int count = container.Elements.Count;

            // Remove previous search result
            for (int i = count; i > 0; --i)
            {
                if (container.Elements[i] != null)
                {
                    if (elementAll.IndexOf(container.Elements[i].Type) > -1)
                    { container.Elements.Remove(i); }
                }
            }

            Element element = container.Elements[0];
            Element elSummary = container.Elements.Create(elementSummary);
            element["query"] = this.query;

            // search
            Hits hits = searcher.Search(query);

            total = hits.Length();

            // create highlighter
            var highlighter = new Highlighter(new QueryScorer(query));

            // initialize startAt
            startFirstAt = InitStartAt();

            // how many items we should show - less than defined at the end of the results
            int resultsCount = SmallerOf(total, MaxResults + startFirstAt);

            for (int i = startFirstAt; i < resultsCount; i++)
            {
                // get the document from index
                Document doc = hits.Doc(i);
                string path = doc.Get("url");

                if (path != null)
                {
                    string plainText = doc.Get("text");

                    TokenStream tokenStream = analyzer.TokenStream("text", new StringReader(plainText));
                    string text = highlighter.GetBestFragments(tokenStream, plainText, 2, "...");

                    element = container.Elements.Create(elementResult);
                    element["title"] = doc.Get("title");
                    element["path"] = searchPage + path.Replace("\\", "/") + ".aspx";
                    element["sample"] = string.IsNullOrEmpty(text) ? plainText : text;
                }
            }

            searcher.Close();

            duration = DateTime.Now - start;
            fromItem = startFirstAt + 1;
            toItem = SmallerOf(startFirstAt + MaxResults, total);

            // result information
            elSummary.Node.InnerText = Summary;
            // paging link
            element = container.Elements.Create(elementPaging);
            element.Node.InnerText = SetPaging();
            process.SearchContext = currentPage;
        }

        /// <summary>
        /// Returns the smaller value of parameters.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static int SmallerOf(int first, int second)
        {
            return first < second ? first : second;
        }

        /// <summary>
        /// Page links. DataTable might be overhead but there used to be more fields in previous version so I'm keeping it for now.
        /// </summary>
        private string SetPaging()
        {
            // pageNumber starts at 1
            int pageNumber = (startFirstAt + MaxResults - 1) / MaxResults;

            var htmlList = new List<string>();
            string html = PagingItemHtml(pageNumber + 1, false);
            htmlList.Add(html);

            const int previousPagesCount = 4;
            for (int i = pageNumber - 1; i >= 0 && i >= pageNumber - previousPagesCount; i--)
            {
                string htm = PagingItemHtml(i + 1, true);
                htmlList.Insert(0, htm);
            }

            const int nextPagesCount = 4;
            for (int i = pageNumber + 1; i <= PageCount && i <= pageNumber + nextPagesCount; i++)
            {
                string htm = PagingItemHtml(i + 1, true);
                htmlList.Add(htm);
            }

            var sb = new StringBuilder();
            foreach (string htm in htmlList)
            {
                sb.Append(htm);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Prepares HTML of a paging item (bold number for current page, links for others).
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns></returns>
        private static string PagingItemHtml(int number, bool active)
        {
            if (active)
            {
                return string.Format("<a href=\"javascript:ThrowEvent('{0}', '');\"> {1} </a>", (MaxResults * (number - 1)), number);
            }

            return "<b>" + number + "</b>";
        }

        /// <summary>
        /// Initializes startAt value. Checks for bad values.
        /// this.Request.Params["start"]
        /// </summary>
        /// <returns></returns>
        private int InitStartAt()
        {
            try
            {
                int sa = Convert.ToInt32(startAt);
                // too small starting item, return first page
                if (startAt < 0)
                {
                    return 0;
                }
                // too big starting item, return last page
                if (startAt >= total - 1)
                {
                    return LastPageStartsAt;
                }
                return sa;
            }
            catch
            {
                return 0;
            }
        }

    }
}