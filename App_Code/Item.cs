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
using System.Collections.Generic;

/// <summary>
/// Summary description for Item
/// </summary>
public class Item
{
    //  The Table getputs.user has the following schema:
    //  getputs.item [IID, Title, Link, Text, Date, UID, NComments, Category, Clicks, UpVotes, DownVotes, Spam]
    //  IID is AutoIncrement.

    //  Attributes with Get/Set : Title, Link, Text, NComments, Category, Clicks, UpVotes, DownVotes, IsSpam  
    //  Attributes with Get     : IID, Date, Time, UID

    //  SortedDictionary<int, float> ageTable = (SortedDictionary<int, float>)System.Configuration.ConfigurationSettings.GetConfig("AgeTable");
    Hashtable ageHashTable = (Hashtable)ConfigurationManager.GetSection("AgeTable");

    //  Tuning Parameters
    private double _clicksTuner = Convert.ToDouble(ConfigurationManager.AppSettings["clicksTuner"]);
    private double _commentsTuner = Convert.ToDouble(ConfigurationManager.AppSettings["clicksTuner"]);    
    private double _savedTuner = Convert.ToDouble(ConfigurationManager.AppSettings["savedTuner"]);
    private double _eMailedTuner = Convert.ToDouble(ConfigurationManager.AppSettings["eMailedTuner"]);
    private double _ratingTuner = Convert.ToDouble(ConfigurationManager.AppSettings["ratingTuner"]);
    private double _ageTuner = Convert.ToDouble(ConfigurationManager.AppSettings["clicksTuner"]);

    SortedDictionary<int, double> ageSortedTable;

    private int _IID;
    private string _title;
    private string _link;
    private string _text;
    private string _date;
    private string _time;
    private string _UID;
    private int _nComments;
    private string _category;
    private int _clicks;
    private int _upVotes;
    private int _downVotes;
    private bool _isSpam;

    //  List of Tags (List Format)
    private List<string> _tagList;
    //  List of Tags (CSV Format)
    private string _tagString;
    
    private int _nSaved;
    private int _nEMailed;

    private string _ip;

    private double _age;
    private double _marks;

    //  Item Rating [0 to 5]
    private double _avgRating;
    //  Item Total Raters
    private int _nRated;

    private string _location;


