//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public partial class Submitted : System.Web.UI.Page
{
    Links links;
    Logger log;
    GUIVariables gui;
    DBOperations dbOps;
    Categories categories;
    General general;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    ItemDisplayer itemDisplayer;

    string queryStringUID = string.Empty;
    string loggedUID = string.Empty;  //  The UID of the logged in user.

    int startItem = 0;  //  Default Value;   
    ProcessingEngine.Sort sort = ProcessingEngine.Sort.New;
    string seperator = " | ";

    ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Flow;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Columns;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Categorized;

    protected void Page_Load(object sender, EventArgs e)
    {
        links = Links.Instance;
        gui = GUIVariables.Instance;
        dbOps = DBOperations.Instance;
        categories = Categories.Instance;
        log = Logger.Instance;
        engine = ProcessingEngine.Instance;
        general = General.Instance;
        imageEngine = ImageEngine.Instance;
        itemDisplayer = ItemDisplayer.Instance;

        seperator = gui.Seperator;

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
            loggedUID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }

        if (string.IsNullOrEmpty(loggedUID))
        {

        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists


        if (string.IsNullOrEmpty(Request.QueryString["UID"]))
        {

        }
        else
        {
            queryStringUID = Request.QueryString["UID"].Trim().ToLower();
        }

        string message = string.Empty;
        MessageLabel.Text = string.Empty;
        if (string.IsNullOrEmpty(queryStringUID))
        {
            message = gui.GreenFontStart + "No such User exists." + gui.GreenFontEnd;
        }
        else
        {            
            List<Item> itemList = engine.LoadItemDB(sort, queryStringUID);
            if (itemList != null && itemList.Count > 0)
            {
                message = gui.GreenFontStart + "Most recent submissions by " + queryStringUID + gui.GreenFontEnd;
                
                //  LoadItemTable(itemList);

                
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
                
                string itemTable = itemDisplayer.LoadItemTable(itemList, itemOptions, itemLayoutOptions, startItem, queryStringUID, sort, links.SubmittedPageLink.Replace("~\\", "") + "?UID=" + queryStringUID);
                ItemDiv.InnerHtml = itemTable;
            }
            else
            {
                message = gui.GreenFontStart + queryStringUID + " has not made any submissions yet." + gui.GreenFontEnd;
            }            
        }
        MessageLabel.Text = message;
    }

    private void LoadItemTable(List<Item> itemList)
    {
        string commentString = "discuss";

        List<int> savedItems = null;
        List<int> ratedItems = null;
        if (!string.IsNullOrEmpty(loggedUID))
        {
            //  Get the list of items that have been saved by the User only if the User is logged in.
            savedItems = general.GetSavedItemIID(loggedUID);
            ratedItems = general.GetRatedItemsIID(loggedUID);
        }

        //  for (int i = 0; i < itemList.Count; i++)
        for (int i = startItem; i < itemList.Count && i < startItem + engine.ItemsPerPage; i++)
        {
            commentString = "discuss";

            Item item = itemList[i];

            if (!string.IsNullOrEmpty(item.Link))
            {
                //HyperLink linkItemHL = new HyperLink();
                //linkItemHL.CssClass = "CSS_LinkItem";
                //linkItemHL.Text = item.Title;
                ////  linkItemHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + item.Link;
                //linkItemHL.NavigateUrl = item.Link;
                //ItemDiv.Controls.Add(linkItemHL);


                string linkItemHL = "<a class='CSS_LinkItem' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + item.Link + "'>" + item.Title + "</a>";
                ItemDiv.Controls.Add(new LiteralControl(linkItemHL));


                Uri uri = new Uri(item.Link);
                string website = uri.Host;

                HyperLink websiteLink = new HyperLink();
                websiteLink.Text = "  " + "[" + website.Replace("www.", "") + "]";
                websiteLink.CssClass = "CSS_ItemDomain";
                websiteLink.Target = "_blank";
                websiteLink.NavigateUrl = "http://" + website;
                ItemDiv.Controls.Add(websiteLink);
            }
            else
            {
                //HyperLink textItemHL = new HyperLink();
                //textItemHL.Text = item.Title;
                //textItemHL.CssClass = "CSS_TextItem";
                ////  textItemHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
                //textItemHL.NavigateUrl = links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
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

            if (!string.IsNullOrEmpty(loggedUID)) //  Show the Saved link only if the User is logged in.
            {
                //  List<int> savedItems = general.GetSavedItemIID(loggedUID);

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
                if (!string.IsNullOrEmpty(loggedUID)) //  Show the Saved link only if the User is logged in.
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
            ////  commentsHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
            //commentsHL.NavigateUrl = links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
            //ItemDiv.Controls.Add(commentsHL);

            if (item.NComments > 0)
            {
                commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
            }
            string commentsHL = "<a class='CSS_Comments' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + commentString + "</a>";
            ItemDiv.Controls.Add(new LiteralControl(commentsHL));


            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak + gui.LineBreak));
            ItemDiv.Controls.Add(new LiteralControl(gui.GrayFontStart + gui.ItemSeperator + gui.GrayFontEnd));
            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

        }   //  itemList Loop Ends.


        string sortStr = sort == ProcessingEngine.Sort.New ? "n" : "h";

        //  Add the "Previous" Link if required.
        if (startItem - engine.ItemsPerPage >= 0)
        {
            int previousItem = startItem - engine.ItemsPerPage;

            HyperLink previousPageHL = new HyperLink();
            previousPageHL.Text = "Previous";
            previousPageHL.CssClass = "CSS_Next";
            previousPageHL.NavigateUrl = links.SubmittedPageLink + "?UID=" + queryStringUID + "&sort=" + sortStr + "&startItem=" + previousItem.ToString();

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

            HyperLink nextPageHL = new HyperLink();
            nextPageHL.Text = "Next";
            nextPageHL.CssClass = "CSS_Next";
            nextPageHL.NavigateUrl = links.SubmittedPageLink + "?UID=" + queryStringUID + "&sort=" + sortStr + "&startItem=" + nextItem.ToString();

            ItemDiv.Controls.Add(nextPageHL);
        }

        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
    }

    

    void saveLB_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(loggedUID)) //  Show the Saved link only if the User is logged in.
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
            string queryString = "INSERT INTO saveditems VALUES ('" + loggedUID + "' , " + IID + " , '" + date + "');";
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
        if (!string.IsNullOrEmpty(loggedUID)) //  Show the Saved link only if the User is logged in.
        {
            ImageButton spamIB = (ImageButton)sender;

            string iidStr = Convert.ToString(spamIB.Attributes["IID"]);
            string reporter_UID = loggedUID;
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

    
}
