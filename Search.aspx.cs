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
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Net;

//  The Table getputs.item has the following schema:
//  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]

//  The Table getputs.user has the following schema:
//  getputs.item [UID, Password, Date, EMail, About, Admin, Points]

public partial class Search : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    Categories categories;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    SearchEngine searchEngine;
    ItemDisplayer itemDisplayer;

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string UID = string.Empty;
    string seperator = " | ";

    int startItem = 0;  //  Default Value;

    //  ProcessingEngine.Sort sort = ProcessingEngine.Sort.Hot;
    ProcessingEngine.Sort sort = ConfigurationManager.AppSettings["Default_Sort"] == "h" ? ProcessingEngine.Sort.Hot : ProcessingEngine.Sort.New;


    private static int itemClicks = 0;

    string query;

    SearchEngine.SearchType sType;
    string sCategory;
    SearchEngine.SearchTime sTime;

    List<Control> controls = new List<Control>();

    ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Flow;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Columns;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Categorized;

    override protected void OnInit(EventArgs e)
    {
        // Create dynamic controls here.

        //  Default Search Parameters.
        query = String.Empty;
        sType = SearchEngine.SearchType.Title;
        sCategory = "All Categories";
        sTime = SearchEngine.SearchTime.Today;

        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        categories = Categories.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        searchEngine = SearchEngine.Instance;
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

        
        LoadSearchDDL();
        GetQueryParameters();

        if (!string.IsNullOrEmpty(SearchTB.Text))
        {
            query = SearchTB.Text.Trim();
        }

        string sTypeDDLValue = SearchTypeDDL.SelectedValue;
        string sCategoryDDLValue = SearchCategoryDDL.SelectedValue;
        string sTimeDDLValue = SearchTimeDDL.SelectedValue;
        

        Control MasterPageSearchTable = this.Master.FindControl("SearchTable");
        MasterPageSearchTable.Visible = false;

        SearchTB.Focus();
        Page.Form.DefaultButton = SearchButton.ID;




        if (!string.IsNullOrEmpty(query))
        {
            SearchTB.Text = query;

            if (sType == SearchEngine.SearchType.Comments)
            {
                sType = SearchEngine.SearchType.Title;
                List<Item> itemList = searchEngine.LoadSearchResults(query, sType, sCategory, sTime);
                LoadItemTable(itemList);
            }
            else
            {
                
                //  List<Item> itemList = searchEngine.LoadSearchResults(query);
                List<Item> itemList = searchEngine.LoadSearchResults(query, sType, sCategory, sTime);
                                
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
                
                string itemTable = itemDisplayer.LoadItemTable(itemList, itemOptions, itemLayoutOptions, startItem, UID, sort, links.SearchPageLink.Replace("~\\", ""));
                ItemDiv.InnerHtml = itemTable;

            }
        }

        // 
        // CODEGEN: This call is required by the ASP.NET Web Form Designer.
        // 
        //  InitializeComponent();
        base.OnInit(e);
    }






    protected void Page_Load(object sender, EventArgs e)
    {
        ////  Default Search Parameters.
        //query = String.Empty;
        //sType = SearchEngine.SearchType.Title;
        //sCategory = "All Categories";
        //sTime = SearchEngine.SearchTime.Today;

        //dbOps = DBOperations.Instance;
        //links = Links.Instance;
        //general = General.Instance;
        //gui = GUIVariables.Instance;
        //categories = Categories.Instance;
        //engine = ProcessingEngine.Instance;
        //imageEngine = ImageEngine.Instance;
        //searchEngine = (SearchEngine)Application["searchEngine"];
        

        //seperator = gui.GrayFontStart + " | " + gui.GrayFontEnd;        

        //if (!string.IsNullOrEmpty(Request.QueryString["startItem"]))
        //{
        //    bool isStartItemInt = int.TryParse(Request.QueryString["startItem"].Trim(), out startItem);
        //    if (!isStartItemInt)
        //    {
        //        startItem = 0;
        //    }

        //    if (startItem < 0)
        //    {
        //        startItem = 0;
        //    }
        //}
        //else
        //{
        //    startItem = 0;
        //}


        //#region CookieAlreadyExists
        ////  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        //if (Request.Cookies["getputsCookie"] != null)
        //{
        //    HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
        //    UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        //}

        //if (string.IsNullOrEmpty(UID))
        //{

        //}
        //else
        //{

        //}
        ////  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        //#endregion CookieAlreadyExists




        //    LoadSearchDDL();
        //    GetQueryParameters();

        //    if (!string.IsNullOrEmpty(SearchTB.Text))
        //    {
        //        query = SearchTB.Text.Trim();
        //    }

        //        string sTypeDDLValue = SearchTypeDDL.SelectedValue;
        //        string sCategoryDDLValue = SearchCategoryDDL.SelectedValue;
        //        string sTimeDDLValue = SearchTimeDDL.SelectedValue;

                


        //        Control MasterPageSearchTable = this.Master.FindControl("SearchTable");
        //        MasterPageSearchTable.Visible = false;

        //        SearchTB.Focus();
        //        Page.Form.DefaultButton = SearchButton.ID;




        //        if (!string.IsNullOrEmpty(query))
        //        {
        //            SearchTB.Text = query;
        //            List<Item> itemList = searchEngine.LoadSearchResults(query);
        //            LoadItemTable(itemList);
        //        }
            
        
        
        
    }





    protected void SearchButton_Click(object sender, EventArgs e)
    {
        //  if (Page.IsPostBack)        
        {
            string query = SearchTB.Text.Trim();

            if (string.IsNullOrEmpty(query))
            {

            }
            else
            {
                string sTypeDDLValue = SearchTypeDDL.SelectedValue;
                string sCategoryDDLValue = SearchCategoryDDL.SelectedValue;
                string sTimeDDLValue = SearchTimeDDL.SelectedValue;

                //  Log the Search Query.
                general.LogQuery(query, UID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), general.GetIP(Request));

               

                //  ItemDiv.Controls.Clear();                
                                
                //  Response.Redirect(links.SearchPageLink, false);
                //  Response.Redirect(links.SearchPageLink + "?query=" + query);
                Response.Redirect(links.SearchPageLink + "?query=" + query + "&sType=" + sTypeDDLValue + "&category=" + sCategoryDDLValue + "&sTime=" + sTimeDDLValue, true);               

            }
        }

        ////  if (!Page.IsPostBack)
        //{
        //    string query = SearchTB.Text.Trim();

        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        //  Response.Redirect(links.SearchPageLink + "?query=" + query, false);

        //        ItemDiv.Controls.Clear();
        //        List<Item> itemList = searchEngine.LoadSearchResults(query);
        //        LoadItemTable(itemList);

        //    }
        //}
    }

    
    private void LoadSearchDDL()
    {
        if (SearchCategoryDDL.Items.Count == 0)
        {
            ////  Populate Type DDL 
            //SearchTypeDDL.Items.Add(new ListItem("All", "All"));
            //SearchTypeDDL.Items.Add(new ListItem("Title", "Title"));
            //SearchTypeDDL.Items.Add(new ListItem("Links", "Links"));
            //SearchTypeDDL.Items.Add(new ListItem("Comments", "Comments"));
            
            //SearchTypeDDL.SelectedIndex = 1;    //  By Default, Search Inside Titles 

            //  Populate Category DDL
            SearchCategoryDDL.Items.Add(new ListItem("All Categories", "All Categories"));

            if (categories.CategoriesList != null)
            {
                for (int i = 0; i < categories.CategoriesList.Count; i++)
                {
                    SearchCategoryDDL.Items.Add(new ListItem(categories.CategoriesList[i], categories.CategoriesList[i]));
                }
            }

            SearchCategoryDDL.SelectedIndex = 0;    //  By Default, Search Inside All the Categories. 

            ////  Populate Time DDL
            //SearchTimeDDL.Items.Add(new ListItem("Today", "Today"));
            //SearchTimeDDL.Items.Add(new ListItem("This Week", "Week"));
            //SearchTimeDDL.Items.Add(new ListItem("This Month", "Month"));
            //SearchTimeDDL.Items.Add(new ListItem("This Year", "Year"));

            //SearchTimeDDL.SelectedIndex = 1;    //  By Default, Search inside Items submitted this week.
        }
    }



    private void GetQueryParameters()
    {        
        //  Default Search Parameters.
        query = String.Empty;
        sType = SearchEngine.SearchType.Title;
        sCategory = "All Categories";
        sTime = SearchEngine.SearchTime.Today;
        
        if (!string.IsNullOrEmpty(Request.QueryString["query"]))
        {
            query = Convert.ToString(Request.QueryString["query"]).Trim();
        }       

        if (!string.IsNullOrEmpty(Request.QueryString["sType"]))
        {
            string sTypeStr = Convert.ToString(Request.QueryString["sType"]).Trim().ToLower();

            //  public enum SearchType    {        All,        Title,        Links,        Comments    }

            switch (sTypeStr)
            {
                case "all":
                    sType = SearchEngine.SearchType.All;
                    break;
                case "title":
                    sType = SearchEngine.SearchType.Title;
                    break;
                case "links":
                    sType = SearchEngine.SearchType.Links;
                    break;
                case "comments":
                    sType = SearchEngine.SearchType.Comments;
                    break;
                default:
                    sType = SearchEngine.SearchType.All;
                    break;
            }
                
        }
        
        if (!string.IsNullOrEmpty(Request.QueryString["category"]))
        {
            sCategory = Convert.ToString(Request.QueryString["category"]).Trim();
            if (!categories.CategoriesList.Contains(sCategory))
            {
                if(sCategory != "All Categories")
                {
                    sCategory = "All Categories";
                }
            }
        }
        
        if (!string.IsNullOrEmpty(Request.QueryString["sTime"]))
        {
            string sTimeStr = Convert.ToString(Request.QueryString["sTime"]).Trim().ToLower();
            
            //  public enum SearchTime    {        Today,        Week,        Month,        Year    }
            
            switch (sTimeStr)
            {
                case "today":
                    sTime = SearchEngine.SearchTime.Today;
                    break;
                case "week":
                    sTime = SearchEngine.SearchTime.Week;
                    break;
                case "month":
                    sTime = SearchEngine.SearchTime.Month;
                    break;
                case "year":
                    sTime = SearchEngine.SearchTime.Year;
                    break;
                default:
                    sTime = SearchEngine.SearchTime.Week;
                    break;
            }
        }

        SearchTypeDDL.ClearSelection();
        SearchCategoryDDL.ClearSelection();
        SearchTimeDDL.ClearSelection();

        SearchTypeDDL.Items.FindByValue(sType.ToString()).Selected = true;
        SearchCategoryDDL.Items.FindByValue(sCategory).Selected = true;
        SearchTimeDDL.Items.FindByValue(sTime.ToString()).Selected = true;       
    }





    private void LoadItemTable(List<Item> itemList)
    {
        if (itemList != null && itemList.Count > 0)
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

                HyperLink nextPageHL = new HyperLink();
                nextPageHL.Text = "Next";
                nextPageHL.CssClass = "CSS_Next";
                nextPageHL.NavigateUrl = links.FrontPageLink + "?sort=" + sortStr + "&startItem=" + nextItem.ToString();

                ItemDiv.Controls.Add(nextPageHL);
            }

            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            ItemDiv.Controls.Add(new LiteralControl(gui.LineBreak));
        }
        else
        {
            ItemDiv.Controls.Add(new LiteralControl("Your Search did not match any Item."));

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

        //  int iid = int.Parse(iidStr);

        Response.Redirect(links.SendItemMailPageLink + "?IID=" + iidStr, false);
    }

}

