//---------------------------------------------------------------------
// <copyright file="MapViewState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the MapViewState class
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
    /// Defines the properties of the map view
    /// </summary>
    public class MapViewState
    {
        /// <summary>
        /// the hotspots list start at row
        /// </summary>
        private UInt32 startAtRow = 1;

        /// <summary>
        /// the page size selection
        /// </summary>
        private UInt32 pageSize = 10;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapViewState()
        {
        }

        /// <summary>
        /// Start at row property
        /// </summary>
        public UInt32 StartAtRow
        {
            get
            {
                return this.startAtRow;
            }

            set
            {
                this.startAtRow = value;
            }
        }

        /// <summary>
        /// Page size property
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
    }
}
