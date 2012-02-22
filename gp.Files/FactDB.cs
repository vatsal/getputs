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
    /// Create and Maintain a List of Random Facts.
    /// </summary>
    public sealed class FactDB
    {
        private static volatile FactDB instance;
        private static object syncRoot = new Object();

        private const string filename = "FactDB.txt";
        private string path;

        private List<string> _factDB;

        /// <summary>
        /// This is the path to the WebPage for the Random Fact from http://www.zocode.com/
        /// </summary>
        string factURL = "http://www.zocode.com/";

        /// <summary>
        ///  Reads Words File
        /// </summary>
        private FactDB()
        {
            path = System.Configuration.ConfigurationSettings.AppSettings["FilesPath"];

            _factDB = new List<string>();

            // Use Standard Loading 
            _factDB = FileLoader.LoadFileInList(filename, true);
        }

        /// <summary>
        /// Initializes a new instance of the FactDB Class, If required.
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        public static FactDB Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new FactDB();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Reload the FactDB List.
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        public void ReLoadList()
        {
            // Use Standard Loading 
            List<string> _reloadFactDBList = FileLoader.LoadFileInList(filename, true);
            lock (_factDB)
            {
                _factDB = _reloadFactDBList;
            }
        }

        /// <summary>
        ///  Return the number of items in FactDB List
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <returns>Number of Items in the FactDB</returns>
        public int Count
        {
            get
            {
                return _factDB.Count;
            }
        }



        /// <summary>
        /// Get and Store n number of Random Facts from the factURL, where n = Count
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <param name="fullFilePath">The Path and the FileName.</param>        
        /// <param name="count">The number of Facts to be returned.</param>        
        public void StoreFact(string fullFilePath, int count)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                using (WebClient wc = new WebClient())
                {
                    for (int i = 0; i < count; i++)
                    {
                        byte[] data = wc.DownloadData(factURL);
                        string text = Encoding.ASCII.GetString(data);

                        if (!string.IsNullOrEmpty(text))
                        {
                            #region URL Dependent Code
                            //  The code is tailored based on the factURL Page.
                            text = text.Substring(text.IndexOf("<h2>"));
                            text = text.Remove(text.IndexOf("</h2>"));
                            text = text.Substring(text.IndexOf(" "));
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
        /// Get a Random Fact from the FactDB List
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <returns>The Random Fact if FactDB is not empty; an empty string otherwise</returns>
        public string GetRandomFact()
        {
            string randomFact = string.Empty;

            if (_factDB != null && _factDB.Count > 0)
            {
                Random random = new Random();
                int randomRow = random.Next(0, _factDB.Count);
                randomFact = _factDB[randomRow];
            }
            return randomFact;
        }



    }
}
