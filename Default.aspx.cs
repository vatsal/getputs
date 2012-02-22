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
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Collections;

public partial class _Default : System.Web.UI.Page 
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    Categories categories;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    Tagger tagger;
    ItemDisplayer itemDisplayer;
    
    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string UID = string.Empty;
    string seperator = " | ";
   

    int startItem = 0;  //  Default Value;   

    //  ProcessingEngine.Sort sort = ProcessingEngine.Sort.Hot;
    ProcessingEngine.Sort sort = ConfigurationManager.AppSettings["Default_Sort"] == "h" ? ProcessingEngine.Sort.Hot : ProcessingEngine.Sort.New;

    ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Flow;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Columns;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Categorized;

    private static int itemClicks = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        //dbOps = DBOperations.Instance;
        //links = Links.Instance;
        //general = General.Instance;
        //gui = GUIVariables.Instance;
        //categories = Categories.Instance;
        //engine = ProcessingEngine.Instance;
        //imageEngine = ImageEngine.Instance;
        //tagger =Tagger.Instance;

        dbOps = DBOperations.Instance;        
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        categories = Categories.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        tagger = Tagger.Instance;
        itemDisplayer = ItemDisplayer.Instance;
        
        seperator = gui.Seperator;
        //  seperator = gui.BlueFontStart + " | " + gui.BlueFontEnd;

        if (!string.IsNullOrEmpty(Request.QueryString["startItem"]))
        {
            bool isStartItemInt = int.TryParse(Request.QueryString["startItem"].Trim(), out startItem);
            if (!isStartItemInt)
            {
                startItem = 0;
            }

            if (startItem < 0)
            {
                startItem = 0;
            }
        }
        else
        {
            startItem = 0;
        }


        if (!string.IsNullOrEmpty(Request.QueryString["sort"]))
        {
            string sortStr = Convert.ToString(Request.QueryString["sort"]);
            sort = engine.GetSortType(sortStr);            
        }


        //if (!string.IsNullOrEmpty(Request.QueryString["layout"]))
        //{
        //    string layoutStr = Convert.ToString(Request.QueryString["layout"]);
        //    itemLayoutOptions = itemDisplayer.GetItemLayoutOptionsType(layoutStr);
        //}

        if (Request.Cookies["getputsLayoutCookie"] != null)
        {
            HttpCookie getputsLayoutCookie = Request.Cookies["getputsLayoutCookie"];
            itemLayoutOptions = itemDisplayer.GetItemLayoutOptionsType(dbOps.Decrypt(getputsLayoutCookie["layout"].ToString().Trim()));
        }


                

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(UID))
        {
            
        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists
        
                
        List<Item> itemList = engine.LoadItemDB(sort);        
        //  Using ASP.NET Controls.
        //  LoadItemTable(itemList);

        //  Using HTML
        ItemDisplayer.ShowItemsOptions itemOptions = ItemDisplayer.ShowItemsOptions.ShowUIDLink
            | ItemDisplayer.ShowItemsOptions.ShowTime
            | ItemDisplayer.ShowItemsOptions.ShowCategoryLink
            | ItemDisplayer.ShowItemsOptions.ShowSaveLink
            | ItemDisplayer.ShowItemsOptions.ShowEMailLink
            | ItemDisplayer.ShowItemsOptions.ShowCommentsLink
            | ItemDisplayer.ShowItemsOptions.ShowImage
            | ItemDisplayer.ShowItemsOptions.ShowRatings
            | ItemDisplayer.ShowItemsOptions.ShowTags
            | ItemDisplayer.ShowItemsOptions.CountClicks
            | ItemDisplayer.ShowItemsOptions.ShowPreviousNextLinks;
        
        string itemTable = itemDisplayer.LoadItemTable(itemList, itemOptions, itemLayoutOptions, startItem, UID, sort, links.FrontPageLink.Replace("~\\", ""));
        ItemDiv.InnerHtml = itemTable;


        //  Master Page ItemImage and ItemContent Carousels are invisble. Make them visible only for this page.
        Control MasterPageCarouselDiv = this.Master.FindControl("CarouselDiv");
        MasterPageCarouselDiv.Visible = true;
        Control MasterPageTopAboutTable = this.Master.FindControl("TopAboutTable");
        MasterPageTopAboutTable.Visible = true;




        //  List<Item> itemList = engine.LoadItemDB(ProcessingEngine.Sort.New);
        if (itemList != null && itemList.Count > 0)
        {
            LoadItemImageCarousel(itemList);
            LoadItemNewsCarousel(itemList);
        }


        //  Load Item Content Carousel JavaScript START
        //if (itemList != null && itemList.Count > 0)
        //{         
        //    LoadItemNewsCarousel(itemList);
        //}
        //  Load Item Content Carousel JavaScript END

    }







    /// <summary>
    /// Get the News Items (Most Recent Ones) which had a picture. Prepare an Image Carousel.
    /// </summary>
    /// <param name="itemList">List of Items</param>
    private void LoadItemImageCarousel(List<Item> itemList)
    {
        StringBuilder strScript = new StringBuilder();
        strScript.Append("var itemImageList = [");




        int maxImagesInCarousel = 50;
        for (int i = 0; i < itemList.Count && i < maxImagesInCarousel; i++)
        {
            string singleItemContent = string.Empty;

            Item item = itemList[i];
            List<string> itemImagesList = general.GetImages(item.Link, imageEngine.ItemImageRetrievalCount, item.IID);

            if (itemImagesList != null)
            {
                for (int iCount = 0; iCount < itemImagesList.Count; iCount++)
                {
                    item.Title = item.Title.Replace("'", "").Replace("\"", "").Replace("`", "");


                    string itemHref = string.Empty;
                    if (!string.IsNullOrEmpty(item.Link))   //  Link Submission.
                    {
                        itemHref = item.Link;
                    }
                    else    //  Text Submission.
                    {
                        itemHref = links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
                    }
                    //  getputs_A.master has no details about whether to count clicks or not.
                    //  Comment the next if/else block line if you do not want to count the clicks.
                    if (!string.IsNullOrEmpty(item.Link))   //  Link Submission.
                    {
                        itemHref = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + item.Link + engine.ItemLinkEndSeperator;
                    }
                    else    //  Text Submission.
                    {
                        itemHref = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator;
                    }


                    //  singleItemContent = gui.MakeClickableImage(itemImagesList[iCount], item.Link, "_blank", item.Title);
                    singleItemContent = gui.MakeClickableImage(itemImagesList[iCount], itemHref, "_blank", item.Title);


                    strScript.Append("\"" + singleItemContent + "\"");

                    if (i < itemList.Count - 1 && i < maxImagesInCarousel - 1)
                    {
                        strScript.Append(",");
                    }
                }
            }

        }


        strScript.Append("];");

        Page.ClientScript.RegisterClientScriptBlock(GetType(), "ItemImageCarouselJavaScript", strScript.ToString(), true);
    }


    /// <summary>
    /// Get the News Items (Most Recent Ones). Prepare a Text Carousel.
    /// </summary>
    /// <param name="itemList">List of Items</param>
    private void LoadItemNewsCarousel(List<Item> itemList)
    {
        StringBuilder strScript = new StringBuilder();
        strScript.Append("var itemContentList = [");


        int maxItemsInCarousel = 25;
        for (int i = 0; i < itemList.Count && i < maxItemsInCarousel; i++)
        {
            string singleItemContent = LoadNewsTickerItemJS(itemList[i]);
            strScript.Append("\"" + singleItemContent + "\"");

            if (i < itemList.Count - 1 && i < maxItemsInCarousel - 1)
            {
                strScript.Append(",");
            }
        }


        strScript.Append("];");

        Page.ClientScript.RegisterClientScriptBlock(GetType(), "ItemContentCarouselJavaScript", strScript.ToString(), true);
    }

    private string LoadNewsTickerItemJS(Item item)
    {
        StringBuilder newsTickerSB = new StringBuilder();

        item.Title = item.Title.Replace("'", "").Replace("\"", "").Replace("`", "");

        //  Have to Trim the Title for ItemContentCarousel, or else the Height of the Carousel Changes automatically.
        //if (item.Title.Length > 110)
        //{
        //    item.Title = item.Title.Substring(0, 110) + "...";
        //}


        string commentString = "discuss";
        string hashLink = string.Empty;

        if (!string.IsNullOrEmpty(item.Link))
        {
            string iLink = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + item.Link + engine.ItemLinkEndSeperator;
            newsTickerSB.Append("<a href='" + iLink + "' class='CSS_LinkItem' >" + item.Title + "</a>");

        }
        else
        {
            string iLink = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator;
            newsTickerSB.Append("<a href='" + iLink + "' class='CSS_TextItem' >" + item.Title + "</a>");
        }

        newsTickerSB.Append(gui.LineBreak);
        //  newsTickerSB.Append(gui.HTMLTab + gui.HTMLTab);


        if (!string.IsNullOrEmpty(item.Category))
        {
            if (imageEngine.IsIconsOn)
            {
                //string iconLocation = imageEngine.LoadIconLocation(item.Category).Replace("~","");
                //if (!string.IsNullOrEmpty(iconLocation))
                //{
                //    //  <img title="Bollywood" src="Images/Icons/film.png" align="absmiddle" style="border-width:0px;" /> 
                //    string categoryIconString = "<img title='" + item.Category + "' src='" + iconLocation + "' />";
                //    newsTickerSB.Append(categoryIconString);
                //}
            }

            //string categoryLink = links.CategoryPageLink.Replace("~", "") + "?category=" + item.Category;
            //newsTickerSB.Append("<a href='" + categoryLink + "' class='CSS_Category'>" + item.Category + "</a>");


            newsTickerSB.Append("<span style=' vertical-align:middle;  background-color: " + Categories.Instance.CategoryColorsHashTable[item.Category] + "' >"
                                + gui.HTMLSpace
                                + "<a class='CSS_Category' href='" + links.CategoryPageLink.Replace("~\\", "") + "?category=" + item.Category + "'>" + item.Category + "</a>"
                                + gui.HTMLSpace
                                + "</span>");

            newsTickerSB.Append(gui.Seperator);
        }


        string userDetailsLink = links.UserDetailsPageLink.Replace("~\\", "") + "?UID=" + item.UID;
        newsTickerSB.Append("<a href='" + userDetailsLink + "' class='CSS_Submitter'>" + item.UID + "</a>");

        newsTickerSB.Append(gui.Seperator);

        string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(item.Date) + gui.GrayFontEnd + gui.SmallFontEnd;
        newsTickerSB.Append("<span class='CSS_SubmissionFreshness'>" + dateFormatString + "</span>");

        newsTickerSB.Append(gui.Seperator);




        if (item.NComments > 0)
        {
            commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
        }
        string commentsLink = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink.Replace("~\\", "~\\\\") + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator;
        newsTickerSB.Append("<a href='" + commentsLink + "' class='CSS_Comments'>" + commentString + "</a>");

        return newsTickerSB.ToString();

    }














    #region Yahoo YUI Item News Carousel
    /*
    
     
    /// <summary>
    /// Get the News Items (Most Recent Ones). Prepare a Text Carousel.
    /// </summary>
    /// <param name="itemList">List of Items</param>
    private void LoadItemNewsCarousel(List<Item> itemList)
    {
        //  Load News Ticker JS START
                
        //if (sort == ProcessingEngine.Sort.Hot)
        //{
        //    NewsTickerLabel.Text = gui.GrayFontStart + "Latest News" + gui.GrayFontEnd;
        //    itemList = engine.SortItems(itemList, ProcessingEngine.Sort.New);
        //}
        //else
        //{
        //    NewsTickerLabel.Text = gui.GrayFontStart + "Hottest News" + gui.GrayFontEnd;
        //    itemList = engine.SortItems(itemList, ProcessingEngine.Sort.Hot);
        //}

        StringBuilder strScript = new StringBuilder();
        strScript.Append("var contentList = [");
        int maxNewsInTicker = 15;
        for (int i = 0; i < itemList.Count && i < maxNewsInTicker; i++)
        {
            string singleItemContent = LoadNewsTickerItemJS(itemList[i]);

            strScript.Append("\"" + singleItemContent + "\"");            
            //  strScript.Append("'" + content[i] + "'");
            if (i < itemList.Count - 1 && i < maxNewsInTicker - 1)
            {
                strScript.Append(",");
            }
        }
        strScript.Append("];");

        ClientScript.RegisterClientScriptBlock(GetType(), "NewsTickerContentLoaderJS", strScript.ToString(), true);


        //  Load News Ticker JS END
    }


    private string LoadNewsTickerItemJS(Item item)
    {
        StringBuilder newsTickerSB = new StringBuilder();

        item.Title = item.Title.Replace("'", "").Replace("\"","");
        
        string commentString = "discuss";
        string hashLink = string.Empty;

        if (!string.IsNullOrEmpty(item.Link))
        {
            string iLink = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + item.Link + engine.ItemLinkEndSeperator;
            newsTickerSB.Append("<a href='" + iLink + "' class='CSS_LinkItem'>" + item.Title + "</a>");
        }
        else
        {
            string iLink = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator;
            newsTickerSB.Append("<a href='" + iLink + "' class='CSS_TextItem'>" + item.Title + "</a>");
        }

        newsTickerSB.Append(gui.LineBreak);

        string userDetailsLink = links.UserDetailsPageLink.Replace("~\\", "") + "?UID=" + item.UID;
        newsTickerSB.Append("<a href='" + userDetailsLink + "' class='CSS_Submitter'>" + item.UID + "</a>");

        newsTickerSB.Append(gui.Seperator);

        string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(item.Date) + gui.GrayFontEnd + gui.SmallFontEnd;
        newsTickerSB.Append("<span class='CSS_SubmissionFreshness'>" + dateFormatString + "</span>");

        newsTickerSB.Append(gui.Seperator);


        if (!string.IsNullOrEmpty(item.Category))
        {
            if (imageEngine.IsIconsOn)
            {
                //string iconLocation = imageEngine.LoadIconLocation(item.Category).Replace("~","");
                //if (!string.IsNullOrEmpty(iconLocation))
                //{
                //    //  <img title="Bollywood" src="Images/Icons/film.png" align="absmiddle" style="border-width:0px;" /> 
                //    string categoryIconString = "<img title='" + item.Category + "' src='" + iconLocation + "' />";
                //    newsTickerSB.Append(categoryIconString);
                //}
            }

            string categoryLink = links.CategoryPageLink.Replace("~", "") + "?category=" + item.Category;
            newsTickerSB.Append("<a href='" + categoryLink + "' class='CSS_Category'>" + item.Category + "</a>");

            newsTickerSB.Append(gui.Seperator);
        }

        if (item.NComments > 0)
        {
            commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
        }
        string commentsLink = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink.Replace("~\\", "~\\\\") + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator;
        newsTickerSB.Append("<a href='" + commentsLink + "' class='CSS_Comments'>" + commentString + "</a>");

        return newsTickerSB.ToString();

    }
    
    */

    #endregion Yahoo YUI Item News Carousel





    //  The Table getputs.item has the following schema:
    //  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]

    //  The Table getputs.user has the following schema:
    //  getputs.item [UID, Password, Date, EMail, About, Admin, Points]




    //  //  This uses LinkButtons.

    //private void LoadItemTable(List<Item> itemList)
    //{
    //    string commentString = "discuss";

    //    //  for (int i = 0; i < itemList.Count; i++)
    //    for(int i=startItem; i < itemList.Count && i<startItem + engine.ItemsPerPage; i++)
    //    {
    //        commentString = "discuss";

    //        Item item = itemList[i];

    //        if (!string.IsNullOrEmpty(item.Link))
    //        {
    //            LinkButton linkItemLB = new LinkButton();
    //            linkItemLB.Click += new EventHandler(linkItemLB_Click);
    //            linkItemLB.CssClass = "CSS_LinkItem";

    //            linkItemLB.Text = item.Title;
    //            //  linkItemLB.ID = item.IID.ToString();

    //            linkItemLB.Attributes.Add("IID", item.IID.ToString());
    //            linkItemLB.Attributes.Add("link", item.Link);
                                
    //            linkItemLB.Attributes.Add("onmouseover", "window.status='" + item.Link + "'; return true;");
    //            linkItemLB.Attributes.Add("onmouseout", "window.status=''; return true;");

    //            //linkItemLB.Attributes.Add("target", "_blank");
    //            //linkItemLB.Attributes.Add("href", item.Link);


    //            ItemDiv.Controls.Add(linkItemLB);

                
               


    //            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(item.Link);
    //            string website = webreq.RequestUri.Host.Replace("www.","");

    //            Label websiteLabel = new Label();
    //            websiteLabel.Text = "  " + "[" + website + "]";
    //            websiteLabel.CssClass = "CSS_ItemDomain";
                
    //            ItemDiv.Controls.Add(websiteLabel);

    //        }
    //        else
    //        {                
    //            LinkButton textItemLB = new LinkButton();
    //            textItemLB.Click += new EventHandler(textItemLB_Click);
    //            textItemLB.Text = item.Title;
                
    //            textItemLB.CssClass = "CSS_TextItem";

    //            textItemLB.Attributes.Add("IID", item.IID.ToString());
    //            //  textItemLB.ID = item.IID.ToString();
                
    //            ItemDiv.Controls.Add(textItemLB);
    //        }


    //        //  Add an icon to open the link in a new window/tab.                
    //        if (imageEngine.isIconsOn)
    //        {
    //            string iconLocation = imageEngine.LoadStaticIconLocation("NewWindow");
    //            if (!string.IsNullOrEmpty(iconLocation))
    //            {
    //                ItemDiv.Controls.Add(new LiteralControl(" "));

    //                iconLocation = iconLocation.Replace("~\\", "");

    //                //  <a target='_blank' href=http://getputs.com><img border= '0' src='images/ext.gif' alt='Open this page in a new window'/></a> 
    //                //  string newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
    //                string newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

    //                //  string newWindow = "<a target='_blank' text-decoration='none' href='" + item.Link + "'><img border= '0' src='Images/Icons/tab_go.png' alt='New Window'/></a> ";
    //                LiteralControl NewWindowLiteral = new LiteralControl(newWindow);
    //                ItemDiv.Controls.Add(NewWindowLiteral);
    //            }
    //        }    


    //        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

    //        LinkButton userLB = new LinkButton();
    //        userLB.Click += new EventHandler(userLB_Click);
    //        userLB.Text = item.UID;
    //        userLB.Font.Underline = false;
    //        userLB.CssClass = "CSS_Submitter";
                        
    //        ItemDiv.Controls.Add(userLB);
                        
    //        ItemDiv.Controls.Add(new LiteralControl(seperator));

    //        string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(item.Date) + gui.GrayFontEnd + gui.SmallFontEnd;
    //        Label dateLabel = new Label();
    //        dateLabel.Text = dateFormatString;
    //        dateLabel.CssClass = "CSS_SubmissionFreshness";
            
    //        ItemDiv.Controls.Add(dateLabel);

    //        ItemDiv.Controls.Add(new LiteralControl(seperator));

    //        if (!string.IsNullOrEmpty(item.Category))
    //        {
    //            if (imageEngine.isIconsOn)
    //            {
    //                string iconLocation = imageEngine.LoadIconLocation(item.Category);
    //                if (!string.IsNullOrEmpty(iconLocation))
    //                {
    //                    System.Web.UI.WebControls.Image icon = new System.Web.UI.WebControls.Image();
    //                    icon.ImageUrl = iconLocation;
    //                    icon.ImageAlign = ImageAlign.AbsMiddle;
    //                    //  icon.ImageUrl = Request.ApplicationPath + iconLocation;
    //                    icon.ToolTip = item.Category;                       
    //                    ItemDiv.Controls.Add(icon);
    //                    ItemDiv.Controls.Add(new LiteralControl(" "));
    //                }
    //            }     

    //            LinkButton categoryLB = new LinkButton();
    //            categoryLB.Click += new EventHandler(categoryLB_Click);
    //            categoryLB.Text = item.Category;
    //            categoryLB.CssClass = "CSS_Category";
               
    //            categoryLB.Attributes.Add("category", item.Category);

    //            ItemDiv.Controls.Add(categoryLB);

    //            ItemDiv.Controls.Add(new LiteralControl(seperator));
    //        }

    //        if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
    //        {
    //            List<int> savedItems = general.GetSavedItemIID(UID);
                
    //            LinkButton saveLB = new LinkButton();
    //            saveLB.Click += new EventHandler(saveLB_Click);
    //            saveLB.Text = "save";
    //            saveLB.CssClass = "CSS_Save";
                
    //            saveLB.Attributes.Add("IID", item.IID.ToString());

    //            if (savedItems != null && savedItems.Count > 0)
    //            {
    //                if (savedItems.Contains(item.IID))
    //                {
    //                    //  Do not show the Save Link.
    //                }
    //                else
    //                {
    //                    //  Show the Save Link.
    //                    ItemDiv.Controls.Add(saveLB);
    //                    ItemDiv.Controls.Add(new LiteralControl(seperator));
    //                }
    //            }
    //            else
    //            {
    //                //  Show the Save Link.
    //                ItemDiv.Controls.Add(saveLB);
    //                ItemDiv.Controls.Add(new LiteralControl(seperator));
    //            }                
                
    //        }


    //        if (engine.IsEMailOn)
    //        {
    //            string emailIconLocation = imageEngine.LoadStaticIconLocation("EMail");
    //            if (!string.IsNullOrEmpty(emailIconLocation))
    //            {
    //                System.Web.UI.WebControls.ImageButton emailIB = new ImageButton();
    //                emailIB.ImageUrl = emailIconLocation;
    //                emailIB.ToolTip = "EMail/Share this Item";
    //                emailIB.Click += new ImageClickEventHandler(emailIB_Click);
    //                emailIB.ImageAlign = ImageAlign.AbsMiddle;
    //                emailIB.Attributes.Add("IID", item.IID.ToString());

                    
    //                ItemDiv.Controls.Add(emailIB);
    //                ItemDiv.Controls.Add(new LiteralControl(seperator));
    //            }
    //        }


    //        if (engine.IsSpamReportingOn)
    //        {
    //            if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
    //            {
    //                string spamIconLocation = imageEngine.LoadStaticIconLocation("Spam");
    //                if (!string.IsNullOrEmpty(spamIconLocation))
    //                {
    //                    System.Web.UI.WebControls.ImageButton spamIB = new ImageButton();
    //                    spamIB.ImageUrl = spamIconLocation;
    //                    spamIB.ToolTip = "Flag/Report this item as Spam/Garbage.";
    //                    spamIB.Click += new ImageClickEventHandler(spamIB_Click);
    //                    spamIB.ImageAlign = ImageAlign.AbsMiddle;
                        
    //                    spamIB.Attributes.Add("IID", item.IID.ToString());
    //                    spamIB.Attributes.Add("Submitter_UID", item.UID);

    //                    if (!string.IsNullOrEmpty(item.Link))
    //                    {
    //                        spamIB.Attributes.Add("Link", item.Link);
    //                    }
    //                    else
    //                    {
    //                        spamIB.Attributes.Add("Link", links.ItemDetailsPageLink + "?IID=" + item.IID.ToString());                            
    //                    }

    //                    ItemDiv.Controls.Add(spamIB);
    //                    ItemDiv.Controls.Add(new LiteralControl(seperator));
    //                }
    //            }
    //        }
            
            
    //        LinkButton commentsLB = new LinkButton();
    //        commentsLB.Click += new EventHandler(commentsLB_Click);

    //        commentsLB.ID = "c" + item.IID.ToString();
    //        commentsLB.CssClass = "CSS_Comments";
            
    //        commentsLB.Attributes.Add("IID", item.IID.ToString());

    //        if (item.NComments > 0)
    //        {
    //            commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
    //        }
    //        commentsLB.Text = commentString;

    //        ItemDiv.Controls.Add(commentsLB);


    //        if (engine.IsDebug)
    //        {                
    //            string debugMessage = string.Empty;
    //            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

    //            if (engine.ClickCount.ContainsKey(item.IID))
    //            {
    //                debugMessage += "Dict Clicks: " + Convert.ToString(engine.ClickCount[item.IID]) + seperator;
    //            }

    //            string queryString = "SELECT Clicks, NSaved, NEMailed FROM item WHERE IID=" + item.IID.ToString() + ";";
    //            MySqlDataReader retList = dbOps.ExecuteReader(queryString);

    //            if (retList != null && retList.HasRows)
    //            {
    //                while (retList.Read())
    //                {
    //                    debugMessage += "DB Clicks: " + Convert.ToString(retList["Clicks"]) + seperator
    //                        + "NSaved: " + Convert.ToString(retList["NSaved"]) + seperator
    //                        + "NEMailed: " + Convert.ToString(retList["NEMailed"]) + seperator;
    //                }
    //                retList.Close();
    //            }

    //            debugMessage += "Age: " + item.Age.ToString() + seperator;
    //            debugMessage += "Marks: " + item.Marks.ToString();

    //            Label debugLabel = new Label();
    //            debugLabel.Text = debugMessage;
    //            debugLabel.Font.Size = FontUnit.Small;
    //            debugLabel.ForeColor = System.Drawing.Color.Gray;
    //            ItemDiv.Controls.Add(debugLabel);
                                       
                
               
    //        }

    //        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
    //        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            
    //    }   //  itemList Loop Ends.


    //    //  Add the "Previous" Link if required.
    //    if (startItem - engine.ItemsPerPage >= 0)
    //    {            

    //        int previousItem = startItem - engine.ItemsPerPage;

    //        LinkButton previousPageLB = new LinkButton();
    //        previousPageLB.Click += new EventHandler(previousPageLB_Click);
    //        previousPageLB.Text = "Previous";
    //        previousPageLB.CssClass = "CSS_Next";

    //        previousPageLB.Attributes.Add("link", links.FrontPageLink + "?startItem=" + previousItem.ToString());

    //        ItemDiv.Controls.Add(previousPageLB);

         
    //    }

    //    //  Add the "Next" Link if required.
    //    if (startItem + engine.ItemsPerPage < itemList.Count)
    //    {
    //        if (startItem - engine.ItemsPerPage >= 0)
    //        {
    //            ItemDiv.Controls.Add(new LiteralControl(seperator));
    //        }

    //        int nextItem = startItem + engine.ItemsPerPage;

    //        LinkButton nextPageLB = new LinkButton();
    //        nextPageLB.Click += new EventHandler(nextPageLB_Click);
    //        nextPageLB.Text = "Next";
    //        nextPageLB.CssClass = "CSS_Next";

    //        nextPageLB.Attributes.Add("link", links.FrontPageLink + "?startItem=" + nextItem.ToString());

    //        ItemDiv.Controls.Add(nextPageLB);
    //    }

    //    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
    //    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
    //}





    //  //  This uses <a></a> Links
    private void LoadItemTable(List<Item> itemList)
    {        
        string commentString = "discuss";
                
        List<int> savedItems = null;
        List<int> ratedItems = null;
        if (!string.IsNullOrEmpty(UID))
        {
            //  Get the list of items that have been saved by the User only if the User is logged in.
            savedItems = general.GetSavedItemIID(UID);
            ratedItems = general.GetRatedItemsIID(UID);
        }

        //  for (int i = 0; i < itemList.Count; i++)
        for (int i = startItem; i < itemList.Count && i < startItem + engine.ItemsPerPage; i++)
        {
            string hashLink = string.Empty;
            commentString = "discuss";

            Item item = itemList[i];

            if (!string.IsNullOrEmpty(item.Link))
            {
                //LinkButton linkItemLB = new LinkButton();
                //linkItemLB.Click += new EventHandler(linkItemLB_Click);
                //linkItemLB.CssClass = "CSS_LinkItem";
                //linkItemLB.Text = item.Title;
                ////  linkItemLB.ID = item.IID.ToString();
                //linkItemLB.Attributes.Add("IID", item.IID.ToString());
                //linkItemLB.Attributes.Add("link", item.Link);

                //linkItemLB.Attributes.Add("onmouseover", "window.status='" + item.Link + "'; return true;");
                //linkItemLB.Attributes.Add("onmouseout", "window.status=''; return true;");

                ////linkItemLB.Attributes.Add("target", "_blank");
                ////linkItemLB.Attributes.Add("href", item.Link);
                
                //ItemDiv.Controls.Add(linkItemLB);
                

                //HyperLink linkItemHL = new HyperLink();                
                //linkItemHL.CssClass = "CSS_LinkItem";
                //linkItemHL.Text = item.Title;
                //linkItemHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + item.Link;                
                //ItemDiv.Controls.Add(linkItemHL);



                string linkItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + item.Link + "'>" + item.Title + "</a>";
                ItemDiv.Controls.Add(new LiteralControl(linkItemHL));



                Uri uri = new Uri(item.Link);
                string website = uri.Host;

                //HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(item.Link);
                //string website = webreq.RequestUri.Host; 

                HyperLink websiteLink = new HyperLink();
                websiteLink.Text = "  " + "[" + website.Replace("www.", "") + "]";
                websiteLink.CssClass = "CSS_ItemDomain";
                websiteLink.Target = "_blank";
                websiteLink.NavigateUrl = "http://" + website ;
                ItemDiv.Controls.Add(websiteLink);

                //Label websiteLabel = new Label();
                //websiteLabel.Text = "  " + "[" + website + "]";
                //websiteLabel.CssClass = "CSS_ItemDomain";

                

            }
            else
            {
                //LinkButton textItemLB = new LinkButton();
                //textItemLB.Click += new EventHandler(textItemLB_Click);
                //textItemLB.Text = item.Title;
                //textItemLB.CssClass = "CSS_TextItem";
                //textItemLB.Attributes.Add("IID", item.IID.ToString());
                ////  textItemLB.ID = item.IID.ToString();
                //  ItemDiv.Controls.Add(textItemLB);

                //HyperLink textItemHL = new HyperLink();                
                //textItemHL.Text = item.Title;
                //textItemHL.CssClass = "CSS_TextItem";                                
                //textItemHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
                //ItemDiv.Controls.Add(textItemHL);

                string textItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + item.Title + "</a>";
                ItemDiv.Controls.Add(new LiteralControl(textItemHL));

            }


            //  Add an icon to open the link in a new window/tab.                
            if (imageEngine.IsIconsOn)
            {
                string iconLocation = imageEngine.LoadStaticIconLocation("NewWindow");
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    ItemDiv.Controls.Add(new LiteralControl(" "));
                    iconLocation = iconLocation.Replace("~\\", "");
                    string newWindow = string.Empty;
                    if (!string.IsNullOrEmpty(item.Link))    //  Link Submission
                    {
                        //  Redirect via R.aspx                        
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

                        //  Redirect Directly, Bypass R.aspx
                        newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                    }
                    else    //  Text Submission
                    {
                        //  Redirect via R.aspx                        
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

                        //  Redirect Directly, Bypass R.aspx
                        newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + links.ItemDetailsPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                    }

                    LiteralControl NewWindowLiteral = new LiteralControl(newWindow);
                    ItemDiv.Controls.Add(NewWindowLiteral);
                }
            }


            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

            
            //HyperLink userHL = new HyperLink();
            //userHL.Text = item.UID;
            //userHL.Font.Underline = false;
            //userHL.CssClass = "CSS_Submitter";
            //userHL.NavigateUrl = links.UserDetailsPageLink + "?UID=" + item.UID;            
            //ItemDiv.Controls.Add(userHL);
            //ItemDiv.Controls.Add(new LiteralControl(seperator));

            string userHL = "<a class='CSS_Submitter' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.UserDetailsPageLink.Replace("~\\", "") + "?UID=" + item.UID + "' >" + item.UID + "</a>";
            ItemDiv.Controls.Add(new LiteralControl(userHL + seperator));



            string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(item.Date) + gui.GrayFontEnd + gui.SmallFontEnd;
            Label dateLabel = new Label();
            dateLabel.Text = dateFormatString;
            dateLabel.CssClass = "CSS_SubmissionFreshness";

            ItemDiv.Controls.Add(dateLabel);

            ItemDiv.Controls.Add(new LiteralControl(seperator));

            if (!string.IsNullOrEmpty(item.Category))
            {
                //if (imageEngine.IsIconsOn)
                //{
                //    string iconLocation = imageEngine.LoadIconLocation(item.Category);
                //    if (!string.IsNullOrEmpty(iconLocation))
                //    {
                //        System.Web.UI.WebControls.Image icon = new System.Web.UI.WebControls.Image();
                //        icon.ImageUrl = iconLocation;
                //        icon.ImageAlign = ImageAlign.AbsMiddle;
                //        //  icon.ImageUrl = Request.ApplicationPath + iconLocation;
                //        icon.ToolTip = item.Category;
                //        ItemDiv.Controls.Add(icon);
                //        ItemDiv.Controls.Add(new LiteralControl(" "));
                //    }
                //}

                ////LinkButton categoryLB = new LinkButton();
                ////categoryLB.Click += new EventHandler(categoryLB_Click);
                ////categoryLB.Text = item.Category;
                ////categoryLB.CssClass = "CSS_Category";
                ////categoryLB.Attributes.Add("category", item.Category);

                //HyperLink categoryHL = new HyperLink();
                //categoryHL.Text = item.Category;
                //categoryHL.CssClass = "CSS_Category";
                //categoryHL.NavigateUrl = links.CategoryPageLink + "?category=" + item.Category;
                
                //ItemDiv.Controls.Add(categoryHL);
                //ItemDiv.Controls.Add(new LiteralControl(seperator));

                

                string iconLocation = imageEngine.LoadIconLocation(item.Category);
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    string categoryHL = "<img border= '0' style='vertical-align:middle;' src='" + iconLocation.Replace("~\\", "") + "' alt=''/>" 
                        + "&nbsp;" 
                        + "<a class='CSS_Category' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.CategoryPageLink.Replace("~\\", "") + "?category=" + item.Category + "'>" + item.Category + "</a>";                    
                    ItemDiv.Controls.Add(new LiteralControl(categoryHL + seperator));
                }
            }

            if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
            {
                //  List<int> savedItems = general.GetSavedItemIID(UID);

                LinkButton saveLB = new LinkButton();
                saveLB.Click += new EventHandler(saveLB_Click);
                saveLB.Text = "save";
                saveLB.CssClass = "CSS_Save";
                saveLB.Attributes.Add("IID", item.IID.ToString());

                if (savedItems != null && savedItems.Count > 0)
                {
                    if (savedItems.Contains(item.IID))
                    {
                        //  Do not show the Save Link.
                    }
                    else
                    {
                        //  Show the Save Link.
                        ItemDiv.Controls.Add(saveLB);
                        ItemDiv.Controls.Add(new LiteralControl(seperator));
                    }
                }
                else
                {
                    //  Show the Save Link.
                    ItemDiv.Controls.Add(saveLB);
                    ItemDiv.Controls.Add(new LiteralControl(seperator));
                }
            }


            if (engine.IsEMailOn)
            {
                //string emailIconLocation = imageEngine.LoadStaticIconLocation("EMail");
                //if (!string.IsNullOrEmpty(emailIconLocation))
                //{
                //    System.Web.UI.WebControls.ImageButton emailIB = new ImageButton();
                //    emailIB.ImageUrl = emailIconLocation;
                //    emailIB.ToolTip = "EMail/Share this Item";
                //    emailIB.Click += new ImageClickEventHandler(emailIB_Click);
                //    emailIB.ImageAlign = ImageAlign.AbsMiddle;
                //    emailIB.Attributes.Add("IID", item.IID.ToString());


                //    ItemDiv.Controls.Add(emailIB);
                //    ItemDiv.Controls.Add(new LiteralControl(seperator));
                //}

                string emailIconLocation = imageEngine.LoadStaticIconLocation("EMail");
                if (!string.IsNullOrEmpty(emailIconLocation))
                {
                    string eMailHL = "<img title='E-Mail/Share this Item' border= '0' style='vertical-align:middle;' src='" + emailIconLocation.Replace("~\\", "") + "' alt='E-Mail'/>"
                        + "&nbsp;"
                        + "<a class='CSS_EMailLink' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.SendItemMailPageLink.Replace("~\\", "") + "?IID=" + item.IID + "'>E-Mail</a>";                    
                    ItemDiv.Controls.Add(new LiteralControl(eMailHL + seperator));
                }
            }

            


            if (engine.IsSpamReportingOn)
            {
                if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
                {
                    string spamIconLocation = imageEngine.LoadStaticIconLocation("Spam");
                    if (!string.IsNullOrEmpty(spamIconLocation))
                    {
                        System.Web.UI.WebControls.ImageButton spamIB = new ImageButton();
                        spamIB.ImageUrl = spamIconLocation;
                        spamIB.ToolTip = "Flag/Report this item as Spam/Garbage.";
                        spamIB.Click += new ImageClickEventHandler(spamIB_Click);
                        spamIB.ImageAlign = ImageAlign.AbsMiddle;

                        spamIB.Attributes.Add("IID", item.IID.ToString());
                        spamIB.Attributes.Add("Submitter_UID", item.UID);

                        if (!string.IsNullOrEmpty(item.Link))
                        {
                            spamIB.Attributes.Add("Link", item.Link);
                        }
                        else
                        {
                            spamIB.Attributes.Add("Link", links.ItemDetailsPageLink + "?IID=" + item.IID.ToString());
                        }

                        ItemDiv.Controls.Add(spamIB);
                        ItemDiv.Controls.Add(new LiteralControl(seperator));
                    }
                }
            }


            //if (item.NComments > 0)
            //{
            //    commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
            //}            
            //HyperLink commentsHL = new HyperLink();
            //commentsHL.CssClass = "CSS_Comments";            
            //commentsHL.Text = commentString;                        
            //commentsHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();                        
            //ItemDiv.Controls.Add(commentsHL);


            if (item.NComments > 0)
            {
                commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
            }
            string commentsHL = "<a class='CSS_Comments' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + commentString + "</a>";
            ItemDiv.Controls.Add(new LiteralControl(commentsHL));




            if (engine.IsShowRating)
            {
                /*
                doWhat = 0 => Call SubmitRating.aspx
                doWhat = 1 => message = "Please Login.";
                doWhat = 2 => message = "You cannot Rate the Item you Submitted.";
                doWhat = 3 => message = "You have already Rated this Item before. Thank You.";
                */


                //  For Two Decimal Places
                //  String.Format("{0:0.00}", 123.4567);    //  123.45

                //  string avgRating = item.AvgRating.ToString();
                string avgRating = String.Format("{0:0.00}", item.AvgRating);
                string nRated = item.NRated.ToString();
                string ratingID = item.IID.ToString();

                // Random ratingRandom = new Random();
                // string avgRating = "2"; //  avgRating is whatever is the current rating. To be retrieved from DB.
                // string avgRating = ratingRandom.Next(0, 5).ToString();
                
                string ratingDiv = string.Empty;
                string ratingLabelID = "ratingLabel_" + item.IID.ToString();
                string ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "[" + nRated + " ratings (Average: " + avgRating + ")]" + gui.SmallFontEnd + gui.GrayFontEnd;

                if (nRated.Equals("0")) //  Do not show any Text.
                {
                    ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + gui.SmallFontEnd + gui.GrayFontEnd;
                }
                if (nRated.Equals("1")) //  Show "rating" instead of "ratings"
                {
                    //  ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + nRated + " vote." + gui.SmallFontEnd + gui.GrayFontEnd;
                    ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "[" + nRated + " rating (Average: " + avgRating + ")]" + gui.SmallFontEnd + gui.GrayFontEnd;
                }

                string ratingLabel = "<span id='" + ratingLabelID + "' >" + ratingLabelText + "</span>";


                if (!string.IsNullOrEmpty(UID))
                {
                    //  Show the rating Div only if the currently logged in user is not the same as the submitter of the item.
                    if (UID != item.UID)
                    {
                        //  printf("<div class="rating" id="rating_%s">%s</div>", ratingId, rating);

                        ratingDiv = "<span class='rating' doWhat=0 id='rating_" + ratingID + "'>" + avgRating + "</span>";

                        if (ratedItems != null && ratedItems.Count > 0 && ratedItems.Contains(item.IID))
                        {
                            //  ratingDiv = "<span class='rating' onclick='alert(\"You have already Rated this Item before. Thank You.\");' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                            ratingDiv = "<span class='rating' doWhat=3 id='rating_" + ratingID + "'>" + avgRating + "</span>";
                        }                                             
                    }                    
                    //  Show the rating Div, but do not allow the User to Rate.
                    else
                    {
                        //  ratingDiv = "<span class='rating' onclick='alert(\"You cannot Rate the Item you Submitted.\");' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                        ratingDiv = "<span class='rating' doWhat=2 id='rating_" + ratingID + "'>" + avgRating + "</span>";
                    }
                }
                else
                {
                    //  The User has not Logged In.
                    //  Show the ratings, but do not allow the User to rate.

                    //  ratingDiv = "<span class='rating' onclick='alert(\"Please Login.\");' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                    ratingDiv = "<span class='rating' doWhat=1 id='rating_" + ratingID + "'>" + avgRating + "</span>";
                }

                ItemDiv.Controls.Add(new LiteralControl(seperator + ratingDiv + gui.HTMLSpace + ratingLabel));

                //else
                //{
                //    if (ratedItems != null && ratedItems.Count > 0 && ratedItems.Contains(item.IID))
                //    {
                //        //  string ratingDiv = "<span class='rating' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                //        string ratingLabelID = "ratingLabel_" + item.IID.ToString();
                //        string ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "Thank You for Rating." + gui.SmallFontEnd + gui.GrayFontEnd;
                //        string ratingLabel = "<span id='" + ratingLabelID + "'>" + ratingLabelText + "</span>";
                //        //  ItemDiv.Controls.Add(new LiteralControl(ratingDiv + gui.HTMLSpace + ratingLabel));

                //        ItemDiv.Controls.Add(new LiteralControl(gui.HTMLSpace + ratingLabel));
                //    }
                //}

                
                
            }

            
            //  //  Tag Production
            if (tagger.IsTaggingOn)
            {
                if (item.TagList != null && item.TagList.Count > 0)
                {
                    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
                    if (imageEngine.IsIconsOn)
                    {
                        string tagIconLocation = imageEngine.LoadStaticIconLocation("Tag");
                        if (!string.IsNullOrEmpty(tagIconLocation))
                        {
                            System.Web.UI.WebControls.Image tagIcon = new System.Web.UI.WebControls.Image();
                            tagIcon.ImageUrl = tagIconLocation;
                            tagIcon.ImageAlign = ImageAlign.AbsMiddle;
                            //  icon.ImageUrl = Request.ApplicationPath + iconLocation;
                            tagIcon.ToolTip = "Auto Generated Tags";
                            ItemDiv.Controls.Add(tagIcon);
                            ItemDiv.Controls.Add(new LiteralControl(" "));
                        }
                    }

                    string tagString = string.Empty;
                    for (int t = 0; t < item.TagList.Count; t++)
                    {
                        string tagLink = "<a href='" + links.AutoTagPageLink.Replace("~\\", "") + "?tag=" + item.TagList[t] + "' class='CSS_TagLinkItem'>" + item.TagList[t] + "</a>";
                        tagString = tagString + ", " + tagLink;
                    }
                    tagString = tagString.Trim().Trim(',');

                    ItemDiv.Controls.Add(new LiteralControl(gui.SmallFontStart + gui.GrayFontStart
                        + "Auto Generated Tags: " + tagString
                        + gui.GrayFontEnd + gui.SmallFontEnd));
                                       
                }
            }

            //  //  Tag Test
            //if (tagger.IsTaggingOn)
            //{
            //    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

            //    List<string> titleTokens = tagger.GetTokens(item.Title, false, false, false);
            //    List<string> impTags = tagger.GetImportantTagsList(titleTokens, tagger.GetPOSTags(titleTokens));
            //    //  string impTagString = general.ConvertListToCSV(impTags).Replace(",", ", ");

            //    if (impTags != null && impTags.Count > 0)
            //    {
            //        if (imageEngine.isIconsOn)
            //        {
            //            string tagIconLocation = imageEngine.LoadStaticIconLocation("Tag");
            //            if (!string.IsNullOrEmpty(tagIconLocation))
            //            {
            //                System.Web.UI.WebControls.Image tagIcon = new System.Web.UI.WebControls.Image();
            //                tagIcon.ImageUrl = tagIconLocation;
            //                tagIcon.ImageAlign = ImageAlign.AbsMiddle;
            //                //  icon.ImageUrl = Request.ApplicationPath + iconLocation;
            //                tagIcon.ToolTip = "Auto Generated Tags";
            //                ItemDiv.Controls.Add(tagIcon);
            //                ItemDiv.Controls.Add(new LiteralControl(" "));
            //            }
            //        }

            //        //ItemDiv.Controls.Add(new LiteralControl(gui.SmallFontStart + gui.GrayFontStart
            //        //    + "Automated Tags: " + gui.BlueFontStart + impTagString + gui.BlueFontEnd
            //        //    + gui.GrayFontEnd + gui.SmallFontEnd));

            //        string tagString = string.Empty;
            //        for (int t = 0; t < impTags.Count; t++)
            //        {
            //            string tagLink = "<a href='" + links.AutoTagPageLink.Replace("~\\", "") + "?tag=" + impTags[t] + "' class='CSS_TagLinkItem'>" + impTags[t] + "</a>";
            //            tagString = tagString + ", " + tagLink;
            //        }
            //        tagString = tagString.Trim().Trim(',');
                    
            //        ItemDiv.Controls.Add(new LiteralControl(gui.SmallFontStart + gui.GrayFontStart
            //            + "Auto Generated Tags: " + tagString
            //            + gui.GrayFontEnd + gui.SmallFontEnd));
            //    }                
            //}




            if (imageEngine.IsItemImageRetrievalOn)
            {
                List<string> itemImagesList = general.GetImages(item.Link, imageEngine.ItemImageRetrievalCount, item.IID);
                //  imageSB.AppendLine("<img src='" + imgSrc + "' class='CSS_ItemImage'>" + "</img>");

                StringBuilder imagesSB = new StringBuilder();
                if (itemImagesList != null)
                {
                    for (int iCount = 0; iCount < itemImagesList.Count; iCount++)
                    {
                        //  imagesSB.AppendLine("<img src='" + itemImagesList[iCount] + "' class='CSS_ItemImage'>" + "</img>");
                        //  imagesSB.AppendLine("<a href='" + item.Link + "' class=''><img src='" + itemImagesList[iCount] + "'>" + "</img></a>");
                        //  imagesSB.AppendLine("<img src='" + itemImagesList[iCount] + "' >" + "</img>");
                        
                        imagesSB.AppendLine(gui.MakeClickableImage(itemImagesList[iCount], item.Link, "_blank", String.Empty) + gui.HTMLTab);
                    }
                }

                string itemImages = string.Empty;
                if (!string.IsNullOrEmpty(imagesSB.ToString()))
                {
                    itemImages = imagesSB.ToString();
                }

                if (!string.IsNullOrEmpty(itemImages))
                {
                    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak + gui.LineBreak));
                    ItemDiv.Controls.Add(new LiteralControl(itemImages));

                }
            }
            
            if (engine.IsDebug)
            {
                string debugMessage = string.Empty;
                ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

                if (engine.ClickCount.ContainsKey(item.IID))
                {
                    debugMessage += "Dict Clicks: " + Convert.ToString(engine.ClickCount[item.IID]) + seperator;
                }

                string queryString = "SELECT Clicks, NSaved, NEMailed FROM item WHERE IID=" + item.IID.ToString() + ";";
                MySqlDataReader retList = dbOps.ExecuteReader(queryString);

                if (retList != null && retList.HasRows)
                {
                    while (retList.Read())
                    {
                        debugMessage += "DB Clicks: " + Convert.ToString(retList["Clicks"]) + seperator
                            + "NSaved: " + Convert.ToString(retList["NSaved"]) + seperator
                            + "NEMailed: " + Convert.ToString(retList["NEMailed"]) + seperator;
                    }
                    retList.Close();
                }

                debugMessage += "Age: " + item.Age.ToString() + seperator;
                debugMessage += "Marks: " + item.Marks.ToString();

                Label debugLabel = new Label();
                debugLabel.Text = debugMessage;
                debugLabel.Font.Size = FontUnit.Small;
                debugLabel.ForeColor = System.Drawing.Color.Gray;
                ItemDiv.Controls.Add(debugLabel);



            }

            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak + gui.LineBreak));
            ItemDiv.Controls.Add(new LiteralControl(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd));
            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

        }   //  itemList Loop Ends.


        string sortStr = sort == ProcessingEngine.Sort.New ? "n" : "h";

        //  Add the "Previous" Link if required.
        if (startItem - engine.ItemsPerPage >= 0)
        {

            int previousItem = startItem - engine.ItemsPerPage;

            //LinkButton previousPageLB = new LinkButton();
            //previousPageLB.Click += new EventHandler(previousPageLB_Click);
            //previousPageLB.Text = "Previous";
            //previousPageLB.CssClass = "CSS_Next";
            //previousPageLB.Attributes.Add("link", links.FrontPageLink + "?startItem=" + previousItem.ToString());

            HyperLink previousPageHL = new HyperLink();            
            previousPageHL.Text = "Previous";
            previousPageHL.CssClass = "CSS_Next";
            previousPageHL.NavigateUrl = links.FrontPageLink + "?sort=" + sortStr + "&startItem=" + previousItem.ToString();
            
            ItemDiv.Controls.Add(previousPageHL);
        }

        //  Add the "Next" Link if required.
        if (startItem + engine.ItemsPerPage < itemList.Count)
        {
            if (startItem - engine.ItemsPerPage >= 0)
            {
                ItemDiv.Controls.Add(new LiteralControl(seperator));
            }

            int nextItem = startItem + engine.ItemsPerPage;

            //LinkButton nextPageLB = new LinkButton();
            //nextPageLB.Click += new EventHandler(nextPageLB_Click);
            //nextPageLB.Text = "Next";
            //nextPageLB.CssClass = "CSS_Next";
            //nextPageLB.Attributes.Add("link", links.FrontPageLink + "?startItem=" + nextItem.ToString());

            HyperLink nextPageHL = new HyperLink();
            nextPageHL.Text = "Next";
            nextPageHL.CssClass = "CSS_Next";
            nextPageHL.NavigateUrl = links.FrontPageLink + "?sort=" + sortStr + "&startItem=" + nextItem.ToString();
            

            ItemDiv.Controls.Add(nextPageHL);
        }

        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));        
    }




    /*
    //  Vatsal Shah
    //  2008-01-09
    //  LoadItemTable has been moved to General.cs so that the same Method can be called by all the .aspx Pages.

    /// <summary>
    /// Show the Items. Configure the Display based on which page is displaying the items. Uses HTML
    /// </summary>
    public void LoadItemTable(List<Item> itemList, ShowItemsOptions itemOptions)
    {        
        StringBuilder showItems = new StringBuilder();
        
        string commentString = "discuss";

        List<int> savedItems = null;
        List<int> ratedItems = null;
        if (!string.IsNullOrEmpty(UID))
        {
            //  Get the list of items that have been saved by the User only if the User is logged in.
            savedItems = general.GetSavedItemIID(UID);
            ratedItems = general.GetRatedItemsIID(UID);
        }

        //  for (int i = 0; i < itemList.Count; i++)
        for (int i = startItem; i < itemList.Count && i < startItem + engine.ItemsPerPage; i++)
        {
            string hashLink = string.Empty;
            commentString = "discuss";

            Item item = itemList[i];


                      


            if (!string.IsNullOrEmpty(item.Link))
            {                               

                string linkItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + item.Link + "'>" + item.Title + "</a>";
                showItems.Append(linkItemHL);
                
                Uri uri = new Uri(item.Link);
                string website = uri.Host;

                string websiteHL = "<a class='CSS_ItemDomain' target='_blank' href='" + "http://" + website + "'>" + "[" + website.Replace("www.", "") + "]" + "</a>";
                showItems.Append(gui.HTMLSpace);
                showItems.Append(websiteHL);
            }
            else
            {
                string textItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + item.Title + "</a>";
                showItems.Append(textItemHL);
            }


            
            
            //  Add an icon to open the link in a new window/tab.                
            if (imageEngine.IsIconsOn)
            {
                string iconLocation = imageEngine.LoadStaticIconLocation("NewWindow");
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    showItems.Append(" ");
                    iconLocation = iconLocation.Replace("~\\", "");
                    string newWindow = string.Empty;
                    if (!string.IsNullOrEmpty(item.Link))    //  Link Submission
                    {
                        //  Redirect via R.aspx                        
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

                        //  Redirect Directly, Bypass R.aspx
                        newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                    }
                    else    //  Text Submission
                    {
                        //  Redirect via R.aspx                        
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

                        //  Redirect Directly, Bypass R.aspx
                        newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + links.ItemDetailsPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                    }

                    showItems.Append(newWindow);
                }
            }


            showItems.Append(gui.LineBreak);


            showItems.Append("<table class='NoDesign'>");
            

            if (imageEngine.IsItemImageRetrievalOn)
            {
                List<string> itemImagesList = general.GetImages(item.Link, imageEngine.ItemImageRetrievalCount, item.IID);
                //  imageSB.AppendLine("<img src='" + imgSrc + "' class='CSS_ItemImage'>" + "</img>");

                StringBuilder imagesSB = new StringBuilder();
                if (itemImagesList != null)
                {
                    for (int iCount = 0; iCount < itemImagesList.Count; iCount++)
                    {
                        //  imagesSB.AppendLine("<img src='" + itemImagesList[iCount] + "' class='CSS_ItemImage'>" + "</img>");
                        //  imagesSB.AppendLine("<a href='" + item.Link + "' class=''><img src='" + itemImagesList[iCount] + "'>" + "</img></a>");
                        //  imagesSB.AppendLine("<img src='" + itemImagesList[iCount] + "' >" + "</img>");

                        imagesSB.AppendLine(gui.MakeClickableImage(itemImagesList[iCount], item.Link) + gui.HTMLTab);
                    }
                }

                string itemImages = string.Empty;
                if (!string.IsNullOrEmpty(imagesSB.ToString()))
                {
                    itemImages = imagesSB.ToString();
                }

                if (!string.IsNullOrEmpty(itemImages))
                {
                    //  showItems.Append(gui.LineBreak);
                    showItems.Append("<tr>");
                    showItems.Append("<td valign='middle'>");

                    showItems.Append(itemImages);

                    showItems.Append("</td>");

                }
            }

            
            
            showItems.Append("<td valign='middle'>");
            
            string userHL = "<a class='CSS_Submitter' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.UserDetailsPageLink.Replace("~\\", "") + "?UID=" + item.UID + "' >" + item.UID + "</a>";
            showItems.Append(userHL + seperator);

            string dateFormatString = "<span class='CSS_SubmissionFreshness'>" + gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(item.Date) + gui.GrayFontEnd + gui.SmallFontEnd + "</span>";
            showItems.Append(dateFormatString);

            showItems.Append(seperator);

            if (!string.IsNullOrEmpty(item.Category))
            {
                string iconLocation = imageEngine.LoadIconLocation(item.Category);
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    string categoryHL = "<img border= '0' style='vertical-align:middle;' src='" + iconLocation.Replace("~\\", "") + "' alt=''/>"
                        + "&nbsp;"
                        + "<a class='CSS_Category' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.CategoryPageLink.Replace("~\\", "") + "?category=" + item.Category + "'>" + item.Category + "</a>";
                    showItems.Append(categoryHL + seperator);
                }
            }

            if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
            {
                string saveHL = "<a class='CSS_Save' IID='" + item.IID + "' href='" + links.SavePageLink.Replace("~\\","") + "?IID=" + item.IID.ToString() + "'>save</a>";
                if (savedItems != null && savedItems.Count > 0)
                {
                    if (savedItems.Contains(item.IID))
                    {
                        //  Do not show the Save Link.
                    }
                    else
                    {
                        //  Show the Save Link.
                        showItems.Append(saveHL + seperator);
                    }
                }
                else
                {
                    //  Show the Save Link.                    
                    showItems.Append(saveHL + seperator);
                }
            }
            
            if (engine.IsEMailOn)
            {
                string emailIconLocation = imageEngine.LoadStaticIconLocation("EMail");
                if (!string.IsNullOrEmpty(emailIconLocation))
                {
                    string eMailHL = "<img title='E-Mail/Share this Item' border= '0' style='vertical-align:middle;' src='" + emailIconLocation.Replace("~\\", "") + "' alt='E-Mail'/>"
                        + "&nbsp;"
                        + "<a class='CSS_EMailLink' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.SendItemMailPageLink.Replace("~\\", "") + "?IID=" + item.IID + "'>E-Mail</a>";
                    showItems.Append(eMailHL + seperator);
                }
            }



            //  Have turned the Spam Link Off for now.
            //if (engine.IsSpamReportingOn)
            //{
            //    if (!string.IsNullOrEmpty(UID)) //  Show the Spam link only if the User is logged in.
            //    {
            //        string spamIconLocation = imageEngine.LoadStaticIconLocation("Spam");
            //        if (!string.IsNullOrEmpty(spamIconLocation))
            //        {
            //            System.Web.UI.WebControls.ImageButton spamIB = new ImageButton();
            //            spamIB.ImageUrl = spamIconLocation;
            //            spamIB.ToolTip = "Flag/Report this item as Spam/Garbage.";
            //            spamIB.Click += new ImageClickEventHandler(spamIB_Click);
            //            spamIB.ImageAlign = ImageAlign.AbsMiddle;

            //            spamIB.Attributes.Add("IID", item.IID.ToString());
            //            spamIB.Attributes.Add("Submitter_UID", item.UID);

            //            if (!string.IsNullOrEmpty(item.Link))
            //            {
            //                spamIB.Attributes.Add("Link", item.Link);
            //            }
            //            else
            //            {
            //                spamIB.Attributes.Add("Link", links.ItemDetailsPageLink + "?IID=" + item.IID.ToString());
            //            }

            //            ItemDiv.Controls.Add(spamIB);
            //            ItemDiv.Controls.Add(new LiteralControl(seperator));
            //        }
            //    }
            //}
            
            if (item.NComments > 0)
            {
                commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
            }
            string commentsHL = "<a class='CSS_Comments' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + commentString + "</a>";
            showItems.Append(commentsHL);

            if (engine.IsShowRating)
            {
                
                //doWhat = 0 => Call SubmitRating.aspx
                //doWhat = 1 => message = "Please Login.";
                //doWhat = 2 => message = "You cannot Rate the Item you Submitted.";
                //doWhat = 3 => message = "You have already Rated this Item before. Thank You.";
                


                //  For Two Decimal Places
                //  String.Format("{0:0.00}", 123.4567);    //  123.45

                //  string avgRating = item.AvgRating.ToString();
                string avgRating = String.Format("{0:0.00}", item.AvgRating);
                string nRated = item.NRated.ToString();
                string ratingID = item.IID.ToString();

                // Random ratingRandom = new Random();
                // string avgRating = "2"; //  avgRating is whatever is the current rating. To be retrieved from DB.
                // string avgRating = ratingRandom.Next(0, 5).ToString();

                string ratingDiv = string.Empty;
                string ratingLabelID = "ratingLabel_" + item.IID.ToString();
                string ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "[" + nRated + " ratings (Average: " + avgRating + ")]" + gui.SmallFontEnd + gui.GrayFontEnd;

                if (nRated.Equals("0")) //  Do not show any Text.
                {
                    ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + gui.SmallFontEnd + gui.GrayFontEnd;
                }
                if (nRated.Equals("1")) //  Show "rating" instead of "ratings"
                {
                    //  ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + nRated + " vote." + gui.SmallFontEnd + gui.GrayFontEnd;
                    ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "[" + nRated + " rating (Average: " + avgRating + ")]" + gui.SmallFontEnd + gui.GrayFontEnd;
                }

                string ratingLabel = "<span id='" + ratingLabelID + "' >" + ratingLabelText + "</span>";


                if (!string.IsNullOrEmpty(UID))
                {
                    //  Show the rating Div only if the currently logged in user is not the same as the submitter of the item.
                    if (UID != item.UID)
                    {
                        //  printf("<div class="rating" id="rating_%s">%s</div>", ratingId, rating);

                        ratingDiv = "<span class='rating' doWhat=0 id='rating_" + ratingID + "'>" + avgRating + "</span>";

                        if (ratedItems != null && ratedItems.Count > 0 && ratedItems.Contains(item.IID))
                        {
                            //  ratingDiv = "<span class='rating' onclick='alert(\"You have already Rated this Item before. Thank You.\");' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                            ratingDiv = "<span class='rating' doWhat=3 id='rating_" + ratingID + "'>" + avgRating + "</span>";
                        }
                    }
                    //  Show the rating Div, but do not allow the User to Rate.
                    else
                    {
                        //  ratingDiv = "<span class='rating' onclick='alert(\"You cannot Rate the Item you Submitted.\");' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                        ratingDiv = "<span class='rating' doWhat=2 id='rating_" + ratingID + "'>" + avgRating + "</span>";
                    }
                }
                else
                {
                    //  The User has not Logged In.
                    //  Show the ratings, but do not allow the User to rate.

                    //  ratingDiv = "<span class='rating' onclick='alert(\"Please Login.\");' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                    ratingDiv = "<span class='rating' doWhat=1 id='rating_" + ratingID + "'>" + avgRating + "</span>";
                }

                showItems.Append(gui.LineBreak + ratingDiv + gui.HTMLSpace + ratingLabel);

                //else
                //{
                //    if (ratedItems != null && ratedItems.Count > 0 && ratedItems.Contains(item.IID))
                //    {
                //        //  string ratingDiv = "<span class='rating' id='rating_" + ratingID + "'>" + avgRating + "</span>";
                //        string ratingLabelID = "ratingLabel_" + item.IID.ToString();
                //        string ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "Thank You for Rating." + gui.SmallFontEnd + gui.GrayFontEnd;
                //        string ratingLabel = "<span id='" + ratingLabelID + "'>" + ratingLabelText + "</span>";
                //        //  ItemDiv.Controls.Add(new LiteralControl(ratingDiv + gui.HTMLSpace + ratingLabel));

                //        ItemDiv.Controls.Add(new LiteralControl(gui.HTMLSpace + ratingLabel));
                //    }
                //}



            }


            //  //  Tag Production
            if (tagger.IsTaggingOn)
            {
                if (item.TagList != null && item.TagList.Count > 0)
                {
                    string tagIcon = string.Empty;

                    if (imageEngine.IsIconsOn)
                    {
                        string tagIconLocation = imageEngine.LoadStaticIconLocation("Tag");
                        if (!string.IsNullOrEmpty(tagIconLocation))
                        {
                            //System.Web.UI.WebControls.Image tagIcon = new System.Web.UI.WebControls.Image();
                            //tagIcon.ImageUrl = tagIconLocation;
                            //tagIcon.ImageAlign = ImageAlign.AbsMiddle;
                            ////  icon.ImageUrl = Request.ApplicationPath + iconLocation;
                            //tagIcon.ToolTip = "Auto Generated Tags";

                            //  <img title='Auto Generated Tags' border= '0' style='vertical-align:middle;' src='" + emailIconLocation.Replace("~\\", "") + "' alt='E-Mail'/>"


                            tagIcon = "<img title='Auto Generated Tags' border= '0' style='vertical-align:middle;' src='" + tagIconLocation.Replace("~\\", "") + "'></img>";

                        }
                    }

                    string tagString = string.Empty;
                    for (int t = 0; t < item.TagList.Count; t++)
                    {
                        string tagLink = "<a href='" + links.AutoTagPageLink.Replace("~\\", "") + "?tag=" + item.TagList[t] + "' class='CSS_TagLinkItem'>" + item.TagList[t] + "</a>";
                        tagString = tagString + ", " + tagLink;
                    }
                    tagString = tagString.Trim().Trim(',');


                    showItems.Append(gui.LineBreak
                        + tagIcon + gui.HTMLSpace + gui.SmallFontStart + gui.GrayFontStart + "Auto Generated Tags: " + tagString + gui.GrayFontEnd + gui.SmallFontEnd
                        + gui.LineBreak);

                }
                else
                {
                    showItems.Append(gui.LineBreak);
                }
            }

            //  //  Tag Test
            //if (tagger.IsTaggingOn)
            //{
            //    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

            //    List<string> titleTokens = tagger.GetTokens(item.Title, false, false, false);
            //    List<string> impTags = tagger.GetImportantTagsList(titleTokens, tagger.GetPOSTags(titleTokens));
            //    //  string impTagString = general.ConvertListToCSV(impTags).Replace(",", ", ");

            //    if (impTags != null && impTags.Count > 0)
            //    {
            //        if (imageEngine.isIconsOn)
            //        {
            //            string tagIconLocation = imageEngine.LoadStaticIconLocation("Tag");
            //            if (!string.IsNullOrEmpty(tagIconLocation))
            //            {
            //                System.Web.UI.WebControls.Image tagIcon = new System.Web.UI.WebControls.Image();
            //                tagIcon.ImageUrl = tagIconLocation;
            //                tagIcon.ImageAlign = ImageAlign.AbsMiddle;
            //                //  icon.ImageUrl = Request.ApplicationPath + iconLocation;
            //                tagIcon.ToolTip = "Auto Generated Tags";
            //                ItemDiv.Controls.Add(tagIcon);
            //                ItemDiv.Controls.Add(new LiteralControl(" "));
            //            }
            //        }

            //        //ItemDiv.Controls.Add(new LiteralControl(gui.SmallFontStart + gui.GrayFontStart
            //        //    + "Automated Tags: " + gui.BlueFontStart + impTagString + gui.BlueFontEnd
            //        //    + gui.GrayFontEnd + gui.SmallFontEnd));

            //        string tagString = string.Empty;
            //        for (int t = 0; t < impTags.Count; t++)
            //        {
            //            string tagLink = "<a href='" + links.AutoTagPageLink.Replace("~\\", "") + "?tag=" + impTags[t] + "' class='CSS_TagLinkItem'>" + impTags[t] + "</a>";
            //            tagString = tagString + ", " + tagLink;
            //        }
            //        tagString = tagString.Trim().Trim(',');

            //        ItemDiv.Controls.Add(new LiteralControl(gui.SmallFontStart + gui.GrayFontStart
            //            + "Auto Generated Tags: " + tagString
            //            + gui.GrayFontEnd + gui.SmallFontEnd));
            //    }                
            //}




            

            if (engine.IsDebug)
            {
                string debugMessage = string.Empty;
                showItems.Append(gui.LineBreak);

                if (engine.ClickCount.ContainsKey(item.IID))
                {
                    debugMessage += "Dict Clicks: " + Convert.ToString(engine.ClickCount[item.IID]) + seperator;
                }

                string queryString = "SELECT Clicks, NSaved, NEMailed FROM item WHERE IID=" + item.IID.ToString() + ";";
                MySqlDataReader retList = dbOps.ExecuteReader(queryString);

                if (retList != null && retList.HasRows)
                {
                    while (retList.Read())
                    {
                        debugMessage += "DB Clicks: " + Convert.ToString(retList["Clicks"]) + seperator
                            + "NSaved: " + Convert.ToString(retList["NSaved"]) + seperator
                            + "NEMailed: " + Convert.ToString(retList["NEMailed"]) + seperator;
                    }
                    retList.Close();
                }

                debugMessage += "Age: " + item.Age.ToString() + seperator;
                debugMessage += "Marks: " + item.Marks.ToString();

                showItems.Append(gui.GrayFontStart + gui.SmallFontStart + debugMessage + gui.SmallFontEnd + gui.GrayFontEnd);
                
            }


            
            showItems.Append("</td>");
            


            showItems.Append("</tr>");

            showItems.Append("</table>");


            
            //showItems.Append("<tr>");            
            //showItems.Append("<td>");

            ////  showItems.Append(gui.LineBreak);
            //showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
            ////  showItems.Append(gui.LineBreak);

            //showItems.Append("</td>");

            //showItems.Append("<td>");

            ////  showItems.Append(gui.LineBreak);
            //showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
            ////  showItems.Append(gui.LineBreak);

            //showItems.Append("</td>");            
            //showItems.Append("</tr>");
            

            showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
            showItems.Append(gui.LineBreak);
            

        }   //  itemList Loop Ends.



        //  showItems.Append("</table>");



        string sortStr = sort == ProcessingEngine.Sort.New ? "n" : "h";

        //  Add the "Previous" Link if required.
        if (startItem - engine.ItemsPerPage >= 0)
        {
            int previousItem = startItem - engine.ItemsPerPage;
                        
            //HyperLink previousPageHL = new HyperLink();
            //previousPageHL.Text = "Previous";
            //previousPageHL.CssClass = "CSS_Next";
            //previousPageHL.NavigateUrl = links.FrontPageLink + "?sort=" + sortStr + "&startItem=" + previousItem.ToString();

            string previousPageHL = "<a class='CSS_Next' href='" + links.FrontPageLink.Replace("~\\","") + "?sort=" + sortStr + "&startItem=" + previousItem.ToString() + "'>Previous</a>";
            showItems.Append(previousPageHL);
        }

        //  Add the "Next" Link if required.
        if (startItem + engine.ItemsPerPage < itemList.Count)
        {
            if (startItem - engine.ItemsPerPage >= 0)
            {
                showItems.Append(seperator);
            }

            int nextItem = startItem + engine.ItemsPerPage;
                        
            //HyperLink nextPageHL = new HyperLink();
            //nextPageHL.Text = "Next";
            //nextPageHL.CssClass = "CSS_Next";
            //nextPageHL.NavigateUrl = links.FrontPageLink + "?sort=" + sortStr + "&startItem=" + nextItem.ToString();

            string nextPageHL = "<a class='CSS_Next' href='" + links.FrontPageLink.Replace("~\\", "") + "?sort=" + sortStr + "&startItem=" + nextItem.ToString() + "'>Next</a>";
            showItems.Append(nextPageHL);
        }

        showItems.Append(gui.LineBreak + gui.LineBreak);
        ItemDiv.Controls.Add(new LiteralControl(showItems.ToString()));
    
    }

    /// <summary>
    /// enum for identifying the Processing Options for Displaying Items.
    /// </summary>
    [FlagsAttribute]
    public enum ShowItemsOptions
    {
        None = 0,
        ShowUIDLink = 1,
        ShowTime = 2,
        ShowCategoryLink = 4,
        ShowEMailLink = 8,
        ShowSpamLink = 16,
        ShowCommentsLink = 32,        
        ShowImage = 64,
        ShowTags = 128,
        CountClicks = 256,
        IsDebug = 512
    }

    */

    




    void linkItemLB_Click(object sender, EventArgs e)
    {
        LinkButton itemLB = (LinkButton)sender;

        string iidStr = itemLB.Attributes["IID"];
        //  string iidStr = itemLB.ID;
        
        int iid = int.Parse(iidStr);

        ////  Code for updating the ClickCount Dictionary.
        //if (engine.ClickCount.ContainsKey(iid))
        //{
        //    engine.ClickCount[iid] = engine.ClickCount[iid] + 1;
        //}
        //else
        //{
        //    engine.ClickCount.Add(iid, 1);
        //}

        engine.UpdateClickDataDictionary(iid, UID, general.GetIP(this.Request), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);


        string link = Convert.ToString(itemLB.Attributes["link"]);
        Response.Redirect(link, false);       

        //Response.Write("<script language='javascript'>"
        //                     + "window.open('" + link + "', '_blank');"
        //                     + "</script>");
                
    }

    void textItemLB_Click(object sender, EventArgs e)
    {           
        LinkButton itemLB = (LinkButton)sender;

        string iidStr = itemLB.Attributes["IID"];
        //  string iidStr = itemLB.ID;

        int iid = int.Parse(iidStr);
        
        //if (engine.ClickCount.ContainsKey(iid))
        //{
        //    engine.ClickCount[iid] = engine.ClickCount[iid] + 1;
        //}
        //else
        //{
        //    engine.ClickCount.Add(iid, 1);
        //}

        engine.UpdateClickDataDictionary(iid, UID, general.GetIP(this.Request), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);

        
        Response.Redirect(links.ItemDetailsPageLink + "?IID=" + iid.ToString(), false);
                
        //Response.Write("<script language='javascript'>"
        //                    + "window.open('" + links.ItemDetailsPageLink + "?IID=" + iid.ToString() + "', '_blank');"
        //                    + "</script>");

    }


    void userLB_Click(object sender, EventArgs e)
    {
        LinkButton userLB = (LinkButton)sender;
        string uid = userLB.Text;

        //  Response.Write("User: " + uid);
        Response.Redirect(links.UserDetailsPageLink + "?UID=" + uid, false);
    }

    void categoryLB_Click(object sender, EventArgs e)
    {        
        LinkButton categoryLB = (LinkButton)sender;

        Response.Redirect(links.CategoryPageLink + "?category=" + categoryLB.Attributes["category"], false);
        

    }

    void saveLB_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
        {
            LinkButton saveLB = (LinkButton)sender;                    
            saveLB.Text = "saved";
            saveLB.Enabled = false;
            saveLB.CssClass = "CSS_AfterSave";
            //saveLB.Font.Underline = false;
            //saveLB.Font.Size = FontUnit.Small;
            //saveLB.ForeColor = System.Drawing.Color.Gray;
            string IID = saveLB.Attributes["IID"];

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string queryString = "INSERT INTO saveditems VALUES ('" + UID + "' , " + IID + " , '" + date + "');";
            int retInt = dbOps.ExecuteNonQuery(queryString);

            //  Also update the getputs.item NSaved Field.
            queryString = "UPDATE item SET NSaved=NSaved+1 WHERE IID=" + IID + ";";
            retInt = dbOps.ExecuteNonQuery(queryString);

            Response.Redirect(Context.Request.Url.ToString());
        }

    }

    void emailIB_Click(object sender, EventArgs e)
    {
        ImageButton emailIB = (ImageButton)sender;
        string iidStr = Convert.ToString(emailIB.Attributes["IID"]);

        //  int iid = int.Parse(iidStr);

        Response.Redirect(links.SendItemMailPageLink + "?IID=" + iidStr, false);
    }


    void spamIB_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
        {
            ImageButton spamIB = (ImageButton)sender;
            
            string iidStr = Convert.ToString(spamIB.Attributes["IID"]);
            string reporter_UID = UID;
            string submitter_UID = Convert.ToString(spamIB.Attributes["Submitter_UID"]);            
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string link = Convert.ToString(spamIB.Attributes["Link"]);
            
            //  int iid = int.Parse(iidStr);

            string queryString = "SELECT Count(*) FROM itemspam WHERE IID = " + iidStr + " AND Reporter_UID = '" + reporter_UID + "';";
            int retVal = dbOps.ExecuteScalar(queryString);

            if (retVal == 0)
            {
                //  //  Allow any user to mark an item as spam.
                //  //  -1 => Undecided, 0 => Not Spam ,  1 => Spam
                //  queryString = "UPDATE item SET Spam = -1 WHERE IID = " + iidStr + ";";
                //  retVal = dbOps.ExecuteNonQuery(queryString);

                //  if (retVal >= 0)
                //  {
                //      queryString = "INSERT INTO itemspam (IID, Reporter_UID, Submitter_UID, Date, Link) VALUES (" + iidStr + ", '" + reporter_UID + "', '" + submitter_UID + "', DATE_FORMAT('" + date + "','%Y-%m-%d %k:%i:%S' ) , '" + link + "');";
                //      retVal = dbOps.ExecuteNonQuery(queryString);
                //  }
                                
                queryString = "INSERT INTO itemspam (IID, Reporter_UID, Submitter_UID, Date, Link) VALUES (" + iidStr + ", '" + reporter_UID + "', '" + submitter_UID + "', DATE_FORMAT('" + date + "','%Y-%m-%d %k:%i:%S' ) , '" + link + "');";
                retVal = dbOps.ExecuteNonQuery(queryString);
                if (retVal >= 0)
                {
                    //  Set an item as spam only if a minimum of X users set it as spam.
                    //  -1 => Undecided, 0 => Not Spam ,  1 => Spam
                    queryString = "SELECT Count(*) FROM itemspam WHERE IID = " + iidStr + ";";
                    retVal = dbOps.ExecuteScalar(queryString);
                    if (retVal >= 3)
                    {
                        //  Set an item as spam only if a minimum of X users set it as spam.
                        queryString = "UPDATE item SET Spam = -1 WHERE IID = " + iidStr + ";";
                        retVal = dbOps.ExecuteNonQuery(queryString);
                    }
                }

            }

            #region Send E-Mail to Admins
            if (engine.IsEMailOn)
            {
                string subject = "[getputs Spam Report]";
                string message = gui.LineBreak
                    + "Item ID: " + iidStr + gui.LineBreak
                    + "User who reported the item as Spam: " + reporter_UID + gui.LineBreak
                    + "User who submitted the spam item: " + submitter_UID + gui.LineBreak
                    + "Date and Time of reporting: " + date + gui.LineBreak
                    + "Link to be checked: " + link + gui.LineBreak;


                general.SendMail(general.TestMailID, general.Admin_EMailAddresses, subject, message);
            }
            #endregion Send E-Mail to Admins
        
            Response.Redirect(Context.Request.Url.ToString());         
        }
    }


    void commentsLB_Click(object sender, EventArgs e)
    {
        LinkButton commentsLB = (LinkButton)sender;
        string iidStr = Convert.ToString(commentsLB.Attributes["IID"]);

        int iid = int.Parse(iidStr);
        
        //if (engine.ClickCount.ContainsKey(iid))
        //{
        //    engine.ClickCount[iid] = engine.ClickCount[iid] + 1;
        //}
        //else
        //{
        //    engine.ClickCount.Add(iid, 1);
        //}

        engine.UpdateClickDataDictionary(iid, UID, general.GetIP(this.Request), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);

        Response.Redirect(links.ItemDetailsPageLink + "?IID=" + iid.ToString(), false);
    }


    void previousPageLB_Click(object sender, EventArgs e)
    {
        LinkButton previousPageLB = (LinkButton)sender;
        string link = Convert.ToString(previousPageLB.Attributes["link"]);
        Response.Redirect(link, false);

    }

    void nextPageLB_Click(object sender, EventArgs e)
    {
        LinkButton nextPageLB = (LinkButton)sender;
        string link = Convert.ToString(nextPageLB.Attributes["link"]);
        Response.Redirect(link, false);

    }



   


    #region OldCodeNotBeingUsedRightNow
    //private void LoadItemTable(List<Item> itemList)
    //{
    //    string commentString = "discuss";

    //    for (int i = 0; i < itemList.Count; i++)
    //    {
    //        TableRow itemRow = new TableRow();
    //        TableRow userRow = new TableRow();
    //        TableRow emptyRow = new TableRow();

    //        Item item = itemList[i];

    //        TableCell itemCell = new TableCell();

    //        TableCell userCell = new TableCell();
    //        TableCell seperatorCell = new TableCell();
    //        TableCell dateCell = new TableCell();
    //        TableCell commentsCell = new TableCell();

    //        TableCell emptyCell = new TableCell();


    //        if (!string.IsNullOrEmpty(item.Link))
    //        {
    //            //  itemLB.NavigateUrl = item.Link;               
    //            //  itemLB.CommandName = item.Link;

    //            LinkButton linkItemLB = new LinkButton();
    //            linkItemLB.Click += new EventHandler(linkItemLB_Click);
    //            linkItemLB.Text = item.Title;
    //            linkItemLB.ID = item.IID.ToString();
    //            linkItemLB.Font.Underline = false;
    //            linkItemLB.PostBackUrl = linkItemLB.ResolveUrl(item.Link);

    //            itemCell.Controls.Add(linkItemLB);

    //            //  ItemDiv.Controls.Add(linkItemLB);
    //        }
    //        else
    //        {
    //            //  itemLB. = links.ItemDetailsPageLink + "?IID=" + item.IID.ToString() ;
    //            LinkButton textItemLB = new LinkButton();
    //            textItemLB.Click += new EventHandler(textItemLB_Click);
    //            textItemLB.Text = item.Title;
    //            textItemLB.ID = item.IID.ToString();
    //            textItemLB.Font.Underline = false;

    //            itemCell.Controls.Add(textItemLB);
    //        }



    //        LinkButton userLB = new LinkButton();
    //        userLB.Click += new EventHandler(userLB_Click);
    //        userLB.Text = item.UID;
    //        userLB.Font.Underline = false;
    //        userLB.Font.Size = FontUnit.Small;
    //        userLB.ForeColor = System.Drawing.Color.OrangeRed;

    //        //  userLB.NavigateUrl = links.UserDetailsPageLink + "?UID=" + item.UID;

    //        userCell.Controls.Add(userLB);


    //        LiteralControl seperatorLiteralControl = new LiteralControl(seperator);
    //        seperatorCell.Controls.Add(seperatorLiteralControl);

    //        string dateFormatString = GetFormattedDate(item.Date);
    //        LiteralControl dateLiteralControl = new LiteralControl(dateFormatString);
    //        dateCell.Controls.Add(dateLiteralControl);


    //        LinkButton commentsLB = new LinkButton();
    //        commentsLB.Click += new EventHandler(commentsLB_Click);

    //        commentsLB.ID = item.UID;
    //        commentsLB.Font.Underline = false;
    //        commentsLB.Font.Size = FontUnit.Small;
    //        commentsLB.ForeColor = System.Drawing.Color.Green;

    //        if (item.NComments > 0)
    //        {
    //            commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
    //        }
    //        commentsLB.Text = item.Text;

    //        commentsCell.Controls.Add(commentsLB);

    //        emptyCell.Controls.Add(new LiteralControl(""));

    //        itemRow.Cells.Add(itemCell);
    //        userRow.Cells.Add(userCell);
    //        userRow.Cells.Add(seperatorCell);
    //        userRow.Cells.Add(dateCell);
    //        //  userRow.Cells.Add(seperatorCell);
    //        userRow.Cells.Add(commentsCell);            
    //        emptyRow.Cells.Add(emptyCell);

    //        ItemTable.Rows.Add(itemRow);
    //        ItemTable.Rows.Add(userRow);
    //        ItemTable.Rows.Add(emptyRow);


    //    }
    //}

    
    //private string LoadItems(List<Item> itemList)
    //{
    //    string commentString = "discuss";
        
    //    //  StringCollection sc = new StringCollection();
    //    StringBuilder sb = new StringBuilder();

    //    for (int i = 0; i < itemList.Count; i++)
    //    {        
    //        Item item = itemList[i];                       
            
    //        if (!string.IsNullOrEmpty(item.Link))
    //        {
    //             //string makeLink = gui.RedFontStart + ">>" + gui.RedFontEnd 
    //             //       + "<a href='" + item.Link + "' style=\"text-decoration:none; font-family:Verdana;\" target='_blank'>" + item.Title + "</a>";
    //             //   newsFeeds = newsFeeds + makeLink + "    " + gui.LineBreak;

    //            string makeItemLink = "<a href='" + item.Link + "' style=\"text-decoration:none; \" target='_blank'>" + item.Title + "</a>";
    //            sb.AppendLine(makeItemLink);
                
    //        }
    //        else
    //        {
    //            sb.Append(item.Title);
    //            sb.Append(gui.LineBreak);
    //            sb.Append(item.Text);
    //        }
    //        sb.AppendLine(gui.LineBreak);

    //        string makeUIDLink = "<a href='" + "/getputs/UserDetails.aspx?UID=" + item.UID + "' style=\"text-decoration:none; color:Orange; \" >" + item.UID + "</a>";
            
    //        if (item.NComments > 0)
    //        {
    //            commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
    //        }
    //        string makeCommentLink = "<a href='" + "/getputs/ItemDetails.aspx?IID=" + item.IID.ToString() + "' style=\"text-decoration:none; color:Green; \"  >" + commentString + "</a>";
            
            
    //        string dateString = general.GetFormattedDate(item.Date);

    //        sb.Append(gui.MediumFontStart);
                
    //            sb.Append(gui.RedFontStart);
    //            sb.Append(makeUIDLink);
    //            sb.Append(gui.RedFontEnd);
                    
    //            sb.Append(seperator);
                
    //            sb.Append(item.Category);
                
    //            sb.Append(seperator);
                
    //            sb.Append(dateString);
                
    //            sb.Append(seperator);

    //            sb.Append(gui.GreenFontStart);
    //                sb.Append(makeCommentLink);
    //            sb.Append(gui.GreenFontEnd);

    //        sb.Append(gui.MediumFontEnd);
            
    //        sb.AppendLine(gui.LineBreak);
    //        sb.AppendLine(gui.LineBreak); 
    //    }        

    //    return sb.ToString();
    //}
    #endregion OldCodeNotBeingUsedRightNow

}

