<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO"%>

<script runat="server">
    private void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext httpContext = HttpContext.Current;
        string currentURL = HttpContext.Current.Request.Path.ToLower();
        
        if (httpContext.Request.ApplicationPath == null) return;
        
        string processpath = currentURL.Substring(httpContext.Request.ApplicationPath.Length).TrimStart('/').ToLower();

        if (!File.Exists(httpContext.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/", StringComparison.Ordinal) + 1))))
        {
            httpContext.RewritePath("~/default.aspx?process=" + processpath.Replace(".aspx", "") + "&" + httpContext.Request.ServerVariables["QUERY_STRING"]);
        }
    }

    private void Application_Start(object sender, EventArgs e) { }

    private void Application_End(object sender, EventArgs e) { }

    private void Application_Error(object sender, EventArgs e) { }

    private void Session_Start(object sender, EventArgs e) { }

    private void Session_End(object sender, EventArgs e) { }
</script>