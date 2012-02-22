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

public partial class Commented : System.Web.UI.Page
{
    Links links;
    Logger log;
    GUIVariables gui;
    DBOperations dbOps;
    Categories categories;
    General general;
    ProcessingEngine engine;
    ImageEngine imageEngine;

    string queryStringUID = string.Empty;

    string seperator = " | ";

    protected void Page_Load(object sender, EventArgs e)
    {
        links = Links.Instance;
        gui = GUIVariables.Instance;
        dbOps = DBOperations.Instance;
        categories = Categories.Instance;
        log = Logger.Instance;
        engine = ProcessingEngine.Instance;
        general = General.Instance;
        imageEngine = ImageEngine.Instance;

        seperator = gui.Seperator;

        if (string.IsNullOrEmpty(Request.QueryString["UID"]))
        {

        }
        else
        {
            queryStringUID = Request.QueryString["UID"].Trim().ToLower();
        }

        if (string.IsNullOrEmpty(queryStringUID))
        {

        }
        else
        {            
            LoadComments(queryStringUID);
        }
    }

    private void LoadComments(string queryStringUID)
    {
        MessageLabel.Text = string.Empty;
        string message = string.Empty;

        //  string queryString = "SELECT CID, UID, Date, Comment FROM comments WHERE UID='" + queryStringUID + "' ORDER BY Date DESC LIMIT 25;";
        //  SELECT comments.CID, comments.UID, comments.Date, comments.Comment , item.Title FROM comments, item WHERE comments.UID='vatsal' AND comments.IID = item.IID ORDER BY Date DESC LIMIT 25;
        string queryString = "SELECT comments.CID, comments.UID, comments.Date, comments.Comment , item.Title, item.Link FROM comments, item WHERE comments.UID = '" + queryStringUID + "' AND comments.IID = item.IID ORDER BY Date DESC LIMIT 25;";
        MySqlDataReader retList;

        retList = dbOps.ExecuteReader(queryString);

        int commentNumber = 0;
        if (retList != null && retList.HasRows)
        {
            while (retList.Read())
            {
                HtmlTableRow row = new HtmlTableRow();

                commentNumber++;
                string commentString = gui.GrayFontStart + commentNumber.ToString() + gui.GrayFontEnd + gui.HTMLTab;
                HtmlTableCell commentNumberCell = new HtmlTableCell();
                commentNumberCell.Controls.Add(new LiteralControl(commentString));
                
                HtmlTableCell commentDetailsCell = new HtmlTableCell();

                string cid = Convert.ToString(retList["CID"]);
                string uid = Convert.ToString(retList["UID"]);
                string date = Convert.ToString(retList["Date"]);
                string comment = Convert.ToString(retList["Comment"]);
                string title = Convert.ToString(retList["Title"]);
                string link = Convert.ToString(retList["Link"]);

                comment = gui.GrayFontStart + general.ProcessTextForDisplay(comment) + gui.GrayFontEnd;

                //LinkButton userLB = new LinkButton();
                ////  userLB.Click += new EventHandler(userLB_Click);
                //userLB.Text = uid;
                //userLB.CssClass = "CSS_Submitter";
                //commentDetailsCell.Controls.Add(userLB);
                //commentDetailsCell.Controls.Add(new LiteralControl(seperator));
                
                string userHL = "<a class='CSS_Submitter' onmouseover = \"this.style.textDecoration='underline'\" onmouseout = \"this.style.textDecoration='none'\" href='" + links.UserDetailsPageLink.Replace("~\\", "") + "?UID=" + uid + "' >" + uid + "</a>";
                commentDetailsCell.Controls.Add(new LiteralControl(userHL + seperator));
                

                string dateFormatString = gui.SmallFontStart + gui.GrayFontStart + general.GetFormattedDate(date) + gui.GrayFontEnd + gui.SmallFontEnd;
                Label dateLabel = new Label();
                dateLabel.Text = dateFormatString;
                dateLabel.CssClass = "CSS_SubmissionFreshness";

                commentDetailsCell.Controls.Add(dateLabel);


                commentDetailsCell.Controls.Add(new LiteralControl(seperator));
                string itemLink = "<a href='" + link + "' class='CSS_LinkItem'>" + title + "</a>";
                commentDetailsCell.Controls.Add(new LiteralControl(itemLink));

                ////  No need to provide Admin Control on this Page.

                ////  'user' is the User who is currently logged in.
                ////  'uid' is the User who submitted the comment.
                //bool isAdmin = general.IsUserAdministrator(user);
                //if (isAdmin)
                //{
                //    commentDetailsCell.Controls.Add(new LiteralControl(seperator));

                //    //  Provide a delete button for the administrator.
                //    LinkButton deleteLB = new LinkButton();
                //    deleteLB.Click += new EventHandler(deleteLB_Click);
                //    deleteLB.Text = "Delete";
                //    deleteLB.CssClass = "CSS_Submitter";

                //    deleteLB.Attributes.Add("IID", iid);
                //    deleteLB.Attributes.Add("CID", cid);
                //    deleteLB.Attributes.Add("UID", uid);   //  'uid' is the User who submitted the comment.

                //    commentDetailsCell.Controls.Add(deleteLB);

                //    commentDetailsCell.Controls.Add(new LiteralControl(seperator));
                //}

                commentDetailsCell.Controls.Add(new LiteralControl(gui.LineBreak));
                commentDetailsCell.Controls.Add(new LiteralControl(comment));

                row.Controls.Add(commentNumberCell);
                row.Controls.Add(commentDetailsCell);
                CommentDiv.Rows.Add(row);


                HtmlTableRow blankRow = new HtmlTableRow();
                HtmlTableCell blankCell = new HtmlTableCell();
                blankCell.Controls.Add(new LiteralControl(gui.LineBreak));
                blankRow.Controls.Add(blankCell);
                CommentDiv.Rows.Add(blankRow);

                //  CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));
                //  CommentDiv.Controls.Add(new LiteralControl(gui.LineBreak));

            }
            retList.Close();

            message = gui.GreenFontStart + "Most recent comments by " + queryStringUID + gui.GreenFontEnd;
        }
        else
        {
            message = gui.GreenFontStart + queryStringUID + " has not made any comments yet." + gui.GreenFontEnd;
        }

        MessageLabel.Text = message;
    }
}
