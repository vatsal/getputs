//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;

/// <summary>
/// Summary description for Tagger
/// </summary>
public class Tagger
{
    General general = General.Instance;

    private static string filesPath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["filesPath"];
    private static string filename = "LEXICON";

    private static volatile Tagger instance;
    private static object syncRoot = new Object();

    //  string[] _splitter = { " " }; 
    string[] _splitter = { " ", "\"", "(", ")", "{", "}", "[", "]", "/", "\\" };
    //  string _symbols = "~`!@#$%^&*()_-+={[}]|:;'<,>.?/\"\\";
    string _symbols = "~`!@#$%^&*()_-+={[}]|:;'<>?/\"\\";
    
    //  IsTaggingOn != 0 => On; IsTaggingOn == 0 => Off
    private bool _isTaggingOn = int.Parse(ConfigurationManager.AppSettings["Tags_IsTaggingOn"]) == 0 ? false : true;
    //  Max. Number of Tags per Item.
    private int _maxTagsPerItem = int.Parse(ConfigurationManager.AppSettings["Tags_MaxTagsPerItem"]);

    private static Dictionary<string,string> _lexHash = null;

    public bool IsTaggingOn
    {
        get
        {
            return _isTaggingOn;
        }        
    }

    public int MaxTagsPerItem
    {
        get
        {
            return _maxTagsPerItem;
        }
    }


