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

using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Net;
using System.Collections.Generic;
using System.Web.Mail;
using System.Security.Cryptography;

using System.Text;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

using gp.Files;
using System.Collections;

/// <summary>
/// Summary description for General
/// </summary>
public class General
{
    private static volatile General instance;
    private static object syncRoot = new Object();

    Logger log = Logger.Instance;
    DBOperations dbOps = DBOperations.Instance;
    GUIVariables gui = GUIVariables.Instance;
    ImageEngine imageEngine = ImageEngine.Instance;
    Links links = Links.Instance;
    
    
    private string smtpServer = ConfigurationManager.AppSettings["smtpServer"];
    private string testMailID = ConfigurationManager.AppSettings["testMailID"];
    
    // Define default min and max password lengths.
    private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
    private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

    // Define supported password characters divided into groups.
    // You can add (or remove) characters to (from) these groups.
    private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
    private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
    private static string PASSWORD_CHARS_NUMERIC = "0123456789";

    //  Administrator E-Mail Addresses, Used to send E-Mails to Admins.
    private string admin_EMailAddresses = ConfigurationManager.AppSettings["Admin_EMailAddresses"];    

    //  ReCaptcha_IsRecaptchaOn = 0 => Off ; ReCaptcha_IsRecaptchaOn != 0 => On
    //  Not used right now, Recaptcha is always On.
    bool recaptcha_IsRecaptchaOn = int.Parse(ConfigurationManager.AppSettings["ReCaptcha_IsRecaptchaOn"]) == 0 ? false : true;

    // For generating random numbers.
    private Random random = new Random();

    private string _captchaImageText = string.Empty;

    public string CaptchaImageText
    {
        get
        {
            return _captchaImageText;
        }
        set
        {
            _captchaImageText = value;
        }
    }

    
    /// <summary>
    /// Use for Testing Mail Functionality.
    /// </summary>
    public string TestMailID
    {
        get
        {
            return testMailID;
        }
    }

    public string Admin_EMailAddresses
    {
        get
        {
            return admin_EMailAddresses;
        }
    }

    public bool IsRecaptchaOn
    {
        get
        {
            return recaptcha_IsRecaptchaOn;
        }
    }


    public General()
    {
        FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["BadFilesPath"];
    }

