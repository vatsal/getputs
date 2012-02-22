//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

//  Redirect through R.aspx if you want to store the clicks made by a user on an item.
//  This page has code that is currently incomplete.

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

//  This page will handle the redirects.
//  The Request should contain a queryString named 'url'

public partial class R : System.Web.UI.Page
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

        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists

        //  Update the Clicks DB

        //  2008-11-18
        //  Only store clicks which come from Logged In Users.        
        //  if (!string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(Request.QueryString["url"]) && !string.IsNullOrEmpty(Request.QueryString["iid"]))

        //  2009-03-18
        //  Store all clicks. Whether from logged in users or from readers who have not logged in.
        if (!string.IsNullOrEmpty(Request.QueryString["url"]) && !string.IsNullOrEmpty(Request.QueryString["iid"]))
        {
            //  url = Request.QueryString["url"].Trim();
            url = Request.Url.OriginalString.Substring(Request.Url.OriginalString.IndexOf(engine.ItemLinkStartSeperator), Request.Url.OriginalString.IndexOf(engine.ItemLinkEndSeperator) - Request.Url.OriginalString.IndexOf(engine.ItemLinkStartSeperator));
            url = url.Replace(engine.ItemLinkStartSeperator, string.Empty).Replace(engine.ItemLinkEndSeperator, string.Empty);
            
            bool isIIDInt = int.TryParse(Request.QueryString["iid"].Trim(), out iid);

            string ip = general.GetIP(this.Request);
            if (!general.IsBadIP(ip))   //  Only count the clicks if the IP is OK.
            {
                if (!string.IsNullOrEmpty(url) && isIIDInt && url.StartsWith("~\\"))    //  Internal Redirect.
                {
                    url = url.Replace("~\\", links.DomainLink);
                    engine.UpdateClickDataDictionary(iid, UID, general.GetIP(this.Request), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);

                }
                else if (!string.IsNullOrEmpty(url) && general.IsValidURL(url) && isIIDInt) //  External Redirect.
                {
                    engine.UpdateClickDataDictionary(iid, UID, general.GetIP(this.Request), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);

                }
            }            
        }


        //  Redirect
        if (!string.IsNullOrEmpty(Request.QueryString["url"]) && !string.IsNullOrEmpty(Request.QueryString["iid"]))
        {
            //  url = Request.QueryString["url"].Trim();
            url = Request.Url.OriginalString.Substring(Request.Url.OriginalString.IndexOf(engine.ItemLinkStartSeperator), Request.Url.OriginalString.IndexOf(engine.ItemLinkEndSeperator) - Request.Url.OriginalString.IndexOf(engine.ItemLinkStartSeperator));
            url = url.Replace(engine.ItemLinkStartSeperator, string.Empty).Replace(engine.ItemLinkEndSeperator, string.Empty);

            bool isIIDInt = int.TryParse(Request.QueryString["iid"].Trim(), out iid);
            if (!string.IsNullOrEmpty(url) && isIIDInt && url.StartsWith("~\\"))    //  Internal Redirect.
            {
                url = url.Replace("~\\", links.DomainLink);
                Response.Redirect(url, true);
            }
            else if (!string.IsNullOrEmpty(url) && general.IsValidURL(url) && isIIDInt) //  External Redirect.
            {
                Response.Redirect(url, true);
            }
            else
            {
                Response.Redirect(links.FrontPageLink, true);
            }
        }
        else
        {
            Response.Redirect(links.FrontPageLink, true);
        }

        

        
    }
}
