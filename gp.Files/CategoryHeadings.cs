//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace gp.Files
{
    public sealed class CategoryHeadings
    {
        private static volatile CategoryHeadings instance;
        private static object syncRoot = new Object();

        private const string filename = "CategoryHeadings.txt";
        private string path;

        private Hashtable _categoryHeadings;

        /// <summary>
		///  Reads Words File
		/// </summary>
        private CategoryHeadings() 
		{
            //  path = System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];
            //  path = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationSettings.AppSettings["BadFilesPath"];

            _categoryHeadings = new Hashtable();

            string[] splitter = { "|" };

            // Use Standard Loading 
            _categoryHeadings = FileLoader.LoadFileInHashtable(filename, splitter);
		}


        /// <summary>
        /// Initializes a new instance of the CategoryHeadings Class, If required.
        /// </summary>
        public static CategoryHeadings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CategoryHeadings();
                    }
                }
                return instance;
            }
        }
    }
}
