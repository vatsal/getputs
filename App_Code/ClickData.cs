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

//  ClickData DB [IID, UID, IP, Date, Count]

/// <summary>
/// Summary description for ClickData
/// </summary>
public class ClickData
{
    private int iid;
    private string uid;
    private string ip;
    private string date;
    private int clicks;

    public int IID
    {
        get
        {
            return iid;
        }
        set
        {
            iid = value;
        }
    }

    public string UID
    {
        get
        {
            return uid;
        }
        set
        {
            uid = value;
        }
    }

    public string IP
    {
        get
        {
            return ip;
        }
        set
        {
            ip = value;
        }
    }

    public string Date
    {
        get
        {
            return date;
        }
        set
        {
            date = value;
        }
    }

    public int Clicks
    {
        get
        {
            return clicks;
        }
        set
        {
            clicks = value;
        }
    }
    
    public ClickData()
    {
                
    }




}
