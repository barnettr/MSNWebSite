<%@ Page Language="C#" MasterPageFile="HSLMain.master" AutoEventWireup="true" CodeFile="ListView.aspx.cs" Inherits="Web_ListView" %>

<asp:Content ID="listview" ContentPlaceHolderID="MainContent" Runat="Server">
<link title="combined" rel="stylesheet" type="text/css" media="screen" href="css/searchresults.css" />
<link title="combined" rel="stylesheet" type="text/css" media="print" href="css/searchresults_print.css" />

<script language="javascript" src="js/ListView.js" type="text/javascript"></script>
 

    <table style="width:100%;" border="0" cellpadding="0" cellspacing="0" id="wrappingTable">
        <tr>
            <td style="width:100%;" colspan="">
                <table style="width:100%;" border="0" class="headerTable">
                    <tr style="height:20px;">
                        <td align="left" style="width:50%; padding-left:2px;">
                            <strong class="textHighlight"><%=this.RenderNumberOfResults() %></strong>
					        &nbsp;|&nbsp;
                            <a id="searchViewLink" href="#" title="Search Again">Search Again</a>
                        </td>
                        <td style="width:50%; padding-right:4px;" align="right">
                            <a href="#" onclick="LaunchHelp('<%=GetHelpURL() %>', 'hsl_web', 'en-us', '', 'keyword', 'qaf', false, '', '', '', '');return false;" title="Help">Help</a>
                            <% if (null == this.InfoMessage) { %>
                            &nbsp;|&nbsp;&nbsp;
                            <a href="javascript: window.print();" title="Print this page"><img src="images/misc/IconPrint.gif" alt="Print this page" style="width:14px; height:15px; border-style:none;" /></a> <a href="javascript: window.print();">Print this page</a>
                            &nbsp;|&nbsp;&nbsp;
                            <a id="mapViewLink" href="#" title="Map view"><img src="images/misc/SmIconMap.gif" alt="Map view" style="width:14px; height:12px; border-style:none;" /> Map View</a>
                            <% } %>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <% if (null == this.InfoMessage) { %>
        <tr>
            <td colspan="">
                <table style="width:100%;" cellpadding="0" cellspacing="0" class="rowLine">
                    <tr style="height:20px;">
                        <td align="left">
                            <p>Show <%=this.RenderPageSizeSelectors() %>results per page</p>
                        </td>
                        <td align="right">
                            <%=this.RenderPageNextPrevious() %>
                        </td>
                    </tr>
                </table>  
            </td>
        </tr>
        <% } %>
        <tr>
            <td colspan="">
                <table class="results" cellpadding="0" cellspacing="1" style="height:100%; width:100%;">
                    <tr valign="middle" class="HslResults_HeadingRow">
                        <td style="width:42px;">&nbsp;</td>
                        <td style="width:300px;" class="HslResults_HeadingRow" onclick="onSort('Name');"><p class="line_height_standard"><a class="HslResults_HeadingRow" style="text-decoration:none" href=# onClick="onSort('Name');">Name</a><%=this.RenderSort("Name") %></p></td>
                        <td style="width:500px;" class="HslResults_HeadingRow" onclick="onSort('Address');"><p class="line_height_standard"><a class="HslResults_HeadingRow" style="text-decoration:none" href=# onClick="onSort('Name');">Address</a><%=this.RenderSort("Address") %></p></td>
                        <% if (true == this.DisplayDistance) { %>
                        <td style="width:70px;" align="center" class="HslResults_HeadingRow" onclick="onSort('Distance');"><p class="line_height_standard"><a class="HslResults_HeadingRow" style="text-decoration:none" href=# onClick="onSort('Name');">Distance</a><%=this.RenderSort("Distance") %></p></td>
                        <% } %>
                        <td style="width:70px;" align="center" class="HslResults_HeadingRow" onclick="onSort('Cost');"><p class="line_height_standard"><a class="HslResults_HeadingRow" style="text-decoration:none" href=# onClick="onSort('Name');">Cost</a><%=this.RenderSort("Cost") %></p></td>
                        <!--td width="58px" align="center"><div class="HslResults_HeadingRow" onclick="onSort('Security');">Security</div></td-->
                    </tr>
                    <%=this.RenderHotspotsTable() %>
                </table>  
            </td>
        </tr>
        <% if (null == this.InfoMessage) { %>
        <tr>
            <td colspan="">
                <table cellpadding="0" cellspacing="0" border="0" style="width:100%;" class="resultsTable">
                    <tr style="height:15px;">
                        <td align="left" style="width:50%;">
                            Show <%=this.RenderPageSizeSelectors() %>results per page
                        </td>
                        <td align="right" style="width:50%;">
                            <%=this.RenderPageNextPrevious() %>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <% } %>
    </table>

<%=this.RenderColdStartState() %>
<%=this.RenderStartAndMaxRowsState() %>
<%=this.RenderAvailableRowsState() %>

<input type="hidden" id="cmd" name="cmd" value="" />
<input type="hidden" id="entry" name="entry" value="" />   
 <!--script type="text/javascript">
     pageLoad();
</script-->

</asp:Content>