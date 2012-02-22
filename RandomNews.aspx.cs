//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

//  RandomNews.aspx: Redirect to a Random News Item everytime this page is loaded.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class RandomNews : System.Web.UI.Page
{
    ProcessingEngine engine;
    DBOperations dbOps;
    Links links;

    protected void Page_Load(object sender, EventArgs e)
    {
        engine = ProcessingEngine.Instance;
        dbOps = DBOperations.Instance;
        links = Links.Instance;
        Random random = new Random();

        bool isDone = false;
        string link = string.Empty;

        //string queryString = "SELECT COUNT(*) FROM item WHERE Spam = 0;";
        //int retInt = dbOps.ExecuteScalar(queryString);

        int retInt = engine.ItemRowCount;

        if (retInt > 0)
        {
            int rowCount = retInt;
            int rowNumber = random.Next(0, rowCount);

            while (!isDone)
            {
                string queryString = "SELECT Link FROM item WHERE Spam = 0 AND IID = " + rowNumber + ";";
                MySqlDataReader retList = dbOps.ExecuteReader(queryString);

                if (retList != null && retList.HasRows)
                {
                    //   DB Hit.                     
                    while (retList.Read())
                    {
                        link = Convert.ToString(retList["Link"]);
                    }
                    retList.Close();
                    isDone = true;
                }
                else
                {
                    //   DB Miss. Select another random number.
                    rowNumber = random.Next(0, rowCount);
                }
            }

            if (isDone && !string.IsNullOrEmpty(link))
            {
                Response.Redirect(link, true);
            }
        }
        else
        {
            //  Not decided what to do.
            //  May be redirect back to getputs.
            Response.Redirect(links.FrontPageLink, true);
        }
    }
}
