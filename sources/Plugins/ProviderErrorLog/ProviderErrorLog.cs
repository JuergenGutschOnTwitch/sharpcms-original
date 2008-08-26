using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Library;
using System.Xml;
using System.IO;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderErrorLog : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "ErrorLog";
            }
        }

        public ProviderErrorLog()
        {
        }

        public ProviderErrorLog(Process process)
        {
            m_Process = process;
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

        public void HandleLog()
        {
            XmlNode messagesNode = CommonXml.GetNode(m_Process.XmlData, "messages", EmptyNodeHandling.Ignore);
            if (messagesNode == null) 
            {
                return;
            }

            string logFileName = m_Process.Settings["errorlog/logpath"];

            XmlNodeList items = messagesNode.SelectNodes("item");
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

                if (!writtenToLogFile)
                {
                    if (CommonXml.GetAttributeValue(item, "messagetype") == "Error")
                    {
                        string errorType = CommonXml.GetAttributeValue(item, "type");
                        string message = item.InnerText;

                        File.AppendAllText(logFileName, string.Format("{0};{1};{2}\r\n", DateTime.Now.ToUniversalTime(), errorType, message));
                        CommonXml.SetAttributeValue(item, "writtenToLogFile", "true");
                    }
                }
            }
        }
    }
}