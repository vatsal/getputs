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
using MySql.Data.MySqlClient;
using System.Net;
using System.Collections.Generic;


public partial class ItemDetails : System.Web.UI.Page
{
    //  The Table getputs.user has the following schema:
    //  getputs.item [UID, Password, Date, EMail, About, Admin, Points]

    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    Categories categories;
    Tagger tagger;
    ItemDisplayer itemDisplayer;

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string seperator = " | ";
    private static int itemClicks = 0;

    string iid  = string.Empty;
    string user = string.Empty; //  Logged In User, different from uid.
    string UID = string.Empty; //   The user who submitted the item. 
    Item item;

    int _minTokensInComment = 2;   //  Minimum words required in the Comments Text.
    int _maxTokensInComment = 500;   //  Minimum words required in the Comments Text.

    ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Flow;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Columns;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Categorized;

    protected void Page_Load(object sender, EventArgs e)
    {
        DirectContactButton.Visible = false;

        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;        
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        categories = Categories.Instance;
        tagger = Tagger.Instance;
        itemDisplayer = ItemDisplayer.Instance;

        seperator = gui.Seperator;

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            user = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(user))
        {
            //  Response.Redirect(links.LoginLink, false);
            MessageLabel.Text = gui.RedFontStart + "Please login to enter a comment." + gui.RedFontEnd;
        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists


        if (string.IsNullOrEmpty(Request.QueryString["IID"]))
        {
            Response.Redirect(links.FrontPageLink, false);
        }
        else
        {
            iid = Request.QueryString["IID"].Trim().ToLower();
        } 
               



        if (!general.IsValidInt(iid))
        {

        }
        else
        {
            LoadItemDetails(iid);
            LoadComments(iid);
        }

       
        
    }

    

