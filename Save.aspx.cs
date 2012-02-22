//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

//  Save.aspx: If the User is Logged In and if an IID is provided in the queryString, Then Save the Item into the SavedItems DB

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

public partial class Save : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    Categories categories;
    ProcessingEngine engine;
    Logger log;

    string UID = string.Empty;
    string url = string.Empty;
    int iid;

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        categories = Categories.Instance;
        engine = ProcessingEngine.Instance;
        log = Logger.Instance;

        UID = string.Empty;
        url = string.Empty;

        //  log.Log("PathInfo: " + Request.PathInfo);

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }

        if (string.IsNullOrEmpty(UID))
        {
            Response.Redirect(links.LoginLink);
        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists


        //  Update the SavedItems DB        
        if (!string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(Request.QueryString["iid"]))
        {
            string IID = Request.QueryString["iid"];

            //  Make sure that the IID is a valid integer.
            if (general.IsValidInt(IID))
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string queryString = "INSERT INTO saveditems VALUES ('" + UID + "' , " + IID + " , '" + date + "');";
                int retInt = dbOps.ExecuteNonQuery(queryString);

                //  Also update the getputs.item NSaved Field.
                queryString = "UPDATE item SET NSaved=NSaved+1 WHERE IID=" + IID + ";";
                retInt = dbOps.ExecuteNonQuery(queryString);

                Response.Redirect(Context.Request.UrlReferrer.OriginalString);
            }
            else
            {
                Response.Redirect(links.FrontPageLink);
            }
        }
        else
        {
            Response.Redirect(links.FrontPageLink);
        }

    }
}
