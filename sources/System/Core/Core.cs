// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Sharpcms.Library;
using Sharpcms.Library.Common;
using Sharpcms.Library.Process;

namespace Sharpcms.Core
{
    public static class Core
    {
        public static void Send(Page httpPage)
        {
            PrepareConfiguration(httpPage);

            ProcessHandler processHandler = new ProcessHandler();
            Process process = processHandler.Run(httpPage);

            if (!process.OutputHandledByModule && process.RedirectUrl == null)
            {
                Parse(httpPage, process);
            }

            if (process.RedirectUrl != null)
            {
                httpPage.Response.Redirect(process.RedirectUrl);
            }
        }

        public static void Request(HttpContext httpContext)
        {
            if (httpContext.Request.ApplicationPath != null)
            {
                string currentUrl = HttpContext.Current.Request.Path;
                string file = httpContext.Server.MapPath(currentUrl.Substring(currentUrl.LastIndexOf("/", StringComparison.Ordinal) + 1));
                if (!File.Exists(file))
                {
                    string path = currentUrl.Substring(httpContext.Request.ApplicationPath.Length).TrimStart('/').Replace(".aspx", String.Empty);
                    string querystring = httpContext.Request.ServerVariables["QUERY_STRING"];
                    string rewritePath = String.IsNullOrEmpty(querystring)
                        ? String.Format("~/default.aspx?process={0}", path)
                        : String.Format("~/default.aspx?process={0}&{1}", path, querystring);

                    httpContext.RewritePath(rewritePath);
                }
            }
        }

        private static void PrepareConfiguration(Page httpPage)
        {
            Cache cache = new Cache(httpPage.Application);

            List<string> configurationPaths = new List<string> {
                httpPage.Server.MapPath("~/Custom/Components"),
                httpPage.Server.MapPath("~/System/Components")
            };

            string[] settingsPaths = new string[3];
            configurationPaths.CopyTo(settingsPaths);
            settingsPaths[2] = httpPage.Server.MapPath("~/Custom/App_Data/CustomSettings.xml");
            Configuration.CombineSettings(settingsPaths, cache);

            string[] processPaths = new string[3];
            configurationPaths.CopyTo(processPaths);
            processPaths[2] = httpPage.Server.MapPath("~/Custom/App_Data/CustomProcess.xml");
            Configuration.CombineProcessTree(processPaths, cache);
        }

        private static void Parse(Page httpPage, Process process)
        {
            if (process.QueryEvents["xml"] == "true")
            {
                httpPage.Response.AddHeader("Content-Type", "text/xml");
                httpPage.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                httpPage.Response.Write(process.XmlData.OuterXml);
            }
            else
            {
                if (process.MainTemplate != null)
                {
                    string output = CommonXml.TransformXsl(process.MainTemplate, process.XmlData, process.Cache);

                    // ToDo: dirty hack
                    string[] badtags = { "<ul />", "<li />", "<h1 />", "<h2 />", "<h3 />", "<div />", "<p />", "<font />", "<b />", "<strong />", "<i />" };
                    
                    output = badtags.Aggregate(output, (current, a) => current.Replace(a, string.Empty));

                    Regex regex = new Regex("(?<email>(mailto:)([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3}))", 
                        RegexOptions.IgnoreCase | 
                        RegexOptions.CultureInvariant | 
                        RegexOptions.IgnorePatternWhitespace | 
                        RegexOptions.Compiled);

                    foreach (Match match in regex.Matches(output))
                    {
                        output = output.Replace(match.Groups["email"].Value, HtmlObfuscate(match.Groups["email"].Value));
                    }

                    httpPage.Response.Write(output);
                }
            }
        }

        private static string HtmlObfuscate(string text)
        {
            return text.Select(t => string.Format("&#{0};", Convert.ToString(Convert.ToInt32(t)))).Aggregate(string.Empty, (current, repl) => current + repl);
        }
    }
}