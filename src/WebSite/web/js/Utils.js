var g_topMenuSpacing = 85;
var g_infoCardHeightCollapsed = 60;
var g_infoCardHeightMaximized = 420;
var MenuInitialized = false;

// JScript File
function createDashboardRoller()
{
    // create the Dashboard Roller
    var toggleGlyph = map.vemapcontrol.GetDashboard().GetHeader().lastChild;
    map.vemapcontrol.GetDashboard().ShowToggleGlyph();
    		
    window.dbRoller = new Microsoft.Web.Animation.Roller(map.vemapcontrol.GetDashboard().GetElement());
    dbRoller.AccFunction = AccelerationFunctions.CrazyElevator;
    dbRoller.LeaveAmount = 17;
    		
    dbRoller.onbeforerollin = function()
    {
        dbRoller.LeaveAmount = map.vemapcontrol.GetDashboard().GetHeader().offsetHeight + 1;
    };
    		
    dbRoller.onafterrollin = function()
    {
        map.vemapcontrol.GetDashboard().GetElement().className = map.vemapcontrol.GetDashboard().GetElement().className.replace(/\s*expanded/g, '');
        map.vemapcontrol.GetDashboard().GetElement().className += ' collapsed';
    };
    		
    dbRoller.onbeforerollout = function()
    {
        map.vemapcontrol.GetDashboard().GetElement().className = map.vemapcontrol.GetDashboard().GetElement().className.replace(/\s*collapsed/g, '');
        map.vemapcontrol.GetDashboard().GetElement().className += ' expanded';
    };
    		
    toggleGlyph.onclick = function()
    {
        if (dbRoller.isExpanded())
        { 
            dbRoller.rollIn(Microsoft.Web.Animation.RollDirection.BottomUp); 
        }
        else
        {
            dbRoller.rollOut(Microsoft.Web.Animation.RollDirection.TopDown); 
        }
    }
    		
    toggleGlyph = null;
}	
 
function genPageURI(page)
{
    var tokens = window.location.pathname.split('/');

    var uri = "";

    for (i = 0; i < tokens.length - 1; i++)
    uri += tokens[i] + '/';

    return uri+page;
}
 
 function getCriteriaAsLink(startAtRow, maxRows)
 {
  var pars = "";
    
  if (document.getElementById("country") != null)
   pars += "?country="+encodeURIComponent(document.getElementById("country").value);
   
  if (document.getElementById("region") != null)
   pars += "&region="+encodeURIComponent(document.getElementById("region").value);

  if (document.getElementById("city") != null)
   pars += "&city="+encodeURIComponent(document.getElementById("city").value);
  
  if (document.getElementById("postalCode") != null)
   pars += "&postalCode="+encodeURIComponent(document.getElementById("postalCode").value);

  if (document.getElementById("distance") != null)
   pars += "&distance="+document.getElementById("distance").value;

  if (document.getElementById("accessFee") != null)
   pars += "&accessFee="+document.getElementById("accessFee").value;

  var searchType = "basic";
  if (document.getElementById("searchType") != null)
   searchType = document.getElementById("searchType").value;

  pars += "&searchType="+searchType;
 
  if (searchType == "advanced")
  {
   if (document.getElementById("areaCode") != null)
    pars += "&areaCode="+encodeURIComponent(document.getElementById("areaCode").value);

   if (document.getElementById("categoriesStr") != null)
    pars += "&categoriesStr="+encodeURIComponent(document.getElementById("categoriesStr").value);

   if (document.getElementById("serviceProvidersStr") != null)
    pars += "&serviceProvidersStr="+encodeURIComponent(document.getElementById("serviceProvidersStr").value);
  }
  
  if (startAtRow == '')
   pars += "&startAtRow="+document.getElementById("startAtRow").value;
  else
   pars += "&startAtRow="+startAtRow;
   
  if (maxRows == '') 
   pars += "&maxRows="+document.getElementById("maxRows").value;
  else
   pars += "&maxRows="+maxRows;
    
  pars += "&sort="+document.getElementById("sort").value;
  
  return pars;
 }

