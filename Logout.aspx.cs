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

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {        
        
        //  Session.Abandon();

        HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
        if (getputsCookie != null)
        {
            getputsCookie.Expires = DateTime.Now.AddMinutes(-1);
            Response.Cookies.Add(getputsCookie);
        }
        //  stoockerCookie["username"] = null;   
    }
}
