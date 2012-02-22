//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;



namespace gp.Files
{
    public sealed class BadUsers
    {
        private static volatile BadUsers instance;
		private static object syncRoot = new Object();

		private const string filename = "BadUsers.txt";
		private string path;

		private Dictionary<string,bool> _badUsers;

		/// <summary>
		///  Reads Words File
		/// </summary>
        private BadUsers() 
		{
            //  path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            //  path = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            
            _badUsers = new Dictionary<string,bool>();
			
            // Use Standard Loading 
            _badUsers = FileLoader.LoadFileInDictionary(filename);
		}

		/// <summary>
        /// Initializes a new instance of the BadSite Class, If required.
		/// </summary>
        public static BadUsers Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (syncRoot) 
					{
						if (instance == null)
                            instance = new BadUsers();
					}
				}
				return instance;
			}
		}

		public void ReLoadList()
		{
			// Use Standard Loading 
            Dictionary<string,bool> _reloadBadUsers = FileLoader.LoadFileInDictionary( filename);
            lock (_badUsers)
			{
                _badUsers = _reloadBadUsers;
			}
		}


		/// <summary>
        /// Indicates whether a url is a contained in the BadUsers List
		/// </summary>
		/// <param name="url">The url to check</param>
		/// <returns>True if url is a BadSite; False Otherwise.</returns>
		public bool Contains(string url) 
		{
            return _badUsers.ContainsKey(url.ToLower());
		}
		
        /// <summary>
        ///  Return the number of items in BadUsers List
		/// </summary>
		/// <returns>int</returns>
		public int Count
		{
            get 
            { 
                return _badUsers.Count; 
            }
		}
    }
}

