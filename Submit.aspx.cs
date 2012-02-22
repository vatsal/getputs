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
using System.Collections.Generic;
using System.Net;
using System.Text;

public partial class Submit : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;
    Categories categories;
    SpamDetection spamDetection;
    Tokenizer tokenizer;
    Tagger tagger;
    ImageEngine imageEngine;
    Logger log;


    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];


    //  A link cannot be submitted more than once in N Hours, Where N=DupSubmissionTimeFrame
    int _dupSubmissionTimeFrame = int.Parse(ConfigurationManager.AppSettings["DupSubmissionTimeFrame"]);
    //  A single user can only submit MaxItemsPerUser within a particular TimeFrame
    int _maxItemsPerUser = int.Parse(ConfigurationManager.AppSettings["MaxItemsPerUser"]);
    //  A single user can only submit MaxItemsPerUser within a particular TimeFrame in Hours
    int _maxItemsPerUserTimeFrame = int.Parse(ConfigurationManager.AppSettings["MaxItemsPerUserTimeFrame"]);

    int _minTokensInTitle = 2;  //  Minimum words required in the Title section.
    int _maxTokensInTitle = 40; //  Maximum words required in the Title section.
    
    int _minTokensInText = 5;   //  Minimum words required in the Text section.

    string UID;

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        engine = ProcessingEngine.Instance;
        categories = Categories.Instance;
        tokenizer = Tokenizer.Instance;
        tagger =Tagger.Instance;
        imageEngine = ImageEngine.Instance;
        log = Logger.Instance;

        spamDetection = (SpamDetection)Application["spamDetection"];

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.
        
        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(UID))
        {
            Response.Redirect(links.LoginLink, false);
        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists

        StringBuilder explanationSB = new StringBuilder();


        explanationSB.Append(gui.GreenFontStart);
        explanationSB.Append(gui.BoldFontStart);

        explanationSB.Append("Posting News has never been this easy!");
        explanationSB.Append(gui.LineBreak);
        
        explanationSB.Append(gui.HTMLTab + "(1) Copy and Paste the Link of the news/webpage you want to Post.");
        explanationSB.Append(gui.LineBreak);
        explanationSB.Append(gui.HTMLTab + "(2) Click on the \"Get Title\" Button to automatically get the Title,");
        explanationSB.Append(gui.LineBreak);
        explanationSB.Append(gui.HTMLTab + "OR, Write a Descriptive Title.");
        explanationSB.Append(gui.LineBreak);
        explanationSB.Append(gui.HTMLTab + "(3) Select an appropriate Category.");
        explanationSB.Append(gui.LineBreak);
        explanationSB.Append("And you are done!");
        explanationSB.Append(gui.LineBreak);

        explanationSB.Append(gui.BoldFontEnd);
        explanationSB.Append(gui.GreenFontEnd);
        
        explanationSB.Append(gui.LineBreak);        
        explanationSB.Append(gui.LineBreak);


        
        explanationSB.Append("If submitting a link, keep the text section empty.");
        explanationSB.Append(gui.LineBreak);
        explanationSB.Append("If submitting text, keep the link section empty.");
        explanationSB.Append(gui.LineBreak);
        explanationSB.Append("If there is a link, the text section will be ignored.");


        ExplanationLabel.Text = explanationSB.ToString(); 
            

        AddTags();


        //  For the Bookmarklet.
        //  Check the Tools.aspx page for matching the Title/Link parameters.
        if (!string.IsNullOrEmpty(Request.QueryString["title"]) && !string.IsNullOrEmpty(Request.QueryString["url"]))
        {
            TitleTB.Text = Request.QueryString["title"].Trim();
            LinkTB.Text = Request.QueryString["url"];
        }

        //  If not using ReCaptcha Project, but using the Captcha Library.
        //if (!IsPostBack)
        //{
        //    HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        //    //  log.Log("Inside !IsPostBack : " + HttpContext.Current.Session["CaptchaImageText"].ToString());
        //}
        
    }

    private void AddTags()
    {
        foreach (string tag in categories.CategoriesList)
        {
            CategoryDDL.Items.Add(tag);
        }
    }


    protected void GetTitleButton_Click(object sender, EventArgs e)
    {

        GetTitleOutputLiteral.Text = string.Empty;
        StringBuilder message = new StringBuilder();

        if (string.IsNullOrEmpty(LinkTB.Text))
        {
            message.Append(gui.RedFontStart);
            message.AppendLine("Please Enter a URL/Link.");
            message.Append(gui.RedFontEnd);
        }
        else
        {
            try
            {
                string url = LinkTB.Text.Trim();
                if (general.IsValidURL(url))
                {
                    if (!url.StartsWith("http://"))
                    {
                        url = "http://" + url;
                    }


                    using (WebClient wc = new WebClient())
                    {
                        byte[] data = wc.DownloadData(url);

                        string htmlText = Encoding.ASCII.GetString(data);
                        if (htmlText.Contains("<title>"))
                        {
                            int startIndex = htmlText.IndexOf("<title>");
                            int endIndex = htmlText.IndexOf("</title>");

                            if (startIndex != -1 && endIndex != -1)
                            {
                                string title = htmlText.Substring(startIndex, endIndex - startIndex).Replace("<title>", "").Replace("</title>", "");
                                TitleTB.Text = title;
                                message.Append(gui.GreenFontStart);
                                message.Append("getputs was able to extract the Title.");
                                message.Append(gui.LineBreak);
                                message.Append("You can change the Title if you want to.");
                                message.Append(gui.GreenFontEnd);

                            }
                            else
                            {
                                message.Append(gui.RedFontStart);
                                message.Append("No appropriate Title Found. Please Type the Title.");
                                message.Append(gui.RedFontEnd);
                            }
                        }
                    }
                }
                else
                {
                    message.Append(gui.RedFontStart);
                    message.AppendLine("Invalid Link/URL. Please Submit a Valid Link.");
                    message.Append(gui.RedFontEnd);
                }
            }
            catch (Exception ex)
            {
                message.Append(gui.RedFontStart);
                message.Append("Unable to automatically extract the Title.");
                message.Append(gui.LineBreak);
                message.Append("Please Type the Title.");
                message.Append(gui.RedFontEnd);             
            }
        }
        GetTitleOutputLiteral.Text = message.ToString();
    }



    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        string message = "";

        //  string date = DateTime.Now.ToString();
        //  string date = DateTime.Now.ToString(dateFormatString);
        //  string date = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");        
        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        string title = TitleTB.Text.Trim();
        string link = LinkTB.Text.Trim();
        string text = TextTB.Text.Trim();

        title = general.EscapeCharacters(title);
        link = general.EscapeCharacters(link);
        text = general.EscapeCharacters(text);

        string category = CategoryDDL.SelectedItem.Text;

        string queryString = string.Empty;

        //  The Table getputs.user has the following schema:
        //  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]
        //  IID is AutoIncrement.


        //  Validate the Captcha
        //  if (Page.IsValid)   //  If using ReCaptcha
        //  if(IsCaptchaValid())  //  If using the other Captcha Library.
        {
            if (!string.IsNullOrEmpty(title))
            {
                List<string> titleTokens = tagger.GetTokens(title, false, false, false);
                if(IsValidTitle(titleTokens))
                //  if (titleTokens.Count >= _minTokensInTitle && titleTokens.Count <= _maxTokensInTitle)
                {
                    string impTagString = string.Empty;
                    if (tagger.IsTaggingOn)
                    {
                        List<string> impTags = tagger.GetImportantTagsList(titleTokens, tagger.GetPOSTags(titleTokens));                        
                        impTagString = general.ConvertListToCSV(impTags);
                    }
                    
                    if (!string.IsNullOrEmpty(link))
                    {
                        if (general.IsValidURL(link))
                        {
                            //  Add http:// and/or www. if  not present.
                            if (link.StartsWith("http://") || link.StartsWith("https://"))
                            {

                            }
                            else if (link.StartsWith("www."))
                            {
                                link = "http://" + link;
                            }
                            else
                            {
                                link = "http://www." + link;
                            }

                            string ip = general.GetIP(this.Request);

                            //  Check the Link against the GSB List.
                            //  if (spamDetection.IsSafeLink(link))

                            if (!general.CheckBlackList(UID, ip, link))
                            {

                                //  Check if the same link has not been submitted in the last 8 hours.
                                DateTime dupDT = DateTime.Now.Subtract(new TimeSpan(_dupSubmissionTimeFrame, 0, 0));
                                string dupDate = dupDT.ToString("yyyy-MM-dd HH:mm:ss");
                                //  SELECT COUNT(*) FROM getputs.item WHERE Link='http://www.getputs.com' AND Date > DATE_FORMAT('2008-02-04', '%Y-%m-%d %k:%i:%S');
                                queryString = "SELECT COUNT(*) FROM item WHERE Link='" + link + "' AND Date > DATE_FORMAT('" + dupDate + "', '%Y-%m-%d %k:%i:%S');";
                                int retInt = dbOps.ExecuteScalar(queryString);

                                if (retInt > 0)
                                {
                                    message = gui.RedFontStart + "Item Already Submitted Today." + gui.RedFontEnd;
                                }
                                else
                                {
                                    DateTime maxItemsPerUserTimeFrameDT = DateTime.Now.Subtract(new TimeSpan(_maxItemsPerUserTimeFrame, 0, 0));
                                    string maxItemsPerUserDateStr = maxItemsPerUserTimeFrameDT.ToString("yyyy-MM-dd HH:mm:ss");
                                    queryString = "SELECT COUNT(*) FROM item WHERE UID = '" + UID + "' AND Date > DATE_FORMAT('" + maxItemsPerUserDateStr + "', '%Y-%m-%d %k:%i:%S');";
                                    retInt = dbOps.ExecuteScalar(queryString);

                                    if (retInt >= 0 && retInt < _maxItemsPerUser)
                                    {

                                        text = string.Empty;

                                        //  queryString that does not insert the IP
                                        //  queryString = "INSERT INTO item (Title, Link, Date, UID, Category) values "
                                        //    + "('" + title + "', '" + link + "', DATE_FORMAT('" + date + "','%Y-%m-%d %k:%i:%S' ) , '" + UID + "' , '" + tag + "')";

                                        //  queryString that also inserts the IP
                                        queryString = "INSERT INTO item (Title, Link, Date, UID, Category, IP, Tags) values "
                                            + "('" + title + "', '" + link + "', DATE_FORMAT('" + date + "','%Y-%m-%d %k:%i:%S' ) , '" + UID + "' , '" + category + "' , INET_ATON('" + ip + "'), '" + impTagString + "');";


                                        int retVal = dbOps.ExecuteNonQuery(queryString);
                                        if (retVal >= 0)    //  Item successfully submitted.
                                        {
                                            if (imageEngine.IsItemImageStorageOn)
                                            {
                                                queryString = "SELECT IID FROM item WHERE Link='" + link + "' AND UID='" + UID + "';";
                                                int iid = dbOps.ExecuteScalar(queryString);

                                                if (iid > 0)
                                                {
                                                    bool isImageStored = general.StoreImages(link, imageEngine.ItemImageStorageCount, iid, ImageEngine.HTMLImageLocation.HTMLBody);
                                                }
                                            }



                                            string newLink = "<a href='Default.aspx?sort=n' class='CSS_LinkItem'>New Section</a>";
                                            message = gui.GreenFontStart + "Item successfully submitted."
                                                + gui.LineBreak
                                                + "Visit the " + newLink + " to view your recently submitted post."
                                                + gui.GreenFontEnd;

                                            TitleTB.Text = string.Empty;
                                            LinkTB.Text = string.Empty;
                                            TextTB.Text = string.Empty;

                                            //  HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
                                                                               



                                        }
                                        else
                                        {
                                            message = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                                        }
                                    }
                                    else
                                    {
                                        message = gui.RedFontStart + "To maintain diversity of content, "
                                            + gui.LineBreak
                                            + "getputs allows a single user to post only "
                                            + _maxItemsPerUser.ToString() + " Items in " + _maxItemsPerUserTimeFrame.ToString() + " Hour. "
                                            + gui.LineBreak
                                            + "Please Try Submitting a link after some time."
                                            + gui.RedFontEnd;
                                    }
                                }
                            }
                            else
                            {
                                //message = gui.RedFontStart + "The getputs filter has detected this link to be a spam/malware/phishing site."
                                //    + gui.LineBreak + "Please do not submit spam/malware/phishing sites." + gui.RedFontEnd;

                                message = gui.RedFontStart + "Either your submission rights have been revoked, or the link you are trying to submit has been blacklisted." + gui.RedFontEnd;
                            }
                        }
                        else
                        {
                            message = gui.RedFontStart + "Please Submit a Valid Link" + gui.RedFontEnd;
                        }

                    }
                    else if (!string.IsNullOrEmpty(text))
                    {
                        List<string> textTokens = tagger.GetTokens(text, false, false, false);
                        if (textTokens.Count >= _minTokensInText)
                        {

                            string ip = general.GetIP(this.Request);

                            if (!general.IsBadUser(UID) && !general.IsBadIP(ip))
                            {

                                DateTime maxItemsPerUserTimeFrameDT = DateTime.Now.Subtract(new TimeSpan(_maxItemsPerUserTimeFrame, 0, 0));
                                string maxItemsPerUserDateStr = maxItemsPerUserTimeFrameDT.ToString("yyyy-MM-dd HH:mm:ss");
                                queryString = "SELECT COUNT(*) FROM item WHERE UID = '" + UID + "' AND Date > DATE_FORMAT('" + maxItemsPerUserDateStr + "', '%Y-%m-%d %k:%i:%S');";
                                int retInt = dbOps.ExecuteScalar(queryString);

                                if (retInt >= 0 && retInt < _maxItemsPerUser)
                                {


                                    //  DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S')

                                    //  queryString that does not insert the IP
                                    //  queryString = "INSERT INTO item (Title, Text, Date , UID, Category) values "
                                    //      + "('" + title + "', '" + text + "', DATE_FORMAT('" + date + "', '%Y-%m-%d %k:%i:%S') , '" + UID + "' , '" + tag + "')";

                                    //  queryString that also inserts the IP
                                    queryString = "INSERT INTO item (Title, Text, Date , UID, Category, IP, Tags) values "
                                        + "('" + title + "', '" + text + "', DATE_FORMAT('" + date + "', '%Y-%m-%d %k:%i:%S') , '" + UID + "' , '" + category + "' , INET_ATON('" + ip + "'), '" + impTagString + "');";

                                    int retVal = dbOps.ExecuteNonQuery(queryString);
                                    if (retVal >= 0)    //  Item successfully submitted.
                                    {
                                        string newLink = "<a href='Default.aspx?sort=n' class='CSS_LinkItem'>New Section</a>";
                                        message = gui.GreenFontStart + "Item successfully submitted."
                                            + gui.LineBreak 
                                            + "Visit the " + newLink + " to view your recently submitted post."
                                            + gui.GreenFontEnd; 
                                        
                                        TitleTB.Text = string.Empty;
                                        LinkTB.Text = string.Empty;
                                        TextTB.Text = string.Empty;

                                        //  HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
                                    }
                                    else
                                    {
                                        message = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                                    }
                                }
                                else
                                {
                                    message = gui.RedFontStart + "To maintain diversity of content, "
                                        + gui.LineBreak
                                        + "getputs allows a single user to post only "
                                        + _maxItemsPerUser.ToString() + " Items in " + _maxItemsPerUserTimeFrame.ToString() + " Hour. "
                                        + gui.LineBreak
                                        + "Please Try Submitting a link after some time."
                                        + gui.RedFontEnd;
                                }
                            }
                            else
                            {
                                message = gui.RedFontStart + "Your submission rights have been revoked." + gui.RedFontEnd;
                            }
                        }
                        else
                        {
                            message = gui.RedFontStart + "Too few words in the Text section." + gui.RedFontEnd;
                        }
                    }
                    else
                    {
                        message = gui.RedFontStart + "Both Link and Text Sections cannot be kept empty at the same time."
                            + gui.LineBreak + "Please fill out atleast one section." + gui.RedFontEnd;
                    }
                }
                else
                {
                    if (titleTokens.Count < _minTokensInTitle)
                    {
                        message = gui.RedFontStart + "Too few words in the Title. Please provide a descriptive Title." + gui.RedFontEnd;
                    }
                    if (titleTokens.Count > _maxTokensInTitle)
                    {
                        message = gui.RedFontStart + "Please shorten the Title of your submission." + gui.RedFontEnd;
                    }
                    else
                    {
                        message = gui.RedFontStart + "Title contains a blocked word or phrase." + gui.RedFontEnd;
                    }
                }
            }
            else
            {
                message = gui.RedFontStart + "Please specify an appropriate Title." + gui.RedFontEnd;
            }
        }
        //else    //  Invalid Captcha was entered.
        //{
        //    message = gui.RedFontStart + "Invalid Captcha. Please Try Again." + gui.RedFontEnd;
        //}

        //HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        //CodeNumberTextBox.Text = string.Empty;


        MessageLabel.Text = message;
    }

    private bool IsValidTitle(List<string> tokens)
    {
        bool isValid = true;

        if (tokens.Count < _minTokensInTitle)
        {
            isValid = false;
        }
        else if (tokens.Count > _maxTokensInTitle)
        {
            isValid = false;
        }
        else
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (general.IsBadWord(tokens[i]))
                {
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    /*
    private bool IsCaptchaValid()
    {
        try
        {
            log.Log("CodeNumberTextBox.Text : " + HttpContext.Current.Session["CaptchaImageText"].ToString());
            log.Log("HttpContext.Current.Session[CaptchaImageText] : " + HttpContext.Current.Session["CaptchaImageText"].ToString());


            bool isValid = false;

            #region Captcha

            // On a postback, check the user input.
            if (HttpContext.Current.Session != null)
            {
                
                if (CodeNumberTextBox.Text == HttpContext.Current.Session["CaptchaImageText"].ToString())
                //  if(CodeNumberTextBox.Text == general.CaptchaImageText)
                {
                    //  Display an informational message.                
                    //  MessageLabel.Text = "Correct.";

                    isValid = true;
                }
                else
                {
                    isValid = false;

                    //  Display an error message.                
                    //  MessageLabel.Text = "Incorrect, Try Again.";

                    // Clear the input and create a new random code.
                    CodeNumberTextBox.Text = string.Empty;
                    HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
                    //  general.CaptchaImageText = general.GenerateRandomCode();
                }
            }
            else
            {
                isValid = true;
            }

            #endregion Captcha

            return isValid;

        }
        catch (Exception ex)
        {
            log.Log(ex);
            return false;
        }
    }
    */

}
