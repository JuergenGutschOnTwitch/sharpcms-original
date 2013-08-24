// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Web.UI;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Sharpcms.Core.Core.Send(this);
    }
}