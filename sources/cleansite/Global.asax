<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO"%>

<script runat="server">

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext httpContext = HttpContext.Current;
            string currentURL = HttpContext.Current.Request.Path.ToLower();
            string processpath =
                currentURL.Substring(httpContext.Request.ApplicationPath.Length).TrimStart('/').ToLower();

            if (!File.Exists(httpContext.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/") + 1))))
                httpContext.RewritePath("~/default.aspx?process=" + processpath.Replace(".aspx", "") + "&" +
                                        httpContext.Request.ServerVariables["QUERY_STRING"]);
        }

        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
        }

        private void Application_End(object sender, EventArgs e)
        {
            // Code that runs on application shutdown
        }

        private void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
        }

        private void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        private void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }

</script>