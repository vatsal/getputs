//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace gp.Files
{
    /// <summary>
    /// If any word from this list occurs inside an image URL (i.e. the image source), Then ignore the image.
    /// </summary>
    public sealed class ImageWordsToBeIgnored
    {
        private static volatile ImageWordsToBeIgnored instance;
		private static object syncRoot = new Object();

		private const string filename = "ImageWordsToBeIgnored.txt";
		private string path;

        private Dictionary<string, bool> _imageWordsToBeIgnored;

		/// <summary>
		///  Reads Words File
		/// </summary>
        private ImageWordsToBeIgnored() 
		{
            path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];

            _imageWordsToBeIgnored = new Dictionary<string, bool>();
			
            // Use Standard Loading 
            _imageWordsToBeIgnored = FileLoader.LoadFileInDictionary(filename);
		}

		/// <summary>
        /// Initializes a new instance of the ImageWordsToBeIgnored Class, If required.
		/// </summary>
        public static ImageWordsToBeIgnored Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (syncRoot) 
					{
						if (instance == null)
                            instance = new ImageWordsToBeIgnored();
					}
				}
				return instance;
			}
		}

		public void ReLoadList()
		{
			// Use Standard Loading 
            Dictionary<string, bool> _reloadImageWordsToBeIgnored = FileLoader.LoadFileInDictionary(filename);
            lock (_imageWordsToBeIgnored)
			{
                _imageWordsToBeIgnored = _reloadImageWordsToBeIgnored;
			}
		}


		/// <summary>
        /// Indicates whether a url is a contained in the ImageWordsToBeIgnored List
		/// </summary>
		/// <param name="url">The url to check</param>
        /// <returns>True if word is a ImageWordsToBeIgnored; False Otherwise.</returns>
		public bool Contains(string word) 
		{
            return _imageWordsToBeIgnored.ContainsKey(word.ToLower());
		}
		
        /// <summary>
        ///  Return the number of items in ImageWordsToBeIgnored List
		/// </summary>
		/// <returns>int</returns>
		public int Count
		{
            get 
            {
                return _imageWordsToBeIgnored.Count; 
            }
		}

        public Dictionary<string, bool> GetImageWordsToBeIgnored()
        {
            return _imageWordsToBeIgnored;
        }
    }
}
