//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using InventIt.SiteSystem.Data.SiteTree;
using InventIt.SiteSystem.Plugin;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderForm : BasePlugin2, IPlugin2
    {
        public ProviderForm()
        {
        }

        public ProviderForm(Process process)
        {
            _process = process;
        }

        #region IPlugin2 Members

        public new string Name
        {
            get { return "Form"; }
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
            var reply = new StringBuilder();
            for (int i = 0; i < _process.QueryData.Count; i++)
            {
                Query current = _process.QueryData[i];
                if (current.Name.StartsWith("form"))
                {
                    string fieldName = current.Name.Replace("form_", string.Empty).Replace("_", " ").Trim();
                    reply.AppendFormat("{0}: {1}\n", fieldName, current.Value);
                }
            }

            var message = new MailMessage
                              {
                                  From = new MailAddress(_process.Settings["mail/servermail"])
                              };
            message.To.Add(_process.Settings["mail/user"]);
            message.Subject = _process.Settings["mail/subject"];
            message.Body = reply.ToString();

            var smtpClient = new SmtpClient(_process.Settings["mail/smtp"]);
            if (_process.Settings["mail/smtpuser"] != string.Empty)
                smtpClient.Credentials = new NetworkCredential(_process.Settings["mail/smtpuser"],
                                                               _process.Settings["mail/smtppass"]);

            smtpClient.Send(message);

            string confirmMessage = _process.Settings["mail/confirm"];
            if (confirmMessage != string.Empty)
                _process.AddMessage(confirmMessage);
        }

        // >>> new save form handler by Kiho 
        private void HandleAddComment()
        {
            Page currentPage = new SiteTree(_process).GetPage(_process.QueryData["pageidentifier"]);
            string element = _process.QueryEvents["mainvalue"];
            string[] list = element.Split('_');
            string elementname = _process.QueryData["container_" + list[1]];

            HandleCommentElement(currentPage.Containers[int.Parse(list[1]) - 1].Elements.Create(elementname));
            currentPage.Save();
        }

        private void HandleCommentElement(Element element)
        {
            for (int i = 0; i < _process.QueryData.Count; i++)
            {
                Query query = _process.QueryData[i];
                string[] list = query.Name.Split('_');
                if (list.Length > 1)
                    if (list[0] == "element")
                        element[list[3]] = query.Value;
            }
            element["date"] = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }

        // <<< new save form handler by Kiho 
    }
}