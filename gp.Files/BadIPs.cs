//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace gp.Files
{
    public sealed class BadIPs
    {
        private static volatile BadIPs instance;
		private static object syncRoot = new Object();

		private const string filename = "BadIPs.txt";
		private string path;

		private Dictionary<string,bool> _badIPs;

		/// <summary>
		///  Reads Words File
		/// </summary>
        private BadIPs() 
		{
            path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            
            _badIPs = new Dictionary<string,bool>();
			
            // Use Standard Loading 
            _badIPs = FileLoader.LoadFileInDictionary(filename);
		}

		/// <summary>
        /// Initializes a new instance of the BadSite Class, If required.
		/// </summary>
        public static BadIPs Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (syncRoot) 
					{
						if (instance == null)
                            instance = new BadIPs();
					}
				}
				return instance;
			}
		}

		public void ReLoadList()
		{
			// Use Standard Loading 
            Dictionary<string,bool> _reloadBadIPs = FileLoader.LoadFileInDictionary(filename);
            lock (_badIPs)
			{
                _badIPs = _reloadBadIPs;
			}
		}


		/// <summary>
        /// Indicates whether a url is a contained in the BadIPs List
		/// </summary>
		/// <param name="url">The url to check</param>
		/// <returns>True if url is a BadSite; False Otherwise.</returns>
		public bool Contains(string url) 
		{
            return _badIPs.ContainsKey(url.ToLower());
		}
		
        /// <summary>
        ///  Return the number of items in BadIPs List
		/// </summary>
		/// <returns>int</returns>
		public int Count
		{
            get 
            { 
                return _badIPs.Count; 
            }
		}
    }
}
