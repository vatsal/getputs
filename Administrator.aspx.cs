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


public partial class Administrator : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];
    string UID;
    static string sortDir = "asc"; //  Default value of the Sort Direction of ItemGridView and UserGridView.

    int maxRows = int.Parse(ConfigurationManager.AppSettings["Admin_MaxRows"]);

    protected void Page_Load(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        engine = ProcessingEngine.Instance;

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(UID))
        {
            if (general.IsUserAdministrator(UID))
            {

            }
            else
            {
                Response.Redirect(links.FrontPageLink, false);
            }
        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists

        //if (!IsPostBack)
        //{
        //    string queryString = @"SELECT * FROM getputs.item;";
        //    PopulateItemGridView(queryString);
        //}

        if(!IsPostBack)
            MaxRowsTB.Text = maxRows.ToString();

    }

    
    private void PopulateItemGridView(string queryString)
    {           
        ItemGridView.DataSource = dbOps.ExecuteReader(queryString);
        ItemGridView.DataBind();      
    }


    private void PopulateUserGridView(string queryString)
    {
        UserGridView.DataSource = dbOps.ExecuteReader(queryString);
        UserGridView.DataBind();
    }


    //  The Table getputs.item has the following schema:
    //  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]

    //  The Table getputs.user has the following schema:
    //  getputs.item [UID, Password, Date, EMail, About, Admin, Points]



    protected void ItemGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {      
        ItemGridView.EditIndex = e.NewEditIndex;
        
        //  string queryString = "SELECT * FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, Tags, INET_NTOA(IP)  FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        PopulateItemGridView(queryString);       
    }

    protected void ItemGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        ItemGridView.EditIndex = -1;

        //  string queryString = "SELECT * FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, Tags, INET_NTOA(IP)  FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        PopulateItemGridView(queryString);
    }

    protected void ItemGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {        
        ItemGridView.EditIndex = -1;
    
        string IID;
        string title;
        string link;
        string text;
        string category;
        string spam;
        string tags;

        IID = title = link = text = category = spam = string.Empty;

        IID = ItemGridView.Rows[e.RowIndex].Cells[1].Text.Trim();
        title = ((TextBox)ItemGridView.Rows[e.RowIndex].Cells[2].Controls[0]).Text.Trim();
        link = ((TextBox)ItemGridView.Rows[e.RowIndex].Cells[3].Controls[0]).Text.Trim();
        text = ((TextBox)ItemGridView.Rows[e.RowIndex].Cells[4].Controls[1]).Text.Trim();
        category = ((TextBox)ItemGridView.Rows[e.RowIndex].Cells[8].Controls[0]).Text.Trim();
        spam = ((TextBox)ItemGridView.Rows[e.RowIndex].Cells[12].Controls[0]).Text.Trim();
        tags = ((TextBox)ItemGridView.Rows[e.RowIndex].Cells[15].Controls[0]).Text.Trim();

        title = general.EscapeCharacters(title);
        text = general.EscapeCharacters(text);
        tags = general.EscapeCharacters(tags);


        //  MessageLabel.Text = MessageLabel.Text + " -- " + link + " -- " + e.NewValues.Count.ToString();
        
        string queryString = "UPDATE item SET Title = '" + title + "' , Link = '" + link + "' , Text = '" + text + "', Category = '" + category + "' , Spam = " + spam + ", Tags='" + tags + "'"
            + " WHERE IID = " + IID + ";";

        dbOps.ExecuteNonQuery(queryString);
        ItemGridView.EditIndex = -1;
        ItemGridView.DataBind();

        //  queryString = "SELECT * FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, Tags, INET_NTOA(IP)  FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        PopulateItemGridView(queryString);        

    }

    protected void ItemGridView_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {        

    }    

    protected void ItemGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {        
        string IID = Server.HtmlEncode(ItemGridView.Rows[e.RowIndex].Cells[1].Text);
        
        string queryString = "DELETE FROM item WHERE IID=" + IID + ";";
        int retInt = dbOps.ExecuteNonQuery(queryString);

        if (retInt >= 0)
        {
            queryString = "DELETE FROM comments WHERE IID=" + IID + ";";
            retInt = dbOps.ExecuteNonQuery(queryString);

            if (retInt >= 0)
            {
                queryString = "DELETE FROM saveditems WHERE IID=" + IID + ";";
                dbOps.ExecuteNonQuery(queryString);
            }

            //  Still to decide. Should the item be removed from the list of spam items?
        }

        //  queryString = "SELECT * FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, Tags, INET_NTOA(IP)  FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();
        PopulateItemGridView(queryString);        
    }

    protected void ItemGridView_Sorting(object sender, GridViewSortEventArgs e)
    {                
        //  string queryString = "SELECT * FROM item ORDER BY " + e.SortExpression + " " + sortDir + " LIMIT " + maxRows.ToString() + ";";
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, Tags, INET_NTOA(IP)  FROM item ORDER BY " + e.SortExpression + " " + sortDir + " LIMIT " + maxRows.ToString() + ";";
        PopulateItemGridView(queryString);

        //  Toggle the Sort Order.
        if (sortDir.Equals("asc"))
        {
            sortDir = "desc";
        }
        else if (sortDir.Equals("desc"))
        {
            sortDir = "asc";
        }       
        //  MessageLabel.Text = "sort: " + sortDir.ToString();
    }

    //  The Table getputs.item has the following schema:
    //  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]

    //  The Table getputs.user has the following schema:
    //  getputs.item [UID, Password, Date, EMail, About, Admin, Points]


    protected void UserGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        UserGridView.EditIndex = e.NewEditIndex;
        string queryString = "SELECT * FROM user LIMIT " + maxRows.ToString();
        PopulateUserGridView(queryString);

    }

    protected void UserGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        UserGridView.EditIndex = -1;

        string queryString = "SELECT * FROM user LIMIT " + maxRows.ToString();
        PopulateUserGridView(queryString);
    }

    protected void UserGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        UserGridView.EditIndex = -1;

        string UID = string.Empty;
        string adminStr = string.Empty;
        string pointsStr = string.Empty;

        string spammerStr = string.Empty;

        

        //                                  0       1       2   3       4       5       6           7               8
        //  Grid will be displayed as [Edit/Cancel, UID, Date, EMail, About, Admin, Points, PreferredCategories, Spammer]

        UID = UserGridView.Rows[e.RowIndex].Cells[1].Text.Trim();
        adminStr = ((TextBox)UserGridView.Rows[e.RowIndex].Cells[5].Controls[0]).Text.Trim();
        pointsStr = ((TextBox)UserGridView.Rows[e.RowIndex].Cells[6].Controls[0]).Text.Trim();
        spammerStr = ((TextBox)UserGridView.Rows[e.RowIndex].Cells[8].Controls[0]).Text.Trim();

        int adminInt;
        int pointsInt;
        int spammerInt;

        string queryString = string.Empty;

        if (int.TryParse(adminStr, out adminInt) && int.TryParse(pointsStr, out pointsInt) && int.TryParse(spammerStr, out spammerInt))
        {
            if (adminInt == 0 || adminInt == 1)
            {
                //  queryString = "UPDATE user SET Admin = " + adminInt + " , Points = " + pointsInt + " WHERE UID = '" + UID + "';";
                queryString = "UPDATE user SET Admin = " + adminInt + " , Points = " + pointsInt + " , Spammer = " + spammerInt + " WHERE UID = '" + UID + "';";

                dbOps.ExecuteNonQuery(queryString);
                UserGridView.EditIndex = -1;
                UserGridView.DataBind();
            }
            else
            {
                MessageLabel.Text = "Invalid Input. Admin Section can only have a value of 0 or 1";
            }

        }
        else
        {
            MessageLabel.Text = "Invalid Input";
        }

        queryString = "SELECT * FROM user LIMIT " + maxRows.ToString();
        PopulateUserGridView(queryString);

    }

    protected void UserGridView_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {

    }

    protected void UserGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string UID = Server.HtmlEncode(UserGridView.Rows[e.RowIndex].Cells[1].Text);

        string queryString = "DELETE FROM user WHERE UID = '" + UID + "';";
        int retInt = dbOps.ExecuteNonQuery(queryString);

        if (retInt >= 0)
        {
            //  Set all of the items submitted by this UID as Spam.
            queryString = "UPDATE item SET Spam = 1 WHERE UID = '" + UID + "';";
            dbOps.ExecuteNonQuery(queryString);
        }

        queryString = "SELECT * FROM user LIMIT " + maxRows.ToString();
        PopulateUserGridView(queryString);
    }

    protected void UserGridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        string queryString = "SELECT * FROM user ORDER BY " + e.SortExpression + " " + sortDir + " LIMIT " + maxRows.ToString() + ";";
        PopulateUserGridView(queryString);

        //  Toggle the Sort Order.
        if (sortDir.Equals("asc"))
        {
            sortDir = "desc";
        }
        else if (sortDir.Equals("desc"))
        {
            sortDir = "asc";
        }
        //  MessageLabel.Text = "sort: " + sortDir.ToString();
    }
    




    protected void ItemHyperLink_Click(object sender, EventArgs e)
    {
        MessageLabel.Text = "";

        bool isInt = int.TryParse(MaxRowsTB.Text.Trim(), out maxRows);
        if (isInt)
        {

        }
        else
        {
            MessageLabel.Text = gui.RedFontStart + "Invalid Input" + gui.RedFontEnd;
        }

        ItemGridView.Visible = true;
        UserGridView.Visible = false;

        //  if (!IsPostBack)
        {
            //  string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();            
            string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, Tags, INET_NTOA(IP) FROM item ORDER BY Date DESC LIMIT " + maxRows.ToString();            
            PopulateItemGridView(queryString);
        }
    }
    protected void UserHyperLink_Click(object sender, EventArgs e)
    {
        MessageLabel.Text = "";

        bool isInt = int.TryParse(MaxRowsTB.Text.Trim(), out maxRows);
        if (isInt)
        {

        }
        else
        {
            MessageLabel.Text = gui.RedFontStart + "Invalid Input" + gui.RedFontEnd;
        }

        UserGridView.Visible = true;
        ItemGridView.Visible = false;
        
        //  if (!IsPostBack)
        {
            string queryString = "SELECT * FROM user LIMIT " + maxRows.ToString();
            PopulateUserGridView(queryString);
        }
    }
    
    protected void GSBUpdateLink_Click(object sender, EventArgs e)
    {
        GSBUpdateOutputLabel.Text = string.Empty;

        //  The api is not that good right now. Will wait for the next version.

        try
        {
            Subkismet.Services.GoogleSafeBrowsing.GoogleSafeBrowsing gsb = new Subkismet.Services.GoogleSafeBrowsing.GoogleSafeBrowsing();

            // Update phishing black list
            bool phishingBool = gsb.UpdateList(Subkismet.Services.GoogleSafeBrowsing.BlackListType.Phishing);

            // Update malware black list
            bool malwareBool = gsb.UpdateList(Subkismet.Services.GoogleSafeBrowsing.BlackListType.Malware);

            GSBUpdateOutputLabel.Text = "Updated the Safe Browsing Lists"
                + gui.LineBreak + "Phishing: " + phishingBool.ToString()
                + gui.LineBreak + "Malware: " + malwareBool.ToString()
                + gui.LineBreak;
               

        }
        catch (Exception ex)
        {
            GSBUpdateOutputLabel.Text = "Exception while Updating the Safe Browsing Lists"
                + gui.LineBreak + gui.LineBreak
                + "Message: " + ex.Message
                + gui.LineBreak + gui.LineBreak
                + "Stack Trace: " + ex.StackTrace;
        }
    }
    
    protected void GSBTestLink_Click(object sender, EventArgs e)
    {
        GSBTestOutputLabel.Text = string.Empty;

        //  The api is not that good right now. Will wait for the next version.

        string url = GSBTestInputTB.Text.Trim();
        if (!string.IsNullOrEmpty(url))
        {

            try
            {
                Subkismet.Services.GoogleSafeBrowsing.GoogleSafeBrowsing gsb = new Subkismet.Services.GoogleSafeBrowsing.GoogleSafeBrowsing();

                Subkismet.Services.GoogleSafeBrowsing.CheckResultType resultType = gsb.CheckLink(url);

                GSBTestOutputLabel.Text = "Output: "
                    + "Is this link valid: " + resultType.ToString()
                    + gui.LineBreak;

            }
            catch (Exception ex)
            {
                GSBTestOutputLabel.Text = "Exception while Testing the Safe Browsing Lists"
                    + gui.LineBreak + gui.LineBreak
                    + "Message: " + ex.Message
                    + gui.LineBreak + gui.LineBreak
                    + "Stack Trace: " + ex.StackTrace;
            }
        }
        else
        {
            GSBTestOutputLabel.Text = "Please Enter a valid URL."
                   + gui.LineBreak;
                  
        }
    }

    protected void ReloadListsLink_Click(object sender, EventArgs e)
    {
        ReloadListsOutputLabel.Text = string.Empty;
        string message = string.Empty;

        for (int i = 0; i < ReloadListsCBL.Items.Count; i++)
        {
            if (ReloadListsCBL.Items[i].Selected)
            {
                string value = ReloadListsCBL.Items[i].Value;
                switch (value)
                {
                    case "BadUsers":
                        general.ReloadBadUsersLists();
                        message += "BadUsers List Reloaded." + gui.LineBreak;
                        break;
                    case "BadIPs":
                        general.ReloadBadIPsLists();
                        message += "BadIPs List Reloaded." + gui.LineBreak;
                        break;
                    case "BadSites":
                        general.ReloadBadSitesLists();
                        message += "BadSites List Reloaded." + gui.LineBreak;
                        break;
                    case "BadDomains":
                        general.ReloadBadDomainsLists();
                        message += "BadDomains List Reloaded." + gui.LineBreak;
                        break;
                    case "TagLexicon":
                        general.ReloadTagLexicon();
                        message += "TagLexicon List Reloaded." + gui.LineBreak;
                        break;
                    default:
                        general.ReloadBadUsersLists();
                        general.ReloadBadIPsLists();
                        general.ReloadBadSitesLists();
                        general.ReloadBadDomainsLists();
                        message += "All Lists Reloaded." + gui.LineBreak;
                        break;
                }
            }
        }

        ReloadListsOutputLabel.Text = message;

    }
}

