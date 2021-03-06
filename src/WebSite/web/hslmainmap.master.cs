//---------------------------------------------------------------------
// <copyright file="HSLMainMap.master.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the Web_HSLMainMap class
// </summary>
//---------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Defines the master page class
/// </summary>
public partial class Web_HSLMainMap : System.Web.UI.MasterPage
{
    /// <summary>
    /// handle page load event
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// returns the current year on the server for the copyright display
    /// </summary>
    /// <returns>the year</returns>
    protected Int16 GetYear()
    {
        return (Int16)DateTime.Now.Year;
    }
}
