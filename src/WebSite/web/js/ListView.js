function pageLoad()
 {
  var searchLink = document.getElementById("searchViewLink");
  var mapLink = document.getElementById("mapViewLink");
  
  if (mapLink != null)
   mapLink.href = "http://"+window.location.host+genPageURI("ListView.aspx")+getCriteriaAsLink('', '')+"&cmd=mapview";
   
  searchLink.href = "http://"+window.location.host+genPageURI("ListView.aspx")+getCriteriaAsLink('', '')+"&cmd=searchview";
 }
 
 function onSearchView()
 {
  document.getElementById("cmd").value = "searchview";
  
  document.aspnetForm.submit();
 }
 
 function onSelect(entry)
 {
  document.getElementById("cmd").value = "detailsview";
  document.getElementById("entry").value = entry;
  
  document.aspnetForm.submit(); 
 }
 
function SelectResultRow(entry)
{
    if (null != entry)
    {
        entry.className = "HslResults_SelectedRow";
    }
}

function DeSelectResultRow(entry)
{
    if (null != entry)
    {
        entry.className = "HslResults_Row";
    }
}
 
 function onSort(colname)
 {
  document.getElementById("cmd").value="sort"+colname;
  
  document.aspnetForm.submit();
 }
 
 function onMapView()
 {
  document.getElementById("cmd").value = "mapview";
  
  document.aspnetForm.submit();
 }
 
 function onPageResize(pgsize, entry)
 {
  document.getElementById("cmd").value = "pgresize";
  document.getElementById("startAtRow").value = entry;
  document.getElementById("maxRows").value = pgsize;
  
  document.aspnetForm.submit();
 }
 
 function onPageNextPrev(entry)
 {
  document.getElementById("cmd").value = "goto";
  document.getElementById("startAtRow").value = entry;
  
  document.aspnetForm.submit();
 }