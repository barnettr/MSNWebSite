//---------------------------------------------------------------------
// <copyright file="ListViewState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the ListViewState class
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
    using MS.Msn.InternetAccess.Hsl.WebSite.Session;

    /// <summary>
    /// Defines the properties of the list view
    /// </summary>
    public class ListViewState
    {
        /// <summary>
        /// the number of results
        /// </summary>
        private UInt32 numberOfResults = 0;

        /// <summary>
        /// default page size
        /// </summary>
        private UInt32 pageSize = HSLView.DefaultMaxRows;

        /// <summary>
        /// the current page
        /// </summary>
        private UInt32 currentPage = 1;

        /// <summary>
        /// the number of pages
        /// </summary>
        private UInt32 numberOfPages = 1;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ListViewState()
        {
        }

        /// <summary>
        /// the page size
        /// </summary>
        public UInt32 PageSize
        {
            get
            {
                return this.pageSize;
            }

            set
            {
                this.pageSize = value;
            }
        }

        /// <summary>
        /// the current page
        /// </summary>
        public UInt32 CurrentPage
        {
            get
            {
                return this.currentPage;
            }

            set
            {
                this.currentPage = value;
            }
        }
        
        /// <summary>
        /// the number of results based on current search criteria
        /// </summary>
        public UInt32 NumberOfResults
        {
            get
            {
                return this.numberOfResults;
            }

            set
            {
                this.numberOfResults = value;
            }
        }

        /// <summary>
        ///  the number of pages
        /// </summary>
        public UInt32 NumberOfPages
        {
            get
            {
                return this.numberOfPages;
            }
        }

        /// <summary>
        /// synch the view with the search criteria, and current search results
        /// </summary>
        /// <param name="ustate">the user state</param>
        public void SyncView(UserState ustate)
        {
            HSLServices.SearchCriteriaDTO criteria = ustate.SearchCriteria;

            this.numberOfResults = ustate.AvailableRows;

            if (null != criteria)
            {
                this.pageSize = criteria.MaxRows;
                this.currentPage = (criteria.StartAtRow - 1) / criteria.MaxRows + 1;
            }

            // calculate number of pages based on pagesize
            this.numberOfPages = this.numberOfResults / this.pageSize;

            // increase by one if we have left over rows
            if (this.numberOfResults % this.pageSize > 0)
            {
                this.numberOfPages++;
            }
        }
    }
}
