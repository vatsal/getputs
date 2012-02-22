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
/// Summary description for SearchEngine
/// </summary>
public class SearchEngine
{
    private static volatile SearchEngine instance;
    private static object syncRoot = new Object();

    DBOperations dbOps = DBOperations.Instance;
    Tokenizer tokenizer = new Tokenizer();
    General general = General.Instance;

    private int _maxSearchResults = int.Parse(ConfigurationManager.AppSettings["Search_MaxResults"]);

	private SearchEngine()
	{        

	}

    /// <summary>
    /// Initialize a new instance of the SearchEngine class, If required.
    /// </summary>
    public static SearchEngine Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new SearchEngine();
                }
            }
            return instance;
        }
    }


    public enum SearchType
    {
        All,
        Title,
        Links,
        Comments
    }

    public enum SearchTime
    {
        Today,
        Week,
        Month,
        Year
    }

    public List<Item> LoadSearchResults(string query)
    {
        List<Item> itemList = new List<Item>();

        string queryString = "SELECT * FROM item WHERE Title LIKE '%" + query + "%' LIMIT " + _maxSearchResults + ";";
        MySqlDataReader retList = dbOps.ExecuteReader(queryString);

        retList = dbOps.ExecuteReader(queryString);

        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                Item item = new Item();
                item.IID = Convert.ToInt32(retList["IID"]);
                item.Title = Convert.ToString(retList["Title"]);
                item.Link = Convert.ToString(retList["Link"]);
                item.Text = Convert.ToString(retList["Text"]);
                item.Date = Convert.ToString(retList["Date"]);
                item.UID = Convert.ToString(retList["UID"]);
                item.NComments = Convert.ToInt32(retList["NComments"]);
                item.Category = Convert.ToString(retList["Category"]);
                item.Clicks = Convert.ToInt32(retList["Clicks"]);
                item.NSaved = Convert.ToInt32(retList["NSaved"]);
                item.NEMailed = Convert.ToInt32(retList["NEMailed"]);

                item.TagString = Convert.ToString(retList["Tags"]);
                item.TagList = general.ConvertCSVToList(item.TagString);

                item.AvgRating = Convert.ToDouble(retList["AvgRating"]);
                item.NRated = Convert.ToInt32(retList["NRated"]);

                item.Age = item.GetAge(item.Date);
                //  item.Marks = item.GetMarks(item.Clicks, item.NComments, item.Age);
                item.Marks = item.GetMarks(item);



                if (!string.IsNullOrEmpty(item.Link))
                {
                    item.Text = string.Empty;
                }

                itemList.Add(item);
            }
            retList.Close();
        }       

        return itemList;
        //  ItemDiv.InnerHtml = LoadItems(itemList);
        //  LoadItemTable(itemList);

    }



    public List<Item> LoadSearchResults(string query, SearchType sType, string sCategory, SearchTime sTime)
    {
        List<Item> itemList = new List<Item>();

        List<string> queryTokens = tokenizer.GetTokens(query, true, true);

        List<int> alreadyAddedIIDs = new List<int>();

        if (queryTokens != null)
        {
            for (int i = 0; i < queryTokens.Count; i++)
            {
                string currentQueryToken = queryTokens[i];

                string queryString = QueryBuilder(currentQueryToken, sType, sCategory, sTime);
                Logger log = Logger.Instance;
                log.Log("Search QueryString: " + queryString);
                //  queryString = "SELECT * FROM item WHERE Title LIKE '%" + query + "%' LIMIT " + _maxSearchResults + ";";

                //  string queryString = "SELECT * FROM item WHERE Title LIKE '%" + query + "%' LIMIT " + _maxSearchResults + ";";
                MySqlDataReader retList = dbOps.ExecuteReader(queryString);

                retList = dbOps.ExecuteReader(queryString);

                if (retList != null && retList.HasRows)
                {
                    while (retList.Read())
                    {
                        Item item = new Item();
                        item.IID = Convert.ToInt32(retList["IID"]);
                        item.Title = Convert.ToString(retList["Title"]);
                        item.Link = Convert.ToString(retList["Link"]);
                        item.Text = Convert.ToString(retList["Text"]);
                        item.Date = Convert.ToString(retList["Date"]);
                        item.UID = Convert.ToString(retList["UID"]);
                        item.NComments = Convert.ToInt32(retList["NComments"]);
                        item.Category = Convert.ToString(retList["Category"]);
                        item.Clicks = Convert.ToInt32(retList["Clicks"]);
                        item.NSaved = Convert.ToInt32(retList["NSaved"]);
                        item.NEMailed = Convert.ToInt32(retList["NEMailed"]);

                        item.TagString = Convert.ToString(retList["Tags"]);
                        item.TagList = general.ConvertCSVToList(item.TagString);

                        item.AvgRating = Convert.ToDouble(retList["AvgRating"]);
                        item.NRated = Convert.ToInt32(retList["NRated"]);

                        item.Age = item.GetAge(item.Date);
                        //  item.Marks = item.GetMarks(item.Clicks, item.NComments, item.Age);
                        item.Marks = item.GetMarks(item);



                        if (!string.IsNullOrEmpty(item.Link))
                        {
                            item.Text = string.Empty;
                        }

                        if (!alreadyAddedIIDs.Contains(item.IID))
                        {
                            alreadyAddedIIDs.Add(item.IID);
                            itemList.Add(item);
                        }                        
                    }
                    retList.Close();
                }
            }

            return itemList;
            //  ItemDiv.InnerHtml = LoadItems(itemList);
            //  LoadItemTable(itemList);
        }
        else
        {
            return null;
        }
    }

    private string QueryBuilder(string query, SearchType sType, string sCategory, SearchTime sTime)
    {
        
        string queryString = string.Empty;

        string tableClause = string.Empty;
        string columnClause = string.Empty;
        string categoryClause = string.Empty;
        string timeClause = string.Empty;

        string dateRange = string.Empty;
        
        
        switch (sType)
        {
            case SearchType.All:
                tableClause = "item";
                break;
            case SearchType.Comments:
                tableClause = "comments";
                break;
            case SearchType.Links:
                tableClause = "item";
                break;
            case SearchType.Title:
                tableClause = "item";
                break;
            default:
                tableClause = "item";
                break;
        }

        switch (sType)
        {
            case SearchType.All:
                columnClause = "Title";
                break;
            case SearchType.Comments:
                columnClause = "Comment";
                break;
            case SearchType.Links:
                columnClause = "Link";
                break;
            case SearchType.Title:
                columnClause = "Title";
                break;
            default:
                columnClause = "Title";
                break;
        }


        int days = 0;

        switch (sTime)
        {
            case SearchTime.Month:
                days = 30;
                break;
            case SearchTime.Today:
                days = 1;
                break;
            case SearchTime.Week:
                days = 7;
                break;
            case SearchTime.Year:
                days = 365;
                break;
            default:
                days = 7;
                break;
        }

        DateTime rangeDT = DateTime.Now.Subtract(new TimeSpan(days, 0, 0, 0));
        timeClause = rangeDT.ToString("yyyy-MM-dd HH:mm:ss");

        string categoryExpression = string.Empty;
        

        if (sType == SearchType.Comments)
        {
            //  SELECT * FROM comments where IID in (Select IID From item where category = 'business');
            if (sCategory != "All Categories")
            {
                categoryExpression = " IID IN (SELECT IID FROM item WHERE Category = '" + sCategory + "') " + " AND ";
            }
            queryString = "SELECT * FROM " + tableClause + " WHERE " + categoryExpression + columnClause + " LIKE '%" + query + "%' " + " AND " + " Date > DATE_FORMAT('" + timeClause + "', '%Y-%m-%d %k:%i:%S')" + " LIMIT " + _maxSearchResults + ";";
        }
        else
        {
            if (sCategory != "All Categories")
            {
                categoryExpression = " Category = '" + sCategory + "'" + " AND ";
            }
            //  string queryString = "SELECT * FROM item WHERE Title LIKE '%" + query + "%' LIMIT " + _maxSearchResults + ";";
            queryString = "SELECT * FROM " + tableClause + " WHERE " + categoryExpression + columnClause + " LIKE '%" + query + "%' " + " AND " + " Date > DATE_FORMAT('" + timeClause + "', '%Y-%m-%d %k:%i:%S')" + " LIMIT " + _maxSearchResults + ";";
        }

        return queryString;
    }




}
