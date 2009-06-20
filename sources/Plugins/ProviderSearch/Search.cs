//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

        private readonly Page _currentPage;
        private readonly string _indexDir;
        private readonly Process _process;
        private readonly string _searchPage;

        /// <summary>
        /// Time it took to make the search.
        /// </summary>
        private TimeSpan _duration;

        /// <summary>
        /// First item on page (user format).
        /// </summary>
        private int _fromItem;

        private string _query;

        private int _startAt;

        /// <summary>
        /// First item on page (index format).
        /// </summary>
        private int _startFirstAt;

        /// <summary>
        /// Last item on page (user format).
        /// </summary>
        private int _toItem;

        /// <summary>
        /// Total items returned by search.
        /// </summary>
        private int _total;

        public Search(Process process)
        {
            _process = process;

            string mainVal = _process.QueryData["mainvalue"];
            if (string.IsNullOrEmpty(mainVal))
            {
                string mStartAt = _process.QueryData["start"];
                _startAt = string.IsNullOrEmpty(mStartAt) ? 0 : int.Parse(mStartAt);
            }
            else
                _startAt = int.Parse(mainVal);

            _indexDir = _process.Settings["search/index"];

            //jig: search only one section
            string[] s = _process.CurrentProcess.Split('/');
            if (s.Length >= 2)
                _indexDir = Path.Combine(_indexDir, s[1]);

            _searchPage = "show/";

            string pageID = _process.QueryData["pageidentifier"];
            _currentPage = new SiteTree(_process).GetPage(pageID);
        }

        public int StartAt
        {
            set { _startAt = value; }
        }

        private string Query
        {
            get { return _query; }
        }

        /// <summary>
        /// Prepares the string with seach summary information.
        /// </summary>
        private string Summary
        {
            get
            {
                if (_total > 0)
                    return "Results <b>" + _fromItem + " - " + _toItem + "</b> of <b>" + _total + "</b> for <b>" + Query +
                           "</b>. (" + _duration.TotalMilliseconds + " mili seconds)";
                return "No results found";
            }
        }

        /// <summary>
        /// How many pages are there in the results.
        /// </summary>
        private int PageCount
        {
            get { return (_total - 1)/MaxResults; } // floor 
        }

        /// <summary>
        /// First item of the last page
        /// </summary>
        private int LastPageStartsAt
        {
            get { return PageCount*MaxResults; }
        }

        /// <summary>
        /// Does the search and stores the information about the results.
        /// </summary>
        public void HandleSearch(string q)
        {
            DateTime start = DateTime.Now;
            _query = q;

            // create the searcher
            // index is placed in "index" subdirectory
            var searcher = new IndexSearcher(_indexDir);
            Analyzer analyzer = new StandardAnalyzer();

            // parse the query, "text" is the default field to search
            Lucene.Net.Search.Query query = QueryParser.Parse(_query, "text", analyzer);

            const string containername = "content";
            Container container = _currentPage.Containers[containername];
            const string elementResult = "result";
            const string elementPaging = "paging";
            const string elementSummary = "summary";
            const string elementAll = elementResult + elementPaging + elementSummary;
            int count = container.Elements.Count;

            // Remove previous search result
            for (int i = count; i > 0; --i)
                if (container.Elements[i] != null)
                    if (elementAll.IndexOf(container.Elements[i].Type) > -1)
                        container.Elements.Remove(i);

            Element element = container.Elements[0];
            Element elSummary = container.Elements.Create(elementSummary);
            element["query"] = _query;

            // search
            Hits hits = searcher.Search(query);

            _total = hits.Length();

            // create highlighter
            var highlighter = new Highlighter(new QueryScorer(query));

            // initialize startAt
            _startFirstAt = InitStartAt();

            // how many items we should show - less than defined at the end of the results
            int resultsCount = SmallerOf(_total, MaxResults + _startFirstAt);

            for (int i = _startFirstAt; i < resultsCount; i++)
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
                    element["path"] = _searchPage + path.Replace("\\", "/") + ".aspx";
                    element["sample"] = string.IsNullOrEmpty(text) ? plainText : text;
                }
            }

            searcher.Close();

            _duration = DateTime.Now - start;
            _fromItem = _startFirstAt + 1;
            _toItem = SmallerOf(_startFirstAt + MaxResults, _total);

            // result information
            elSummary.Node.InnerText = Summary;
            // paging link
            element = container.Elements.Create(elementPaging);
            element.Node.InnerText = SetPaging();
            _process.SearchContext = _currentPage;
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
            int pageNumber = (_startFirstAt + MaxResults - 1)/MaxResults;

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
                sb.Append(htm);

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
                return "<a href=\"javascript:ThrowEvent('" + (MaxResults*(number - 1)) + "', '');\"> " + number +
                       " </a>";

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
                int sa = Convert.ToInt32(_startAt);
                // too small starting item, return first page
                if (_startAt < 0)
                    return 0;
                // too big starting item, return last page
                if (_startAt >= _total - 1)
                    return LastPageStartsAt;
                return sa;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ParseHtml(string html) // ToDo: Is a unused Method
        {
            string temp = html.Replace("<p>", " ");
            temp = temp.Replace("</p>", " ");
            temp = Regex.Replace(temp, "<[^>]*>", "");
            temp = temp.Replace("&lt;", "<");
            temp = temp.Replace("&gt;", ">");
            temp = temp.Replace("<p>", " ");
            temp = temp.Replace("</p>", " ");
            temp = Regex.Replace(temp, "<[^>]*>", "");
            return temp.Replace("&nbsp;", " ");
        }
    }
}