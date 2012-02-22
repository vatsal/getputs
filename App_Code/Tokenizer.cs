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
using System.Collections.Generic;

/// <summary>
/// Summary description for Tokenizer
/// </summary>
public class Tokenizer
{

    private static volatile Tokenizer instance;
    private static object syncRoot = new Object();

    string[] _splitter = { " " };
    string _symbols = "~`!@#$%^&*()_-+={[}]|:;'<,>.?/\"\\";

    public Tokenizer()
    {
        
    }

    /// <summary>
    /// Initialize a new instance of the Tokenizer class, If required.
    /// </summary>
    public static Tokenizer Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new Tokenizer();
                }
            }
            return instance;
        }
    }

    public List<string> GetTokens(string input, bool removeSingleCharacterSymbols, bool removeDuplicateTokens)
    {
        List<string> tokens;
        if (!string.IsNullOrEmpty(input))
        {
            tokens = new List<string>();
                        
            string[] inputArray = input.Split(_splitter, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < inputArray.Length; i++)
            {
                string currentToken = inputArray[i];

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

}


