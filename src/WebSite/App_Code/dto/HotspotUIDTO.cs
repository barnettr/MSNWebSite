//---------------------------------------------------------------------
// <copyright file="HotspotUIDTO.cs" company="Microsoft">
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
    /// Hotspot data transfer object
    /// </summary>
    public class HotspotUIDTO : HotspotDTO
    {
        /// <summary>
        /// the details view url
        /// </summary>
        private String detailsViewURL;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HotspotUIDTO()
        {
        }

        /// <summary>
        /// DetailsViewURL property
        /// </summary>
        public String DetailsViewURL
        {
            get
            {
                return this.detailsViewURL;
            }

            set
            {
                this.detailsViewURL = value;
            }
        }
    }
}