<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO" %>

<script RunAt="server">
    private void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext httpContext = HttpContext.Current;
        if (httpContext.Request.ApplicationPath != null)
        {
            string currentUrl = HttpContext.Current.Request.Path;
            string file = httpContext.Server.MapPath(currentUrl.Substring(currentUrl.LastIndexOf("/", StringComparison.Ordinal) + 1));
            if (!File.Exists(file))
            {
                string path = currentUrl.Substring(httpContext.Request.ApplicationPath.Length).TrimStart('/').Replace(".aspx", string.Empty);
                string querystring = httpContext.Request.ServerVariables["QUERY_STRING"];
                string rewritePath = String.IsNullOrEmpty(querystring)
                    ? String.Format("~/default.aspx?process={0}", path)
                    : String.Format("~/default.aspx?process={0}&{1}", path, querystring);

                httpContext.RewritePath(rewritePath);
            }
        }
    }

    private void Application_Start(object sender, EventArgs e) { }

    private void Application_End(object sender, EventArgs e) { }

    private void Application_Error(object sender, EventArgs e) { }

    private void Session_Start(object sender, EventArgs e) { }

    private void Session_End(object sender, EventArgs e) { }
</script>