function getFirefoxHeader() 
{
    var browserName = navigator.userAgent;
    var isIE = browserName.match(/MSIE/);
    if ( (isIE == ' ') || (isIE == null) )
    {
        document.getElementById("MSNWebSearch").style.cssText = "margin:0 0 0 0;";
        document.getElementById("searchDiv").style.cssText = "margin-top:35px;";
        document.getElementById("searchMapDiv").style.cssText = "margin-bottom:10px;";
        document.getElementById("header").style.cssText = "margin-bottom:0.6em;";
        document.getElementById("name").style.cssText = "vertical-align:middle;padding-bottom:16px;";
        document.getElementById("copyright").style.cssText = "padding-bottom:16px;";
        document.getElementById("headerButton").style.cssText = "height:19px;";
    }
 }
 
function addLoadEvent(func) 
{
    var oldonload = window.onload;
    if (typeof window.onload != 'function') 
    {
        window.onload = func;
    } 
    else 
    {
        window.onload = function() 
        {
            oldonload();
            func();
        }
    }
}

addLoadEvent(getFirefoxHeader);

function showMoreList(buttonMode) 
{
    if (buttonMode == "moreItems")
    {
        document.getElementById("moreItems").style.cssText = "display:none;";
        document.getElementById("lessItems").style.cssText = "display:inline; border:0px;";
        window.setTimeout('document.getElementById("rowShowMenu").style.cssText = "display:table-row; background-color:#ffffff;"', 80);
        document.getElementById("bg").style.cssText = "padding-left:5px; padding-top:3px; height:22px; background-image:url(images/misc/cell_bg.gif); background-repeat:repeat-x;";
        document.getElementById("bg_2").style.cssText = "vertical-align:middle; background-image:url(images/misc/cell_bg.gif); background-repeat:repeat-x;";
        window.focus();
        // only do this for pages with hotspot and info card
        if (null != document.getElementById("hotspots"))
        {
            var infoCardHeight = g_infoCardHeightMaximized;
            if (true == g_HotspotsListCollapsed)
            {
                infoCardHeight = g_infoCardHeightCollapsed;
            }
            
            document.getElementById("hotspots").style.cssText = "left:100px; top:208px; height:" + infoCardHeight + "px; filter:Alpha(Opacity=95);";            
            if (g_CurrentInfoCard != -1)
            {
                document.getElementById("infoCard").style.cssText = "position:absolute; left:700px; top:208px; filter:Alpha(Opacity=85);";
            }
        }
    }
}
 
function showNoList(buttonMode) 
{   
    if (buttonMode == "lessItems")
    {
        document.getElementById("lessItems").style.cssText = "display:none;";
        document.getElementById("moreItems").style.cssText = "display:inline; border:0px;";
        window.setTimeout('document.getElementById("rowShowMenu").style.cssText = "display:none;"', 80);
        document.getElementById("bg").style.cssText = "padding-left:5px; padding-top:3px; height:22px; background-color:#869cb2;";
        document.getElementById("bg_2").style.cssText = "vertical-align:middle; background-color:#869cb2;";
        window.focus();
        
        // only do this for pages with hotspot and info card
        if (null != document.getElementById("hotspots"))
        {
            var infoCardHeight = g_infoCardHeightMaximized;
            if (true == g_HotspotsListCollapsed)
            {
                infoCardHeight = g_infoCardHeightCollapsed;
            }

            document.getElementById("hotspots").style.cssText = "left:100px; top:126px; height:" + infoCardHeight + "px; filter:Alpha(Opacity=95);";
            if (g_CurrentInfoCard != -1)
            {
             document.getElementById("infoCard").style.cssText = "position:absolute; left:700px; top:126px; filter:Alpha(Opacity=85);";
            }
        }
    }
}
 
