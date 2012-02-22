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
using System.Collections;
using System.Collections.Specialized;

using MySql.Data.MySqlClient;
using System.Collections.Generic;

/// <summary>
/// Summary description for ProcessingEngine
/// </summary>
public class ProcessingEngine
{
    DBOperations dbOps;
    General general;
    Logger log;

    private static volatile ProcessingEngine instance;
    private static object syncRoot = new Object();

    //  Count the number of clicks for each Item. Dictionary[Key: IID; Value: ClickCount]
    private Dictionary<int, int> _clickCount;   //  The scheduler will dump this into the DB every hour.
    private Dictionary<int, List<ClickData>> _clickDataDB;  //  //  The scheduler will dump this into the DB every hour.
   
    private List<string> _popularCategories;

    private bool _isDebug = int.Parse(ConfigurationManager.AppSettings["IsDebug"]) == 0 ? false : true;


    private bool _isEMailOn = int.Parse(ConfigurationManager.AppSettings["isEMailOn"]) == 0 ? false : true;

    //  Limit the DB Retrieval to these many Rows.
    private int _maxItems = int.Parse(ConfigurationManager.AppSettings["MaxItems"]);
    private int _itemsPerPage = int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"]);

    private bool _isSpamReportingOn = int.Parse(ConfigurationManager.AppSettings["Spam_isSpamReportingOn"]) == 0 ? false : true;
    private int _spamMinVotes = int.Parse(ConfigurationManager.AppSettings["Spam_MinVotes"]);


    //  Get the number of rows in the item Table, used by RandomArticleLinkButton.
    private int _itemRowCount = 0;

    //  Rating_IsShowRating != 0 => On; Rating_IsShowRating == 0 => Off
    private bool _isShowRating = int.Parse(ConfigurationManager.AppSettings["Rating_IsShowRating"]) == 0 ? false : true;

    private string _itemLinkStartSeperator = ConfigurationManager.AppSettings["ItemLinkStartSeperator"];
    private string _itemLinkEndSeperator = ConfigurationManager.AppSettings["ItemLinkEndSeperator"];


    public Dictionary<int, int> ClickCount
    {
        get
        {
            return _clickCount;
        }
        set
        {
            _clickCount = value;
        }
    }

    public Dictionary<int, List<ClickData>> ClickDataDB
    {
        get
        {
            return _clickDataDB;
        }
        set
        {
            _clickDataDB = value;
        }
    }
            

    public List<string> PopularCategories
    {
        get
        {
            return _popularCategories;
        }
        set
        {
            _popularCategories = value;
        }
    }



    public bool IsDebug
    {
        get
        {
            return _isDebug;
        }
    }

    public bool IsEMailOn
    {
        get
        {
            return _isEMailOn;
        }
    }


    public int MaxItems
    {
        get
        {
            return _maxItems;
        }
    }

    public int ItemsPerPage
    {
        get
        {
            return _itemsPerPage;
        }
    }


    public bool IsSpamReportingOn
    {
        get
        {
            return _isSpamReportingOn;
        }
    }

    public int SpamMinVotes
    {
        get
        {
            return _spamMinVotes;
        }
    }

    public int ItemRowCount
    {
        get
        {
            return _itemRowCount;
        }
        set
        {
            _itemRowCount = value;
        }
    }

    public bool IsShowRating
    {
        get
        {
            return _isShowRating;
        }
        set
        {
            _isShowRating = value;
        }
    }

    
    /// <summary>
    /// For Counting Clicks, the URL will be wrapped between these seperators
    /// Example: ItemLinkStartSeperator + Item.Link + ItemLinkEndSeperator
    /// The ItemLinkStartSeperator and ItemLinkEndSeperator will be replaced by and Empty String in R.aspx
    /// </summary>
    public string ItemLinkStartSeperator
    {
        get
        {
            return _itemLinkStartSeperator;
        }
    }

    /// <summary>
    /// For Counting Clicks, the URL will be wrapped between these seperators
    /// Example: ItemLinkStartSeperator + Item.Link + ItemLinkEndSeperator
    /// The ItemLinkStartSeperator and ItemLinkEndSeperator will be replaced by and Empty String in R.aspx
    /// </summary>
    public string ItemLinkEndSeperator
    {
        get
        {
            return _itemLinkEndSeperator;
        }
    }
   


    private ProcessingEngine()
    {
        dbOps = DBOperations.Instance;
        general = General.Instance;
        log = Logger.Instance;

        //  Dictionary[Key: IID; Value=ClickCount]
        _clickCount = new Dictionary<int, int>();
        _clickDataDB = new Dictionary<int, List<ClickData>>();
        _popularCategories = new List<string>();
    }

