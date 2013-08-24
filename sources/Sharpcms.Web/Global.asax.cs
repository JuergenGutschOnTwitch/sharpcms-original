// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Web;

namespace Sharpcms.Web
{
    public class Global : HttpApplication
    {
        private const String EntryPageName = ("Default");

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Sharpcms.Request(HttpContext.Current, EntryPageName);
        }
    }
}