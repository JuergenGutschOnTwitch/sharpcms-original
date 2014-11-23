<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO"%>

<script runat="server">
    private void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext httpContext = HttpContext.Current;
        string currentURL = HttpContext.Current.Request.Path.ToLower();
        
        if (httpContext.Request.ApplicationPath == null) return;

        string file = httpContext.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/", StringComparison.Ordinal) + 1));
        if (!File.Exists(file))
        {
            string path = currentURL.Substring(httpContext.Request.ApplicationPath.Length).TrimStart('/').ToLower().Replace(".aspx", string.Empty);
            string querystring = httpContext.Request.ServerVariables["QUERY_STRING"];
            string rewritePath = String.IsNullOrEmpty(querystring) 
                ? String.Format("~/default.aspx?process={0}", path) 
                : String.Format("~/default.aspx?process={0}&{1}", path, querystring); 
            
            httpContext.RewritePath(rewritePath);
        }
    }

    private void Application_Start(object sender, EventArgs e) { }

    private void Application_End(object sender, EventArgs e) { }

    private void Application_Error(object sender, EventArgs e) { }

    private void Session_Start(object sender, EventArgs e) { }

    private void Session_End(object sender, EventArgs e) { }
</script>