<%@ Page Language="C#" MasterPageFile="HSLMainDetailMap.master" AutoEventWireup="true" CodeFile="DetailsView.aspx.cs" Inherits="Web_DetailsView" %>

<asp:Content ID="detailsview" ContentPlaceHolderID="MainContent" Runat="Server">
<link title="combined" rel="stylesheet" type="text/css" media="all" href="css/hotspotdetails.css" />
<link title="combined" rel="stylesheet" type="text/css" media="all" href="css/table_test.css" />
<link title="combined" rel="stylesheet" type="text/css" media="all" href="css/DrivingDirections.css" />
<%--<link title="combined" rel="stylesheet" type="text/css" media="print" href="css/hotspotdetails_print.css" />
<link title="combined" rel="stylesheet" type="text/css" media="print" href="css/table_test_print.css" />--%>

<script language="javascript" src="js/DetailView.js" type="text/javascript"></script>
<div>
     <table id="detailTable" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td valign="middle" id="wrappingCell">
                <table width="100%" class="headerTable">
                    <tr style="height:20px;">
                        <td style="width:50%; padding-left:6px;" valign="middle" align="left">
                            <a id="history" href="javascript:history.back()" title="Back">Back</a> | <a id="searchViewLink" href="#" title="Search again">Search Again</a>
                        </td>
                        <td style="width:50%;" align="right">
                            <a href="#" title="Launch Help" style="vertical-align:text-top;" onclick="LaunchHelp('http://help.live.com', 'hsl_web', 'en-us', '', 'keyword', 'qaf', false, '', '', '', '');return false;">Help</a>
                            &nbsp;|&nbsp;&nbsp;<a href="javascript: window.print();" title="Print this page"><img src="images/misc/IconPrint.gif" alt="Print this page" style="border:0px;" /></a> <a style="vertical-align:text-top;" href="javascript: window.print();" title="Print this page">Print this page</a>
                            &nbsp;|&nbsp;&nbsp;<a id="listViewLink" href="#" title="Back to Search results"><img src="images/misc/SmIconMap.gif" alt="Back to Search results" style="border:0px;" /> <span style="vertical-align:text-top;" title="Back to Search results">List View</span></a>&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top" id="lowerCell">
                <div id="myMap" class="mapImage" style="position:relative; width:99.8%; height:869px;"></div>
            </td>
        </tr>
    </table>
    
    <%--Large Info Card--%>
    <div id="detailList" style="filter:Alpha(Opacity=90); left:100px; top:126px;">
        <table cellpadding="0" cellspacing="0" border="0" id="gadgetContainer" class="title1">
            <tr>
                <td style="height:16px; margin:0; padding:0; background-color: #5885b5;">
                    <table style="width:100%;" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="titleCell">
                                <strong class="infocardTitle"><%=this.RenderName() %></strong>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="height:56px; padding-top:6px;">
                    <table style="width:100%; height:56px;" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="indexImages" align="left" valign="top">
                                <img alt="Photo of Search result" src="<%=this.RenderPhoto() %>" class="indexImage" style="width:111px; height:56px;" />
                            </td>
                            <td align="left" valign="top" class="line_height_standard detailListAddress" style="width:100%;">
                                <%=this.RenderAddress() %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="amenityCell">
                    <table border="0" cellpadding="0" cellspacing="0" style="width:100%;">
                        <tr>
                            <%=this.RenderAmenities() %>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="margin:0; padding:0;">
                
                    <div id="hotspotsList">
                        <table cellpadding="0" cellspacing="0" border="0" style="width:95.5%;">
                            <tr>
                                <td colspan="2" style="width:100%;" class="line_height_standard leftAlignment">
                                    <%=this.RenderDescription() %>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="width:100%; padding-top: 4px;" class="line_height_standard leftAlignment">
                                    <%=this.RenderCoverage() %>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding-top: 6px; padding-bottom:0px;" class="line_height_standard leftAlignment">
                                    <strong class="textHighlight">Categories</strong><br />
                                    <%=this.RenderCategories() %>
                                    
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding-top: 6px; padding-bottom:0px;" class="line_height_standard leftAlignment">
                                    
                                    <strong class="textHighlight">Providers</strong> (<%=this.RenderNumberOfProviders() %>)
                                </td>
                            </tr>
                            <%=this.RenderProviders() %>
                        </table>
                    </div>
                </td>
            </tr>
            <tr class="footerRow">
                <td colspan="2" class="footerCell">
                    <table cellpadding="0" cellspacing="0" border="0" style="background-color: #ebf3fb; width:100%;">
                        <tr>
                            <td>
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
                            <textarea style="font-family:Arial; font-size:small;" id="endAddress" name="endAddress" rows="3" cols="40"><%= this.RenderAddress().Replace("<br />","") %></textarea>
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
                                        <p style="color:Green;">
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
                                        <p style="color:Red;">
                                            End:
                                        </p>
                                    </td>
                                    <td style="width:250px;">
                                        <p id="showEndAddress">
                                            <%=this.RenderAddress() %>
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
<%=this.RenderLatLong() %>
<%=this.RenderHotspotInformation() %>
<script type="text/javascript">
    document.onmousemove = MouseMove;
</script>
<input type="hidden" id="cmd" /> 

</asp:Content>
