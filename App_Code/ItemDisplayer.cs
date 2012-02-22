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

using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Net;
using System.Collections.Generic;
using System.Web.Mail;
using System.Security.Cryptography;

using System.Text;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

using gp.Files;
using System.Collections;

/// <summary>
/// Summary description for ItemDisplayer
/// </summary>
public class ItemDisplayer
{
    private static volatile ItemDisplayer instance;
    private static object syncRoot = new Object();

    Logger log = Logger.Instance;
    DBOperations dbOps = DBOperations.Instance;
    GUIVariables gui = GUIVariables.Instance;
    ImageEngine imageEngine = ImageEngine.Instance;
    Links links = Links.Instance;
    ProcessingEngine engine = ProcessingEngine.Instance;
    General general = General.Instance;
    Tagger tagger = Tagger.Instance;


    public ItemDisplayer()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    /// <summary>
    /// Initialize a new instance of the ItemDisplayer class, If required.
    /// </summary>
    public static ItemDisplayer Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ItemDisplayer();
                }
            }
            return instance;
        }
    }


    public ItemLayoutOptions GetItemLayoutOptionsType(string itemLayoutOptionsStr)
    {
        itemLayoutOptionsStr = itemLayoutOptionsStr.ToLower();

        ItemLayoutOptions itemLayoutOptions = ItemLayoutOptions.Flow;

        if (!string.IsNullOrEmpty(itemLayoutOptionsStr))
        {
            switch (itemLayoutOptionsStr)
            {
                case("flow"):
                    itemLayoutOptions = ItemLayoutOptions.Flow;
                    break;
                case("columns"):
                    itemLayoutOptions = ItemLayoutOptions.Columns;
                    break;
                case ("categorized"):
                    itemLayoutOptions = ItemLayoutOptions.Categorized;
                    break;
            }
        }

        return itemLayoutOptions;

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
        ShowSaveLink = 8,
        ShowEMailLink = 16,
        ShowSpamLink = 32,
        ShowCommentsLink = 64,
        ShowImage = 128,
        ShowRatings = 256,
        /// <summary>
        /// Show the Rating Analytics (HTML Table of Ratings / corresponding Frequencies)
        /// </summary>
        ShowRatingsAnalytics = 512,
        ShowTags = 1024,
        /// <summary>
        /// Show the Item Statistics. (NEMailed, NSaved, Clicks etc.)
        /// </summary>
        ShowItemStats = 2048,
        /// <summary>
        /// Route the Redirection through R.aspx which does the Click Counting.
        /// </summary>
        CountClicks = 4096,
        /// <summary>
        /// Show the Previous/Next Links.
        /// </summary>
        ShowPreviousNextLinks = 8192
    }

    /// <summary>
    /// enum for identifying the Layout Options for Displaying Items.
    /// </summary>
    [FlagsAttribute]
    public enum ItemLayoutOptions
    {
        /// <summary>
        /// The Default Layout.
        /// </summary>
        Flow = 0,
        /// <summary>
        /// Items divided into 2 Columns
        /// </summary>
        Columns = 1,
        /// <summary>
        /// Items divided into Categories (2 Columns)
        /// </summary>
        Categorized = 2
    }


    /// <summary>
    /// Show the Items. Configure the Display based on which page is displaying the items. Uses HTML
    /// </summary>
    /// <param name="itemList">A List of Objects of Type Item.</param>
    /// <param name="itemOptions">Enumeration of Item Display Options.</param>
    /// <param name="startItem">The starting Item Number</param>
    /// <param name="UID">The UID of the User. If UID = Empty Then the User has not logged in.</param>
    /// <param name="sort">The sort Type</param>
    /// <param name="currentPageLink">The Page where the current Previous/Next Links should redirect</param>
    /// <returns>A string containing the HTML to be displayed.</returns>
    public string LoadItemTable(List<Item> itemList, ShowItemsOptions itemOptions, ItemLayoutOptions itemLayoutOptions, int startItem, string UID, ProcessingEngine.Sort sort, string currentPageLink)
    {
        string seperator = gui.Seperator;
        StringBuilder showItems = new StringBuilder();
        
        //  string commentString = "discuss";
        string commentString = "comment";

        List<int> savedItems = null;
        List<int> ratedItems = null;
        if (!string.IsNullOrEmpty(UID))
        {
            //  Get the list of items that have been saved by the User only if the User is logged in.
            savedItems = general.GetSavedItemIID(UID);
            ratedItems = general.GetRatedItemsIID(UID);
        }

        string linkItemHL = string.Empty;
        string websiteHL = string.Empty;
        string textItemHL = string.Empty;
        string newWindow = string.Empty;    //  Add an icon to open the link in a new window/tab. 
        string itemImages = string.Empty;
        string userHL = string.Empty;
        string dateFormatString = string.Empty;
        string categoryHL = string.Empty;
        string saveHL = string.Empty;
        string eMailHL = string.Empty;
        string commentsHL = string.Empty;
        string ratingDiv = string.Empty;
        string ratingLabel = string.Empty;
        string tagIcon = string.Empty;
        string tagString = string.Empty;
        string debugMessage = string.Empty;
        string previousPageHL = string.Empty;
        string nextPageHL = string.Empty;

        string ratingAnalyticsHTMLString = string.Empty;


        //  for (int i = 0; i < itemList.Count; i++)
        for (int i = startItem; i < itemList.Count && i < startItem + engine.ItemsPerPage; i++)
        {            
            commentString = "discuss";
            
            Item item = itemList[i];



            string itemHref = string.Empty; //  The HREF when the Item is Clicked.
            if ((itemOptions & ShowItemsOptions.CountClicks) == ShowItemsOptions.CountClicks)
            {
                //  Redirect to R.aspx to count the clicks.

                if (!string.IsNullOrEmpty(item.Link))   //  Link Submission.
                {                    
                    itemHref = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + item.Link + engine.ItemLinkEndSeperator;
                }
                else    //  Text Submission.
                {
                    itemHref = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator;
                }
            }
            else
            {
                //  Do not Redirect to R.aspx. Do not count the clicks.

                if (!string.IsNullOrEmpty(item.Link))   //  Link Submission.
                {
                    itemHref = item.Link;                    
                }
                else    //  Text Submission.
                {
                    itemHref = links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
                }                
            }



            if (!string.IsNullOrEmpty(item.Link))   //  Link Submission
            {

                //  linkItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + item.Link + engine.ItemLinkEndSeperator + "'>" + item.Title + "</a>";
                linkItemHL = "<a class='CSS_LinkItem' target='_blank' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + itemHref + "'>" + item.Title + "</a>";
                //  showItems.Append(linkItemHL);

                Uri uri = new Uri(item.Link);
                string website = uri.Host;

                websiteHL = "<a class='CSS_ItemDomain' target='_blank' href='" + "http://" + website + "'>" + "[" + website.Replace("www.", "") + "]" + "</a>";
                //  showItems.Append(gui.HTMLSpace);
                //  showItems.Append(websiteHL);
            }
            else    //  Text Submission
            {
                //  textItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator + "'>" + item.Title + "</a>";
                textItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + itemHref + "'>" + item.Title + "</a>";

                websiteHL = string.Empty;
                //  showItems.Append(textItemHL);
            }
            

            /*
            //  Add an icon to open the link in a new window/tab.                
            if (imageEngine.IsIconsOn)
            {
                string iconLocation = imageEngine.LoadStaticIconLocation("NewWindow");
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    //  showItems.Append(gui.HTMLSpace);
                    iconLocation = iconLocation.Replace("~\\", string.Empty);

                    if (!string.IsNullOrEmpty(item.Link))    //  Link Submission
                    {
                        //  Redirect via R.aspx                        
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

                        //  Redirect Directly, Bypass R.aspx
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + item.Link + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                        newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + itemHref + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                    }
                    else    //  Text Submission
                    {
                        //  Redirect via R.aspx                        
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='R.aspx?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";

                        //  Redirect Directly, Bypass R.aspx
                        //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + links.ItemDetailsPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                        newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + itemHref + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
                    }
                    //  showItems.Append(newWindow);
                }
                */


                ////  Using Sprites
                //if (!string.IsNullOrEmpty(item.Link))   //  Link Submission.
                //{
                //    //  Using Sprites.
                //    //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + item.Link + "'>" + imageEngine.LoadSpriteStaticIcon("NewWindow") + "</a> ";
                //    newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + itemHref + "'>" + imageEngine.LoadSpriteStaticIcon("NewWindow") + "</a> ";

                //}
                //else    //  Text Submission.
                //{
                //    //  Using Sprites.
                //    //  newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + links.ItemDetailsPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "'>" + imageEngine.LoadSpriteStaticIcon("NewWindow") + "</a> ";
                //    newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + itemHref + "'>" + imageEngine.LoadSpriteStaticIcon("NewWindow") + "</a> ";
                //}
            }
            
            //  showItems.Append(gui.LineBreak);
            //  showItems.Append("<table class='NoDesign'>");

            if((itemOptions & ShowItemsOptions.ShowImage) == ShowItemsOptions.ShowImage)
            {
                if (imageEngine.IsItemImageRetrievalOn)
                {
                    List<string> itemImagesList = general.GetImages(item.Link, imageEngine.ItemImageRetrievalCount, item.IID);
                    
                    StringBuilder imagesSB = new StringBuilder();
                    if (itemImagesList != null)
                    {
                        for (int iCount = 0; iCount < itemImagesList.Count; iCount++)
                        {
                            //  imagesSB.AppendLine(gui.MakeClickableImage(itemImagesList[iCount], item.Link, "_blank", string.Empty) + gui.HTMLTab);
                            imagesSB.AppendLine(gui.MakeClickableImage(itemImagesList[iCount], itemHref, "_blank", string.Empty) + gui.HTMLTab);
                        }
                    }

                    itemImages = string.Empty;
                    if (!string.IsNullOrEmpty(imagesSB.ToString()))
                    {
                        itemImages = imagesSB.ToString();
                    }

                    if (!string.IsNullOrEmpty(itemImages))
                    {                        
                        //showItems.Append("<tr>");
                        //showItems.Append("<td valign='middle'>");
                        //showItems.Append(itemImages);
                        //showItems.Append("</td>");
                    }
                }
            }
            
            //  showItems.Append("<td valign='middle'>");

            if ((itemOptions & ShowItemsOptions.ShowUIDLink) == ShowItemsOptions.ShowUIDLink)
            {
                userHL = "<a class='CSS_Submitter' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.UserDetailsPageLink.Replace("~\\", "") + "?UID=" + item.UID + "' >" + item.UID + "</a>";
                //  showItems.Append(userHL + seperator);

                string userIconLocation = imageEngine.LoadStaticIconLocation("User");
                if (!string.IsNullOrEmpty(userIconLocation))
                {
                    userHL = "<img title='The user who posted this Item.' border= '0' style='vertical-align:middle;' src='" + userIconLocation.Replace("~\\", "") + "' alt=''/>"
                        + gui.HTMLSpace
                        + userHL;
                }

            }

            if ((itemOptions & ShowItemsOptions.ShowTime) == ShowItemsOptions.ShowTime)
            {
                string formattedDate = general.GetFormattedDate(item.Date);
                dateFormatString = "<span class='CSS_SubmissionFreshness'>" + gui.SmallFontStart + gui.GrayFontStart + formattedDate + gui.GrayFontEnd + gui.SmallFontEnd + "</span>";
                //  showItems.Append(dateFormatString + seperator);


                string dateIconLocation = imageEngine.LoadStaticIconLocation("DateTime");
                if (!string.IsNullOrEmpty(dateIconLocation))
                {
                    dateFormatString = "<img title='" + "This post was posted " + formattedDate + " ago." + "' border= '0' style='vertical-align:middle;' src='" + dateIconLocation.Replace("~\\", "") + "' alt='Time '/>"
                        + gui.HTMLSpace
                        + dateFormatString;
                }

            }

            if ((itemOptions & ShowItemsOptions.ShowCategoryLink) == ShowItemsOptions.ShowCategoryLink)
            {
                if (!string.IsNullOrEmpty(item.Category))
                {
                    string iconLocation = imageEngine.LoadIconLocation(item.Category);
                    if (!string.IsNullOrEmpty(iconLocation))
                    {
                        categoryHL =
                            "<span style=\" vertical-align:middle; background-color: " + Categories.Instance.CategoryColorsHashTable[item.Category] + "\" >"
                                + gui.HTMLSpace
                                + "<a class='CSS_Category' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.CategoryPageLink.Replace("~\\", "") + "?category=" + item.Category + "'>" + item.Category + "</a>"
                                + gui.HTMLSpace
                            + "</span>"

                            + gui.HTMLSpace
                            + "<img border= '0' style='vertical-align:middle;' src='" + iconLocation.Replace("~\\", "") + "' alt=''/>"
                            
                            ;
                        //  showItems.Append(categoryHL + seperator);
                    }

                    ////  Using Sprites.
                    //categoryHL = imageEngine.LoadSpriteIcon(item.Category)
                    //    + gui.HTMLSpace
                    //    + "<a class='CSS_Category' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.CategoryPageLink.Replace("~\\", "") + "?category=" + item.Category + "'>" + item.Category + "</a>";

                }
            }

            if ((itemOptions & ShowItemsOptions.ShowSaveLink) == ShowItemsOptions.ShowSaveLink)
            {
                //  if (!string.IsNullOrEmpty(UID)) //  Show the Saved link only if the User is logged in.
                {
                    saveHL = "<a class='CSS_Save' IID='" + item.IID + "' href='" + links.SavePageLink.Replace("~\\", "") + "?IID=" + item.IID.ToString() + "'>save</a>";


                    string saveIconLocation = imageEngine.LoadStaticIconLocation("Save");
                    if (!string.IsNullOrEmpty(saveIconLocation))
                    {
                        saveHL = "<img title='Save this Post for future reference.' border= '0' style='vertical-align:middle;' src='" + saveIconLocation.Replace("~\\", "") + "' alt='Save '/>"
                            + gui.HTMLSpace
                            + saveHL;
                    }


                    if (savedItems != null && savedItems.Count > 0)
                    {
                        if (savedItems.Contains(item.IID))
                        {
                            //  Do not show the Save Link.
                            saveHL = string.Empty;
                        }
                        else
                        {
                            //  Show the Save Link.
                            //  showItems.Append(saveHL + seperator);
                        }
                    }
                    else
                    {
                        //  Show the Save Link.                    
                        //  showItems.Append(saveHL + seperator);
                    }
                }

               


            }

            if ((itemOptions & ShowItemsOptions.ShowEMailLink) == ShowItemsOptions.ShowEMailLink)
            {
                if (engine.IsEMailOn)
                {
                    string emailIconLocation = imageEngine.LoadStaticIconLocation("EMail");
                    if (!string.IsNullOrEmpty(emailIconLocation))
                    {
                        //eMailHL = "<img title='E-Mail/Share this Item' border= '0' style='vertical-align:middle;' src='" + emailIconLocation.Replace("~\\", "") + "' alt='E-Mail'/>"
                        //    + "&nbsp;"
                        //    + "<a class='CSS_EMailLink' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.SendItemMailPageLink.Replace("~\\", "") + "?IID=" + item.IID + "'>E-Mail</a>";

                        eMailHL = "<img title='E-Mail/Share this Post.' border= '0' style='vertical-align:middle;' src='" + emailIconLocation.Replace("~\\", "") + "' alt='E-Mail'/>"
                            + gui.HTMLSpace
                            + "<a class='CSS_EMailLink' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.SendItemMailPageLink.Replace("~\\", "") + "?IID=" + item.IID + "'>share</a>";
                        

                        
                        //  showItems.Append(eMailHL + seperator);
                    }

                    ////  Using Sprites.
                    //eMailHL = imageEngine.LoadSpriteStaticIcon("EMail")
                    //    + gui.HTMLSpace
                    //    + "<a class='CSS_EMailLink' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.SendItemMailPageLink.Replace("~\\", "") + "?IID=" + item.IID + "'>E-Mail</a>";
                    
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

            if ((itemOptions & ShowItemsOptions.ShowCommentsLink) == ShowItemsOptions.ShowCommentsLink)
            {
                if (item.NComments > 0)
                {
                    commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
                }


                if ((itemOptions & ShowItemsOptions.CountClicks) == ShowItemsOptions.CountClicks)   //  Redirect to R.aspx to count the clicks.
                {
                    commentsHL = "<a class='CSS_Comments' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + engine.ItemLinkStartSeperator + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + engine.ItemLinkEndSeperator + "'>" + commentString + "</a>";
                }
                else    //  Do not Redirect to R.aspx. Do not count the clicks.
                {
                    commentsHL = "<a class='CSS_Comments' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + commentString + "</a>";
                }
                                

                string commentsIconLocation = imageEngine.LoadStaticIconLocation("Comment");
                if (!string.IsNullOrEmpty(commentsIconLocation))
                {
                    commentsHL = "<img title='Share your opinion with the community. Comment on this post.' border= '0' style='vertical-align:middle;' src='" + commentsIconLocation.Replace("~\\", "") + "' alt=''/>"
                        + gui.HTMLSpace
                        + commentsHL;
                }

                //  showItems.Append(commentsHL);
            }

            if ((itemOptions & ShowItemsOptions.ShowRatings) == ShowItemsOptions.ShowRatings)
            {
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

                    ratingDiv = string.Empty;
                    string ratingLabelID = "ratingLabel_" + item.IID.ToString();
                    string ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "[" + nRated + " ratings, Avg: " + avgRating + "]" + gui.SmallFontEnd + gui.GrayFontEnd;

                    if (nRated.Equals("0")) //  Do not show any Text.
                    {
                        ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + gui.SmallFontEnd + gui.GrayFontEnd;
                    }
                    if (nRated.Equals("1")) //  Show "rating" instead of "ratings"
                    {
                        //  ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + nRated + " vote." + gui.SmallFontEnd + gui.GrayFontEnd;
                        ratingLabelText = gui.GrayFontStart + gui.SmallFontStart + "[" + nRated + " rating, Avg: " + avgRating + "]" + gui.SmallFontEnd + gui.GrayFontEnd;
                    }

                    ratingLabel = "<span id='" + ratingLabelID + "' >" + ratingLabelText + "</span>";


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
                    
                    ratingDiv = gui.GrayFontStart + gui.SmallFontStart + "Was this worth reading?" + gui.SmallFontEnd + gui.GrayFontEnd
                        + gui.HTMLSpace 
                        //  + gui.LineBreak
                        + ratingDiv;


                    ratingAnalyticsHTMLString = string.Empty;
                    if (item.NRated > 0)
                    {
                        ratingAnalyticsHTMLString = GetRatingAnalytics(item);                
                    }



                    //  showItems.Append(gui.LineBreak + ratingDiv + gui.HTMLSpace + ratingLabel);

                    
                    
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
            }

            if ((itemOptions & ShowItemsOptions.ShowTags) == ShowItemsOptions.ShowTags)
            {
                //  //  Tag Production
                if (tagger.IsTaggingOn)
                {
                    tagIcon = string.Empty;
                    tagString = string.Empty;

                    if (item.TagList != null && item.TagList.Count > 0)
                    {
                        if (imageEngine.IsIconsOn)
                        {
                            string tagIconLocation = imageEngine.LoadStaticIconLocation("Tag");
                            if (!string.IsNullOrEmpty(tagIconLocation))
                            {
                                tagIcon = "<img title='Auto Generated Tags' border= '0' style='vertical-align:middle;' src='" + tagIconLocation.Replace("~\\", "") + "'></img>";
                            }

                            ////  Using Sprites
                            //tagIcon = imageEngine.LoadSpriteStaticIcon("Tag");
                        }
                                                
                        for (int t = 0; t < item.TagList.Count; t++)
                        {
                            string tagLink = "<a href='" + links.AutoTagPageLink.Replace("~\\", "") + "?tag=" + item.TagList[t] + "' class='CSS_TagLinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" >" + item.TagList[t] + "</a>";
                            tagString = tagString + ", " + tagLink;
                            //  tagString = tagString + " " + tagLink;
                        }
                        tagString = tagString.Trim().Trim(',');


                        //showItems.Append(gui.LineBreak
                        //    + tagIcon + gui.HTMLSpace + gui.SmallFontStart + gui.GrayFontStart + "Auto Generated Tags: " + tagString + gui.GrayFontEnd + gui.SmallFontEnd
                        //    + gui.LineBreak);

                    }
                    else
                    {
                        //  showItems.Append(gui.LineBreak);                        
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

            }


            
            
                if (engine.IsDebug)
                {
                    debugMessage = string.Empty;
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

                    //  showItems.Append(gui.GrayFontStart + gui.SmallFontStart + debugMessage + gui.SmallFontEnd + gui.GrayFontEnd);

                }
            
            //showItems.Append("</td>");
            //showItems.Append("</tr>");
            //showItems.Append("</table>");            
            //showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
            //showItems.Append(gui.LineBreak);
            
            
            #region Populate_ShowItems
            //  All the Display elements are added to showItems based on the ItemLayout Options.

                if (itemLayoutOptions == ItemLayoutOptions.Flow)
                {
                    #region Before 2009-05-01
                    //if (!string.IsNullOrEmpty(item.Link))
                    //{
                    //    showItems.Append(linkItemHL);
                    //    showItems.Append(gui.HTMLSpace);
                    //    showItems.Append(websiteHL);
                    //}
                    //else
                    //{
                    //    showItems.Append(textItemHL);
                    //}

                    //if (!string.IsNullOrEmpty(newWindow))
                    //{
                    //    showItems.Append(gui.HTMLSpace);
                    //    showItems.Append(newWindow);
                    //}

                    //showItems.Append(gui.LineBreak);


                    //if (!string.IsNullOrEmpty(userHL))
                    //{
                    //    showItems.Append(userHL);
                    //    showItems.Append(seperator);
                    //}

                    //if (!string.IsNullOrEmpty(dateFormatString))
                    //{
                    //    showItems.Append(dateFormatString);
                    //    showItems.Append(seperator);
                    //}

                    //if (!string.IsNullOrEmpty(categoryHL))
                    //{
                    //    showItems.Append(categoryHL);
                    //    //  showItems.Append(seperator);
                    //}

                    ////  showItems.Append(gui.LineBreak);
                    //showItems.Append(seperator);

                    //if (!string.IsNullOrEmpty(saveHL))
                    //{
                    //    showItems.Append(saveHL + seperator);
                    //}

                    //if (!string.IsNullOrEmpty(eMailHL))
                    //{
                    //    showItems.Append(eMailHL + seperator);
                    //}

                    //showItems.Append(commentsHL);




                    ////  showItems.Append(gui.LineBreak);

                    //showItems.Append("<table class='NoDesign'>");
                    //showItems.Append("<tr>");


                    //if (!string.IsNullOrEmpty(itemImages))
                    //{
                    //    //  showItems.Append(gui.LineBreak);

                    //    showItems.Append("<td valign='middle'>");
                    //    showItems.Append(itemImages);
                    //    showItems.Append("</td>");
                    //}

                    //showItems.Append("<td valign='middle'>");

                    

                    //if (!string.IsNullOrEmpty(ratingDiv) && !string.IsNullOrEmpty(ratingLabel))
                    //{
                    //    //  showItems.Append(gui.LineBreak);
                    //    showItems.Append(ratingDiv + gui.HTMLSpace + ratingLabel);


                    //    //  Show the RatingAnalytics HTML.
                    //    if (!string.IsNullOrEmpty(ratingAnalyticsHTMLString))
                    //    {
                    //        showItems.Append(gui.LineBreak);
                    //        showItems.Append(ratingAnalyticsHTMLString);
                    //        //  showItems.Append(gui.LineBreak);
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(tagString))
                    //{
                    //    showItems.Append(gui.LineBreak);
                    //    showItems.Append(tagIcon + gui.HTMLSpace + gui.SmallFontStart + gui.GrayFontStart
                    //        + "Auto Generated Tags: " + tagString + gui.GrayFontEnd + gui.SmallFontEnd + gui.LineBreak);
                    //}
                    //else
                    //{
                    //    showItems.Append(gui.LineBreak);
                    //}

                    //if (engine.IsDebug && !string.IsNullOrEmpty(debugMessage))
                    //{
                    //    showItems.Append(gui.GrayFontStart + gui.SmallFontStart + debugMessage + gui.SmallFontEnd + gui.GrayFontEnd);
                    //}

                    //showItems.Append("</td>");
                    //showItems.Append("</tr>");
                    //showItems.Append("</table>");

                    //showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
                    //showItems.Append(gui.LineBreak);

                    #endregion Before 2009-05-01

                    

                    //  Vatsal Shah | 2009-05-01 | Trying a New Layout.

                    if (!string.IsNullOrEmpty(item.Link))
                    {
                        showItems.Append(linkItemHL);
                        showItems.Append(gui.HTMLSpace);
                        showItems.Append(websiteHL);
                    }
                    else
                    {
                        showItems.Append(textItemHL);
                    }

                    if (!string.IsNullOrEmpty(newWindow))
                    {
                        showItems.Append(gui.HTMLSpace);
                        showItems.Append(newWindow);
                    }

                    showItems.Append(gui.LineBreak);

                    if (!string.IsNullOrEmpty(categoryHL))
                    {                        
                        showItems.Append(categoryHL);                                                                        
                        showItems.Append(seperator);
                    }                    

                    if (!string.IsNullOrEmpty(dateFormatString))
                    {
                        showItems.Append(dateFormatString);
                        showItems.Append(seperator);
                    }

                    if (!string.IsNullOrEmpty(userHL))
                    {
                        showItems.Append(userHL);
                        showItems.Append(seperator);
                    }

                    if (!string.IsNullOrEmpty(saveHL))
                    {
                        showItems.Append(saveHL);
                        showItems.Append(seperator);                            
                    }

                    if (!string.IsNullOrEmpty(eMailHL))
                    {
                        showItems.Append(eMailHL);
                        showItems.Append(seperator);                            
                    }

                    showItems.Append(commentsHL);

                    showItems.Append(gui.LineBreak);



                    showItems.Append("<table class='NoDesign'>");
                    showItems.Append("<tr>");


                        showItems.Append("<td valign='top' >");
                        if (!string.IsNullOrEmpty(itemImages))
                        {
                            //  showItems.Append(gui.LineBreak);                        
                            showItems.Append(itemImages);

                        }
                        showItems.Append("</td>");

                    

                        //  showItems.Append("<td style=\"border-right-color:#dfe4ee; border-right-width:thin; border-right-style:solid; \" valign='top'>");
                        showItems.Append("<td valign='middle'>");

                        if (!string.IsNullOrEmpty(ratingDiv) && !string.IsNullOrEmpty(ratingLabel))
                        {
                            //  showItems.Append(gui.LineBreak);
                            showItems.Append(ratingDiv + gui.HTMLSpace + ratingLabel);
                            showItems.Append(gui.LineBreak);   

                            //  Show the RatingAnalytics HTML.
                            if (!string.IsNullOrEmpty(ratingAnalyticsHTMLString))
                            {                                
                                showItems.Append(ratingAnalyticsHTMLString);
                                //  showItems.Append(gui.LineBreak);
                            }
                        }

                        

                        if (!string.IsNullOrEmpty(tagString))
                        {
                            //  showItems.Append(gui.LineBreak);    
                            showItems.Append(tagIcon 
                                + gui.HTMLSpace
                                + gui.SmallFontStart + gui.GrayFontStart
                                + "Auto Generated Tags: "
                                + tagString
                                + gui.GrayFontEnd + gui.SmallFontEnd);
                            showItems.Append(gui.LineBreak);
                        }
                        else
                        {
                            showItems.Append(gui.LineBreak);
                        }



                        showItems.Append("</td>");
                    
                    
                    showItems.Append("</tr>");
                    showItems.Append("</table>");

                    if (engine.IsDebug && !string.IsNullOrEmpty(debugMessage))
                    {
                        showItems.Append(gui.GrayFontStart + gui.SmallFontStart 
                            + debugMessage 
                            + gui.SmallFontEnd + gui.GrayFontEnd);
                        showItems.Append(gui.LineBreak);

                    }

                    showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
                    showItems.Append(gui.LineBreak);
                    
                    

                }

                else if (itemLayoutOptions == ItemLayoutOptions.Columns)
                {
                    if (i % 2 == 0)  //  Even Item
                    {
                        showItems.Append("<table class='NoDesign' style=\"width:100%; \" >");
                        showItems.Append("<tr>");


                        showItems.Append("<td valign=\"top\" style=\" width:50%; \">");


                                if (!string.IsNullOrEmpty(item.Link))
                                {
                                    showItems.Append(linkItemHL);
                                    //  showItems.Append(gui.HTMLSpace);
                                    showItems.Append(gui.LineBreak);
                                    showItems.Append(websiteHL);
                                }
                                else
                                {
                                    showItems.Append(textItemHL);
                                }

                                if (!string.IsNullOrEmpty(newWindow))
                                {
                                    showItems.Append(gui.HTMLSpace);
                                    showItems.Append(newWindow);
                                }

                                showItems.Append(gui.LineBreak);
                                showItems.Append("<table class='NoDesign'>");
                                showItems.Append("<tr>");


                                if (!string.IsNullOrEmpty(itemImages))
                                {
                                    //  showItems.Append(gui.LineBreak);

                                    showItems.Append("<td valign='middle'>");
                                    showItems.Append(itemImages);
                                    showItems.Append("</td>");
                                }

                                showItems.Append("<td valign='middle'>");

                                if (!string.IsNullOrEmpty(userHL))
                                {
                                    showItems.Append(userHL + seperator);
                                }

                                if (!string.IsNullOrEmpty(dateFormatString))
                                {
                                    showItems.Append(dateFormatString);
                                }

                                showItems.Append(gui.LineBreak);

                                if (!string.IsNullOrEmpty(categoryHL))
                                {
                                    showItems.Append(categoryHL);
                                }

                                showItems.Append(gui.LineBreak);
                        
                                if (!string.IsNullOrEmpty(eMailHL))
                                {
                                    showItems.Append(eMailHL + seperator);
                                }

                                if (!string.IsNullOrEmpty(saveHL))
                                {
                                    showItems.Append(saveHL + seperator);
                                }

                                showItems.Append(commentsHL);

                                if (!string.IsNullOrEmpty(ratingDiv) && !string.IsNullOrEmpty(ratingLabel))
                                {
                                    showItems.Append(gui.LineBreak + ratingDiv + gui.HTMLSpace + ratingLabel);
                                }

                                if (!string.IsNullOrEmpty(tagString))
                                {
                                    showItems.Append(gui.LineBreak + tagIcon + gui.HTMLSpace + gui.SmallFontStart + gui.GrayFontStart
                                        + "Auto Generated Tags: " + tagString + gui.GrayFontEnd + gui.SmallFontEnd + gui.LineBreak);
                                }
                                else
                                {
                                    showItems.Append(gui.LineBreak);
                                }

                                if (engine.IsDebug && !string.IsNullOrEmpty(debugMessage))
                                {
                                    showItems.Append(gui.GrayFontStart + gui.SmallFontStart + debugMessage + gui.SmallFontEnd + gui.GrayFontEnd);
                                }

                                showItems.Append("</td>");
                                showItems.Append("</tr>");
                                showItems.Append("</table>");

                                //  showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
                                //  showItems.Append(gui.LineBreak + gui.LineBreak);
                                showItems.Append(gui.LineBreak);


                        showItems.Append("</td>");

                        
                        if (i == itemList.Count - 1)     //  Last Item
                        {
                            showItems.Append("</tr>");
                            showItems.Append("</table>");
                        }
                    }
                    else
                    {
                        //  showItems.Append("<td>");
                        showItems.Append("<td valign=\"top\" style=\" width:50%; \">");




                        if (!string.IsNullOrEmpty(item.Link))
                        {
                            showItems.Append(linkItemHL);
                            //  showItems.Append(gui.HTMLSpace);
                            showItems.Append(gui.LineBreak);                            
                            showItems.Append(websiteHL);
                        }
                        else
                        {
                            showItems.Append(textItemHL);
                        }

                        if (!string.IsNullOrEmpty(newWindow))
                        {
                            showItems.Append(gui.HTMLSpace);
                            showItems.Append(newWindow);
                        }

                        showItems.Append(gui.LineBreak);
                        showItems.Append("<table class='NoDesign'>");
                        showItems.Append("<tr>");


                        if (!string.IsNullOrEmpty(itemImages))
                        {
                            //  showItems.Append(gui.LineBreak);

                            showItems.Append("<td valign='middle'>");
                            showItems.Append(itemImages);
                            showItems.Append("</td>");
                        }

                        showItems.Append("<td valign='middle'>");

                        if (!string.IsNullOrEmpty(userHL))
                        {
                            showItems.Append(userHL + seperator);
                        }

                        if (!string.IsNullOrEmpty(dateFormatString))
                        {
                            showItems.Append(dateFormatString);
                        }

                        showItems.Append(gui.LineBreak);

                        if (!string.IsNullOrEmpty(categoryHL))
                        {
                            showItems.Append(categoryHL);
                        }

                        showItems.Append(gui.LineBreak);

                        if (!string.IsNullOrEmpty(eMailHL))
                        {
                            showItems.Append(eMailHL + seperator);
                        }

                        if (!string.IsNullOrEmpty(saveHL))
                        {
                            showItems.Append(saveHL + seperator);
                        }

                        showItems.Append(commentsHL);

                        if (!string.IsNullOrEmpty(ratingDiv) && !string.IsNullOrEmpty(ratingLabel))
                        {
                            showItems.Append(gui.LineBreak + ratingDiv + gui.HTMLSpace + ratingLabel);
                        }

                        if (!string.IsNullOrEmpty(tagString))
                        {
                            showItems.Append(gui.LineBreak + tagIcon + gui.HTMLSpace + gui.SmallFontStart + gui.GrayFontStart
                                + "Auto Generated Tags: " + tagString + gui.GrayFontEnd + gui.SmallFontEnd + gui.LineBreak);
                        }
                        else
                        {
                            showItems.Append(gui.LineBreak);
                        }

                        if (engine.IsDebug && !string.IsNullOrEmpty(debugMessage))
                        {
                            showItems.Append(gui.GrayFontStart + gui.SmallFontStart + debugMessage + gui.SmallFontEnd + gui.GrayFontEnd);
                        }

                        showItems.Append("</td>");
                        showItems.Append("</tr>");
                        showItems.Append("</table>");

                        //  showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
                        //  showItems.Append(gui.LineBreak + gui.LineBreak);
                        showItems.Append(gui.LineBreak);



                        showItems.Append("</td>");



                        showItems.Append("</tr>");
                        showItems.Append("</table>");
                    }
                                       
                }
                else if (itemLayoutOptions == ItemLayoutOptions.Categorized)
                {
                    showItems.Append("<table class='NoDesign'>");
                    showItems.Append("<tr>");

                    /*
                    if (!string.IsNullOrEmpty(categoryHL))
                    {
                        showItems.Append("<td valign=\"top\" style=\"width: 140px;\">");
                        showItems.Append(categoryHL + gui.HTMLSpace);
                        showItems.Append("</td>");
                    }
                    else
                    {
                        showItems.Append("<td valign=\"top\" style=\"width: 14px;\">");
                        showItems.Append("</td>");
                    }
                    */


                    showItems.Append("<td>");
                    showItems.Append("</td>");

                    //  showItems.Append("<td>");
                    showItems.Append("<td style=\" width: 100%; background-color: " + Categories.Instance.CategoryColorsHashTable[item.Category] + "\" >");

                    showItems.Append("<span id='Title_" + item.IID.ToString() + "'>");
                    //  showItems.Append("<a href='#'>This is the Title</a>");

                    if (!string.IsNullOrEmpty(item.Link))
                    {
                        showItems.Append(linkItemHL);
                        showItems.Append(gui.HTMLSpace);
                        //  showItems.Append(websiteHL);
                    }
                    else
                    {
                        showItems.Append(textItemHL);
                    }

                    
                    showItems.Append("</span>");

                    showItems.Append(gui.LineBreak);

                    showItems.Append("<span id='OpenCloseSign_" + item.IID.ToString() + "' >");
                    //  showItems.Append("<a href=\"javascript:;\" onclick=\"OpenClose(" + item.IID.ToString() + ", 'block')\" >[+]</a>");
                    showItems.Append("<a href=\"javascript:;\" style=\"text-decoration: none;\" onclick=\"OpenClose(" + item.IID.ToString() + ", 'block')\" ><img src='Images/Icons/Plus.gif' style=\"border: none;\" /></a>");

                    showItems.Append("</span>");
                    showItems.Append("&nbsp;");
                    
                    if (!string.IsNullOrEmpty(newWindow))
                    {                       
                        showItems.Append(gui.HTMLSpace);
                        showItems.Append(newWindow);                        
                    }
                    
                    
                    showItems.Append("</td>");

                    
                    showItems.Append("</tr>");


                    

                    //  showItems.Append(gui.LineBreak);


                    /*
                        <span id="OpenCloseSign_1">
                            <a href="javascript:;" onclick="OpenClose(1, 'block')" >[+]</a>
                        </span>
                        &nbsp;
                        <span id="Title_1">
                            <a href="#">This is the Title</a>
                        </span>
                        <span id="DetailText_1" class="CSS_HideIt">These are the Details</span>                     
                    */


                    showItems.Append("<tr>");
                    
                    showItems.Append("<td>");
                        //  Empty Column, comes below the Displayed Categories.
                    showItems.Append("</td>");
                    
                    
                    showItems.Append("<td>");

                    #region HideShow
                    
                    showItems.Append("<span id='DetailText_" + item.IID.ToString() + "' class='CSS_HideIt'>");
                                        
                    showItems.Append("<table class='NoDesign'>");
                    showItems.Append("<tr>");


                    if (!string.IsNullOrEmpty(itemImages))
                    {
                        //  showItems.Append(gui.LineBreak);

                        showItems.Append("<td valign='middle'>");
                        showItems.Append(itemImages);
                        showItems.Append("</td>");
                    }

                    showItems.Append("<td valign='middle'>");

                    if (!string.IsNullOrEmpty(websiteHL))
                    {
                        showItems.Append(websiteHL);
                        showItems.Append(gui.LineBreak);
                    }

                    if (!string.IsNullOrEmpty(userHL))
                    {
                        showItems.Append(userHL + seperator);
                    }

                    if (!string.IsNullOrEmpty(dateFormatString))
                    {
                        showItems.Append(dateFormatString + seperator);
                    }

                    //  If Category is already displayed, Then comment this if() Statement.
                    if (!string.IsNullOrEmpty(categoryHL))
                    {
                        showItems.Append(categoryHL + seperator);
                    }

                    if (!string.IsNullOrEmpty(saveHL))
                    {
                        showItems.Append(saveHL + seperator);
                    }

                    if (!string.IsNullOrEmpty(eMailHL))
                    {
                        showItems.Append(eMailHL + seperator);
                    }

                    showItems.Append(commentsHL);

                    if (!string.IsNullOrEmpty(ratingDiv) && !string.IsNullOrEmpty(ratingLabel))
                    {
                        showItems.Append(gui.LineBreak + ratingDiv + gui.HTMLSpace + ratingLabel);
                    }

                    if (!string.IsNullOrEmpty(tagString))
                    {
                        showItems.Append(gui.LineBreak + tagIcon + gui.HTMLSpace + gui.SmallFontStart + gui.GrayFontStart
                            + "Auto Generated Tags: " + tagString + gui.GrayFontEnd + gui.SmallFontEnd + gui.LineBreak);
                    }
                    else
                    {
                        showItems.Append(gui.LineBreak);
                    }

                    if (engine.IsDebug && !string.IsNullOrEmpty(debugMessage))
                    {
                        showItems.Append(gui.GrayFontStart + gui.SmallFontStart + debugMessage + gui.SmallFontEnd + gui.GrayFontEnd);
                    }

                    showItems.Append("</td>");
                    showItems.Append("</tr>");
                    showItems.Append("</table>");

                    //  Displaying the gui.ItemSeperator does not look good.
                    //  showItems.Append(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd);
                    showItems.Append(gui.LineBreak);
                    
                    

                    showItems.Append("</span>");

                    #endregion HideShow


                    showItems.Append("</tr>");
                    showItems.Append("</td>");
                    showItems.Append("</table>");


                }


            #endregion Populate_ShowItems



        }   //  itemList Loop Ends.
                
        string sortStr = engine.GetSortString(sort);

        if ((itemOptions & ShowItemsOptions.ShowPreviousNextLinks) == ShowItemsOptions.ShowPreviousNextLinks)
        {
            showItems.Append(gui.LineBreak);
            
            //  Add the "Previous" Link if required.
            if (startItem - engine.ItemsPerPage >= 0)
            {
                int previousItem = startItem - engine.ItemsPerPage;

                //  string previousPageHL = "<a class='CSS_Next' href='" + links.FrontPageLink.Replace("~\\", "") + "?sort=" + sortStr + "&startItem=" + previousItem.ToString() + "'>Previous</a>";
                previousPageHL = "<a class='CSS_Next' href='" + currentPageLink + (currentPageLink.Contains("?") ? "&" : "?") + "sort=" + sortStr + "&startItem=" + previousItem.ToString() + "'>Previous</a>";
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

                //  string nextPageHL = "<a class='CSS_Next' href='" + links.FrontPageLink.Replace("~\\", "") + "?sort=" + sortStr + "&startItem=" + nextItem.ToString() + "'>Next</a>";
                nextPageHL = "<a class='CSS_Next' href='" + currentPageLink + (currentPageLink.Contains("?") ? "&" : "?") + "sort=" + sortStr + "&startItem=" + nextItem.ToString() + "'>Next</a>";
                showItems.Append(nextPageHL);
            }
        }

        showItems.Append(gui.LineBreak + gui.LineBreak);

        return showItems.ToString();
    }



    /// <summary>
    /// Get the Rating Analytics for the input Item in HTML Format.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private string GetRatingAnalytics(Item item)
    {
        StringBuilder ratingAnalyticsSB = new StringBuilder();

        //  ratingFrequency Dictionary would have Keys = [1,2,3,4,5] and Values = Frequency of each Rating.
        Dictionary<int, int> ratingFrequency = new Dictionary<int, int>();

        //  Initialize the ratingFrequency Dictionary with all Values = 0;
        for (int i = 1; i <= 5; i++)
        {
            ratingFrequency.Add(i, 0);
        }
        
        string queryString = string.Format("SELECT Rating FROM rating WHERE IID = {0};", item.IID.ToString());
        MySqlDataReader retList = dbOps.ExecuteReader(queryString);

        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                int currentRating = Convert.ToInt32(retList["Rating"]);

                if (currentRating >= 1 && currentRating <= 5)
                {
                    ratingFrequency[currentRating]++;
                }
            }
        }

        //  ratingAnalyticsSB.Append("<div class='CSS_RatingAnalyticsDiv'>");
        //  ratingAnalyticsSB.Append("<div class='NoDesign'>");
        //  ratingAnalyticsSB.Append("<table>");
        ratingAnalyticsSB.Append(gui.SmallFontStart + gui.GrayFontStart);        
        ratingAnalyticsSB.Append("<table class='NoDesign'>");
                for (int i = 5; i >=1; i--)
                {
                    ratingAnalyticsSB.Append("<tr>");

                        ratingAnalyticsSB.Append("<td>");
                            ratingAnalyticsSB.Append(i + " star:");
                        ratingAnalyticsSB.Append("</td>");


                        /*                        
                        <td style="min-width:60;" width="60" align="left" bgcolor="eeeecc" class="tiny" title="62%">
                            <div style="background-color:#FFCC66; height:13px; width:62%;">
                            </div>
                        </td>
                        */

                        //  No need to check for "Divide By Zero". If item.NRated == 0, We won't even be able to enter this method.
                        int percentageFrequency = (ratingFrequency[i] * 100) / item.NRated;

                        ratingAnalyticsSB.Append("<td style= \" min-width:60; background-color:#EEEECC; \" width='60' align='left' title='" + percentageFrequency + "%' >");
                            ratingAnalyticsSB.Append("<div style= \" background-color:#FFCC66; height:13px; width:" + percentageFrequency + "%; \" >");

                            ratingAnalyticsSB.Append("</div>");
                        ratingAnalyticsSB.Append("</td>");


                        ratingAnalyticsSB.Append("<td>");
                            ratingAnalyticsSB.Append("[" + ratingFrequency[i] + "]");
                        ratingAnalyticsSB.Append("</td>");                    

                    ratingAnalyticsSB.Append("</tr>");
                }
            ratingAnalyticsSB.Append("</table>");
            ratingAnalyticsSB.Append(gui.GrayFontEnd + gui.SmallFontEnd);
        //  ratingAnalyticsSB.Append("</div>");

        return ratingAnalyticsSB.ToString();    
    }

}
