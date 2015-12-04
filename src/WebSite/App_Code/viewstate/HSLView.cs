//---------------------------------------------------------------------
// <copyright file="HSLView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the HSLView class
// </summary>
//---------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MS.Msn.InternetAccess.Common.Utilities;
using MS.Msn.InternetAccess.Hsl.WebSite.Model;
using MS.Msn.InternetAccess.Hsl.WebSite.Monitoring;
using MS.Msn.InternetAccess.Hsl.WebSite.Session;
using MS.Msn.InternetAccess.Hsl.WebSite.ViewState;
using Microsoft.Security.Application;

/// <summary>
/// struct to hold localized hotspot info
/// </summary>
public struct LocalizedHotspotInfo
{
    /// <summary>
    /// the country
    /// </summary>
    public String Country;

    /// <summary>
    /// the region
    /// </summary>
    public String Region;

    /// <summary>
    /// the phone
    /// </summary>
    public String Phone;
}

/// <summary>
/// Defines common functionality for all views
/// </summary>
public class HSLView : System.Web.UI.Page
{
    /// <summary>
    /// the default start row
    /// </summary>
    public const UInt32 DefaultStartRow = 0;

    /// <summary>
    /// the default max rows
    /// </summary>
    public const UInt32 DefaultMaxRows = 10;

    /// <summary>
    /// the current user state
    /// </summary>
    private UserState userState = null;

    /// <summary>
    /// indicates if this is a cold start
    /// </summary>
    private Boolean coldStart = false;

    /// <summary>
    /// indicates of hotspots were fetched
    /// </summary>
    private Boolean fetchedHotspots = false;

    /// <summary>
    /// indicates if hotspots need to be refetched
    /// </summary>
    private Boolean needFetchRefresh = false;

    /// <summary>
    /// indicates if we have a new criteria irrespective of pagination or sort
    /// </summary>
    private Boolean newCriteria = false;

    /// <summary>
    /// current error message if any
    /// </summary>
    private String errorMessage = null;

    /// <summary>
    /// current info message if any
    /// </summary>
    private String infoMessage = null;

    /// <summary>
    /// performance counter avergae time ticker
    /// </summary>
    private AccurateTimer averagePerformanceTicker = null;

    /// <summary>
    /// Default constructor
    /// </summary>
    public HSLView()
    {
    }

    /// <summary>
    /// the current user state
    /// </summary>
    protected UserState UserState
    {
        get
        {
            return this.userState;
        }

        set
        {
            this.userState = value;
        }
    }

    /// <summary>
    /// indicates if this is a cold start
    /// </summary>
    protected Boolean ColdStart
    {
        get
        {
            return this.coldStart;          
        }

        set
        {
            this.coldStart = value;
        }
    }

    /// <summary>
    /// indicates of hotspots were fetched
    /// </summary>
    protected Boolean FetchedHotspots
    {
        get
        {
            return this.fetchedHotspots;          
        }

        set
        {
            this.fetchedHotspots = value;
        }
    }

    /// <summary>
    /// indicates if hotspots need to be refetched
    /// </summary>
    protected Boolean NeedFetchRefresh
    {
        get
        {
            return this.needFetchRefresh;          
        }

        set
        {
            this.needFetchRefresh = value;
        }
    }

    /// <summary>
    /// indicates if we have a new criteria irrespective of pagination or sort
    /// </summary>
    protected Boolean NewCriteria
    {
        get
        {
            return this.newCriteria;          
        }

        set
        {
            this.newCriteria = value;
        }
    }

    /// <summary>
    /// current error message if any
    /// </summary>
    protected String ErrorMessage
    {
        get
        {
            return this.errorMessage;
        }

        set
        {
            this.errorMessage = value;
        }
    }

    /// <summary>
    /// current info message if any
    /// </summary>
    protected String InfoMessage
    {
        get
        {
            return this.infoMessage;          
        }

        set
        {
            this.infoMessage = value;
        }
    }

