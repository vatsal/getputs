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


//  The Table getputs.item has the following schema:
//  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]

//  The Table getputs.user has the following schema:
//  getputs.item [UID, Password, Date, EMail, About, Admin, Points]


public partial class Category : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    Categories categories;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    ItemDisplayer itemDisplayer;
    

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string UID;
    string seperator = " | ";

    int startItem = 0;  //  Default Value;
    ProcessingEngine.Sort sort = ProcessingEngine.Sort.Hot;

    private static int itemClicks = 0;
    string requestedCategory = string.Empty;

    ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Flow;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Columns;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Categorized;

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        categories = Categories.Instance;
        engine = ProcessingEngine.Instance;        
        imageEngine = ImageEngine.Instance;
        itemDisplayer = ItemDisplayer.Instance;

        seperator = gui.Seperator;

        //  requestedTag = Request.QueryString["category"].Trim();
        requestedCategory = Request.QueryString.Get("category");        

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

        if (string.IsNullOrEmpty(requestedCategory))
        {
            LoadGenericCategoryTable();
        }
        else if (!(categories.CategoriesList.Contains(requestedCategory) || categories.CategoriesList.Contains(requestedCategory.ToLower())))
        {
            MessageLabel.Text = gui.RedFontStart + "Invalid Category" + gui.RedFontEnd;
        }
        else
        {
            string currentCategory = requestedCategory;
            if (imageEngine.IsIconsOn)
            {
                string iconLocation = imageEngine.LoadIconLocation(currentCategory);
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    System.Web.UI.WebControls.Image icon = new System.Web.UI.WebControls.Image();
                    //  icon.ImageUrl = links.DomainLink + iconLocation;
                    icon.ImageUrl = iconLocation;
                    icon.ToolTip = currentCategory;
                    CategoryDiv.Controls.Add(icon);
                    CategoryDiv.Controls.Add(new LiteralControl(" "));
                }
            }  
            MessageLabel.Text = gui.GreenFontStart + requestedCategory + gui.GreenFontEnd;            
            //  MessageLabel.Text = gui.WhiteFontStart + requestedCategory + gui.WhiteFontEnd;            
            LoadCategoryTable(requestedCategory, sort);
        }
    }

    private void LoadGenericCategoryTable()
    {
        MessageLabel.Text = "Categories";
                
        //  ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));                

        for (int i = 0; i < categories.CategoriesList.Count; i++)
        {
            string currentCategory = categories.CategoriesList[i];

            if (imageEngine.IsIconsOn)
            {                
                string iconLocation = imageEngine.LoadIconLocation(currentCategory);
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    System.Web.UI.WebControls.Image icon = new System.Web.UI.WebControls.Image();
                    //  icon.ImageUrl = links.DomainLink + iconLocation;
                    icon.ImageUrl = iconLocation;
                    //  icon.ImageUrl = Request.ApplicationPath + iconLocation;
                    icon.ToolTip = currentCategory;
                    ItemDiv.Controls.Add(icon);
                    ItemDiv.Controls.Add(new LiteralControl(" "));
                }
            }

            HyperLink categoryHL = new HyperLink();
            categoryHL.NavigateUrl = links.CategoryPageLink + "?category=" + currentCategory;
            categoryHL.Text = currentCategory;
            categoryHL.CssClass = "CSS_NavigationTableLinkButtons";

            ItemDiv.Controls.Add(categoryHL);
            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            
        }
    }

    private void LoadCategoryTable(string requestedTag, ProcessingEngine.Sort sortType)
    {
        List<Item> itemList = new List<Item>();


        //  select * from item order by DATE_FORMAT(date, '%Y-%m-%d %k:%i:%S') desc;
        //  string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category FROM getputs.item WHERE Spam = 0 ORDER BY Date DESC";
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Clicks, NSaved, NEMailed, Tags, AvgRating, NRated  FROM item WHERE Category = '" + requestedTag + "' AND Spam = 0 ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S') DESC LIMIT " + engine.MaxItems.ToString() + ";";
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                Item item = new Item();
                item.IID = Convert.ToInt32(retList["IID"]);
                item.Title = Convert.ToString(retList["Title"]);
                item.Link = Convert.ToString(retList["Link"]);
                item.Text = Convert.ToString(retList["Text"]);
                item.Date = Convert.ToString(retList["Date"]);
                item.UID = Convert.ToString(retList["UID"]);
                item.NComments = Convert.ToInt32(retList["NComments"]);
                item.Category = requestedTag;
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

                itemList.Add(item);
            }
            retList.Close();
        }

        if (sortType == ProcessingEngine.Sort.Hot)
        {
            itemList = engine.SortItems(itemList, sortType);
        }

        //  ItemDiv.InnerHtml = LoadItems(itemList);
        
        //  LoadItemTable(itemList);

        ItemDisplayer.ShowItemsOptions itemOptions = ItemDisplayer.ShowItemsOptions.ShowUIDLink
            | ItemDisplayer.ShowItemsOptions.ShowTime            
            | ItemDisplayer.ShowItemsOptions.ShowSaveLink
            | ItemDisplayer.ShowItemsOptions.ShowEMailLink
            | ItemDisplayer.ShowItemsOptions.ShowCommentsLink
            | ItemDisplayer.ShowItemsOptions.ShowImage
            | ItemDisplayer.ShowItemsOptions.ShowRatings
            | ItemDisplayer.ShowItemsOptions.ShowTags
            | ItemDisplayer.ShowItemsOptions.CountClicks
            | ItemDisplayer.ShowItemsOptions.ShowPreviousNextLinks;
        
        string itemTable = itemDisplayer.LoadItemTable(itemList, itemOptions, itemLayoutOptions, startItem, UID, sort, links.CategoryPageLink.Replace("~\\", "") + "?category=" + requestedCategory);
        ItemDiv.InnerHtml = itemTable;

    }


    private void LoadItemTable(List<Item> itemList)
    {       

        string commentString = "discuss";
        
        if (itemList == null || itemList.Count == 0)
        {
            string submitLink = "<a href='Submit.aspx' class='CSS_LinkItem'>Submit Page</a>";

            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            ItemDiv.Controls.Add(new LiteralControl("No items found in this category."));
            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            ItemDiv.Controls.Add(new LiteralControl("To post items in the " + requestedCategory + " Category, visit the " + submitLink)); 
            
        }
        else
        {
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
                            newWindow = "<a title='Open Link in a New Tab' style='vertical-align: middle' target='_blank' text-decoration='none' href='" + links.ItemDetailsPageLink.Replace("~\\","") + "?iid=" + item.IID.ToString() + "'><img border= '0' src='" + iconLocation + "' alt='New Window'/></a> ";
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

                //  //No need to show a category for this page.
                //if (!string.IsNullOrEmpty(item.Category))
                //{
                //    LinkButton categoryLB = new LinkButton();
                //    categoryLB.Click += new EventHandler(categoryLB_Click);
                //    categoryLB.Text = item.Category;
                //    categoryLB.CssClass = "CSS_Category";  
                //      //    categoryLB.Font.Underline = false;
                //      //    categoryLB.Font.Size = FontUnit.Small;
                //      //    categoryLB.ForeColor = System.Drawing.Color.Silver;
                //    categoryLB.Attributes.Add("category", item.Category);

                //    ItemDiv.Controls.Add(categoryLB);

                //    ItemDiv.Controls.Add(new LiteralControl(seperator));
                //}

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


                if (engine.IsDebug)
                {
                    string debugMessage = string.Empty;
                    ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));

                    if (engine.ClickCount.ContainsKey(item.IID))
                    {
                        debugMessage += "Dictionary Clicks: " + Convert.ToString(engine.ClickCount[item.IID]) + seperator;
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

            }   //  ItemList Loop Ends.


            string sortStr = sort == ProcessingEngine.Sort.New ? "n" : "h";

            //  Add the "Previous" Link if required.
            if (startItem - engine.ItemsPerPage >= 0)
            {
                int previousItem = startItem - engine.ItemsPerPage;

                HyperLink previousPageHL = new HyperLink();
                previousPageHL.Text = "Previous";
                previousPageHL.CssClass = "CSS_Next";
                previousPageHL.NavigateUrl = links.CategoryPageLink + "?category=" + requestedCategory + "&sort=" + sortStr + "&startItem=" + previousItem.ToString();

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
                nextPageHL.NavigateUrl = links.CategoryPageLink + "?category=" + requestedCategory + "&sort=" + sortStr + "&startItem=" + nextItem.ToString();

                ItemDiv.Controls.Add(nextPageHL);
            }

        }
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

        int iid = int.Parse(iidStr);
        Response.Redirect(links.SendItemMailPageLink + "?IID=" + iid.ToString(), false);
    }

   

}

