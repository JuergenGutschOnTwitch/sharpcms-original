// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Xml;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;

namespace Sharpcms.Library.Process
{
    public class ProcessHandler
    {
        private PluginServices _plugins;

        public Process Run(Page httpPage)
        {
            _plugins = new PluginServices();
            Process process = new Process(httpPage, _plugins);
            _plugins.InvokeAll("system", "init");

            XmlDocument xmlDocument = process.Cache["process"] as XmlDocument;
            if (xmlDocument != null)
            {
                XmlNode xmlNode = xmlDocument.DocumentElement;
                string[] args = process.CurrentProcess.Trim('/').Split('/');

                LoopThroughProcess(args, xmlNode, process);
            }

            _plugins.InvokeAll("system", "exit");
            _plugins.ClosePlugins();

            return process;
        }

        private IPlugin GetProvider(string name)
        {
            AvailablePlugin plugin = _plugins.AvailablePlugins.Find(name);

            return plugin != null ? plugin.Instance : null;
        }

        private void LoopThroughProcess(string[] args, XmlNode xmlNode, Process process)
        {
            if (args[0] != string.Empty)
            {
                args[0] = CommonXml.RenameIntegerPath(args[0]);
                xmlNode = xmlNode.SelectSingleNode(args[0]);

                if (xmlNode != null)
                {
                    if (process.CheckGroups(CommonXml.GetAttributeValue(xmlNode, "rights")))
                    {
                        XmlNodeList contentNodes = xmlNode.SelectNodes("*");

                        LoopThroughProcessOneByOne(contentNodes, "template", process, args);
                        LoopThroughProcessOneByOne(contentNodes, "handle", process, args);
                        LoopThroughProcessOneByOne(contentNodes, "redirect", process, args);
                        LoopThroughProcessOneByOne(contentNodes, "load", process, args);

                        args = Common.Common.RemoveOne(args);

                        if (args != null)
                        {
                            LoopThroughProcess(args, xmlNode, process);
                        }
                    }
                    else
                    {
                        string redirectUrl = GetRedirectUrl(process.CurrentProcess);

                        process.HttpPage.Response.Redirect(redirectUrl); // ToDo: is this the way to do it
                    }
                }
            }
        }

        private static string GetRedirectUrl(string currentProcess)
        {
            string redirectUrl = currentProcess;

            if (redirectUrl.Trim().EndsWith("/"))
            {
                redirectUrl = redirectUrl.Remove(redirectUrl.Length - 1, 1);
            }

            return string.Format("login/?redirect={0}", redirectUrl);
        }

        private void LoopThroughProcessOneByOne(XmlNodeList contentNodes, String type, Process process, string[] args)
        {
            // not getting prettier ;-) mah
            foreach (XmlNode contentNode in contentNodes)
            {
                // Not very pretty, I know, but it seems to be the
                // best way to allow for proper debugging
                if (type == contentNode.Name)
                {
                    if (process.HttpPage.Request.ServerVariables["REMOTE_ADDR"] == "127.0.0.1")
                    {
                        switch (contentNode.Name)
                        {
                            case "load":
                                HandlePlugin(contentNode, args, process);
                                break;
                            case "handle":
                                HandlePlugin(contentNode, args, process);
                                break;
                            case "template":
                                if (contentNode.Attributes != null)
                                {
                                    process.MainTemplate = process.Settings["templates/" + contentNode.Attributes["name"].Value];
                                }
                                break;
                            case "redirect":
                                if (contentNode.Attributes != null)
                                {
                                    process.HttpPage.Response.Redirect(contentNode.Attributes["href"].Value.IndexOf("http://", StringComparison.Ordinal) > -1 
                                        ? contentNode.Attributes["href"].Value
                                        : process.GetUrl(contentNode.Attributes["href"].Value));
                                }
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            switch (contentNode.Name)
                            {
                                case "load":
                                    HandlePlugin(contentNode, args, process);
                                    break;
                                case "handle":
                                    HandlePlugin(contentNode, args, process);
                                    break;
                                case "template":
                                    if (contentNode.Attributes != null)
                                    {
                                        process.MainTemplate = process.Settings["templates/" + contentNode.Attributes["name"].Value];
                                    }
                                    break;
                                case "redirect":
                                    if (contentNode.Attributes != null)
                                    {
                                        process.HttpPage.Response.Redirect(process.GetUrl(contentNode.Attributes["href"].Value));
                                    }
                                    break;
                            }
                        }
                        catch (Exception exception)
                        {
                            MailStackTrace(process, exception);
                            process.AddMessage(exception);
                        }
                    }
                }
            }
        }

