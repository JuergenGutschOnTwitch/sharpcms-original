using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Providers
{
    public class Transformer
    {
        //Handler mod -- new function
        public static string Send()
        {
            string ret = string.Empty;
            //PrepareConfiguration(httpCtx.Application, httpCtx.Server);

            ProcessHandler processHandler = new ProcessHandler();
            Process process = processHandler.Run(null);

            if (!process.OutputHandledByModule)
            {
                if (process.mainTemplate != null)
                {
                    ret = Parse(process);
                }
            }

            // make invalid all the class reference to old HttpContext when next run
            process.Context = null;
            return ret;
        }

        //private static void PrepareConfiguration(HttpApplicationState app, HttpServerUtility server)
        //{
        //    Cache cache = new Cache(app);

        //    List<string> configurationPaths = new List<string>();
        //    configurationPaths.Add(server.MapPath("~/Custom/Components"));
        //    configurationPaths.Add(server.MapPath("~/System/Components"));

        //    string[] settingsPaths = new string[3];
        //    configurationPaths.CopyTo(settingsPaths);
        //    settingsPaths[2] = server.MapPath("~/Custom/App_Data/CustomSettings.xml");
        //    Configuration.CombineSettings(settingsPaths, cache);

        //    string[] processPaths = new string[3];
        //    configurationPaths.CopyTo(processPaths);
        //    processPaths[2] = server.MapPath("~/Custom/App_Data/CustomProcess.xml");
        //    Configuration.CombineProcessTree(processPaths, cache);
        //}

        private static string Parse(Process process)
        {
            //string output = CommonXml.TransformXsl(process.mainTemplate, process.XmlData, process.Cache, firstRun);

            string output = CommonXml.TransformXsl(process);
            // todo: dirty hack 
            string[] badtags = { "<ul />", "<li />", "<h1 />", "<h2 />", "<h3 />", "<div />", "<p />", "<font />", "<b />", "<strong />", "<i />" };
            foreach (string a in badtags)
                output.Replace(a, "");

            return output;
        }
    }
}
