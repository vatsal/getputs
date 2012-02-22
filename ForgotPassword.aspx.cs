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
using System.Text;

public partial class ForgotPassword : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    GUIVariables gui;
    Logger log;
    General general;
    ProcessingEngine engine;

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

    }

    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;
        string message = string.Empty;

        string UID = UsernameTB.Text.Trim();
        string eMail = EMailTB.Text.Trim();

        UID = general.EscapeCharacters(UID);
        eMail = general.EscapeCharacters(eMail);


        if (string.IsNullOrEmpty(UID) || string.IsNullOrEmpty(eMail))
        {
            message = gui.RedFontStart + "Please Enter Your Username/EMail" + gui.RedFontEnd;
        }
        else if (!general.IsAlphabetOrNumber(UID))
        {
            message = gui.RedFontStart + "UserID can only contain Alphabets & Numbers." + gui.RedFontEnd;
        }
        else if (!general.IsValidEMail(eMail))
        {
            message = gui.RedFontStart + "Please Enter a valid E-Mail Address" + gui.RedFontEnd;
        }
        else
        {
            //  SELECT Password FROM user where UID='vatsal' and EMail = 'vatsals@vatsals.com';
            string queryString = "SELECT Password FROM user WHERE UID='" + UID +"' AND EMail = '" + eMail +"';";
            MySqlDataReader retList = dbOps.ExecuteReader(queryString);
                        
            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    string hashedPassword = Convert.ToString(retList["Password"]);
                    
                    //  string password = dbOps.Decrypt(hashedPassword);
                    string password = general.GenerateRandomPassword();

                    if (!string.IsNullOrEmpty(password))
                    {
                        hashedPassword = dbOps.HashPassword(password);
                        queryString = "UPDATE user SET Password='" + hashedPassword + "' WHERE UID='" + UID + "';";
                        int retVal = dbOps.ExecuteNonQuery(queryString);

                        if (retVal >= 0)
                        {
                            bool isPasswordEMailed = SendPasswordByEMail(UID, password, eMail);
                            if (isPasswordEMailed)
                            {
                                message = gui.GreenFontStart + "Your Password has been reset. The new password has been E-Mailed to you." + gui.GreenFontEnd;
                            }
                            else
                            {
                                message = gui.RedFontStart + "Please Try Again" + gui.RedFontEnd;
                            }
                        }
                        else
                        {
                            message = gui.RedFontStart + "Please Try Again" + gui.RedFontEnd;
                        }
                    }


                }
                retList.Close();
            }            
            else
            {
                message = gui.RedFontStart + "Please Try Again" + gui.RedFontEnd;
            }
        }
        MessageLabel.Text = message;
    }

    private bool SendPasswordByEMail(string UID, string password, string eMail)
    {
        string from = general.TestMailID;
        string to = eMail;
        string subject = "getputs.com Account Details";

        string message = "Your getputs UserID: " + UID
            + gui.LineBreak
            + "Your New Password: " + password
            + gui.LineBreak
            + gui.LineBreak
            + "The next time you visit getputs.com, please use the new password to login."
            + gui.LineBreak;            
        
        return general.SendMail(from, to, subject, message.ToString());        
    }
}
