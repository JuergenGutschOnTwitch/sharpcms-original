// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.IO;
using System.Xml;
using Sharpcms.Library.Common;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.ErrorLog
{
    public class ProviderErrorLog : BasePlugin2, IPlugin2
    {
        public ProviderErrorLog()
        {
            Process = null;
        }

        public ProviderErrorLog(Process process) : this()
        {
            Process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "ErrorLog"; }
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }

        #endregion

        private void HandleLog()
        {
            XmlNode messagesNode = CommonXml.GetNode(Process.XmlData, "messages", EmptyNodeHandling.Ignore);
            if (messagesNode == null)
            {
                return;
            }

            string logFileName = Process.Settings["errorlog/logpath"];

            XmlNodeList items = messagesNode.SelectNodes("item");
            if (items != null)
            {
                foreach (XmlNode item in items)
                {
                    bool writtenToLogFile = false;
                    try
                    {
                        if (CommonXml.GetAttributeValue(item, "writtenToLogFile") == "true")
                        {
                            writtenToLogFile = true;
                        }
                    }
                    catch
                    {
                        // Ignore
                    }

                    if (writtenToLogFile)
                    {
                        continue;
                    }

                    if (CommonXml.GetAttributeValue(item, "messagetype") != "Error")
                    {
                        continue;
                    }

                    string errorType = CommonXml.GetAttributeValue(item, "type");
                    string message = item.InnerText;
                    File.AppendAllText(logFileName, string.Format("{0};{1};{2}\r\n", DateTime.Now.ToUniversalTime(), errorType, message));
                    CommonXml.SetAttributeValue(item, "writtenToLogFile", "true");
                }
            }
        }
    }
}