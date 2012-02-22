<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JCarousel.aspx.cs" Inherits="JCarousel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>jCarousel Examples</title>


    <!-- jCarousel core stylesheet -->
    <link rel="stylesheet" type="text/css" href="StyleSheets/jquery.jcarousel.css" />
    <!-- jCarousel skin stylesheet -->
    <link rel="stylesheet" type="text/css" href="StyleSheets/skins/ie7/skin.css" />   
    
    <script src="JS/jquery-1.2.3.pack.js" type="text/javascript"></script>    
    <script src="JS/jquery.jcarousel.pack.js" type="text/javascript"></script>

<script type="text/javascript">

    var itemImageList = [
    "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
    "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>",
     "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
    "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>",
    "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
    "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>",
    "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
    "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>"

    ];


    function ItemImageCarousel_ItemLoadCallback(carousel, state) {
        for (var i = carousel.first; i <= carousel.last; i++) {
            if (carousel.has(i)) {
                continue;
            }

            if (i > itemImageList.length) {
                break;
            }

            carousel.add(i, ItemImageCarousel_GetItemHTML(itemImageList[i - 1]));
        }
    };

    /**
    * Item html creation helper.
    */
    function ItemImageCarousel_GetItemHTML(item) {
        return item;
        //  return '<img src="' + item.url + '" width="75" height="75" alt="' + item.url + '" />';
    };


    //  itemLoadCallback: ItemImageCarousel_ItemLoadCallback
    //  itemLoadCallback: { onBeforeAnimation: ItemImageCarousel_ItemLoadCallback }

    jQuery(document).ready(function() {
        jQuery('#mycarousel').jcarousel({
            size: itemImageList.length,
            itemLoadCallback: ItemImageCarousel_ItemLoadCallback
        });
    });

</script>
</head>
<body>


    <form id="form1" runat="server">
        <ul id="mycarousel" class="jcarousel-skin-ie7">
            
        </ul>  
        
        
    </form>
  
  


</body>
</html>
