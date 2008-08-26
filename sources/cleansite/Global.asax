<%@ Application Language="C#" %>

<script runat="server">

    void Application_BeginRequest(object sender, EventArgs e)
    {
        System.Web.HttpContext httpContext = HttpContext.Current;
        String currentURL = httpContext.Request.Path.ToLower();

        string processPath = currentURL.Substring(httpContext.Request.ApplicationPath.Length).TrimStart('/').ToLower();
        string physicalPath = httpContext.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/") + 1));
        if (!System.IO.File.Exists(physicalPath))
        {
            string queryString  = httpContext.Request.ServerVariables["QUERY_STRING"];
            string defaultPage = "~/default.aspx?process=";
            if (processPath.EndsWith(".aspx"))
            {
                processPath = processPath.Substring(0, processPath.Length - ".aspx".Length);
            }
            httpContext.RewritePath(defaultPage + processPath + "&" + queryString);
        }
    }
    void Application_Start(object sender, EventArgs e) 
    {
        
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>


