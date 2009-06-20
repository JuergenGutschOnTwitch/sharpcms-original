//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Web.UI;
using InventIt.SiteSystem;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Core.Send(this);
    }
}