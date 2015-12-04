//---------------------------------------------------------------------
// <copyright file="MapView.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the MapView class
// </summary>
//---------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MS.Msn.InternetAccess.Hsl.WebSite.ViewState;
using System.Text;
using MS.Msn.InternetAccess.Hsl.WebSite.Monitoring;

/// <summary>
/// Defines the MapView page's control and dynamic content rendering
/// </summary>
public partial class Web_MapView : HSLView
{
    /// <summary>
    /// Handle the page load event
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.MapViewRequests);
        this.StartAveragePerformanceCounterTicker();

        this.CheckColdStart(false);

        // ensure we have a map view state
        if (null == this.UserState.MapView)
        {
            this.UserState.MapView = new MapViewState();
        }

        // dispatch cmd
        String cmd = this.PreprocessStr(Request["cmd"]);

        if (null != cmd)
        {
            switch (cmd)
            {
                case "searchview":
                    Server.Transfer("SearchView.aspx");
                    break;

                case "listview":
                    if (true == this.ColdStart || true == this.NeedFetchRefresh)
                    {
                        this.FetchHotspots(this.UserState);
                    }

                    Server.Transfer("ListView.aspx");
                    break;

                case "detailsview":
                    String entryStr = this.PreprocessStr(Request["entry"]);
                    UInt32 entry = 1;
                    UInt32.TryParse(entryStr, out entry);

                    Context.Items["entry"] = entry;

                    if (true == this.ColdStart || true == this.NeedFetchRefresh)
                    {
                        this.FetchHotspots(this.UserState);                   
                    }

                    Server.Transfer("DetailsView.aspx");
                    break;
            }
        }
    }

    /// <summary>
    /// page load complete handler update the average performance counter
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        this.EndAveragePerformanceCounterTicker(HSLWebSitePerformanceCounters.AveMapViewResponseTime);
    }

    /// <summary>
    /// Render the number of search results to HTML
    /// </summary>
    /// <returns>the HTML representing the number of search results</returns>
    protected override String RenderNumberOfResults()
    {
        return String.Format("{0} Results", this.UserState.AvailableRows);
    }

}