function showDetailPageMoreList(buttonMode) 
{    
    MenuInitialized = true;
    if (buttonMode == "moreItems")
    {
        document.getElementById("moreItems").style.cssText = "display:none;";
        document.getElementById("lessItems").style.cssText = "display:inline; border:0px;";
        document.getElementById("lessItems").focus();
        window.setTimeout('document.getElementById("rowShowMenu").style.cssText = "display:table-row; background-color:#ffffff;"', 80);
        document.getElementById("bg").style.cssText = "padding-left:5px; padding-top:3px; height:22px; background-image:url(images/misc/cell_bg.gif); background-repeat:repeat-x;";
        document.getElementById("bg_2").style.cssText = "vertical-align:middle; background-image:url(images/misc/cell_bg.gif); background-repeat:repeat-x;";
        window.focus();
        
        var browserName = navigator.userAgent;
        var isIE = browserName.match(/MSIE/);
        if (document.getElementById("hotspots"))
        {
            if (isIE)
            {
                var l_HotspotsRelativeY = g_topMenuSpacing + document.getElementById("hotspots").style.posTop;
                var l_HotspotsRelativeX = document.getElementById("hotspots").style.posLeft;
            }
            else // Firefox
            {
                var l_HotspotsRelativeY = g_topMenuSpacing + parseInt(document.getElementById("hotspots").style.top);
                var l_HotspotsRelativeX = parseInt(document.getElementById("hotspots").style.left);
            }
            
            var infoCardHeight = g_infoCardHeightMaximized;
            if (true == g_HotspotsListCollapsed)
            {
                infoCardHeight = g_infoCardHeightCollapsed;
            }
            
            document.getElementById("hotspots").style.cssText = "left:"+l_HotspotsRelativeX+"px; top:"+l_HotspotsRelativeY+"px; height:" + infoCardHeight + "px; filter:Alpha(Opacity=95);";
            if (g_CurrentInfoCard != -1)
            {
                if (isIE)
                {
                    var c_InfoCardRelativeY = g_topMenuSpacing + document.getElementById("infoCard").style.posTop;
                    var c_InfoCardRelativeX = document.getElementById("infoCard").style.posLeft;
                }
                else // Firefox
                {
                    var c_InfoCardRelativeY = g_topMenuSpacing + parseInt(document.getElementById("infoCard").style.top);
                    var c_InfoCardRelativeX = parseInt(document.getElementById("infoCard").style.left);
                }
                
                document.getElementById("infoCard").style.cssText = "position:absolute; left:"+c_InfoCardRelativeX+"px; top:"+c_InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";
            }
        }
        
        if (document.getElementById("detailList"))
        {
            document.getElementById("detailList").style.cssText = "filter:Alpha(Opacity=90); left:100px; top:211px;";
        }
        
        if (directionsCardVisible)
        {
            if (isIE)
                {
                    var d_InfoCardRelativeY = g_topMenuSpacing + document.getElementById("directions").style.posTop;
                    var d_InfoCardRelativeX = document.getElementById("directions").style.posLeft;
                }
                else // Firefox
                {
                    var d_InfoCardRelativeY = g_topMenuSpacing + parseInt(document.getElementById("directions").style.top);
                    var d_InfoCardRelativeX = parseInt(document.getElementById("directions").style.left);
                }
                
                document.getElementById("directions").style.cssText = "position:absolute; left:"+d_InfoCardRelativeX+"px; top:"+d_InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";
                if (document.getElementById("hotspots"))
                {
                    document.getElementById("hotspots").style.cssText = "display:none;";
                    document.getElementsByTagName("select")[0].style.visibility = "hidden";
                    document.getElementById("infoCard").style.cssText = "display:none;";
                }
                
                if (document.getElementById("detailList"))
                {
                    document.getElementById("detailList").style.cssText = "display:none;";
                }
        }
    }
}
 
