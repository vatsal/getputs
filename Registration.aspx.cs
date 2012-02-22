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

using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

public partial class Registration : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;

    int sessionTimeoutMinutes = int.Parse(ConfigurationManager.AppSettings["sessionTimeoutMinutes"]);
    int isDoPasswordHashing = int.Parse(ConfigurationManager.AppSettings["isDoPasswordHashing"]);

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        engine = ProcessingEngine.Instance;

        //  If not using ReCaptcha Project, but using the other Captcha Library.
        //if (!IsPostBack)
        //{
        //    HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        //}

        
    }
    protected void RegistrationButton_Click(object sender, EventArgs e)
    {        
        string UID = UsernameTB.Text.Trim().ToLower();
        string password = PasswordTB.Text.Trim();
        string confirmPassword = ConfirmPasswordTB.Text.Trim();
        string email = EMailTB.Text.Trim();
        string about = AboutTB.Text.Trim();

        UID = general.EscapeCharacters(UID);
        password = general.EscapeCharacters(password);
        confirmPassword = general.EscapeCharacters(confirmPassword);
        email = general.EscapeCharacters(email);
        about = general.EscapeCharacters(about);
                
        bool isValid = false;
        string errorMessage = "";
        int userRegistrationCode = 0;

        
        


            //  if (UID != "" && password != "" && confirmPassword != "" && email != "")
            if (!string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(email))
            {
                if (UID.Length < 3)
                {
                    errorMessage = gui.RedFontStart + "UserID should be atleast 3 characters long" + gui.RedFontEnd;
                }
                else if (password.Length < 3)
                {
                    errorMessage = gui.RedFontStart + "Password should be atleast 3 characters" + gui.RedFontEnd;
                }
                else if (!general.IsAlphabetOrNumber(UID))
                {
                    errorMessage = gui.RedFontStart + "UserID can only contain Alphabets & Numbers." + gui.RedFontEnd;
                }
                else if (!general.IsAlphabetOrNumber(password))
                {
                    errorMessage = gui.RedFontStart + "Password can only contain Alphabets & Numbers." + gui.RedFontEnd;
                }
                else if ((email.Length < 4) || (!general.IsValidEMail(email)))
                {
                    errorMessage = gui.RedFontStart + "Enter a valid E-Mail Address." + gui.RedFontEnd;
                }

                else
                {
                    if (password == confirmPassword)
                    {
                        //  Validate the Captcha
                        if (Page.IsValid)   //  If using ReCaptcha
                        //if(IsCaptchaValid())  //  If using the other Captcha Library.   
                        {
                            string ip = general.GetIP(this.Request);

                            if (!general.IsBadIP(ip))
                            {
                                //  Generate the Hashed Password.
                                string passwordHash = password;
                                if (isDoPasswordHashing == 1)
                                {
                                    passwordHash = dbOps.HashPassword(password);
                                }
                                //  userRegistrationCode = CreateUser(username, password, email, expertiseLevel);
                                userRegistrationCode = CreateUser(UID, passwordHash, email, about, ip);

                                if (userRegistrationCode == 0)
                                    isValid = true;
                                else if (userRegistrationCode == 1)
                                {
                                    errorMessage = gui.RedFontStart + "User Already Exists. Please Try a different UserID" + gui.RedFontEnd;
                                }
                                else if (userRegistrationCode == 2)
                                {
                                    errorMessage = gui.RedFontStart + "The EMail Address " + email + " has already been registered before."
                                        + gui.LineBreak
                                        + "Please use a different EMail Address."
                                        + gui.RedFontEnd;
                                }
                                else
                                {
                                    errorMessage = gui.RedFontStart + "Please Try Again" + gui.RedFontEnd;
                                }
                            }
                            else
                            {
                                errorMessage = gui.RedFontStart + "Your IP Address has been blocked." + gui.RedFontEnd;
                            }
                        }
                        else    //  Invalid Captcha was entered.
                        {
                            errorMessage = gui.RedFontStart + "Invalid Captcha. Please Try Again." + gui.RedFontEnd;
                        }
                    }
                }
            }
            else
            {
                isValid = false;
                errorMessage = gui.RedFontStart + "Please fill out all the required fields." + gui.RedFontEnd;
            }

            if (errorMessage != "")
            {
                MessageLabel.Text = errorMessage;
            }

            if (isValid)
            {
                CreateSession(UID);
                Response.Redirect(links.FrontPageLink, false);
            }
            else
            {

            }
        
            //HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
            //CodeNumberTextBox.Text = string.Empty;
    }

    /// <summary>
    /// Create the User if not already existing inside Stoocks
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <param name="about"></param>
    /// <param name="ip"></param>
    /// <returns>(0) -> Success, (1) -> User already exists, (2) -> EMail already exists, (-1) -> Error</returns>
    private int CreateUser(string UID, string password, string email, string about, string ip)
    {
        //  The Table getputs.user has the following schema:
        //  getputs.user [ UID, Password, Date, EMail, About, Admin, Points] 

        string today = DateTime.Now.ToString(dateFormatString);
        int isAdmin = 0;
        int points = 0;

        string queryString = @"SELECT Count(*) FROM user WHERE UID='" + UID + "';";
        int retVal = dbOps.ExecuteScalar(queryString);

        if (retVal == 0)    //  No User with the Username=UID exists. Create the User.
        {
            queryString = @"SELECT Count(*) FROM user WHERE EMail='" + email + "';";
            retVal = dbOps.ExecuteScalar(queryString);

            if (retVal == 0)     //  No User with the given email exists. Create the User.
            {

                queryString = @"INSERT INTO user(UID, Password, Date, EMail, About, Admin, Points, PreferredCategories, IP) VALUES ('" + UID + "', '" + password + "', '" + today + "', '" + email + "', '" + about + "', " + isAdmin + ", " + points + ", '', INET_ATON('" + ip + "'));";
                retVal = dbOps.ExecuteNonQuery(queryString);
                if (retVal >= 0)    //  User successfully registered.
                {
                    retVal = 0;
                }
                else    //  An error occurred.
                {
                    retVal = -1;
                }
            }
            else if (retVal > 0)    //  A UserID with the input EMail Address already exists. Return Error Code = 2
            {
                retVal = 2;
            }
            else    //  An error occurred.
            {
                retVal = -1;
            }

        }
        else if (retVal > 0)    //  User Already Exists.
        {
            retVal = 1;
        }
        else    //  An error occurred.
        {
            retVal = -1;
        }
        return retVal;
    }


    private void CreateSession(string UID)
    {
        HttpCookie getputsCookie = new HttpCookie("getputsCookie");
        getputsCookie["UID"] = dbOps.Encrypt(UID);

        Response.Cookies.Add(getputsCookie);
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
