//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Data;
using System.Configuration;
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
using MySql.Data.MySqlClient;
using System.Net;
using System.Collections.Generic;

public partial class EditComment : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;
    ImageEngine imageEngine;
    Categories categories;
    Tagger tagger;

    string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];

    string seperator = " | ";
    private static int itemClicks = 0;

    string iid = string.Empty;
    string user = string.Empty; //  Logged In User, different from uid.
    string uid = string.Empty; //   The user who submitted the item. 
    string cid = string.Empty;

    Item item;

    int _minTokensInComment = 2;   //  Minimum words required in the Comments Text.
    int _maxTokensInComment = 500;   //  Minimum words required in the Comments Text.

    protected void Page_Load(object sender, EventArgs e)
    {
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        categories = Categories.Instance;
        tagger = new Tagger();

        seperator = gui.Seperator;

        #region CookieAlreadyExists
        //  START: If a getputsCookie with the Username already exists, do not show the Login Page.

        if (Request.Cookies["getputsCookie"] != null)
        {
            HttpCookie getputsCookie = Request.Cookies["getputsCookie"];
            user = dbOps.Decrypt(getputsCookie["UID"].ToString().Trim());
        }
        if (string.IsNullOrEmpty(user))
        {
            //  Response.Redirect(links.LoginLink, false);
            MessageLabel.Text = gui.RedFontStart + "Please login to enter a comment." + gui.RedFontEnd;
        }
        else
        {

        }
        //  END: If a getputsCookie with the Username already exists, do not show the Login Page.
        #endregion CookieAlreadyExists


        if (string.IsNullOrEmpty(Request.QueryString["uid"]) || string.IsNullOrEmpty(Request.QueryString["iid"]) || string.IsNullOrEmpty(Request.QueryString["cid"]))
        {            
            Response.Redirect(links.FrontPageLink, true);
        }
        else
        {
            uid = Request.QueryString["uid"].Trim();
            iid = Request.QueryString["iid"].Trim();
            cid = Request.QueryString["cid"].Trim();
        }


        if (!general.IsValidInt(iid) || !general.IsValidInt(cid) || !uid.Equals(user))
        {
            Response.Redirect(links.FrontPageLink, true);
        }
        else
        {
            string comment = LoadComment(uid, iid, cid);

            if (string.IsNullOrEmpty(comment))
            {
                Response.Redirect(links.FrontPageLink, true);
            }
            else
            {
                MessageLabel.Text = gui.GreenFontStart + "Your comment cannot be edited."
                    + gui.LineBreak + "However, you can append more details to your previous comment." + gui.GreenFontEnd;

                CurrentCommentLabel.Text = gui.GreenFontStart + "Your comment: " + gui.GreenFontEnd + gui.LineBreak + comment;
            }
        }    

    }

    private string LoadComment(string uid, string iid, string cid)
    {
        string comment = string.Empty;

        string queryString = "SELECT Comment FROM comments WHERE IID = " + iid + " AND uid = '" + uid + "' AND cid = " + cid + ";";
        
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                comment = Convert.ToString(retList["Comment"]);
            }
        }
        return comment;
    }

    protected void EditCommentButton_Click(object sender, EventArgs e)
    {
        string message = string.Empty;
        string editedComment = EditCommentTB.Text.Trim();

        if (!string.IsNullOrEmpty(editedComment))
        {
            string editCommentSeperator = gui.LineBreak + gui.LineBreak + gui.GrayFontStart + uid + " made the following update on " + DateTime.Now.ToString() + gui.GrayFontEnd + gui.LineBreak + gui.LineBreak;
            editedComment = general.EscapeCharacters(editedComment);

            List<string> editedCommentTokens = tagger.GetTokens(editedComment, false, false, false);
            if (editedCommentTokens.Count >= _minTokensInComment && editedCommentTokens.Count <= _maxTokensInComment)
            {

                //  UPDATE comments SET Comment = CONCAT(Comment, "that is good") WHERE IID = 447 AND uid = 'vatsal' AND cid = 1;
                string queryString = "UPDATE comments SET Comment = CONCAT(Comment, ' " + editCommentSeperator + editedComment + "') WHERE IID = " + iid + " AND uid = '" + uid + "' AND cid = " + cid + ";";
                int retInt = dbOps.ExecuteNonQuery(queryString);

                if (retInt >= 0)
                {
                    message = gui.GreenFontStart + "New details have been appended to your comment successfully." + gui.GreenFontEnd;
                    EditCommentTB.Text = string.Empty;
                }
                else
                {
                    message = gui.RedFontStart + "Please Try Again." + gui.RedFontEnd;
                }
            }
            else
            {
                if (editedCommentTokens.Count < _minTokensInComment)
                {
                    message = gui.RedFontStart + "Too few words to append to your comment." + gui.RedFontEnd;
                }
                else if(editedCommentTokens.Count > _maxTokensInComment)
                {
                    message = gui.RedFontStart + "Too many words to append to your comment." + gui.RedFontEnd;
                }
            }
        }
        else
        {
            message = gui.RedFontStart + "The Comment cannot be empty" + gui.RedFontEnd;
        }
        MessageLabel.Text = message;
    }
}
