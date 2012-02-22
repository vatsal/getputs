//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace gp.Files
{
    public sealed class TipsDB
    {
        private static volatile TipsDB instance;
        private static object syncRoot = new Object();

        private const string filename = "TipsDB.txt";
        private string path;

        private List<string> _tipsDB;

        /// <summary>
		///  Reads Words File
		/// </summary>
        private TipsDB() 
		{
            //  path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            //  path = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];

            _tipsDB = new List<string>();

            string[] splitter = { "|" };

            // Use Standard Loading 
            _tipsDB = FileLoader.LoadFileInList(filename, true);
		}


        /// <summary>
        /// Initializes a new instance of the TipsDB Class, If required.
        /// </summary>
        public static TipsDB Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TipsDB();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Reload the TipsDB File into Memory.
        /// </summary>
        public void ReLoadList()
        {
            // Use Standard Loading 
            List<string> _reloadTipsDB = FileLoader.LoadFileInList(filename, true);
            lock (_reloadTipsDB)
            {
                _tipsDB = _reloadTipsDB;
            }
        }

        /// <summary>
        /// Get a RandomTip from the TipDB List
        /// [Note: Do not forget to assign the FilePath (excluding the FileName) to the property: gp.Files.FileLoader.FilePath]
        /// </summary>
        /// <returns>The RandomTip if TipDB is not empty; an empty string otherwise</returns>
        public string GetRandomTip()
        {
            string randomTip = string.Empty;

            if (_tipsDB != null && _tipsDB.Count > 0)
            {
                Random random = new Random();
                int randomRow = random.Next(0, _tipsDB.Count);
                randomTip = _tipsDB[randomRow];
            }
            return randomTip;
        }

    }
}
