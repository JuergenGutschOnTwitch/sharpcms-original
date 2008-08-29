/* $id */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using InventIt.SiteSystem.Library;

using System.Text.RegularExpressions;
using System.Web;

namespace InventIt.SiteSystem
{
    public static class Core
    {
        public static void Send(System.Web.UI.Page httpPage)
        {
            // Counter counter = new Counter();
            // counter.Start();

            PrepareConfiguration(httpPage);

            ProcessHandler processHandler = new ProcessHandler();
            Process process = processHandler.Run(httpPage);

            if (!process.OutputHandledByModule && process.RedirectUrl == null )
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

        private static void PrepareConfiguration(System.Web.UI.Page httpPage)
        {
            Cache cache = new Cache(httpPage.Application);

            List<string> configurationPaths = new List<string>();
            configurationPaths.Add(httpPage.Server.MapPath("~/Custom/Components"));
            configurationPaths.Add(httpPage.Server.MapPath("~/System/Components"));

            string[] settingsPaths = new string[3];
            configurationPaths.CopyTo(settingsPaths);
            settingsPaths[2] = httpPage.Server.MapPath("~/Custom/App_Data/CustomSettings.xml");
            Configuration.CombineSettings(settingsPaths, cache);

            string[] processPaths = new string[3];
            configurationPaths.CopyTo(processPaths);
            processPaths[2] = httpPage.Server.MapPath("~/Custom/App_Data/CustomProcess.xml");
            Configuration.CombineProcessTree(processPaths, cache);
        }

        private static void Parse(System.Web.UI.Page httpPage, Process process)
        {            
            if (process.QueryEvents["xml"] == "true")
            {
                httpPage.Response.AddHeader("Content-Type", "text/xml");
                httpPage.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                httpPage.Response.Write(process.XmlData.OuterXml);
            }
            else
            {
                if (process.mainTemplate != null)
                {
                    string output = CommonXml.TransformXsl(process.mainTemplate, process.XmlData, process.Cache);
                    // todo: dirty hack 
                    string[] badtags = { "<ul />", "<li />", "<h1 />", "<h2 />", "<h3 />", "<div />", "<p />", "<font />", "<b />", "<strong />", "<i />" };
                    foreach (string a in badtags)
                        output = output.Replace(a, "");

                    Regex regex = new Regex("(?<email>(mailto:)([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3}))", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
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
            int acode;
            string repl;
            string t = "";
            for (int i = 0; i < text.Length; i++)
            {
                acode = Convert.ToInt32(text[i]);

                repl = "&#" + Convert.ToString(acode) + ";";

                t += repl;

            }
            return t;
        }

    }
}