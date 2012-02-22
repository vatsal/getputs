//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections;


/// <summary>
/// Summary description for GUIVariables
/// </summary>
public class GUIVariables
{
    private static volatile GUIVariables instance;
    private static object syncRoot = new Object();

    private string gpFont;    

    private string lineBreak;
    private string htmlSpace;
    private string htmlTab;

    private string boldFontStart;
    private string boldFontEnd;

    private string redFontStart;
    private string redFontEnd;

    private string greenFontStart;
    private string greenFontEnd;

    private string blueFontStart;
    private string blueFontEnd;

    private string grayFontStart;
    private string grayFontEnd;

    private string whiteFontStart;
    private string whiteFontEnd;

    private string mediumFontStart;
    private string mediumFontEnd;

    private string smallFontStart;
    private string smallFontEnd;

    private string greenSpanStart;
    private string greenSpanEnd;

    private string redSpanStart;
    private string redSpanEnd;
    
    private string cDataStart;
    private string cDataEnd;

    private string seperator;
    private string itemSeperator;



    public string GPFont
    {
        get { return gpFont; }
        set { gpFont = value; }
    }   

    public string LineBreak
    {
        get { return lineBreak; }
        set { lineBreak = value; }
    }

    public string HTMLSpace
    {
        get { return htmlSpace; }
        set { htmlSpace = value; }
    }

    public string HTMLTab
    {
        get { return htmlTab; }
        set { htmlTab = value; }
    }

    public string BoldFontStart
    {
        get { return boldFontStart; }
        set { boldFontStart = value; }
    }

    public string BoldFontEnd
    {
        get { return boldFontEnd; }
        set { boldFontEnd = value; }
    }

    public string RedFontStart
    {
        get { return redFontStart; }
        set { redFontStart = value; }
    }

    public string RedFontEnd
    {
        get { return redFontEnd; }
        set { redFontEnd = value; }
    }

    public string GreenFontStart
    {
        get { return greenFontStart; }
        set { greenFontStart = value; }
    }

    public string GreenFontEnd
    {
        get { return greenFontEnd; }
        set { greenFontEnd = value; }
    }

    public string BlueFontStart
    {
        get { return blueFontStart; }
        set { blueFontStart = value; }
    }

    public string BlueFontEnd
    {
        get { return blueFontEnd; }
        set { blueFontEnd = value; }
    }


    public string GrayFontStart
    {
        get { return grayFontStart; }
        set { grayFontStart = value; }
    }

    public string GrayFontEnd
    {
        get { return grayFontEnd; }
        set { grayFontEnd = value; }
    }
       

    public string WhiteFontStart
    {
        get { return whiteFontStart; }
        set { whiteFontStart = value; }
    }

    public string WhiteFontEnd
    {
        get { return whiteFontEnd; }
        set { whiteFontEnd = value; }
    }

    public string MediumFontStart
    {
        get { return mediumFontStart; }
        set { mediumFontStart = value; }
    }

    public string MediumFontEnd
    {
        get { return mediumFontEnd; }
        set { mediumFontEnd = value; }
    }

    public string SmallFontStart
    {
        get { return smallFontStart; }
        set { smallFontStart = value; }
    }

    public string SmallFontEnd
    {
        get { return smallFontEnd; }
        set { smallFontEnd = value; }
    }

    public string GreenSpanStart
    {
        get { return greenSpanStart; }
        set { greenSpanStart = value; }
    }

    public string GreenSpanEnd
    {
        get { return greenSpanEnd; }
        set { greenSpanEnd = value; }
    }

    public string RedSpanStart
    {
        get { return redSpanStart; }
        set { redSpanStart = value; }
    }

    public string RedSpanEnd
    {
        get { return redSpanEnd; }
        set { redSpanEnd = value; }
    }

    public string CDataStart
    {
        get { return cDataStart; }
        set { cDataStart = value; }
    }

    public string CDataEnd
    {
        get { return cDataEnd; }
        set { cDataEnd = value; }
    }

    public string Seperator
    {
        get { return seperator; }
        set { seperator = value; } 
    }

    public string ItemSeperator
    {
        get { return itemSeperator; }
        set { itemSeperator = value; }
    }




	public GUIVariables()
	{
        LoadGUIVariables();
	}

    /// <summary>
    /// Initialize a new instance of the GUIVariables class, If required.
    /// </summary>
    public static GUIVariables Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new GUIVariables();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Load the GUI Variables like RGB Colored Fonts, Bold Font and LineBreaks.
    /// </summary>
    private void LoadGUIVariables()
    {
        gpFont = "<b><font color=red> getputs </font>!</b>";         

        lineBreak = "<br />";
        htmlSpace = "&nbsp;";
        htmlTab = "&nbsp;&nbsp;&nbsp;&nbsp;";                                   

        boldFontStart = "<b>";
        boldFontEnd = "</b>";

        redFontStart = "<font color=red>";
        redFontEnd = "</font>";

        greenFontStart = "<font color=green>";
        greenFontEnd = "</font>";

        blueFontStart = "<font color=blue>";
        blueFontEnd = "</font>";

        grayFontStart = "<font color=gray>";
        grayFontEnd = "</font>";

        whiteFontStart = "<font color=white>";
        whiteFontEnd = "</font>";

        mediumFontStart = "<font size=2.5>";
        mediumFontEnd = "</font>";

        smallFontStart = "<font size=1.8>";
        smallFontEnd = "</font>";
                
        greenSpanStart = "<span style=\"background-color:Green;\">";
        greenSpanEnd = "</span>";

        redSpanStart = "<span style=\"background-color:Red;\">";
        redSpanEnd = "</span>";

        cDataStart = "<![CDATA[";
        cDataEnd = "]]>";

        seperator = grayFontStart + htmlSpace + "|" + htmlSpace + grayFontEnd;
        
        //  itemSeperator = grayFontStart + "-----------------------------------------------------------------------------------------------------" + grayFontEnd;
        //  itemSeperator = "<table width='100%' cellspacing=0 cellpadding=1><tr><td bgcolor=#ff6600></td></tr></table>";

        //  LightGray: #dfe4ee
        //  Orange: #ff6600
        itemSeperator = "<table width='100%' class='NoDesign'><tr><td bgcolor='#dfe4ee'></td></tr></table>";

        
        
    }


    /// <summary>
    /// Convert an image into a Clickable Image.
    /// </summary>
    /// <param name="imgSrc">Source of the Image.</param>
    /// <param name="link">The href Link.</param>
    /// <param name="linkTarget">The Target. Use: "_blank" for opening the image into a new window.</param>
    /// <param name="imgTitle">The Image Title to be shown when mouse is hovered over the Image.</param>
    /// <returns></returns>
    public string MakeClickableImage(string imgSrc, string link, string linkTarget, string imgTitle)
    {
        string toReturn = string.Empty;

        if (!string.IsNullOrEmpty(imgSrc) && !string.IsNullOrEmpty(link))
        {
            //  <a href='http://www.getputs.com' target='_blank' style='text-decoration:none; border-color:Gray;'></a>
            //  toReturn = "<a href='" + link + "' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='" + imgSrc + "' style='border: none;' ></img></a>";
            toReturn = "<a href='" + link + "' target='" + linkTarget + "' style='text-decoration:none; border-color:Gray;'><img src='" + imgSrc + "' style='border: none;' title='" + imgTitle + "'></img></a>";
        }
        return toReturn;
    }
    

}
