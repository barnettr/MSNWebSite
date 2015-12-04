 var g_ImageIndex = 0;
 var g_ImageNames;
 
 var g_SelectedProviders = new Array();
 var g_ServiceProviders;

 function pageLoad()
 {
  setSearchMode(document.getElementById("searchType").value);
    
  g_ImageNames = new Array();
  g_ImageNames.push("Student.jpg");
  g_ImageNames.push("Couple.jpg");
  g_ImageNames.push("Freshman.jpg");  
  g_ImageNames.push("Tourists.jpg");  
  g_ImageNames.push("Elderly.jpg");  
  g_ImageNames.push("Professionals.jpg");  
  g_ImageNames.push("Woman.jpg");  
  g_ImageNames.push("MotherDaughter.jpg");
    
  setTimeout("rotateImage();", 5000);
  
  var country = document.getElementById("country");
  var region = document.getElementById("region");
  
  if (country.selectedIndex > 0 && region.options.length == 0)
  {
   onCountryChanged();   
  }
  else
  {
   preprocessLabels();
   preprocessForm();
  } 

  if (document.getElementById("serviceProvidersStr").value == "")
   g_SelectedProviders = new Array();
  else
   g_SelectedProviders = document.getElementById("serviceProvidersStr").value.split("|"); 
   
  g_ServiceProviders = document.getElementById("availableProviders").value.split("|");
 }
 
 function preprocessLabels()
 {
  var country = document.getElementById("country");
  var countryName = country.options[country.selectedIndex].text;
  var regionLabel = document.getElementById("regionLabel");
  var postalCodeLabel = document.getElementById("postalCodeLabel");
  
  if (countryName == "United States")
  {
   regionLabel.innerText= "State:";
   postalCodeLabel.innerText = "Zip Code:";
  }
  else if (countryName == "Canada")
  {
   regionLabel.innerText = "Province:";
   postalCodeLabel.innerText = "Postal Code:";
  }
  else
  {
   regionLabel.innerText = "Area:";
   postalCodeLabel.innerText = "Postal Code:";
  } 
 }
 
 function rotateImage()
 {
  g_ImageIndex++;
  
  document.getElementById("diversityImage").src = "images/misc/" + g_ImageNames[g_ImageIndex % g_ImageNames.length];
  
  setTimeout("rotateImage();", 5000);
 }

 function onCountryChanged()
 {

  var country = document.getElementById("country");
  var btnSearch = document.getElementById("btnSearch");

  btnSearch.className = "fancyButton fancyButtonDisabled";
  btnSearch.disabled = true;

  if (country.selectedIndex == 0)
  {
   onRegionsCallBack(null);
   onProvidersCallBack(null);
  }
  else
  {
   var countryName = country.options[country.selectedIndex].text;
   
   request = HSLAppModelFacade.GetRegions(countryName, onRegionsCallBack, onTimeout);
   request = HSLAppModelFacade.GetCountryServiceProviders(countryName, onProvidersCallBack, onTimeout);
  }

  preprocessLabels();  
  populateDistance();
 }
 
 function onTimeout()
 {
  alert("Timeout!");
 }
 
 function populateDistance()
 {
  var country = document.getElementById("country");
  var distance = document.getElementById("distance");
  
  distance.options.length = 0;
  
  var proximity = new Array();
  var unit;
  
  proximity[0] = "-1";
  
  if (country.options[country.selectedIndex].text == "United States")
  {
   proximity[1] = "0.2";
   proximity[2] = "0.5";
   proximity[3] = "1.0";
   proximity[4] = "2.0";
   proximity[5] = "10";
   proximity[6] = "20";
   proximity[7] = "50";
   proximity[8] = "100";

   unit = "miles";
  }
  else
  {
   unit = "km";
  
   proximity[1] = "0.5";
   proximity[2] = "1.0";
   proximity[3] = "2.0";
   proximity[4] = "5.0";
   proximity[5] = "10";
   proximity[6] = "20";
   proximity[7] = "50";
   proximity[8] = "100";  
  }
  
  for (i = 0; i < proximity.length; i++)
  {
   var option = new Option();
   
   if (proximity[i] == "-1")
    option.text = "";
   else
    option.text = proximity[i] + " " + unit;
    
   option.value = proximity[i];
   distance.options[i] = option; 
  }
 }
 
 function onRegionsCallBack(regions)
 {
  var region = document.getElementById("region");

  region.options.length = 0;
 
  if (regions != null)
  {
   region.options[0] = new Option("");
   var index = 1;
 
   for (i = 0; i < regions.length; i++)
   {
    if (regions[i].RegionName != "")
    {
     region.options[index++] = new Option(regions[i].RegionName);
    }
   }
   
   var regionSelected = document.getElementById("regionSelected").value;

   if (regionSelected != "")
   {
    for (i = 0; i < region.options.length; i++)
    {
     if (region.options[i].text == regionSelected)
     {
      region.selectedIndex = i;
      break;
     }
    }
   }
  }

  preprocessForm();  
 }

 function onRegionProvidersCallBack(providers)
 {
  if (providers != null)
   onProvidersCallBack(providers);
  else
   preprocessForm();
 }

 function onProvidersCallBack(providers)
 {
  var serviceProviders = document.getElementById("serviceProviders");   
  serviceProviders.innerHTML = "";
  
  g_ServiceProviders = new Array();
  g_SelectedProviders = new Array();
 
  if (providers != null)
  {    
   var htmlArray = new Array(providers.length);  
    
   for (i = 0; i < providers.length; i++)
   {
    htmlArray[i] = '<input type="checkbox" onclick="onProviderCheckToggle('+i+');" />' + providers[i].ProviderName + '<br />\n';
    g_ServiceProviders.push(providers[i].ProviderName);
   }
   
   serviceProviders.innerHTML = htmlArray.join("");  
  } 
  
  preprocessForm();
 }
 
 function onProviderCheckToggle(providerIndex)
 {    
  if (g_ServiceProviders != null && providerIndex < g_ServiceProviders.length)
  {
   var addProvider = true;
   var providerName = g_ServiceProviders[providerIndex];
  
   for (i = 0; i < g_SelectedProviders.length; i++)
   {
    if (providerName == g_SelectedProviders[i])
    {
     g_SelectedProviders.splice(i, 1);
     addProvider = false;
     break;
    }
   }
  
   if (addProvider == true)
    g_SelectedProviders.push(providerName);
  }
 }
  
 function onRegionChanged()
 {
  var country = document.getElementById("country");
  var region = document.getElementById("region");
  var city = document.getElementById("city");
  var zip = document.getElementById("postalCode");  
  var btnSearch = document.getElementById("btnSearch");

  btnSearch.className = "fancyButton fancyButtonDisabled";
  btnSearch.disabled = true;
   
  if (country.selectedIndex > 0 && region.selectedIndex > 0)
  {
   request = HSLAppModelFacade.GetRegionServiceProviders(
                                country.options[country.selectedIndex].text,
                                region.options[region.selectedIndex].text,
                                onRegionProvidersCallBack,
                                onTimeout);
  }
  else if (country.selectedIndex > 0 && region.selectedIndex == 0)
  {
   request = HSLAppModelFacade.GetCountryServiceProviders(
                                country.options[country.selectedIndex].text,
                                onProvidersCallBack,
                                onTimeout);
  }
  
  document.getElementById("regionSelected").value = region.options[region.selectedIndex].text;
 }
 
 function onSearch()
 {
  var providers = document.getElementById("serviceProviders").childNodes; 
  var categories = document.getElementById("categories").childNodes;
  var region = document.getElementById("region");
  
  document.getElementById("serviceProvidersStr").value = g_SelectedProviders.join("|");
  document.getElementById("categoriesStr").value = getCheckedBoxesAsPipeDelimitedStr(categories);
  
  document.getElementById("startAtRow").value = "0";
    
  document.aspnetForm.submit();
 }
  
 function preprocessForm()
 {
    var btnSearch = document.getElementById("btnSearch");
    var country = document.getElementById("country");
    
    // disable if country is not specified
    if (country.selectedIndex == 0)
    {
     btnSearch.className = "fancyButton fancyButtonDisabled";
     btnSearch.disabled = true;
    }
    else
    {
     btnSearch.className = "fancyButton fancyButtonRest";
     btnSearch.disabled = false;
    }
 }
 
 function getCheckedBoxesAsPipeDelimitedStr(checkBoxes)
 {
   retArray = new Array();
  
   for (i = 0; i < checkBoxes.length; i++)
   {
    if (checkBoxes[i].checked)
    {
     retArray.push(checkBoxes[i].name);
     checkBoxes[i].removeAttribute("name");
    }
   }
   
   return retArray.join("|");
 }
 
 function setSearchMode(searchMode)
 {
    var basicSearch = document.getElementById("basicSearch");
    var advancedSearch = document.getElementById("advancedSearch");
    
    var divAreaCode = document.getElementById("divAreaCode");
    var categoriesDiv = document.getElementById("categoriesDiv");
    var providersDiv = document.getElementById("providersDiv");
    
    var searchType = document.getElementById("searchType");

    if (searchMode == "basic")
    {
        searchType.value = "basic";
    
        basicSearch.innerHTML = '<label style="color:#333333;">Basic Search</label>';
        advancedSearch.innerHTML = '<a href="#" title="Advanced Search" onclick="setSearchMode(' +"'advanced'"+');">Advanced Search</a>';
        
        divAreaCode.style.display = 'none';
        categoriesDiv.style.display = 'none';
        providersDiv.style.display = 'none';
    }
    else
    {
        searchType.value = "advanced";

        basicSearch.innerHTML = '<a href="#" title="Basic Search" onclick="setSearchMode(' + "'basic'" + ');">Basic Search</a>';
        advancedSearch.innerHTML = '<label style="color:#333333;">Advanced Search</label>';

        divAreaCode.style.display = 'block';
        categoriesDiv.style.display = 'block';
        providersDiv.style.display = 'block';
    }
 }