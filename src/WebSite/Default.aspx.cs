//---------------------------------------------------------------------
// <copyright file="Default.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the Default class
// </summary>
//---------------------------------------------------------------------
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
/// Defines the default page
/// </summary>
public partial class _Default : System.Web.UI.Page 
{
    /// <summary>
    /// on page load we transfer the user to the search view
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("web/SearchView.aspx");
    }
}