    /// <summary>
    /// Initialize a new instance of the General class, If required.
    /// </summary>
    public static General Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new General();
                }
            }
            return instance;
        }
    }






    /// <summary>
    /// Send an E-Mail.
    /// </summary>
    /// <param name="fromAddress">Address of the Sender</param>
    /// <param name="toAddress">Address(es) of the Recepient(s)</param>
    /// <param name="subject">The Subject of the E-Mail</param>
    /// <param name="userMessage">The Message that the User wants to send.</param>
    /// <returns>True if Mail was sent successfully, False Otherwise.</returns>
    public bool SendMail(string fromAddress, string toAddress, string subject, string userMessage)
    {
        try
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            System.DateTime currentTime = System.DateTime.Now;


            #region Sending E-Mails to Multiple Addresses
            string[] splitter = { "," , ";" };
            string[] toAddressArray = toAddress.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            if (toAddressArray != null && toAddressArray.Length > 0)
            {
                for (int i = 0; i < toAddressArray.Length; i++)
                {
                    string to = toAddressArray[i].Trim();
                    System.Net.Mail.MailAddress ma = new System.Net.Mail.MailAddress(to);
                    if (!message.To.Contains(ma))
                    {
                        message.To.Add(ma);
                    }
                }
            }
            #endregion Sending E-Mails to Multiple Addresses

            //  message.To.Add(toAddress);
            message.IsBodyHtml = true;
            message.Subject = subject;            
            message.From = new System.Net.Mail.MailAddress(fromAddress);
            message.Body = userMessage
                            + @"<br/>"
                            + @"<br/>"
                            + @"<br/>"
                            + "Visit www.getputs.com"
                            + @"<br/>";
            

            //  System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("mail.hakia.com");
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpServer);            
            smtp.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            if (log.isLoggingOn && log.isAppLoggingOn)
            {
                log.Log("Error while sending an e-mail"
                    + Environment.NewLine
                    + smtpServer);
                log.Log(ex);
            }
            return false;
        }
    }
       
    
    public bool IsValidEMail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }
        else
        {

            string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                        + @"[a-zA-Z]{2,}))$";
            Regex emailRegex = new Regex(emailPattern, RegexOptions.Compiled);
            return emailRegex.IsMatch(email);
        }
    }

    public bool IsValidEMailList(string emails)
    {
        if (string.IsNullOrEmpty(emails))
        {
            return false;
        }
        else
        {
            bool isValid = false;
            
            string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                        + @"[a-zA-Z]{2,}))$";
            Regex emailRegex = new Regex(emailPattern, RegexOptions.Compiled);
                        
            string[] splitter = { ",", ";" };
            string[] toAddressArray = emails.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            if (toAddressArray != null && toAddressArray.Length > 0)
            {
                for (int i = 0; i < toAddressArray.Length; i++)
                {
                    string to = toAddressArray[i].Trim();
                    
                    isValid = emailRegex.IsMatch(to);
                    if (!isValid)
                    {
                        break;
                    }
                }
            }
            return isValid;
        }
    }




    public bool IsAlphabetOrNumber(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsLetterOrDigit(input[i]))
            {
                return false;
            }
        }
        return true;
    }
    
    //  Check if the Search Query is a URL.
    //  Send a WebRequest to a specified URL.
    //  If you get a response, it means it is a URL. Return True.
    //  Else, catch the exception in Catch Block. Return False.
    public bool IsValidURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            if (!url.Contains(" ") && url.Contains("."))
            {
                //if (url.StartsWith("http://") || url.StartsWith("https://"))
                //{

                //}
                //else if (url.StartsWith("www."))
                //{
                //    url = "http://" + url;
                //}
                //else
                //{
                //    url = "http://www." + url;
                //}

                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {

                }               
                else
                {
                    url = "http://" + url;
                }


                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {

                }
                else if (url.StartsWith("www."))
                {
                    url = "http://" + url;
                }
                else
                {
                    url = "http://www." + url;
                }



                try
                {

                    string linkPattern = @"(?#WebOrIP)((?#protocol)((http|https):\/\/)?(?#subDomain)(([a-zA-Z0-9]+\.(?#domain)[a-zA-Z0-9\-]+(?#TLD)(\.[a-zA-Z]+){1,2})|(?#IPAddress)((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])))+(?#Port)(:[1-9][0-9]*)?)+(?#Path)((\/((?#dirOrFileName)[a-zA-Z0-9_\-\%\~\+]+)?)*)?(?#extension)(\.([a-zA-Z0-9_]+))?(?#parameters)(\?([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?(?#additionalParameters)(\&([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?)*)?";
                    Regex linkRegex = new Regex(linkPattern, RegexOptions.Compiled);
                    bool isValid = linkRegex.IsMatch(url);

                    if (!isValid)
                    {
                        //  Option 1
                        WebRequest hReq = WebRequest.Create(url);
                        WebResponse hResp = hReq.GetResponse();
                        hResp.Close();
                        isValid = true;
                    }                   
                    
                    return isValid;
                    

                    //  Option 2
                    //HttpWebRequest hReq = (HttpWebRequest)WebRequest.Create(url);
                    //HttpWebResponse hResp = (HttpWebResponse)hReq.GetResponse();
                    //hResp.Close();
                    //return true;

                    //  Option 2
                    //string linkPattern = @"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$";                
                    //Regex linkRegex = new Regex(linkPattern, RegexOptions.Compiled);
                    //return linkRegex.IsMatch(url);

                    ////  Option 3
                    //string linkPattern = @"(?#WebOrIP)((?#protocol)((http|https):\/\/)?(?#subDomain)(([a-zA-Z0-9]+\.(?#domain)[a-zA-Z0-9\-]+(?#TLD)(\.[a-zA-Z]+){1,2})|(?#IPAddress)((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])))+(?#Port)(:[1-9][0-9]*)?)+(?#Path)((\/((?#dirOrFileName)[a-zA-Z0-9_\-\%\~\+]+)?)*)?(?#extension)(\.([a-zA-Z0-9_]+))?(?#parameters)(\?([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?(?#additionalParameters)(\&([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?)*)?";                
                    //Regex linkRegex = new Regex(linkPattern, RegexOptions.Compiled);
                    //return linkRegex.IsMatch(url);


                }
                catch (Exception e)
                {
                    log.Log(url);
                    log.Log(e);
                    return false;

                    //  Option 3
                    //string linkPattern = @"(?#WebOrIP)((?#protocol)((http|https):\/\/)?(?#subDomain)(([a-zA-Z0-9]+\.(?#domain)[a-zA-Z0-9\-]+(?#TLD)(\.[a-zA-Z]+){1,2})|(?#IPAddress)((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])))+(?#Port)(:[1-9][0-9]*)?)+(?#Path)((\/((?#dirOrFileName)[a-zA-Z0-9_\-\%\~\+]+)?)*)?(?#extension)(\.([a-zA-Z0-9_]+))?(?#parameters)(\?([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?(?#additionalParameters)(\&([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?)*)?";
                    //Regex linkRegex = new Regex(linkPattern, RegexOptions.Compiled);
                    //return linkRegex.IsMatch(url);                   
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    public bool IsValidInt(string strToBeChecked)
    {
        if (string.IsNullOrEmpty(strToBeChecked))
        {
            return false;
        }
        else
        {
            int intIID;
            bool isInt = int.TryParse(strToBeChecked, out intIID);
            return isInt;
        }
    }

    public string GetFormattedDate(string datetime)
    {
        string returnStr = string.Empty;

        DateTime dtItem;
        DateTime dtCurrent = DateTime.Now;

        //  dtCurrent = dtCurrent.Subtract(new TimeSpan(12, 0, 0));
        //  dtCurrent = dtCurrent.Add(new TimeSpan(12, 0, 0));

        
        
        
        if (DateTime.TryParse(datetime, out dtItem))        
        {
            TimeSpan ts = dtCurrent.Subtract(dtItem);

                    
            
            if (ts.Days == 0)
            {
                if (ts.Hours == 0)
                {
                    if (ts.Minutes >= 0)
                    {
                        returnStr = ts.Minutes == 1 || ts.Minutes == 0 ? "1 minute" : ts.Minutes.ToString() + " minutes";
                    }
                }
                else
                {
                    returnStr = ts.Hours == 1 ? "1 hour" : ts.Hours.ToString() + " hours";
                }

                //if (log.isLoggingOn && log.isAppLoggingOn)
                //{
                //    string logDates = "Current: " + dtCurrent.ToString() + Environment.NewLine
                //        + "Item Date: " + dtItem.ToString() + Environment.NewLine
                //        + "Time Span: " + ts.ToString();

                //    log.Log(logDates);
                //}     
            }
            else
            {
                returnStr = ts.Days == 1 ? "1 day" : ts.Days.ToString() + " days";
            }


        }


        return returnStr;
    }

    /// <summary>
    /// Get Item IID's for Items that were saved by UserID = UID.
    /// </summary>
    /// <param name="UID">The UID of the User</param>
    /// <returns>The List of Saved Item IID's</returns>
    public List<int> GetSavedItemIID(string UID)
    {
        List<int> savedItems = new List<int>();
        if (!string.IsNullOrEmpty(UID))
        {
            string queryString = "SELECT IID FROM saveditems WHERE UID= '" + UID + "';";
            MySqlDataReader retList = dbOps.ExecuteReader(queryString);
            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    savedItems.Add(Convert.ToInt32(retList["IID"]));
                }
                retList.Close();
            }
        }
        else
        {
            savedItems = null;
        }
        return savedItems;
    }

    /// <summary>
    /// Get Item IID's for Items that were rated by UserID = UID.
    /// </summary>
    /// <param name="UID">The UID of the User</param>
    /// <returns>The List of Rated Item IID's</returns>
    public List<int> GetRatedItemsIID(string UID)
    {
        List<int> ratedItems = new List<int>();
        if (!string.IsNullOrEmpty(UID))
        {
            string queryString = "SELECT IID FROM rating WHERE UID= '" + UID + "';";
            MySqlDataReader retList = dbOps.ExecuteReader(queryString);
            if (retList != null && retList.HasRows)
            {
                while (retList.Read())
                {
                    ratedItems.Add(Convert.ToInt32(retList["IID"]));
                }
                retList.Close();
            }
        }
        else
        {
            ratedItems = null;
        }
        return ratedItems;
    }


    /// <summary>
    /// Log the Query into the searchlog DB.
    /// </summary>
    /// <param name="query">The Query.</param>
    /// <param name="uid">The UID of the User (can be null)</param>
    /// <param name="date">The Date/Time</param>
    /// <param name="ip">The IP Address</param>
    public void LogQuery(string query, string uid, string date, string ip)
    {
        string queryString = "INSERT INTO searchlog (Query, UID, Date, IP) values "
                                            + "('" + query + "', '" + uid + "', DATE_FORMAT('" + date + "','%Y-%m-%d %k:%i:%S' ) , INET_ATON('" + ip + "') );";

        int retVal = dbOps.ExecuteNonQuery(queryString);
        if (retVal >= 0)    //  Item successfully submitted.
        {

        }
    }

    

    public List<string> ConvertCSVToList(string csv)
    {
        if (!string.IsNullOrEmpty(csv))
        {
            List<string> list = new List<string>();
            
            string[] delimiter = new string[1];
            delimiter[0] = ",";

            string[] csvArray = csv.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < csvArray.Length; i++)
            {
                list.Add(csvArray[i].Trim());
            }
            return list;
        }
        else
        {
            return null;
        }
    }



    public string ConvertListToCSV(List<string> impTags)
    {
        string csvString = string.Empty;

        if (impTags != null && impTags.Count > 0)
        {
            for (int i = 0; i < impTags.Count; i++)
            {
                csvString = csvString + "," + impTags[i];
            }
        }
        if (!string.IsNullOrEmpty(csvString))
        {
            csvString = csvString.Trim();
            csvString = csvString.Trim(',');
        }
        return csvString;
    }



    




    /// <summary>
    /// Check if the UID has Administrative Privileges.
    /// </summary>
    /// <param name="UID">UID</param>
    /// <returns>True, If User = Administrator, False otherwise.</returns>
    public bool IsUserAdministrator(string UID)
    {
        bool isAdmin = false;
        string queryString = "SELECT Admin FROM user WHERE UID ='" + UID + "';";
        int intResult = dbOps.ExecuteScalar(queryString);
        if (intResult != -1)
        {
            if (intResult == 1)
            {
                isAdmin = true;
            }
        }
        return isAdmin;
    }


    
    public string EscapeCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        else
        {
            input = input.Replace("'", "''");
            input = input.Replace("`", "''");
            return input;
        }
    }

    public string ProcessTextForDisplay(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        else
        {
            input = input.Replace("\r\n", gui.LineBreak);            
            input = ConvertTextLinksToHTMLLinks(input);
            return input;
        }
    }

    public string ConvertTextLinksToHTMLLinks(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        else
        {
            ////  string linkPattern = @"(?#WebOrIP)((?#protocol)((http|https):\/\/)?(?#subDomain)(([a-zA-Z0-9]+\.(?#domain)[a-zA-Z0-9\-]+(?#TLD)(\.[a-zA-Z]+){1,2})|(?#IPAddress)((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])))+(?#Port)(:[1-9][0-9]*)?)+(?#Path)((\/((?#dirOrFileName)[a-zA-Z0-9_\-\%\~\+]+)?)*)?(?#extension)(\.([a-zA-Z0-9_]+))?(?#parameters)(\?([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?(?#additionalParameters)(\&([a-zA-Z0-9_\-]+\=[a-z-A-Z0-9_\-\%\~\+]+)?)*)?";
            //string strPattern = @"(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?";
            //Regex linkRegex = new Regex(strPattern, RegexOptions.Compiled);
            //MatchCollection linkMC = linkRegex.Matches(input);
            //for (int i = 0; i < linkMC.Count; i++)
            //{
            //    string originalLink = linkMC[i].Value.Trim();
            //    string link = originalLink;
            //    if (originalLink.StartsWith("http://") || originalLink.StartsWith("https://") || originalLink.StartsWith("ftp://") || originalLink.StartsWith("ftps://"))
            //    {
            //        link = originalLink;
            //    }
            //    else if (originalLink.StartsWith("www."))
            //    {
            //        link = "http://" + originalLink;
            //    }
            //    else
            //    {
            //        link = "http://www." + originalLink;
            //    }

            //    //  input = input.Replace(originalLink, "<a href='" + link + "'>" + originalLink + "</a>");
                
            //    //  if (linkMC[i].Captures.Count == 1)
            //    {
            //        input = input.Replace(" " + originalLink, " " + "<a href='" + link + "' class='CSS_LinkItem'>" + originalLink + "</a>");
            //    }
            //}            
            return input;
        }
    }


    public string GenerateRandomPassword()
    {
        return GenerateRandomPassword(DEFAULT_MIN_PASSWORD_LENGTH,
                        DEFAULT_MAX_PASSWORD_LENGTH);
    }

    public string GenerateRandomPassword(int length)
    {
        return GenerateRandomPassword(length, length);
    }

    public string GenerateRandomPassword(int minLength, int maxLength)
    {
        // Make sure that input parameters are valid.
        if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
            return null;

        // Create a local array containing supported password characters grouped by types. 
        //  You can remove character groups from this array, but doing so will weaken the password strength.
        char[][] charGroups = new char[][] 
        {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray()
            
        };

        // Use this array to track the number of unused characters in each character group.
        int[] charsLeftInGroup = new int[charGroups.Length];

        // Initially, all characters in each group are not used.
        for (int i = 0; i < charsLeftInGroup.Length; i++)
            charsLeftInGroup[i] = charGroups[i].Length;

        // Use this array to track (iterate through) unused character groups.
        int[] leftGroupsOrder = new int[charGroups.Length];

        // Initially, all character groups are not used.
        for (int i = 0; i < leftGroupsOrder.Length; i++)
            leftGroupsOrder[i] = i;

        // Because we cannot use the default randomizer, which is based on the
        // current time (it will produce the same "random" number within a
        // second), we will use a random number generator to seed the
        // randomizer.

        // Use a 4-byte array to fill it with random bytes and convert it then to an integer value.
        byte[] randomBytes = new byte[4];

        // Generate 4 random bytes.
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(randomBytes);

        // Convert 4 bytes into a 32-bit integer value.
        int seed = (randomBytes[0] & 0x7f) << 24 |
                    randomBytes[1] << 16 |
                    randomBytes[2] << 8 |
                    randomBytes[3];

        // Now, this is real randomization.
        Random random = new Random(seed);

        // This array will hold password characters.
        char[] password = null;

        // Allocate appropriate memory for the password.
        if (minLength < maxLength)
            password = new char[random.Next(minLength, maxLength + 1)];
        else
            password = new char[minLength];

        // Index of the next character to be added to password.
        int nextCharIdx;

        // Index of the next character group to be processed.
        int nextGroupIdx;

        // Index which will be used to track not processed character groups.
        int nextLeftGroupsOrderIdx;

        // Index of the last non-processed character in a group.
        int lastCharIdx;

        // Index of the last non-processed group.
        int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

        // Generate password characters one at a time.
        for (int i = 0; i < password.Length; i++)
        {
            // If only one character group remained unprocessed, process it;
            // otherwise, pick a random character group from the unprocessed
            // group list. To allow a special character to appear in the
            // first position, increment the second parameter of the Next
            // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
            if (lastLeftGroupsOrderIdx == 0)
                nextLeftGroupsOrderIdx = 0;
            else
                nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

            // Get the actual index of the character group, from which we will
            // pick the next character.
            nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

            // Get the index of the last unprocessed characters in this group.
            lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

            // If only one unprocessed character is left, pick it; otherwise,
            // get a random character from the unused character list.
            if (lastCharIdx == 0)
                nextCharIdx = 0;
            else
                nextCharIdx = random.Next(0, lastCharIdx + 1);

            // Add this character to the password.
            password[i] = charGroups[nextGroupIdx][nextCharIdx];

            // If we processed the last character in this group, start over.
            if (lastCharIdx == 0)
                charsLeftInGroup[nextGroupIdx] =
                                          charGroups[nextGroupIdx].Length;
            // There are more unprocessed characters left.
            else
            {
                // Swap processed character with the last unprocessed character
                // so that we don't pick it until we process all characters in
                // this group.
                if (lastCharIdx != nextCharIdx)
                {
                    char temp = charGroups[nextGroupIdx][lastCharIdx];
                    charGroups[nextGroupIdx][lastCharIdx] =
                                charGroups[nextGroupIdx][nextCharIdx];
                    charGroups[nextGroupIdx][nextCharIdx] = temp;
                }
                // Decrement the number of unprocessed characters in
                // this group.
                charsLeftInGroup[nextGroupIdx]--;
            }

            // If we processed the last group, start all over.
            if (lastLeftGroupsOrderIdx == 0)
                lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
            // There are more unprocessed groups left.
            else
            {
                // Swap processed group with the last unprocessed group
                // so that we don't pick it until we process all groups.
                if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                {
                    int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                    leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                leftGroupsOrder[nextLeftGroupsOrderIdx];
                    leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                }
                // Decrement the number of unprocessed groups.
                lastLeftGroupsOrderIdx--;
            }
        }

        // Convert password characters into a string and return the result.
        return new string(password);
    }


    public bool IsBadUser(string uid)
    {        
        return BadUsers.Instance.Contains(uid);
    }

    public bool IsBadIP(string ip)
    {        
        return BadIPs.Instance.Contains(ip);
    }

    public bool IsBadSite(string link)
    {
        if (link.StartsWith("http://"))
        {
            link = link.Substring(7);
        }
        else if (link.StartsWith("https://"))
        {
            link = link.Substring(8);
        }

        if (link.StartsWith("www."))
        {
            link = link.Substring(4);
        }

        return BadSites.Instance.Contains(link);
    }

    public bool IsBadWord(string word)
    {
        return BadWords.Instance.Contains(word);
    }

    public bool IsImageWordToBeIgnored(string word)
    {
        return ImageWordsToBeIgnored.Instance.Contains(word);
    }
    

    /// <summary>
    /// Check the UID, IP and Link BlackLists
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="ip"></param>
    /// <param name="link"></param>
    /// <returns>True If any one of the input parameters is found inside the relevant blacklists; False otherwise.</returns>
    public bool CheckBlackList(string uid, string ip, string link)
    {
        if (link.StartsWith("http://"))
        {
            link = link.Substring(7);
        }
        else if (link.StartsWith("https://"))
        {
            link = link.Substring(8);
        }

        if (link.StartsWith("www."))
        {
            link = link.Substring(4);
        }


        return (BadUsers.Instance.Contains(uid)
            || BadIPs.Instance.Contains(ip)
            || BadSites.Instance.Contains(link));
    }


    public void ReloadBadUsersLists()
    {
        BadUsers.Instance.ReLoadList();
    }

    public void ReloadBadIPsLists()
    {
        BadIPs.Instance.ReLoadList();
    }

    public void ReloadBadSitesLists()
    {
        BadSites.Instance.ReLoadList();
    }

    public void ReloadBadDomainsLists()
    {
        BadWords.Instance.ReLoadList();
    }

    public void ReloadImageWordsToBeIgnoredLists()
    {
        ImageWordsToBeIgnored.Instance.ReLoadList();
    }

    public void ReloadTagLexicon()
    {
        Tagger tagger = new Tagger();
        tagger.LoadTagLexicon(true);
        tagger.ReLoadLexiconHash();
        
        
    }



    public string GenerateRandomCode()
    {
        string s = string.Empty;
        for (int i = 0; i < 4; i++)
            s = String.Concat(s, this.random.Next(10).ToString());
        return s;
    }

    public string GetIP(HttpRequest Request)
    {        
        string ipAddress;
        ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = Request.ServerVariables["REMOTE_ADDR"];
        }
        return ipAddress;
    }



    /// <summary>
    /// Get Images from the Input Page.
    /// </summary>
    /// <param name="url">The URL of the Web Page</param>
    /// <param name="numOfImages">Number of Images to Retrieve.</param>
    /// <param name="onlyImagesFromHTMLBody">If True, Then only retrieve images which are in the body of the HTML</param>
    /// <returns>Image Tags in HTML Format</returns>
    public List<string> GetImages(string url, int numOfImages, ImageEngine.HTMLImageLocation htmlImageLocation)
    {
        //  StringBuilder imageSB = new StringBuilder();
        List<string> imageList = new List<string>();
        
        try
        {            
            if (IsValidURL(url))
            {
                if (!url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }

                using (WebClient wc = new WebClient())
                {
                    byte[] data = wc.DownloadData(url);
                    string htmlText = Encoding.ASCII.GetString(data);
                    
                    string textToProcess = GetHTMLImageTextToProcess(htmlText, htmlImageLocation);

                    if (!string.IsNullOrEmpty(textToProcess))
                    {

                        //  This works
                        //  Regex imageRegex = new Regex(@"img\ssrc\s*=\s*\""*[^\"">]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        Regex imageRegex = new Regex(@"<\s*\bimg\b\s.*\bsrc\b\s*=\s*[\""|'](?'SRC'[^\s\""']+)[\""|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);


                        MatchCollection matchCollection = imageRegex.Matches(textToProcess);
                        foreach (Match imageMatch in matchCollection)
                        {
                            string imgSrc = imageMatch.Groups["SRC"].Captures[0].Value.ToLower();


                            bool isValidFormat = false;
                            for (int i = 0; i < imageEngine.ItemImageValidFormats.Count; i++)
                            {
                                if (imgSrc.EndsWith("." + imageEngine.ItemImageValidFormats[i]))
                                {
                                    isValidFormat = true;
                                    break;
                                }
                            }


                            //  if (imgSrc.EndsWith(".jpg") || imgSrc.EndsWith(".png") || imgSrc.EndsWith(".jpeg") || imgSrc.EndsWith(".gif"))
                            //  if (imageMatch.Captures[0].Value.EndsWith(".jpg") || imageMatch.Captures[0].Value.EndsWith(".png") || imageMatch.Captures[0].Value.EndsWith(".jpeg") || imageMatch.Captures[0].Value.EndsWith(".gif"))
                            if (isValidFormat)                            
                            {
                                //  string imgSrc = match.Captures[0].Value.ToLower().Replace("img src=\"", "");
                                //  Regex imageSrcRegex = new Regex("src=[\"|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                                //  string imgSrc = imageMatch.Captures[0].Value.ToLower().Replace("img src=\"", "");

                                if (!imgSrc.Contains("http://"))
                                {
                                    Uri uri = new Uri(url);
                                    string website = uri.Host;
                                    imgSrc = website + "/" + imgSrc;

                                    if (!imgSrc.StartsWith("http://"))
                                    {
                                        imgSrc = "http://" + imgSrc;
                                    }
                                }

                                bool isGood = true;
                                for (int x = 0; x < imageEngine.ImageWordsToBeIgnored.Count; x++)
                                {
                                    if (imgSrc.Contains(imageEngine.ImageWordsToBeIgnored[x]))
                                    {
                                        isGood = false;
                                    }
                                }

                                if (isGood && !imageList.Contains(imgSrc))
                                {
                                    try
                                    {
                                        byte[] imageData = wc.DownloadData(imgSrc);

                                        if (imageList.Count < numOfImages)
                                        {
                                            if (imageData.Length > 1024 && imageData.Length < 15000)    //  Only store the image if its size is greater than 2KB.
                                            {
                                                imageList.Add(imgSrc);
                                                //  imageSB.AppendLine("<img src='" + imgSrc + "' class='CSS_ItemImage'>" + "</img>");
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                            }
                        }


                    }



                }
            }
            else
            {
                //  imageSB.Append(string.Empty);                
            }            
        }
        catch
        {
            imageList = null;
        }
        return imageList;
    }





    /// <summary>
    /// Get Images from the getputs Image Store.
    /// </summary>
    /// <param name="url">The URL of the Web Page</param>
    /// <param name="numOfImages">Number of Images to Retrieve.</param>
    /// <param name="iid">The Item ID</param>
    /// <returns>Image Tags in HTML Format</returns>
    public List<string> GetImages(string url, int numOfImages, int iid)
    {
        //  StringBuilder imageSB = new StringBuilder();
        List<string> imageList = new List<string>();

        try
        {            
            //  Image FileName Format: IID-1; IID-2; IID-3
            //  Example: 444-1; 444-2; 444-3;
            //  Final Path: //Images//ItemImageStore// IID // IID-1 

            string imageDirectoryName = Path.Combine(imageEngine.ItemThumbNailStorePath, iid.ToString());
            if (Directory.Exists(imageDirectoryName))
            {              
                DirectoryInfo dirInfo = new DirectoryInfo(imageDirectoryName);
                // Get a list of fileInfo. Since we store files in .png format, only select files of type .png
                FileInfo[] fileInfo = dirInfo.GetFiles("*.png"); 

                for (int i = 0; i < fileInfo.Length && i < numOfImages; i++)
                {
                    string fileName = fileInfo[i].Name;
                    if (fileInfo[i].Length > 0)     //  Only consider files of size > 0
                    {
                        //  string imgSrc = Path.Combine(imageDirectoryName, fileName);
                        //  string imgSrc = imageDirectoryName + "//" + fileName;
                        
                        //  This works
                        //  string imgSrc = "\\Images\\ItemThumbNailStore\\" + iid.ToString() + "\\" + fileName;

                        string imgSrc = links.DomainLink + "/Images/ItemThumbNailStore/" + iid.ToString() + "/" + fileName;
                        

                        imageList.Add(imgSrc);
                    }
                }
            }
            else
            {
                imageList = null;
            }

            
        }
        catch(Exception ex)
        {
            imageList = null;

            string exMessage = "(GetImages)"                                           
               + Environment.NewLine + "Link: " + url
               + Environment.NewLine + ex.Message
               + Environment.NewLine + ex.StackTrace;
            log.Log(exMessage);
        }
        return imageList;
    }




    /// <summary>
    /// Get Images from the Input Page and Store them into the getputs Image Store.
    /// </summary>
    /// <param name="url">The URL of the Web Page</param>
    /// <param name="numOfImages">Number of Images to Retrieve.</param>
    /// <param name="iid">The Item ID</param>
    /// <param name="onlyImagesFromHTMLBody">If True, Then only retrieve images which are in the body of the HTML</param>
    /// <returns>Image Tags in HTML Format</returns>
    //  public bool StoreImages(string url, int numOfImages, int iid, bool onlyImagesFromHTMLBody)
    public bool StoreImages(string url, int numOfImages, int iid, ImageEngine.HTMLImageLocation htmlImageLocation)        
    {
        bool toReturn = true;

        //  StringBuilder imageSB = new StringBuilder();

        //  imageList is currently used for the single purpose of making sure that duplicate images are not included.
        List<string> imageList = new List<string>();
        
        List<Bitmap> imageDataList = new List<Bitmap>();

        List<Bitmap> imageBitmapList = new List<Bitmap>();

        try
        {
            if (IsValidURL(url))
            {
                if (!url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }

                using (WebClient wc = new WebClient())
                {
                    byte[] data = wc.DownloadData(url);
                    string htmlText = Encoding.ASCII.GetString(data);

                    string textToProcess = GetHTMLImageTextToProcess(htmlText, htmlImageLocation);

                    if (!string.IsNullOrEmpty(textToProcess))
                    {
                        //  This works
                        //  Regex imageRegex = new Regex(@"img\ssrc\s*=\s*\""*[^\"">]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        //  This works, but does not allow image files whose names have spaces.
                        //  Regex imageRegex = new Regex(@"<\s*\bimg\b\s.*\bsrc\b\s*=\s*[\""|'](?'SRC'[^\s\""']+)[\""|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);


                        //  This works perfectly fine, except that it is greedy matching.
                        //  Regex imageRegex = new Regex(@"<\s*\bimg\b\s.*\bsrc\b\s*=\s*[\""|'](?'SRC'[^\""']+)[\""|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        Regex imageRegex = new Regex(@"<\s*\bimg\b\s.*?\bsrc\b\s*=\s*[\""|'](?'SRC'[^\""']+)[\""|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);


                        MatchCollection matchCollection = imageRegex.Matches(textToProcess);
                        foreach (Match imageMatch in matchCollection)
                        {
                            //  Links and URL's should not be converted to Lower Case.
                            //  string imgSrc = imageMatch.Groups["SRC"].Captures[0].Value.ToLower();
                            string imgSrc = imageMatch.Groups["SRC"].Captures[0].Value;


                            bool isValidFormat = false;

                            if (imgSrc.Contains("?"))
                            {
                                isValidFormat = true;
                            }
                            else
                            {

                                for (int i = 0; i < imageEngine.ItemImageValidFormats.Count; i++)
                                {
                                    //  Since imgSrc is not converted to lower, make the check using imgSrc.ToLower()
                                    if (imgSrc.ToLower().EndsWith("." + imageEngine.ItemImageValidFormats[i]))
                                    {
                                        isValidFormat = true;
                                        break;
                                    }
                                }
                            }

                            //  if (imgSrc.EndsWith(".jpg") || imgSrc.EndsWith(".png") || imgSrc.EndsWith(".jpeg") || imgSrc.EndsWith(".gif"))
                            //  if (imageMatch.Captures[0].Value.EndsWith(".jpg") || imageMatch.Captures[0].Value.EndsWith(".png") || imageMatch.Captures[0].Value.EndsWith(".jpeg") || imageMatch.Captures[0].Value.EndsWith(".gif"))
                            if (isValidFormat)
                            {
                                //  string imgSrc = match.Captures[0].Value.ToLower().Replace("img src=\"", "");
                                //  Regex imageSrcRegex = new Regex("src=[\"|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                //  string imgSrc = imageMatch.Captures[0].Value.ToLower().Replace("img src=\"", "");

                                //  Since imgSrc is not converted to lower, make the check using imgSrc.ToLower()
                                if (!imgSrc.ToLower().StartsWith("http://"))
                                {
                                    Uri uri = new Uri(url);
                                    string website = uri.Host;


                                    
                                    if (imgSrc.ToLower().StartsWith("/"))
                                    {
                                        //  Example: /images/spacer.gif
                                        //  Absolute path: http://cricinfo.com/images/spacer.gif
                                        //  Retrieve just the URL Host.

                                        imgSrc = website + imgSrc; 
                                    }
                                    else
                                    {
                                        //  Example: images/spacer.gif
                                        //  Absolute path: http://cricinfo.com/content/usa/images/spacer.gif
                                        //  Retrieve all the segments of the URL except for the last segment.
                                        //  The last segment represents the web-page name.

                                        //  Example:
                                        //  http://www-personal.umich.edu/~mejn/election/2008/
                                        //  uri.Segments.Length = 4;
                                        //  uri.Segments[0] = /
                                        //  uri.Segments[1] = ~mejn
                                        //  uri.Segments[2] = election
                                        //  uri.Segments[3] = 2008
                                        //  That's why don't do uri.Segments.Length - 1, instead do uri.Segments.Length;

                                        for (int w = 0; w < uri.Segments.Length - 1; w++)
                                        //  for (int w = 0; w < uri.Segments.Length; w++)
                                        {
                                            website = website + uri.Segments[w];
                                        }
                                        
                                        if (!website.EndsWith("/"))
                                        {
                                            imgSrc = website + "/" + imgSrc;
                                        }
                                        else
                                        {
                                            imgSrc = website + imgSrc;
                                        }

                                    }
                                    
                                    if (!imgSrc.ToLower().StartsWith("http://"))
                                    {
                                        imgSrc = "http://" + imgSrc;
                                    }
                                }

                                bool isGood = true;
                                
                                foreach (KeyValuePair<string, bool> kvp in ImageWordsToBeIgnored.Instance.GetImageWordsToBeIgnored())
                                {
                                    if (imgSrc.Contains(kvp.Key))
                                    {
                                        isGood = false;
                                    }
                                }


                                if (isGood && !imageList.Contains(imgSrc.ToLower()))
                                {
                                    try
                                    {
                                        byte[] imageData = wc.DownloadData(imgSrc);

                                        if (imageData.Length > 2048)
                                        {
                                            Bitmap bmp = imageEngine.CreateThumbnail(imageData);
                                            if (bmp != null)
                                            {
                                                imageDataList.Add(bmp);
                                                imageList.Add(imgSrc.ToLower());
                                            }
                                        }                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        //  Most likely the exception will be because of not being able to Download the Data.

                                        string exMessage = "(StoreImages) Error while Downloading Image Data:"
                                            + Environment.NewLine + "Image: " + imgSrc
                                            + Environment.NewLine + "Link: " + url
                                            + Environment.NewLine + ex.Message 
                                            + Environment.NewLine + ex.StackTrace;
                                        log.Log(exMessage);

                                        //  toReturn = false;
                                        //  break;
                                    }
                                }
                            }
                        }   //  MatchCollection Loop Ends.
                       
                        
                        //  Considering that the bigger the image, the more relevant it is.                        
                        if (imageDataList != null && imageDataList.Count > 0)
                        {
                            //  Sort the imageDataList based on the Length of the byte Array.
                            //ByteArraySorter iSorter = new ByteArraySorter();
                            //imageDataList.Sort(iSorter);


                            //  Sort the imageDataList based on the Bitmap Sorter.
                            BitmapSorter iSorter = new BitmapSorter();
                            imageDataList.Sort(iSorter);

                            int iCount = 1;

                            for (int n = 0; n < imageDataList.Count && n < numOfImages; n++)
                            {
                                //  byte[] imageData = imageDataList[n];
                                Bitmap imageData = imageDataList[n];
                                
                                //  Prepare the Image FileName
                                string imageFileName = iid.ToString() + "-" + iCount.ToString();

                                if (imageEngine.IsItemThumbNailStorageOn)
                                {
                                    try
                                    {
                                        //  Bitmap _bmp = imageEngine.CreateThumbnail(imageData, 100, 100);

                                        //  Width = 100; Height = 100;
                                        //  Bitmap _bmp = imageEngine.ScaleBitmap(imageData, 100, 100);
                                        //  Width = 125; Height = 125;
                                        Bitmap _bmp = imageEngine.ScaleBitmap(imageData, 125, 125);

                                        if (_bmp != null)
                                        {
                                            string thumbNailDirectoryName = Path.Combine(imageEngine.ItemThumbNailStorePath, iid.ToString());
                                            if (!Directory.Exists(thumbNailDirectoryName))
                                            {
                                                DirectoryInfo dirInfo = Directory.CreateDirectory(thumbNailDirectoryName);
                                            }
                                            string thumbNailPath = Path.Combine(thumbNailDirectoryName, imageFileName);
                                            _bmp.Save(thumbNailPath + ".png", System.Drawing.Imaging.ImageFormat.Png);

                                            iCount++;
                                            _bmp.Dispose();
                                        }
                                        else
                                        {
                                            //  The ThumbNail was not saved. Delete the original file.
                                            //  Could be one of the following reasons:
                                            //  (1) Error in CreateThumbnail.
                                            //  (2) Size of the image resembles an icon (H < 50 || W < 50).                                       
                                        }
                                    }

                                    catch (Exception ex)
                                    {
                                        //  Most likely the exception will be because of not being able to Save the Image.

                                        string exMessage = "(StoreImages) Error while Processing the ImageDataList OR while Saving the Image:"
                                            + Environment.NewLine + "Link: " + url
                                            + Environment.NewLine + ex.Message
                                            + Environment.NewLine + ex.StackTrace;
                                        log.Log(exMessage);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //  Do Nothing.                       
                    }                    
                }
            }
            else
            {                
                toReturn = false;
            }
        }
        catch(Exception ex)
        {
            //  Most likely the exception will be because of not being able to Download the Data from the Input Link.

            string exMessage = "(StoreImages) Error while Downloading Data from the Input Link:"                
                + Environment.NewLine + "Link: " + url
                + Environment.NewLine + ex.Message
                + Environment.NewLine + ex.StackTrace;
            log.Log(exMessage);


            toReturn = false;
        }
        return toReturn;
    }

    private string GetHTMLImageTextToProcess(string htmlText, ImageEngine.HTMLImageLocation htmlImageLocation)
    {
        string textToProcess = htmlText;
        
        if (!string.IsNullOrEmpty(textToProcess))
        {
            switch (htmlImageLocation)
            {
                case (ImageEngine.HTMLImageLocation.HTMLNone):
                    break;
                case (ImageEngine.HTMLImageLocation.HTMLTitle):
                    if (htmlText.Contains("</title>"))
                    {
                        int startIndex = htmlText.IndexOf("<head>");
                        int endIndex = htmlText.IndexOf("</head>");

                        //  int startIndex = 0;
                        //  int endIndex = htmlText.Length - 1;

                        if (startIndex != -1 && endIndex != -1 && (endIndex > startIndex))
                        {
                            textToProcess = htmlText.Substring(startIndex, endIndex - startIndex).Replace("<head>", "").Replace("</head>", "");
                            //  bodyText = htmlText;
                        }
                    }
                    break;
                case (ImageEngine.HTMLImageLocation.HTMLBody):
                    if (htmlText.Contains("</body>"))
                    {
                        int startIndex = htmlText.IndexOf("<body>");
                        int endIndex = htmlText.IndexOf("</body>");

                        //  int startIndex = 0;
                        //  int endIndex = htmlText.Length - 1;

                        if (startIndex != -1 && endIndex != -1 && (endIndex > startIndex))
                        {
                            textToProcess = htmlText.Substring(startIndex, endIndex - startIndex).Replace("<body>", "").Replace("</body>", "");
                            //  bodyText = htmlText;
                        }
                    }
                    break;
                case (ImageEngine.HTMLImageLocation.HTMLAll):
                    break;
                default:
                    break;
            }
        }
        return textToProcess;
    }

    /// <summary>
    /// Get the Image Extension
    /// </summary>
    /// <param name="imgSrc">The src. of the Image</param>
    /// <returns>The extension of the Image (Includes the '.'; Example Output: ".png", ".jpeg")</returns>
    private string GetImageExtension(string imgSrc)
    {        
        string toReturn = string.Empty;

        if (!string.IsNullOrEmpty(imgSrc))
        {
            imgSrc = imgSrc.ToLower();

            if (imgSrc.EndsWith(".jpg"))
            {
                toReturn = ".jpg";
            }
            if (imgSrc.EndsWith(".jpeg"))
            {
                toReturn = ".jpeg";
            }
            if (imgSrc.EndsWith(".png"))
            {
                toReturn = ".png";
            }
            if (imgSrc.EndsWith(".gif"))
            {
                toReturn = ".gif";
            }
        }
        return toReturn;
    }


}


public class ByteArraySorter : IComparer<byte[]>
{    

    public ByteArraySorter()
    {
        
    }

    #region IComparer Members

    int IComparer<byte[]>.Compare(byte[] x, byte[] y)
    {
        if (x == y) return 0;
        if (x == null) return 1;
        else if (y == null) return -1;

        if (x.Length < y.Length)
            return 1;
        return -1;                
    }

    #endregion IComparer Members
}




public class BitmapSorter : IComparer<Bitmap>
{

    public BitmapSorter()
    {

    }

    #region IComparer Members

    int IComparer<Bitmap>.Compare(Bitmap x, Bitmap y)
    {
        if (x == y) return 0;
        if (x == null) return 1;
        else if (y == null) return -1;

        if (x.Height < y.Height && x.Width < y.Width)
        //  if (x.PhysicalDimension.Height < y.PhysicalDimension.Height && x.PhysicalDimension.Width < y.PhysicalDimension.Width)
        {
            return 1;
        }
        return -1;
    }

    #endregion IComparer Members
}