    /// <summary>
    /// performance counter avergae time ticker
    /// </summary>
    protected AccurateTimer AveragePerformanceTicker
    {
        get
        {
            return this.averagePerformanceTicker;
        }

        set
        {
            this.averagePerformanceTicker = value;
        }
    }

    /// <summary>
    /// localizes country sensitive fields in the hotspot dto
    /// </summary>
    /// <param name="hotspot">the hotspot dto</param>
    /// <returns>the localized hotspot info</returns>
    public static LocalizedHotspotInfo GetLocalizedHotspotInfo(HSLServices.HotspotDTO hotspot)
    {
        LocalizedHotspotInfo info = new LocalizedHotspotInfo();

        info.Country = hotspot.Country;
        info.Region = hotspot.Region;
        info.Phone = hotspot.Phone;

        if ("United States" == info.Country || "Canada" == info.Country)
        {
            info.Country = hotspot.CountryAbbr;
            info.Region = hotspot.RegionAbbr;
        }

        if (null != info.Phone && ("US" == info.Country || "CA" == info.Country))
        {
            String[] tokens = info.Phone.Split('-');

            if (tokens.Length == 3)
            {
                info.Phone = String.Format("({0}) {1}-{2}", tokens[0], tokens[1], tokens[2]);
            }
        }

        return info;
    }

    /// <summary>
    /// compares existing and new criteria without pagination or sort
    /// </summary>
    /// <param name="existing">existing criteria</param>
    /// <param name="criteria">new criteria</param>
    /// <returns>true if the two criteria are indentical, otherwise false</returns>
    public static Boolean SameCriteriaNoPaginationOrSort(HSLServices.SearchCriteriaDTO existing, HSLServices.SearchCriteriaDTO criteria)
    {
        Boolean res = false;

        if (null != existing && null != criteria)
        {
            res = (existing.Country == criteria.Country) &&
                   (existing.Region == criteria.Region) &&
                   (existing.City == criteria.City) &&
                   (existing.PostalCode == criteria.PostalCode) &&
                   (existing.Proximity == criteria.Proximity) &&
                   (existing.AreaCode == criteria.AreaCode) &&
                   (existing.Cost == criteria.Cost) &&
                   (GetAsPipeDelimitedStr(existing.Categories) == GetAsPipeDelimitedStr(criteria.Categories)) &&
                   (GetAsPipeDelimitedStr(existing.ServiceProviders) == GetAsPipeDelimitedStr(criteria.ServiceProviders));
        }

        return res;
    }

    /// <summary>
    /// compares existing and new criteria
    /// </summary>
    /// <param name="existing">existing criteria</param>
    /// <param name="criteria">new criteria</param>
    /// <returns>true if the two criteria are indentical, otherwise false</returns>
    public static Boolean SameCriteria(HSLServices.SearchCriteriaDTO existing, HSLServices.SearchCriteriaDTO criteria)
    {
        return SameCriteriaNoPaginationOrSort(existing, criteria) &&
               (existing.StartAtRow == criteria.StartAtRow) &&
               (existing.MaxRows == criteria.MaxRows) &&
               (existing.Sort == criteria.Sort) &&
               (existing.SortDir == criteria.SortDir);
    }

    /// <summary>
    /// map a given cost to an image, eg. Free, Paid or Both
    /// </summary>
    /// <param name="cost">the cost scheme</param>
    /// <returns>the HTML image</returns>
    public static String GetImageForCost(HSLServices.CostScheme cost)
    {
        String img = String.Empty;

        switch (cost)
        {
            case HSLServices.CostScheme.Free:
                img = "CostFree.png";
                break;

            case HSLServices.CostScheme.Pay:
                img = "CostPaid.png";
                break;

            case HSLServices.CostScheme.Both:
                img = "CostBoth.png";
                break;

            default:
                img = "CostUnknown.png";
                break;
        }

        return img;
    }

