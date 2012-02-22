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
using System.Threading;
using System.Collections.Specialized;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Summary description for Scheduler
/// </summary>
public class Scheduler
{
    private SchedulerConfiguration configuration = null;
    Logger log = Logger.Instance;

    public Scheduler(SchedulerConfiguration config)
	{
        configuration = config;
	}

    public void Start()
    {
        while (true)
        {
            try
            {
                //    For each job, call the execute method
                foreach (ISchedulerJob job in configuration.Jobs)
                {
                    job.Execute();
                }
                //  ((ISchedulerJob)configuration.Jobs[0]).Execute();
            }
            catch (Exception ex)
            {
                if (log.isLoggingOn && log.isAppLoggingOn)
                {
                    log.Log(ex);
                }
            }
            finally
            {
                Thread.Sleep(configuration.SleepInterval);
            }
        }
    }

}

///<summary>
///Interface for Scheduler Jobs
///</summary>
public interface ISchedulerJob
{
    //  All the contract asks for is that a Job implements an Execute method that the scheduling engine can call. 
    void Execute();
}


public class DoJob : ISchedulerJob
{
    DBOperations dbOps;       
    Logger log = Logger.Instance;

    ProcessingEngine engine = ProcessingEngine.Instance;

    private string dateFormatString = ConfigurationManager.AppSettings["dateFormatString"];
            
    public void Execute()
    {
        dbOps = DBOperations.Instance;            

        //  Update the DB. 
        UpdateDB();
        UpdateClickDataDB();
        UpdatePopularCategories();
        UpdateItemRowCount();
        ReLoadLists();

        //  Log the time at which the Scheduler runs the Execute() Method 
        //  And the DB was Updated using the UpdateDB() Method. 
           
        log.Log("Run Execute(). DB Updated. Yay!");

    }

    
        
    
    /// <summary>
    /// Update the DB.
    /// </summary>
    private void UpdateDB()
    {
        //  DB Variables.
        StringBuilder queryBuilder = new StringBuilder();
        string queryString = string.Empty;
        int retVal = 0;         
        MySqlDataReader retList;

        if (engine.ClickCount != null && engine.ClickCount.Count > 0)
        {
            queryBuilder.Append("UPDATE item SET Clicks = CASE IID ");

            foreach (KeyValuePair<int, int> kv in engine.ClickCount)
            {
                int iid = Convert.ToInt32(kv.Key);
                int clicks = Convert.ToInt32(kv.Value);
                if (clicks > 0)
                {
                    //  UPDATE getputs.item SET Clicks = CASE IID WHEN 6 THEN Clicks + 1 WHEN 18 THEN Clicks + 4 ELSE Clicks END;

                    //  Clicks = Clicks + clicks;
                    queryBuilder.Append(" WHEN " +  iid.ToString() + " THEN Clicks + " + clicks.ToString() + " ");
                    
                    //  Clicks = clicks;
                    //  queryBuilder.Append(" WHEN " + iid.ToString() + " THEN " + clicks.ToString() + " ");
                }

                //  For all Items: Reset ClickCount to 0.
                //  engine.ClickCount[iid] = 0;
            }

            engine.ClickCount.Clear();

            queryBuilder.Append(" ELSE Clicks END;");
            queryString = queryBuilder.ToString();

            retVal = dbOps.ExecuteNonQuery(queryString);
            if (retVal >= 0)
            {
                if (log.isAppLoggingOn && log.isDBLoggingOn)
                {
                    log.Log("UpdateDB Succeeded. Query: " + queryString);
                }
            }
            else
            {
                if (log.isAppLoggingOn && log.isDBLoggingOn)
                {
                    log.Log("UpdateDB Failed. Query: " + queryString);
                }
            }

            //  engine.ClickCount = new System.Collections.Generic.Dictionary<int, int>();
            
        }
        else
        {
            if (log.isAppLoggingOn && log.isDBLoggingOn)
            {
                log.Log("ClickCount = 0 ");
            }
        }
    }


