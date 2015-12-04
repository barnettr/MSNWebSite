//---------------------------------------------------------------------
// <copyright file="HSLAppModelFacade.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the HSLAppModelFacade class
// </summary>
//---------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

using HSLServices;
using MS.Msn.InternetAccess.Hsl.WebSite.Model;
using MS.Msn.InternetAccess.Hsl.WebSite.Session;
using MS.Msn.InternetAccess.Hsl.Website.Shared.Dto;

/// <summary>
/// Defines WebMethods that wrap the HSLAppModel, this is used to make client side model calls by Atlas
/// </summary>
[WebService(Namespace = "MS.Msn.InternetAccess.HSLServices")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class HSLAppModelFacade : System.Web.Services.WebService 
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public HSLAppModelFacade() 
    {
    }

    /// <summary>
    /// Get a list of countries
    /// </summary>
    /// <returns>array of country objects</returns>
    [WebMethod]
    public object[] GetCountries()
    {
        return HSLAppModel.Instance.GetCountries();
    }

    /// <summary>
    /// Given a country name get its regions
    /// </summary>
    /// <param name="countryName">the country name</param>
    /// <returns>array of region objects</returns>
    [WebMethod]
    public object[] GetRegions(String countryName)
    {
        return HSLAppModel.Instance.GetRegions(HSLAppModel.Instance.CountryNameToCode(countryName));
    }

    /// <summary>
    /// Get service providers given a country name
    /// </summary>
    /// <param name="countryName">the country name</param>
    /// <returns>array of service providers</returns>
    [WebMethod]
    public object[] GetCountryServiceProviders(String countryName)
    {
        return HSLAppModel.Instance.GetCountryServiceProviders(countryName);
    }

    /// <summary>
    /// Get service providers given a country and region name
    /// </summary>
    /// <param name="countryName">the country name</param>
    /// <param name="regionName">region name</param>
    /// <returns>array of service providers</returns>
    [WebMethod]
    public object[] GetRegionServiceProviders(String countryName, String regionName)
    {
        return HSLAppModel.Instance.GetRegionServiceProviders(countryName, regionName);
    }

    /// <summary>
    /// Get a list of categories
    /// </summary>
    /// <returns>array of category objects</returns>
    [WebMethod]
    public object[] GetCategories()
    {
        return HSLAppModel.Instance.GetCategories();
    }

    /// <summary>
    /// Calls the model's search method
    /// </summary>
    /// <param name="startAt">start at row</param>
    /// <param name="maxRows">maximum rows</param>
    /// <param name="fetchRowsCount">fetch rows count?</param>
    /// <param name="country">the country name</param>
    /// <param name="region">the region name</param>
    /// <param name="city">the city name</param>
    /// <param name="postalCode">the postal code</param>
    /// <param name="distance">the distance in km</param>
    /// <param name="accessFee">the access fee</param>
    /// <param name="areaCode">the area code</param>
    /// <param name="categories">pipe delimited categories</param>
    /// <param name="serviceProviders">pipe delimited service providers</param>
    /// <param name="sort">the sort order and dir (pipe delimited)</param>
    /// <returns>the search result object matching the criteria</returns>
    [WebMethod(EnableSession = true)]
    public HSLServices.SearchResultDTO SearchHotspots(
                                            UInt32 startAt,
                                            UInt32 maxRows,
                                            Boolean fetchRowsCount,
                                            String country, 
                                            String region, 
                                            String city, 
                                            String postalCode,
                                            Double distance,
                                            String accessFee,
                                            String areaCode,
                                            String categories,
                                            String serviceProviders,
                                            String sort)
    {
        HSLServices.SearchResultDTO result = null;
        HSLServices.SearchCriteriaDTO criteria = new HSLServices.SearchCriteriaDTO();

        // get the user state from the http context, so that we leverage the session
        // by looking at the existing search result in case it's the same criteria
        UserState ustate = UserState.Get(this.Context);

        criteria.Country = country;
        criteria.Region = region;
        criteria.City = city;
        criteria.PostalCode = postalCode;
        criteria.AreaCode = areaCode;
        criteria.ServiceProviders = (null == serviceProviders) ? null : serviceProviders.Split('|');
        criteria.Proximity = distance;
        criteria.Categories = (null == categories) ? null : categories.Split('|');
        criteria.Cost = HSLView.ConvertStrToCostScheme(accessFee);
        criteria.StartAtRow = startAt;
        criteria.MaxRows = maxRows;

        criteria.FetchRowsCount = true;

        if (null != ustate)
        {
            criteria.FetchRowsCount = !HSLView.SameCriteriaNoPaginationOrSort(ustate.SearchCriteria, criteria);
        }

        criteria.Sort = HSLView.ConvertStrToSortOrder(sort);
        criteria.SortDir = HSLView.ConvertStrToSortDirection(sort);

        if (null != ustate && null != ustate.SearchCriteria)
        {
            if (true == HSLView.SameCriteria(ustate.SearchCriteria, criteria))
            {
                result = ustate.SearchResult;
            }
            else
            {
                result = HSLAppModel.Instance.SearchHotspots(criteria);
                ustate.SearchResult = result;
                ustate.SearchCriteria = criteria;

                if (true == fetchRowsCount)
                {
                    ustate.AvailableRows = (UInt32)result.Hotspots.AvailableRows;
                }
            }
        }
        else
        {
            result = HSLAppModel.Instance.SearchHotspots(criteria);
        }
        
        return result;
    }

    /// <summary>
    /// method for user interface calls
    /// </summary>
    /// <param name="startAt">start at row</param>
    /// <param name="maxRows">maximum rows</param>
    /// <param name="country">the country name</param>
    /// <param name="region">the region name</param>
    /// <param name="city">the city name</param>
    /// <param name="postalCode">the postal code</param>
    /// <param name="distance">the distance in km</param>
    /// <param name="accessFee">the access fee</param>
    /// <param name="areaCode">the area code</param>
    /// <param name="categories">pipe delimited categories</param>
    /// <param name="serviceProviders">pipe delimited service providers</param>
    /// <param name="sort">the sort order and dir (pipe delimited)</param>
    /// <returns>the search result object matching the criteria</returns>
    [WebMethod]
    [XmlInclude(typeof(BaseDTO))]
    [XmlInclude(typeof(TableDTO))]
    [XmlInclude(typeof(GenericDTO))]
    [XmlInclude(typeof(SearchResultDTO))]
    [XmlInclude(typeof(SearchResultUIDTO))]
    [XmlInclude(typeof(LatitudeLongitudeDTO))]
    [XmlInclude(typeof(HotspotDTO))]
    [XmlInclude(typeof(HotspotUIDTO))]
    [XmlInclude(typeof(AccessPointDTO))]
    [XmlInclude(typeof(CostScheme))]
    [XmlInclude(typeof(SecurityScheme))]
    [XmlInclude(typeof(AmenityDTO))]
    [XmlInclude(typeof(ArrayList))]
    public SearchResultUIDTO SearchHotspotsUI(
                                            UInt32 startAt,
                                            UInt32 maxRows,
                                            String country,
                                            String region,
                                            String city,
                                            String postalCode,
                                            Double distance,
                                            String accessFee,
                                            String areaCode,
                                            String categories,
                                            String serviceProviders,
                                            String sort)
    {
        SearchResultUIDTO result = new SearchResultUIDTO();

        HSLServices.SearchResultDTO search = this.SearchHotspots(startAt, maxRows, true, country, region, city, postalCode, distance, accessFee, areaCode, categories, serviceProviders, sort);

        String criteria = this.GetCriteriaAsLink(startAt - 1, maxRows, country, region, city, postalCode, distance, accessFee, areaCode, categories, serviceProviders, sort);

        result.MapViewURL = "ListView.aspx" + criteria + "&cmd=mapview";
        result.ListViewURL = "MapView.aspx" + criteria + "&cmd=listview";
        result.LatLong = search.LatLong;

        TableDTO tbl = null;
        List<BaseDTO> dtos = new List<BaseDTO>();

        if (null != search.Hotspots && null != search.Hotspots.DTOs)
        {
            tbl = new TableDTO();
            for (Int32 row = 0; row < search.Hotspots.DTOs.Length; row++)
            {
                HotspotDTO hotspot = (HotspotDTO)search.Hotspots.DTOs[row];
                HotspotUIDTO hotspotUI = new HotspotUIDTO();

                // populate new dto
                hotspotUI.AccessPoints = hotspot.AccessPoints;
                hotspotUI.AddrStreet = hotspot.AddrStreet;
                hotspotUI.Amenities = hotspot.Amenities;
                hotspotUI.AmenitiesComments = hotspot.AmenitiesComments;
                hotspotUI.Category = hotspot.Category;
                hotspotUI.City = hotspot.City;
                hotspotUI.Cost = hotspot.Cost;
                hotspotUI.Country = hotspot.Country;
                hotspotUI.CountryAbbr = hotspot.CountryAbbr;
                hotspotUI.Distance = hotspot.Distance;
                hotspotUI.Latitude = hotspot.Latitude;
                hotspotUI.LocationPhotoUrl = hotspot.LocationPhotoUrl;
                hotspotUI.Longitude = hotspot.Longitude;
                hotspotUI.Name = hotspot.Name;
                hotspotUI.Phone = hotspot.Phone;
                hotspotUI.PostalCode = hotspot.PostalCode;
                hotspotUI.Region = hotspot.Region;
                hotspotUI.RegionAbbr = hotspot.RegionAbbr;
                hotspotUI.Security = hotspot.Security;
                hotspotUI.UseableAreasComments = hotspot.UseableAreasComments;
                hotspotUI.DetailsViewURL = "ListView.aspx" + criteria + "&cmd=detailsview&entry=" + row.ToString();

                dtos.Add(hotspotUI);
            }

            tbl.AvailableRows = search.Hotspots.AvailableRows;

            tbl.DTOs = dtos.ToArray();
        }

        result.Hotspots = tbl;

        return result;
    }

    /// <summary>
    /// returns the link to append to search criteria
    /// </summary>
    /// <param name="startAtRow">start at row</param>
    /// <param name="maxRows">maximum rows</param>
    /// <param name="country">the country name</param>
    /// <param name="region">the region name</param>
    /// <param name="city">the city name</param>
    /// <param name="postalCode">the postal code</param>
    /// <param name="distance">the distance in km</param>
    /// <param name="accessFee">the access fee</param>
    /// <param name="areaCode">the area code</param>
    /// <param name="categoriesStr">pipe delimited categories</param>
    /// <param name="serviceProvidersStr">pipe delimited service providers</param>
    /// <param name="sort">the sort order and dir (pipe delimited)</param>
    /// <returns>the criteria as a link</returns>
    private String GetCriteriaAsLink(
                                        UInt32 startAtRow,
                                        UInt32 maxRows,
                                        String country,
                                        String region,
                                        String city,
                                        String postalCode,
                                        Double distance,
                                        String accessFee,
                                        String areaCode,
                                        String categoriesStr,
                                        String serviceProvidersStr,
                                        String sort)
    {
        String pars = String.Empty;

        if (!String.IsNullOrEmpty(country))
        {
            pars += "?country=" + country;
        }
        
        if (!String.IsNullOrEmpty(region))
        {
            pars += "&region=" + region;
        }
   
        if (!String.IsNullOrEmpty(city))
        {
            pars += "&city=" + city;
        }

        if (!String.IsNullOrEmpty(postalCode))
        {
            pars += "&postalCode=" + postalCode;
        }

        if (-1 != distance)
        {
            pars += "&distance=" + distance.ToString();
        }

        if (!String.IsNullOrEmpty(accessFee))
        {
            pars += "&accessFee=" + accessFee;
        }

        if (!String.IsNullOrEmpty(areaCode))
        {
            pars += "&areaCode=" + areaCode;
        }

        if (!String.IsNullOrEmpty(categoriesStr))
        {
            pars += "&categoriesStr=" + categoriesStr;
        }

        if (!String.IsNullOrEmpty(serviceProvidersStr))
        {
            pars += "&serviceProvidersStr=" + serviceProvidersStr;
        }

        pars += "&startAtRow=" + startAtRow;
        pars += "&maxRows=" + maxRows;

        if (!String.IsNullOrEmpty(sort))
        {
            pars += "&sort=" + sort;
        }

        return pars;
    }
}

