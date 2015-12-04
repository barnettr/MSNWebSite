//---------------------------------------------------------------------
// <copyright file="DetailsView.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the DetailsView class
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
using HSLServices;
using MS.Msn.InternetAccess.Hsl.WebSite.ViewState;
using MS.Msn.InternetAccess.Hsl.WebSite.Monitoring;
using Microsoft.Security.Application;

/// <summary>
/// Defines the DetailsView page's control and dynamic content
/// </summary>
public partial class Web_DetailsView : HSLView
{
    /// <summary>
    /// static table for category to photo lookup
    /// </summary>
    private static Hashtable categoryPhotoLookup = null;

    /// <summary>
    /// static table for amenity to photo lookup
    /// </summary>
    private static Hashtable amenityPhotoLookup = null;

    /// <summary>
    /// the target hotspot to display details for
    /// </summary>
    private HSLServices.HotspotDTO hotspot = null;

    /// <summary>
    /// render the hotspot information 
    /// </summary>
    /// <returns>the html to be rendered</returns>
    protected String RenderHotspotInformation()
    {
        StringBuilder html = new StringBuilder();

        this.RenderAsHidden(html, "cardName", this.hotspot.Name);

        String address = this.hotspot.AddrStreet + "\n" + this.hotspot.City + " " + this.hotspot.Region + " " + this.hotspot.PostalCode;
        this.RenderAsHidden(html, "cardAddress", address);
        this.RenderAsHidden(html, "cardPhone", this.hotspot.Phone);

        String amenities = String.Empty;
        foreach (HSLServices.AmenityDTO am in this.hotspot.Amenities)
        {
            if (!((null == am.Flag) || (true == am.Flag.HasValue && false == am.Flag.Value)))
            {
                if (!String.IsNullOrEmpty(amenities))
                {
                    amenities += ", ";
                }

                amenities += am.Name;
            }
        }

        this.RenderAsHidden(html, "cardAmenities", amenities);
        this.RenderAsHidden(html, "cardDescription", this.hotspot.AmenitiesComments);
        this.RenderAsHidden(html, "cardCoverage", this.hotspot.UseableAreasComments);
        this.RenderAsHidden(html, "cardCategories", this.hotspot.Category);

        String providers = String.Empty;
        foreach (HSLServices.AccessPointDTO ap in this.hotspot.AccessPoints)
        {
            if (!String.IsNullOrEmpty(providers))
            {
                providers += "\n";
            }

            providers += ap.ServiceProvider;
            providers += " (" + ConvertCostSchemeToStr(ap.Cost) + ")";
        }

        this.RenderAsHidden(html, "cardProviders", providers);

        return html.ToString();
    }

