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