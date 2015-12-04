//---------------------------------------------------------------------
// <copyright file="HSLAppModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the HSLAppModel class
// </summary>
//---------------------------------------------------------------------
namespace MS.Msn.InternetAccess.Hsl.WebSite.Model
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.Collections;

    /// <summary>
    /// Defines the application model for HSL
    /// </summary>
    public class HSLAppModel
    {
        /// <summary>
        ///  singelton instance of the model
        /// </summary>
        private static HSLAppModel instance = new HSLAppModel();

        /// <summary>
        /// the HSLServices proxy
        /// </summary>
        private static HSLServices.HSLServices hslProxy = new HSLServices.HSLServices();

        /// <summary>
        /// cache lock
        /// </summary>
        private static volatile object cacheLock = null;

        /// <summary>
        /// cache of all countries
        /// </summary>
        private object[] countries = null;

        /// <summary>
        /// hashtable used to cache country name to code mapping
        /// </summary>
        private Hashtable tableCountryNameToCode = null;

        /// <summary>
        /// us states
        /// </summary>
        private object[] americanStates = null;

        /// <summary>
        /// hashtable used to cache US state name to code mapping
        /// </summary>
        private Hashtable tableUSStateNameToCode = null;

        /// <summary>
        /// static constructor
        /// </summary>
        static HSLAppModel()
        {
            cacheLock = new object();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        private HSLAppModel()
        {
        }

        /// <summary>
        /// static property for the singelton instance
        /// </summary>
        public static HSLAppModel Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// get the HSL counts i.e. hotspots and countries
        /// </summary>
        /// <returns>the hsl counts dto</returns>
        public HSLServices.HSLCountsDTO GetHSLCounts()
        {
            return hslProxy.GetCounts();
        }

        /// <summary>
        /// search hotspots given criteria
        /// </summary>
        /// <param name="criteria">the search criteria object</param>
        /// <returns>the search result objet</returns>
        public HSLServices.SearchResultDTO SearchHotspots(HSLServices.SearchCriteriaDTO criteria)
        {
            Double proximity = criteria.Proximity;

            if (criteria.Proximity != -1 && "United States" == criteria.Country)
            {
                criteria.Proximity = proximity * 1.61;
            }

            // if zipcode is specified, override region/city
            // this is a temporary fix until we get mappoint to return the address
            // on a match code method of zip
            String region = criteria.Region;
            String city = criteria.City;

            if (null != criteria.PostalCode)
            {
                criteria.Region = null;
                criteria.City = null;
            }

            HSLServices.SearchResultDTO result = null;

            try
            {
                result = hslProxy.Search(criteria);
            }
            finally
            {
                if (null != criteria.PostalCode)
                {
                    criteria.Region = region;
                    criteria.City = city;
                }

                criteria.Proximity = proximity;
            }

            return result;
        }

        /// <summary>
        /// get list of countries in the world
        /// </summary>
        /// <returns>array of country objects</returns>
        public object[] GetCountries()
        {
            if (null == this.countries)
            {
                lock (cacheLock)
                {
                    if (null == this.countries)
                    {
                        HSLServices.TableDTO tbl = hslProxy.GetCountries();
                        this.countries = tbl.DTOs;

                        if (null != this.countries)
                        {
                            this.tableCountryNameToCode = new Hashtable();

                            foreach (HSLServices.CountryDTO country in this.countries)
                            {
                                this.tableCountryNameToCode[country.CountryName] = country.CountryCode;
                            }
                        }
                    }
                }
            }

            return this.countries;
        }

        /// <summary>
        /// map country name to code
        /// </summary>
        /// <param name="countryName">the country name</param>
        /// <returns>the country code</returns>
        public String CountryNameToCode(String countryName)
        {
            return (String)this.tableCountryNameToCode[countryName];
        }

        /// <summary>
        /// get the regions for a given country code
        /// </summary>
        /// <param name="countryCode">the country code</param>
        /// <returns>array of region objects</returns>
        public object[] GetRegions(String countryCode)
        {
            object[] regions = null;

            if ("US" != countryCode)
            {
                HSLServices.TableDTO tbl = hslProxy.GetCountryRegions(countryCode);

                if (null != tbl)
                {
                    regions = tbl.DTOs;
                }
            }
            else
            {
                if (null == this.americanStates)
                {
                    lock (cacheLock)
                    {
                        // if US states are not cached then fetch from service and cache
                        if (null == this.americanStates)
                        {
                            HSLServices.TableDTO tbl = hslProxy.GetCountryRegions(countryCode);

                            if (null != tbl)
                            {
                                this.americanStates = tbl.DTOs;

                                this.tableUSStateNameToCode = new Hashtable();

                                foreach (HSLServices.RegionDTO state in this.americanStates)
                                {
                                    this.tableUSStateNameToCode[state.RegionName] = state.RegionCode;
                                }
                            }
                        }
                    }
                }

                regions = this.americanStates;
            }

            return regions;
        }

        /// <summary>
        /// get list of categories
        /// </summary>
        /// <returns>array of category objects</returns>
        public object[] GetCategories()
        {
            object[] categories = null;
            HSLServices.TableDTO tbl = hslProxy.GetCategories();

            if (null != tbl)
            {
                categories = tbl.DTOs;
            }
            
            return categories;
        }

        /// <summary>
        /// get the list of service providers given country 
        /// </summary>
        /// <param name="countryName">the country name</param>
        /// <returns>array of service provider objects</returns>
        public object[] GetCountryServiceProviders(String countryName)
        {
            object[] providers = null;
            HSLServices.TableDTO tbl = hslProxy.GetCountryServiceProvidersDistinct((String)this.tableCountryNameToCode[countryName]);

            if (null != tbl)
            {
                providers = tbl.DTOs;
            }

            return providers;
        }

        /// <summary>
        /// get the list of service providers given country and region name
        /// </summary>
        /// <param name="countryName">the country name</param>
        /// <param name="regionName">the region name</param>
        /// <returns>array of service provider objects</returns>
        public object[] GetRegionServiceProviders(String countryName, String regionName)
        {
            object[] providers = null;

            if ("United States" == countryName)
            {
                providers = this.GetUSRegionServiceProviders(regionName);
            }

            return providers;
        }

        /// <summary>
        /// get the distinct list of world service providers
        /// </summary>
        /// <returns>array of service provider objects</returns>
        public object[] GetAllDistinctServiceProviders()
        {
            object[] providers = null;
            HSLServices.TableDTO tbl = hslProxy.GetAllServiceProvidersDistinct();

            if (null != tbl)
            {
                providers = tbl.DTOs;
            }

            return providers;
        }

        /// <summary>
        /// get the US state service providers
        /// </summary>
        /// <param name="stateName">which us state</param>
        /// <returns>array of service provider objects</returns>
        public object[] GetUSRegionServiceProviders(String stateName)
        {
            object[] providers = null;

            if (null != this.americanStates)
            {
                HSLServices.TableDTO tbl = hslProxy.GetServiceProviders("US", (String)this.tableUSStateNameToCode[stateName]);

                if (null != tbl)
                {
                    providers = tbl.DTOs;
                }
            }

            return providers;
        }
    }
}