    private void UpdateClickDataDB()
    {
        //  DB Variables.
        StringBuilder queryBuilder = new StringBuilder();
        string queryString = string.Empty;
        int retVal = 0;
        MySqlDataReader retList;

        if (engine.ClickDataDB != null && engine.ClickDataDB.Count > 0)
        {
            //  INSERT IGNORE INTO clickdata(IID, UID, IP) VALUES (1,'vatsal', 1), (1,'', 3);

            queryBuilder.Append("INSERT IGNORE INTO clickdata(IID, UID, IP, Date) VALUES ");

            foreach(KeyValuePair<int, List<ClickData>> kv1 in engine.ClickDataDB)
            {
                int iid = kv1.Key;
                List<ClickData> cdList = kv1.Value;

                foreach(ClickData cd in cdList)
                {
                    queryBuilder.Append("(" + cd.IID + ", '" + cd.UID + "', INET_ATON('" + cd.IP + "'), DATE_FORMAT('" + cd.Date + "','%Y-%m-%d %k:%i:%S' )),");                    
                }                
            }
            
                        
            queryString = queryBuilder.ToString();
            if (queryString.EndsWith(","))
            {
                queryString = queryString.Remove(queryString.Length - 1);
            }

            retVal = dbOps.ExecuteNonQuery(queryString);
            if (retVal >= 0)
            {
                if (log.isAppLoggingOn && log.isDBLoggingOn)
                {
                    log.Log("UpdateClickDataDB Succeeded. Query: " + queryString);
                }
            }
            else
            {
                if (log.isAppLoggingOn && log.isDBLoggingOn)
                {
                    log.Log("UpdateClickDataDB Failed. Query: " + queryString);
                }
            }

            //  engine.ClickCount = new System.Collections.Generic.Dictionary<int, int>();
            engine.ClickDataDB.Clear();
        }
        else
        {
            if (log.isAppLoggingOn && log.isDBLoggingOn)
            {
                log.Log("ClickDataDB = 0 ");
            }
        }
    }


    /// <summary>
    /// Update the Popular Categories.
    /// </summary>
    private void UpdatePopularCategories()
    {
        try
        {
            engine.PopularCategories.Clear();

            DateTime dupDT = DateTime.Now.Subtract(new TimeSpan(10, 0, 0, 0));
            string dupDate = dupDT.ToString("yyyy-MM-dd HH:mm:ss");

            string queryString = "SELECT Category FROM item WHERE Date > '" + dupDate + "' GROUP BY Category ORDER BY Count(Category) DESC LIMIT 10;";
            MySqlDataReader retList = dbOps.ExecuteReader(queryString);

            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    string currentCategory = Convert.ToString(retList["Category"]);
                    engine.PopularCategories.Add(currentCategory);
                }
                retList.Close();
            }

            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Updated Popular Categories Successfully. Count: " + engine.PopularCategories.Count.ToString());
            }

        }
        catch (Exception ex)
        {
            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Error occurred inside UpdatePopularCategories");
                log.Log(ex);
            }
        }        
    }


    private void UpdateItemRowCount()
    {
        try
        {
            string queryString = "SELECT COUNT(*) FROM item WHERE Spam = 0;";
            int retInt = dbOps.ExecuteScalar(queryString);

            engine.ItemRowCount = retInt;

            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Updated Item Row Count: " + engine.ItemRowCount.ToString());
            }

        }
        catch (Exception ex)
        {
            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Error occurred inside UpdateItemRowCount");
                log.Log(ex);
            }
        }        
    }

    private void ReLoadLists()
    {
        try
        {
            General general = General.Instance;
            general.ReloadBadUsersLists();
            general.ReloadBadIPsLists();
            general.ReloadBadSitesLists();
            general.ReloadBadDomainsLists();
            general.ReloadImageWordsToBeIgnoredLists();
            log.Log("Lists Update Successfully");
        }
        catch (Exception ex)
        {
            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Error occurred inside ReLoadLists");
                log.Log(ex);
            }
        }
    }


}




///<summary>
///Scheduler Configuration. The scheduling engine needs to know what jobs it has to run and how often. 
///</summary>
public class SchedulerConfiguration
{
    private int sleepInterval;
    private ArrayList jobs = new ArrayList();

    public SchedulerConfiguration(int newSleepInterval)
    {
        sleepInterval = newSleepInterval;
    }

    public int SleepInterval
    {
        get
        {
            return sleepInterval;
        }
    }

    public ArrayList Jobs
    {
        get
        {
            return jobs;
        }
    }
}
  

