//---------------------------------------------------------------------
// <copyright file="UserState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the UserState class
// </summary>
//---------------------------------------------------------------------
namespace MS.Msn.InternetAccess.Hsl.WebSite.Session
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
    using MS.Msn.InternetAccess.Hsl.WebSite.ViewState;

    /// <summary>
    /// Contains state information related to a user
    /// </summary>
    public class UserState
    {
        /// <summary>
        /// the current search criteria
        /// </summary>
        private HSLServices.SearchCriteriaDTO searchCriteria = null;

        /// <summary>
        /// the current search result
        /// </summary>
        private HSLServices.SearchResultDTO searchResult = null;

        /// <summary>
        /// number of available rows for current search
        /// </summary>
        private UInt32 availableRows = 0;

        /// <summary>
        /// the search view
        /// </summary>
        private SearchViewState searchView = null;

        /// <summary>
        /// the list view
        /// </summary>
        private ListViewState listView = null;

        /// <summary>
        /// the map view
        /// </summary>
        private MapViewState mapView = null;

        /// <summary>
        /// the details view
        /// </summary>
        private DetailsViewState detailsView = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserState()
        {
        }

        /// <summary>
        /// search criteria property
        /// </summary>
        public HSLServices.SearchCriteriaDTO SearchCriteria
        {
            get
            {
                return this.searchCriteria;
            }

            set
            {
                this.searchCriteria = value;
            }
        }

        /// <summary>
        /// search result property
        /// </summary>
        public HSLServices.SearchResultDTO SearchResult
        {
            get
            {
                return this.searchResult;
            }

            set
            {
                this.searchResult = value;
            }
        }

        /// <summary>
        /// Available Rows based on current search criteria
        /// </summary>
        public UInt32 AvailableRows
        {
            get
            {
                return this.availableRows;
            }

            set
            {
                this.availableRows = value;
            }
        }

        /// <summary>
        /// search view property
        /// </summary>
        public SearchViewState SearchView
        {
            get
            {
                return this.searchView;
            }

            set
            {
                this.searchView = value;
            }
        }

        /// <summary>
        /// list view property
        /// </summary>
        public ListViewState ListView
        {
            get
            {
                return this.listView;
            }

            set
            {
                this.listView = value;
            }
        }

        /// <summary>
        /// map view property
        /// </summary>
        public MapViewState MapView
        {
            get
            {
                return this.mapView;
            }

            set
            {
                this.mapView = value;
            }
        }

        /// <summary>
        /// details view property
        /// </summary>
        public DetailsViewState DetailsView
        {
            get
            {
                return this.detailsView;
            }

            set
            {
                this.detailsView = value;
            }
        }

        /// <summary>
        /// get the current user state from the session, create it if doesn't exist
        /// </summary>
        /// <param name="context">the http context that contains the session</param>
        /// <returns>the user state object</returns>
        public static UserState GetOrCreate(HttpContext context)
        {
            UserState ustate = null;

            if (null != context.Session)
            {
                ustate = (UserState)context.Session["UserState"];

                if (null == ustate)
                {
                    ustate = new UserState();

                    context.Session["UserState"] = ustate;
                }
            }

            return ustate;
        }

        /// <summary>
        /// get the user state given the http context
        /// </summary>
        /// <param name="context">the http context</param>
        /// <returns>the user state object</returns>
        public static UserState Get(HttpContext context)
        {
            UserState ustate = null;

            if (null != context.Session)
            {
                ustate = (UserState)context.Session["UserState"];
            }

            return ustate;
        }

        /// <summary>
        /// check if user state is in the session
        /// </summary>
        /// <param name="context">the http context</param>
        /// <returns>true if user state exists, otherwise false</returns>
        public static Boolean IsAvailable(HttpContext context)
        {
            return (null != context.Session["UserState"]);
        }
    }
}
