var map = null;
var pinID = 1;

var g_SearchResults = null;
var g_PageSize = 10;
var g_Sort = 1;
var g_SortDir = 0;
var g_StartAtRow = 1;
var g_CurrentPage = 1;
var g_AvailableRows = 1;
var g_CurrentInfoCard = -1;

var g_CategoryNames = null;
var g_CategoryPhotos = null;

var g_AmenityNames = null;
var g_AmenityPhotos = null;

var g_CostSchemePhotos = null;
var g_SecuritySchemePhotos = null;
var g_SecuritySchemeAlt = null;

var g_SortEnum = null;

var g_MovePanel = null;
var g_MouseRelativeY;
var g_MouseRelativeX;

var g_Initialized = false;
var g_MapLoaded = false;
var g_firefox = document.getElementById && !document.all;	//detect browser
var directionsCardVisible = false;

var g_CostSchemeStrs;       //cost types

var g_scrollBarHeight = 13; // height of a scroll bar in a div
 
 function pageLoad()
 {
  if (g_Initialized == false)
  {
   map = new VEMap('myMap');

   g_Initialized = true;
   
   setListViewLink();
   
   var searchLink = document.getElementById("searchViewLink");
   searchLink.href = "http://"+window.location.host+genPageURI("MapView.aspx")+getCriteriaAsLink('', '')+"&cmd=searchview";
   
   g_StartAtRow = parseInt(document.getElementById("startAtRow").value) + 1;
   g_AvailableRows = parseInt(document.getElementById("availableRows").value);
  
   g_CategoryNames = new Array(); g_CategoryPhotos = new Array();
   g_CategoryNames.push('Unknown'); g_CategoryPhotos.push('null.jpg');
   g_CategoryNames.push('Airport'); g_CategoryPhotos.push('airport.jpg');
   g_CategoryNames.push('Bar'); g_CategoryPhotos.push('bar.jpg');
   g_CategoryNames.push('Beach'); g_CategoryPhotos.push('beach.jpg');
   g_CategoryNames.push('Boat / Ferry / Ship'); g_CategoryPhotos.push('boat.jpg');
   g_CategoryNames.push('Bus Station'); g_CategoryPhotos.push('bus-station.jpg');
   g_CategoryNames.push('Business Center'); g_CategoryPhotos.push('business-center.jpg');
   g_CategoryNames.push('Cafe'); g_CategoryPhotos.push('cafe.jpg');
   g_CategoryNames.push('Campground'); g_CategoryPhotos.push('campground.jpg');
   g_CategoryNames.push('Car Wash'); g_CategoryPhotos.push('car-wash.jpg');
   g_CategoryNames.push('Convention Center'); g_CategoryPhotos.push('convention-center.jpg');
   g_CategoryNames.push('Disco'); g_CategoryPhotos.push('disco.jpg');
   g_CategoryNames.push('Downtown Area'); g_CategoryPhotos.push('downtown-area.jpg');
   g_CategoryNames.push('Gas Station'); g_CategoryPhotos.push('gas-station.jpg');       
   g_CategoryNames.push('Government Office'); g_CategoryPhotos.push('govt-office.jpg');
   g_CategoryNames.push('Health Club'); g_CategoryPhotos.push('health-club.jpg');
   g_CategoryNames.push('Hospital'); g_CategoryPhotos.push('hospital.jpg');
   g_CategoryNames.push('Hotel / Resort'); g_CategoryPhotos.push('hotel-resort.jpg');
   g_CategoryNames.push('Hotzone'); g_CategoryPhotos.push('hotzone.jpg');
   g_CategoryNames.push('Internet Cafe'); g_CategoryPhotos.push('internet-cafe.jpg');
   g_CategoryNames.push('Library'); g_CategoryPhotos.push('library.jpg');
   g_CategoryNames.push('Marina'); g_CategoryPhotos.push('marina.jpg');
   g_CategoryNames.push('Office Building'); g_CategoryPhotos.push('office-building.jpg');
   g_CategoryNames.push('Other'); g_CategoryPhotos.push('other.jpg');
   g_CategoryNames.push('Park'); g_CategoryPhotos.push('park.jpg'); 
   g_CategoryNames.push('Phone Booth'); g_CategoryPhotos.push('phone-booth.jpg');
   g_CategoryNames.push('Private Club'); g_CategoryPhotos.push('private-club.jpg');
   g_CategoryNames.push('Pub'); g_CategoryPhotos.push('pub.jpg');
   g_CategoryNames.push('Public Space / Public Building'); g_CategoryPhotos.push('public-space.jpg');
   g_CategoryNames.push('Residence'); g_CategoryPhotos.push('residence.jpg');
   g_CategoryNames.push('Residential Area'); g_CategoryPhotos.push('residential-area.jpg');
   g_CategoryNames.push('Rest Area'); g_CategoryPhotos.push('rest-area.jpg');
   g_CategoryNames.push('Restaurant'); g_CategoryPhotos.push('restaurant.jpg');
   g_CategoryNames.push('RV Resort'); g_CategoryPhotos.push('rv.jpg');
   g_CategoryNames.push('School / University'); g_CategoryPhotos.push('school-university.jpg');
   g_CategoryNames.push('Sports Venue'); g_CategoryPhotos.push('sports-venue.jpg');
   g_CategoryNames.push('Store / Shopping Mall'); g_CategoryPhotos.push('store-shopping-mall.jpg');
   g_CategoryNames.push('Tourist Attraction'); g_CategoryPhotos.push('tourist-attraction.jpg');
   g_CategoryNames.push('Train Station'); g_CategoryPhotos.push('train-station.jpg');
   g_CategoryNames.push('Training Center'); g_CategoryPhotos.push('training-center.jpg');
   g_CategoryNames.push('Travel Center / Truck Stop'); g_CategoryPhotos.push('traveltruck.jpg');
   g_CategoryNames.push('Citywide Network'); g_CategoryPhotos.push('citywide.jpg');

   g_AmenityNames = new Array(); g_AmenityPhotos = new Array();
   g_AmenityNames[0] = "Food"; g_AmenityPhotos[0] = "sm_food";
   g_AmenityNames[1] = "Beverages"; g_AmenityPhotos[1] = "sm_beverages";
   g_AmenityNames[2] = "Restrooms"; g_AmenityPhotos[2] = "sm_restrooms";
   g_AmenityNames[3] = "Smoking"; g_AmenityPhotos[3] = "sm_smoking";
   g_AmenityNames[4] = "Outlets"; g_AmenityPhotos[4] = "sm_outlets";

   g_CostSchemePhotos = new Array();
   g_CostSchemePhotos[0] = "CostUnknown.png";
   g_CostSchemePhotos[1] = "CostFree.png";
   g_CostSchemePhotos[2] = "CostPaid.png";
   g_CostSchemePhotos[3] = "CostBoth.png";

   g_SecuritySchemePhotos = new Array(); g_SecuritySchemeAlt = new Array();
   g_SecuritySchemePhotos[0] = "lock_unknown.png"; g_SecuritySchemeAlt[0] = "Unknown";
   g_SecuritySchemePhotos[1] = "unlocked.png"; g_SecuritySchemeAlt[1] = "Unsecure";
   g_SecuritySchemePhotos[2] = "locked.png"; g_SecuritySchemeAlt[2] = "Secure";
   g_SecuritySchemePhotos[3] = "na.png"; g_SecuritySchemeAlt[3] = "N/A";
   g_SecuritySchemePhotos[4] = "na.png"; g_SecuritySchemeAlt[4] = "N/A";
   g_SecuritySchemePhotos[5] = "na.png"; g_SecuritySchemeAlt[5] = "N/A";
   
   g_SortEnum = new Array();
   g_SortEnum[0] = "None";
   g_SortEnum[1] = "Distance";
   g_SortEnum[2] = "Name";
   g_SortEnum[3] = "Cost";
   g_SortEnum[4] = "Security";
   g_SortEnum[5] = "Address"; 
   
   /*cost scheme */
   g_CostSchemeStrs = new Array();
   g_CostSchemeStrs[0] = "Unknown";
   g_CostSchemeStrs[1] = "Free";
   g_CostSchemeStrs[2] = "Paid";
   g_CostSchemeStrs[3] = "Both";  
   
   var sort = document.getElementById("sort").value.split('|');
   
   for (i = 0; i < g_SortEnum.length; i++)
   {
    if (g_SortEnum[i] == sort[0])
    {
     g_Sort = i;
     g_SortDir = (sort[1] == "Ascending") ? 0 : 1;
     break;
    }
   }
   
   var pageSizeSelector = document.getElementById("pageSizeSelector");
   var pageSizeOptions = pageSizeSelector.options;
   var maxRows = document.getElementById("maxRows");
   
   g_PageSize = parseInt(maxRows.value);
      
   for (i = 0; i < pageSizeOptions.length; i++)
   {
    if (maxRows.value == pageSizeOptions[i].value)
    {
     pageSizeSelector.selectedIndex = i;
     break;
    }
   }

   FetchHotspots();
  }
 }
  
 function PageNext()
 { 
  var refresh = false;
  
  InfoCardOff();
  
  if (g_HotspotsListCollapsed == true)
   onHotspotsListToggleCollapse();
 
  if ((g_StartAtRow + g_PageSize) <= g_AvailableRows)
  {
   g_StartAtRow = g_StartAtRow + g_PageSize;
   refresh = true;
  }

  if (refresh == true)
  {
   FetchHotspots();
  }    
 }
 
 function PagePrevious()
 {
  var refresh = false;
  
  InfoCardOff();
  
  if (g_HotspotsListCollapsed == true)
   onHotspotsListToggleCollapse();
  
  if ((g_StartAtRow - g_PageSize) >= 1)
  {
   g_StartAtRow = g_StartAtRow - g_PageSize;
   refresh = true;
  }
    
  if (refresh == true)
  {
   FetchHotspots();
  }
 }
 
 function onPageResize()
 {
  var pageSizeSelector = document.getElementById("pageSizeSelector");
  
  if (g_HotspotsListCollapsed == true)
   onHotspotsListToggleCollapse();
  
  g_PageSize = parseInt(pageSizeSelector.options[pageSizeSelector.selectedIndex].text);
  
  ProcessPageIndicator(false);
  
  g_StartAtRow = (g_CurrentPage - 1) * g_PageSize + 1;
  
  FetchHotspots();
 }
 
 function ProcessPageIndicator(display)
 {
  var pages = parseInt(g_AvailableRows / g_PageSize);
  
  if ((g_AvailableRows % g_PageSize) > 0)
   pages++;
  
  var page = parseInt((g_StartAtRow) / g_PageSize) + 1;
  if (((g_StartAtRow) % g_PageSize) == 0)
   page--;
   
  g_CurrentPage = page;
  
  if (display == true)
  {
   var hotspotsPage = document.getElementById("hotspotsPage");
   var arrowLeft = document.getElementById("arrowLeft");
   var arrowRight = document.getElementById("arrowRight");
   
   if (g_CurrentPage == 1)
   {
    arrowLeft.src = "images/misc/ArrowLeftOff.gif";
   }
   else
   {
    arrowLeft.src = "images/misc/ArrowLeft.gif";
   }
   
   if ((g_StartAtRow + g_PageSize) <= g_AvailableRows)
   {
    arrowRight.src = "images/misc/ArrowRight.gif";
   }
   else
   {
    arrowRight.src = "images/misc/ArrowRightOff.gif";
   }
   
   hotspotsPage.innerText = "Page " + page + " of " + pages; 
  }
 }
 
 function FetchHotspots()
 {
  var country = null;
  var region = null;
  var city = null;
  var postalCode = null;
  var areaCode = null;
  var categories = null;
  var serviceProviders = null;
  var distance = -1;
  var accessFee = "Free";
  
  if (document.getElementById("country") != null)
   country = document.getElementById("country").value;
   
  if (document.getElementById("region") != null)
   region = document.getElementById("region").value;

  if (document.getElementById("city") != null)
   city = document.getElementById("city").value;
  
  if (document.getElementById("postalCode") != null)
   postalCode = document.getElementById("postalCode").value;

  if (document.getElementById("distance") != null)
   distance = document.getElementById("distance").value;

  if (document.getElementById("accessFee") != null)
   accessFee = document.getElementById("accessFee").value;

  var searchType = document.getElementById("searchType");
  
  if (searchType == null || searchType.value == "advanced")
  {
   if (document.getElementById("areaCode") != null)
    areaCode = document.getElementById("areaCode").value;

   if (document.getElementById("categoriesStr") != null)
    categories = document.getElementById("categoriesStr").value;

   if (document.getElementById("serviceProvidersStr") != null)
    serviceProviders = document.getElementById("serviceProvidersStr").value;
  }
 
  setListViewLink();
    
  HSLAppModelFacade.SearchHotspots(
                            g_StartAtRow,
                            g_PageSize,
                            false,
                            country,
                            region,
                            city,
                            postalCode,
                            distance,
                            accessFee,
                            areaCode,
                            categories,
                            serviceProviders,
                            g_SortEnum[g_Sort] + "|" + (g_SortDir == 0 ? "Ascending" : "Descending"),
                            onFetchHotspotsCallBack,
                            onTimeout);
 }
 function setListViewLink()
 {
  var listLink = document.getElementById("listViewLink");
  
  listLink.href = "http://"+window.location.host+genPageURI("MapView.aspx")+getCriteriaAsLink(g_StartAtRow-1, g_PageSize)+"&cmd=listview";
 }

