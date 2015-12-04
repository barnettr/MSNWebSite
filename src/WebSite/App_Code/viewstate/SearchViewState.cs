//---------------------------------------------------------------------
// <copyright file="SearchViewState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the SearchViewState class
// </summary>
//---------------------------------------------------------------------
namespace MS.Msn.InternetAccess.Hsl.WebSite.ViewState
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

    /// <summary>
    /// Enumeration for the basic and advanced search types
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// basic search
        /// </summary>
        Basic,

        /// <summary>
        /// advanced search
        /// </summary>
        Advanced
    }

    /// <summary>
    /// Defines the properties of the search view
    /// </summary>
    public class SearchViewState
    {
        /// <summary>
        /// the search mode i.e. basic / advanced
        /// </summary>
        private SearchType searchMode = SearchType.Basic;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchViewState()
        {
        }

        /// <summary>
        /// the search mode property
        /// </summary>
        public SearchType SearchMode
        {
            get
            {
                return this.searchMode;
            }

            set
            {
                this.searchMode = value;
            }
        }
    }
}
