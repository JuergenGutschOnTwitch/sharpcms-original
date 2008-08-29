//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net.Mail;
using System.Net;
using InventIt.SiteSystem;
using InventIt.SiteSystem.Plugin;
using InventIt.SiteSystem.Library;
using InventIt.SiteSystem.Data.SiteTree;

namespace InventIt.SiteSystem.Providers
{
    public class ProviderForm : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "Form";
            }
        }

        public ProviderForm()
        {
        } 

        public ProviderForm(Process process)
        {
            m_Process = process;
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

        public void HandleSubmitForm()
        {
			StringBuilder reply = new StringBuilder();
            for (int i = 0; i < m_Process.QueryData.Count; i++)
            {
                Query current = m_Process.QueryData[i];
                if (current.Name.StartsWith("form"))
                {
					string fieldName = current.Name.Replace("form_", string.Empty).Replace("_", " ").Trim();
					reply.AppendFormat("{0}: {1}\n", fieldName, current.Value);
                }
            }

			MailMessage message = new MailMessage();
			message.From = new MailAddress(m_Process.Settings["mail/servermail"]);
			message.To.Add(m_Process.Settings["mail/user"]);
			message.Subject = m_Process.Settings["mail/subject"];
			message.Body = reply.ToString();

            SmtpClient smtpClient = new SmtpClient(m_Process.Settings["mail/smtp"]);
            if (m_Process.Settings["mail/smtpuser"] != string.Empty)
            {
                smtpClient.Credentials = new NetworkCredential(
                    m_Process.Settings["mail/smtpuser"],
                    m_Process.Settings["mail/smtppass"]);
            }
            smtpClient.Send(message); 

            string confirmMessage = m_Process.Settings["mail/confirm"];
            if (confirmMessage != string.Empty)
            {
                m_Process.AddMessage(confirmMessage); 
            }
        }  

        // >>> new save form handler by Kiho 
        private void HandleAddComment()
        {
            Page CurrentPage = new SiteTree(m_Process).GetPage(m_Process.QueryData["pageidentifier"]);
            string element = m_Process.QueryEvents["mainvalue"];
            string[] a_list = element.Split('_');
            string elementname = m_Process.QueryData["container_" + a_list[1]];

            HandleCommentElement(CurrentPage.Containers[int.Parse(a_list[1]) - 1].Elements.Create(elementname));
            CurrentPage.Save();
        }

        private void HandleCommentElement(Element element)
        {
            for (int i = 0; i < m_Process.QueryData.Count; i++)
            {
                Query query = m_Process.QueryData[i];
                string[] a_list = query.Name.Split('_');

                if (a_list.Length > 1)
                {
                    if (a_list[0] == "element")
                    {
                        element[a_list[3]] = query.Value;
                    }
                }
            }
            element["date"] = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

        }
        // <<< new save form handler by Kiho 
    }
}