    public int IID
    {
        get
        {
            return _IID;
        }
        set
        {
            _IID = value;
        }
    }

    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
        }
    }

    public string Link
    {
        get
        {
            return _link;
        }
        set
        {
            _link = value;
        }
    }

    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
        }
    }

    public string Date
    {
        get
        {
            return _date;
        }
        set
        {
            _date = value;
        }
    }

    public string Time
    {
        get
        {
            return _time;
        }
    }

    public string UID
    {
        get
        {
            return _UID;
        }
        set
        {
            _UID = value;
        }
    }
    
    public int NComments
    {
        get
        {
            return _nComments;
        }
        set
        {
            _nComments = value;
        }
    }

    public string Category
    {
        get
        {
            return _category;
        }
        set
        {
            _category = value;
        }
    }

    public int Clicks
    {
        get
        {
            return _clicks;
        }
        set
        {
            _clicks = value;
        }
    }

    public int UpVotes
    {
        get
        {
            return _upVotes;
        }
        set
        {
            _upVotes = value;
        }
    }

    public int DownVotes
    {
        get
        {
            return _downVotes;
        }
        set
        {
            _downVotes = value;
        }
    }

    public bool IsSpam
    {
        get
        {
            return _isSpam;
        }
        set
        {
            _isSpam = value;
        }
    }
    
    public int NSaved
    {
        get
        {
            return _nSaved;
        }
        set
        {
            _nSaved = value;
        }
    }

    public int NEMailed
    {
        get
        {
            return _nEMailed;
        }
        set
        {
            _nEMailed = value;
        }
    }

    public string IP
    {
        get
        {
            return _ip;
        }
        set
        {
            _ip = value;
        }
    }


    public double Age
    {
        get
        {
            return _age;
        }
        set
        {
            _age = value;
        }
    }

    public double Marks
    {
        get
        {
            return _marks;
        }
        set
        {
            _marks = value;
        }
    }

    /// <summary>
    /// List of Tags (List Format)
    /// </summary>
    public List<string> TagList
    {
        get
        {
            return _tagList;
        }
        set
        {
            _tagList = value;
        }
    }

    /// <summary>
    /// List of Tags (String Format)
    /// </summary>
    public string TagString
    {
        get
        {
            return _tagString;
        }
        set
        {
            _tagString = value;
        }
    }


    public double AvgRating
    {
        get
        {
            return _avgRating;
        }
        set
        {
            _avgRating = value;
        }
    }

    public int NRated
    {
        get
        {
            return _nRated;
        }
        set
        {
            _nRated = value;
        }
    }

    public string Location
    {
        get
        {
            return _location;
        }
        set
        {
            _location = value;
        }

    }




	public Item()
	{
        ageSortedTable = new SortedDictionary<int, double>();

        foreach (DictionaryEntry de in ageHashTable)
        {
            int key = Convert.ToInt32(de.Key);
            double value = Convert.ToDouble(de.Value);

            if(!ageSortedTable.ContainsKey(key))
                ageSortedTable.Add(key, value);
        }
        
	}

    public double GetAge(string datetime)
    {
        double returnAge = 1;

        DateTime dtItem;
        DateTime dtCurrent = DateTime.Now;

        bool isDone = false;

        if (DateTime.TryParse(datetime, out dtItem))
        {
            TimeSpan ts = dtCurrent.Subtract(dtItem);
            int totalHours = Convert.ToInt32(ts.TotalHours);

            returnAge = Convert.ToDouble(totalHours);

            //if (totalHours > 192)
            //{
            //    returnAge = ageSortedTable[192];
            //}
            //else
            //{

            //    foreach (KeyValuePair<int,double> de in ageSortedTable)
            //    {
            //        if (!isDone && totalHours <= de.Key)
            //        {
            //            returnAge = de.Value;
            //            isDone = true;                    
            //        }
            //    }
            //}
        }


        return returnAge;
    }

    /*
    public double GetMarks(int clicks, int comments, double age)
    {
        double returnMarks = 1;

        //if (clicks == 0 && comments == 0)
        //{
        //    returnMarks = 0;
        //}
        //else
        //{

        //    if (clicks == 0)
        //    {
        //        clicks = 1;
        //    }
        //    if (comments == 0)
        //    {
        //        comments = 1;
        //    }

        //    if (age > 0)    //  If age = 0, it will cause a Divide-By-Zero Error.
        //    {
        //        returnMarks = ((_clicksTuner * clicks) * (_commentsTuner * comments)) / (_ageTuner * age);
        //    }
        //}


        if (clicks == 0)
        {
            clicks = 1;
        }
        if (comments == 0)
        {
            comments = 1;
        }

        if (age > 0)    //  If age = 0, it will cause a Divide-By-Zero Error.
        {
            //  returnMarks = ((_clicksTuner * clicks) * (_commentsTuner * comments)) / (_ageTuner * Math.Pow(age,2));
            //  returnMarks = ((_clicksTuner * clicks) * (_commentsTuner * comments)) / (_ageTuner * Math.Log(age));
            returnMarks = ((_clicksTuner * clicks) * (_commentsTuner * comments)) / (_ageTuner * Math.Pow(age, 9.8));
        }
        return returnMarks;
    }
    */




    public double GetMarks(Item item)
    {
        double returnMarks = 0.01;

        double clicksScore = 0;
        double commentsScore = 0;
        double savedScore = 0;
        double eMailedScore = 0;
        double ratingScore = 0;
        double ageScore = 0;

        if (item.Clicks == 0)
        {
            //  item.Clicks = 1;
            
            //  clicksScore = _clicksTuner;
            clicksScore = 0;
        }
        else
        {
            clicksScore = _clicksTuner * item.Clicks;
        }

        if (item.NComments == 0)
        {
            //  item.NComments = 1;
            
            //  commentsScore = _commentsTuner;

            commentsScore = 0;
        }
        else
        {
            commentsScore = _commentsTuner * item.NComments;
        }

        if (item.NSaved == 0)
        {
            //  item.NSaved = 1;
            
            //  savedScore = _savedTuner;

            savedScore = 0;
        }
        else
        {
            savedScore = _savedTuner * item.NSaved;
        }

        if (item.NEMailed == 0)
        {
            //  item.NEMailed = 1;
            
            //  eMailedScore = _eMailedTuner;

            eMailedScore = 0;
        }
        else
        {
            eMailedScore = _eMailedTuner * item.NEMailed;
        }

        if (item.AvgRating == 0)
        {
            ratingScore = 0;
        }
        else
        {
            ratingScore = _ratingTuner * item.AvgRating * item.NRated;
        }

        if (item.Age > 0)    //  If age = 0, it will cause a Divide-By-Zero Error.
        {            
            //  ageScore = _ageTuner * Math.Pow(item.Age, 9.8);
            //  ageScore = _ageTuner * Math.Log(item.Age);
            //  ageScore = _ageTuner * item.Age;
            ageScore = _ageTuner * Math.Pow(item.Age, 1.5);

            //  returnMarks = ((_clicksTuner * clicks) * (_commentsTuner * comments)) / (_ageTuner * Math.Pow(age,2));
            //  returnMarks = ((_clicksTuner * clicks) * (_commentsTuner * comments)) / (_ageTuner * Math.Log(age));
            //  returnMarks = ((_clicksTuner * item.Clicks) * (_commentsTuner * item.NComments) * (_savedTuner * item.NSaved) * (_eMailedTuner * item.NEMailed))/ (_ageTuner * Math.Pow(item.Age, 9.8));
            
            //  returnMarks = (clicksScore * commentsScore * savedScore * eMailedScore) / (ageScore);
            returnMarks = (clicksScore + commentsScore + savedScore + eMailedScore + ratingScore) / (ageScore);
        }
        return returnMarks;
    }
        
}
