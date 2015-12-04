<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Set the connection limit high so that calls to the 
        // MapPoint service is not the bottle neck
        Uri url = new Uri(ConfigurationManager.AppSettings["HSLServices.HSLServices"]);
        System.Net.ServicePoint sp = System.Net.ServicePointManager.FindServicePoint(url);
        sp.ConnectionLimit = 1000;
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        //get reference to the source of the exception chain
        Exception ex = Server.GetLastError().GetBaseException();
        MS.Msn.InternetAccess.Hsl.WebSite.Monitoring.HSLWebSiteErrorHandler.Instance.HandleException(ex);
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