    /// <summary>
    /// Renders the given security scheme into an HTML image tag complete with ALT attribute
    /// for mouseover tool tip
    /// </summary>
    /// <param name="security">the security scheme</param>
    /// <returns>the HTML markup representing the image tag</returns>
    public static String GetImageForSecurity(HSLServices.SecurityScheme security)
    {
        String img = String.Empty;
        String tip = String.Empty;

        switch (security)
        {
            case HSLServices.SecurityScheme.None:
                img = "unlocked.png";
                tip = "Unsecure";
                break;

            case HSLServices.SecurityScheme.Secured:
                img = "locked.png";
                tip = "Secure";
                break;

            case HSLServices.SecurityScheme.Both:
                img = "mixed_locks.png";
                tip = "Mixed";
                break;

            default:
                img = "lock_unknown.png";
                tip = "Unknown";
                break;
        }

        return String.Format("<img width=\"18px\" height=\"18px\" src=\"images/security/{0}\" alt=\"{1}\" />", img, tip);
    }

    /// <summary>
    /// convert an array of strings into a pipe delimited string
    /// </summary>
    /// <param name="args">the array of string arguments</param>
    /// <returns>the pipe delimited string output</returns>
    public static String GetAsPipeDelimitedStr(object[] args)
    {
        StringBuilder str = new StringBuilder(1024);

        if (null != args)
        {
            foreach (String arg in args)
            {
                if (str.Length > 0)
                {
                    str.Append("|");
                }

                str.Append(arg);
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// convert a given cost scheme to a string
    /// </summary>
    /// <param name="cost">the cost scheme</param>
    /// <returns>the string representing the cost scheme</returns>
    public static String ConvertCostSchemeToStr(HSLServices.CostScheme cost)
    {
        String accessFee = "Free";

        switch (cost)
        {
            case HSLServices.CostScheme.Free:
                accessFee = "Free";
                break;

            case HSLServices.CostScheme.Pay:
                accessFee = "Paid";
                break;

            case HSLServices.CostScheme.Both:
                accessFee = "Both";
                break;
        }

        return accessFee;
    }

    /// <summary>
    /// convert string to cost scheme
    /// </summary>
    /// <param name="cost">the cost string</param>
    /// <returns>the cost scheme</returns>
    public static HSLServices.CostScheme ConvertStrToCostScheme(String cost)
    {
        HSLServices.CostScheme scheme = HSLServices.CostScheme.Unknown;

        switch (cost)
        {
            case "Free":
                scheme = HSLServices.CostScheme.Free;
                break;

            case "Paid":
                scheme = HSLServices.CostScheme.Pay;
                break;

            case "Both":
                scheme = HSLServices.CostScheme.Both;
                break;
        }

        return scheme;
    }

    /// <summary>
    /// convert given search type enumeration to string
    /// </summary>
    /// <param name="mode">the search type enumeration</param>
    /// <returns>the string</returns>
    public static String ConvertSearchTypeToStr(SearchType mode)
    {
        String searchType = "basic";

        switch (mode)
        {
            case SearchType.Basic:
                searchType = "basic";
                break;

            case SearchType.Advanced:
                searchType = "advanced";
                break;
        }

        return searchType;
    }

    /// <summary>
    /// convert search type string to search type enumeration
    /// </summary>
    /// <param name="searchType">the search type string</param>
    /// <returns>the search type enumeration</returns>
    public static SearchType ConvertStrToSearchType(String searchType)
    {
        SearchType mode = SearchType.Basic;

        switch (searchType)
        {
            case "basic":
                mode = SearchType.Basic;
                break;

            case "advanced":
                mode = SearchType.Advanced;
                break;
        }

        return mode;
    }

    /// <summary>
    /// convert the sort and direction enumeration to a pipe delimited string
    /// </summary>
    /// <param name="sort">the sort enumeration</param>
    /// <param name="dir">the dir of the sort</param>
    /// <returns>the pipe delimited string</returns>
    public static String ConvertSortToStr(HSLServices.SortOrder sort, HSLServices.SortDirection dir)
    {
        return String.Format("{0}|{1}", sort.ToString(), dir.ToString());
    }

    /// <summary>
    /// gets the help URL
    /// </summary>
    /// <returns>the help URL string</returns>
    public static String GetHelpURL()
    {
        return AntiXss.UrlEncode(ConfigurationManager.AppSettings["HelpURL"]);
    }

    /// <summary>
    /// convert sort and dir string to sort order enum
    /// </summary>
    /// <param name="sortAndDir">the pipe delimited sort order and dir</param>
    /// <returns>the sort order enum</returns>
    public static HSLServices.SortOrder ConvertStrToSortOrder(String sortAndDir)
    {
        String[] tokens = sortAndDir.Split('|');
        HSLServices.SortOrder order = HSLServices.SortOrder.None;

        switch (tokens[0])
        {
            case "Distance":
                order = HSLServices.SortOrder.Distance;
                break;

            case "Name":
                order = HSLServices.SortOrder.Name;
                break;

            case "Cost":
                order = HSLServices.SortOrder.Cost;
                break;

            case "Address":
                order = HSLServices.SortOrder.Address;
                break;

            case "Security":
                order = HSLServices.SortOrder.Security;
                break;

            default:
                order = HSLServices.SortOrder.None;
                break;
        }

        return order;
    }

    /// <summary>
    /// convert sort and dir string to sort direction enum
    /// </summary>
    /// <param name="sortAndDir">the pipe delimited sort and dir string</param>
    /// <returns>the sort direction enum</returns>
    public static HSLServices.SortDirection ConvertStrToSortDirection(String sortAndDir)
    {
        String[] tokens = sortAndDir.Split('|');
        HSLServices.SortDirection dir = HSLServices.SortDirection.Ascending;

        switch (tokens[1])
        {
            case "Ascending":
                dir = HSLServices.SortDirection.Ascending;
                break;

            case "Descending":
                dir = HSLServices.SortDirection.Descending;
                break;
        }

        return dir;
    }

    /// <summary>
    /// Starts the average performance counter ticker
    /// </summary>
    public void StartAveragePerformanceCounterTicker()
    {
        this.averagePerformanceTicker = new AccurateTimer();
    }

    /// <summary>
    /// Ends the average performance counter ticker, increments
    /// </summary>
    /// <param name="counterName">perf counter name</param>
    public void EndAveragePerformanceCounterTicker(String counterName)
    {
        HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(counterName, (Int64)this.averagePerformanceTicker.ElapsedMilliseconds);
    }

    /// <summary>
    /// Check if this is a cold start, and fetch data if we were asked to
    /// </summary>
    /// <param name="prefetchHotspots">indicates if we're allowed to fetch data</param>
    public void CheckColdStart(Boolean prefetchHotspots)
    {
        this.needFetchRefresh = false;

        // a cold start is when we don't have a session yet and
        // we're passed a cold start state on the URL
        // the cold start state (a collection of parameters) is used to enable
        // link bookmarks, and session timeout recovery
        this.coldStart = !UserState.IsAvailable(Context);

        // get or create a user session
        UserState ustate = UserState.GetOrCreate(Context);

        // get criteria from http request
        HSLServices.SearchCriteriaDTO criteria = this.CriteriaFromRequest();

        // if it's a cold start then init search criteria from request
        if (true == this.coldStart)
        {
            this.newCriteria = true;

            ustate.SearchCriteria = criteria;

            // prefetch data if we were asked
            if (true == prefetchHotspots)
            {
                this.FetchHotspots(ustate);
            }
            else
            {
                // since we're not allowed to prefetch and we know we need to
                // indicate this fact
                this.needFetchRefresh = true;
            }
        }

        // for a warm start, we check if request criteria is different from existing
        else
        {
            HSLServices.SearchCriteriaDTO existing = ustate.SearchCriteria;

            this.newCriteria = !SameCriteriaNoPaginationOrSort(existing, criteria);
          
            // is it different
            if (false == SameCriteria(existing, criteria))
            {
                // store the new criteria
                ustate.SearchCriteria = criteria;

                // prefetch data if we were asked
                if (true == prefetchHotspots)
                {
                    this.FetchHotspots(ustate);
                }
                else
                {
                    // since we're not allowed to prefetch and we know we need to
                    // indicate this fact
                    this.needFetchRefresh = true;
                }
            }
        }

        // retain the user state
        this.userState = ustate;
    }

    /// <summary>
    /// get the criteria from the request
    /// </summary>
    /// <returns>the search criteria</returns>
    public HSLServices.SearchCriteriaDTO CriteriaFromRequest()
    {
        HSLServices.SearchCriteriaDTO criteria = null;

        // ensure we have the mandatory country field
        if (null != Request["country"])
        {
            // ******************************************** //
            // * jump start the criteria from the Request * //
            // ******************************************** //
            criteria = new HSLServices.SearchCriteriaDTO();

            criteria.Country = this.PreprocessStr(Request["country"]);
            criteria.Region = this.PreprocessStr(Request["region"]);
            criteria.City = this.PreprocessStr(Request["city"]);
            criteria.PostalCode = this.PreprocessStr(Request["postalCode"]);

            Double distance = -1;
            Double.TryParse(Request["distance"], out distance);

            // ensure distance is within acceptable boundary for web site
            if (distance < -1)
            {
                distance = -1;
            }
            else if (distance > 100)
            {
                distance = 100;
            }

            criteria.Proximity = distance;

            // default cost criteria to both unless specified
            if (null == this.PreprocessStr(Request["accessFee"]))
            {
                criteria.Cost = HSLServices.CostScheme.Both;
            }
            else
            {
                criteria.Cost = ConvertStrToCostScheme(this.PreprocessStr(Request["accessFee"]));
            }

            // get sort order and direction
            if (null == this.PreprocessStr(Request["sort"]))
            {
                if (String.IsNullOrEmpty(criteria.City) && String.IsNullOrEmpty(criteria.PostalCode))
                {
                    criteria.Sort = HSLServices.SortOrder.Name;
                }
                else
                {
                    criteria.Sort = HSLServices.SortOrder.Distance;
                }

                criteria.SortDir = HSLServices.SortDirection.Ascending;
            }
            else
            {
                String sort = this.PreprocessStr(Request["sort"]);
                criteria.Sort = ConvertStrToSortOrder(sort);
                criteria.SortDir = ConvertStrToSortDirection(sort);
            }

            // the rest of the criteria is sensitive to the search type
            String searchType = this.PreprocessStr(Request["searchType"]);
            if (null == searchType)
            {
                searchType = "basic";
            }

            // ***************************************************************************** //
            // * area code, categories, and service providers are read for advanced search * //
            // * only, otherwise, they're nulled                                           * //
            // ***************************************************************************** //

            criteria.AreaCode = null;
            criteria.Categories = null;
            criteria.ServiceProviders = null;

            if ("advanced" == searchType)
            {
                criteria.AreaCode = this.PreprocessStr(Request["areaCode"]);

                String pipeArgs = this.PreprocessStr(Request["categoriesStr"]);
                if (null != pipeArgs)
                {
                    criteria.Categories = pipeArgs.Split('|');
                }

                pipeArgs = this.PreprocessStr(Request["serviceProvidersStr"]);
                if (null != pipeArgs)
                {
                    criteria.ServiceProviders = pipeArgs.Split('|');
                }
            }

            UInt32 startAtRow = DefaultStartRow;

            if (null != this.PreprocessStr(Request["startAtRow"]))
            {
                UInt32.TryParse(Request["startAtRow"], out startAtRow);
            }

            criteria.StartAtRow = startAtRow + 1;

            UInt32 maxRows = DefaultMaxRows;

            if (null != this.PreprocessStr(Request["maxRows"]))
            {
                UInt32.TryParse(Request["maxRows"], out maxRows);

                // ensure maxrows is valid, to protect against URL tampering
                if (10 != maxRows && 15 != maxRows && 20 != maxRows && 25 != maxRows)
                {
                    maxRows = DefaultMaxRows;
                }
            }

            criteria.MaxRows = maxRows;
        }

        return criteria;
    }

    /// <summary>
    /// preprocess a string, if it's empty or null then return null, otherwise return it
    /// </summary>
    /// <param name="arg">the string argument</param>
    /// <returns>the preprocessed string</returns>
    public String PreprocessStr(String arg)
    {
        if (String.IsNullOrEmpty(arg))
        {
            return null;
        }
        else
        {
            // protect against cross-site scripting, note: javascript symbols are case sensitive, so only lower case is checked
            if (arg.IndexOf("expression") > -1 || arg.IndexOf("javascript") > -1 || arg.IndexOf("jscript") > -1)
            {
                arg = null;
            }
        }

        return arg;
    }

    /// <summary>
    /// fetch the hotspots based on current criteria
    /// </summary>
    /// <param name="ustate">the current user state</param>
    public void FetchHotspots(UserState ustate)
    {
        if (null != ustate && null != ustate.SearchCriteria)
        {
            HSLServices.SearchCriteriaDTO criteria = ustate.SearchCriteria;

            // on a new search we ask for the number of available rows
            criteria.FetchRowsCount = this.newCriteria;

            // perform a search, and retain it in the session
            ustate.SearchResult = HSLAppModel.Instance.SearchHotspots(criteria);

            // indicate we fetched hotspots
            this.fetchedHotspots = true;

            // update available rows if this is a new search
            if (true == this.newCriteria)
            {
                ustate.AvailableRows = (UInt32)ustate.SearchResult.Hotspots.AvailableRows;
            }
        }
    }

    /// <summary>
    /// output cold start state as a collection of hidden variables
    /// </summary>
    /// <returns>the HTML string representing hidden variable for the cold start state</returns>
    public String RenderColdStartState()
    {
        StringBuilder html = new StringBuilder(1024);

        return this.RenderColdStartState(html);
    }

    /// <summary>
    /// output cold start state as a collection of hidden variables
    /// </summary>
    /// <param name="html">the HTML string builder</param>
    /// <returns>an HTML string representing the collection of hidden variables</returns>
    public String RenderColdStartState(StringBuilder html)
    {
        if (null != this.userState)
        {
            HSLServices.SearchCriteriaDTO criteria = this.userState.SearchCriteria;

            this.RenderAsHidden(html, "country", criteria.Country);
            this.RenderAsHidden(html, "region", criteria.Region);
            this.RenderAsHidden(html, "city", criteria.City);
            this.RenderAsHidden(html, "postalCode", criteria.PostalCode);
            this.RenderAsHidden(html, "distance", criteria.Proximity.ToString());
            this.RenderAsHidden(html, "areaCode", criteria.AreaCode);
            this.RenderAsHidden(html, "accessFee", ConvertCostSchemeToStr(criteria.Cost));

            if (null != criteria.Categories)
            {
                this.RenderAsHidden(html, "categoriesStr", GetAsPipeDelimitedStr(criteria.Categories));
            }

            this.RenderAsHidden(html, "sort", ConvertSortToStr(criteria.Sort, criteria.SortDir));

            if (null != criteria.ServiceProviders)
            {
                this.RenderAsHidden(html, "serviceProvidersStr", GetAsPipeDelimitedStr(criteria.ServiceProviders));
            }

            if (null != this.userState.SearchView)
            {
                this.RenderAsHidden(html, "searchType", ConvertSearchTypeToStr(this.userState.SearchView.SearchMode));
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// helper method to render a hidden variable key value pair
    /// </summary>
    /// <param name="html">the HTML string builder</param>
    /// <param name="key">the key</param>
    /// <param name="value">the value</param>
    public void RenderAsHidden(StringBuilder html, String key, String value)
    {
        if (null != value)
        {
            html.AppendLine(String.Format("<input id=\"{0}\" type=\"hidden\" name=\"{0}\" value=\"{1}\" />", AntiXss.HtmlAttributeEncode(key), AntiXss.HtmlAttributeEncode(value)));
        }
    }

    /// <summary>
    /// Render the selected providers as hidden pipe delimited str
    /// </summary>
    /// <returns>the hidden pipe delimited str</returns>
    public String RenderServiceProvidersState()
    {
        StringBuilder html = new StringBuilder(128);
        HSLServices.SearchCriteriaDTO criteria = this.userState.SearchCriteria;

        if (null != criteria)
        {
            if (null != criteria.ServiceProviders)
            {
                this.RenderAsHidden(html, "serviceProvidersStr", GetAsPipeDelimitedStr(criteria.ServiceProviders));
            }
            else
            {
                this.RenderAsHidden(html, "serviceProvidersStr", "");
            }
        }
        else
        {
            this.RenderAsHidden(html, "serviceProvidersStr", "");
        }

        return html.ToString();
    }

    /// <summary>
    /// render the start and max rows as hidden variables
    /// </summary>
    /// <returns>the HTML hidden vars for start and max rows</returns>
    public String RenderStartAndMaxRowsState()
    {
        StringBuilder html = new StringBuilder(128);

        UInt32 startAtRow = DefaultStartRow;
        UInt32 maxRows = DefaultMaxRows;

        if (null != this.userState.SearchCriteria)
        {
            startAtRow = this.userState.SearchCriteria.StartAtRow - 1;
            maxRows = this.userState.SearchCriteria.MaxRows;
        }

        this.RenderAsHidden(html, "startAtRow", startAtRow.ToString());
        this.RenderAsHidden(html, "maxRows", maxRows.ToString());

        return html.ToString();
    }

    /// <summary>
    /// render the number of available rows as a hidden variable
    /// </summary>
    /// <returns>the HTML string representing the number of available rows</returns>
    public String RenderAvailableRowsState()
    {
        StringBuilder html = new StringBuilder(128);

        this.RenderAsHidden(html, "availableRows", this.userState.AvailableRows.ToString());

        return html.ToString();
    }

    /// <summary>
    /// renders the current error/info message as a hig compliant notification message
    /// </summary>
    /// <param name="attributes">extra attributes that style the HTML out</param>
    /// <returns>the HTML markup of the message</returns>
    public String RenderNotificationMessage(String attributes)
    {
        String html = "";

        if (null != this.errorMessage)
        {
            html = String.Format("<td {0}><div class=\"Verdana_DarkGray_11_bold\" style=\"background-color:#FFAEB9; border:solid 1px #7695B2;\"><img width=\"16px\" height=\"16px\" src=\"images/misc/hig_notif_icn_critical.png\" align=\"left\" />&nbsp;{1}</div></td>", attributes, AntiXss.HtmlEncode(this.errorMessage));
        }
        else if (null != this.infoMessage)
        {
            html = String.Format("<td {0}><div class=\"Verdana_DarkGray_11_bold\" style=\"background-color:#FFFFAE; border:solid 1px #7695B2;\"><img width=\"16px\" height=\"16px\" src=\"images/misc/hig_notif_icn_info.png\" align=\"left\" />&nbsp;{1}</div></td>", attributes, AntiXss.HtmlEncode(this.infoMessage));
        }

        return html;
    }

    /// <summary>
    /// Render the number of search results into HTML
    /// </summary>
    /// <returns>the HTML string for the number of search results</returns>
    protected virtual String RenderNumberOfResults()
    {
        if (null != this.userState)
        {
            return this.userState.AvailableRows.ToString();
        }

        return "0";
    }
}
