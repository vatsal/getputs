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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;



/// <summary>
/// Summary description for Tags
/// </summary>
public class Categories
{
    private static volatile Categories instance;
    private static object syncRoot = new Object();


    private string _categoriesString = ConfigurationManager.AppSettings["CategoriesTable"];
    Hashtable _categoryColorsHashTable = (Hashtable)ConfigurationManager.GetSection("CategoryColorsTable");
    //  Tags for which Direct E-Mail Contact between Submitter and Commenter is supported.
    private string _categoriesDirectContactString = ConfigurationManager.AppSettings["CategoriesTableDirectContact"];

    private List<string> _categoriesList;
    private List<string> _categoriesListDirectContact;


    public List<string> CategoriesList
    {
        get
        {
            return _categoriesList;
        }
    }

    public Hashtable CategoryColorsHashTable
    {
        get
        {
            return _categoryColorsHashTable;
        }
    }

    public List<string> CategoriesListDirectContact
    {
        get
        {
            return _categoriesListDirectContact;
        }
    }


	public Categories()
	{
        string[] delimiter = new string[1];
        delimiter[0] = ",";

        _categoriesList = new List<string>();
        
        string[] categoriesArray = _categoriesString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < categoriesArray.Length; i++)
        {
            _categoriesList.Add(categoriesArray[i].Trim());
        }

        _categoriesList.Sort();

        _categoriesListDirectContact = new List<string>();
        string[] categoriesArrayDirectContact = _categoriesDirectContactString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < categoriesArrayDirectContact.Length; i++)
        {
            _categoriesListDirectContact.Add(categoriesArrayDirectContact[i].Trim());
        }
        
	}

    /// <summary>
    /// Initialize a new instance of the Categories class, If required.
    /// </summary>
    public static Categories Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new Categories();
                }
            }
            return instance;
        }
    }
}

