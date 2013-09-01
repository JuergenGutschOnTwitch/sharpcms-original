// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Sharpcms.Base.Library;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Process;

namespace Sharpcms
{
    public static class Sharpcms
    {
        public static void Send(Page page)
        {
            PrepareConfiguration(page);

            ProcessHandler processHandler = new ProcessHandler();
            Process process = processHandler.Run(page);

            if (!process.OutputHandledByModule && process.RedirectUrl == null)
            {
                Parse(page, process);
            }

            if (process.RedirectUrl != null)
            {
                page.Response.Redirect(process.RedirectUrl);
            }
        }

        public static void Request(HttpContext httpContext, String entryPageName)
        {
            HttpRequest httpRequest = httpContext.Request;
            String applicationPath = httpRequest.ApplicationPath;

            if (applicationPath != null)
            {
                String currentUrl = httpRequest.Path;
                String file = httpContext.Server.MapPath(currentUrl.Substring(currentUrl.LastIndexOf("/", StringComparison.Ordinal) + 1));

                if (!File.Exists(file))
                {
                    String process = currentUrl.Substring(applicationPath.Length).TrimStart('/').Replace(".aspx", String.Empty);
                    String querystring = httpRequest.ServerVariables["QUERY_STRING"];
                    String rewritePath = !String.IsNullOrEmpty(querystring)
                        ? String.Format("~/{0}.aspx?process={1}&{2}", entryPageName, process, querystring)
                        : String.Format("~/{0}.aspx?process={1}", entryPageName, process);

                    httpContext.RewritePath(rewritePath);
                }
            }
        }

        private static void PrepareConfiguration(Page httpPage)
        {
            Cache cache = new Cache(httpPage.Application);

            List<String> configurationPaths = new List<String> {
                httpPage.Server.MapPath("~/Custom/Components"),
                httpPage.Server.MapPath("~/System/Components")
            };

            String[] settingsPaths = new String[3];
            configurationPaths.CopyTo(settingsPaths);
            settingsPaths[2] = httpPage.Server.MapPath("~/Custom/App_Data/CustomSettings.xml");
            Configuration.CombineSettings(settingsPaths, cache);

            String[] processPaths = new String[3];
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
                    String output = CommonXml.TransformXsl(process.MainTemplate, process.XmlData, process.Cache);

                    // ToDo: dirty hack
                    String[] badtags = { "<ul />", "<li />", "<h1 />", "<h2 />", "<h3 />", "<div />", "<p />", "<font />", "<b />", "<strong />", "<i />" };
                    
                    output = badtags.Aggregate(output, (current, a) => current.Replace(a, String.Empty));

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

        private static String HtmlObfuscate(String text)
        {
            return text.Select(t => String.Format("&#{0};", Convert.ToString(Convert.ToInt32(t)))).Aggregate(String.Empty, (current, repl) => current + repl);
        }
    }
}