function onFetchHotspotsCallBack(results)
{
    g_SearchResults = results;

    ProcessPageIndicator(true);
      
    if (null != results && null != results.Hotspots && null != results.Hotspots.DTOs)
    {
        var hotspots = results.Hotspots.DTOs; 
        var hotspotsTable = document.getElementById("hotspotsTable");

        PlaceTagsOnMap(hotspots, -1, true);
             
        while (hotspotsTable.rows.length > 0)
        {
            hotspotsTable.deleteRow(0);      
        }
        
        for (i = 0; i < hotspots.length; i++)
        { 
            var hotspot = hotspots[i]; 
            var row = hotspotsTable.insertRow(hotspotsTable.rows.length);
            row.className="HslResults_Row";

            var cell1 = row.insertCell(0);
            var cell2 = row.insertCell(1);
            cell1.innerHTML = '<img id="tagNumber' + i + '" class="hotspotNumber" valign="top" src="images/numbers/' + (i + 1) + 'sm.gif" />&nbsp;';
            cell1.style.verticalAlign = 'top';

            var html = '<a href="#" onclick="ShowInfoCard(' + i + ');">' + hotspot.Name + '</a><br />' + getAddress(hotspot);
            cell2.innerHTML = html;
            cell2.style.verticalAlign = 'top';
        }
    }
}
 
 function HslResults_SelectedRow()
 {
  row.runtimeStyle.color  = '#B7D8ED';
  row.runtimeStyle.cursor = 'hand';
 }
 
 function getAddress(hotspot)
 {
  var region = hotspot.Region;
  var country = hotspot.Country;
  var phone = hotspot.Phone;
 
  if (country != null && (country == "United States" || country == "Canada"))
  {
   region = hotspot.RegionAbbr;
   country = hotspot.CountryAbbr;
  }
  
  if (phone != null && phone != "" && (country == "US" || country == "CA"))
  {
   var tokens = phone.split('-');
   
   if (tokens.length == 3)
    phone = '('+tokens[0]+') '+tokens[1]+'-'+tokens[2];
  }

  var address = hotspot.AddrStreet + '<br /> ' + hotspot.City + ',  ' + region + ' ' + hotspot.PostalCode + ', ' + country + '<br >' + phone;
  
  return address;               
 }
 
