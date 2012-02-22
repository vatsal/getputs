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

using Subkismet;

/// <summary>
/// Summary description for SpamDetection
/// </summary>
public class SpamDetection
{
    private static volatile SpamDetection instance;
    private static object syncRoot = new Object();

    Subkismet.Services.GoogleSafeBrowsing.GoogleSafeBrowsing gsb;

	public SpamDetection()
	{
        gsb = new Subkismet.Services.GoogleSafeBrowsing.GoogleSafeBrowsing();
	}

    /// <summary>
    /// Initialize a new instance of the SpamDetection class, If required.
    /// </summary>
    public static SpamDetection Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new SpamDetection();
                }
            }
            return instance;
        }
    }


    public bool VerifyAkismetApiKey()
    {
        string apiKey = "07a1c1f0edc1";
        Akismet<Comment> akismet = Akismet<Comment>.CreateSimpleAkismet(apiKey, new Uri("http://www.getputs.com/"));
        return akismet.VerifyApiKey(); 
    }

    public bool IsSpam(string commentAuthor, string commentIP, string commentContent)
    {
        string apiKey = "07a1c1f0edc1";
        

        Akismet<Comment> akismet = Akismet<Comment>.CreateSimpleAkismet(apiKey, new Uri("http://www.getputs.com/"));
        if (akismet.VerifyApiKey())
        {
            System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(commentIP);

            Subkismet.Comment comment = new Subkismet.Comment(ipAddress, "user-agent");
            comment.Author = commentAuthor;
            comment.CommentType = "comment";
            comment.Content = commentContent;

            return akismet.IsSpam(comment);
        }
        else
        {
            return false;
        }
    }


    //  The api is not that good right now. Will wait for the next version.
    public bool IsSafeLink(string link)
    {
        // Create an instance of the GoogleSafeBrowsing
                        
        // Peform a check and get the number of bad URLs
        Subkismet.Services.GoogleSafeBrowsing.CheckResultType resultType = gsb.CheckLink(link);
        
        // Display the number of bad URLs
        if (resultType == Subkismet.Services.GoogleSafeBrowsing.CheckResultType.Safe)
        {
            //  Safe
            return true;
        }
        else
        {
            //  Malware or Phishing
            return false;
        }
        
    }


}
