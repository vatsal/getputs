//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace gp.Files
{
    /// <summary>
    /// Create and Maintain a List of Random Quotes.
    /// </summary>
    public sealed class QuoteDB
    {
        private static volatile QuoteDB instance;
        private static object syncRoot = new Object();

        private const string filename = "QuoteDB.txt";
        private string path;

        private List<string> _quoteDB;

        /// <summary>
        /// This is the path to the RSS Feed for the Random Quote from www.quotedb.com
        /// </summary>
        //  string quoteURL = "http://www.quotedb.com/quote/quote.php?action=random_quote_rss&=&=&";
        
        /// <summary>
        /// This is the path to the JavaScript for the Random Quote from www.quotedb.com
        /// </summary>
        string quoteURL = "http://www.quotedb.com/quote/quote.php?action=random_quote&=&=&";

        /// <summary>
		///  Reads Words File
		/// </summary>
        private QuoteDB() 
		{
            path = System.Configuration.ConfigurationSettings.AppSettings["FilesPath"];

            _quoteDB = new List<string>();
			
            // Use Standard Loading 
            _quoteDB = FileLoader.LoadFileInList(filename, true);
		}

        /// <summary>
        /// Initializes a new instance of the QuoteDB Class, If required.
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        public static QuoteDB Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new QuoteDB();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Reload the QuoteDB List.
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        public void ReLoadList()
        {
            // Use Standard Loading 
            List<string> _reloadQuoteDBList = FileLoader.LoadFileInList(filename, true);
            lock (_quoteDB)
            {
                _quoteDB = _reloadQuoteDBList;
            }
        }

        /// <summary>
        ///  Return the number of items in QuoteDB List
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <returns>Number of Items in the QuoteDB</returns>
        public int Count
        {
            get
            {
                return _quoteDB.Count;
            }
        }



        /// <summary>
        /// Get and Store n number of Random Quotes from the quoteLink, where n = Count
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <param name="fullFilePath">The Path and the FileName.</param>        
        /// <param name="count">The number of quotes to be returned.</param>        
        public void StoreQuote(string fullFilePath, int count)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                using (WebClient wc = new WebClient())
                {
                    for (int i = 0; i < count; i++)
                    {
                        byte[] data = wc.DownloadData(quoteURL);
                        string text = Encoding.ASCII.GetString(data);

                        if (!string.IsNullOrEmpty(text))
                        {
                            #region URL Dependent Code
                            //  The code is tailored based on the quoteURL Page.
                            text = text.Replace("document.write", "").Replace("('", "").Replace("');", "").Replace("More quotes from", "").Replace("<i>", "").Replace("</i>", "").Replace("\n", "").Replace("<br>", "").Replace("<a href=", "").Replace("</a>", "");
                            text = text.Remove(text.IndexOf("\""), text.LastIndexOf("\"") - text.IndexOf("\"")).Replace(">", "").Replace("\"", " | ");
                            text = text.Trim();
                            #endregion URL Dependent Code

                            if (text.Length < 500)
                            {
                                string textToAppend = string.Empty;

                                textToAppend = text;
                                sb.AppendLine(textToAppend);
                            }
                        }
                    }

                    File.AppendAllText(fullFilePath, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Get a RandomQuote from the QuoteDB List
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <returns>The RandomQuote if QuoteDB is not empty; an empty string otherwise</returns>
        public string GetRandomQuote()
        {
            string randomQuote = string.Empty;
            
            if (_quoteDB != null && _quoteDB.Count > 0)
            {
                Random random = new Random();
                int randomRow = random.Next(0, _quoteDB.Count);
                randomQuote = _quoteDB[randomRow];
            }
            return randomQuote;
        }



    }
}
