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

public partial class SLogin : System.Web.UI.Page
{

    DBOperations dbOps;
    Links links;
    GUIVariables gui;
    Logger log;
    General general;
    ProcessingEngine engine;

    int sessionTimeoutMinutes = int.Parse(ConfigurationManager.AppSettings["sessionTimeoutMinutes"]);
    int isDoPasswordHashing = int.Parse(ConfigurationManager.AppSettings["isDoPasswordHashing"]);

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];


    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        gui = GUIVariables.Instance;
        log = Logger.Instance;
        general = General.Instance;
        engine = ProcessingEngine.Instance;

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.
        string UID = string.Empty;
        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (!string.IsNullOrEmpty(UID))
        {
            Response.Redirect(links.FrontPageLink, false);
        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists

        if (!IsPostBack)
        {
            Control MasterPageLoginTable = this.Master.FindControl("LoginTable");
            MasterPageLoginTable.Visible = false;
        }

        UsernameTB.Focus();
        Page.Form.DefaultButton = SLoginButton.UniqueID;


    }
    protected void SLoginButton_Click(object sender, EventArgs e)
    {
        string message = "";

        string UID = UsernameTB.Text.Trim().ToLower();
        string password = PasswordTB.Text.Trim();
        
        UID = general.EscapeCharacters(UID);
        password = general.EscapeCharacters(password);


        if (string.IsNullOrEmpty(UID) || string.IsNullOrEmpty(password))
        {
            message = gui.RedFontStart + "Please Enter Your Username/Password" + gui.RedFontEnd;
        }
        else if (!general.IsAlphabetOrNumber(UID))
        {
            message = gui.RedFontStart + "UserID can only contain Alphabets & Numbers." + gui.RedFontEnd;
        }
        else if (!general.IsAlphabetOrNumber(password))
        {
            message = gui.RedFontStart + "Password can only contain Alphabets & Numbers." + gui.RedFontEnd;
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
                message = gui.RedFontStart + "The UserID: " + UID + " has been blacklisted." + gui.RedFontEnd;
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
}
