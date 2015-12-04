<%@ Page Language="C#" MasterPageFile="HSLMain.master" AutoEventWireup="true" CodeFile="SearchView.aspx.cs" Inherits="Web_SearchView" %>

<asp:Content ID="searchview" ContentPlaceHolderID="MainContent" Runat="Server">
  <link title="combined" rel="stylesheet" type="text/css" media="screen" href="css/index.css" />
  <script language="javascript" src="js/SearchView.js" type="text/javascript"></script>

<script language="javascript" type="text/javascript">
function checkSearchSubmit(e)
{
    var keynum;

    if(window.event) // IE
    {
        keynum = e.keyCode;
    }
    else
    {
        keynum = e.which;
    }

    var btnSearch = document.getElementById("btnSearch");
    if (keynum == 13 && btnSearch.disabled == false)
    {
        onSearch();
    }

    return true;
}
</script>
     
<table style="width:825px; height:600px; margin-top:8px; background-color:#fff;" cellpadding="0" cellspacing="0" border="0">
    <tr style="padding-top:0px;" valign="top">
        <!--  Header Images Left -->
        <td align="left" valign="top" class="indexImages">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr valign="top">
                    <td colspan="3" valign="top">
                        <img id="diversityImage" src="images/misc/MotherDaughter.jpg" style="width:210px; height:225px;" alt="MSN Wifi Hotspots Search" class="indexImage" />
                    </td>
                </tr>
                <tr>
                    <td class="croppedAssets" align="left"><img alt="MSN Wifi Hotspots Search" src="images/misc/CoupleCrop.jpg" style="width:62px; height:45px;" class="indexImage" /></td>
                    <td class="croppedAssets" align="center"><img alt="MSN Wifi Hotspots Search" src="images/misc/ProfessionalsCrop.jpg" style="width:62px; height:45px;" class="indexImage" /></td>
                    <td class="croppedAssets" align="right"><img alt="MSN Wifi Hotspots Search" src="images/misc/WomanCrop.jpg" style="width:62px; height:45px;" class="indexImage" /></td>
                 </tr>
            </table>            
        </td>
        <%--Beginning of the Middle Cell set at 100%--%>
        <td valign="top" align="left" class="contentHoldingCell">
            
            <table border="0" cellpadding="0" cellspacing="0" class="contentTable">
                <tr>
                    <td valign="top">
                        <!--  Welcome and Help -->
                        <table border="0" cellpadding="0" cellspacing="0" class="welcomeTable">
                            <tr>
                                <td><h1 id="Locator" class="headingMargin">Welcome to MSN WiFi Hotspot Locator</h1></td>
                                <td class="helpCell" align="right"><a href="#" title="Help" onclick="LaunchHelp('<%=GetHelpURL() %>', 'hsl_web', 'en-us', '', 'keyword', 'qaf', false, '', '', '', '');return false;">Help</a></td>
                            </tr>
                        </table>
                        <!--  Start of Welcome content and the Search Panel in a 2 row table -->
                        <table cellpadding="0" cellspacing="0" border="0" class="width100">
                            <tr>
                                <td>
                                    <table cellpadding="0" cellspacing="0" border="0" style="width:560px;">
                                        <tr>
                                            <td>
                                                <p class="line_height_loose">With <%=this.RenderHotspotsCount() %> free and paid WiFi hotspots in <%=this.RenderCountriesCount() %> countries, our Hotspot Locator makes 
                                                it easy to locate wireless Internet access around the globe. Use the View on Map feature to see your search results on
                                                a map powered by Virtual Earth.</p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <!-- Search Panel -->
                                    <table border="0" cellpadding="0" cellspacing="0" class="">
                                        <tr>
                                            <td class="searchbox">&nbsp;</td>
                                            <td id="basicSearch">Basic Search</td>
                                            <td style="width:18px;" align="center">&nbsp;|&nbsp;</td>
                                            <td align="left" id="advancedSearch"><a href="#" title="Advanced Search" onclick="setSearchMode('advanced');">Advanced Search</a></td>
                                        </tr>
                                    </table>
                                    <br />
                                 </td>
                             </tr>
    <%  
                             if (null != this.ErrorMessage || null != this.InfoMessage)
                             {
    %>
                             <tr>
                                <%=this.RenderNotificationMessage("style=\"padding-left:10px; padding-right:115px;\"")%>
                             </tr>
    <%     
                             }
    %>
                             <tr>
                                 <td style="width:600px;" align="left" valign="top">
                                    <table border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="top" class="leftAlignment">
                                                <label class="Label">Country/Region:</label><br />
                                                <select id="country" class="ComboBox" onkeydown="return checkSearchSubmit(event);" style="width:198px;" name="country" onchange="onCountryChanged()">
                                                <%=this.RenderCountriesList() %>
                                                </select>
                                            </td>
                                            <td style="width: 8px;">&nbsp;</td>
                                            <td colspan="3" align="left" valign="top">
                                                <label id="regionLabel" class="Label">Region:</label><br />
                                                <select id="region" class="ComboBox" onkeydown="return checkSearchSubmit(event);" style="width:252px;" name="region" onchange="onRegionChanged()">
                                                <%=this.RenderRegionsList() %>
                                                </select>
                                            </td>
                                        </tr>
                                        <tr class="heightRuler">
                                            <td style="width: 198px;" class="leftTopAlignment" align="left" valign="top">
                                                <label class="Label">City:</label><br />
                                                <input class="TextBox" type="text" onkeydown="return checkSearchSubmit(event);" style="width:194px;" maxlength="80" name="city" value="<%=this.RenderCity() %>" />                        
                                            </td>
                                            <td style="width: 0px;" align="left" valign="top">&nbsp;</td>
                                            <td style="width: 103px; padding-top: 8px;" align="left" valign="top">
                                                <label style="font-weight:bold;" id="postalCodeLabel" class="Label">Postal Code:</label><br />
                                                <input class="TextBox" type="text" onkeydown="return checkSearchSubmit(event);" style="width:100%;" maxlength="10" name="postalCode" value="<%=this.RenderPostalCode() %>" />
                                            </td>
                                            <td style="width: 12px;">&nbsp;</td>
                                            <td style="width: 196px; padding-top: 8px;" align="left" valign="top">
                                                <table border="0" cellpadding="0" cellspacing="0" style="width:136px;">
                                                    <tr>
                                                        <td valign="baseline">
                                                            <label class="Label">Distance:</label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <select id="distance" class="ComboBox" onkeydown="return checkSearchSubmit(event);" style="width:137px;" name="distance">
                                                <%=this.RenderDistance() %>
                                                </select>
                                                <p class="small">* includes surrounding areas</p>
                                            </td>
                                        </tr>
                                        <tr class="heightRuler">
                                            <td align="left" valign="top" class="leftTopAlignment">
                                                <div id="divAreaCode" style="display:none;">
                                                <label class="Label">Area Code:</label><br />
                                                <input id="areaCode" class="TextBox" type="text" onkeydown="return checkSearchSubmit(event);" style="width:60px;" maxlength="3" name="areaCode" value="<%=this.RenderAreaCode() %>" />
                                                </div>
                                            </td>
                                            <td style="width: 3px;" align="left" valign="top">&nbsp;</td>
                                            <td colspan="3" align="left" valign="top" style="padding-top: 8px;">
                                                <label class="Label">Access Fee:</label>
                                                <br />
                                                <%=this.RenderAccessFee() %> 
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="left" class="leftTopAlignment">
                                                <div id="categoriesDiv" style="display:none;">
                                                    <label class="Label">Category:</label><br />
                                                    <div id="categories" class="CheckedListBox">
                                                        <%=this.RenderCategories() %>
                                                    </div>
                                                </div>
                                            </td>
                                            <td></td>
                                            <td colspan="3" valign="top" align="left" style="padding-top: 8px;">
                                                <div id="providersDiv" style="display:none;">
                                                    <label class="Label">Service Providers:</label><br />
                                                    <div id="serviceProviders" class="WideCheckedListBox">
                                                        <%=this.RenderServiceProviders() %>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr class="heightRuler">
                                            <td colspan="5" align="right" style="padding-right:42px;">
                                                <input id="btnSearch" type="button" value="Search" title="Click to search for hotspots" onclick="onSearch();" class="fancyButton fancyButtonDisabled"
                                                    onmouseover="className='fancyButton fancyButtonHover';" onmouseout="className='fancyButton fancyButtonRest';" onclick="className='fancyButton fancyButtonPressed'; onSearch();"/>
                                                <br /><br />
                                            </td>
                                        </tr>
                                    </table>
                                 </td>
                             </tr>
                          </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

<%=this.RenderStartAndMaxRowsState() %>
<input type="hidden" id="cmd" />

<!--input type="hidden" id="serviceProvidersStr" name="serviceProvidersStr" /-->
<%=this.RenderServiceProvidersState() %>
<input type="hidden" id="categoriesStr" name="categoriesStr" />
<input type="hidden" id="searchType" name="searchType" value="<%=this.RenderSearchType() %>"/>

<input type="hidden" id="regionSelected" value="" />

<%=this.RenderAvailableProvidersState() %>
  
</asp:Content>

