//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
/* $id */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem
{
    public static class Core
    {
        public static void Send(Page httpPage)
        {
            // Counter counter = new Counter();
            // counter.Start();

            PrepareConfiguration(httpPage);

            ProcessHandler processHandler = new ProcessHandler();
            Process process = processHandler.Run(httpPage);

            if (!process.OutputHandledByModule && process.RedirectUrl == null)
            {
                Parse(httpPage, process);

                // counter.Stop();
                //  httpPage.Response.Write(string.Format("<!-- {0} ms -->", counter.Milliseconds));
            }

            if (process.RedirectUrl != null)
            {
                httpPage.Response.Redirect(process.RedirectUrl);
            }
        }

        private static void PrepareConfiguration(Page httpPage)
        {
            Cache cache = new Cache(httpPage.Application);

            List<string> configurationPaths = new List<string>
                                                  {
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

                    // ToDo: dirty hack (old)
                    string[] badtags = {
                                           "<ul />", "<li />", "<h1 />", "<h2 />", "<h3 />", "<div />", "<p />",
                                           "<font />"
                                           , "<b />", "<strong />", "<i />"
                                       };
                    foreach (string a in badtags)
                    {
                        output = output.Replace(a, "");
                    }

                    Regex regex =
                        new Regex(
                            "(?<email>(mailto:)([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3}))",
                            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant |
                            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

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
            string t = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                int acode = Convert.ToInt32(text[i]);
                string repl = "&#" + Convert.ToString(acode) + ";";

                t += repl;
            }
            return t;
        }
    }
}