// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.XPath;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Sharpcms.Providers.ProviderSearch
{
    internal class Rule
    {
        public readonly string Name;
        public readonly string Type;
        public readonly string Xpath;

        public Rule(string n, string t, string x)
        {
            Name = n;
            Type = t;
            Xpath = x;
        }

        public override string ToString()
        {
            return String.Format("Rule '{0}', Type '{1}', XPath '{2}'", Name, Type, Xpath);
        }
    }

    internal class RuleValue
    {
        public readonly Rule Rule;
        public readonly string Value;

        public RuleValue(Rule r, string v)
        {
            Rule = r;
            Value = v;
        }
    }

    internal class DocItem
    {
        private readonly string _key;
        private readonly ArrayList _rules = new ArrayList();
        private readonly string _xPath;

        public DocItem(string xPath, string key)
        {
            _xPath = xPath;
            _key = key;
        }

        public ArrayList Rules
        {
            get { return _rules; }
        }

        public string XPath
        {
            get { return _xPath; }
        }

        public string Key
        {
            get { return _key; }
        }

        private void AddRule(Rule rule)
        {
            _rules.Add(rule);
        }

        public void AddRule(string name, string type, string xPath)
        {
            AddRule(new Rule(name, type, xPath));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("DocItem: Key '{0}' XPath '{1}'" + Environment.NewLine, Key, XPath);
            stringBuilder.Append("Rules:" + Environment.NewLine);

            foreach (Rule rule in Rules)
            {
                stringBuilder.Append(rule + Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }

    internal class RuleSet
    {
        private readonly ArrayList _docItems = new ArrayList();
        private readonly ArrayList _globalRules = new ArrayList();
        private readonly string _name;
        private readonly string _namespace;

        public RuleSet(string name, string nameSpace)
        {
            _name = name;
            _namespace = nameSpace;
        }

        public string Name
        {
            get { return _name; }
        }

        private string Namespace
        {
            get { return _namespace; }
        }

        public ArrayList GlobalRules
        {
            get { return _globalRules; }
        }

        public ArrayList DocItems
        {
            get { return _docItems; }
        }

        private void AddGlobalRule(Rule r)
        {
            _globalRules.Add(r);
        }

        public void AddGlobalRule(string name, string type, string xpath)
        {
            AddGlobalRule(new Rule(name, type, xpath));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("RuleSet for document type '{0}', namespace '{1}'" + Environment.NewLine, Name, Namespace);
            stringBuilder.Append("Global rules:" + Environment.NewLine);

            foreach (Rule rule in _globalRules)
            {
                stringBuilder.Append(rule + Environment.NewLine);
            }

            stringBuilder.Append("Document items:" + Environment.NewLine);
            foreach (DocItem docItem in _docItems)
            {
                stringBuilder.Append(docItem.ToString());
            }

            return stringBuilder.ToString();
        }

        public DocItem AddDocItem(string xp, string key)
        {
            var addDocItem = new DocItem(xp, key);

            _docItems.Add(addDocItem);

            return addDocItem;
        }
    }

    public class Indexer
    {
        private const string Rules = "";
        private readonly ArrayList _fileList = new ArrayList();
        private readonly string _indexName;
        private readonly ArrayList _ruleSets;
        private readonly StringBuilder _messageStringBuilder;
        private readonly bool _verbose;
        private string _docRootDirectory;
        private string _pattern;

        public Indexer(string name, bool verbose)
        {
            _ruleSets = new ArrayList();
            _verbose = verbose;
            _indexName = name;
            _messageStringBuilder = new StringBuilder();
        }

        public Indexer(string name) : this(name, false)
        {
        }

        public string DocRootDirectory
        {
            get { return _docRootDirectory; }
        }

        public string ProcMessage
        {
            get { return _messageStringBuilder.ToString(); }
        }

        /// <summary>
        /// Add HTML files from <c>directory</c> and its subdirectories that match <c>pattern</c>.
        /// </summary>
        /// <param name="directory">Directory with the HTML files.</param>
        /// <param name="pattern">Search pattern, e.g. <c>"*.html"</c></param>
        public void AddDirectory(DirectoryInfo directory, string pattern)
        {
            _docRootDirectory = directory.FullName;
            _pattern = pattern;

            AddSubDirectory(directory);
        }

        public void AddDirectory(string filePath, string pattern)
        {
            AddDirectory(new DirectoryInfo(filePath), pattern);
        }

        private void AddSubDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo fileInfo in directory.GetFiles(_pattern))
            {
                if (_fileList.Contains(fileInfo.FullName))
                {
                    _messageStringBuilder.AppendFormat("Error : {0}\n", fileInfo.FullName);
                }
                else
                {
                    _fileList.Add(fileInfo.FullName);
                }
            }

            foreach (DirectoryInfo directoryInfo in directory.GetDirectories())
            {
                AddSubDirectory(directoryInfo);
            }
        }

        public void LoadRules(string filename)
        {
            var xPathDocument = new XPathDocument(new StreamReader(filename));
            XPathNavigator pathNavigator = xPathDocument.CreateNavigator();
            XPathNodeIterator xPathNodeIterator = pathNavigator.Select("/rules/doctype");

            while (xPathNodeIterator.MoveNext())
            {
                if (xPathNodeIterator.Current != null)
                {
                    XPathNavigator clone = xPathNodeIterator.Current.Clone();
                    string root = clone.GetAttribute("root", Rules);
                    string nameSpace = clone.GetAttribute("namespace", Rules);
                    var ruleSet = new RuleSet(root, nameSpace);
                    XPathNodeIterator riter = clone.Select("rule");

                    while (riter.MoveNext())
                    {
                        if (riter.Current != null)
                        {
                            ruleSet.AddGlobalRule(riter.Current.GetAttribute("field", Rules), riter.Current.GetAttribute("type", Rules), riter.Current.GetAttribute("xpath", Rules));
                        }
                    }

                    riter = clone.Select("document");

                    while (riter.MoveNext())
                    {
                        if (riter.Current != null)
                        {
                            XPathNavigator xPathNavigator = riter.Current.Clone();
                            string xpath = xPathNavigator.GetAttribute("xpath", Rules);
                            string key = xPathNavigator.GetAttribute("key", Rules);
                            DocItem docItem = ruleSet.AddDocItem(xpath, key);
                            XPathNodeIterator diter = xPathNavigator.Select("rule");
                            while (diter.MoveNext())
                            {
                                if (diter.Current != null)
                                {
                                    docItem.AddRule(diter.Current.GetAttribute("field", Rules), diter.Current.GetAttribute("type", Rules), diter.Current.GetAttribute("xpath", Rules));
                                }
                            }
                        }
                    }

                    if (_verbose)
                    {
                        _messageStringBuilder.Append(ruleSet + "\n");
                    }

                    _ruleSets.Add(ruleSet);
                }
            }
        }

        private void AddField(Document ludoc, Rule rule, string val)
        {
            switch (rule.Type)
            {
                case "text":
                    ludoc.Add(Field.Text(rule.Name, parseHtml(val)));
                    break;
                case "keyword":
                    ludoc.Add(Field.Keyword(rule.Name, val));
                    break;
                default:
                    _messageStringBuilder.AppendFormat("Ignoring unknown rule type: {0}\n", rule);
                    break;
            }
        }

        private static void AddKeyField(Document ludoc, string val)
        {
            ludoc.Add(Field.Keyword("_key", val));
        }

        private static void AddNameField(Document ludoc, string val)
        {
            ludoc.Add(Field.Keyword("_name", val));
        }

        private static void DeleteDocument(IndexModifier modifier, string val)
        {
            modifier.Delete(new Term("_key", val));
        }

        public void IndexDocument(string fname)
        {
            var arrayList = new ArrayList { fname };
            IndexDocuments(arrayList);
        }

        public ArrayList IndexDocuments()
        {
            return IndexDocuments(_fileList);
        }

        public ArrayList IndexDocuments(ArrayList fnames)
        {
            var failures = new ArrayList();
 
            // create index if it doesn't exist
             IndexModifier writer = File.Exists(Path.Combine(_indexName, "segments")) 
                ? new IndexModifier(_indexName, new StandardAnalyzer(), false) 
                : new IndexModifier(_indexName, new StandardAnalyzer(), true);

            foreach (string fname in fnames)
            {
                try
                {
                    if (_verbose)
                    {
                        _messageStringBuilder.AppendFormat("Indexing file: {0}\n", fname);
                    }

                    var xPathDocument = new XPathDocument(new StreamReader(fname));
                    XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
                    xPathNavigator.MoveToFirstChild();
                    string docname = xPathNavigator.Name;

                    if (_verbose)
                    {
                        _messageStringBuilder.AppendFormat("Found document type: {0}\n", docname);
                    }

                    foreach (RuleSet ruleSet in _ruleSets)
                    {
                        if (ruleSet.Name == docname)
                        {
                            if (_verbose)
                            {
                                _messageStringBuilder.Append("Found matching ruleset, indexing.\n");
                            }

                            xPathNavigator.MoveToRoot();

                            // first thing is to read and cache global fields
                            // these are duped for each document found
                            var globals = new ArrayList();
                            foreach (Rule rule in ruleSet.GlobalRules)
                            {
                                XPathExpression xPathExpression = xPathNavigator.Compile(rule.Xpath);
                                XPathNodeIterator iter = xPathNavigator.Select(xPathExpression);
                                while (iter.MoveNext())
                                {
                                    if (_verbose)
                                    {
                                        _messageStringBuilder.AppendFormat("Found field '{0}' value '{1}'\n", rule.Name, iter.Current.Value);
                                    }

                                    globals.Add(new RuleValue(rule, iter.Current.Value));
                                }
                            }

                            // ok, all globals cached. now to index documents

                            xPathNavigator.MoveToRoot();
                            foreach (DocItem docItem in ruleSet.DocItems)
                            {
                                XPathExpression docex = xPathNavigator.Compile(docItem.XPath);
                                XPathNodeIterator diter = xPathNavigator.Select(docex);

                                while (diter.MoveNext())
                                {
                                    XPathNavigator dn = diter.Current.Clone();

                                    if (_verbose)
                                    {
                                        _messageStringBuilder.Append("Found document\n");
                                    }

                                    // now compute key
                                    string key = "";
                                    if (docItem.Key != "")
                                    {
                                        //key = dn.Evaluate(doc.Key).ToString();

                                        object result = dn.Evaluate(docItem.Key);
                                        var iterator = result as XPathNodeIterator;
                                        if (iterator != null)
                                        {
                                            while (iterator.MoveNext())
                                            {
                                                key = iterator.Current.ToString();
                                            }
                                        }
                                        else
                                        {
                                            key = result.ToString();
                                        }
                                    }
                                    else
                                    {
                                        key = fname;
                                    }

                                    if (_verbose)
                                    {
                                        _messageStringBuilder.AppendFormat("Key is {0}\n", key);
                                    }

                                    // delete if document exists
                                    DeleteDocument(writer, key);

                                    var ludoc = new Document();

                                    AddKeyField(ludoc, key);
                                    AddNameField(ludoc, ruleSet.Name);

                                    string textField = string.Empty;
                                    Rule ruleText = null;
                                    foreach (Rule rule in docItem.Rules)
                                    {
                                        XPathExpression expr = dn.Compile(rule.Xpath);
                                        XPathNodeIterator iter = dn.Select(expr);
                                        while (iter.MoveNext())
                                        {
                                            string textVal = parseHtml(iter.Current.Value);

                                            if (_verbose)
                                            {
                                                _messageStringBuilder.AppendFormat("Found field '{0}' value '{1}'\n", rule.Name, textVal);
                                            }

                                            // index away!
                                            if (rule.Name == "text")
                                            {
                                                if (ruleText == null)
                                                {
                                                    ruleText = rule;
                                                    textField = textVal;
                                                }
                                                else
                                                {
                                                    textField += " ... " + textVal;
                                                }
                                            }
                                            else
                                            {
                                                AddField(ludoc, rule, iter.Current.Value);
                                            }
                                        }
                                    }

                                    if (textField.Length > 0)
                                    {
                                        AddField(ludoc, ruleText, textField);
                                    }

                                    // now add the globals into this document
                                    foreach (RuleValue ruleValue in globals)
                                    {
                                        AddField(ludoc, ruleValue.Rule, ruleValue.Value);
                                    }

                                    writer.AddDocument(ludoc);
                                }
                            }
                        }
                    }
                    // end of fnames loop
                }
                catch (Exception e)
                {
                    failures.Add(String.Format("Failed to index file {0}, exception: {1}", fname, e));
                    if (_verbose)
                    {
                        _messageStringBuilder.AppendFormat("Failed to index {0}\n", fname);
                        _messageStringBuilder.Append(e.StackTrace + "\n");
                    }
                }
            }

            writer.Optimize();
            writer.Close();

            return failures;
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
        /// </summary>
        /// <param name="html">HTML document</param>
        /// <returns>Plain text.</returns>
        private string parseHtml(string html)
        {
            html = HttpContext.Current.Server.HtmlDecode(html);
            html = HttpContext.Current.Server.HtmlDecode(html);
            html = Regex.Replace(html, @"<([^>]|\s)*>", " "); //replace all HTML 
            return html;
        }
    }
}