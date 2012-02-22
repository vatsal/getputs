//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Data;
using System.Configuration;
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
using System.Collections;

//  DB Details
//  rating (IID, UID, CID, Rating, Date, IP);
//  item (IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam, NSaved, NEMailed, IP, Tags, LastCommentDate, AvgRating, NRated);



public partial class SubmitRating : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    Categories categories;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    Tagger tagger;
    Logger log;

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string UID = string.Empty;
    string seperator = " | ";

    int startItem = 0;  //  Default Value;   

    //  ProcessingEngine.Sort sort = ProcessingEngine.Sort.Hot;
    ProcessingEngine.Sort sort = ConfigurationManager.AppSettings["Default_Sort"] == "h" ? ProcessingEngine.Sort.Hot : ProcessingEngine.Sort.New;


    private static int itemClicks = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        categories = Categories.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        tagger = Tagger.Instance;
        log = Logger.Instance;

        seperator = gui.Seperator;

        //  QueryString Param Names.
        //  ratingID
        //  value

        string iid = string.Empty;
        string value = string.Empty;


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


        if (Request.QueryString != null && Request.QueryString.Count > 0)
        {
            //  ratingID is the Item IID.
            if (!string.IsNullOrEmpty(Request.QueryString["ratingID"]))
            {
                iid = Request.QueryString["ratingID"];
            }
            //  Value is the Rating given by the user. Value: [0, 1, 2, 3, 4]. So add +1 so as to convert Value: [1, 2, 3, 4, 5]
            if (!string.IsNullOrEmpty(Request.QueryString["value"]))
            {
                int intValue = -1;
                value = int.TryParse(Request.QueryString["value"], out intValue) ? (intValue + 1).ToString() : "-1";
            }
        }


        if (!string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(iid) && !string.IsNullOrEmpty(value))
        {
            UpdateRatings(UID, iid, value);
        }
        



    }

    /// <summary>
    /// Update the Database with the Votes, provided all the filtering results are positive.
    /// </summary>
    /// <param name="UID">The UID of the Logged In User.</param>
    /// <param name="iid">The Item ID.</param>
    /// <param name="value">The value of the vote.</param>
    private void UpdateRatings(string UID, string iid, string value)
    {
        /*
            IF UID has already rated IID THEN
                Don't do anything
            ELSE
                Insert rating into rating Table
                Update item Table
          
          
            AvgRating = (AvgRating * NRated + NewRating) / (NRated + 1 );
            NRated = NRated + 1;
        */

        try
        {

            string queryString = string.Empty;

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ip = general.GetIP(this.Request);

            queryString = "SELECT COUNT(*) FROM rating WHERE IID = " + iid + " AND UID='" + UID + "';";
            int count = dbOps.ExecuteScalar(queryString);

            if (count == 0)     //  Insert rating into rating Table, Update item Table.
            {
                queryString = "INSERT INTO rating(IID, UID, Rating, Date, IP) VALUE ( " + iid + ", '" + UID + "', " + value + ", '" + date + "', INET_ATON('" + ip + "'));";
                int retInt = dbOps.ExecuteNonQuery(queryString);

                if (retInt >= 0)    //  Success in executing the Insert Statement for rating Table, Now update the item Table.
                {
                    //  queryString = UPDATE item SET AvgRating = ( AvgRating * NRated + 4 ) / (NRated + 1) , NRated = NRated + 1 WHERE IID = 900;
                    queryString = "UPDATE item SET AvgRating = ( AvgRating * NRated + " + value + " ) / (NRated + 1) , NRated = NRated + 1 WHERE IID = " + iid + ";";
                    retInt = dbOps.ExecuteNonQuery(queryString);
                }
            }
            else    //  Don't Do Anything.
            {

            }
        }
        catch (Exception ex)
        {
            log.Log("Error while updating the Ratings." + " UID: " + UID + ", IID: " + iid + ", value: " + value);
            log.Log(ex);
        }

    }
}
