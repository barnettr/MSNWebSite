//---------------------------------------------------------------------
// <copyright file="SearchResultUIDTO.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the HotspotLocationDTO class
// </summary>
//---------------------------------------------------------------------
namespace MS.Msn.InternetAccess.Hsl.Website.Shared.Dto
{
    using System;
    using System.Text;

    using HSLServices;
    using MS.Msn.InternetAccess.Common.Utilities.Database;

    /// <summary>
    /// Search Result data transfer object
    /// </summary>
    public class SearchResultUIDTO : SearchResultDTO
    {
        /// <summary>
        /// the map view url
        /// </summary>
        private String mapViewURL;

        /// <summary>
        /// the list view url
        /// </summary>
        private String listViewURL;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SearchResultUIDTO()
        {
        }

        /// <summary>
        /// MapViewURL property
        /// </summary>
        public String MapViewURL
        {
            get
            {
                return this.mapViewURL;
            }

            set
            {
                this.mapViewURL = value;
            }
        }

        /// <summary>
        /// ListViewURL property
        /// </summary>
        public String ListViewURL
        {
            get
            {
                return this.listViewURL;
            }

            set
            {
                this.listViewURL = value;
            }
        }
    }
}