function showDetailPageNoList(buttonMode) 
{   
    MenuInitialized = false;
    if (buttonMode == "lessItems")
    {
        document.getElementById("lessItems").style.cssText = "display:none;";
        document.getElementById("moreItems").style.cssText = "display:inline; border:0px;";
        window.setTimeout('document.getElementById("rowShowMenu").style.cssText = "display:none;"', 80);
        document.getElementById("bg").style.cssText = "padding-left:5px; padding-top:3px; height:22px; background-color:#869cb2;";
        document.getElementById("bg_2").style.cssText = "vertical-align:middle; background-color:#869cb2;";
        window.focus();
        
        var browserName = navigator.userAgent;
        var isIE = browserName.match(/MSIE/);
        if (document.getElementById("hotspots"))
        {
            if (isIE)
            {
                var l_HotspotsRelativeY = document.getElementById("hotspots").style.posTop - g_topMenuSpacing;
                var l_HotspotsRelativeX = document.getElementById("hotspots").style.posLeft;
            }
            else // Firefox
            {
                var l_HotspotsRelativeY = parseInt(document.getElementById("hotspots").style.top) - g_topMenuSpacing;
                var l_HotspotsRelativeX = parseInt(document.getElementById("hotspots").style.left);
            }
            
            var infoCardHeight = g_infoCardHeightMaximized;
            if (true == g_HotspotsListCollapsed)
            {
                infoCardHeight = g_infoCardHeightCollapsed;
            }
            
            document.getElementById("hotspots").style.cssText = "left:"+l_HotspotsRelativeX+"px; top:"+l_HotspotsRelativeY+"px; height:" + infoCardHeight + "px; filter:Alpha(Opacity=95);";        
            if (g_CurrentInfoCard != -1)
            {
                if (isIE)
                {
                    var c_InfoCardRelativeY = document.getElementById("infoCard").style.posTop - g_topMenuSpacing;
                    var c_InfoCardRelativeX = document.getElementById("infoCard").style.posLeft;
                }
                else // Firefox
                {
                    var c_InfoCardRelativeY = parseInt(document.getElementById("infoCard").style.top) - g_topMenuSpacing;
                    var c_InfoCardRelativeX = parseInt(document.getElementById("infoCard").style.left);
                }
                
                document.getElementById("infoCard").style.cssText = "position:absolute; left:"+c_InfoCardRelativeX+"px; top:"+c_InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";
            }
        }
        
        if (document.getElementById("detailList"))
        {
            document.getElementById("detailList").style.cssText = "filter:Alpha(Opacity=90); left:100px; top:129px;";
        }
        
        if (directionsCardVisible)
        {
            if (isIE)
                {
                    var d_InfoCardRelativeY = document.getElementById("directions").style.posTop - g_topMenuSpacing;
                    var d_InfoCardRelativeX = document.getElementById("directions").style.posLeft;
                }
                else // Firefox
                {
                    var d_InfoCardRelativeY = parseInt(document.getElementById("directions").style.top) - g_topMenuSpacing;
                    var d_InfoCardRelativeX = parseInt(document.getElementById("directions").style.left);
                }
                
                document.getElementById("directions").style.cssText = "position:absolute; left:"+d_InfoCardRelativeX+"px; top:"+d_InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";
                if (document.getElementById("hotspots"))
                {
                    document.getElementById("hotspots").style.cssText = "display:none;";
                    document.getElementsByTagName("select")[0].style.visibility = "hidden";
                    document.getElementById("infoCard").style.cssText = "display:none;";
                }
                
                if (document.getElementById("detailList"))
                {
                    document.getElementById("detailList").style.cssText = "display:none;";
                }
        }
    }
}

function getEmailLink(establishment, address, phone, amenities, description, coverage, categories, providers, uri)
{
    var link = "mailto:";
    link += "?subject=MSN WiFi Hotspot Locator";
    link += "&body=Hi, I've found this great Hotspot location:";

    if ("" != establishment)
    {
        link += "%0A%0A" + encodeURIComponent(establishment);
    }

    if ("" != address)
    {
        link += "%0A" + encodeURIComponent(address);
    }

    if ("" != phone)
    { 
        link += "%0A" + encodeURIComponent(phone);
    }

    if ("" != amenities)
    {
        link += "%0A%0AAmenities:%0A" + encodeURIComponent(amenities);
    }

    if ("" != description)
    {
        link += "%0A%0ADescription:%0A" + encodeURIComponent(description);
    }

    if ("" != coverage)
    {
        link += "%0A%0ACoverage:%0A" + encodeURIComponent(coverage);
    }

    if ("" != categories)
    {
        link += "%0A%0ALocation Type:%0A" + encodeURIComponent(categories);
    }

    if ("" != providers)
    {
        link += "%0A%0AService Providers:%0A" + encodeURIComponent(providers);
    }

    if ("" != uri)
    {
        link += "%0A%0AHere's the web link:%0A" + encodeURIComponent(uri);
    }

    return link;
}