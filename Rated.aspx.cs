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

public partial class Rated : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    Categories categories;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    ItemDisplayer itemDisplayer;
    

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string UID = string.Empty;
    string seperator = " | ";

    int startItem = 0;  //  Default Value;

    //  ProcessingEngine.Sort sort = ProcessingEngine.Sort.Hot;
    ProcessingEngine.Sort sort = ConfigurationManager.AppSettings["Default_Sort"] == "h" ? ProcessingEngine.Sort.Hot : ProcessingEngine.Sort.New;

    private static int itemClicks = 0;

    ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Flow;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Columns;
    //  ItemDisplayer.ItemLayoutOptions itemLayoutOptions = ItemDisplayer.ItemLayoutOptions.Categorized;

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        categories = Categories.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        itemDisplayer = ItemDisplayer.Instance;

        seperator = gui.Seperator;

        if (!string.IsNullOrEmpty(Request.QueryString["startItem"]))
        {
            bool isStartItemInt = int.TryParse(Request.QueryString["startItem"].Trim(), out startItem);
            if (!isStartItemInt)
            {
                startItem = 0;
            }

            if (startItem < 0)
            {
                startItem = 0;
            }
        }
        else
        {
            startItem = 0;
        }

        if (!string.IsNullOrEmpty(Request.QueryString["sort"]))
        {
            string sortStr = Convert.ToString(Request.QueryString["sort"]);
            sort = engine.GetSortType(sortStr);
        }

        if (Request.Cookies["getputsLayoutCookie"] != null)
        {
            HttpCookie getputsLayoutCookie = Request.Cookies["getputsLayoutCookie"];
            itemLayoutOptions = itemDisplayer.GetItemLayoutOptionsType(dbOps.Decrypt(getputsLayoutCookie["layout"].ToString().Trim()));
        }


        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            UID = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(UID))
        {
            Response.Redirect(links.LoginLink, false);
        }
        else
        {
            MessageLabel.Text = gui.GreenFontStart + "Items Rated by " + UID + gui.GreenFontEnd;
        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists

        
        LoadItemDB(sort);
    }

    private void LoadItemDB(ProcessingEngine.Sort sortType)
    {
        List<Item> itemList = new List<Item>();

        //  SELECT * FROM item where IID = ( SELECT IID FROM saveditems where UID = 'vatsal');
        //  SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks FROM getputs.item WHERE IID = ( SELECT IID FROM saveditems WHERE UID = 'vatsal') ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S') DESC LIMIT 25;
        string queryString = "SELECT IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, NSaved, NEMailed, Tags, AvgRating, NRated FROM item WHERE IID IN ( SELECT IID FROM rating where UID = '" + UID + "') ORDER BY DATE_FORMAT(Date, '%Y-%m-%d %k:%i:%S') DESC LIMIT " + engine.MaxItems.ToString() + ";";

        
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

        if (sortType == ProcessingEngine.Sort.Hot)
        {
            itemList = engine.SortItems(itemList, sortType);
        }
                
        ItemDisplayer.ShowItemsOptions itemOptions = ItemDisplayer.ShowItemsOptions.ShowUIDLink
            | ItemDisplayer.ShowItemsOptions.ShowTime
            | ItemDisplayer.ShowItemsOptions.ShowCategoryLink
            | ItemDisplayer.ShowItemsOptions.ShowSaveLink
            | ItemDisplayer.ShowItemsOptions.ShowEMailLink
            | ItemDisplayer.ShowItemsOptions.ShowCommentsLink
            | ItemDisplayer.ShowItemsOptions.ShowImage
            | ItemDisplayer.ShowItemsOptions.ShowRatings
            | ItemDisplayer.ShowItemsOptions.ShowTags
            | ItemDisplayer.ShowItemsOptions.CountClicks
            | ItemDisplayer.ShowItemsOptions.ShowPreviousNextLinks;
        
        string itemTable = itemDisplayer.LoadItemTable(itemList, itemOptions, itemLayoutOptions, startItem, UID, sort, links.RatedPageLink.Replace("~\\", ""));
        ItemDiv.InnerHtml = itemTable;
        

    }


}
