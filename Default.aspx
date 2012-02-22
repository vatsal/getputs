<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" Title="getputs.com - Social News Recommendation Engine." AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>


<asp:Content ID="Main" ContentPlaceHolderID="Main" runat="server">



<!-- Yahoo YUI Carousel JavaScript START -->

<!--
<script type="text/javascript" src="http://yui.yahooapis.com/2.5.2/build/utilities/utilities.js"></script>
<script type="text/javascript" src="http://yui.yahooapis.com/2.5.2/build/container/container_core-min.js"></script>
<script type="text/javascript" src="JS/carousel.js"></script>
-->

<!--
<script type="text/javascript">

//  Loaded dynamically via ASP.NET
//  var contentList = ["vatsal", "hello", "world", "shah"];


var lastRan = -1;

var fmtItem = function(innerText, url, title, index) 
{
    /*
    var innerHTML = 
        '<a id="dhtml-carousel-a-'+index+'" href="' + url + '">'
        + '<img id="dhtml-carousel-img-' + index + '" src="' + innerText + '" width="' + 75 + '" height="' + 75 + '"/>'
        + title 
        + '<\/a>';
    */
        
    var innerHTML = innerText;

    return innerHTML;
};


/* 
Custom inital load handler. 
Called when the carousel loads the initial set of data items. 
Specified to the carousel as the configuration parameter: loadInitHandler
*/
var loadInitialItems = function(type, args) 
{
    var start = args[0];
    var last = args[1]; 

    load(this, start, last);    
};

/*
Custom load next handler. 
Called when the carousel loads the next set of data items. 
Specified to the carousel as the configuration parameter: loadNextHandler
*/
var loadNextItems = function(type, args)
{    
    var start = args[0];
    var last = args[1]; 
    var alreadyCached = args[2];
    
    if(!alreadyCached) 
    {
        load(this, start, last);
    }
};

/*
Custom load previous handler. 
Called when the carousel loads the previous set of data items. 
Specified to the carousel as the configuration parameter: loadPrevHandler
*/
var loadPrevItems = function(type, args) 
{
    var start = args[0];
    var last = args[1]; 
    var alreadyCached = args[2];
    
    if(!alreadyCached) 
    {
        load(this, start, last);
    }
};

var load = function(carousel, start, last) 
{
    for(var i=start;i<=last;i++) 
    {
        var liItem = carousel.addItem(i, fmtItem(contentList[i-1], "#", "Number " + i, i));
        
    }
}

/*
Custom button state handler for enabling/disabling button state. 
Called when the carousel has determined that the previous button state should be changed.
Specified to the carousel as the configuration parameter: prevButtonStateHandler
*/
var handlePrevButtonState = function(type, args)
{
    var enabling = args[0];
    var leftImage = args[1];
    if(enabling)
    {
        leftImage.src = "Images/Icons/small-left-disabled.gif";    
    } 
    else 
    {
        leftImage.src = "Images/Icons/small-left-disabled.gif";
    }    
};

var carousel; // for ease of debugging; globals generally not a good idea

/*
You must create the carousel after the page is loaded since it is dependent on an HTML element (in this case 'dhtml-carousel'.) 
See the HTML code below.
*/
var pageLoad = function() 
{
    carousel = new YAHOO.extension.Carousel("dhtml-carousel", 
        {            
            numVisible:        1,
            animationSpeed:   0.3,
            scrollInc:         1,                
            prevElement:     "prev-arrow",
            nextElement:     "next-arrow",
            loadInitHandler:   loadInitialItems,
            loadNextHandler:   loadNextItems,
            loadPrevHandler:   loadPrevItems,
            prevButtonStateHandler:   handlePrevButtonState,
            autoPlay: 4000,
            size:15,
            wrap:true
        }
    );    
};

/* Illustrates stop autoplay */
var stopAutoPlay = function(e)
{
    YAHOO.util.Dom.get("status").innerHTML = "Auto Play Stopped!";
    carousel.stopAutoPlay();
};

/* Illustrates start autoplay */
var startAutoPlay = function(e) 
{
    YAHOO.util.Dom.get("status").innerHTML = "Auto Play Started!";
    carousel.startAutoPlay(2000);
};

YAHOO.util.Event.addListener(window, 'load', pageLoad);

// If you want to add handlers for mouse over, mouse out or other events, here is the pattern
//  YAHOO.util.Event.addListener("dhtml-carousel", "onmouseover", carousel.stopAutoPlay());
//  YAHOO.util.Event.addListener("dhtml-carousel", "onmouseout", carousel.startAutoPlay(2000));

</script>
-->
<!-- Yahoo YUI Carousel JavaScript END -->

                <table width="100%" class="NoDesign">

                <tr>
                <td>                
                
                
                <!-- Yahoo YUI Carousel HTML START -->                                             
                <%--
                    <div id="dhtml-carousel" class="carousel-component" onmouseover="carousel.stopAutoPlay();" onmouseout="carousel.startAutoPlay(4000);">
                        <asp:Label ID="NewsTickerLabel" runat="server"></asp:Label>
                        <div class="carousel-clip-region">
                            <ul class="carousel-list">

                            </ul>
                        </div>
                        <div style="vertical-align:bottom; text-align:right;">                            
                            <img id="prev-arrow" class="left-button-image" src="Images/Icons/small-left-disabled.gif" alt="Previous Button"/>
                            &nbsp;
                            <img id="next-arrow" class="right-button-image" src="Images/Icons/small-right-disabled.gif" alt="Next Button"/>
                        </div>
                    </div>
                --%>
                <!-- Yahoo YUI Carousel HTML END -->
                
                
                </td>
                </tr>
                
                <tr>
                <td style="vertical-align:top; text-align:right;">
                                    <!-- If activating Yahoo Carousel, Then uncomment the Next Line -->
                                    <%--<asp:Label ID="TestLabel" runat="server" Text="`"></asp:Label><br />--%>
                </td>
                </tr>                     
                
                <tr>
                <td>
                                    <div id="ItemDiv" runat="server">
                
                                    </div>
                </td>
                </tr>          
                </table>
    
    <!--
    <asp:Table ID="ItemTable" runat="server">    
    </asp:Table>
    -->
  
      
</asp:Content>
