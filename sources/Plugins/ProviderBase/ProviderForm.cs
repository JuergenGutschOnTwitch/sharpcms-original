// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Sharpcms.Data.SiteTree;
using Sharpcms.Library.Plugin;
using Sharpcms.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderForm : BasePlugin2, IPlugin2
    {
        public ProviderForm() { }

        public ProviderForm(Process process)
        {
            Process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get
            {
                return "Form";
            }
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                    // jig: add >>> new save form handler by Kiho
                case "addpagecomment":
                    HandleAddComment();
                    break;
                    // jig: add <<< new save form handler by Kiho

                case "submitform":
                    HandleSubmitForm();
                    break;
            }
        }

        #endregion

        private void HandleSubmitForm()
        {
            StringBuilder reply = new StringBuilder();
            for (int i = 0; i < Process.QueryData.Count; i++)
            {
                Query current = Process.QueryData[i];
                if (current.Name.StartsWith("form"))
                {
                    string fieldName = current.Name.Replace("form_", string.Empty).Replace("_", " ").Trim();
                    reply.AppendFormat("{0}: {1}\n", fieldName, current.Value);
                }
            }

            MailMessage message = new MailMessage { From = new MailAddress(Process.Settings["mail/servermail"]) };
            message.To.Add(Process.Settings["mail/user"]);
            message.Subject = Process.Settings["mail/subject"];
            message.Body = reply.ToString();

            SmtpClient smtpClient = new SmtpClient(Process.Settings["mail/smtp"]);
            if (Process.Settings["mail/smtpuser"] != string.Empty)
            {
                smtpClient.Credentials = new NetworkCredential(Process.Settings["mail/smtpuser"], Process.Settings["mail/smtppass"]);
            }

            smtpClient.Send(message);

            string confirmMessage = Process.Settings["mail/confirm"];
            if (confirmMessage != string.Empty)
            {
                Process.AddMessage(confirmMessage);
            }
        }

        // >>> new save form handler by Kiho 
        private void HandleAddComment()
        {
            Page currentPage = new SiteTree(Process).GetPage(Process.QueryData["pageidentifier"]);
            string element = Process.QueryEvents["mainvalue"];
            string[] list = element.Split('_');
            string elementname = Process.QueryData["container_" + list[1]];

            HandleCommentElement(currentPage.Containers[int.Parse(list[1]) - 1].Elements.Create(elementname));
            currentPage.Save();
        }

        private void HandleCommentElement(Element element)
        {
            for (int i = 0; i < Process.QueryData.Count; i++)
            {
                Query query = Process.QueryData[i];
                string[] list = query.Name.Split('_');
                if (list.Length > 1)
                {
                    if (list[0] == "element")
                    {
                        element[list[3]] = query.Value;
                    }
                }
                    
            }
            element["date"] = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }

        // <<< new save form handler by Kiho 
    }
}