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
using System.Text.RegularExpressions;
using System.Text;

public partial class getputs_A : System.Web.UI.MasterPage
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

    int sessionTimeoutMinutes = int.Parse(ConfigurationManager.AppSettings["sessionTimeoutMinutes"]);
    int isDoPasswordHashing = int.Parse(ConfigurationManager.AppSettings["isDoPasswordHashing"]);

    string copyrightText = ConfigurationManager.AppSettings["CopyrightText"];

    string requestedTag = string.Empty;

    string UID = string.Empty;

    Random random = new Random();
        
    override protected void OnInit(EventArgs e)
    {
        try
        {
            links = Links.Instance;
            gui = GUIVariables.Instance;
            dbOps = DBOperations.Instance;
            categories = Categories.Instance;
            log = Logger.Instance;
            engine = ProcessingEngine.Instance;
            general = General.Instance;
            imageEngine = ImageEngine.Instance;
            gui = GUIVariables.Instance;
            itemDisplayer = ItemDisplayer.Instance;

            if (Request != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["category"]))
                {
                    requestedTag = Request.QueryString["category"].Trim();
                }
            }

            #region CookieAlreadyExists
            //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

            if (Request.Cookies["getputsCookie"] != null)
            {
                HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
                UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
            }



            //TopAboutLabel.Text = "getputs is a utility for discovering, sharing and recommending user generated content."
            //    + gui.LineBreak
            //    + "Read/Post the latest/hottest news and classified submissions, ask queries, discuss your views!";

            TopAboutLabel.Text = gui.GrayFontStart 
                + "getputs is a utility for discovering, sharing, and recommending news."
                + gui.GrayFontEnd;




            if (string.IsNullOrEmpty(UID))
            {
                TopAboutTable.Visible = true;

                //  If the Page is SLogin.aspx, then the LoginTable will not be visible.
                //  Else it is going to be visible in anycase.
                //  LoginTable.Visible = true;

                UsernameTB.Focus();
                Page.Form.DefaultButton = LoginButton.ID;

                //LoginHL.Visible = true;
                //RegisterHL.Visible = true;

                //  SubmitHL.Visible = false;
                SubmitHL.Visible = true;
                SavedHL.Visible = false;
                UserAccountHL.Visible = false;
                MyNewsHL.Visible = false;

                LogoutHL.Visible = false;

                UserWelcomeLabel.Text = "";

                PostItDiv.Visible = true;

            }
            else
            {
                TopAboutTable.Visible = false;

                LoginTable.Visible = false;

                //LoginHL.Visible = false;
                //RegisterHL.Visible = false;

                SubmitHL.Visible = true;
                SavedHL.Visible = true;
                UserAccountHL.Visible = true;
                MyNewsHL.Visible = true;

                LogoutHL.Visible = true;
                UserAccountHL.NavigateUrl = links.UserDetailsPageLink + "?UID=" + UID;

                //  UserWelcomeLabel.Text = gui.GrayFontStart + "Welcome " + UID + gui.GrayFontEnd;
                UserWelcomeLabel.Text = gui.BoldFontStart + gui.GreenFontStart + "Welcome " + UID + gui.GreenFontEnd + gui.BoldFontEnd;

                PostItDiv.Visible = false;

                //  Vatsal Shah | 2009-08-08 | LogVisitor() Throws a lot of errors. Thus commented for now.
                //  LogVisitor(UID);
            }

            //  PopularCategoriesLabel.Visible = false;
            //  CategoryDiv.Visible = false;
            //  MoreHL.Visible = false;



            //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
            #endregion CookieAlreadyExists

            RandomNewsHL.Text = gui.BlueFontStart + "Bored? " + gui.BlueFontEnd
                //  + gui.LineBreak
                + gui.GreenFontStart + "Read random stuff" + gui.GreenFontEnd;
                

            CopyrightLabel.Text = gui.SmallFontStart + gui.GrayFontStart
                + copyrightText
                + gui.GrayFontEnd + gui.SmallFontEnd;


            #region RandomQuoteFact
            Random randomQuoteFact = new Random();
            int randomQuoteFactInt = randomQuoteFact.Next(0, 100);

            if (randomQuoteFactInt % 2 == 0)    //  Serve Facts
            {
                gp.Files.FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"];
                string randomFact = gp.Files.FactDB.Instance.GetRandomFact();
                if (!string.IsNullOrEmpty(randomFact))
                {
                    QuoteFactLiteral.Text = gui.BoldFontStart + gui.GreenFontStart + "getputs Fact: " + gui.GreenFontEnd + gui.BoldFontEnd + gui.LineBreak
                        + gui.GrayFontStart + randomFact + gui.GrayFontEnd;
                }
            }
            else    //  Serve Quotes
            {
                gp.Files.FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"];
                string randomQuote = gp.Files.QuoteDB.Instance.GetRandomQuote();
                if (!string.IsNullOrEmpty(randomQuote))
                {
                    randomQuote = randomQuote.Replace("|", gui.LineBreak + " - <i>") + "</i>";
                    QuoteFactLiteral.Text = gui.BoldFontStart + gui.GreenFontStart + "getputs Quote: " + gui.GreenFontEnd + gui.BoldFontEnd + gui.LineBreak
                        + gui.GrayFontStart + randomQuote + gui.GrayFontEnd;
                }
            }
            #endregion RandomQuoteFact


            #region RandomTip

            gp.Files.FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"];
            string randomTip = gp.Files.TipsDB.Instance.GetRandomTip();
            if (!string.IsNullOrEmpty(randomTip))
            {
                //  TipLiteral.Text = gui.MediumFontStart + gui.GrayFontStart + randomTip + gui.GrayFontEnd + gui.MediumFontEnd;
                TipLiteral.Text = randomTip;
            }

            #endregion RandomTip

            GetNavigationTableURL();
            LoadCategoryTable(UID);
            AddMouseEffects();


            //  Load the Carousels only for the Front-Page (Default.aspx)
            //  For all other pages CarouselDiv would be invisible.
            CarouselDiv.Visible = false;
            //  Make the TopAboutTable Invisible for any page other than the Front-Page (Default.aspx)
            TopAboutTable.Visible = false;

            StringBuilder strScript = new StringBuilder();
            strScript.Append("var itemImageList = [];");
            strScript.Append("var itemContentList = [];");
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "CarouselJavaScript", strScript.ToString(), true);





            
            
            //List<Item> itemList = engine.LoadItemDB(ProcessingEngine.Sort.New);
            //if (itemList != null && itemList.Count > 0)
            //{
            //    LoadItemImageCarousel(itemList);
            //    LoadItemNewsCarousel(itemList);
            //}


            ////  string pagename = System.IO.Path.GetFileName(Request.ServerVariables["SCRIPT_NAME"]);
            //string pagename = Path.GetFileName(System.Web.HttpContext.Current.Request.Url.AbsolutePath);
            //if (links.FrontPageLink.EndsWith(pagename))     //  Load the Carousels only for the Front-Page (Default.aspx)
            //{
            //    List<Item> itemList = engine.LoadItemDB(ProcessingEngine.Sort.New);
            //    if (itemList != null && itemList.Count > 0)
            //    {
            //        LoadItemImageCarousel(itemList);
            //        LoadItemNewsCarousel(itemList);
            //    }

            //}
            //else    //  Do not show the CarouselDiv.
            //{
            //    CarouselDiv.Visible = false;

            //    //  Instantiate Empty JavaScriptLists so as to avoid Client Side Exceptions.
            //    StringBuilder strScript = new StringBuilder();
            //    strScript.Append("var itemImageList = [];");
            //    strScript.Append("var itemContentList = [];");
            //    Page.ClientScript.RegisterClientScriptBlock(GetType(), "CarouselJavaScript", strScript.ToString(), true);

            //    //  Make the TopAboutTable Invisible for any page other than the Front-Page (Default.aspx)
            //    TopAboutTable.Visible = false;
            //}


            //  SearchTB.Attributes.Add("onkeypress", "if ((event.which ? event.which : event.keyCode) == 13){var sendElem = document.getElementById(\"SearchButton\"); if(!sendElem.disabled) DoSearch(); }");
            //  SearchTB.Attributes.Add("onkeypress", "if ((event.which ? event.which : event.keyCode) == 13){document.getElementById('SearchButton').click()}");
            //  SearchTB.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + SearchButton.ID + "').click();return false;}} else {return true}; ");

        }
        catch (Exception ex)
        {
            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Error in getputs.master: OnInit() Method: ");
                log.Log(ex);
            }
        }
    }




    private void AddMouseEffects()
    {
        //  hl.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        //  hl.Attributes.Add("onmouseout", "this.style.textDecoration='none'");


        AboutHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        AboutHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        BlogHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        BlogHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        FAQHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        FAQHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        FeedbackHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        FeedbackHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        ContactHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        ContactHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        PrivacyPolicyHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        PrivacyPolicyHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        RssHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        RssHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");

        TopAboutHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        TopAboutHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");

        NewUserHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        NewUserHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        
        ForgotPasswordHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        ForgotPasswordHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");    

        HotHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        HotHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        NewestHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        NewestHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        UserAccountHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        UserAccountHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        MyNewsHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        MyNewsHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        SubmitHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        SubmitHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
        LogoutHL.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
        LogoutHL.Attributes.Add("onmouseout", "this.style.textDecoration='none'");
    }




    protected void Page_Load(object sender, EventArgs e)
    {       
        //try
        //{
        //    links = Links.Instance;
        //    gui = GUIVariables.Instance;
        //    dbOps = DBOperations.Instance;
        //    categories = Categories.Instance;
        //    log = Logger.Instance;
        //    engine = ProcessingEngine.Instance;
        //    general = General.Instance;
        //    imageEngine = ImageEngine.Instance;

        //    if (Request != null)
        //    {
        //        if (!string.IsNullOrEmpty(Request.QueryString["category"]))
        //        {
        //            requestedTag = Request.QueryString["category"].Trim();
        //        }
        //    }

        //    #region CookieAlreadyExists
        //    //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        //    if (Request.Cookies["getputsCookie"] != null)
        //    {
        //        HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
        //        UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        //    }



        //    //TopAboutLabel.Text = "getputs is a utility for discovering, sharing and recommending user generated content."
        //    //    + gui.LineBreak
        //    //    + "Read/Post the latest/hottest news and classified submissions, ask queries, discuss your views!";

        //    TopAboutLabel.Text = "getputs is a utility for discovering, sharing and recommending user generated content.";


        //    if (string.IsNullOrEmpty(UID))
        //    {
        //        TopAboutTable.Visible = true;

        //        //  If the Page is SLogin.aspx, then the LoginTable will not be visible.
        //        //  Else it is going to be visible in anycase.
        //        //  LoginTable.Visible = true;

        //        UsernameTB.Focus();
        //        Page.Form.DefaultButton = LoginButton.ID;

        //        //LoginHL.Visible = true;
        //        //RegisterHL.Visible = true;

        //        //  SubmitHL.Visible = false;
        //        SubmitHL.Visible = true;
        //        SavedHL.Visible = false;
        //        UserAccountHL.Visible = false;
        //        MyNewsHL.Visible = false;

        //        LogoutHL.Visible = false;

        //        UserWelcomeLabel.Text = "";

        //    }
        //    else
        //    {
        //        TopAboutTable.Visible = false;

        //        LoginTable.Visible = false;

        //        //LoginHL.Visible = false;
        //        //RegisterHL.Visible = false;

        //        SubmitHL.Visible = true;
        //        SavedHL.Visible = true;
        //        UserAccountHL.Visible = true;
        //        MyNewsHL.Visible = true; 

        //        LogoutHL.Visible = true;


        //        UserWelcomeLabel.Text = gui.GrayFontStart + "Welcome " + UID + gui.GrayFontEnd;

        //        LogVisitor(UID);
        //    }

        //    PopularCategoriesLabel.Visible = false;
        //    CategoryDiv.Visible = false;
        //    MoreHL.Visible = false;
        //    LoadCategoryTable(UID);

        //    //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        //    #endregion CookieAlreadyExists

        //    RandomArticleLinkButton.Text = gui.BlueFontStart + "Bored?" + gui.BlueFontEnd
        //        + gui.LineBreak 
        //        + gui.GreenFontStart + "Read a random article" + gui.GreenFontEnd;

        //    CopyrightLabel.Text = gui.SmallFontStart + gui.GrayFontStart
        //        + copyrightText
        //        + gui.GrayFontEnd + gui.SmallFontEnd;


        //    //  SearchTB.Attributes.Add("onkeypress", "if ((event.which ? event.which : event.keyCode) == 13){var sendElem = document.getElementById(\"SearchButton\"); if(!sendElem.disabled) DoSearch(); }");
        //    //  SearchTB.Attributes.Add("onkeypress", "if ((event.which ? event.which : event.keyCode) == 13){document.getElementById('SearchButton').click()}");
        //    //  SearchTB.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + SearchButton.ID + "').click();return false;}} else {return true}; ");

        //}
        //catch (Exception ex)
        //{

        //}

        

    }

    private void LoadCategoryTable(string UID)
    {
        if (string.IsNullOrEmpty(UID))    //  User has not logged in.
        {
            #region Popular Categories
            //if (engine.PopularCategories != null && engine.PopularCategories.Count > 0)
            //{
            //    for (int i = 0; i < engine.PopularCategories.Count; i++)
            //    {
            //        string currentCategory = engine.PopularCategories[i];

            //        if (imageEngine.isIconsOn)
            //        {                        
            //            string iconLocation = imageEngine.LoadIconLocation(currentCategory);
            //            if (!string.IsNullOrEmpty(iconLocation))
            //            {
            //                System.Web.UI.WebControls.Image icon = new System.Web.UI.WebControls.Image();                            
            //                //  icon.ImageUrl = links.DomainLink + iconLocation;
            //                icon.ImageUrl = iconLocation;
            //                //  icon.ImageUrl = Request.ApplicationPath + iconLocation;                            
            //                icon.ToolTip = currentCategory;                            
            //                CategoryDiv.Controls.Add(icon);
            //                CategoryDiv.Controls.Add(new LiteralControl(" "));
            //            }
            //        }                    

            //        HyperLink hl = new HyperLink();
            //        hl.NavigateUrl = links.CategoryPageLink + "?category=" + currentCategory;
            //        hl.Text = currentCategory;                    
            //        hl.CssClass = "CSS_NavigationTableLinkButtons";

            //        hl.Attributes.Add("onmouseover", "this.style.backgroundColor='gainsboro'");
            //        hl.Attributes.Add("onmouseout", "this.style.backgroundColor='white'");

            //        CategoryDiv.Controls.Add(hl);
            //        CategoryDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            //    }

            //    PopularCategoriesLabel.Visible = false;
            //    CategoryDiv.Visible = true;
            //    MoreHL.Visible = true;
            //}
            #endregion Popular Categories



            #region All Categories

            if (categories.CategoriesList != null && categories.CategoriesList.Count > 0)
            {
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
                            CategoryDiv.Controls.Add(icon);
                            CategoryDiv.Controls.Add(new LiteralControl(" "));
                        }

                        //  Using Sprites                            
                        //  CategoryDiv.Controls.Add(new LiteralControl(imageEngine.LoadSpriteIcon(currentCategory) + gui.HTMLSpace));

                    }

                    HyperLink hl = new HyperLink();
                    hl.NavigateUrl = links.CategoryPageLink + "?category=" + currentCategory;
                    hl.Text = currentCategory;
                    hl.CssClass = "CSS_NavigationTableLinkButtons";

                    //  hl.Attributes.Add("onmouseover", "this.style.backgroundColor='gainsboro'");
                    //  hl.Attributes.Add("onmouseout", "this.style.backgroundColor='white'");
                                       
                    hl.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
                    hl.Attributes.Add("onmouseout", "this.style.textDecoration='none'");


                    CategoryDiv.Controls.Add(hl);
                    CategoryDiv.Controls.Add(new LiteralControl(gui.LineBreak));
                }

                //  PopularCategoriesLabel.Visible = false;
                //  CategoryDiv.Visible = true;
                //  MoreHL.Visible = true;
            }
            #endregion All Categories

        }
        else    //  User has logged in.
        {
            #region Popular Categories
            //if (engine.PopularCategories != null && engine.PopularCategories.Count > 0)
            //{
            //    for (int i = 0; i < engine.PopularCategories.Count; i++)
            //    {
            //        string currentCategory = engine.PopularCategories[i];

            //        if (imageEngine.isIconsOn)
            //        {
            //            string iconLocation = imageEngine.LoadIconLocation(currentCategory);
            //            if (!string.IsNullOrEmpty(iconLocation))
            //            {
            //                System.Web.UI.WebControls.Image icon = new System.Web.UI.WebControls.Image();
            //                //  icon.ImageUrl = links.DomainLink + iconLocation;
            //                icon.ImageUrl = iconLocation;
            //                //  icon.ImageUrl = Request.ApplicationPath + iconLocation;                            
            //                icon.ToolTip = currentCategory;
            //                CategoryDiv.Controls.Add(icon);
            //                CategoryDiv.Controls.Add(new LiteralControl(" "));
            //            }
            //        }          

            //        HyperLink hl = new HyperLink();
            //        hl.NavigateUrl = links.CategoryPageLink + "?category=" + currentCategory;
            //        hl.Text = currentCategory;                    
            //        hl.CssClass = "CSS_NavigationTableLinkButtons";

            //        hl.Attributes.Add("onmouseover", "this.style.backgroundColor='gainsboro'");
            //        hl.Attributes.Add("onmouseout", "this.style.backgroundColor='white'");

            //        CategoryDiv.Controls.Add(hl);
            //        CategoryDiv.Controls.Add(new LiteralControl(gui.LineBreak));
            //    }
            //    PopularCategoriesLabel.Visible = false;
            //    CategoryDiv.Visible = true;
            //    MoreHL.Visible = true;
            //}
            #endregion Popular Categories


            #region All Categories

            if (categories.CategoriesList != null && categories.CategoriesList.Count > 0)
            {
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
                            CategoryDiv.Controls.Add(icon);
                            CategoryDiv.Controls.Add(new LiteralControl(" "));
                        }

                        //  Using Sprites                            
                        //  CategoryDiv.Controls.Add(new LiteralControl(imageEngine.LoadSpriteIcon(currentCategory) + gui.HTMLSpace));

                    }

                    HyperLink hl = new HyperLink();
                    hl.NavigateUrl = links.CategoryPageLink + "?category=" + currentCategory;
                    hl.Text = currentCategory;
                    hl.CssClass = "CSS_NavigationTableLinkButtons";

                    //  hl.Attributes.Add("onmouseover", "this.style.backgroundColor='gainsboro'");
                    //  hl.Attributes.Add("onmouseout", "this.style.backgroundColor='white'");

                    hl.Attributes.Add("onmouseover", "this.style.textDecoration='underline'");
                    hl.Attributes.Add("onmouseout", "this.style.textDecoration='none'");


                    CategoryDiv.Controls.Add(hl);
                    CategoryDiv.Controls.Add(new LiteralControl(gui.LineBreak));
                }

                //  PopularCategoriesLabel.Visible = false;
                //  CategoryDiv.Visible = true;
                //  MoreHL.Visible = true;
            }

            #endregion All Categories
        }

    }

    private void LogVisitor(string UID)
    {
        string ipAddress = general.GetIP(this.Request);

        if (string.IsNullOrEmpty(UID))
        {
            UID = "PugWash";
        }


        string hostName = Dns.GetHostEntry(ipAddress).HostName;
        string date = DateTime.Now.ToString();
        string url = Request.Url.ToString();

        //  Add Visitor to the DB.
    }


    //  //  The FrontPageHL has been replaced by the Logo
    //protected void FrontPageHL_Click(object sender, EventArgs e)
    //{        
    //    Response.Redirect(links.FrontPageLink, false);
    //}


    private void GetNavigationTableURL()
    {
        string pagename = System.IO.Path.GetFileName(Request.ServerVariables["SCRIPT_NAME"]);

        string hotLink = Request.Url.ToString();
        string newLink = Request.Url.ToString();

        Regex startItemRegex = new Regex(@"startItem=\d+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        if (links.FrontPageLink.EndsWith(pagename)
            || links.NewestPageLink.EndsWith(pagename)
            || links.MyNewsPageLink.EndsWith(pagename)
            || links.SavedPageLink.EndsWith(pagename)
            || links.RatedPageLink.EndsWith(pagename)
            || links.CategoryPageLink.EndsWith(pagename)
            || links.AutoTagPageLink.EndsWith(pagename))
        {
            #region hotLink
            if (hotLink.Contains("sort=n"))
            {
                hotLink = hotLink.Replace("sort=n", "sort=h");
            }
            else if (hotLink.Contains("sort=h"))
            {

            }
            else
            {
                if (hotLink.Contains("?"))
                {
                    hotLink = hotLink + "&sort=h";
                }
                else
                {
                    hotLink = hotLink + "?sort=h";
                }
            }

            if (startItemRegex.IsMatch(hotLink))
            {
                hotLink = startItemRegex.Replace(hotLink, "startItem=0");
            }

            #endregion hotLink



            #region newLink
            if (newLink.Contains("sort=n"))
            {

            }
            else if (newLink.Contains("sort=h"))
            {
                newLink = newLink.Replace("sort=h", "sort=n");
            }
            else
            {
                if (newLink.Contains("?"))
                {
                    newLink = newLink + "&sort=n";
                }
                else
                {
                    newLink = newLink + "?sort=n";
                }
            }

            if (startItemRegex.IsMatch(newLink))
            {
                newLink = startItemRegex.Replace(newLink, "startItem=0");
            }
            #endregion newLink

        }
        else
        {
            hotLink = links.FrontPageLink + "?sort=h";
            newLink = links.FrontPageLink + "?sort=n";
        }
        HotHL.NavigateUrl = hotLink;
        NewestHL.NavigateUrl = newLink;




        #region RandomArticleLink
        //  The whole point of random article is that the user does not know what he will se next.
        //  May be that's why a linkbutton will be better then a hyperlink (since the linkbutton hides the link)

        //bool isDone = false;
        //string randomLink = string.Empty;

        ////string queryString = "SELECT COUNT(*) FROM item WHERE Spam = 0;";
        ////int retInt = dbOps.ExecuteScalar(queryString);

        //int retInt = engine.ItemRowCount;

        //if (retInt > 0)
        //{
        //    int rowCount = retInt;
        //    int rowNumber = random.Next(0, rowCount);

        //    while (!isDone)
        //    {
        //        string queryString = "SELECT Link FROM item WHERE Spam = 0 AND IID = " + rowNumber + ";";
        //        MySqlDataReader retList = dbOps.ExecuteReader(queryString);

        //        if (retList != null && retList.HasRows)
        //        {
        //            //   DB Hit.                     
        //            while (retList.Read())
        //            {
        //                randomLink = Convert.ToString(retList["Link"]);
        //            }
        //            retList.Close();
        //            isDone = true;
        //        }
        //        else
        //        {
        //            //   DB Miss. Select another random number.
        //            rowNumber = random.Next(0, rowCount);
        //        }
        //    }

        //    if (isDone && !string.IsNullOrEmpty(randomLink))
        //    {
        //        //  Response.Redirect(randomLink, true);                
        //        RandomArticleLinkButton.NavigateUrl = randomLink;
        //    }
        //}
        //else
        //{
        //    //  Not decided what to do.
        //    //  May be redirect back to getputs.
        //    RandomArticleLinkButton.NavigateUrl = links.FrontPageLink;
        //}

        #endregion RandomArticleLink


    }


    protected void HotPageHL_Click(object sender, EventArgs e)
    {
        //if (string.IsNullOrEmpty(requestedTag) || !(tags.TagsList.Contains(requestedTag) || tags.TagsList.Contains(requestedTag.ToLower())))
        //{
        //    Response.Redirect(links.FrontPageLink, false);
        //}
        //else
        //{
        //    Response.Redirect(links.CategoryPageLink + "?category=" + requestedTag, false);
        //}

        string pagename = System.IO.Path.GetFileName(Request.ServerVariables["SCRIPT_NAME"]);
        if (links.FrontPageLink.EndsWith(pagename)
            || links.NewestPageLink.EndsWith(pagename)
            || links.MyNewsPageLink.EndsWith(pagename)
            || links.SavedPageLink.EndsWith(pagename)
            || links.RatedPageLink.EndsWith(pagename)
            || links.CategoryPageLink.EndsWith(pagename))
        {

            //  string link = Request.Url.ToString();
            string link = Request.Url.OriginalString;

            if (link.Contains("sort=n"))
            {
                link = link.Replace("sort=n", "sort=h");
            }
            else if (link.Contains("sort=h"))
            {

            }
            else
            {
                if (link.Contains("?"))
                {
                    link = link + "&sort=h";
                }
                else
                {
                    link = link + "?sort=h";
                }
            }

            Regex startItemRegex = new Regex(@"startItem=\d+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (startItemRegex.IsMatch(link))
            {
                link = startItemRegex.Replace(link, "startItem=0");
            }

            Response.Redirect(link, false);
        }
        else
        {
            Response.Redirect(links.FrontPageLink + "?sort=h", false);
        }
    }

    protected void NewestPageHL_Click(object sender, EventArgs e)
    {
        //if (string.IsNullOrEmpty(requestedTag) || !(tags.TagsList.Contains(requestedTag) || tags.TagsList.Contains(requestedTag.ToLower())))
        //{
        //    Response.Redirect(links.NewestPageLink, false);
        //}
        //else
        //{
        //    Response.Redirect(links.CategoryPageLink + "?category=" + requestedTag, false);          
        //}

        string pagename = System.IO.Path.GetFileName(Request.ServerVariables["SCRIPT_NAME"]);
        if (links.FrontPageLink.EndsWith(pagename)
            || links.NewestPageLink.EndsWith(pagename)
            || links.MyNewsPageLink.EndsWith(pagename)
            || links.SavedPageLink.EndsWith(pagename)
            || links.RatedPageLink.EndsWith(pagename)
            || links.CategoryPageLink.EndsWith(pagename))
        {
            //  string link = Request.Url.ToString();
            string link = Request.Url.OriginalString;

            if (link.Contains("sort=n"))
            {

            }
            else if (link.Contains("sort=h"))
            {
                link = link.Replace("sort=h", "sort=n");
            }
            else
            {
                if (link.Contains("?"))
                {
                    link = link + "&sort=n";
                }
                else
                {
                    link = link + "?sort=n";
                }
            }

            Regex startItemRegex = new Regex(@"startItem=\d+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (startItemRegex.IsMatch(link))
            {
                link = startItemRegex.Replace(link, "startItem=0");
            }

            Response.Redirect(link, false);
        }
        else
        {
            Response.Redirect(links.FrontPageLink + "?sort=n", false);
        }


    }

    protected void UserAccountHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.UserDetailsPageLink + "?UID=" + UID, false);
    }

    protected void MyNewsHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.MyNewsPageLink, false);
    }


    protected void SavedHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.SavedPageLink, false);
    }



    protected void SubmitHL_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        Response.Redirect(links.SubmitPageLink, false);
    }

    //protected void LoginHL_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect(links.LoginLink, false);
    //}
    //protected void RegisterHL_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect(links.RegisterLink, false);
    //}



    protected void LogoutHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.LogoutPageLink, false);
    }


    protected void AboutHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.AboutPageLink, false);
    }

    protected void BlogHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.BlogPageLink, false);
    }


    protected void FeedbackHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.FeedbackPageLink, false);
    }
    protected void ContactHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.ContactUsPageLink, false);
    }




    protected void RssHL_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.RssPageLink, false);
    }


    protected void getputs_logo_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect(links.FrontPageLink, false);
    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
        string message = "";

        string UID = UsernameTB.Text.Trim();
        string password = PasswordTB.Text.Trim();

        UID = general.EscapeCharacters(UID);
        password = general.EscapeCharacters(password);


        if (string.IsNullOrEmpty(UID) || string.IsNullOrEmpty(password))
        {
            //  message = gui.RedFontStart + "Please Enter Your Username/Password" + gui.RedFontEnd;
            message = gui.RedFontStart + "Invalid Username/Password" + gui.RedFontEnd;
        }
        else if (!general.IsAlphabetOrNumber(UID))
        {
            //  message = gui.RedFontStart + "UserID can only contain Alphabets & Numbers." + gui.RedFontEnd;
            message = gui.RedFontStart + "Invalid Username/Password" + gui.RedFontEnd;
        }
        else if (!general.IsAlphabetOrNumber(password))
        {
            //  message = gui.RedFontStart + "Password can only contain Alphabets & Numbers." + gui.RedFontEnd;
            message = gui.RedFontStart + "Invalid Username/Password" + gui.RedFontEnd;
        }
        else
        {
            if (!general.IsBadUser(UID))
            {

                string passwordHash = password;
                //  Generate the Hashed Password if PasswordHashing Parameter from Web.Config is turned ON.        
                if (isDoPasswordHashing == 1)
                {
                    passwordHash = dbOps.HashPassword(password);
                }
                bool isValid = IsValidCredentials(UID, passwordHash);
                if (isValid)
                {
                    CreateSession(UID);
                    log.Log(UID + " Logged.");
                    Response.Redirect(links.FrontPageLink, false);
                }
                else
                {
                    message = gui.RedFontStart + "Please Try Again" + gui.RedFontEnd;
                }
            }
            else    //  The User has been BlackListed.
            {
                message = gui.RedFontStart + "User " + UID + " has been blacklisted." + gui.RedFontEnd;
            }
        }
        MessageLabel.Text = message;
    }



    private bool IsValidCredentials(string UID, string password)
    {
        bool isValid = false;
        string queryString = @"SELECT Count(*) FROM user WHERE UID='" + UID + "' AND Password='" + password + "' ;";
        int retVal = dbOps.ExecuteScalar(queryString);

        if (retVal >= 1)
        {
            isValid = true;
        }
        return isValid;
    }

    private void CreateSession(string UID)
    {
        HttpCookie getputsCookie = new HttpCookie("getputsCookie");
        getputsCookie["UID"] = dbOps.Encrypt(UID);

        Response.Cookies.Add(getputsCookie);

    }

    private void LayoutCookie(string layout)
    {
        if (Request.Cookies["getputsLayoutCookie"] != null)
        {
            HttpCookie getputsLayoutCookie = Request.Cookies["getputsLayoutCookie"];
            getputsLayoutCookie["layout"] = dbOps.Encrypt(layout);
            Response.Cookies.Add(getputsLayoutCookie);
        }
        else
        {
            HttpCookie getputsLayoutCookie = new HttpCookie("getputsLayoutCookie");
            getputsLayoutCookie["layout"] = dbOps.Encrypt(layout);
            Response.Cookies.Add(getputsLayoutCookie);
        }
    }



    protected void SearchButton_Click(object sender, EventArgs e)
    {
        string query = SearchTB.Text.Trim();

        if (!string.IsNullOrEmpty(query))
        {
            //  Log the Search Query.
            general.LogQuery(query, UID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), general.GetIP(Request));

            //  Response.Redirect(links.SearchPageLink + "?query=" + query, false);
            Response.Redirect(links.SearchPageLink + "?query=" + query + "&sType=Title&category=All Categories&sTime=Year", false);
        }
    }



    /*
    //  Vatsal Shah | 2009-04-14 | This Entire Method Code has been moved to the PageLoad() Method of RandomNews.aspx
    protected void RandomArticleLinkButton_Click(object sender, EventArgs e)
    {
        //bool isDone = false;
        //string link = string.Empty;

        //string queryString = "SELECT COUNT(*) FROM item WHERE Spam = 0;";
        //int retInt = dbOps.ExecuteScalar(queryString);

        //if (retInt > 0)
        //{
        //    int rowCount = retInt;
        //    int rowNumber = random.Next(0, rowCount);

        //    queryString = "SELECT IID, Link FROM item WHERE Spam = 0;";
        //    MySqlDataReader retList = dbOps.ExecuteReader(queryString);

        //    if (retList != null && retList.HasRows)
        //    {
        //        int rows = 0;
        //        while (retList.Read() && !isDone)
        //        {
        //            rows++;
        //            if (rows == rowNumber)
        //            {
        //                isDone = true;
        //                link = Convert.ToString(retList["Link"]);
        //            }
        //        }                                                  
        //        log.Log("Rows: " + rowCount.ToString() + " Selected: " + rowNumber.ToString());                                               
        //        retList.Close();
        //    }

        //    if (isDone)
        //    {
        //        Response.Redirect(link, false);
        //    }
        //}


        bool isDone = false;
        string link = string.Empty;

        //string queryString = "SELECT COUNT(*) FROM item WHERE Spam = 0;";
        //int retInt = dbOps.ExecuteScalar(queryString);

        int retInt = engine.ItemRowCount;

        if (retInt > 0)
        {
            int rowCount = retInt;
            int rowNumber = random.Next(0, rowCount);

            while (!isDone)
            {
                string queryString = "SELECT Link FROM item WHERE Spam = 0 AND IID = " + rowNumber + ";";
                MySqlDataReader retList = dbOps.ExecuteReader(queryString);

                if (retList != null && retList.HasRows)
                {
                    //   DB Hit.                     
                    while (retList.Read())
                    {
                        link = Convert.ToString(retList["Link"]);
                    }
                    retList.Close();
                    isDone = true;
                }
                else
                {
                    //   DB Miss. Select another random number.
                    rowNumber = random.Next(0, rowCount);
                }
            }

            if (isDone && !string.IsNullOrEmpty(link))
            {
                Response.Redirect(link, true);
            }
        }
        else
        {
            //  Not decided what to do.
            //  May be redirect back to getputs.
            Response.Redirect(links.FrontPageLink, true);
        }
    }
    */
    



    protected void FlowIB_Click(object sender, ImageClickEventArgs e)
    {
        string redirectURL = Request.Url.OriginalString;
        LayoutCookie("flow");
        Response.Redirect(redirectURL, true);
    }
    protected void ColumnsIB_Click(object sender, ImageClickEventArgs e)
    {
        string redirectURL = Request.Url.OriginalString;
        LayoutCookie("columns");
        Response.Redirect(redirectURL, true);
    }
    
    protected void CategorizedIB_Click(object sender, ImageClickEventArgs e)
    {
        string redirectURL = Request.Url.OriginalString;
        LayoutCookie("categorized");
        Response.Redirect(redirectURL, true);
    }
}
