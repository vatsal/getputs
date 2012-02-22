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
using System.Collections.Generic;
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for User
/// </summary>
public class User
{
    Logger log = Logger.Instance;
    DBOperations dbOps = DBOperations.Instance;

    private string _uid;
    private string _eMail;
    private bool _isAdmin;
    private List<string> _preferredCategories;
    private List<int> _savedItems;
    private Dictionary<string, string> _filteredList;

    private bool _isSpammer;

    public string UID
    {
        get
        {
            return _uid;
        }
        set
        {
            _uid = value;
        }
    }

    public string EMail
    {
        get
        {
            return _eMail;
        }
        set
        {
            _eMail = value;
        }
    }

    public bool IsAdmin
    {
        get
        {
            return _isAdmin;
        }
        set
        {
            _isAdmin = value;
        }
    }

    public List<string> PreferredCategories
    {
        get
        {
            return _preferredCategories;
        }
        set
        {
            _preferredCategories = value;
        }
    }

    public List<int> SavedItems
    {
        get
        {
            return _savedItems;
        }
        set
        {
            _savedItems = value;
        }
    }

    public Dictionary<string, string> FilteredList
    {
        get
        {
            return _filteredList;
        }
        set
        {
            _filteredList = value;
        }
    }

    public bool IsSpammer
    {
        get
        {
            return _isSpammer;
        }
        set
        {
            _isSpammer = value;
        }
    }


	public User()
	{
    
	}

    public User(string UID)
    {
        //  No need to populate anything here, populate this directly from the database.
        //_uid = UID;
        //_isAdmin = IsUserAdministrator(_uid);        
        //_preferredTags = new List<string>();
        //_savedItems = GetSavedItems(_uid);
        //_filteredList = new Dictionary<string, string>();
        
    }

    private bool IsUserAdministrator(string UID)
    {
        bool isAdmin = false;
        string queryString = "SELECT Admin FROM user WHERE UID ='" + UID + "';";
        int intResult = dbOps.ExecuteScalar(queryString);
        if (intResult != -1)
        {
            if (intResult == 1)
            {
                isAdmin = true;
            }
        }
        return isAdmin;
    }

    

   

}


