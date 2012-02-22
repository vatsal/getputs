//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;

namespace gp.Files
{
    public sealed class BadSites
    {
        private static volatile BadSites instance;
		private static object syncRoot = new Object();

		private const string filename = "BadSites.txt";
		private string path;

		private Dictionary<string,bool> _badSites;

		/// <summary>
		///  Reads Words File
		/// </summary>
        private BadSites() 
		{
            path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            
            _badSites = new Dictionary<string,bool>();
			
            // Use Standard Loading 
            _badSites = FileLoader.LoadFileInDictionary(filename);
		}

		/// <summary>
        /// Initializes a new instance of the BadSite Class, If required.
		/// </summary>
        public static BadSites Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (syncRoot) 
					{
						if (instance == null)
                            instance = new BadSites();
					}
				}
				return instance;
			}
		}

		public void ReLoadList()
		{
			// Use Standard Loading 
            Dictionary<string,bool> _reloadBadSites = FileLoader.LoadFileInDictionary(filename);
            lock (_badSites)
			{
                _badSites = _reloadBadSites;
			}
		}


		/// <summary>
        /// Indicates whether a url is a contained in the BadSites List
		/// </summary>
		/// <param name="url">The url to check</param>
		/// <returns>True if url is a BadSite; False Otherwise.</returns>
		public bool Contains(string url) 
		{
            return _badSites.ContainsKey(url.ToLower());
		}
		
        /// <summary>
        ///  Return the number of items in BadSites List
		/// </summary>
		/// <returns>int</returns>
		public int Count
		{
            get 
            { 
                return _badSites.Count; 
            }
		}
    }
}