    private void LoadItemDetails(string IID)
    {
        item = new Item();
                
        //  string queryString = "SELECT Title, Link, Text, Date, UID, NComments, Category FROM item WHERE IID=" + IID + ";";
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, NSaved, NEMailed, Tags, AvgRating, NRated FROM item WHERE IID=" + IID + ";";
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                item.IID = Convert.ToInt32(retList["IID"]);
                item.UID = Convert.ToString(retList["UID"]);
                item.Title = Convert.ToString(retList["Title"]);
                item.Link = Convert.ToString(retList["Link"]);
                item.Text = Convert.ToString(retList["Text"]);
                item.Date = Convert.ToString(retList["Date"]);
                item.NComments = Convert.ToInt32(retList["NComments"]);
                item.Category = Convert.ToString(retList["Category"]);
                               
                item.Clicks = Convert.ToInt32(retList["Clicks"]);
                item.NSaved = Convert.ToInt32(retList["NSaved"]);
                item.NEMailed = Convert.ToInt32(retList["NEMailed"]);

                item.TagString = Convert.ToString(retList["Tags"]);
                item.TagList = general.ConvertCSVToList(item.TagString);

                item.AvgRating = Convert.ToDouble(retList["AvgRating"]);
                item.NRated = Convert.ToInt32(retList["NRated"]);

                item.Age = item.GetAge(item.Date);
                //  item.Marks = item.GetMarks(item.Clicks, item.NComments, item.Age);
                item.Marks = item.GetMarks(item);


                if (!string.IsNullOrEmpty(item.Link))
                {
                    item.Text = string.Empty;
                }
            }
            retList.Close();
        }

        
        Page.Title = item.Title + " - getputs.com";
        
        //  Vatsal Shah
        //  2009-01-12
        //  The code below is not used. ItemDisplayer.cs is used instead.

        /*
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        string commentString = "discuss";

        List<int> savedItems = null;
        List<int> ratedItems = null;
        if (!string.IsNullOrEmpty(UID))
        {
            //  Get the list of items that have been saved by the User only if the User is logged in.
            savedItems = general.GetSavedItemIID(UID);
            ratedItems = general.GetRatedItemsIID(UID);
        }

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

        if (!string.IsNullOrEmpty(user)) //  Show the Saved link only if the User is logged in.
        {
            //  List<int> savedItems = general.GetSavedItemIID(user);

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
                
            //    //  emailIB.Attributes.Add("IID", item.IID.ToString());
            //    emailIB.Attributes.Add("IID", IID);


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

        //if (item.NComments > 0)
        //{
        //    commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
        //}
        //HyperLink commentsHL = new HyperLink();
        //commentsHL.CssClass = "CSS_Comments";
        //commentsHL.Text = commentString;
        ////  commentsHL.NavigateUrl = links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
        //commentsHL.NavigateUrl = links.ItemDetailsPageLink + "?iid=" + iid;
        //ItemDiv.Controls.Add(commentsHL);

        if (item.NComments > 0)
        {
            commentString = item.NComments == 1 ? (item.NComments.ToString() + " comment") : (item.NComments.ToString() + " comments");
        }
        string commentsHL = "<a class='CSS_Comments' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.RPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "&url=" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + commentString + "</a>";
        ItemDiv.Controls.Add(new LiteralControl(commentsHL));

         
        ///////////////////////////////////////////////////////////////////////////////////////////////////// 
        */

        List<Item> itemList = new List<Item>(1);
        itemList.Add(item);
        
        ItemDisplayer.ShowItemsOptions itemOptions = ItemDisplayer.ShowItemsOptions.ShowUIDLink
            | ItemDisplayer.ShowItemsOptions.ShowTime
            | ItemDisplayer.ShowItemsOptions.ShowCategoryLink
            | ItemDisplayer.ShowItemsOptions.ShowSaveLink
            | ItemDisplayer.ShowItemsOptions.ShowEMailLink
            | ItemDisplayer.ShowItemsOptions.ShowCommentsLink
            | ItemDisplayer.ShowItemsOptions.ShowImage
            | ItemDisplayer.ShowItemsOptions.ShowRatings
            | ItemDisplayer.ShowItemsOptions.ShowTags
            | ItemDisplayer.ShowItemsOptions.CountClicks;

        //  The variable user = Logged In User, UID = User who submitted the item.
        string itemTable = itemDisplayer.LoadItemTable(itemList, itemOptions, itemLayoutOptions, 0, user, ProcessingEngine.Sort.Hot, links.ItemDetailsPageLink.Replace("~\\", ""));
        ItemDiv.Controls.Add(new LiteralControl(itemTable));


        if (!string.IsNullOrEmpty(item.Text))
        {
            string itemText = general.ProcessTextForDisplay(item.Text);
            
            Label itemTextLabel = new Label();
            itemTextLabel.Text = itemText;
            itemTextLabel.CssClass = "CSS_ItemTextDetails";
        
            ItemDiv.Controls.Add(itemTextLabel);
            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        }

        ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        
        #region DirectContactButton
        //  If the Direct Contact Tags List (from the Web.Config) contains the current Item.Category
        //      Display the DirectContactButton
        //  Else
        //      Do not display the DirectContactButton

        UID = item.UID;

        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(item.Category))
        {
            if (categories.CategoriesListDirectContact != null && categories.CategoriesListDirectContact.Count > 0)
            {
                if(categories.CategoriesListDirectContact.Contains(item.Category))
                {
                    DirectContactButton.Visible = true;
                }                
            }
        }
        #endregion DirectContactButton

    }



    

    void deleteLB_Click(object sender, EventArgs e)
    {
        LinkButton deleteLB = (LinkButton)sender;
        string iidStr = Convert.ToString(deleteLB.Attributes["IID"]);
        string cidStr = Convert.ToString(deleteLB.Attributes["CID"]);
        string uidStr = Convert.ToString(deleteLB.Attributes["UID"]);

        string queryString = "DELETE FROM comments WHERE IID=" + iidStr + " AND CID=" + cidStr + " AND UID='" + uidStr + "';";
        int retInt = dbOps.ExecuteNonQuery(queryString);

        if (retInt >= 0)
        {
            queryString = "UPDATE item SET NComments = NComments - 1 WHERE IID=" + iidStr + ";";
            retInt = dbOps.ExecuteNonQuery(queryString);

            if (retInt >= 0)
            {
                Response.Redirect(links.ItemDetailsPageLink + "?IID=" + iidStr, false);
            }
        }
        
    }

    
       
    void saveLB_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(user)) //  Show the Saved link only if the User is logged in.
        {
            LinkButton saveLB = (LinkButton)sender;
            saveLB.Text = "saved";
            saveLB.Enabled = false;
            saveLB.CssClass = "CSS_AfterSave";
            
            string IID = saveLB.Attributes["IID"];

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string queryString = "INSERT INTO saveditems VALUES ('" + user + "' , " + IID + " , '" + date + "');";
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

        int iid = int.Parse(iidStr);
        Response.Redirect(links.SendItemMailPageLink + "?IID=" + iid.ToString(), false);
    }
    

    private void LoadComments(string iid)
    {
        //CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        //CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));

        //  string queryString = "SELECT CID, UID, Date, Comment FROM getputs.comments WHERE IID=" + iid + " ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S')  DESC;";
        
        //  string queryString = "SELECT CID, UID, Date, Comment FROM getputs.comments WHERE IID=" + iid + " ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S')  ASC;";
        string queryString = "SELECT CID, UID, Date, Comment FROM comments WHERE IID=" + iid + " ORDER BY CID ASC;";
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        int commentNumber = 0;
        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                HtmlTableRow row = new HtmlTableRow();

                commentNumber++;
                string commentString = gui.GrayFontStart + commentNumber.ToString() + gui.GrayFontEnd + gui.HTMLTab;
                HtmlTableCell commentNumberCell = new HtmlTableCell();
                commentNumberCell.Controls.Add(new LiteralControl(commentString));
                //  commentNumberCell.VAlign = "top";

                HtmlTableCell commentDetailsCell = new HtmlTableCell();

                string cid = Convert.ToString(retList["CID"]);
                string uid = Convert.ToString(retList["UID"]);
                string date = Convert.ToString(retList["Date"]);
                string originalComment = Convert.ToString(retList["Comment"]);



                string comment = general.ProcessTextForDisplay(originalComment);

                HyperLink userHL = new HyperLink();
                userHL.Text = uid;
                userHL.Font.Underline = false;
                userHL.CssClass = "CSS_Submitter";
                userHL.NavigateUrl = links.UserDetailsPageLink + "?UID=" + uid;
                commentDetailsCell.Controls.Add(userHL);
                                
                commentDetailsCell.Controls.Add(new LiteralControl(seperator));

                string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(date) + gui.GrayFontEnd + gui.SmallFontEnd;
                Label dateLabel = new Label();
                dateLabel.Text = dateFormatString;
                dateLabel.CssClass = "CSS_SubmissionFreshness";

                commentDetailsCell.Controls.Add(dateLabel);

                //  'user' is the User who is currently logged in.
                //  'uid' is the User who submitted the comment.
                if (!string.IsNullOrEmpty(user) && uid.Equals(user))   //  Allow the user the ability to edit the comment.
                {
                    commentDetailsCell.Controls.Add(new LiteralControl(seperator));

                    HyperLink editCommentHL = new HyperLink();
                    editCommentHL.NavigateUrl = links.EditCommentPageLink + "?uid=" + uid + "&iid=" + iid + "&cid=" + cid;
                    editCommentHL.CssClass = "CSS_CommentsEdit";
                    editCommentHL.Text = "Edit";

                    commentDetailsCell.Controls.Add(editCommentHL);

                    //  commentDetailsCell.Controls.Add(new LiteralControl(seperator));
                }




                //  'user' is the User who is currently logged in.
                //  'uid' is the User who submitted the comment.
                bool isAdmin = general.IsUserAdministrator(user);
                if (isAdmin)
                {
                    commentDetailsCell.Controls.Add(new LiteralControl(seperator));

                    //  Provide a delete button for the administrator.
                    LinkButton deleteLB = new LinkButton();
                    deleteLB.Click += new EventHandler(deleteLB_Click);
                    deleteLB.Text = "Delete";
                    deleteLB.CssClass = "CSS_CommentsDelete";
                    
                    deleteLB.Attributes.Add("IID", iid);
                    deleteLB.Attributes.Add("CID", cid);
                    deleteLB.Attributes.Add("UID", uid);   //  'uid' is the User who submitted the comment.
                    
                    commentDetailsCell.Controls.Add(deleteLB);

                    commentDetailsCell.Controls.Add(new LiteralControl(seperator));
                }

                commentDetailsCell.Controls.Add(new LiteralControl(gui.LineBreak));
                
                //  Display the comments in Black.
                //  commentDetailsCell.Controls.Add(new LiteralControl(comment));
                //  Display the comments in Gray.
                commentDetailsCell.Controls.Add(new LiteralControl(gui.GrayFontStart + comment + gui.GrayFontEnd));

                row.Controls.Add(commentNumberCell);
                row.Controls.Add(commentDetailsCell);
                CommentDiv.Rows.Add(row);


                HtmlTableRow blankRow = new HtmlTableRow();
                HtmlTableCell blankCell = new HtmlTableCell();
                blankCell.Controls.Add(new LiteralControl(gui.LineBreak));
                blankRow.Controls.Add(blankCell);
                CommentDiv.Rows.Add(blankRow);
                
                //  CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));
                //  CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));

            }
            retList.Close();
        }
    }

    /*
    private void LoadComments(string iid)
    {
        CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));

        //  string queryString = "SELECT CID, UID, Date, Comment FROM getputs.comments WHERE IID=" + iid + " ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S')  DESC;";
        string queryString = "SELECT CID, UID, Date, Comment FROM getputs.comments WHERE IID=" + iid + " ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S')  ASC;";
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        if (retList != null && retList.HasRows)
            while (retList.Read())
            {
                string cid = Convert.ToString(retList["CID"]);
                string uid = Convert.ToString(retList["UID"]);
                string date = Convert.ToString(retList["Date"]);
                string comment = Convert.ToString(retList["Comment"]);

                LinkButton userLB = new LinkButton();
                userLB.Click += new EventHandler(userLB_Click);
                userLB.Text = uid;
                userLB.CssClass = "CSS_Submitter";
                //userLB.Font.Underline = false;
                //userLB.Font.Size = FontUnit.Small;
                //userLB.ForeColor = System.Drawing.Color.Blue;

                CommentDiv.Controls.Add(userLB);

                CommentDiv.Controls.Add(new LiteralControl(seperator));

                string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(date) + gui.GrayFontEnd + gui.SmallFontEnd;
                Label dateLabel = new Label();
                dateLabel.Text = dateFormatString;
                dateLabel.CssClass = "CSS_SubmissionFreshness";
                //dateLabel.Font.Size = FontUnit.Small;
                //dateLabel.ForeColor = System.Drawing.Color.Gray;
                CommentDiv.Controls.Add(dateLabel);

                CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));

                CommentDiv.Controls.Add(new LiteralControl(comment));

                CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));
                CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));

            }
        retList.Close();
    }
    */


    protected void CommentButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(user))
        {
            Response.Redirect(links.LoginLink, false);
        }
        else
        {
            MessageLabel.Text = string.Empty;

            string comment = CommentTB.Text.Trim();
            comment = general.EscapeCharacters(comment);
            
            if (string.IsNullOrEmpty(comment))
            {
                MessageLabel.Text = gui.RedFontStart + "Please enter a valid comment." + gui.RedFontEnd;
            }
            else
            {
                List<string> commentTokens = tagger.GetTokens(comment, false, false, false);
                if (commentTokens.Count >= _minTokensInComment && commentTokens.Count <= _maxTokensInComment)
                {

                    //  Get the IP Address
                    string ip = general.GetIP(this.Request);

                    //  Check if the Comment is a Spam Comment.
                    SpamDetection spamDetection = new SpamDetection();

                    if (spamDetection.IsSpam(user, ip, comment))
                    {
                        //  Spam

                        //  Store this item as spam. This DB of spam comments might be helpful in training a filter. 

                        MessageLabel.Text = gui.RedFontStart + "Our Spam Filter detected this Comment to be spam." + gui.RedFontEnd;
                    }
                    else
                    {
                        //  Not Spam
                             
           
                        //  Get the Tags for the Comment.
                        string commentTagString = string.Empty;
                        ////  Vatsal Shah | 2009-04-21 | Am turning tagging off for now. Not too sure whether we should have this feature.
                        //if (tagger.IsTaggingOn)
                        //{
                        //    List<string> commentTags = tagger.GetImportantTagsList(commentTokens, tagger.GetPOSTags(commentTokens));
                        //    commentTagString = general.ConvertListToCSV(commentTags);
                        //}
                                                

                        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //  string queryString = "SELECT NComments FROM item WHERE iid = " + iid + ";";

                        //  SELECT MAX(CID) FROM comments WHERE IID = 200;
                        string queryString = "SELECT MAX(CID) FROM comments WHERE IID = " + iid + ";";
                        int cid = dbOps.ExecuteScalar(queryString) + 1;

                        //  queryString = "INSERT INTO comments (CID, IID, UID, Date, Comment) VALUES(" + cid.ToString() + ", " + iid.ToString() + ", '" + user + "', '" + date + "', '" + comment + "');";
                        //  queryString = "INSERT INTO comments (CID, IID, UID, Date, Comment, IP) VALUES(" + cid.ToString() + ", " + iid.ToString() + ", '" + user + "', '" + date + "', '" + comment + "' , INET_ATON('" + ip + "'));";
                        queryString = "INSERT INTO comments (CID, IID, UID, Date, Comment, IP, Tags) VALUES(" + cid.ToString() + ", " + iid.ToString() + ", '" + user + "', '" + date + "', '" + comment + "' , INET_ATON('" + ip + "'), '" + commentTagString + "');";

                        int retValue = dbOps.ExecuteNonQuery(queryString);
                        if (retValue > 0)
                        {
                            //  Update the NComments section in getputs.item Table.
                            queryString = "UPDATE item SET NComments=NComments+1 WHERE iid=" + iid.ToString() + ";";
                            retValue = dbOps.ExecuteNonQuery(queryString);
                            if (retValue > 0)
                            {
                                MessageLabel.Text = gui.GreenFontStart + "Comment Added." + gui.GreenFontEnd;
                                CommentTB.Text = string.Empty;
                            }
                        }
                        else
                        {
                            MessageLabel.Text = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                        }
                    }
                }
                else
                {
                    if (commentTokens.Count < _maxTokensInComment)
                    {
                        MessageLabel.Text = gui.RedFontStart + "Too few words in the comment." + gui.RedFontEnd;
                    }
                    else if(commentTokens.Count > _maxTokensInComment)
                    {
                        MessageLabel.Text = gui.RedFontStart + "Too many words in the comment." + gui.RedFontEnd;
                    }
                }
            }
        }
    }



    protected void DirectContactButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(user))
        {
            Response.Redirect(links.LoginLink, false);
        }
        else
        {
            MessageLabel.Text = string.Empty;

            string comment = CommentTB.Text.Trim();
            comment = general.EscapeCharacters(comment);

            if (string.IsNullOrEmpty(comment))
            {
                MessageLabel.Text = gui.RedFontStart + "Please enter a valid comment." + gui.RedFontEnd;
            }
            else
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string commenterEMail = string.Empty;
                string submitterEMail = string.Empty;
                
                //  Get the Commenter's and Submitter's E-Mail Addresses.
                //  user = Commenter
                //  uid = Submitter
                if (!string.IsNullOrEmpty(user) && !(string.IsNullOrEmpty(UID)))
                {                    
                    string queryString = "SELECT EMail FROM user WHERE UID='" + user + "';";
                    MySqlDataReader retList = dbOps.ExecuteReader(queryString);
                    
                    if (retList != null && retList.HasRows)
                    {
                        while (retList.Read())
                        {
                            commenterEMail = retList.GetString(0);
                        }
                        retList.Close();
                    }

                    queryString = "SELECT EMail FROM user WHERE UID='" + UID + "';";
                    retList = dbOps.ExecuteReader(queryString);

                    if (retList != null && retList.HasRows)
                    {
                        while (retList.Read())
                        {
                            submitterEMail = retList.GetString(0);
                        }
                        retList.Close();
                    }

                }

                

                if (!string.IsNullOrEmpty(commenterEMail) && !string.IsNullOrEmpty(submitterEMail))
                {
                    string subject = "[From getputs.com] : " + item.Title;
                    string message = commenterEMail + " has sent you a Private Message for a getputs Item you submitted." + gui.LineBreak;

                    message = message + gui.LineBreak;
                    //  message = message + item.Title + gui.LineBreak;

                    if (!string.IsNullOrEmpty(item.Link))
                    {
                        message = message + "<a href='" + item.Link + "'>" + item.Title + "</a>" + gui.LineBreak;
                    }
                    else
                    {
                        string link = links.ItemDetailsPageLink + "?iid=" + item.IID.ToString();
                        message = message + "<a href='" + link + "'>" + item.Title + "</a>" + gui.LineBreak;
                    }

                    message = message + gui.LineBreak;

                    if (!string.IsNullOrEmpty(comment))
                    {
                        comment = "Personal Message" + gui.LineBreak
                            + gui.GrayFontStart + "____________________________________" + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak
                            + comment + gui.LineBreak + gui.LineBreak
                            + gui.GrayFontStart + "____________________________________" + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak;
                        message = message + comment + gui.LineBreak;
                    }


                    string discussionsLink = "<a href='" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + "[getputs.com]" + "</a>";
                    message = message
                        + "For more discussions on this topic, visit " + discussionsLink + gui.LineBreak;
                    
                    //commenterEMail = "vshah@hakia.com";
                    //submitterEMail = "vshah@hakia.com";

                    bool isSent = general.SendMail(commenterEMail, submitterEMail, subject, message);
                    if (isSent)
                    {
                        //  Also update the getputs.item NEMailed Field.
                        string queryString = "UPDATE item SET NEMailed=NEMailed+1 WHERE IID=" + item.IID + ";";
                        int retInt = dbOps.ExecuteNonQuery(queryString);

                        MessageLabel.Text = gui.GreenFontStart + "EMail sent successfully." + gui.GreenFontEnd; 
                       
                    }
                    else
                    {
                        MessageLabel.Text = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                    }
                }

            }
        }
    }
}

