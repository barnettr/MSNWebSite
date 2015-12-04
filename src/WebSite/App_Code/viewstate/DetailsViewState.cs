//---------------------------------------------------------------------
// <copyright file="DetailsViewState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the DetailsViewState class
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
    /// Defines the properties of the details view
    /// </summary>
    public class DetailsViewState
    {
        /// <summary>
        ///  the current info card entry
        /// </summary>
        private UInt32 infoCardEntry = 1;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DetailsViewState()
        {
        }

        /// <summary>
        /// info card entry property
        /// </summary>
        public UInt32 InfoCardEntry
        {
            get
            {
                return this.infoCardEntry;
            }

            set
            {
                this.infoCardEntry = value;
            }
        }
    }
}
