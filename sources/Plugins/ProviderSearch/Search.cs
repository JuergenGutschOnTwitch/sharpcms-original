using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Analysis;

using InventIt.SiteSystem.Data.SiteTree;

namespace InventIt.SiteSystem.Providers
{
    public class Search
    {
        public Search(Process process)
        {
            m_Process = process;

            string mainVal = m_Process.QueryData["mainvalue"];
            if (string.IsNullOrEmpty(mainVal))
            {
                string startAt = m_Process.QueryData["start"];
                _startAt = string.IsNullOrEmpty(startAt) ? 0 : int.Parse(startAt);
            }
            else
                _startAt = int.Parse(mainVal);

            _indexDir = m_Process.Settings["search/index"];

            //jig: search only one section
            string[] s = m_Process.CurrentProcess.Split('/');
            if (s.Length >= 2)
            {
                _indexDir = Path.Combine(_indexDir, s[1]);
            }

            _searchPage = "show/";

            string pageID = m_Process.QueryData["pageidentifier"];
            m_CurrentPage = new SiteTree(m_Process).GetPage(pageID);
            m_Page = _searchPage + pageID;
        }

        Page m_CurrentPage;
        Process m_Process;
        string m_Page;

        /// <summary>
        /// First item on page (index format).
        /// </summary>
        private int startAt;

        /// <summary>
        /// First item on page (user format).
        /// </summary>
        private int fromItem;

        /// <summary>
        /// Last item on page (user format).
        /// </summary>
        private int toItem;

        /// <summary>
        /// Total items returned by search.
        /// </summary>
        private int total;

        /// <summary>
        /// Time it took to make the search.
        /// </summary>
        private TimeSpan duration;

        /// <summary>
        /// How many items can be showed on one page.
        /// </summary>
        private readonly int maxResults = 5;

        private string _searchPage;
        private string _indexDir;
        private string _query;
        private int _startAt;

        public int StartAt
        {
            get { return _startAt; }
            set { _startAt = value; }
        }

        public string Query
        {
            get { return _query; }
            set { _query = value; }
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
            IndexSearcher searcher = new IndexSearcher(_indexDir);
            Analyzer analyzer = new StandardAnalyzer();

            // parse the query, "text" is the default field to search
            Lucene.Net.Search.Query query = QueryParser.Parse(_query, "text", analyzer);

            string containername = "content";
            Container container = m_CurrentPage.Containers[containername];
            string elementResult = "result";
            string elementPaging = "paging";
            string elementSummary = "summary";
            string elementAll = elementResult + elementPaging + elementSummary;
            int count = container.Elements.Count;

            // Remove previous search result
            for (int i = count; i > 0; --i)
            {
                if (container.Elements[i] != null)
                {
                    if (elementAll.IndexOf(container.Elements[i].Type) > -1)
                        container.Elements.Remove(i);
                }
            }

            Element element = container.Elements[0];
            Element elSummary = container.Elements.Create(elementSummary);
            element["query"] = _query;

            // search
            Hits hits = searcher.Search(query);

            this.total = hits.Length();

            // create highlighter
            Highlighter highlighter = new Highlighter(new QueryScorer(query));

            // initialize startAt
            this.startAt = initStartAt();

            // how many items we should show - less than defined at the end of the results
            int resultsCount = smallerOf(total, this.maxResults + this.startAt);

            for (int i = startAt; i < resultsCount; i++)
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

            duration = DateTime.Now - start;
            fromItem = startAt + 1;
            toItem = smallerOf(startAt + maxResults, total);

            // result information
            elSummary.Node.InnerText = this.Summary;
            // paging link
            element = container.Elements.Create(elementPaging);
            element.Node.InnerText = SetPaging();
            m_Process.SearchContext = m_CurrentPage;
        }

        /// <summary>
        /// Returns the smaller value of parameters.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private int smallerOf(int first, int second)
        {
            return first < second ? first : second;
        }

        /// <summary>
        /// Page links. DataTable might be overhead but there used to be more fields in previous version so I'm keeping it for now.
        /// </summary>
        public string SetPaging()
        {
            // pageNumber starts at 1
            int pageNumber = (startAt + maxResults - 1) / maxResults;

            List<string> htmlList = new List<string>();
            string html = pagingItemHtml(startAt, pageNumber + 1, false);
            htmlList.Add(html);

            int previousPagesCount = 4;
            for (int i = pageNumber - 1; i >= 0 && i >= pageNumber - previousPagesCount; i--)
            {
                int step = i - pageNumber;
                string htm = pagingItemHtml(startAt + (maxResults * step), i + 1, true);
                htmlList.Insert(0, htm);
            }

            int nextPagesCount = 4;
            for (int i = pageNumber + 1; i <= pageCount && i <= pageNumber + nextPagesCount; i++)
            {
                int step = i - pageNumber;
                string htm = pagingItemHtml(startAt + (maxResults * step), i + 1, true);
                htmlList.Add(htm);
            }

            StringBuilder sb = new StringBuilder();
            foreach (string htm in htmlList)
                sb.Append(htm);

            return sb.ToString();
        }


        /// <summary>
        /// Prepares HTML of a paging item (bold number for current page, links for others).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="number"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        private string pagingItemHtml(int start, int number, bool active)
        {

            if (active)
                return "<a href=\"javascript:ThrowEvent('" + (maxResults * (number - 1)) + "', '');\"> " + number + " </a>";
            else
                return "<b>" + number + "</b>";
        }

        /// <summary>
        /// Prepares the string with seach summary information.
        /// </summary>
        private string Summary
        {
            get
            {
                if (total > 0)
                    return "Results <b>" + this.fromItem + " - " + this.toItem + "</b> of <b>" + this.total + "</b> for <b>" + this.Query + "</b>. (" + this.duration.TotalMilliseconds + " mili seconds)";
                return "No results found";
            }
        }

        /// <summary>
        /// Initializes startAt value. Checks for bad values.
        /// this.Request.Params["start"]
        /// </summary>
        /// <returns></returns>
        private int initStartAt()
        {
            try
            {
                int sa = Convert.ToInt32(_startAt);
                // too small starting item, return first page
                if (_startAt < 0)
                    return 0;
                // too big starting item, return last page
                if (_startAt >= total - 1)
                    return lastPageStartsAt;
                return sa;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// How many pages are there in the results.
        /// </summary>
        private int pageCount
        {
            get { return (total - 1) / maxResults; }// floor 
        }

        /// <summary>
        /// First item of the last page
        /// </summary>
        private int lastPageStartsAt
        {
            get { return pageCount * maxResults; }
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string parseHtml(string html)
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
