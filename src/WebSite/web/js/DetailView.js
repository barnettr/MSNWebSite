function pageLoad()
{
    var searchLink = document.getElementById("searchViewLink");
    var listLink = document.getElementById("listViewLink");

    searchLink.href = "http://"+window.location.host+genPageURI("DetailsView.aspx")+getCriteriaAsLink('', '')+"&cmd=searchview";
    listLink.href = "http://"+window.location.host+genPageURI("DetailsView.aspx")+getCriteriaAsLink('', '')+"&cmd=listview";

    map = new VEMap('myMap');
    map.LoadMap();

    var lat = document.getElementById("latitude");
    var lng = document.getElementById("longitude");

    if (lat != null && lng != null)
    {
        var loc = new VELatLong(lat.value, lng.value);
        var pin = new VEPushpin(1, loc, 'images/numbers/1.gif', null, null);

        map.AddPushpin(pin);
        map.SetZoomLevel(15);
        map.SetCenter(loc);
    }

    var email = document.getElementById("email");
    email.href = getEmailLink(document.getElementById("cardName").value, 
                              document.getElementById("cardAddress").value, 
                              document.getElementById("cardPhone").value,
                              document.getElementById("cardAmenities").value, 
                              document.getElementById("cardDescription").value,
                              document.getElementById("cardCoverage").value, 
                              document.getElementById("cardCategories").value,
                              document.getElementById("cardProviders").value,
                              window.location.href);
}
 
 function DashboardRoleup()
 {
  window.dbRoller.rollIn(Microsoft.Web.Animation.RollDirection.BottomUp);
 }

 function onSearchView()
 {
  var cmd = document.getElementById("cmd");
  cmd.setAttribute("value", "searchview");
  cmd.setAttribute("name", "cmd");
  
  document.aspnetForm.submit(); 
 }

 
 function onMapView()
 {
  var cmd = document.getElementById("cmd");
  cmd.setAttribute("value", "mapview");
  cmd.setAttribute("name", "cmd");
  
  document.aspnetForm.submit(); 
 }
 
 function onListView()
 {
  var cmd = document.getElementById("cmd");
  cmd.setAttribute("value", "listview");
  cmd.setAttribute("name", "cmd");
  
  document.aspnetForm.submit();
 }
 
 function getMoreActionsMenu(arrowMode) {
    
    if (arrowMode == "moreActions")
    {
        document.getElementById("openActions").style.cssText = "display:none;";
        document.getElementById("closeActions").style.cssText = "display:inline;";
        document.getElementById("Actions").style.cssText = "display:block; background-color: #ebf3fb;";
    }
 }
 
 function closeMoreActionsMenu(arrowMode) {
    
    if (arrowMode == "noActions")
    {
        document.getElementById("closeActions").style.cssText = "display:none;";
        document.getElementById("openActions").style.cssText = "display:inline;";
        document.getElementById("Actions").style.cssText = "display:none; background-color: #ebf3fb;";
    }
 }
 
 var directionsCardVisible = false;
 function showDirectionCard() {
    
    directionsCardVisible = true;
    document.getElementById("detailList").style.cssText = "display:none;";
    document.getElementById("history").setAttribute("href","javascript:history.go()");
    
    var browserName = navigator.userAgent;
    var isIE = browserName.match(/MSIE/);
    if (MenuInitialized)
    {
        if (isIE) {
            var d_InfoCardRelativeY = g_topMenuSpacing + document.getElementById("directions").style.posTop;
            var d_InfoCardRelativeX = document.getElementById("directions").style.posLeft;
        }
        else // Firefox
        {
            var d_HotspotsRelativeY = g_topMenuSpacing + parseInt(document.getElementById("hotspots").style.top);
            var d_HotspotsRelativeX = parseInt(document.getElementById("hotspots").style.left);
        }
        document.getElementById("directions").style.cssText = "position:absolute; left:"+d_InfoCardRelativeX+"px; top:"+d_InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";
    }
    else
    {
        document.getElementById("directions").style.cssText = "display:block; filter:Alpha(Opacity=95); position:absolute; left:100px; top:126px;";
    }
 }
 
 function GetDirectionMap()
 {         
    var start = document.getElementById("startAddress").value;
    var end = document.getElementById("endAddress").value;
    map.GetRoute(start, end, null, null, onGotRoute);
    
    alert ("TEST 1: pick up the myMap_veplacelistpanel DIV with document.getElementById and fire the click() event");
    var myMap_veplacelistpanel = document.getElementById("myMap_veplacelistpanel");
    myMap_veplacelistpanel.getElementsByTagName("A")[2].click();
    return true;
 }
 
 function onGotRoute(route)
 {
    document.getElementById("detailList").style.cssText = "display:none;";
    var browserName = navigator.userAgent;
    var isIE = browserName.match(/MSIE/);
    if (MenuInitialized)
    {
        if (isIE)
        {
            var d_InfoCardRelativeY = document.getElementById("directions").style.posTop;
            var d_InfoCardRelativeX = document.getElementById("directions").style.posLeft;
        }
        else // Firefox
        {
            var d_HotspotsRelativeY = parseInt(document.getElementById("hotspots").style.top);
            var d_HotspotsRelativeX = parseInt(document.getElementById("hotspots").style.left);
        }
        document.getElementById("directions").style.cssText = "position:absolute; left:"+d_InfoCardRelativeX+"px; top:"+d_InfoCardRelativeY+"px; filter:Alpha(Opacity=95);";
    }
    else
    {
        document.getElementById("directions").style.cssText = "display:block; filter:Alpha(Opacity=95); position:absolute; left:100px; top:126px;";
    }
    var showStartAddress = document.getElementById("startAddress").value;
    document.getElementById("showStartAddress").innerHTML = showStartAddress;
    var routeinfo="<strong>Driving Directions:</strong><br />";
    routeinfo+="Total distance: ";
    routeinfo+=   route.Itinerary.Distance+" ";
    routeinfo+=   route.Itinerary.DistanceUnit+"<br />";
    routeinfo+="Total time: ";
    routeinfo+=   route.Itinerary.Time+"<br />";
    
    var steps="<table border='0' cellspacing='0' cellpadding='0'>";
    var len = route.Itinerary.Segments.length;
       for(var i = 0; i < len ;i++)
       {
          if (i == 0)
          {
            steps += '<tr><td valign="top"><img src="images/misc/DepartFlag.gif" /></td>'
          }
          else if (i == len - 1)
          {
            steps += '<tr><td valign="top"><img src="images/misc/Arrive.gif" /></td>'
          }
          else                   
          {
            steps += '<tr><td valign="top" align="center"><img src="images/misc/stepNumber'+(i)+'.gif" /></td>'
          } 
          steps += "<td valign='top' style='padding-left:4px; padding-bottom:2px; padding-right:2px; padding-top:0px;'>"+route.Itinerary.Segments[i].Instruction+" -- (";
          steps += route.Itinerary.Segments[i].Distance+") ";
          steps += route.Itinerary.DistanceUnit+"</td></tr>";
       }
    steps += "</table><br />"
    routeinfo+="<br />"+steps;
    document.getElementById("showDirections").innerHTML = routeinfo;
    //alert ("end of onGotRoute Method");
 }
 
 function DrivingCardOff()
 {
    document.getElementById("directions").style.display = "none";
    document.getElementById("detailList").style.display = "none";
    g_MovePanel = null;
  
    g_CurrentInfoCard = -1;
 }
 
 var g_MovePanel = null;
 var g_firefox = document.getElementById && !document.all;	//detect browser
 
 function MouseDownDirectionsList(e)
 {
    MouseDown(e, document.getElementById("directions"));
 }
 
 function MouseDown(e, panel)
 {
    if (g_MovePanel == null)
    {
        g_MovePanel = panel;  
  
        if (g_firefox)
        {
            g_MouseRelativeY = parseInt(e.clientY) - parseInt(panel.style.top) + "px";
            g_MouseRelativeX = parseInt(e.clientX) - parseInt(panel.style.left) + "px";
        }
        else
        {
	        g_MouseRelativeY = event.clientY - panel.style.posTop;
            g_MouseRelativeX = event.clientX - panel.style.posLeft;
        }
    }
    else
    {
        g_MovePanel = null;
    }
 }
  
 function MouseMove(e)
 {
  

    if (g_MovePanel == null)
        return;
  
    if (g_firefox)
    {
        x = e.clientX;
        y = e.clientY;
   
        g_MovePanel.style.top = parseInt(y) - parseInt(g_MouseRelativeY) + "px";
        g_MovePanel.style.left = parseInt(x) - parseInt(g_MouseRelativeX) + "px";		
    }
    else
    {
        x=event.clientX;
        y=event.clientY;
        
        g_MovePanel.style.posTop = y - g_MouseRelativeY;
        g_MovePanel.style.posLeft = x - g_MouseRelativeX;
    }
 }