<%@ Application Language="C#" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="System.Security" %>

<script runat="server">

    //  Owner: Vatsal Shah
    //  Product: getputs.com
    //  Copyright Notice: Copyright Protected. All Rights Reserved.
    
    public DBOperations dbOps;
    public General general;
    public GUIVariables gui;
    public Links links;
    public Logger log;    
    public ProcessingEngine engine;
    public Categories categories;
    public ImageEngine imageEngine;
    public SearchEngine searchEngine;
    public SpamDetection spamDetection;
    public Tokenizer tokenizer;
    public Tagger tagger;
    public ItemDisplayer itemDisplayer;

    private System.Threading.Thread schedulerThread = null;
    private int scheduleEveryNSeconds = int.Parse(ConfigurationManager.AppSettings["scheduleEveryNSeconds"]);
        
    void LoadClasses()
    {
        dbOps = DBOperations.Instance;
        general = General.Instance;
        gui = GUIVariables.Instance;
        links = Links.Instance;
        log = Logger.Instance;                
        categories = Categories.Instance;
        //  engine = ProcessingEngine.Instance;
        engine = ProcessingEngine.Instance;
        imageEngine = ImageEngine.Instance;
        searchEngine = SearchEngine.Instance;
        spamDetection = SpamDetection.Instance;
        tokenizer = Tokenizer.Instance;
        tagger = Tagger.Instance;
        itemDisplayer = ItemDisplayer.Instance;
        
        Application["dbOps"] = dbOps;
        Application["general"] = general;
        Application["gui"] = gui;
        Application["links"] = links;
        Application["log"] = log;
        Application["categories"] = categories;
        Application["engine"] = engine;
        Application["imageEngine"] = imageEngine;
        Application["searchEngine"] = searchEngine;
        Application["spamDetection"] = spamDetection;
        Application["tokenizer"] = tokenizer;
        Application["tagger"] = tagger;
        Application["itemDisplayer"] = itemDisplayer;
    }

    /// <summary>
    /// The Scheduling Engine that assigns daily tasks to the worker thread. 
    /// Includes Updation of the UserPrediction and StockPrediction DB.
    /// </summary>
    void LaunchScheduler()
    {
        //  SchedulerConfiguration sConfig = new SchedulerConfiguration(24 * 60 * 60 * 1000);
        SchedulerConfiguration sConfig = new SchedulerConfiguration(scheduleEveryNSeconds);

        sConfig.Jobs.Add(new DoJob());
        Scheduler scheduler = new Scheduler(sConfig);

        System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(scheduler.Start);
        schedulerThread = new System.Threading.Thread(threadStart);
        schedulerThread.Start();
    }
    
    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        try
        {
            //  Now the classes are singletons. No need to load them at application start.
            //  LoadClasses();
            
            LaunchScheduler();
        }
        catch(Exception ex)
        {
            
        }
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
        //  Finally, in order to shut the scheduler down, we need to call Abort() on the scheduler thread.
        if (null != schedulerThread)
        {
            schedulerThread.Abort();
        }

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
