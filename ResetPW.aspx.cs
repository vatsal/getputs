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

public partial class ResetPW : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    GUIVariables gui;
    Logger log;
    General general;
    ProcessingEngine engine;

    string UID;

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
        UID = string.Empty;
        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(UID))
        {
            Response.Redirect(links.FrontPageLink, false);
        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists

    }

    protected void ResetPasswordButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(UID))
        {
            Response.Redirect(links.FrontPageLink, false);
        }
        else
        {
            string message = string.Empty;

            string currentPassword = CurrentPasswordTB.Text.Trim();
            string newPassword = NewPasswordTB.Text.Trim();
            string confirmNewPassword = ConfirmNewPasswordTB.Text.Trim();

            currentPassword = general.EscapeCharacters(currentPassword);
            newPassword = general.EscapeCharacters(newPassword);
            confirmNewPassword = general.EscapeCharacters(confirmNewPassword);
           
            if ((!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmNewPassword))
                && general.IsAlphabetOrNumber(currentPassword) && general.IsAlphabetOrNumber(newPassword) && general.IsAlphabetOrNumber(confirmNewPassword))
            {
                if (currentPassword.Length < 3 || newPassword.Length < 3 || confirmNewPassword.Length < 3)
                {
                    message = "Password should be atleast 3 characters";
                }
                else
                {
                    if (newPassword.Equals(confirmNewPassword))
                    {
                        //  SELECT Count(*) FROM getputs.user WHERE UID='vatsal' AND Password='83B292279DC10EF498A82D6606C9278AF35A4232';
                        currentPassword = dbOps.HashPassword(currentPassword);
                        string queryString = "SELECT Count(*) FROM getputs.user WHERE UID='" + UID + "' AND Password = '" + currentPassword + "';";
                        int retVal = dbOps.ExecuteScalar(queryString);
                        
                        if (retVal == 1)    //  Valid Current Password Submitted. Update the DB with the New Password.
                        {
                            newPassword = dbOps.HashPassword(newPassword);
                            queryString = "UPDATE getputs.user SET Password = '" + newPassword + "' WHERE UID='" + UID + "';";
                            retVal = dbOps.ExecuteNonQuery(queryString);
                            if (retVal > 0) //  DB Updated Successfully.
                            {
                                message = gui.GreenFontStart 
                                    + "Your Password has been reset."
                                    + gui.LineBreak
                                    + "Please use your new password from now on." 
                                    + gui.GreenFontEnd;
                            }
                            else    //  An Error Occurred.
                            {
                                message = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                            }
                        }
                        else if (retVal == 0)   //  Invalid Current Password Submitted.
                        {
                            message = gui.RedFontStart + "Invalid Password." + gui.RedFontEnd;
                        }
                        else if (retVal < 0)    //  An Error Occurred.
                        {
                            message = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                        }
                        

                    }
                    else
                    {
                        message = gui.RedFontStart + "The Fields New Password and Confirm New Password do not match." + gui.RedFontEnd;
                    }
                }
            }
            else
            {
                message = gui.RedFontStart 
                    + "Please Enter a Valid Password." 
                    + gui.LineBreak
                    + "Password can only contain Alphabets & Numbers."
                    + gui.RedFontEnd;
            }

            MessageLabel.Text = message;
        }
    }
}