        private void HandlePlugin(XmlNode contentNode, string[] args, Process process)
        {
            if (contentNode.Attributes != null)
            {
                IPlugin provider = GetProvider(contentNode.Attributes["provider"].Value);
                if (provider != null)
                {
                    switch (contentNode.Name)
                    {
                        case "load":
                            ControlList control = process.Content.GetSubControl(CommonXml.GetAttributeValue(contentNode, "place"));
                            string action = CommonXml.GetAttributeValue(contentNode, "action");
                            string value = GetValue(contentNode, process);
                            string pathTrail = JoinPath(Common.Common.RemoveOne(args));
                            if (provider is IPlugin2)
                            {
                                ((IPlugin2) provider).Load(control, action, value, pathTrail);
                            }
                            else
                            {
                                provider.Load(control, action, pathTrail);
                            }
                            break;
                        case "handle":
                            string mainEvent = process.QueryEvents["main"];
                            if (mainEvent != string.Empty)
                            {
                                provider.Handle(mainEvent);
                            }
                            break;
                    }
                }
            }
        }

        private static string GetValue(XmlNode contentNode, Process process)
        {
            StringBuilder value = new StringBuilder(CommonXml.GetAttributeValue(contentNode, "value"));
            if (value.ToString() == string.Empty)
            {
                // No value is specified. Maybe a variable was requested?
                string variable = CommonXml.GetAttributeValue(contentNode, "variable");
                if (variable != string.Empty)
                {
                    value = new StringBuilder(process.Variables[variable]);
                }
            }
            else
            {
                ReplaceVariables(process.Variables, value); // Replace variables
            }

            return value.ToString();
        }

        private static void ReplaceVariables(IDictionary<string, string> variables, StringBuilder value)
        {
            while (true)
            {
                int startCurlyBrace = value.ToString().IndexOf("{", StringComparison.Ordinal);
                if (startCurlyBrace < 0)
                {
                    break;
                }

                int endCurlyBrace = value.ToString().IndexOf("}", startCurlyBrace, StringComparison.Ordinal);
                int stringLength = endCurlyBrace - startCurlyBrace - 1;
                string variable = value.ToString().Substring(startCurlyBrace + 1, stringLength);
                
                value.Replace("{" + variable + "}", variables.ContainsKey(variable) ? variables[variable] : string.Empty);
            }
        }

        private static string JoinPath(string[] args)
        {
            string path = args != null 
                ? string.Join("/", args) 
                : string.Empty;

            return path;
        }

        private static void MailStackTrace(Process process, Exception exception)
        {
            // Try sending an email with the stack trace
            try
            {
                if (process.Settings["site/stacktrace/recipient"] != string.Empty)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(process.Settings["site/stacktrace/recipient"]);
                    mail.From = new MailAddress(process.Settings["site/stacktrace/sender"]);
                    mail.IsBodyHtml = false;
                    mail.Body = FormatStackTrace(process.Settings["site/stacktrace/body"], exception, process);
                    mail.Subject = FormatStackTrace(process.Settings["site/stacktrace/subject"], exception, process);

                    SmtpClient smtpClient = new SmtpClient(process.Settings["mail/smtp"]);
                    if (process.Settings["mail/smtpuser"] != string.Empty)
                    {
                        smtpClient.Credentials = new NetworkCredential(process.Settings["mail/smtpuser"], process.Settings["mail/smtppass"]);
                    }
                    smtpClient.Send(mail);
                }
            }
            catch
            {
                // Ignore
            }
        }

        private static string FormatStackTrace(string original, Exception exception, Process process)
        {
            List<string> lines = new List<string>(original.Trim().Split('\n'));
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Trim();
            }

            string output = string.Join("\n", lines.ToArray());

            output = output.Replace("{type}", exception.GetType().FullName);
            output = output.Replace("{message}", exception.Message);
            output = output.Replace("{stacktrace}", exception.StackTrace);
            output = output.Replace("{url}", process.HttpPage.Request.Url.OriginalString);
            output = output.Replace("{domain}", process.HttpPage.Request.Url.Host);
            output = output.Replace("{process}", process.CurrentProcess);
            output = output.Replace("{user}", process.CurrentUser);
            output = output.Replace("{params}", string.Join("\n", (from string key in process.HttpPage.Request.Params.Keys select string.Format("{0} = {1}", key, process.HttpPage.Request.Params[key])).ToArray()));

            return output;
        }
    }
}