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

using System.Net;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public partial class RssPage : System.Web.UI.Page
{
    Links links;
    Logger log;
    GUIVariables gui;
    DBOperations dbOps;
    Categories categories;
    ProcessingEngine engine;

    protected void Page_Load(object sender, EventArgs e)
    {
        links = Links.Instance;
        gui = GUIVariables.Instance;
        dbOps = DBOperations.Instance;
        categories = Categories.Instance;
        log = Logger.Instance;
        engine = ProcessingEngine.Instance;

        WriteRSSFile();
        Response.Redirect(links.RssFeedLink, false);
    }

    protected void WriteRSSFile()
    {
        try
        {
            string fileName = links.RssFeedLink.Replace(@"~\", ""); ;
            string filePath = HttpRuntime.AppDomainAppPath + fileName;
                        
            XmlWriter xml = new XmlTextWriter(new StreamWriter(filePath));
            RssWriter rss = new RssWriter(xml);


            List<Item> itemList = engine.LoadItemDB(ProcessingEngine.Sort.Hot);

            rss.WriteHeader("getputs.com", "http://www.getputs.com", "All Your News Belong To Us!", null);

            foreach (Item item in itemList)
            {
                rss.WriteItem(item);             
            }

            rss.Close();
            xml.Close();

            
        }
        catch (Exception ex)
        {
            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Error in RssPage.aspx");
                log.Log(ex);
            }
        }
    }


    #region Example Usage
    /*
    static void WriteWholeFile()
    {
        XmlWriter xml = new XmlTextWriter(new StreamWriter("myfeed.xml"));
        RssWriter rss = new RssWriter(xml);

        rss.WriteHeader("getputs.com", "http://www.getputs.com", "News", null);
        rss.WriteItem(
            "First item",
            "Hi! Here's the <b>first</b> item",
            new System.Uri("http://xyz.com/1.html"),
            new DateTime(1999, 12, 1));

        rss.WriteItem(
            "Second item",
            "Here's the <b>2nd</b> item.",
            new System.Uri("http://xyz.com/2.html"),
            new DateTime(1988, 10, 4));
        rss.Close();
        xml.Close();
    }
    */
    #endregion Example Usage


    
}
