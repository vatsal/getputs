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
using System.Collections.Generic;

public partial class UserDetails : System.Web.UI.Page
{
    //  The Table getputs.user has the following schema:
    //  getputs.item [UID, Password, Date, EMail, About, Admin, Points]

    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;
    Categories categories;

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string loggedUID = string.Empty;
    string queryStringUID = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        //UserDetailsLabel.Visible = false;
        //UpdateUserDetailsTable.Visible = false;

        InterestsCBL.Visible = false;
        AddButton.Visible = false;
        SaveChangesLabel.Visible = false;

        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        engine = ProcessingEngine.Instance;
        categories = Categories.Instance;


        if (string.IsNullOrEmpty(Request.QueryString["UID"]))
        {
           
        }
        else
        {
            queryStringUID = Request.QueryString["UID"].Trim().ToLower();
        }        

        if (string.IsNullOrEmpty(queryStringUID))
        {
            UserDetailsLabel.Visible = false;
            UpdateUserDetailsTable.Visible = false;

            SubmittedLB.Visible = false;
            CommentedLB.Visible = false;
        }
        else
        {
            UserDetailsLabel.Visible = true;
            UpdateUserDetailsTable.Visible = false;

            string userDetails = string.Empty;
            StringBuilder returnSB = new StringBuilder();


            string date = string.Empty;
            string eMail = string.Empty;
            string about = string.Empty;

            string queryString = "SELECT Date, EMail, About FROM getputs.user where UID='" + queryStringUID + "';";
            MySqlDataReader retList = dbOps.ExecuteReader(queryString);

            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    date = Convert.ToString(retList["Date"]);
                    eMail = Convert.ToString(retList["EMail"]);
                    about = Convert.ToString(retList["About"]);

                    date = general.GetFormattedDate(date);

                    returnSB.AppendLine(gui.GrayFontStart + "User: " + gui.GrayFontEnd + queryStringUID);
                    returnSB.AppendLine(gui.LineBreak);
                    returnSB.AppendLine(gui.GrayFontStart + "Created: " + gui.GrayFontEnd + date);
                    returnSB.AppendLine(gui.LineBreak);
                    //  //  Keep E-Mail Private.
                    //  returnSB.AppendLine(gui.GrayFontStart + "EMail: " + gui.GrayFontEnd + eMail);
                    //  returnSB.AppendLine(gui.LineBreak);
                    returnSB.AppendLine(gui.GrayFontStart + "About: " + gui.GrayFontEnd + about);

                    userDetails = returnSB.ToString();
                }
                retList.Close();
            }

            if (string.IsNullOrEmpty(userDetails))
            {
                userDetails = gui.RedFontStart + "No such user found." + gui.RedFontEnd;
                
                SubmittedLB.Visible = false;
                CommentedLB.Visible = false;
                SavedLB.Visible = false;
                RatedLB.Visible = false;
            }


            UserDetailsLabel.Text = userDetails;
        }

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            loggedUID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());

            if (string.IsNullOrEmpty(loggedUID))
            {
                UpdateUserDetailsTable.Visible = false;
            }
            else if (loggedUID.Equals(queryStringUID))
            {
                UserDetailsLabel.Visible = false;
                UpdateUserDetailsTable.Visible = true;
                
                InterestsCBL.Visible = true;
                AddButton.Visible = true;
                SaveChangesLabel.Visible = true;


                if (!IsPostBack)
                {
                    GetPreferredCategories(loggedUID);

                    StringBuilder returnSB = new StringBuilder();

                    string date = string.Empty;
                    string eMail = string.Empty;
                    string about = string.Empty;


                    string queryString = "SELECT Date, EMail, About FROM getputs.user where UID='" + loggedUID + "';";
                    MySqlDataReader retList = dbOps.ExecuteReader(queryString);


                    if (retList != null && retList.HasRows)
                    {
                        while (retList.Read())
                        {
                            date = Convert.ToString(retList["Date"]);
                            eMail = Convert.ToString(retList["EMail"]);
                            about = Convert.ToString(retList["About"]);

                            date = general.GetFormattedDate(date);

                            UserLabelR.Text = loggedUID;
                            CreatedLabelR.Text = date;

                            AboutTB.Text = about;
                            EMailTB.Text = eMail;

                        }
                        retList.Close();
                    }
                }
            }
        }

        //  Whether to show the SavedLB or not.
        if (!string.IsNullOrEmpty(queryStringUID) && !string.IsNullOrEmpty(loggedUID))
        {
            if (queryStringUID.Equals(loggedUID))
            {
                SavedLB.Visible = true;
                RatedLB.Visible = true;
            }
            else
            {
                SavedLB.Visible = false;
                RatedLB.Visible = false;
            }
        }
        else
        {
            SavedLB.Visible = false;
            RatedLB.Visible = false;
        }

    }

    private void GetPreferredCategories(string loggedUID)
    {
        for (int i = 0; i < categories.CategoriesList.Count; i++)
        {
            string currentTag = categories.CategoriesList[i];
            if (!InterestsCBL.Items.Contains(new ListItem(currentTag, currentTag)))
            {
                InterestsCBL.Items.Add(new ListItem(currentTag, currentTag));
            }
        }

        string preferredCategories = string.Empty;

        string queryString = "SELECT PreferredCategories FROM getputs.user where UID='" + loggedUID + "';";
        MySqlDataReader retList;
        retList = dbOps.ExecuteReader(queryString);
        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                if (retList["PreferredCategories"] != null)
                {
                    preferredCategories = Convert.ToString(retList["PreferredCategories"]);
                }

            }
            retList.Close();
        }

        List<string> userCategories = new List<string>();

        if (!string.IsNullOrEmpty(preferredCategories))
        {
            string[] splitter = { "," };
            string[] pCategories = preferredCategories.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < pCategories.Length; i++)
            {
                if (categories.CategoriesList.Contains(pCategories[i]))
                {
                    userCategories.Add(pCategories[i]);
                }
            }
        }

        if (userCategories != null && userCategories.Count > 0)
        {
            for (int j = 0; j < InterestsCBL.Items.Count; j++)
            {
                ListItem interestsCB = InterestsCBL.Items.FindByValue(InterestsCBL.Items[j].Value);
                if (userCategories.Contains(InterestsCBL.Items[j].Value))
                {
                    interestsCB.Selected = true;
                }
            }
        }

        for (int i = 0; i < InterestsCBL.Items.Count; i++)
        {
            InterestsCBL.Items[i].Attributes.Add("onmouseover", "this.style.backgroundColor='gainsboro'");
            InterestsCBL.Items[i].Attributes.Add("onmouseout", "this.style.backgroundColor='white'");
        }
    }



    /*
    private string GetUserDetails(string queryStringUID)
    {
        StringBuilder returnSB = new StringBuilder();

        string queryString = "SELECT Date, EMail, About FROM getputs.user where UID='" + queryStringUID + "';";
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        string date = string.Empty;
        string eMail = string.Empty;
        string about = string.Empty;

        if (retList != null && retList.HasRows)
            while (retList.Read())
            {
                date = Convert.ToString(retList["Date"]);
                eMail = Convert.ToString(retList["EMail"]);
                about = Convert.ToString(retList["About"]);

                date = general.GetFormattedDate(date);
                
                returnSB.AppendLine(gui.GrayFontStart + "User: " + gui.GrayFontEnd + queryStringUID);
                returnSB.AppendLine(gui.LineBreak);
                returnSB.AppendLine(gui.GrayFontStart + "Created: " + gui.GrayFontEnd + date);
                returnSB.AppendLine(gui.LineBreak);
                //  //  Keep E-Mail Private.
                //  returnSB.AppendLine(gui.GrayFontStart + "EMail: " + gui.GrayFontEnd + eMail);
                //  returnSB.AppendLine(gui.LineBreak);
                returnSB.AppendLine(gui.GrayFontStart + "About: " + gui.GrayFontEnd + about);
            }                
        retList.Close();

        
        return returnSB.ToString();
    }
    */


    protected void AddButton_Click(object sender, EventArgs e)
    {
        SaveChangesLabel.Text = String.Empty;

        string userCategories = string.Empty;

        for (int i = 0; i < InterestsCBL.Items.Count; i++)
        {
            if (InterestsCBL.Items[i].Selected)
            {
                if (userCategories != string.Empty)
                {
                    userCategories = userCategories + "," + InterestsCBL.Items[i].Value;
                }
                else
                {
                    userCategories = InterestsCBL.Items[i].Value;
                }
            }
        }

        string queryString = "UPDATE getputs.user SET PreferredCategories = '" + userCategories + "' WHERE UID = '" + loggedUID + "';";
        int retVal = dbOps.ExecuteNonQuery(queryString);

        if (retVal < 0)
        {
            SaveChangesLabel.Text = gui.RedFontStart + "Please Try Again" + gui.RedFontEnd;
        }
        else
        {
            SaveChangesLabel.Text = gui.GreenFontStart + "Your Preferences have been Saved." + gui.GreenFontEnd;
        }
    }
        
    protected void UpdateButton_Click(object sender, EventArgs e)
    {        
            MessageLabel.Text = string.Empty;

            string UID = loggedUID;
            //  string password = PasswordTB.Text.Trim();
            //  string confirmPassword = ConfirmPasswordTB.Text.Trim();
            string email = EMailTB.Text.Trim();
            string about = AboutTB.Text.Trim();

            UID = general.EscapeCharacters(UID);
            //  password = general.EscapeCharacters(password);
            //  confirmPassword = general.EscapeCharacters(confirmPassword);
            email = general.EscapeCharacters(email);
            about = general.EscapeCharacters(about);

            bool isValid = false;
            string errorMessage = "";
            int userRegistrationCode = 0;

            if (!string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(email))
            {
                //if (UID.Length < 3)
                //{
                //    errorMessage = "UserID should be atleast 3 characters long";
                //}
                //else if (password.Length < 3)
                //{
                //    errorMessage = "Password should be atleast 3 characters";
                //}
                //else if (!general.IsAlphabetOrNumber(UID))
                //{
                //    errorMessage = gui.RedFontStart + "UserID can only contain Alphabets & Numbers." + gui.RedFontEnd;
                //}
                //else if (!general.IsAlphabetOrNumber(password))
                //{
                //    errorMessage = gui.RedFontStart + "Password can only contain Alphabets & Numbers." + gui.RedFontEnd;
                //}
                //else if ((email.Length < 4) || (!general.IsValidEMail(email)))
                //{
                //    errorMessage = gui.RedFontStart + "Enter a valid E-Mail Address." + gui.RedFontEnd;
                //}

                if ((email.Length < 4) || (!general.IsValidEMail(email)))
                {
                    errorMessage = gui.RedFontStart + "Enter a valid E-Mail Address." + gui.RedFontEnd;
                }

                else
                {
                    //if (password == confirmPassword)
                    //{
                    //    //  Generate the Hashed Password.

                    //    string passwordHash = password;
                    //    if (isDoPasswordHashing == 1)
                    //    {
                    //        passwordHash = dbOps.HashPassword(password);
                    //    }
                    //    //  userRegistrationCode = CreateUser(username, password, email, expertiseLevel);
                    //    userRegistrationCode = CreateUser(UID, passwordHash, email, about);

                    //    if (userRegistrationCode == 0)
                    //        isValid = true;
                    //    else if (userRegistrationCode == 1)
                    //    {
                    //        errorMessage = "User Already Exists. Please Try a different UserID";
                    //    }
                    //    else
                    //    {
                    //        errorMessage = "Please Try Again";
                    //    }
                    //}


                    string queryString = "UPDATE getputs.user SET EMail='" + email + "' , About='" + about + "' WHERE UID='" + UID + "';";
                    userRegistrationCode = dbOps.ExecuteNonQuery(queryString);

                    if (userRegistrationCode < 0)
                    {
                        errorMessage = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                    }
                    else
                    {
                        errorMessage = gui.GreenFontStart + "Your Account has been Updated." + gui.GreenFontEnd;
                    }
                }
            }
            else
            {
                isValid = false;
                errorMessage = gui.RedFontStart + "Please fill out all the required fields." + gui.RedFontEnd;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageLabel.Text = errorMessage;
            }

            EMailTB.Text = email;
            AboutTB.Text = about;        
    }
    
    protected void SubmittedLB_Click(object sender, EventArgs e)
    {
        string link = links.SubmittedPageLink + "?UID=" + queryStringUID;
        Response.Redirect(link, true);
    }


    protected void CommentedLB_Click(object sender, EventArgs e)
    {
        string link = links.CommentedPageLink + "?UID=" + queryStringUID;
        Response.Redirect(link, true);
    }
    
    protected void SavedLB_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.SavedPageLink, true);
    }

    protected void RatedLB_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.RatedPageLink, true);
    }


}