    /// <summary>
    /// Initialize a new instance of the ProcessingEngine class, If required.
    /// </summary>
    public static ProcessingEngine Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ProcessingEngine();
                }
            }
            return instance;
        }
    }

    public enum Sort
    {
        Hot,
        New
    }


    public enum Category
    {
         General,
         World_News,
         Ask_a_Query,
         Advertise,
         Buy_Sell,
         Recommend,
         Career,
         Job_Opportunity,
         Matrimonial,
         Politics,
         Sports,
         Cricket,        
         Music,
         Bollywood,
         Hollywood,
         Business,
         Finance,
         Investment,
         Real_Estate,
         NRI,
         Education,
         Technology,
         Software, 
         Startups,  
         Programming,
         Travel,
         Art,
         Science,
         Funny, 
         Health,
         Food,
         Dating,
         Automobile,
         Random,
         Women
    }

    /// <summary>
    /// Get the Type of Sorting (Hot or New) to be used.
    /// </summary>
    /// <param name="sortStr">"h" -> Hot, "n" -> New</param>
    /// <returns>The Type of Sorting (Hot or New) to be used.</returns>
    public Sort GetSortType(string sortStr)
    {
        Sort sortType = Sort.Hot;
        if (!string.IsNullOrEmpty(sortStr))
        {
            sortStr = sortStr.Trim().ToLower();

            if (sortStr.Equals("h"))
            {
                sortType = Sort.Hot;
            }
            else if (sortStr.Equals("n"))
            {
                sortType = Sort.New;
            }
            else
            {
                sortType = Sort.Hot;
            }
        }
        return sortType;
    }

    /// <summary>
    /// Get the Sort String based on the Type of Sorting.
    /// </summary>
    /// <param name="sort">ProcessingEngine.Sort (Hot or New)</param>
    /// <returns>"n" if Sort.New, "h" otherwise.</returns>
    public string GetSortString(Sort sort)
    {
        return (sort == ProcessingEngine.Sort.New ? "n" : "h");        
    }


    /// <summary>
    /// Update the In Memory Click Data Dictionary. The Dictionary will later on be dumped into the Database by the Schdeuler.
    /// </summary>
    /// <param name="iid">The IID of the Item.</param>
    /// <param name="uid">The UID of the person who viewed the Item. Could be an empty string.</param>
    /// <param name="ip">The IP Address.</param>
    /// <param name="date">The Date on which the Item was viewed.</param>
    /// <param name="clicks"></param>
    public void UpdateClickDataDictionary(int iid, string uid, string ip, string date, int clicks)
    {
        ////  Code for updating the ClickCount Dictionary.
        //if (engine.ClickCount.ContainsKey(iid))
        //{
        //    engine.ClickCount[iid] = engine.ClickCount[iid] + 1;
        //}
        //else
        //{
        //    engine.ClickCount.Add(iid, 1);
        //}

        try
        {

            if (_clickDataDB == null)
            {
                _clickDataDB = new Dictionary<int, List<ClickData>>();
            }

            ClickData cd = new ClickData();
            cd.IID = iid;
            cd.UID = uid;   //  Could be null/empty string.
            cd.IP = ip;
            cd.Date = date;
            cd.Clicks = clicks;

            List<ClickData> cdList;

            bool isDone = false;

            if (_clickDataDB.ContainsKey(iid))
            {
                //  cdList = _clickDataDB[iid];
                for (int i = 0; !isDone && i < _clickDataDB[iid].Count; i++)
                {
                    //  ClickData currentCD = _clickDataDB[iid][i];
                    if (!string.IsNullOrEmpty(_clickDataDB[iid][i].UID)
                        && !string.IsNullOrEmpty(cd.UID)
                        && _clickDataDB[iid][i].UID.Equals(cd.UID))
                    {
                        //  _clickDataDB[iid][i].Clicks = _clickDataDB[iid][i].Clicks + 1;
                        isDone = true;
                    }
                    else if (!string.IsNullOrEmpty(_clickDataDB[iid][i].IP)
                        && !string.IsNullOrEmpty(cd.IP)
                        && _clickDataDB[iid][i].IP.Equals(cd.IP))
                    {
                        //  _clickDataDB[iid][i].Clicks = _clickDataDB[iid][i].Clicks + 1;
                        isDone = true;
                    }
                }

                if (!isDone)
                {
                    //  cdList.Add(cd);
                    _clickDataDB[iid].Add(cd);


                    if (_clickCount.ContainsKey(iid))
                    {
                        _clickCount[iid] = _clickCount[iid] + 1;
                    }
                    else
                    {
                        _clickCount.Add(iid, 1);
                    }
                }

            }
            else
            {
                cdList = new List<ClickData>();
                cdList.Add(cd);
                _clickDataDB.Add(iid, cdList);

                if (_clickCount.ContainsKey(iid))
                {
                    _clickCount[iid] = _clickCount[iid] + 1;
                }
                else
                {
                    _clickCount.Add(iid, 1);
                }

            }
        }
        catch (Exception ex)
        {
            log.Log("Error in UpdateClickDataDictionary()");
            log.Log(ex);
        }
    }




    //  The Table getputs.item has the following schema:
    //  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]

    //  The Table getputs.user has the following schema:
    //  getputs.item [UID, Password, Date, EMail, About, Admin, Points]


    //  Not being Used Anywhere right now, but keep it. Might be useful later.
    //public User GetUser(string UID)
    //{
    //    if (!string.IsNullOrEmpty(UID))
    //    {
    //        User user = new User();

    //        user.UID = UID;

    //        //  UID, EMail, Admin, PreferredCategories, Spammer
    //        string queryString = "SELECT EMail, Admin, PreferredCategories, Spammer FROM user WHERE UID = '" + UID + "';";
    //        MySqlDataReader retList = dbOps.ExecuteReader(queryString);

    //        if (retList != null && retList.HasRows)
    //        {
    //            while (retList.Read())
    //            {
    //                user.EMail = Convert.ToString(retList["EMail"]);
    //                user.IsAdmin = Convert.ToString(retList["Admin"]) == "0" ? false : true;
    //                user.PreferredCategories = general.ConvertCSVToList(Convert.ToString(retList["PreferredCategories"]));
    //                user.IsSpammer = Convert.ToString(retList["Spammer"]) == "0" ? false : true;
    //            }
    //            retList.Close();
    //        }

    //        return user;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}




    public List<Item> LoadItemDB(Sort sortType)
    {
        List<Item> itemList = new List<Item>();
        
        //  select * from item order by DATE_FORMAT(date, '%Y-%m-%d %k:%i:%S') desc;
        //  string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category FROM getputs.item WHERE Spam = 0 ORDER BY Date DESC";
        
        //  Order By Date sometimes gives incorrect ordering.
        //  string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, NSaved, NEMailed FROM item WHERE Spam = 0 ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S') DESC LIMIT " + MaxItems.ToString() + ";";
        //  Order By IID
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, NSaved, NEMailed, Tags, AvgRating, NRated FROM item WHERE Spam = 0 ORDER BY IID DESC LIMIT " + MaxItems.ToString() + ";";
        MySqlDataReader retList;

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
        if (sortType == Sort.Hot)
        {
            itemList = SortItems(itemList, sortType);
        }

        return itemList;
        
    }



    public List<Item> LoadItemDB(Sort sortType, string UID)
    {
        List<Item> itemList = new List<Item>();

        //  select * from item order by DATE_FORMAT(date, '%Y-%m-%d %k:%i:%S') desc;
        //  string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category FROM getputs.item WHERE Spam = 0 ORDER BY Date DESC";
        
        //  Order By Date sometimes gives incorrect ordering.
        //  string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, NSaved, NEMailed FROM item WHERE UID='" + UID + "' AND Spam = 0 ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S') DESC LIMIT " + MaxItems.ToString() + ";";
        //  Order By IID
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, NSaved, NEMailed, Tags, AvgRating, NRated FROM item WHERE UID='" + UID + "' AND Spam = 0 ORDER BY IID DESC LIMIT " + MaxItems.ToString() + ";";

        MySqlDataReader retList;

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
        if (sortType == Sort.Hot)
        {
            itemList = SortItems(itemList, sortType);
        }

        return itemList;        
    }





    public List<Item> SortItems(List<Item> itemList, Sort sortType)
    {
        ItemSorter iSorter = new ItemSorter(sortType);
        itemList.Sort(iSorter);

        return itemList;
    }  

}



public class ItemSorter : IComparer<Item>
{
    ProcessingEngine.Sort _sort = ProcessingEngine.Sort.Hot;

    public ItemSorter(ProcessingEngine.Sort sortType)
    {
        _sort = sortType;
    }

    #region IComparer<Item> Members

    int IComparer<Item>.Compare(Item x, Item y)
    {
        if (_sort.Equals(ProcessingEngine.Sort.Hot))
        {
            if (x == y) return 0;
            if (x == null) return 1;
            else if (y == null) return -1;

            if (x.Marks < y.Marks)
                return 1;
            return -1;
        }
        else
        {
            if (x == y) return 0;
            if (x == null) return 1;
            else if (y == null) return -1;

            if (x.IID < y.IID)
                return 1;
            return -1;
        }
    }

    #endregion IComparer<Item> Members
}
