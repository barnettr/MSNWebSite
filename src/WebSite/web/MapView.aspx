<%@ Page Language="C#" MasterPageFile="HSLMainMap.master" AutoEventWireup="true" CodeFile="MapView.aspx.cs" Inherits="Web_MapView" %>

<asp:Content ID="mapview" ContentPlaceHolderID="MainContent" Runat="Server">
<link title="combined" rel="stylesheet" type="text/css" media="screen" href="css/mapview2.css" />
<link title="combined" rel="stylesheet" type="text/css" media="all" href="css/DrivingDirections.css" />
<link title="combined" rel="stylesheet" type="text/css" media="screen" href="css/infocard.css" />
<link title="combined" rel="stylesheet" type="text/css" media="print" href="css/mapview2_print.css" />
<link title="combined" rel="stylesheet" type="text/css" media="print" href="css/infocard_print.css" />

<script language="javascript" src="js/MapView.js" type="text/javascript"></script>
  <div>

    <table id="mapviewTable" cellpadding="0" cellspacing="0" border="0">
        <tr>
	        <td valign="middle" class="wrappingCell">
	            <table width="100%" class="headerTable">
                    <tr style="height:20px;">
                        <td align="left" valign="middle" style="padding-left:6px; width:50%;">
                            <a id="history" href="javascript:history.back()" title="Back">Back</a> | <a id="searchViewLink" href="#" title="Search Again">Search Again</a>
                        </td>
                        <td style="width:50%;" align="right">
                            <a href="#" title="Help" style="vertical-align:middle;" onclick="LaunchHelp('http://help.live.com', 'hsl_web', 'en-us', '', 'keyword', 'qaf', false, '', '', '', '');return false;">Help</a>
                            &nbsp;|&nbsp;&nbsp;<a href="javascript: window.print();" title="Print this page"><img alt="Print this page" src="images/misc/IconPrint.gif" border="0" /></a> <a style="vertical-align:middle;" href="javascript: window.print();" title="Print this page">Print this page</a>
                            &nbsp;|&nbsp;&nbsp;<a id="listViewLink" href="#" title="Back to the List"><img alt="Back to the list" src="images/misc/SmIconMap.gif" border="0" /> <span style="vertical-align:middle;">List View</span></a>&nbsp;
                        </td>
                    </tr>
                </table>
	        </td>
	    </tr>
	    <tr>
	        <td valign="top" class="lowerCell">
                <div id="myMap" class="VEmapHolder" style="position:relative; width:99.8%; height:869px;"></div>
	        </td>
	    </tr>
	</table>
	
    <%--The hotspot list selector--%>    
    <div id="hotspots" style="left:100px; top:126px; height:420px; filter:Alpha(Opacity=95);">

    <table cellpadding="0" cellspacing="0" border="0" id="gadgetContainer" class="hotspotTableWrapper">
        <tr class="title2" onmousedown="MouseDownHotspotsList(event);" onmouseup="g_MovePanel = null;" onmouseover="this.style.cursor = 'move';">
            <td align="left" class="rowLine">
                <p title="Hotspots">
                    Hotspots
                </p>
            </td>
            <td align="right" class="rowLine">
                <p>
                    <strong><%=this.RenderNumberOfResults() %></strong>&nbsp;
                    <a href="#" onclick="onHotspotsListToggleCollapse();"><img alt="Collapse Hotspot list" id="hotspotsCollapseDir" style="border:0px;" src="images/misc/LgArrowUp.gif" /></a>
                </p>
            </td>
        </tr>
        <tr>
            <td class="showme" id="showme">
                <label for="pageSizeSelector">Show</label>
                <select id="pageSizeSelector" style="font-size:xx-small;" onchange="onPageResize()">
                    <option value="10">10 per page</option>
                    <option value="15">15 per page</option>
                    <option value="20">20 per page</option>
                    <option value="25">25 per page</option>
                </select>
            </td>
            <td align="right" class="navButtons">
                <a href="#" onclick="PagePrevious();" title="Previous page"><img alt="Previous page" id="arrowLeft" src="images/misc/ArrowLeft.gif" style="border:0px;" /></a> <span id="hotspotsPage" class="Label"></span><a href="#" onclick="PageNext();" title="Next page"><img alt="Next page" id="arrowRight" src="images/misc/ArrowRight.gif" style="border:0px;" /></a>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <div id="hotspotsList">
                    <table id="hotspotsTable" cellpadding="0" cellspacing="0" border="1" bordercolor="white" rules="rows">
                    </table>
                </div>
            </td>
        </tr>
    </table>  
    </div>


    <%--Small Info Card--%>
    <div id="infoCard" style="position:absolute; left:700px; top:126px; background-color:#fff; filter:Alpha(Opacity=85); display:none;">

    <table id="infoCardTable" style="width:230px; border:solid 1px #8aa5c2; background-image: url(images/misc/steel_firefoxGradient.gif); background-repeat:no-repeat;" class="title1" border="0" cellpadding="0" cellspacing="0">
        <tr style="background-color: #b47c4f;">
            <td class="leftAlignment">
                <table style="width:100%;  background-color: #5885b5;" onmousedown="MouseDownInfoCard(event);" onmouseup="g_MovePanel = null;">
                    <tr style="cursor:move;">
                        <td align="left" valign="top">
                            <strong class="infocardTitle" id="cardName">McDonalds</strong><br />
                        </td>
                        <td id="CloseBtn" valign="top" align="right" class="rightAlignment" style="">
                            <a href="#" onclick="InfoCardOff();" title="Close"><img alt="Close" src="images/misc/wh_closebtn.gif" /></a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr valign="top">
            <td class="leftLocationPhotoAlignment">
                <table border="0">
                    <tr>
                        <td>
                            <img alt="Photo of location" id="cardLocationPhoto" class="infoCardCat" style="border:1px solid #8aa5c2;" width="111px" height="56px" src="images/categories/airport.jpg" />
                        </td>
                        <td valign="top" class="line_height_median textCell">
                            <span class="infoCardText" id="cardAddress" title="Location address">12327 NE 92nd St.</span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width:100%;" align="center" class="leftAmenityAlignment">
                <table style="width:100%;" cellpadding="0" cellspacing="0" border="0">
                    <tr id="cardAmenities" style="width:100%;">
                        <td>
                            <img alt="" src="images/amenities/sm_food.gif" />
                        </td>
                    </tr>
                </table> 
            </td>
        </tr>
        <tr>
            <td class="listAlignment" style="width:99%;">
                <div id="infocardsList">
                    <p class="providerList">
                        <strong class="textHighlight">Providers</strong> (<span id="cardNumProviders">3</span>)
                    </p>
                    <table id="cardProviders" cellpadding="0" cellspacing="0" border="0" style="margin-top:-10px;">
                    </table>
                </div>
            </td>  
        </tr>
        <tr class="BluePanel">
            <td class="line_height_standard footerAlignment">
                <a href="#" onclick="onDetailsView();" title="View details">View Details</a><br />
                <a href="javascript: window.print();" title="Print this">Print this</a><br />
                <div id="Actions" style="display:none;">
                    <a id="email" href="#" title="Email to a friend">Email to a friend</a><br />
                    <a href="#" onclick="showDirectionCard();" title="Get Directions">Get Directions</a><br />
                    <hr style="width:100%; height:1px; color:#07519a;" />
                </div>
                <span id="openActions" style=""><a href="#" onclick="getMoreActionsMenu('moreActions');" title="More actions">More actions <img alt="More actions" id="moreActions" src="images/misc/MoreActionArrow_dn.gif" /></a></span>
                <span id="closeActions" style="display:none;"><a href="#" onclick="closeMoreActionsMenu('noActions');" title="Close More actions">More actions <img alt="Close More actions" id="noActions" src="images/misc/MoreActionArrow_up.gif" /></a></span><br />
                <span style="font-size:0.8em; color:#666666;">Hotspot data provided by JiWire</span>
            </td>  
        </tr>
    </table>

    </div>
    
    
    <%--Driving Directions Card--%>
    <div id="directions" style="filter:Alpha(Opacity=95); position:absolute; left:100px; top:126px; display:none;">
    <table cellpadding="0" cellspacing="0" id="directionContainer" border="0">
        <tr>
            <td style="width:100%; background-color:#d7edf4;">
                <table cellpadding="0" cellspacing="0" border="0" style="width:100%; background-color:#d7edf4;" class="headerTable" onmousedown="MouseDownDirectionsList(event);" onmouseup="g_MovePanel = null;" onmouseover="this.style.cursor = 'move';">
                    <tr class="firstTitle" style="background-color:#d7edf4;">
                        <td style="width:23px; padding-left:8px;">
                            <img src="images/misc/driving.gif" border="0" />
                        </td>
                        <td align="left">
                            <p>Driving directions</p>
                        </td>
                        <td align="right">
                            <a href="#" onclick="DrivingCardOff();" title="Close"><img src="images/misc/glyph_close_rest.gif" border="0" /></a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width:100%;">
                <table cellpadding="0" cellspacing="0" border="0" style="width:100%;" class="bottomTable">
                    <tr class="secondTitle">
                        <td valign="middle" style="width:23px; padding-bottom:4px;">
                            <a id="email2" href="#" title="Email to a friend">Email</a>
                        </td>
                        <td valign="middle" style="width:23px; padding-bottom:4px;">
                            <a href="javascript: window.print();" title="Print this">Print</a>
                        </td>
                        <td style="width:201px; padding-bottom:8px;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="width:100%;">
                <div id="directionList">
                <table cellpadding="0" cellspacing="0" border="0" style="width:270px; background-color:#f8f8f8; border-bottom:1px solid #e6e6e6; border-right:1px solid #b3c0cc;">
                    <tr class="title3">
                        <td style="width:100%;" colspan="2">
                            <p><strong>Start</strong></p>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:100%; padding-left:8px;" colspan="2">
                            <textarea style="font-family:Arial; font-size:small;" id="startAddress" name="startAddress" rows="3" cols="40"></textarea>        
                        </td>
                    </tr>
                    <tr class="title3" style="padding-top:0px;">
                        <td style="width:100%;" valign="bottom">
                            <p>
                                <strong>End</strong>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:100%; padding-left:8px;" colspan="2">
                            <textarea style="font-family:Arial; font-size:small;" id="endAddress" name="endAddress" rows="3" cols="40"></textarea>
                        </td>
                    </tr>
                    <tr class="title3">
                        <td colspan="2" style="padding-left:8px;">
                            <%--<input type="radio" name="Speed" value="Quickest" /> <strong>Quickest</strong>--%>
                        </td>
                    </tr>
                    <tr class="directionButton">
                        <td align="right" colspan="2" style="padding-right:3px; padding-top:6px; padding-bottom:6px;">
                            <img alt="Get Directions" src="images/misc/Driving_Button.gif" onclick="GetDirectionMap();" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" border="0" style="width:270px;">
                    <tr class="secondTitle">
                        <td align="left" style="border-right:1px solid #b3c0cc;">
                            <p>
                                <strong>Route Summary</strong>
                            </p>
                        </td>
                    </tr>
                    <tr style="margin:0; height:15px;">
                        <td colspan="2" style="height:15px; padding:0px 0px 0px 8px; border-right:1px solid #b3c0cc;">
                            <table cellpadding="0" cellspacing="0" border="0" style="width:100%;">
                                <tr>
                                    <td valign="top" style="width:20px;">
                                        <p style="color:#008000;">
                                            Start:
                                        </p>
                                    </td>
                                    <td style="width:250px;">
                                        <p id="showStartAddress">
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr style="margin:0; height:15px;">
                        <td colspan="2" style="height:15px; padding:0px 0px 0px 8px; border-bottom:1px solid #b3c0cc; border-right:1px solid #b3c0cc;">
                            <table cellpadding="0" cellspacing="0" border="0" style="width:100%;">
                                <tr>
                                    <td valign="top" style="width:20px; padding-right:8px;">
                                        <p style="color:#ff0000;">
                                            End:
                                        </p>
                                    </td>
                                    <td style="width:250px;">
                                        <p id="showEndAddress">
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="secondTitle">
                        <td align="left" colspan="2" style="width:100%; border-right:1px solid #b3c0cc;">
                            <span id="showDirections"></span>
                        </td>
                    </tr>
                </table>
                </div>
            </td>
        </tr>
        <tr style="padding-top:4px; background-color:#d7edf4;">
            <td style="width:100%; border-top:1px solid #b3c0cc;">
                <table cellpadding="0" cellspacing="0" border="0" style="width:100%;">
                    <tr>
                        <td style="height:18px;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </div>

    </div>
    <%=this.RenderColdStartState() %>
    <%=this.RenderStartAndMaxRowsState() %>
    <%=this.RenderAvailableRowsState() %>
    <input type="hidden" id="cmd" />
    <input type="hidden" id="entry" />
    <script type="text/javascript">
        document.onmousemove = MouseMove;
    </script>

</asp:Content>