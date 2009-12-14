//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

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
using Env = System.Environment;

namespace InventIt.SiteSystem.Providers
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
        private readonly string key;
        private readonly ArrayList rules = new ArrayList();
        private readonly string xPath;

        public DocItem(string xPath, string key)
        {
            this.xPath = xPath;
            this.key = key;
        }

        public ArrayList Rules
        {
            get { return rules; }
        }

        public string XPath
        {
            get { return xPath; }
        }

        public string Key
        {
            get { return key; }
        }

        private void AddRule(Rule r)
        {
            rules.Add(r);
        }

        public void AddRule(string name, string type, string xPath)
        {
            AddRule(new Rule(name, type, xPath));
        }

        public override string ToString()
        {
            var rs = new StringBuilder();
            rs.AppendFormat("DocItem: Key '{0}' XPath '{1}'" + Environment.NewLine, Key, XPath);
            rs.Append("Rules:" + Environment.NewLine);
            foreach (Rule r in Rules)
            {
                rs.Append(r + Environment.NewLine);
            }

            return rs.ToString();
        }
    }

    internal class RuleSet
    {
        private readonly ArrayList docItems = new ArrayList();
        private readonly ArrayList globalRules = new ArrayList();
        private readonly string name;
        private readonly string ns;

        public RuleSet(string name, string nameSpace)
        {
            this.name = name;
            ns = nameSpace;
        }

        public string Name
        {
            get { return name; }
        }

        private string Namespace
        {
            get { return ns; }
        }

        public ArrayList GlobalRules
        {
            get { return globalRules; }
        }

        public ArrayList DocItems
        {
            get { return docItems; }
        }

        private void AddGlobalRule(Rule r)
        {
            globalRules.Add(r);
        }

        public void AddGlobalRule(string name, string type, string xpath)
        {
            AddGlobalRule(new Rule(name, type, xpath));
        }

        public override string ToString()
        {
            var rs = new StringBuilder();
            rs.AppendFormat("RuleSet for document type '{0}', namespace '{1}'" + Environment.NewLine,
                            Name, Namespace);
            rs.Append("Global rules:" + Environment.NewLine);
            foreach (Rule r in globalRules)
            {
                rs.Append(r + Environment.NewLine);
            }

            rs.Append("Document items:" + Environment.NewLine);
            foreach (DocItem d in docItems)
            {
                rs.Append(d.ToString());
            }

            return rs.ToString();
        }

        public DocItem AddDocItem(string xp, string key)
        {
            var d = new DocItem(xp, key);
            docItems.Add(d);
            return d;
        }
    }

    public class Indexer
    {
        private static string RULENS = "";
        private readonly ArrayList fileList = new ArrayList();
        private readonly string indexName;
        private readonly ArrayList ruleSets;
        private readonly StringBuilder sbMsg;
        private readonly bool verbose;
        private string docRootDirectory;
        private string pattern;

        public Indexer(string name, bool verbose)
        {
            ruleSets = new ArrayList();
            this.verbose = verbose;
            indexName = name;
            sbMsg = new StringBuilder();
        }

        public Indexer(string name)
            : this(name, false)
        {
        }

        public string DocRootDirectory
        {
            get { return docRootDirectory; }
        }

        public string ProcMessage
        {
            get { return sbMsg.ToString(); }
        }

        /// <summary>
        /// Add HTML files from <c>directory</c> and its subdirectories that match <c>pattern</c>.
        /// </summary>
        /// <param name="directory">Directory with the HTML files.</param>
        /// <param name="pattern">Search pattern, e.g. <c>"*.html"</c></param>
        public void AddDirectory(DirectoryInfo directory, string pattern)
        {
            docRootDirectory = directory.FullName;
            this.pattern = pattern;

            AddSubDirectory(directory);
        }

        public void AddDirectory(string filePath, string pattern)
        {
            AddDirectory(new DirectoryInfo(filePath), pattern);
        }

        private void AddSubDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo fi in directory.GetFiles(pattern))
            {
                if (fileList.Contains(fi.FullName))
                    sbMsg.AppendFormat("Error : {0}\n", fi.FullName);
                else
                    fileList.Add(fi.FullName);
            }

            foreach (DirectoryInfo di in directory.GetDirectories())
                AddSubDirectory(di);
        }

        public void LoadRules(string filename)
        {
            var rd = new XPathDocument(new StreamReader(filename));
            XPathNavigator n = rd.CreateNavigator();
            XPathNodeIterator iter = n.Select("/rules/doctype");

            while (iter.MoveNext())
            {
                XPathNavigator cn = iter.Current.Clone();
                string root = cn.GetAttribute("root", RULENS);
                string ns = cn.GetAttribute("namespace", RULENS);
                var rset = new RuleSet(root, ns);
                XPathNodeIterator riter = cn.Select("rule");

                while (riter.MoveNext())
                {
                    rset.AddGlobalRule(riter.Current.GetAttribute("field", RULENS),
                                       riter.Current.GetAttribute("type", RULENS),
                                       riter.Current.GetAttribute("xpath", RULENS));
                }

                riter = cn.Select("document");

                while (riter.MoveNext())
                {
                    XPathNavigator dn = riter.Current.Clone();
                    string xpath = dn.GetAttribute("xpath", RULENS);
                    string key = dn.GetAttribute("key", RULENS);
                    DocItem d = rset.AddDocItem(xpath, key);
                    XPathNodeIterator diter = dn.Select("rule");
                    while (diter.MoveNext())
                    {
                        d.AddRule(diter.Current.GetAttribute("field", RULENS),
                                  diter.Current.GetAttribute("type", RULENS),
                                  diter.Current.GetAttribute("xpath", RULENS));
                    }
                }

                if (verbose)
                    sbMsg.Append(rset + "\n");

                ruleSets.Add(rset);
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
                    sbMsg.AppendFormat("Ignoring unknown rule type: {0}\n", rule);
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
            var r = new ArrayList { fname };
            IndexDocuments(r);
        }

        public ArrayList IndexDocuments()
        {
            return IndexDocuments(fileList);
        }

        public ArrayList IndexDocuments(ArrayList fnames)
        {
            var failures = new ArrayList();
            IndexModifier writer;

            if (File.Exists(Path.Combine(indexName, "segments")))
            {
                // create index if it doesn't exist
                writer = new IndexModifier(indexName, new StandardAnalyzer(), false);
            }
            else
            {
                writer = new IndexModifier(indexName, new StandardAnalyzer(), true);
            }

            foreach (string fname in fnames)
            {
                try
                {
                    if (verbose)
                    {
                        sbMsg.AppendFormat("Indexing file: {0}\n", fname);
                    }

                    var rd = new XPathDocument(new StreamReader(fname));
                    XPathNavigator n = rd.CreateNavigator();
                    n.MoveToFirstChild();
                    string docname = n.Name;

                    if (verbose)
                        sbMsg.AppendFormat("Found document type: {0}\n", docname);

                    foreach (RuleSet r in ruleSets)
                    {
                        if (r.Name == docname)
                        {
                            if (verbose)
                                sbMsg.Append("Found matching ruleset, indexing.\n");
                            n.MoveToRoot();

                            // first thing is to read and cache global fields
                            // these are duped for each document found
                            var globals = new ArrayList();
                            foreach (Rule rule in r.GlobalRules)
                            {
                                XPathExpression expr = n.Compile(rule.Xpath);
                                XPathNodeIterator iter = n.Select(expr);
                                while (iter.MoveNext())
                                {
                                    if (verbose)
                                        sbMsg.AppendFormat(
                                            "Found field '{0}' value '{1}'\n",
                                            rule.Name, iter.Current.Value);
                                    globals.Add(new RuleValue(rule, iter.Current.Value));
                                }
                            }

                            // ok, all globals cached. now to index documents

                            n.MoveToRoot();
                            foreach (DocItem doc in r.DocItems)
                            {
                                XPathExpression docex = n.Compile(doc.XPath);
                                XPathNodeIterator diter = n.Select(docex);

                                while (diter.MoveNext())
                                {
                                    XPathNavigator dn = diter.Current.Clone();

                                    if (verbose)
                                        sbMsg.Append("Found document\n");

                                    // now compute key
                                    string key = "";
                                    if (doc.Key != "")
                                    {
                                        //key = dn.Evaluate(doc.Key).ToString();

                                        object result = dn.Evaluate(doc.Key);
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

                                    if (verbose)
                                        sbMsg.AppendFormat("Key is {0}\n", key);

                                    // delete if document exists
                                    DeleteDocument(writer, key);

                                    var ludoc = new Document();

                                    AddKeyField(ludoc, key);
                                    AddNameField(ludoc, r.Name);

                                    string textField = string.Empty;
                                    Rule ruleText = null;
                                    foreach (Rule rule in doc.Rules)
                                    {
                                        XPathExpression expr = dn.Compile(rule.Xpath);
                                        XPathNodeIterator iter = dn.Select(expr);
                                        while (iter.MoveNext())
                                        {
                                            string textVal = parseHtml(iter.Current.Value);

                                            if (verbose)
                                                sbMsg.AppendFormat(
                                                    "Found field '{0}' value '{1}'\n",
                                                    rule.Name, textVal);
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
                                                AddField(ludoc, rule, iter.Current.Value);
                                        }
                                    }
                                    if (textField.Length > 0)
                                        AddField(ludoc, ruleText, textField);

                                    // now add the globals into this document
                                    foreach (RuleValue rval in globals)
                                    {
                                        AddField(ludoc, rval.Rule, rval.Value);
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
                    if (verbose)
                    {
                        sbMsg.AppendFormat("Failed to index {0}\n", fname);
                        sbMsg.Append(e.StackTrace + "\n");
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