    public Dictionary<string,string> LoadTagLexicon(bool convertWordsToLower)
    {
        _lexHash = new  Dictionary<string,string>();

        string[] lineSplitter = { " " };

        string fileToLoad = Path.Combine(filesPath, filename);
        if (File.Exists(fileToLoad))
        {
            try
            {
                FileStream file = new FileStream(fileToLoad, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader sr = new StreamReader(file, System.Text.Encoding.GetEncoding("iso-8859-1"));
                while (sr.Peek() >= 0)
                {
                    string currentLine = sr.ReadLine().Trim();

                    string[] currentLineArray = currentLine.Split(lineSplitter, StringSplitOptions.RemoveEmptyEntries);

                    string word = currentLineArray[0];
                    string tag = currentLineArray[1];

                    if (!currentLine.StartsWith("//") && (currentLine.Length > 0))
                    {
                        if (!_lexHash.ContainsKey(word))
                        {
                            _lexHash.Add(word, tag);
                        }
                    }
                }
                sr.Close();
            }
            catch
            {

            }
        }
        return _lexHash;


        #region Serialization Not Allowed on Shared Servers
        //try
        //{
        //    Hashtable hash = new Hashtable();                
        //    FileInfo lexFile = new FileInfo("LEXICON");
        //    StreamReader reader = lexFile.OpenText();
        //    string line;
            
        //    do
        //    {
        //        line = reader.ReadLine();
                
        //        if (string.IsNullOrEmpty(line)) 
        //            break;
                
        //        int index = line.IndexOf(" ");
                                    
        //        string word = line.Substring(0, index).Trim();
        //        string tags = line.Substring(index).Trim();

        //        if (convertWordsToLower)
        //        {
        //            word = word.ToLower();
        //        }

        //        if (hash.ContainsKey(word))
        //        {
        //            hash[word] = tags;  //  If a tag exists, overwrite the tag.
        //        }
        //        else
        //        {
        //            hash.Add(word, tags);
        //        }

        //        //  if (hash[word] == null)
        //        //      hash.Add(word, tags);

        //    } while (line != null);

        //    reader.Close();

        //    Stream file = File.Open(Path.Combine(filesPath, "lex.dat"), FileMode.Create);
        //    IFormatter formatter = (IFormatter)new BinaryFormatter();
            
        //    // Serialize the object hash to stream
        //    formatter.Serialize(file, hash);
        //    file.Close();

        //}
        //catch (Exception ex)
        //{

        //}
        #endregion Serialization Not Allowed on Shared Servers
    }



    public Tagger()
    {
        if (_lexHash != null && _lexHash.Count > 0) // singleton pattern
            return; 
        
        _lexHash = LoadTagLexicon(true);           
    }

    public void ReLoadLexiconHash()
    {
        // Use Standard Loading 
        Dictionary<string,string> _reloadLexHash = LoadTagLexicon(true);

        lock (_lexHash)
        {
            _lexHash = _reloadLexHash;
        }
    }

    


    /// <summary>
    /// Initialize a new instance of the Tagger class, If required.
    /// </summary>
    public static Tagger Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new Tagger();
                }
            }
            return instance;
        }
    }

    public List<string> GetTokens(string input, bool removeSingleCharacterSymbols, bool removeDuplicateTokens, bool convertTokensToLower)
    {
        input = FormatInput(input);

        List<string> tokens;
        if (!string.IsNullOrEmpty(input))
        {
            tokens = new List<string>();

            string[] inputArray = input.Split(_splitter, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < inputArray.Length; i++)
            {
                string currentToken = inputArray[i];

                if (convertTokensToLower)
                {
                    currentToken = currentToken.ToLower();
                }

                if (removeSingleCharacterSymbols)
                {
                    if (currentToken.Length == 1)
                    {
                        if (_symbols.Contains(currentToken))
                        {
                            continue;
                        }
                    }
                }

                if (currentToken.Length > 1 
                    && (currentToken.EndsWith(";") 
                    || currentToken.EndsWith(",") 
                    || currentToken.EndsWith("?") 
                    || currentToken.EndsWith(")") 
                    || currentToken.EndsWith(":") 
                    || currentToken.EndsWith(".")))
                {
                    currentToken = currentToken.Substring(0, currentToken.Length - 1);
                }

                if (currentToken.EndsWith("'s") || currentToken.EndsWith("`s")
                    || currentToken.EndsWith("s'") || currentToken.EndsWith("s`"))
                {
                    currentToken = currentToken.Replace("''", "'");
                    currentToken = currentToken.Replace("``", "`");

                    currentToken = currentToken.Replace("'s", "");
                    currentToken = currentToken.Replace("`s", "");

                    currentToken = currentToken.Replace("s'", "");
                    currentToken = currentToken.Replace("s`", "");

                    currentToken = currentToken.Replace("'", "");
                    currentToken = currentToken.Replace("`", "");

                }

                currentToken = currentToken.Replace("'", "");
                currentToken = currentToken.Replace("`", "");


                if (removeDuplicateTokens)
                {
                    if (!tokens.Contains(currentToken))
                    {
                        tokens.Add(currentToken);
                    }
                }
                else
                {
                    tokens.Add(currentToken);
                }
            }
            return tokens;
        }
        else
        {
            tokens = null;
        }
        return tokens;
    }

    private string FormatInput(string input)
    {
        input = input.Replace(". ", " . ");
        input = input.Replace("; ", " ; ");
        input = input.Replace(", ", " , ");
        input = input.Replace(": ", " : ");
        input = input.Replace("? ", " ? ");
        input = input.Replace(") ", " ) ");
        input = input.Replace("!", " ! ");

        return input;
    }

    #region Not Good. Use GetTokens() instead.
    //public ArrayList tokenize(string s)
    //{
    //    ArrayList v = new ArrayList();
    //    Regex reg = new Regex(@"(\S+)\s");
    //    MatchCollection m = reg.Matches(s);
    //    foreach (Match m2 in m)
    //    {
    //        if (m2.Length != 0)
    //        {
    //            string z = m2.ToString().Trim();
    //            if (z.EndsWith(";") || z.EndsWith(",") ||
    //                z.EndsWith("?") || z.EndsWith(")") ||
    //                z.EndsWith(":") || z.EndsWith("."))
    //            {
    //                z = z.Substring(0, z.Length - 1);
    //            }
    //            v.Add(z);
    //        }
    //    }
    //    return v;
    //}
    #endregion Not Good. Use GetTokens() instead.

    /// <summary>
    /// Get the Part-Of-Speech Tags.
    /// </summary>
    /// <param name="words"></param>
    /// <returns></returns>
    public List<string> GetPOSTags(List<string> words)
    {
        List<string> ret = new List<string>();
        for (int i = 0, size = words.Count; i < size; i++)
        {
            ret.Add("NN");  // default
            string s = string.Empty;
            if (_lexHash.ContainsKey(words[i]))
            {
                s = _lexHash[words[i]];
                ret[i] = s;
            }
            else if (_lexHash.ContainsKey(words[i].ToLower()))
            {
                s = _lexHash[words[i].ToLower()];
                ret[i] = s;
            }
            
        }
        /**
         * Apply transformational rules
         **/
        for (int i = 0; i < words.Count; i++)
        {
            //  rule 1: DT, {VBD | VBP} --> DT, NN
            if (i > 0 && ret[i - 1].Equals("DT"))
            {
                if (ret[i].Equals("VBD")
                    || ret[i].Equals("VBP")
                    || ret[i].Equals("VB"))
                {
                    ret[i] = "NN";
                }
            }
            // rule 2: convert a noun to a number (CD) if "." appears in the word
            if ((ret[i]).StartsWith("N"))
            {
                if ((words[i]).IndexOf(".") > -1)
                    ret[i] = "CD";
            }
            // rule 3: convert a noun to a past participle if (words[i]) ends with "ed"
            if ((ret[i]).StartsWith("N") && (words[i]).EndsWith("ed") && words[i].Equals(words[i].ToLower()))
                ret[i] = "VBN";
            // rule 4: convert any type to adverb if it ends in "ly";
            if ((words[i]).EndsWith("ly") && words[i].Equals(words[i].ToLower()))
                ret[i] = "RB";
            // rule 5: convert a common noun (NN or NNS) to a adjective if it ends with "al"
            if ((ret[i]).StartsWith("NN") && (words[i]).EndsWith("al") && words[i].Equals(words[i].ToLower()))
                ret[i] = "JJ";
            // rule 6: convert a noun to a verb if the preceeding work is "would"
            if (i > 0
                && (ret[i]).StartsWith("NN")
                && (words[i - 1]).ToLower().Equals("would"))
                ret[i] = "VB";
            // rule 7: if a word has been categorized as a common noun and it ends with "s",
            //         then set its type to plural common noun (NNS)
            if ((ret[i]).Equals("NN") && (words[i]).EndsWith("s"))
                ret[i] = "NNS";
            // rule 8: convert a common noun to a present prticiple verb (i.e., a gerand)
            if ((ret[i]).StartsWith("NN") && (words[i]).EndsWith("ing"))
                ret[i] = "VBG";
            // rule 9: convert a 4 digit number (assuming that a 4 digit number would represent a year 90% of the times) into a NN.
            // Example: 2008, 1900, 1947 etc.
            if (words[i].Length == 4 && general.IsValidInt(words[i]))
                ret[i] = "NN";            
            if (IsNumberToken(words[i]))
                ret[i] = "CD";
            if (ret[i].StartsWith("NN") && i + 1 < words.Count && !ret[i + 1].StartsWith("NN") && words[i].Equals(words[i].ToLower()))
                ret[i] = "JW";
            if (words[i].Length == 1 && _symbols.Contains(words[i]))
                ret[i] = "SYM";
            if (IsSymbolWord(words[i]))
                ret[i] = "SYM";
            if (words[i].Contains(".") && words[i].Length > 5 && general.IsValidURL(words[i]))
                ret[i] = "NN";
            // rule: if Previous Word is Capitalized, Current Word is "of", Next Word is Capitalized, then tag current word as "NN".
            // Example: Department of Defense, United States of America.
            if (words[i].Equals("of")
                && (i - 1 >= 0 && words[i - 1][0].Equals(words[i - 1][0].ToString().ToUpper()) && ret[i-1].StartsWith("NN"))
                && (i + 1 < words.Count && words[i + 1][0].Equals(words[i + 1][0].ToString().ToUpper()) && ret[i+1].StartsWith("NN")))
                ret[i] = "NN";
        }
        return ret;
    }

    private bool IsSymbolWord(string word)
    {
        bool isSymbol = true;
        bool isDone = false;

        for (int i = 0; i < word.Length && !isDone; i++)
        {            
            if (!_symbols.Contains(word[i].ToString()))
            {
                isSymbol = false;
                isDone = true;
            }
        }
        return isSymbol;
    }


    public Dictionary<string, string> GetImportantTagsDictionary(List<string> tokens, List<string> posTags)
    {
        Dictionary<string, string> importantTags = new Dictionary<string, string>();

        if (tokens != null && posTags != null && tokens.Count == posTags.Count)
        {
            for (int i = 0; i < posTags.Count && importantTags.Count <= _maxTagsPerItem; i++)
            {
                string currentToken = tokens[i];
                string currentTag = posTags[i];

                //if (currentTag.Equals("NN"))
                //{
                //    string tokenToAdd = currentToken;
                //    string tagToAdd = currentTag;

                //    int j = i + 1;
                //    for (j = i + 1; j < posTags.Count && (posTags[j].Equals("NN")); j++)
                //    {
                //        tokenToAdd = tokenToAdd + " " + tokens[j];
                //    }
                //    i = j - 1;

                //    if (!importantTags.ContainsKey(tokenToAdd))
                //    {
                //        importantTags.Add(tokenToAdd, tagToAdd);
                //    }
                //}
                //else if (currentTag.Equals("NNP"))
                //{
                //    string tokenToAdd = currentToken;
                //    string tagToAdd = currentTag;

                //    int j = i + 1;
                //    for (j = i + 1; j < posTags.Count && (posTags[j].Equals("NNP")); j++)
                //    {
                //        tokenToAdd = tokenToAdd + " " + tokens[j];
                //    }
                //    i = j - 1;

                //    if (!importantTags.ContainsKey(tokenToAdd))
                //    {
                //        importantTags.Add(tokenToAdd, tagToAdd);
                //    }
                //}
                ////else
                ////{
                ////    if (!importantTags.ContainsKey(currentToken))
                ////    {
                ////        importantTags.Add(currentToken, currentTag);
                ////    }
                ////}

                if (currentToken.Length > 2 && currentTag.StartsWith("NN"))
                {
                    string tokenToAdd = currentToken;
                    string tagToAdd = currentTag;

                    int j = i + 1;
                    for (j = i + 1; j < posTags.Count && (posTags[j].StartsWith("NN")); j++)
                    {
                        tokenToAdd = tokenToAdd + " " + tokens[j];
                    }
                    i = j - 1;

                    if (!importantTags.ContainsKey(tokenToAdd))
                    {
                        importantTags.Add(tokenToAdd, tagToAdd);
                    }
                }              
                
            }
        }
        else
        {
            importantTags = null;
        }
        return importantTags;
    }


    public List<string> GetImportantTagsList(List<string> tokens, List<string> posTags)
    {
        List<string> importantTags = new List<string>();

        if (tokens != null && posTags != null && tokens.Count == posTags.Count)
        {
            for (int i = 0; i < posTags.Count && importantTags.Count <= _maxTagsPerItem; i++)
            {
                string currentToken = tokens[i];
                string currentTag = posTags[i];

                //if (currentTag.Equals("NN"))
                //{
                //    string tokenToAdd = currentToken;
                //    string tagToAdd = currentTag;

                //    int j = i + 1;
                //    for (j = i + 1; j < posTags.Count && (posTags[j].Equals("NN")); j++)
                //    {
                //        tokenToAdd = tokenToAdd + " " + tokens[j];
                //    }
                //    i = j - 1;

                //    if (!importantTags.ContainsKey(tokenToAdd))
                //    {
                //        importantTags.Add(tokenToAdd, tagToAdd);
                //    }
                //}
                //else if (currentTag.Equals("NNP"))
                //{
                //    string tokenToAdd = currentToken;
                //    string tagToAdd = currentTag;

                //    int j = i + 1;
                //    for (j = i + 1; j < posTags.Count && (posTags[j].Equals("NNP")); j++)
                //    {
                //        tokenToAdd = tokenToAdd + " " + tokens[j];
                //    }
                //    i = j - 1;

                //    if (!importantTags.ContainsKey(tokenToAdd))
                //    {
                //        importantTags.Add(tokenToAdd, tagToAdd);
                //    }
                //}
                ////else
                ////{
                ////    if (!importantTags.ContainsKey(currentToken))
                ////    {
                ////        importantTags.Add(currentToken, currentTag);
                ////    }
                ////}

                if (currentToken.Length > 2 && currentTag.StartsWith("NN"))
                {
                    string tokenToAdd = currentToken;
                    string tagToAdd = currentTag;

                    int j = i + 1;
                    for (j = i + 1; j < posTags.Count && (posTags[j].StartsWith("NN")); j++)
                    {
                        tokenToAdd = tokenToAdd + " " + tokens[j];
                    }
                    i = j - 1;

                    if (!importantTags.Contains(tokenToAdd))
                    {
                        importantTags.Add(tokenToAdd);
                    }
                }

            }
        }
        else
        {
            importantTags = null;
        }
        return importantTags;
    }


    /// <summary>
    /// Valid if the string is a number
    /// </summary>
    /// <remarks>Valid Numbers : digit(s) , digit(s)%, digit(s)/digit(s) or #digit(s)</remarks>
    /// <remarks> 1st, Digit(s)"2nd", Digit(s)"3rd", Digit(s)th </remarks>
    /// <param name="str">a string</param>
    /// <returns>true if s is an Numeric; otherwise, false.</returns>
    public static bool IsNumberToken(string str)
    {
        if (str.EndsWith("%") || str.StartsWith("%"))
        {
            // remove % in 10% for number test
            str = str.Trim('%');
        }
        // Remove Any "/" or "#" or ":" ( 3/4, 16/32, #1, #2, 3:4)
        str = str.Replace("/", "");
        str = str.Replace("#", "");                        
        str = str.Replace(":", "");
                                
        if (str.EndsWith("1st"))
        {
            str = str.Remove(str.Length - 2, 2);
            return IsNumeric(str);
        }            
        if (str.EndsWith("2nd") || str.EndsWith("3rd"))
        {
            str = str.Remove(str.Length - 3, 3);
            // If the Length 0 then it's a number.
            if (str.Length == 0)
                return true;
            if (IsNumeric(str))
                return true;
        }
        if (str.EndsWith("th"))
        {
            int strLength = str.Length;
            str = str.Remove(strLength - 2, 2);
        }            
        return IsNumeric(str);            
    }

    private static bool IsNumeric(string str)
    {
        double newVal;
        return double.TryParse(str, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out newVal);
    }
    
}