function onTimeout()
{
    alert("onTimeout");
}
 
function PlaceTagsOnMap(hotspots, tophotspot, reposition)
{
    if (g_MapLoaded == false)
    {
        map.LoadMap();
        g_MapLoaded = true;
    }
 
    var locs = new Array;

    for (i = 0; i < hotspots.length; i++)
    {
        var hotspot = hotspots[i];

        var loc = new VELatLong(hotspot.Latitude, hotspot.Longitude);
        locs.push(loc);
    }
  
    map.DeleteAllPushpins();
               
    for (i = locs.length - 1; i >= 0; i--)
    {
        if (i != tophotspot)
        {
            var pin = new VEPushpin(pinID, locs[i], 'images/numbers/'+(i + 1)+'.gif', hotspots[i].Name, getAddress(hotspots[i]));
               
            map.AddPushpin(pin);
            pinID++;
        }
    }
  
    if (tophotspot != -1)
    {
        var pin = new VEPushpin(pinID, locs[tophotspot], 'images/numbers/'+(tophotspot+1)+'b.gif', hotspots[tophotspot].Name, hotspots[tophotspot].Address);

        map.AddPushpin(pin);
        pinID++;  
    }
  
    if (reposition == true)
    {
        map.SetMapView(locs);
    }
}

function InfoCardOff()
{
    document.getElementById("infoCard").style.display = "none";
    g_MovePanel = null;

    UnfocusCurrentHotspotEntry();

    g_CurrentInfoCard = -1;
}

