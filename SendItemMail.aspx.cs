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

//  Table Information: 
//  emaileditems (IID, EMailfrom, EMailto, Message, Date, UID)

public partial class SendItemMail : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;
    ImageEngine imageEngine;


    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string seperator = " | ";
    private static int itemClicks = 0;

    string iid = string.Empty;
    string user = string.Empty; //  Logged In User, different from uid.
    Item item;

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;

        seperator = gui.Seperator;

        if (string.IsNullOrEmpty(Request.QueryString["IID"]))
        {
            EMailDiv.Visible = false;
            ItemTitleLabel.Text = "No such item found.";
        }
        else
        {
            iid = Request.QueryString["IID"].Trim().ToLower();
        }


        if (!general.IsValidInt(iid))
        {
            EMailDiv.Visible = false;
        }
        else
        {
            item = new Item();

            string queryString = "SELECT Title, Link, Text, Date, UID, NComments, Category FROM item WHERE IID=" + iid + ";";
            MySqlDataReader retList;

            retList = dbOps.ExecuteReader(queryString);

            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    item.IID = Convert.ToInt32(iid);
                    item.UID = Convert.ToString(retList["UID"]);
                    item.Title = Convert.ToString(retList["Title"]);
                    item.Link = Convert.ToString(retList["Link"]);
                    item.Text = Convert.ToString(retList["Text"]);
                    item.Date = Convert.ToString(retList["Date"]);
                    item.NComments = Convert.ToInt32(retList["NComments"]);
                    item.Category = Convert.ToString(retList["Category"]);


                    if (!string.IsNullOrEmpty(item.Link))
                    {
                        item.Text = string.Empty;
                    }
                }
                retList.Close();
            }
            ItemTitleLabel.Text = "Title: " + gui.GrayFontStart + item.Title + gui.GrayFontEnd;
        }

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            user = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(user))
        {
            
        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists
        
        //  If the user has logged in, populate the 'EMail From' TextBox.
        //  The variable 'user' = Logged in User.
                
        EMailOutputMessageLabel.Text = string.Empty;
        if (!string.IsNullOrEmpty(user))
        {
            string queryString = "SELECT EMail FROM user WHERE UID='" + user + "';";
            MySqlDataReader retList = dbOps.ExecuteReader(queryString);
            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    FromTB.Text = Convert.ToString(retList["EMail"]);
                }
                retList.Close();
            }
        }

        //  If not using ReCaptcha Project, but using the Captcha Library.
        //if (!IsPostBack)
        //{
        //    HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        //}
    }



    protected void EMailButton_Click(object sender, EventArgs e)
    {
        EMailOutputMessageLabel.Text = string.Empty;

        //  string from = FromTB.Text.Trim();
        string fromTB = FromTB.Text.Trim(); //  The E-Mail Address entered by the User.

        string from = general.TestMailID;   //  The Test E-Mail Address that needs to be from a @getputs Domain.
        string to = ToTB.Text.Trim();
        string personalMessage = EMailMessageTB.Text.Trim();
        string message = string.Empty;  //  The Entire E-Mail message to be sent.
        string subject = string.Empty;

        if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.IsNullOrEmpty(fromTB)
            || (!general.IsValidEMail(from) || !general.IsValidEMailList(to) || !general.IsValidEMail(fromTB)))
        {
            EMailOutputMessageLabel.Text = gui.RedFontStart + "Please enter valid e-mail addresses." + gui.RedFontEnd;
        }
        else
        {
            if (item != null)
            {
                //  Validate the Captcha
                if (Page.IsValid)   //  If using ReCaptcha
                //if(IsCaptchaValid())  //  If using the other Captcha Library.        
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        subject = "[getputs.com] " + item.Title;

                        message = fromTB + " has sent you a getputs Item." + gui.LineBreak;

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

                        if (!string.IsNullOrEmpty(personalMessage))
                        {
                            //personalMessage = "Personal Message" + gui.LineBreak
                            //    + gui.GrayFontStart + "____________________________________" + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak
                            //    + personalMessage + gui.LineBreak + gui.LineBreak
                            //    + gui.GrayFontStart + "____________________________________" + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak;
                            
                            message = message + 
                                "Personal Message" + gui.LineBreak
                                + gui.GrayFontStart + "____________________________________" + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak
                                + personalMessage + gui.LineBreak + gui.LineBreak
                                + gui.GrayFontStart + "____________________________________" + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak                                
                                + gui.LineBreak;
                        }


                        //  string discussionsLink = "<a href='" + links.ItemDetailsPageLink + "?iid=" + item.IID.ToString() + "'>" + "[getputs.com]" + "</a>";
                        string discussionsLink = "<a href='" + links.DomainLink + links.ItemDetailsPageLink.Replace("~\\", "") + "?iid=" + item.IID.ToString() + "'>" + "[getputs Discussion]" + "</a>";
                        message = message
                            + "For more discussions on this topic, visit " + discussionsLink + gui.LineBreak;


                        bool isSent = general.SendMail(from, to, subject, message);
                        if (isSent)
                        {
                            //  Also update the getputs.item NEMailed Field.
                            string queryString = "UPDATE item SET NEMailed=NEMailed+1 WHERE IID=" + item.IID.ToString() + ";";
                            int retInt = dbOps.ExecuteNonQuery(queryString);

                            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //  Update the emaileditems Table (UID in the Table = Logged in User).
                            //  INSERT INTO emaileditems (IID, EMailfrom, EMailto, Message, date, UID) VALUES (111, 'vatsals@vatsals.com', 'poojahs@yahoo.com', 'Good Article', '12:00:00', 'vatsal');

                            queryString = "INSERT INTO emaileditems (IID, EMailfrom, EMailto, Message, Date, UID) VALUES ("
                                + item.IID.ToString() + ", '" + fromTB + "', '" + to + "', '" + personalMessage + "', DATE_FORMAT('" + date + "','%Y-%m-%d %k:%i:%S' ) , '" + user + "')";
                            retInt = dbOps.ExecuteNonQuery(queryString);

                            EMailOutputMessageLabel.Text = gui.GreenFontStart + "EMail sent successfully." + gui.GreenFontEnd;
                            ToTB.Text = string.Empty;
                            EMailMessageTB.Text = string.Empty;
                        }
                        else
                        {
                            EMailOutputMessageLabel.Text = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                        }
                    }
                }
                else    //  Invalid Captcha was entered.
                {
                    EMailOutputMessageLabel.Text = gui.RedFontStart + "Invalid Captcha. Please Try Again." + gui.RedFontEnd;
                }
            }
        }
        
        //HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        //CodeNumberTextBox.Text = string.Empty;
    }

    /*
    private bool IsCaptchaValid()
    {
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
    */
}
