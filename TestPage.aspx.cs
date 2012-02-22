//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

//  TestPage.aspx - A playground for testing new stuff or for testing small features before including them into getputs.com main code.

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Drawing;


public partial class TestPage : System.Web.UI.Page
{
    DBOperations dbOps;
    Links links;
    General general;
    GUIVariables gui;
    ProcessingEngine engine;
    Categories categories;
    ImageEngine imageEngine;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            OutputTB.Text = string.Empty;

            dbOps = DBOperations.Instance;
            links = Links.Instance;
            general = General.Instance;
            gui = GUIVariables.Instance;
            engine = ProcessingEngine.Instance;
            categories = Categories.Instance;
            imageEngine = ImageEngine.Instance;
        }
        catch (Exception ex)
        {
            OutputTB.Text = "Error in PageLoad: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
        }

    }
    protected void TestButton_Click(object sender, EventArgs e)
    {
        try
        {
            OutputTB.Text = string.Empty;

            string queryString = "SELECT Count(*) FROM getputs.item";
            int retInt = 111;
                
            retInt = dbOps.ExecuteScalar(queryString);

            OutputTB.Text = "Output: " + retInt.ToString();
            
        }
        catch (Exception ex)
        {
            OutputTB.Text = "Error in TestButtonClick: " + gui.LineBreak + ex.Message + gui.LineBreak + ex.StackTrace;
        }

    }

    protected void TestLinkClick()
    {
        Response.Redirect(links.ContactUsPageLink, false);
    }
    protected void TestLinkButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(links.ContactUsPageLink, false);
    }
    protected void TestHTMLLinkRegExpLB_Click(object sender, EventArgs e)
    {
        OutputLabel.Text = "";
        string input = RegExpInputTB.Text;

        if(string.IsNullOrEmpty(input))
        {
            OutputLabel.Text = "Empty Input";
        }
        else
        {
            OutputLabel.Text += "Original Input: " + input + "       " + Environment.NewLine + gui.LineBreak;

            //  string textLinkPattern = "(\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])";
            //  Regex linkRegex = new Regex(, RegexOptions.Compiled);
            //  input = linkRegex.Replace(input, "<a href='\0'>\0</a>");

            //input = Regex.Replace(input, "(\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$1'>$1</a>");
            //input = Regex.Replace(input, "(\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$2'>$2</a>");
            //input = Regex.Replace(input, "(\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$3'>$3</a>");            
            //input = Regex.Replace(input, "(\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$4'>$4</a>");

            //input = Regex.Replace(input, "\b((https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$1'>$1</a>");
            //input = Regex.Replace(input, "\b((https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$2'>$2</a>");
            //input = Regex.Replace(input, "\b((https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$3'>$3</a>");
            //input = Regex.Replace(input, "\b((https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])", "<a href='$4'>$4</a>");

            //  replaceAll("(?<!\")(http://[^\\s]+)(?!\")", "<a href=\"$1\">$1</a>")

            //  input = input.Replace("((?<!\")(http://[^\\s]+)(?!\"))", "<a href=\"$1\">$1</a>");

            //  input = input.Replace("(http://[^\\s]+)", "<a href='$1'>$1</a>");
            //  input = Regex.Replace(input, "(http)", "vatsal$1");


            //return Regex.Replace(messageInput, @"^([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*?)?", @"<href=""$0"">$0</a>");

            
            //  input = Regex.Replace(input, @"(?<!href=""http://)www?\.[\-\w\.\?\&\:/@]+", "<a href=\"http://$&\">$&</a>");


            //string pattern = @"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$";
            //string replacement = @"<a href='$1\'>$1</a>";
            //input = Regex.Replace(input, pattern, replacement);

            //  input =  Regex.Replace(input, @"(\bhttp://[^ ]+\b)", @"<a href=""$0"">$0</a>"); 
            
            
            //string strPattern = @"(?<url>(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&~=]*[^.])?)";
            //string strReplace = "<a href='${url}'>${url}</a>";
            //input = Regex.Replace(input, strPattern, strReplace);
            
            //strPattern = @"(?<url>http://(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&~=]*[^.])?)";
            //strReplace = "<a href='${url}'>${url}</a>";
            //input = Regex.Replace(input, strPattern, strReplace);

            //strPattern = @"(?<!http://)(?<url>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&~=]*[^.])?)";
            //strReplace = "<a href='http://${url}'>${url}</a>";
            //input = Regex.Replace(input, strPattern, strReplace);

                        
            //  Regex urlregex = new Regex(@"(http:\/\/([\w.]+\/?)\S*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //  input = urlregex.Replace(input, "<a href='$1'>$1</a>");

            //  string strPattern = @"(?<url>\b([(https?|ftp|file)://|(https?|ftp|file)://www.])?[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])";


            //string strPattern = @"(?<url>[http://|https://|ftp://|http://www.|https://www.|ftp://www.][-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|])";
            //string strReplace = "<a href='${url}'>${url}</a>";
            //input = Regex.Replace(input, strPattern, strReplace);


            //  string strPattern = @"(?:(?:http)://(?:(?:(?:(?:(?:(?:[a-zA-Z0-9][-a-zA-Z0-9]*)?[a-zA-Z0-9])[.])*(?:[a-zA-Z][-a-zA-Z0-9]*[a-zA-Z0-9]|[a-zA-Z])[.]?)|(?:[0-9]+[.][0-9]+[.][0-9]+[.][0-9]+)))(?::(?:(?:[0-9]*)))?(?:/(?:(?:(?:(?:(?:(?:[a-zA-Z0-9\-_.!~*'():@&amp;=+$,]+|(?:%[a-fA-F0-9][a-fA-F0-9]))*)(?:;(?:(?:[a-zA-Z0-9\-_.!~*'():@&amp;=+$,]+|(?:%[a-fA-F0-9][a-fA-F0-9]))*))*)(?:/(?:(?:(?:[a-zA-Z0-9\-_.!~*'():@&amp;=+$,]+|(?:%[a-fA-F0-9][a-fA-F0-9]))*)(?:;(?:(?:[a-zA-Z0-9\-_.!~*'():@&amp;=+$,]+|(?:%[a-fA-F0-9][a-fA-F0-9]))*))*))*))(?:[?](?:(?:(?:[;/?:@&amp;=+$,a-zA-Z0-9\-_.!~*'()]+|(?:%[a-fA-F0-9][a-fA-F0-9]))*)))?))?)";
            
            //  This was the best implementation.
            //string strPattern = @"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$";
            //Regex linkRegex = new Regex(strPattern, RegexOptions.Compiled);
            //bool isMatch = linkRegex.IsMatch(input);
            //OutputLabel.Text += "Changed Input: " + isMatch;


            //  string strPattern = @"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$";
            string strPattern = @"(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?";
            Regex linkRegex = new Regex(strPattern, RegexOptions.Compiled);
            MatchCollection linkMC = linkRegex.Matches(input);
            for (int i = 0; i < linkMC.Count; i++)
            {
                string originalLink = linkMC[i].Value.Trim();
                string link = originalLink;
                if (originalLink.StartsWith("http://") || originalLink.StartsWith("https://") || originalLink.StartsWith("ftp://") || originalLink.StartsWith("ftps://"))
                {
                    link = originalLink;
                }
                else if (originalLink.StartsWith("www."))
                {
                    link = "http://" + originalLink;
                }
                else
                {
                    link = "http://www." + originalLink;
                }

                input = input.Replace(originalLink, "<a href='" + link + "'>" + originalLink + "</a>");
            }            
            OutputLabel.Text += "Changed Input: " + input;
        }
        
    }
    
    
    protected void URLButton_Click(object sender, EventArgs e)
    {
        List<string> imageWordsToBeIgnored = new List<string>();
        imageWordsToBeIgnored.Add("rss");
        imageWordsToBeIgnored.Add("feed");
        imageWordsToBeIgnored.Add("/ads/");
        imageWordsToBeIgnored.Add("/ad/");
        imageWordsToBeIgnored.Add("error");
        imageWordsToBeIgnored.Add("exception");

        

        OutputURLLabel.Text = string.Empty;
        StringBuilder message = new StringBuilder();
        StringBuilder imageMessage = new StringBuilder();   //  Will contain the image sources of all the good images found on a page.

        string url = InputURLTB.Text.Trim();

        if (general.IsValidURL(url))
        {
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }


            using (WebClient wc = new WebClient())
            {
                byte[] data = wc.DownloadData(url);

                string htmlText = Encoding.ASCII.GetString(data);
                if (htmlText.Contains("<title>"))
                {
                    int startIndex = htmlText.IndexOf("<title>");
                    int endIndex = htmlText.IndexOf("</title>");

                    if (startIndex != -1 && endIndex != -1)
                    {
                        string title = htmlText.Substring(startIndex, endIndex - startIndex).Replace("<title>", "").Replace("</title>", "");
                        message.AppendLine("Start: " + startIndex.ToString() + " End: " + endIndex.ToString());
                        message.AppendLine(gui.LineBreak);
                        message.AppendLine("Title: " + title);
                    }
                    else
                    {
                        message.AppendLine("Index = -1");
                    }
                }
                                
                //Need to reference mshtml DLL from COM
                //mshtml.HTMLDocumentClass ms = new mshtml.HTMLDocumentClass();
                //string strHTML = Encoding.ASCII.GetString(data);
                //mshtml.IHTMLDocument2 objMyDoc = (mshtml.IHTMLDocument2)ms;
                //objMyDoc.write(strHTML);

                //string title = string.Empty;
                //if (!string.IsNullOrEmpty(objMyDoc.title))
                //{
                //    title = objMyDoc.title;
                //    message.AppendLine("Title: ");
                //    message.AppendLine(title);
                //}
                //else
                //{
                //    message.AppendLine("Title: ");
                //    message.AppendLine("No Title Detected.");
                //}





                //  //  You can then use the IHTMLDocument2 object to get lists of images, anchors, and other HTML collections that you're interested in. The following code continues the previous fragment and gets all the anchors in the document.
                //  mshtml.IHTMLElementCollection ec = (mshtml.IHTMLElementCollection)objMyDoc.links;

                //for (int i = 0; i < ec.length; i++)
                //{
                //    string strLink;
                //    mshtml.HTMLAnchorElementClass objAnchor;
                //    try
                //    {
                //        objAnchor = (mshtml.HTMLAnchorElementClass)ec.item(i, 0);
                //        strLink = objAnchor.href;
                //    }
                //    catch
                //    {
                //        continue;
                //    }

                //}

            }
        }
        else
        {
            message.AppendLine("Invalid URL");
        }
        OutputURLLabel.Text = message.ToString();
    }

    protected void IMGButton_Click(object sender, EventArgs e)
    {
        List<string> imageWordsToBeIgnored = new List<string>();
        imageWordsToBeIgnored.Add("rss");
        imageWordsToBeIgnored.Add("feed");
        imageWordsToBeIgnored.Add("/ads/");
        imageWordsToBeIgnored.Add("/ad/");
        imageWordsToBeIgnored.Add("error");
        imageWordsToBeIgnored.Add("exception");



        OutputURLLabel.Text = string.Empty;
        StringBuilder message = new StringBuilder();
        StringBuilder imageMessage = new StringBuilder();   //  Will contain the image sources of all the good images found on a page.

        


        imageMessage.AppendLine(gui.LineBreak);
        imageMessage.AppendLine("----------------- The selected images: -----------------");
        imageMessage.AppendLine(gui.LineBreak);

        string url = InputURLTB.Text.Trim();

        if (general.IsValidURL(url))
        {
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }


            using (WebClient wc = new WebClient())
            {
                byte[] data = wc.DownloadData(url);

                string htmlText = Encoding.ASCII.GetString(data);
                


                string bodyText = string.Empty;
                if (htmlText.Contains("</body>"))
                {
                    //  int startIndex = htmlText.IndexOf("<body>");
                    //  int endIndex = htmlText.IndexOf("</body>");

                    int startIndex = 0;
                    int endIndex = htmlText.Length - 1;


                    if (startIndex != -1 && endIndex != -1)
                    {
                        //  bodyText = htmlText.Substring(startIndex, endIndex - startIndex).Replace("<body>", "").Replace("</body>", "");

                        bodyText = htmlText;


                        //message.AppendLine("Start: " + startIndex.ToString() + " End: " + endIndex.ToString());
                        //message.AppendLine(gui.LineBreak);
                        //message.AppendLine("Body: " + bodyText);
                    }
                    else
                    {
                        message.AppendLine("Index = -1");
                    }
                }


                //  Parsing out Image Locations.
                message.AppendLine(gui.LineBreak);
                message.AppendLine("Image Src: ");
                message.AppendLine(gui.LineBreak);

                List<string> imageList = new List<string>();

                Regex imageRegex = new Regex(@"img\ssrc\s*=\s*\""*[^\"">]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                //  Regex imageRegex = new Regex(@"<img.+</img>", RegexOptions.IgnoreCase | RegexOptions.Compiled);



                //  Regex imageRegex = new Regex("<img .+ src[ ]*=[ ]*\"(.+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                MatchCollection matchCollection = imageRegex.Matches(bodyText);
                foreach (Match imageMatch in matchCollection)
                {
                    if (imageMatch.Captures[0].Value.EndsWith(".jpg") || imageMatch.Captures[0].Value.EndsWith(".png") || imageMatch.Captures[0].Value.EndsWith(".jpeg") || imageMatch.Captures[0].Value.EndsWith(".gif"))
                    {
                        //  string imgSrc = match.Captures[0].Value.ToLower().Replace("img src=\"", "");
                        //  Regex imageSrcRegex = new Regex("src=[\"|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        string imgSrc = imageMatch.Captures[0].Value.ToLower().Replace("img src=\"", "");

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
                        for (int x = 0; x < imageWordsToBeIgnored.Count; x++)
                        {
                            if (imgSrc.Contains(imageWordsToBeIgnored[x]))
                            {
                                isGood = false;
                            }
                        }

                        if (isGood && !imageList.Contains(imgSrc))
                        {
                            try
                            {
                                byte[] imageData = wc.DownloadData(imgSrc);
                                
                                message.AppendLine("IMGDATALENGTH: " + imageData.Length);
                                message.AppendLine(gui.LineBreak);
                                message.AppendLine("    IMGDATASRC: " + imgSrc);
                                message.AppendLine(gui.LineBreak);

                                if (imageList.Count < 5)
                                {
                                    if (imageData.Length > 1024 && imageData.Length < 15000)    //  Only store the image if its size is greater than 2KB.
                                    {
                                        imageList.Add(imgSrc);
                                        message.AppendLine(gui.LineBreak);

                                        /*
                                        imageMessage.AppendLine(gui.LineBreak);
                                        imageMessage.AppendLine("IMGDATALENGTH: " + imageData.Length);
                                        imageMessage.AppendLine(gui.LineBreak);
                                        imageMessage.AppendLine("    IMGDATASRC: " + imgSrc);
                                        imageMessage.AppendLine(gui.LineBreak);
                                        */
                                        imageMessage.AppendLine("<img src='" + imgSrc + "' height='100px' width='100px'>" + "</img>");

                                        //  imageMessage.AppendLine("<img src='" + imgSrc + "'>" + "</img>");
                                        




                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }

                    //message.AppendLine(match.Captures[0].Value);
                    //message.AppendLine(gui.LineBreak);

                }

                //message.AppendLine(gui.LineBreak);
                //if (matchCollection != null && matchCollection.Count > 0)
                //{
                //    string imgSrc = matchCollection[0].Captures[0].Value.Replace("img src=\"" , "");
                //    PageImage.Src = imgSrc;
                //}



                /*
                byte[] firstImageData = wc.DownloadData(imageList[0]);
                MemoryStream imageMS = new MemoryStream();
                imageMS.Write(firstImageData, 0, firstImageData.Length);

                Bitmap imageBitmap = new Bitmap(imageMS);
                System.Drawing.Image thumbImage = imageBitmap.GetThumbnailImage(100, 100, null, IntPtr.Zero);


                MemoryStream imageStream = new MemoryStream();
                thumbImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                
                // make byte array the same size as the image
                byte[] imageContent = new Byte[imageStream.Length];
                // rewind the memory stream
                imageStream.Position = 0;
                // load the byte array with the image
                imageStream.Read(imageContent, 0, (int)imageStream.Length);

                Response.ContentType = "image/jpeg";
                Response.BinaryWrite(imageContent);
                */
                
                


                message.AppendLine(gui.LineBreak);
                message.AppendLine(imageMessage.ToString());
                message.AppendLine(gui.LineBreak);
                
                if (imageList != null && imageList.Count > 0)
                {
                    PageImage.Src = imageList[0];
                }           

            }
        }
        else
        {
            message.AppendLine("Invalid URL");
        }

        message.AppendLine(gui.LineBreak);
        message.AppendLine(gui.LineBreak);
        message.AppendLine(gui.LineBreak);
        message.AppendLine("------------------ From the Method GetImages() ------------------");
        message.AppendLine(gui.LineBreak);
        message.AppendLine(GetImages(url));
        message.AppendLine(gui.LineBreak);
        




        OutputURLLabel.Text = message.ToString();
    }



    public string GetImages(string url)
    {
        StringBuilder imageSB = new StringBuilder();

        List<string> imageWordsToBeIgnored = new List<string>();
        imageWordsToBeIgnored.Add("rss");
        imageWordsToBeIgnored.Add("feed");
        imageWordsToBeIgnored.Add("/ads/");
        imageWordsToBeIgnored.Add("/ad/");
        imageWordsToBeIgnored.Add("error");
        imageWordsToBeIgnored.Add("exception");

        if (general.IsValidURL(url))
        {
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }

            using (WebClient wc = new WebClient())
            {
                byte[] data = wc.DownloadData(url);
                string htmlText = Encoding.ASCII.GetString(data);
                string bodyText = htmlText;

                //if (htmlText.Contains("</body>"))
                //{
                //    //  int startIndex = htmlText.IndexOf("<body>");
                //    //  int endIndex = htmlText.IndexOf("</body>");

                //    int startIndex = 0;
                //    int endIndex = htmlText.Length - 1;


                //    if (startIndex != -1 && endIndex != -1)
                //    {
                //        //  bodyText = htmlText.Substring(startIndex, endIndex - startIndex).Replace("<body>", "").Replace("</body>", "");
                //        bodyText = htmlText;                        
                //    }
                //    else
                //    {

                //    }
                //}

                List<string> imageList = new List<string>();
                Regex imageRegex = new Regex(@"img\ssrc\s*=\s*\""*[^\"">]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                //  Regex imageRegex = new Regex(@"<img.+</img>", RegexOptions.IgnoreCase | RegexOptions.Compiled);                
                //  Regex imageRegex = new Regex("<img .+ src[ ]*=[ ]*\"(.+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                MatchCollection matchCollection = imageRegex.Matches(bodyText);
                foreach (Match imageMatch in matchCollection)
                {
                    if (imageMatch.Captures[0].Value.EndsWith(".jpg") || imageMatch.Captures[0].Value.EndsWith(".png") || imageMatch.Captures[0].Value.EndsWith(".jpeg") || imageMatch.Captures[0].Value.EndsWith(".gif"))
                    {
                        //  string imgSrc = match.Captures[0].Value.ToLower().Replace("img src=\"", "");
                        //  Regex imageSrcRegex = new Regex("src=[\"|']", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        string imgSrc = imageMatch.Captures[0].Value.ToLower().Replace("img src=\"", "");

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
                        for (int x = 0; x < imageWordsToBeIgnored.Count; x++)
                        {
                            if (imgSrc.Contains(imageWordsToBeIgnored[x]))
                            {
                                isGood = false;
                            }
                        }

                        if (isGood && !imageList.Contains(imgSrc))
                        {
                            try
                            {
                                byte[] imageData = wc.DownloadData(imgSrc);

                                if (imageList.Count < 5)
                                {
                                    if (imageData.Length > 1024 && imageData.Length < 15000)    //  Only store the image if its size is greater than 2KB.
                                    {
                                        imageList.Add(imgSrc);
                                        imageSB.AppendLine("<img src='" + imgSrc + "' height='100px' width='100px'>" + "</img>");
                                    }
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
        else
        {
            imageSB.Append(string.Empty);
        }
        return imageSB.ToString();
    }


    protected void ImageStoreButton_Click(object sender, EventArgs e)
    {
         string message = string.Empty;
         string url = InputURLTB.Text.Trim();

         if (general.IsValidURL(url))
         {
             if (!url.StartsWith("http://"))
             {
                 url = "http://" + url;
             }
         }
         else
         {
             url = "http://content-www.cricinfo.com/ci/content/story/367277.html";
         }

         int iid = int.TryParse(InputIIDTB.Text.Trim(), out iid) ? iid : 710;
        
        try
        {
            ImageEngine imageEngine = ImageEngine.Instance;
            
            //  Try the algorithm on the input image.
            //  bool isImageStored = general.StoreImages(url, imageEngine.ItemImageRetrievalCount, iid, ImageEngine.HTMLImageLocation.HTMLBody);            
            //  message = isImageStored.ToString();

            //  Try the algorithm on the following images. Batch Testing.
            /*
            947	http://www.mypyramid.gov/
            950	http://www.ndtv.com/convergence/ndtv/uspolls2008/Election_Story.aspx?ID=NEWEN20080070797&type=topstory
            952	http://www.wired.com/cars/coolwheels/multimedia/2008/11/gallery_homemade_bicycle
            957	http://www.nytimes.com/2008/10/31/health/research/31anxiety.html?_r=1&oref=slogin
            962	http://www.financialexpress.com/news/global-crisis-affects-indian-economy-pm/380797/
            964	http://finance.yahoo.com/expert/article/moneyhappy/116572
            965	http://news.yahoo.com/s/ap/obama_grandmother
            974 http://economictimes.indiatimes.com/articleshow/3675523.cms
            978 http://www.junefabrics.com/index.php
            980	http://cricket.timesofindia.indiatimes.com/articleshow/3680351.cms
            982	http://www.lightreading.com/document.asp?doc_id=167402
            983	http://english.aljazeera.net/news/americas/2008/11/2008116929648438.html
            993 http://www.businesssheet.com/2008/11/world-s-4th-richest-man-loses-50-billion-in-five-months             
            1008 http://www-personal.umich.edu/~mejn/election/2008/ 
            
            Items that produce and image: 947, 950, 952, 962, 965, 978, 982, 
             
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                      
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                      
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                      
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                      
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                      
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                                  
            general.StoreImages("", imageEngine.ItemImageRetrievalCount, 9, ImageEngine.HTMLImageLocation.HTMLBody);                                      
            */

            general.StoreImages("http://www.mypyramid.gov/", imageEngine.ItemImageRetrievalCount, 947, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.ndtv.com/convergence/ndtv/uspolls2008/Election_Story.aspx?ID=NEWEN20080070797&type=topstory", imageEngine.ItemImageRetrievalCount, 950, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.wired.com/cars/coolwheels/multimedia/2008/11/gallery_homemade_bicycle", imageEngine.ItemImageRetrievalCount, 952, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.nytimes.com/2008/10/31/health/research/31anxiety.html?_r=1&oref=slogin", imageEngine.ItemImageRetrievalCount, 957, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.financialexpress.com/news/global-crisis-affects-indian-economy-pm/380797/", imageEngine.ItemImageRetrievalCount, 962, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://finance.yahoo.com/expert/article/moneyhappy/116572", imageEngine.ItemImageRetrievalCount, 964, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://news.yahoo.com/s/ap/obama_grandmother", imageEngine.ItemImageRetrievalCount, 965, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://economictimes.indiatimes.com/articleshow/3675523.cms", imageEngine.ItemImageRetrievalCount, 974, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.junefabrics.com/index.php", imageEngine.ItemImageRetrievalCount, 978, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://cricket.timesofindia.indiatimes.com/articleshow/3680351.cms", imageEngine.ItemImageRetrievalCount, 980, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.lightreading.com/document.asp?doc_id=167402", imageEngine.ItemImageRetrievalCount, 982, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://english.aljazeera.net/news/americas/2008/11/2008116929648438.html", imageEngine.ItemImageRetrievalCount, 983, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www.businesssheet.com/2008/11/world-s-4th-richest-man-loses-50-billion-in-five-months", imageEngine.ItemImageRetrievalCount, 993, ImageEngine.HTMLImageLocation.HTMLBody);
            general.StoreImages("http://www-personal.umich.edu/~mejn/election/2008/", imageEngine.ItemImageRetrievalCount, 1008, ImageEngine.HTMLImageLocation.HTMLBody);                                      
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        ImageStoreLabel.Text = message;
    }
    
    protected void StoreQuotesButton_Click(object sender, EventArgs e)
    {
        string fileName = "QuoteDB.txt";
        string fullFilePath = Path.Combine(HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"], fileName);
        gp.Files.QuoteDB.Instance.StoreQuote(fullFilePath, 5000);
    }
    protected void GetRandomQuoteButton_Click(object sender, EventArgs e)
    {
        gp.Files.FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"];
        gp.Files.QuoteDB.Instance.ReLoadList();
        ShowRandomQuoteLabel.Text = "Random Quote: " + gp.Files.QuoteDB.Instance.GetRandomQuote();
    }
    
    
    protected void StoreFactsButton_Click(object sender, EventArgs e)
    {
        string fileName = "FactDB.txt";
        string fullFilePath = Path.Combine(HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"], fileName);
        gp.Files.FactDB.Instance.StoreFact(fullFilePath, 2500);
    }
    protected void GetRandomFactButton_Click(object sender, EventArgs e)
    {
        gp.Files.FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"];
        gp.Files.FactDB.Instance.ReLoadList();
        ShowRandomFactLabel.Text = "Random Fact: " + gp.Files.FactDB.Instance.GetRandomFact();
    }
    protected void ReloadRandomTipButton_Click(object sender, EventArgs e)
    {
        gp.Files.FileLoader.FilePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["FilesPath"];
        gp.Files.TipsDB.Instance.ReLoadList();
        ShowRandomTipLabel.Text = "Random Tip: " + gp.Files.TipsDB.Instance.GetRandomTip();
    }
}