function DrivingCardOff()
{
    document.getElementById("directions").style.display = "none";
    g_MovePanel = null;
  
    g_CurrentInfoCard = -1;
}
 
function ShowInfoCard(entry)
{
    if (null != g_SearchResults)
    {
        var hotspot = g_SearchResults.Hotspots.DTOs[entry];   
        var cardName = document.getElementById("cardName");
        var cardNumProviders = document.getElementById("cardNumProviders");
        var cardLocationPhoto = document.getElementById("cardLocationPhoto");
        var cardAddress = document.getElementById("cardAddress");
        var cardProviders = document.getElementById("cardProviders");
        var email = document.getElementById("email");
        var email2 = document.getElementById("email2");

        UnfocusCurrentHotspotEntry();
   
        document.getElementById("tagNumber"+entry).src = "images/numbers/"+(entry+1)+"bsm.gif";
      
        g_CurrentInfoCard = entry + g_StartAtRow - 1;
        
        document.getElementById("infoCard").style.display = "block";
        
        var browserName = navigator.userAgent;
        var isIE = browserName.match(/MSIE/);
        var isIE7 = browserName.match(/MSIE 7/);
        var isFirefox = browserName.match(/Firefox/);
        
        if (MenuInitialized)
        {
            if (isIE)
            {
                var InfoCardRelativeY = g_topMenuSpacing + document.getElementById("infoCard").style.posTop;
                var InfoCardRelativeX = document.getElementById("infoCard").style.posLeft;
                document.getElementById("infoCard").style.cssText = "position:absolute; left:"+InfoCardRelativeX+"px; top:"+InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";    
            }
            else //Firefox
            {
                var InfoCardRelativeY = g_topMenuSpacing + document.getElementById("infoCard").style.top;
                var InfoCardRelativeX = document.getElementById("infoCard").style.left;
                document.getElementById("infoCard").style.cssText = "position:absolute; left:"+InfoCardRelativeX+"px; top:"+InfoCardRelativeY+"px; filter:Alpha(Opacity=85);";
            }
        }
        
        if (isIE7)
        {
            document.getElementById("infocardsList").style.cssText = "width:282px;";
        }
        
        if (isFirefox)
        {
            document.getElementById("infoCard").style.cssText = "position:absolute; left:700px; top:119px;";
        }
        
        PlaceTagsOnMap(g_SearchResults.Hotspots.DTOs, entry, false);
        map.SetCenter(new VELatLong(hotspot.Latitude, hotspot.Longitude));
        cardName.innerText = hotspot.Name;

        var html;

        if (hotspot.AccessPoints != null)
        {
            html = hotspot.AccessPoints.length + "";

            if (hotspot.AccessPoints.length > 1)
            {
                html += "";
            }
        }
        else
        {
            html = "0";
        }

        cardNumProviders.innerHTML = html;

        if (hotspot.LocationPhotoUrl != null)
        {
            cardLocationPhoto.src = hotspot.LocationPhotoUrl;
        }
        else
        {
            for (i = 0; i < g_CategoryNames.length; i++)
            {
                if (hotspot.Category == g_CategoryNames[i])
                {
                    cardLocationPhoto.src = 'images/categories/' + g_CategoryPhotos[i];
                    break;
                }
            }
        }

        cardAddress.innerHTML = getAddress(hotspot);

        while (cardProviders.rows.length > 0)
        {
            cardProviders.deleteRow(0);
        }
        
        if (null != hotspot.AccessPoints)
        {
            var providers = "";
            for (i = 0; i < hotspot.AccessPoints.length; i++)
            {     
                var ap = hotspot.AccessPoints[i];
                var row = cardProviders.insertRow(cardProviders.rows.length);
                var cell = row.insertCell(0);

                cell.width = "20%";
                cell.className = "providerImage";
                cell.innerHTML = '<img src="images/cost/' + g_CostSchemePhotos[ap.Cost] + '" align="right" />';
                if ("" != providers)
                {
                    providers += "\n";
                }
                
                providers += ap.ServiceProvider;

                providers += " (" + g_CostSchemeStrs[ap.Cost] + ")";

                cell = row.insertCell(0);
                cell.width = "80%";
                cell.className = "providerAlignment";         
                cell.innerText = ap.ServiceProvider;
            }
        }

        var amenities = hotspot.Amenities;
        var row = document.getElementById("cardAmenities");

        while (row.cells.length > 0)
        {
            row.deleteCell(0);
        }

        var cell = document.createElement("TD");
        cell.setAttribute("width", "20%");
        cell.setAttribute("align", "left");
        var img = document.createElement("IMG");
        img.setAttribute("src", "images/amenities/"+getAmenityPhoto(amenities[0]));
        img.setAttribute("alt", "Outlet availability");
        cell.appendChild(img);
        row.appendChild(cell);
        var cell = document.createElement("TD");
        cell.setAttribute("width", "20%");
        cell.setAttribute("align", "left");
        cell.setAttribute("className", "amenityCenterAsset");
        var img = document.createElement("IMG");
        img.setAttribute("src", "images/amenities/"+getAmenityPhoto(amenities[1]));
        img.setAttribute("alt", "Beverage availability");
        cell.appendChild(img);
        row.appendChild(cell);
        var cell = document.createElement("TD");
        cell.setAttribute("width", "20%");        
        var img = document.createElement("IMG");
        img.setAttribute("src", "images/amenities/"+getAmenityPhoto(amenities[2]));
        img.setAttribute("alt", "Food availability");
        cell.appendChild(img);
        row.appendChild(cell);
        var cell = document.createElement("TD");
        cell.setAttribute("width", "20%");
        cell.setAttribute("align", "right");
        cell.setAttribute("className", "amenity2RightAsset");
        var img = document.createElement("IMG");
        img.setAttribute("src", "images/amenities/"+getAmenityPhoto(amenities[3]));
        img.setAttribute("alt", "Smoking or Non-Smoking");
        cell.appendChild(img);
        row.appendChild(cell);
        var cell = document.createElement("TD");
        cell.setAttribute("width", "20%");
        cell.setAttribute("align", "right");
        cell.setAttribute("className", "amenityRightAsset");
        var img = document.createElement("IMG");
        img.setAttribute("src", "images/amenities/"+getAmenityPhoto(amenities[4]));
        img.setAttribute("alt", "Restroom availability");
        cell.appendChild(img);
        row.appendChild(cell);

        var amenities = "";
        for (i = 0; i < hotspot.Amenities.length; i++)
        {
            var amenity = hotspot.Amenities[i];
            if (!((null == amenity.Flag) || (true == amenity.Flag.HasValue && false == amenity.Flag.Value)))
            {
                if ("" != amenities)
                {
                    amenities += ", ";
                }
                
                amenities += amenity.Name;
            }
        }

        uri =  "http://" + window.location.host + genPageURI("MapView.aspx") + getCriteriaAsLink(g_StartAtRow-1, g_PageSize) + "&cmd=detailsview&entry=" + g_CurrentInfoCard;
        var href = getEmailLink(hotspot.Name, 
                                  hotspot.AddrStreet + "\n" + hotspot.City + " " + hotspot.Region + " " + hotspot.PostalCode, 
                                  hotspot.Phone,
                                  amenities, 
                                  hotspot.AmenitiesComments,
                                  hotspot.UseableAreasComments, 
                                  hotspot.Category,
                                  providers, 
                                  uri);
        email.href = href;
        email2.href = href;
    }
}

 
function getAmenityPhoto(amenity)
{
    var photo = null;
    
    for (j = 0; j < g_AmenityNames.length; j++)
    {
        if (g_AmenityNames[j] == amenity.Name)
        {
            photo = g_AmenityPhotos[j];
            break;
        }
    }
 
    if (photo != null)
    {      
        if ((null == amenity.Flag) || (true == amenity.Flag.HasValue && false == amenity.Flag.Value))
        {
            photo += '_off.gif';
        }
        else
        {
            photo += '.gif';
        }       
    }
    
    return photo;
}
  
 function UnfocusCurrentHotspotEntry()
 {
   if (g_CurrentInfoCard != -1 && (g_CurrentInfoCard >= (g_StartAtRow - 1)) && (g_CurrentInfoCard < (g_StartAtRow - 1 + g_PageSize)))
   {
    var tag = document.getElementById("tagNumber"+(g_CurrentInfoCard - g_StartAtRow + 1));
    tag.src = "images/numbers/"+(g_CurrentInfoCard - g_StartAtRow + 2) + "sm.gif";
   } 
 }
 
 
 
 function MouseDownHotspotsList(e)
 {
    MouseDown(e, document.getElementById("hotspots"));
 }
 
 function MouseDownInfoCard(e)
 {
    MouseDown(e, document.getElementById("infoCard"));
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
 
 function transitionStateOut()
 {
  document.getElementById("startAtRow").value = g_StartAtRow - 1;
  document.getElementById("maxRows").value = g_PageSize; 
 }
 
 function onSearchView()
 {
  var cmd = document.getElementById("cmd");
  cmd.setAttribute("value", "searchview");
  cmd.setAttribute("name", "cmd");

  document.aspnetForm.submit();
 }
 
 function onListView()
 {
  var cmd = document.getElementById("cmd");
  cmd.setAttribute("value", "listview");
  cmd.setAttribute("name", "cmd");
  
  transitionStateOut();
  
  document.aspnetForm.submit();
 }

 function onDetailsView()
 {
  var cmd = document.getElementById("cmd");
  cmd.setAttribute("value", "detailsview");
  cmd.setAttribute("name", "cmd");
  
  transitionStateOut();

  var cmd = document.getElementById("entry");
  cmd.setAttribute("value", g_CurrentInfoCard);
  cmd.setAttribute("name", "entry");

  document.aspnetForm.submit();
 }
 
 var g_HotspotsListCollapsed = false;
 
 var g_AnimateHeightDelta = 0;
 var g_AnimatePanel = null;
 var g_AnimatePanelPartner = null;
 var g_AnimateTargetHeight = 0;
 
 function onHotspotsListToggleCollapse()
 {
  var hotspots = document.getElementById("hotspotsList");
  
  if (g_AnimatePanel == null)
  {
   g_AnimateHeightDelta = (g_HotspotsListCollapsed == false) ? -60 : 60;
   g_AnimateTargetHeight = (g_HotspotsListCollapsed == false) ? 0 : 360;
   g_HotspotsListCollapsed = (g_HotspotsListCollapsed == false) ? true : false;
   
   var hotspotsCollapseDir = document.getElementById("hotspotsCollapseDir");
   hotspotsCollapseDir.src = "images/misc/" + ((g_HotspotsListCollapsed == true) ? "LgArrowDn.gif" : "LgArrowUp.gif");
   
   g_AnimatePanel = hotspots;
   g_AnimatePanelPartner = document.getElementById("hotspots");
   setTimeout("AnimatePanel();", 50);
  }
 }
 
function AnimatePanel()
{
    if (g_AnimatePanel != null)
    {
        var height = g_AnimatePanel.clientHeight;
        var barHeight = 0;
        var targetHeight = g_AnimateTargetHeight;
        
        // if there is a horizontal scroll bar
        if (0 != (height % g_AnimateHeightDelta))
        {
            /* height of the horizontal scroll bar is 13 pixels which is added to the 
               panel height, else it will never reach intended height and throws errors */
            barHeight = g_scrollBarHeight;
            // if roll down
            if ( 0 < g_AnimateHeightDelta)
            {
                /* new target height should be less for rolldown to account for 
                   horizontal scroll bar */
                targetHeight = g_AnimateTargetHeight - barHeight;
            }
        }
        
        if (height != targetHeight)
        {            
            g_AnimatePanel.style.height = (height + g_AnimateHeightDelta + barHeight) + "px";

            if (g_AnimatePanelPartner != null)
            {
                height = parseInt(g_AnimatePanelPartner.style.height.split('px'));
                try
                {
                    g_AnimatePanelPartner.style.height = "" + (height + g_AnimateHeightDelta) + "px";
                }
                catch(ex)
                {
                    g_AnimatePanelPartner.style.height = "0px";
                }
            }
            
            setTimeout("AnimatePanel();", 50);
        }
        else
        {
            g_AnimatePanel = null;
            g_AnimatePanelPartner = null;
        }
    }
}
 
 function getMoreActionsMenu(arrowMode) {
    
    if (arrowMode == "moreActions")
    {
        document.getElementById("openActions").style.cssText = "display:none;";
        document.getElementById("closeActions").style.cssText = "display:inline;";
        document.getElementById("Actions").style.cssText = "display:block; background-color: #ebf3fb; margin-bottom:-2px;";
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
 
 function showDirectionCard()
 {
    directionsCardVisible = true;
    document.getElementById("hotspots").style.cssText = "display:none;";
    document.getElementsByTagName("select")[0].style.visibility = "hidden";
    document.getElementById("history").setAttribute("href","javascript:history.go()");
    
    var browserName = navigator.userAgent;
    var isIE = browserName.match(/MSIE/);
    if (MenuInitialized)
    {
        if (isIE)
        {
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
    
    document.getElementById("infoCard").style.cssText = "display:none;";
    var hotspot = g_SearchResults.Hotspots.DTOs[g_CurrentInfoCard]
    document.getElementById("endAddress").innerHTML = hotspot.AddrStreet + ', ' + hotspot.City + ',  ' + hotspot.Region + ' ' + hotspot.PostalCode + ', ' + hotspot.Country;
    document.getElementById("showEndAddress").innerHTML = hotspot.AddrStreet + '<br /> ' + hotspot.City + ', ' + hotspot.Region + ' ' + hotspot.PostalCode + '<br />' + hotspot.Country;
 }
 
 function GetDirectionMap()
 {         
    var start = document.getElementById("startAddress").value;
    var end = document.getElementById("endAddress").value;
    map.GetRoute(start, end, null, null, onGotRoute);
 }
 
function onGotRoute(route)
{
    document.getElementById("hotspots").style.cssText = "display:none;";
    document.getElementsByTagName("select")[0].style.visibility = "hidden";
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
    routeinfo+= "Total distance: ";
    routeinfo+= route.Itinerary.Distance+" ";
    routeinfo+= route.Itinerary.DistanceUnit+"<br />";
    routeinfo+= "Total time: ";
    routeinfo+= route.Itinerary.Time+"<br />";

    var steps="<table border='0' cellspacing='0' cellpadding='0'>";
    var len = route.Itinerary.Segments.length;
    for(var i = 0; i < len; i++)
    {
        if (i == 0)
        {
            steps += '<tr><td valign="top"><img src="images/misc/DepartFlag.gif" /></td>';
        }
        else if (i == len - 1)
        {
            steps += '<tr><td valign="top"><img src="images/misc/Arrive.gif" /></td>';
        }
        else                   
        {
            steps += '<tr><td valign="top" align="center"><img src="images/misc/stepNumber'+(i)+'.gif" /></td>';
        } 
        
        steps += "<td valign='top' style='padding-left:4px; padding-bottom:2px; padding-right:2px; padding-top:0px;'>" + route.Itinerary.Segments[i].Instruction + " -- (";
        steps += route.Itinerary.Segments[i].Distance+") ";
        steps += route.Itinerary.DistanceUnit + "</td></tr>";
    }
    
    steps += "</table><br />";
    routeinfo+="<br />"+steps;
    document.getElementById("showDirections").innerHTML = routeinfo;
}

function MouseDownDirectionsList(e)
{
    MouseDown(e, document.getElementById("directions"));
}