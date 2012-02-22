//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace gp.Files
{
    public sealed class BadWords
    {
        private static volatile BadWords instance;
		private static object syncRoot = new Object();

		private const string filename = "BadWords.txt";
		private string path;

		private Dictionary<string,bool> _badWords;

		/// <summary>
		///  Reads Words File
		/// </summary>
        private BadWords() 
		{
            path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            
            _badWords = new Dictionary<string,bool>();
			
            // Use Standard Loading 
            _badWords = FileLoader.LoadFileInDictionary(filename);
		}

		/// <summary>
        /// Initializes a new instance of the BadWords Class, If required.
		/// </summary>
        public static BadWords Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (syncRoot) 
					{
						if (instance == null)
                            instance = new BadWords();
					}
				}
				return instance;
			}
		}

        /// <summary>
        /// Reload the BadWords File into Memory.
        /// </summary>
		public void ReLoadList()
		{
			// Use Standard Loading 
            Dictionary<string,bool> _reloadBadWords = FileLoader.LoadFileInDictionary(filename);
            lock (_badWords)
			{
                _badWords = _reloadBadWords;
			}
		}


		/// <summary>
        /// Indicates whether a url is a contained in the BadWords List
		/// </summary>
		/// <param name="url">The url to check</param>
		/// <returns>True if word is a BadWord; False Otherwise.</returns>
		public bool Contains(string word) 
		{
            return _badWords.ContainsKey(word.ToLower());
		}
		
        /// <summary>
        ///  Return the number of items in BadWords List
		/// </summary>
		/// <returns>int</returns>
		public int Count
		{
            get 
            { 
                return _badWords.Count; 
            }
		}
    }
}
