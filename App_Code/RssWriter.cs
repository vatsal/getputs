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

using System.Xml;
using System.IO;

// Class to write out RSS
// Expected Usage:
//  rss = new RSSWriter(...);
//  rss.WriteHeader(...);
//    rss.WriteItem(..);
//    rss.WriteItem(..);
//    rss.WriteItem(..);
//  rss.Close();

//  Validate feeds by URL: 
//  http://feedvalidator.org
//  http://validator.w3.org/feed



/// <summary>
/// Summary description for RSSWriter
/// </summary>
public class RssWriter
{
	XmlWriter m_writer;

    Links links;
    GUIVariables gui;

    public RssWriter(XmlWriter tw)
    {
        m_writer = tw;

        links = Links.Instance;
        gui = GUIVariables.Instance;
    }

    // Write header for RSS Feed
    // Parameters:
    // title - Pretty title of the RSS feed. Eg "My Pictures"
    // link - optional URL to link RSS feed back to a web page.
    // description - more verbose human-readable description of this feed.
    // generator - optional string for 'generator' tag.
    public void WriteHeader(string title, string link, string description, string generator)
    {
        m_writer.WriteStartElement("rss");
        m_writer.WriteAttributeString("version", "2.0");
        m_writer.WriteStartElement("channel");
        m_writer.WriteElementString("title", title);

        if (link != null)
        {
            m_writer.WriteElementString("link", link); // link to generated report.
        }
        m_writer.WriteElementString("description", description);

        if (generator != null)
        {
            m_writer.WriteElementString("generator", generator);
        }

        m_writer.WriteElementString("lastBuildDate", ConvertDate(new DateTime()));

    }

    // Write out an item.
    // title - title of the blog entry
    // content - main body of the blog entry
    // link - link the blog entry back to a webpage.
    // time - date for the blog entry.
    public void WriteItem(string title, string content, System.Uri link, DateTime time)
    {
        m_writer.WriteStartElement("item");
        WriteItemBody(m_writer, title, content, link, time);
        m_writer.WriteEndElement(); // item
    }

    // Write just the body (InnerXml) of a new item
    private void WriteItemBody(XmlWriter w, string title, string content, System.Uri link, DateTime time)
    {
        w.WriteElementString("title", title);
        if (link != null) { w.WriteElementString("link", link.ToString()); }
        w.WriteElementString("description", content); // this will escape
        w.WriteElementString("pubDate", ConvertDate(time));
    }










    // Write out an item.
    // title - title of the blog entry
    // content - main body of the blog entry
    // link - link the blog entry back to a webpage.
    // time - date for the blog entry.
    public void WriteItem(Item item)
    {
        m_writer.WriteStartElement("item");
        WriteItemBody(m_writer, item);
        m_writer.WriteEndElement(); // item
    }

    // Write just the body (InnerXml) of a new item
    private void WriteItemBody(XmlWriter w, Item item)
    {
        w.WriteElementString("title", item.Title);
        if (!string.IsNullOrEmpty(item.Link))
        {
            w.WriteElementString("link", item.Link);
        }
        else
        {
            string fileName = links.ItemDetailsPageLink.Replace(@"~\", "") + "?IID=" + item.IID.ToString();            
            string filePath = links.DomainLink + fileName;
            w.WriteElementString("link", filePath );
        }

        string commentsfileName = links.ItemDetailsPageLink.Replace(@"~\", "") + "?IID=" + item.IID.ToString();
        string commentsfilePath = links.DomainLink + commentsfileName;

        //  <![CDATA[<a href="http://news.ycombinator.com/item?id=189830">Comments</a>]]>
        string comment = string.Empty;

        if (item.NComments == 0)
        {
            comment = @"<a href='" + commentsfilePath + @"'>" + " Comment" + @"</a>";
            //  comment = gui.CDataStart + @"<a href='" + commentsfilePath + @"'>" + item.NComments.ToString() + " Comment" + @"</a>" + gui.CDataEnd;
        }
        else if (item.NComments == 1)
        {
            comment = @"<a href='" + commentsfilePath + @"'>" + item.NComments.ToString() + " Comment" + @"</a>";
            //  comment = gui.CDataStart + @"<a href='" + commentsfilePath + @"'>" + item.NComments.ToString() + " Comment" + @"</a>" + gui.CDataEnd;
        }
        else
        {
            comment = @"<a href='" + commentsfilePath + @"'>" + item.NComments.ToString() + " Comments" + @"</a>";
            //  comment = gui.CDataStart + @"<a href='" + commentsfilePath + @"'>" + item.NComments.ToString() + " Comments" + @"</a>" + gui.CDataEnd;
        }        
        
        w.WriteElementString("description", comment);
         
        DateTime dtItem;
        bool isDate = DateTime.TryParse(item.Date, out dtItem);
        w.WriteElementString("pubDate", ConvertDate(dtItem));
    }














    // Close out the RSS stream.
    // This does not close the underlying XML writer.
    public void Close()
    {
        m_writer.WriteEndElement(); // channel
        m_writer.WriteEndElement(); // rss
    }

    // Convert a DateTime into the format needed for RSS (from RFC 822).
    // This looks like: "Wed, 04 Jan 2006 16:03:00 GMT"
    private string ConvertDate(DateTime t)
    {

        // See this for help on format string
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconcustomdatetimeformatstrings.asp 
        DateTime t2 = t.ToUniversalTime();
        return t2.ToString(@"ddd, dd MMM yyyy HH:mm:ss G\MT");
    }
}
