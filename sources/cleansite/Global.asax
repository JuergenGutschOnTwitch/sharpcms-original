<%@ Application Language="C#" %>

<script RunAt="server">
    private void Application_BeginRequest(object sender, EventArgs e)
    {
        Sharpcms.Core.Core.Request(HttpContext.Current);
    }

    private void Application_Start(object sender, EventArgs e)
    {
        
    }
</script>