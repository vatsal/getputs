
/*
function SwitchCSS(theme)
{   
	if(theme == 'blackwhite')
	{
		
	}
	else if(theme == 'whiteblue')
	{
	
	}    
}
*/

////


//////////////////////////////////////////
//  Code for Showing and Hiding ItemDetails in the 3rd Layout (Categorized).

function OpenClose(id, actionItem) 
{
    var openCloseLink = document.getElementById('OpenCloseSign_' + id);
    var detailText = document.getElementById('DetailText_' + id);

    detailText.style.display = actionItem;
    if (actionItem == 'block') 
    {
        //  openCloseLink.innerHTML = "<a href=\"javascript:;\" onclick=\"OpenClose(" + id + ", 'none')\" >[-]</a>";
        openCloseLink.innerHTML = "<a href=\"javascript:;\" style=\"text-decoration: none;\" onclick=\"OpenClose(" + id + ", 'none')\" ><img src='Images/Icons/Minus.gif' style=\"border: none;\" /></a>";
    }
    else 
    {
        //  openCloseLink.innerHTML = "<a href=\"javascript:;\" onclick=\"OpenClose(" + id + ", 'block')\" >[+]</a>";
        openCloseLink.innerHTML = "<a href=\"javascript:;\" style=\"text-decoration: none;\" onclick=\"OpenClose(" + id + ", 'block')\" ><img src='Images/Icons/Plus.gif' style=\"border: none;\" /></a>";
    }
}






/////////////////////////////////////////



var aryImages = new Array();

//  var host = "http://localhost/getputs/";
//aryImages[1] = host + "Images/Logo/getputs_logo_passion.png";
//aryImages[2] = host + "Images/Logo/getputs_logo_serene.png";
//aryImages[3] = host + "Images/Logo/getputs_logo_feminine.png";

aryImages[1] = "Images/Logo/getputs_logo_passion.png";
aryImages[2] = "Images/Logo/getputs_logo_serene.png";
aryImages[3] = "Images/Logo/getputs_logo_feminine.png";

//for (i=0; i < aryImages.length; i++)
//{
//	var preload = new Image();
//	preload.src = aryImages[i];
//}

function swap(imgIndex, imgTarget) 
{    
    //  document.forms[0]['getputs_logo'].src = aryImages[imgIndex]; 
    
    
    if(document.forms[0] != null)
    {   
        document.forms[0][imgTarget].src = aryImages[imgIndex];    
    }
    else
    {
        
    }
    
    
}





////


function setActiveStyleSheet(title) 
{
  var i, a, main;
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) 
  {
    if(a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title")) 
    {
      a.disabled = true;
      if(a.getAttribute("title") == title) a.disabled = false;   
         
    }
  }
}

function getActiveStyleSheet() {
  var i, a;
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if(a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title") && !a.disabled) return a.getAttribute("title");
  }
  return null;
}

function getPreferredStyleSheet() {
  var i, a;
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if(a.getAttribute("rel").indexOf("style") != -1
       && a.getAttribute("rel").indexOf("alt") == -1
       && a.getAttribute("title")
       ) return a.getAttribute("title");
  }
  return null;
}

function createCookie(name,value,days) {
  if (days) {
    var date = new Date();
    date.setTime(date.getTime()+(days*24*60*60*1000));
    var expires = "; expires="+date.toGMTString();
  }
  else expires = "";
  document.cookie = name+"="+value+expires+"; path=/";
}

function readCookie(name) {
  var nameEQ = name + "=";
  var ca = document.cookie.split(';');
  for(var i=0;i < ca.length;i++) {
    var c = ca[i];
    while (c.charAt(0)==' ') c = c.substring(1,c.length);
    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
  }
  return null;
}



function alertMessage(message) {
    alert(message);
}


window.onload = function(e) 
{
  var cookie = readCookie("style");
  var title = cookie ? cookie : getPreferredStyleSheet();
  setActiveStyleSheet(title);
  
  //  Set the Logo according to the Style Sheet Retrieved from the Cookie.
      if(title == 'passion')
      {
        swap(1,'getputs_logo');
      }
      if(title == 'serene')
      {
        swap(2,'getputs_logo');
      }
      if(title == 'feminine')
      {
        swap(3,'getputs_logo');
      }   
}

window.onunload = function(e) 
{
  var title = getActiveStyleSheet();
  createCookie("style", title, 365);
  
}

var cookie = readCookie("style");
var title = cookie ? cookie : getPreferredStyleSheet();

setActiveStyleSheet(title);
//  Set the Logo according to the Style Sheet Retrieved from the Cookie.
      if(title == 'passion')
      {
        //  alert("title: 1: " + title);
        swap(1,'getputs_logo');
      }
      if(title == 'serene')
      {
        //  alert("title: 2: " + title);
        swap(2,'getputs_logo');
      }
      if(title == 'feminine')
      {
        //  alert("title: 3: " + title);
        swap(3,'getputs_logo');
      }   
      








