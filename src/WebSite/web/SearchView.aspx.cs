//---------------------------------------------------------------------
// <copyright file="SearchView.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the SearchView class
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
using System.Text;
using System.Web.Services.Protocols;

using MS.Msn.InternetAccess.Hsl.WebSite.Model;
using MS.Msn.InternetAccess.Hsl.WebSite.Session;
using MS.Msn.InternetAccess.Hsl.WebSite.ViewState;
using MS.Msn.InternetAccess.Hsl.WebSite.Monitoring;
using Microsoft.Security.Application;

/// <summary>
/// Defines the SearchView page's control and dynamic content
/// </summary>
public partial class Web_SearchView : HSLView
{
    /// <summary>
    /// the available list of providers
    /// </summary>
    private object[] availableProviders = null;

    /// <summary>
    /// Countains the HSL location/country counts
    /// </summary>
    private HSLServices.HSLCountsDTO hslCount = null;

    /// <summary>
    /// The page load event handler
    /// On postback, it performs the search based on the search criteria and forwards to the ListView
    /// When an error occurs on the search it displays an inline error per hig specification
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.SearchViewRequests);
        this.StartAveragePerformanceCounterTicker();

        this.UserState = UserState.GetOrCreate(Context);

        // ensure we have a search view
        SearchViewState view = this.UserState.SearchView;
        if (null == this.UserState.SearchView)
        {
            view = new SearchViewState();
            this.UserState.SearchView = view;
        }

        // get the hotspots and country count
        if (null == this.hslCount)
        {
            this.hslCount = HSLAppModel.Instance.GetHSLCounts();
        }

        // check if it's a postback
        if (true == this.IsPostBack)
        {
            this.UserState.SearchCriteria = this.CriteriaFromRequest();
            HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;

            // if criteria is null then there is something invalid about the fields
            if (null == criteria)
            {
                this.ErrorMessage = "Invalid Search Criteria";
            }
            else
            {
                String searchType = Request["searchType"];

                if (null != searchType)
                {
                    switch (searchType)
                    {
                        case "basic":
                            view.SearchMode = SearchType.Basic;
                            break;

                        case "advanced":
                            view.SearchMode = SearchType.Advanced;
                            break;
                    }
                }

                // fetch hotspots and transfer the user to the list view
                try
                {
                    this.NewCriteria = true;
                    this.FetchHotspots(this.UserState);
                    Server.Transfer("ListView.aspx");
                }
                catch (SoapException ex)
                {
                    if (ex.Code.Name == SoapException.ClientFaultCode.Name &&
                        ex.Code.Namespace == SoapException.ClientFaultCode.Namespace)
                    {
                        String postalOrZip = "Zip";

                        if ("United States" != criteria.Country)
                        {
                            postalOrZip = "Postal";
                        }

                        if (false == String.IsNullOrEmpty(criteria.City) && true == String.IsNullOrEmpty(criteria.PostalCode))
                        {
                            this.ErrorMessage = "Invalid City";
                        }
                        else if (true == String.IsNullOrEmpty(criteria.City) && false == String.IsNullOrEmpty(criteria.PostalCode))
                        {
                            this.ErrorMessage = String.Format("Invalid {0} Code", postalOrZip);
                        }
                        else
                        {
                            this.ErrorMessage = String.Format("Invalid City and/or {0} Code", postalOrZip);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
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
        this.EndAveragePerformanceCounterTicker(HSLWebSitePerformanceCounters.AveSearchViewResponseTime);
    }

    /// <summary>
    /// Render the countries drop list into HTML
    /// </summary>
    /// <returns>The HTML markup for the countries select tag</returns>
    protected String RenderCountriesList()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        StringBuilder html = new StringBuilder(2048);
        String countryName = null;

        if (null != criteria)
        {
            countryName = criteria.Country;
        }

        html.AppendLine(String.Format("<option value=\"\"{0}>&lt;Choose One&gt;</option>", null == countryName ? "selected" : ""));

        object[] countries = HSLAppModel.Instance.GetCountries();

        if (null != countries)
        {
            foreach (HSLServices.CountryDTO country in countries)
            {
                html.AppendLine(String.Format("<option {0}>{1}</option>", countryName == country.CountryName ? "selected" : "", country.CountryName));
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// Renders the regions drop list based on current country
    /// </summary>
    /// <returns>The HTML markup for the regions drop list</returns>
    protected String RenderRegionsList()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        StringBuilder html = new StringBuilder(2048);
        String countryName = null;

        if (null != criteria && null != criteria.Country)
        {
            countryName = HSLAppModel.Instance.CountryNameToCode(criteria.Country);
        }

        // the first option in the list if <Choose One>
        html.AppendLine(String.Format("<option value=\"\" {0}></option>", null == countryName ? "selected" : ""));

        if (null != countryName)
        {
            String countryCode = HSLAppModel.Instance.CountryNameToCode(criteria.Country);

            // get the regions from the model, based on current countly selection
            object[] regions = HSLAppModel.Instance.GetRegions(countryCode);

            if (null != regions)
            {
                String selectedRegion = criteria.Region;

                foreach (HSLServices.RegionDTO region in regions)
                {
                    if (false == String.IsNullOrEmpty(region.RegionName))
                    {
                        html.AppendLine(String.Format("<option {0}>{1}</option>", (selectedRegion == region.RegionName) ? " selected" : "", region.RegionName));
                    }
                }

                return html.ToString();
            }
        }

        return "";
    }

    /// <summary>
    /// Render the city
    /// </summary>
    /// <returns>The selected city</returns>
    protected String RenderCity()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        String city = "";

        if (null != criteria && null != criteria.City)
        {
            city = criteria.City;
        }

        return AntiXss.HtmlEncode(city);
    }

    /// <summary>
    /// Render the postal code
    /// </summary>
    /// <returns>The current selected postal code</returns>
    protected String RenderPostalCode()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        String postalCode = "";

        if (null != criteria && null != criteria.PostalCode)
        {
            postalCode = criteria.PostalCode;
        }

        return AntiXss.HtmlEncode(postalCode);
    }

    /// <summary>
    /// Render the search type
    /// </summary>
    /// <returns>returns basic or advanced based on current search type</returns>
    protected String RenderSearchType()
    {
        String searchType = "basic";

        if (null != this.UserState.SearchView)
        {
            searchType = this.UserState.SearchView.SearchMode == SearchType.Basic ? "basic" : "advanced";
        }

        return searchType;
    }

    /// <summary>
    /// Render the area code
    /// </summary>
    /// <returns>returns the current area code</returns>
    protected String RenderAreaCode()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        String areaCode = "";

        if (null != criteria && null != criteria.AreaCode)
        {
            areaCode = criteria.AreaCode;
        }

        return AntiXss.HtmlEncode(areaCode);
    }

    /// <summary>
    /// Renders the distance drop list
    /// </summary>
    /// <returns>the list of distance options including the current selection</returns>
    protected String RenderDistance()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        StringBuilder html = new StringBuilder(1024);

        if (null != criteria)
        {
            this.RenderDistanceItem(html, criteria.Proximity, "-1", "");

            if ("United States" == criteria.Country)
            {
                this.RenderDistanceItem(html, criteria.Proximity,  "0.2", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "0.5", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "1.0", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "2.0", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "10", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "20", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "50", "miles");
                this.RenderDistanceItem(html, criteria.Proximity, "100", "miles");
            }
            else
            {
                this.RenderDistanceItem(html, criteria.Proximity,  "0.5", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "1.0", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "2.0", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "5.0", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "10", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "20", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "50", "km");
                this.RenderDistanceItem(html, criteria.Proximity, "100", "km");
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// Renders a distance HTML option tag
    /// </summary>
    /// <param name="html">the current HTML string builder</param>
    /// <param name="current">current selection</param>
    /// <param name="valStr">distance value</param>
    /// <param name="unit">unit in km or mile</param>
    protected void RenderDistanceItem(StringBuilder html, Double current, String valStr, String unit)
    {
        Double val = Convert.ToDouble(valStr);

        html.AppendLine(String.Format("<option value=\"{0}\"{1}>{2} {3}</option>", valStr, (current == val) ? " selected" : "", (val == -1) ? "" : valStr, unit));
    }

    /// <summary>
    /// Renders the current access fee selection
    /// </summary>
    /// <returns>the radio buttons HTML of the access fee</returns>
    protected String RenderAccessFee()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        StringBuilder html = new StringBuilder(128);
        
        String accessFee = "Both";

        if (null != criteria)
        {
            switch (criteria.Cost)
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
        }

        this.RenderAccessFeeItem(html, accessFee, "Free");
        this.RenderAccessFeeItem(html, accessFee, "Paid");
        this.RenderAccessFeeItem(html, accessFee, "Both"); 

        return html.ToString();
    }

    /// <summary>
    /// Renders an access fee option as HTML radio button
    /// </summary>
    /// <param name="html">the current HTML string builder</param>
    /// <param name="current">the current selection</param>
    /// <param name="text">the title for the access fee option (eg. Free/Paid/Both)</param>
    protected void RenderAccessFeeItem(StringBuilder html, String current, String text)
    {
      html.AppendLine(String.Format(
          "<input type=\"radio\" name=\"accessFee\" value=\"{0}\"{1} />{2}",
          text,
          current == text ? " checked" : "",
          text));
    }

    /// <summary>
    /// Renders the categories drop list into an HTML select
    /// </summary>
    /// <returns>The HTML option tags representing the criteria items</returns>
    protected String RenderCategories()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        StringBuilder html = new StringBuilder(2048);

        object[] categories = HSLAppModel.Instance.GetCategories();

        object[] currentCategories = null;
        if (null != criteria)
        {
            currentCategories = criteria.Categories;
        }

        if (null != categories)
        {
            foreach (HSLServices.CategoryDTO category in categories)
            {
                Boolean selected = false;

                if (null != currentCategories)
                {
                    foreach (String cat in currentCategories)
                    {
                        if (cat == category.CategoryName)
                        {
                            selected = true;
                            break;
                        }
                    }
                }

                html.AppendLine(String.Format("<input type=\"checkbox\" name=\"{1}\" {0} />{1}<br />", selected ? "checked" : "", AntiXss.HtmlEncode(category.CategoryName)));
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// Renders the services providers as a list of check boxes
    /// </summary>
    /// <returns>The HTML for the list of check boxes, including the selected providers</returns>
    protected String RenderServiceProviders()
    {
        HSLServices.SearchCriteriaDTO criteria = this.UserState.SearchCriteria;
        StringBuilder html = new StringBuilder(2048);
        object[] providers = null;

        if (null != criteria)
        {
            if (null != criteria.Country && null == criteria.Region)
            {
                providers = HSLAppModel.Instance.GetCountryServiceProviders(criteria.Country);
            }
            else if (null != criteria.Country && null != criteria.Region)
            {
                providers = HSLAppModel.Instance.GetRegionServiceProviders(criteria.Country, criteria.Region);

                // if we don't have regional providers (this is the case for non US/Canada regions)
                // then get the country providers
                if (null == providers)
                {
                    providers = HSLAppModel.Instance.GetCountryServiceProviders(criteria.Country);
                }
            }
        }

        object[] currentProviders = null;
        if (null != criteria)
        {
            currentProviders = criteria.ServiceProviders;
        }

        if (null != providers)
        {
            this.availableProviders = providers;

            Hashtable selectedProviders = new Hashtable();

            // build a hashtable for selected providers
            if (null != currentProviders)
            {
                foreach (String prov in currentProviders)
                {
                    selectedProviders[prov] = "chosen";
                }
            }

            UInt32 providerIndex = 0;

            foreach (HSLServices.ServiceProviderDTO provider in providers)
            {
                Boolean selected = (null != selectedProviders[provider.ProviderName]);

                html.AppendLine(String.Format("<input type=\"checkbox\" {1} onclick=\"onProviderCheckToggle({0});\" />{2}<br />", providerIndex, selected ? "checked" : "", AntiXss.HtmlEncode(provider.ProviderName)));

                providerIndex++;
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// Render the available service providers list as hidden pipe delimited str
    /// </summary>
    /// <returns>the hidden pipe delimited str</returns>
    protected String RenderAvailableProvidersState()
    {
        StringBuilder html = new StringBuilder(2048);
        String providers = "";

        if (null != this.availableProviders)
        {
            String[] choices = new String[this.availableProviders.Length];

            for (int i = 0; i < this.availableProviders.Length; i++)
            {
                choices[i] = ((HSLServices.ServiceProviderDTO)this.availableProviders[i]).ProviderName;
            }

            providers = GetAsPipeDelimitedStr(choices);
        }

        html.AppendLine(String.Format("<input id=\"{0}\" type=\"hidden\" value=\"{1}\" />", "availableProviders", providers));

        return html.ToString();
    }

    /// <summary>
    /// render the number of hotspots
    /// </summary>
    /// <returns>number of hotspots</returns>
    protected String RenderHotspotsCount()
    {
        if (null != this.hslCount)
        {
            return this.hslCount.TotalLocations.ToString("#,###");
        }

        return "";
    }

    /// <summary>
    /// render the number of countries
    /// </summary>
    /// <returns>number of countries</returns>
    protected String RenderCountriesCount()
    {
        if (null != this.hslCount)
        {
            return this.hslCount.TotalCountries.ToString();
        }

        return "";
    }
}
