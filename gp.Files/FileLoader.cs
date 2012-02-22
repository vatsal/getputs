//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace gp.Files
{
    public class FileLoader
    {
        static string filePath = string.Empty;

        public static string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }
        
        public static ArrayList LoadFileInArrayList(string filename)
        {
            ArrayList outputAL = new ArrayList();
            string fileToLoad = Path.Combine(filePath, filename);
            if (File.Exists(fileToLoad))
            {
                try
                {
                    FileStream file = new FileStream(fileToLoad, FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(file, System.Text.Encoding.GetEncoding("iso-8859-1"));
                    while (sr.Peek() >= 0)
                    {
                        string currentLine = sr.ReadLine().Trim().ToLower();
                        if (!currentLine.StartsWith("//") && (currentLine.Length > 0))
                        {
                            outputAL.Add(currentLine);
                        }
                    }
                    sr.Close();
                }
                catch 
                {
                    
                }
            }
            return outputAL;
        }


        /// <summary>
        /// Read a File and Store it into a List.
        /// </summary>
        /// <param name="filename">The File to be read into a List.</param>
        /// <param name="maintainCase">If True: Maintain the Case of the Words in the File. If False: Convert everything into LowerCase.</param>
        /// <returns>A List where each entry corresponds to a line from the "filename"</returns>
        public static List<string> LoadFileInList(string filename, bool maintainCase)
        {
            List<string> outputList = new List<string>();
            string fileToLoad = Path.Combine(filePath, filename);
            if (File.Exists(fileToLoad))
            {
                try
                {
                    FileStream file = new FileStream(fileToLoad, FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(file, System.Text.Encoding.GetEncoding("iso-8859-1"));
                    while (sr.Peek() >= 0)
                    {
                        string currentLine = sr.ReadLine().Trim();
                        if (!maintainCase)
                        {
                            currentLine = currentLine.ToLower();
                        }
                        
                        if (!currentLine.StartsWith("//") && (currentLine.Length > 0))
                        {
                            outputList.Add(currentLine);
                        }
                    }
                    sr.Close();
                }
                catch
                {

                }
            }
            return outputList;
        }




        public static Dictionary<string, bool> LoadFileInDictionary(string filename)
        {
            Dictionary<string, bool> outputDict = new Dictionary<string, bool>();
            string fileToLoad = Path.Combine(filePath, filename);
            if (File.Exists(fileToLoad))
            {
                try
                {
                    FileStream file = new FileStream(fileToLoad, FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(file, System.Text.Encoding.GetEncoding("iso-8859-1"));
                    while (sr.Peek() >= 0)
                    {
                        string currentLine = sr.ReadLine().Trim().ToLower();
                        if (!currentLine.StartsWith("//") && (currentLine.Length > 0))
                        {
                            if (!outputDict.ContainsKey(currentLine))
                            {
                                outputDict.Add(currentLine, true);
                            }
                        }
                    }
                    sr.Close();
                }
                catch
                {

                }
            }
            return outputDict;
        }

        /// <summary>
        /// Load the Contents of a "|" seperated file in a HashTable.
        /// The Content[0] = "Key", Content[1] = "Value"
        /// </summary>
        /// <param name="filename">The input file.</param>
        /// <returns>The output HashTable</returns>
        public static Hashtable LoadFileInHashtable(string filename, string[] splitter)
        {
            Hashtable outputHT = new Hashtable();
            string fileToLoad = Path.Combine(filePath, filename);
            if (File.Exists(fileToLoad))
            {
                try
                {
                    FileStream file = new FileStream(fileToLoad, FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(file, System.Text.Encoding.GetEncoding("iso-8859-1"));
                    
                    while (sr.Peek() >= 0)
                    {

                        string currentLine = sr.ReadLine().Trim().ToLower();

                        if (!currentLine.StartsWith("//") && (currentLine.Length > 0))
                        {
                            string[] currentLineArray = currentLine.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                            string key = string.Empty;
                            string value = string.Empty;

                            //  Has to have a Key and a Value.
                            if (currentLineArray.Length >= 2)
                            {
                                key = currentLineArray[0].Trim();
                                value = currentLineArray[1].Trim();
                            }

                            if (!string.IsNullOrEmpty(key) && !outputHT.ContainsKey(key))
                            {
                                outputHT.Add(key, value);
                            }
                        }
                        
                    }
                    sr.Close();
                }
                catch
                {

                }
            }
            return outputHT;
        }

    }
}
