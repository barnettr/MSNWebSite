//---------------------------------------------------------------------
// <copyright file="ListView.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the ListView class
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
using MS.Msn.InternetAccess.Hsl.WebSite.Session;
using MS.Msn.InternetAccess.Hsl.WebSite.Model;
using System.Text;
using MS.Msn.InternetAccess.Hsl.WebSite.Monitoring;
using Microsoft.Security.Application;

/// <summary>
/// Defines the ListView page's control and dynamic content
/// </summary>
public partial class Web_ListView : HSLView
{
    /// <summary>
    /// page selector sizes
    /// </summary>
    private static UInt32[] pageSizes = new UInt32[] { 10, 15, 20, 25 };

    /// <summary>
    /// the display of the distance column is conditional
    /// this is a boolean to specify whether it should be displayed
    /// </summary>
    private Boolean displayDistance = true;

    /// <summary>
    /// the display of the distance column is conditional
    /// this is a boolean to specify whether it should be displayed
    /// </summary>
    protected Boolean DisplayDistance
    {
        get
        {
            return this.displayDistance;
        }

        set
        {
            this.displayDistance = value;
        }
    }

    /// <summary>
    /// html markup of the hotspots list table
    /// </summary>
    /// <param name="tbl">the table of hotspots</param>
    /// <param name="label">the enumeration start</param>
    /// <returns>html for the hotspots table</returns>
    public String GetAsHTMLTable(HSLServices.TableDTO tbl, UInt32 label)
    {
        StringBuilder html = new StringBuilder(2048);

        object[] hotspots = null;

        if (null != tbl)
        {
            hotspots = tbl.DTOs;
        }

        if (null != this.InfoMessage)
        {
            html.AppendLine(String.Format("<tr><td colspan=\"6\">&nbsp;</td></tr><tr>{0}</tr>", this.RenderNotificationMessage("colspan=\"6\"")));
            html.AppendLine("<tr>");
            html.AppendLine("<td colspan=\"6\" height=\"" + (10 * 50) + "px\"></td>");
            html.AppendLine("</tr>");
        }
        else
        {
            UInt32 num = 1;
            String resultRowNum = String.Empty;

            // body
            foreach (HSLServices.HotspotDTO hotspot in hotspots)
            {
                LocalizedHotspotInfo info = GetLocalizedHotspotInfo(hotspot);
                resultRowNum = "resultRow" + num.ToString();

                html.AppendLine(String.Format("<tr id=\"{1}\" class=\"HslResults_Row\" onmouseover=\"className='HslResults_SelectedRow';\" onmouseout=\"className='HslResults_Row';\" onclick=\"onSelect({0});\">", label, resultRowNum));
                html.AppendLine(String.Format(" <td width=\"42px\" align=\"center\" valign=\"center\">&nbsp;<a href=# onFocus=\"SelectResultRow({1});\" onBlur=\"DeSelectResultRow({1});\" onClick=\"onSelect({2});\"><img src=\"images/numbers/{0}sm.gif\" border=\"0\"></a>&nbsp;</td>", num++, resultRowNum, label));

                html.AppendLine(String.Format(" <td width=\"300px\"><h5 class=\"line_height_standard\">{0}</h5></td>", AntiXss.HtmlEncode(hotspot.Name)));
                html.AppendLine(String.Format(
                                " <td width=\"500px\"><p class=\"line_height_standard\">{0}<br />{1}, {2} {3}, {4} {5}<br />{6}</p></td>",
                                AntiXss.HtmlEncode(hotspot.AddrStreet),
                                AntiXss.HtmlEncode(hotspot.City),
                                AntiXss.HtmlEncode(info.Region),
                                AntiXss.HtmlEncode(hotspot.PostalCode),
                                AntiXss.HtmlEncode(info.Country),
                                AntiXss.HtmlEncode(info.Phone),
                                GetSummaryServiceProviders(hotspot)));

                // distance is displayed conditionally
                if (true == this.displayDistance)
                {
                    Double distance = hotspot.Distance;
                    String unit = "km";
                    if ("united states" == hotspot.Country.Trim().ToLower())
                    {
                        distance = distance / 1.61;
                        unit = "mi";
                    }

                    html.AppendLine(String.Format(" <td width=\"70px\" align=\"center\">{0} {1}&nbsp;</td>", distance.ToString("N1"), unit));
                }

                html.AppendLine(String.Format(" <td width=\"70px\" valign=\"middle\" align=\"center\"><img src=\"images/cost/{0}\"></td>", GetImageForCost(hotspot.Cost)));

                // html.AppendLine(String.Format(" <td width=\"58px\" align=\"center\">{0}</td>", GetImageForSecurity(hotspot.Security)));

                html.AppendLine("</tr>");

                label++;
            }

            if (num < 10)
            {
                html.AppendLine("<tr>");
                html.AppendLine("<td colspan=\"6\" height=\"" + ((10 - num) * 50) + "px\"></td>");
                html.AppendLine("</tr>");
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// event handle for page load
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.ListViewRequests);
        this.StartAveragePerformanceCounterTicker();

        this.CheckColdStart(true);

        // get the search criteria
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;

        // ensure the view state is initialized
        if (null == this.UserState.ListView)
        {
            this.UserState.ListView = new ListViewState();
        }

        // we only display the distance column if city or postal code is specified in the criteria
        this.displayDistance = (null != criteria.City || null != criteria.PostalCode || -1 != criteria.Proximity);

        // synch the view
        this.UserState.ListView.SyncView(this.UserState);

        // for the case of zero results, set an info message
        if (0 == this.UserState.ListView.NumberOfResults)
        {
            this.InfoMessage = "No results found. Please refine your search criteria.";
        }

        // dispatch cmd
        String cmd = this.PreprocessStr(Request["cmd"]);

        if (null != cmd)
        {
            Boolean refreshData = false;

            switch (cmd)
            {
                case "mapview":
                    Server.Transfer("MapView.aspx");
                    break;

                case "detailsview":
                    UInt32 entry = 0;
                    UInt32.TryParse(Request["entry"], out entry);
                    Context.Items["entry"] = entry;
                    Server.Transfer("DetailsView.aspx");
                    break;

                case "searchview":
                    Server.Transfer("SearchView.aspx");
                    break;
            }

            // check if it is a sort
            if (cmd.StartsWith("sort"))
            {
                String sortBy = cmd.Substring(4, cmd.Length - 4);

                // toggle direction if it's on an existing sort
                if (sortBy == criteria.Sort.ToString())
                {
                    if (criteria.SortDir == HSLServices.SortDirection.Ascending)
                    {
                        criteria.SortDir = HSLServices.SortDirection.Descending;
                    }
                    else if (criteria.SortDir == HSLServices.SortDirection.Descending)
                    {
                        criteria.SortDir = HSLServices.SortDirection.Ascending;
                    }
                }
                else
                {
                    criteria.Sort = ConvertStrToSortOrder(sortBy + "|Ascending");
                    criteria.SortDir = HSLServices.SortDirection.Ascending;
                }

                criteria.StartAtRow = 1;

                refreshData = true;

                this.UserState.ListView.SyncView(this.UserState);
            }

            if (true == refreshData)
            {
                this.UserState.SearchResult = HSLAppModel.Instance.SearchHotspots(criteria);
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
        this.EndAveragePerformanceCounterTicker(HSLWebSitePerformanceCounters.AveListViewResponseTime);
    }

    /// <summary>
    /// Render the number of search results to HTML
    /// </summary>
    /// <returns>the HTML representing the number of search results</returns>
    protected override String RenderNumberOfResults()
    {
        return String.Format("{0} Results", this.UserState.ListView.NumberOfResults);
    }

    /// <summary>
    /// html markup the page size selector links
    /// </summary>
    /// <returns>the html markup for page selection links</returns>
    protected String RenderPageSizeSelectors()
    {
        StringBuilder html = new StringBuilder(128);
        UInt32 pageSize = this.UserState.ListView.PageSize;

        for (int i = 0; i < pageSizes.Length; i++)
        {
            if (pageSize == pageSizes[i])
            {
                html.Append(String.Format("<span>{0}&nbsp;</span>", pageSize));
            }
            else
            {
                UInt32 newEntry = this.UserState.SearchCriteria.StartAtRow - 1;
                newEntry = (newEntry / pageSizes[i]) * pageSizes[i];
                html.Append(String.Format("<span><a href=\"#\" onclick=\"onPageResize({0}, {1});\">{0}</a>&nbsp;</span>", pageSizes[i], newEntry));
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// html markup next/previous arrows and page indicator
    /// </summary>
    /// <returns>the html for next/previous and page indicator</returns>
    protected String RenderPageNextPrevious()
    {
        StringBuilder html = new StringBuilder(128);
        ListViewState view = this.UserState.ListView;

        // left arrow on or off
        if (view.CurrentPage > 1)
        {
            UInt32 entry = (view.CurrentPage - 2) * view.PageSize;
            html.Append(String.Format("<a href=\"#\" onclick=\"onPageNextPrev({0});\"><img border=\"0\" src=\"images/misc/ArrowLeft.gif\" align=\"bottom\" /></a>", entry));
        }
        else
        {
            html.Append("<img border=\"0\" src=\"images/misc/ArrowLeftOff.gif\" align=\"bottom\" />");
        }

        html.Append(String.Format(" Page {0} of {1} ", view.CurrentPage, view.NumberOfPages));

        // right arrow on or off
        if (view.CurrentPage < view.NumberOfPages)
        {
            UInt32 entry = view.CurrentPage * view.PageSize;
            html.Append(String.Format("<a href=\"#\" onclick=\"onPageNextPrev({0});\"><img border=\"0\" src=\"images/misc/ArrowRight.gif\" align=\"bottom\" /></a>", entry));
        }
        else
        {
            html.Append("<img border=\"0\" src=\"images/misc/ArrowRightOff.gif\" align=\"bottom\" />");
        }

        return html.ToString();
    }

    /// <summary>
    /// html markup of the hotspots list table
    /// </summary>
    /// <returns>the html for the hotspots as a table</returns>
    protected String RenderHotspotsTable()
    {
        return this.GetAsHTMLTable(this.UserState.SearchResult.Hotspots, this.UserState.SearchCriteria.StartAtRow - 1);
    }

    /// <summary>
    /// Render the sort arrow
    /// </summary>
    /// <param name="columnName">the column name we're currently rendering</param>
    /// <returns>the html for the sort arrow</returns>
    protected String RenderSort(String columnName)
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        String html = String.Empty;

        if (null != criteria && columnName == criteria.Sort.ToString())
        {
            if (criteria.SortDir == HSLServices.SortDirection.Descending)
            {
                html = String.Format("&nbsp;<img align=\"middle\" src=\"images/misc/ascending.gif\" />");
            }
            else
            {
                html = String.Format("&nbsp;<img align=\"middle\" src=\"images/misc/descending.gif\" />");
            }
        }
        
        return html;
    }

    /// <summary>
    /// gets a summary string for the service providers, 2 named providers and others
    /// as numeric
    /// </summary>
    /// <param name="hotspot">the input hotspot</param>
    /// <returns>a summary string representing the service providers</returns>
    private static String GetSummaryServiceProviders(HSLServices.HotspotDTO hotspot)
    {
        StringBuilder providers = new StringBuilder(128);

        providers.Append("Providers: ");

        for (int i = 0; i < 2 && i < hotspot.AccessPoints.Length; i++)
        {
            HSLServices.AccessPointDTO ap = (HSLServices.AccessPointDTO)hotspot.AccessPoints[i];

            if (i > 0)
            {
                providers.Append(", ");
            }

            providers.Append(ap.ServiceProvider);
        }

        if (3 == hotspot.AccessPoints.Length)
        {
            providers.Append(String.Format(", and {0} other...", hotspot.AccessPoints.Length - 2));
        }
        else if (3 < hotspot.AccessPoints.Length)
        {
            providers.Append(String.Format(", and {0} others...", hotspot.AccessPoints.Length - 2));
        }

        return providers.ToString();
    }
}