    /// <summary>
    /// event handler for page load
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.DetailsViewRequests);
        this.StartAveragePerformanceCounterTicker();

        this.CheckColdStart(true);

        // ensure we have a details view
        DetailsViewState view = this.UserState.DetailsView;

        if (null == view)
        {
            view = new DetailsViewState();
            this.UserState.DetailsView = view;
        }

        if (false == this.IsPostBack && null != Context.Items["entry"])
        {
            // get the entry from the items bag
            UInt32 entry = (UInt32)Context.Items["entry"];

            // if entry is out of bounds with respect to the current page then force it to be the first entry
            UInt32 relativeEntry = entry - (this.UserState.SearchCriteria.StartAtRow - 1);
            if (relativeEntry >= this.UserState.SearchResult.Hotspots.DTOs.Length)
            {
                relativeEntry = 0;
            }

            view.InfoCardEntry = relativeEntry + (this.UserState.SearchCriteria.StartAtRow - 1);

            this.hotspot = (HSLServices.HotspotDTO)this.UserState.SearchResult.Hotspots.DTOs[view.InfoCardEntry - this.UserState.SearchCriteria.StartAtRow + 1];
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
                    Server.Transfer("ListView.aspx");
                    break;

                case "mapview":
                    Server.Transfer("MapView.aspx");
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
        this.EndAveragePerformanceCounterTicker(HSLWebSitePerformanceCounters.AveDetailsViewResponseTime);
    }

    /// <summary>
    /// render the name of the hotspot
    /// </summary>
    /// <returns>hotspot name</returns>
    protected String RenderName()
    {
        if (null != this.hotspot)
        {
            return this.hotspot.Name;
        }

        return "";
    }

    /// <summary>
    /// render the number of service providers for the given hotspot
    /// </summary>
    /// <returns>number of service providers</returns>
    protected String RenderNumberOfProviders()
    {
        if (null != this.hotspot && null != this.hotspot.AccessPoints)
        {
            return String.Format(
                                 "{0}",
                                 this.hotspot.AccessPoints.Length.ToString(),
                                 this.hotspot.AccessPoints.Length > 1 ? "" : "");
        }

        return "0";
    }

    /// <summary>
    /// render the location photo path (used in image html tag)
    /// </summary>
    /// <returns>the qualified path to the location photo</returns>
    protected String RenderPhoto()
    {
        if (null != this.hotspot)
        {
            if (this.hotspot.LocationPhotoUrl == null)
            {
                return "images/categories/" + LookupCategoryPlaceHolderPhoto(this.hotspot.Category);
            }
            else
            {
                return this.hotspot.LocationPhotoUrl;
            }
        }

        return "images/categories/null.jpg";
    }

    /// <summary>
    /// html markup the hotspot addresss
    /// </summary>
    /// <returns>the html for hotspot address</returns>
    protected String RenderAddress()
    {
        StringBuilder html = new StringBuilder();

        if (null != this.hotspot)
        {
            LocalizedHotspotInfo info = GetLocalizedHotspotInfo(this.hotspot);

            html.AppendLine(
                String.Format(
                    "{0} <br />",
                    AntiXss.HtmlEncode(this.hotspot.AddrStreet)));

            html.AppendLine(
                String.Format(
                    "{0}, {1} {2}, {3}<br />{4}",
                    AntiXss.HtmlEncode(this.hotspot.City),
                    AntiXss.HtmlEncode(info.Region),
                    AntiXss.HtmlEncode(this.hotspot.PostalCode),
                    AntiXss.HtmlEncode(info.Country),
                    AntiXss.HtmlEncode(info.Phone)));
        }

        return html.ToString();
    }

    /// <summary>
    /// html markup for amenities photos
    /// </summary>
    /// <returns>the html for amenities</returns>
    protected String RenderAmenities()
    {
        StringBuilder html = new StringBuilder(128);

        if (null != this.hotspot && null != this.hotspot.Amenities)
        {
            for (int i = 0; i < this.hotspot.Amenities.Length; i++)
            {
                AmenityDTO amenity = (AmenityDTO)this.hotspot.Amenities[i];

                String photo = LookupAmenityPhoto(amenity.Name);

                if ((null == amenity.Flag) || (true == amenity.Flag.HasValue && false == amenity.Flag.Value))
                {
                    photo += "_off.gif";
                }
                else
                {
                    photo += ".gif";
                }

                if (i == this.hotspot.Amenities.Length - 1)
                {
                    html.AppendLine("<td align=\"right\" style=\"padding-right:6px;\">");
                    html.AppendLine(String.Format("<img alt=\"Restroom availability\" src=\"images/amenities/{0}\" />", photo));
                    html.AppendLine("</td>");
                }
                else if (i == this.hotspot.Amenities.Length - 2)
                {
                    html.AppendLine("<td align=\"right\" style=\"padding-right:0px;\">");
                    html.AppendLine(String.Format("<img alt=\"Smoking or Non-Smoking\" src=\"images/amenities/{0}\" />", photo));
                    html.AppendLine("</td>");
                }
                else if (i == this.hotspot.Amenities.Length - 3)
                {
                    html.AppendLine("<td align=\"center\" style=\"padding-left:0px;\">");
                    html.AppendLine(String.Format("<img alt=\"Food availability\" src=\"images/amenities/{0}\" />", photo));
                    html.AppendLine("</td>");
                }
                else if (i == this.hotspot.Amenities.Length - 4)
                {
                    html.AppendLine("<td align=\"left\" style=\"padding-left:0px;\">");
                    html.AppendLine(String.Format("<img alt=\"Beverage availability\" src=\"images/amenities/{0}\" />", photo));
                    html.AppendLine("</td>");
                }
                else
                {
                    html.AppendLine("<td style=\"padding-left:6px;\">");
                    html.AppendLine(String.Format("<img alt=\"Outlet availability\" src=\"images/amenities/{0}\" />", photo));
                    html.AppendLine("</td>");
                }
            }

            return html.ToString();
        }

        return "";
    }

    /// <summary>
    /// html markup the description section of the info card
    /// </summary>
    /// <returns>the html for the description section</returns>
    protected String RenderDescription()
    {
        if (null != this.hotspot && null != this.hotspot.AmenitiesComments && "" != this.hotspot.AmenitiesComments)
        {
            return this.RenderCardSection("<strong class=\"textHighlight\">Description</strong>", this.hotspot.AmenitiesComments);
        }

        return "";
    }

    /// <summary>
    /// html markup the coverage section of the info card
    /// </summary>
    /// <returns>the html string for coverage</returns>
    protected String RenderCoverage()
    {
        if (null != this.hotspot && null != this.hotspot.UseableAreasComments && "" != this.hotspot.UseableAreasComments)
        {
            return this.RenderCardSection("<strong class=\"textHighlight\">Coverage</strong>", this.hotspot.UseableAreasComments);
        }

        return "";
    }

    /// <summary>
    /// get the hotspot cateogy field
    /// </summary>
    /// <returns>the category field</returns>
    protected String RenderCategories()
    {
        if (null != this.hotspot)
        {
            return this.hotspot.Category;
        }

        return "";
    }

    /// <summary>
    /// html markup the hotspot service providers
    /// </summary>
    /// <returns>the html for the hotspot service providers</returns>
    protected String RenderProviders()
    {
        StringBuilder html = new StringBuilder(128);

        if (null != this.hotspot && null != this.hotspot.AccessPoints)
        {
            foreach (HSLServices.AccessPointDTO ap in this.hotspot.AccessPoints)
            {
                html.AppendLine("<tr>");

                html.Append(String.Format("<td style=\"padding-left:4px;\" width=\"60%\">{0}</td>", AntiXss.HtmlEncode(ap.ServiceProvider)));

                html.Append(String.Format("<td style=\"padding-right:4px;\" width=\"40%\" align=\"right\"><img src=\"images/cost/{0}\" border=\"0\" /></td>", GetImageForCost(ap.Cost)));

                html.AppendLine("</tr>");
            }
        }

        return html.ToString();
    }

    /// <summary>
    /// html markup the lat/long of the hotspot as hidden fields, for use by VE jscript
    /// </summary>
    /// <returns>the html representing hidden fields for lat/long</returns>
    protected String RenderLatLong()
    {
        StringBuilder html = new StringBuilder(128);

        if (null != this.hotspot)
        {
            this.RenderAsHidden(html, "latitude", this.hotspot.Latitude.ToString());
            this.RenderAsHidden(html, "longitude", this.hotspot.Longitude.ToString());
        }

        return html.ToString();
    }

    /// <summary>
    /// html markup a section of the info card
    /// </summary>
    /// <param name="title">section title eg. Description</param>
    /// <param name="details">the text details</param>
    /// <returns>the html markup for a section of the card info</returns>
    protected String RenderCardSection(String title, String details)
    {
        StringBuilder html = new StringBuilder(512);

        html.AppendLine(String.Format("{0}<br />", title));
        html.AppendLine(String.Format("{0}", details));

        return html.ToString();
    }

    /// <summary>
    /// perform a lookup for category name to placeholder photo
    /// </summary>
    /// <param name="categoryName">the category name</param>
    /// <returns>the placeholder photo file name</returns>
    private static String LookupCategoryPlaceHolderPhoto(String categoryName)
    {
        // TODO: put this initialization in Application scope on init
        if (null == categoryPhotoLookup)
        {
            categoryPhotoLookup = new Hashtable();

            categoryPhotoLookup["Unknown"] = "null.jpg";
            categoryPhotoLookup["Airport"] = "airport.jpg";
            categoryPhotoLookup["Bar"] = "bar.jpg";
            categoryPhotoLookup["Beach"] = "beach.jpg";
            categoryPhotoLookup["Boat / Ferry / Ship"] = "boat.jpg";
            categoryPhotoLookup["Bus Station"] = "bus-station.jpg";
            categoryPhotoLookup["Business Center"] = "business-center.jpg";
            categoryPhotoLookup["Cafe"] = "cafe.jpg";
            categoryPhotoLookup["Campground"] = "campground.jpg";
            categoryPhotoLookup["Car Wash"] = "car-wash.jpg";
            categoryPhotoLookup["Convention Center"] = "convention-center.jpg";
            categoryPhotoLookup["Disco"] = "disco.jpg";
            categoryPhotoLookup["Downtown Area"] = "downtown-area.jpg";
            categoryPhotoLookup["Gas Station"] = "gas-station.jpg";
            categoryPhotoLookup["Government Office"] = "govt-office.jpg";
            categoryPhotoLookup["Health Club"] = "health-club.jpg";
            categoryPhotoLookup["Hospital"] = "hospital.jpg";
            categoryPhotoLookup["Hotel / Resort"] = "hotel-resort.jpg";
            categoryPhotoLookup["Hotzone"] = "hotzone.jpg";
            categoryPhotoLookup["Internet Cafe"] = "internet-cafe.jpg";
            categoryPhotoLookup["Library"] = "library.jpg";
            categoryPhotoLookup["Marina"] = "marina.jpg";
            categoryPhotoLookup["Office Building"] = "office-building.jpg";
            categoryPhotoLookup["Other"] = "other.jpg";
            categoryPhotoLookup["Park"] = "park.jpg";
            categoryPhotoLookup["Phone Booth"] = "phone-booth.jpg";
            categoryPhotoLookup["Private Club"] = "private-club.jpg";
            categoryPhotoLookup["Pub"] = "pub.jpg";
            categoryPhotoLookup["Public Space / Public Building"] = "public-space.jpg";
            categoryPhotoLookup["Residence"] = "residence.jpg";
            categoryPhotoLookup["Residential Area"] = "residential-area.jpg";
            categoryPhotoLookup["Rest Area"] = "rest-area.jpg";
            categoryPhotoLookup["Restaurant"] = "restaurant.jpg";
            categoryPhotoLookup["RV Resort"] = "rv.jpg";
            categoryPhotoLookup["School / University"] = "school-university.jpg";
            categoryPhotoLookup["Sports Venue"] = "sports-venue.jpg";
            categoryPhotoLookup["Store / Shopping Mall"] = "store-shopping-mall.jpg";
            categoryPhotoLookup["Tourist Attraction"] = "tourist-attraction.jpg";
            categoryPhotoLookup["Train Station"] = "train-station.jpg";
            categoryPhotoLookup["Training Center"] = "training-center.jpg";
            categoryPhotoLookup["Travel Center / Truck Stop"] = "traveltruck.jpg";
            categoryPhotoLookup["Citywide Network"] = "citywide.jpg";
        }

        return (String)categoryPhotoLookup[categoryName];
    }

    /// <summary>
    /// lookup the amenity photo given its name
    /// </summary>
    /// <param name="amenityName">the amenity name</param>
    /// <returns>the photo file</returns>
    private static String LookupAmenityPhoto(String amenityName)
    {
        if (null == amenityPhotoLookup)
        {
            amenityPhotoLookup = new Hashtable();

            amenityPhotoLookup["Food"] = "sm_food";
            amenityPhotoLookup["Beverages"] = "sm_beverages";
            amenityPhotoLookup["Restrooms"] = "sm_restrooms";
            amenityPhotoLookup["Smoking"] = "sm_smoking";
            amenityPhotoLookup["Outlets"] = "sm_outlets";
        }

        return (String)amenityPhotoLookup[amenityName];
    }
}
