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
using System.Collections;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// Summary description for ImageEngine
/// </summary>
public class ImageEngine
{
    private static volatile ImageEngine instance;
    private static object syncRoot = new Object();

    bool _isIconsOn = Convert.ToInt32(ConfigurationManager.AppSettings["isIconsOn"]) != 0 ? true : false;
    Hashtable _iconsHashTable = (Hashtable)ConfigurationManager.GetSection("IconsTable");
    Hashtable _staticIconsHashTable = (Hashtable)ConfigurationManager.GetSection("StaticIconsTable");

    //  string _iconsPath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["iconsPath"];    
    string _iconsPath = ConfigurationManager.AppSettings["iconsPath"];
    string _iconsExt = ConfigurationManager.AppSettings["iconsExt"];

    bool _isItemImageRetrievalOn = Convert.ToInt32(ConfigurationManager.AppSettings["isItemImageRetrievalOn"]) != 0 ? true : false;    
    int _itemImageRetrievalCount = Convert.ToInt32(ConfigurationManager.AppSettings["ItemImageRetrievalCount"]);

    bool _isItemImageStorageOn = Convert.ToInt32(ConfigurationManager.AppSettings["isItemImageStorageOn"]) != 0 ? true : false;
    int _itemImageStorageCount = Convert.ToInt32(ConfigurationManager.AppSettings["ItemImageStorageCount"]);
        
    string _itemImageStorePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["ItemImageStorePath"];

    string _itemImageValidFormats = ConfigurationManager.AppSettings["ItemImageValidFormats"];
    List<string> _itemImageFormat;

    // If the retrieved image contains a word which is also present in the ImageWordsToBeIgnored List, Then ignore the image.
    List<string> _imageWordsToBeIgnored;


    string _itemThumbNailStorePath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["ItemThumbNailStorePath"];
    bool _isItemThumbNailStorageOn = Convert.ToInt32(ConfigurationManager.AppSettings["isItemThumbNailStorageOn"]) != 0 ? true : false;    


    /// <summary>
    /// True: Display Icons; False: Don't Display Icons;
    /// </summary>
    public bool IsIconsOn
    {
        get
        {
            return _isIconsOn;
        }
    }

    public Hashtable IconsHashTable
    {
        get
        {
            return _iconsHashTable;
        }
    }

    public Hashtable StaticIconsHashTable
    {
        get
        {
            return _staticIconsHashTable;
        }
    }
        
    /// <summary>
    /// True: Display Item Images; False: Don't Display Item Images;
    /// </summary>
    public bool IsItemImageRetrievalOn
    {
        get
        {
            return _isItemImageRetrievalOn;
        }
    }

    /// <summary>
    /// Number of Item Images to be displayed.
    /// </summary>
    public int ItemImageRetrievalCount
    {
        get
        {
            return _itemImageRetrievalCount;
        }
    }




    /// <summary>
    /// True: Store Item Images; False: Don't Store Item Images;
    /// </summary>
    public bool IsItemImageStorageOn
    {
        get
        {
            return _isItemImageStorageOn;
        }
    }

    /// <summary>
    /// Number of Item Images to be Stored.
    /// </summary>
    public int ItemImageStorageCount
    {
        get
        {
            return _itemImageStorageCount;
        }
    }

    /// <summary>
    /// Path to the Item Image Store
    /// </summary>
    public string ItemImageStorePath
    {
        get
        {
            return _itemImageStorePath;
        }
    }

    /// <summary>
    /// If the retrieved image contains a word which is also present in the ImageWordsToBeIgnored List, Then ignore the image.
    /// </summary>
    public List<string> ImageWordsToBeIgnored
    {
        get
        {
            return _imageWordsToBeIgnored;
        }
    }

    public List<string> ItemImageValidFormats
    {
        get
        {
            return _itemImageFormat;
        }
    }
      
    /// <summary>
    /// Path to the Item ThumbNail Store
    /// </summary>
    public string ItemThumbNailStorePath
    {
        get
        {
            return _itemThumbNailStorePath;
        }
    }

    /// <summary>
    /// True: Store Item ThumbNails; False: Don't Store Item Images;
    /// </summary>
    public bool IsItemThumbNailStorageOn
    {
        get
        {
            return _isItemThumbNailStorageOn;
        }
    }



	public ImageEngine()
	{
        /*
         _imageWordsToBeIgnored = new List<string>();
            _imageWordsToBeIgnored.Add("rss");
            _imageWordsToBeIgnored.Add("feed");
            _imageWordsToBeIgnored.Add("/ads/");
            _imageWordsToBeIgnored.Add("/ad/");
            _imageWordsToBeIgnored.Add("error");
            _imageWordsToBeIgnored.Add("exception");
            _imageWordsToBeIgnored.Add("ad.doubleclick.net");
            _imageWordsToBeIgnored.Add("icon");
            _imageWordsToBeIgnored.Add("quantserve.com");
            _imageWordsToBeIgnored.Add("statcounter.com");
        */

        //  General general = General.Instance;
        


            _itemImageFormat = new List<string>();
            string[] splitter = { "," };
            string[] _imageFormats = _itemImageValidFormats.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            if (_imageFormats != null && _imageFormats.Length > 0)
            {
                for (int i = 0; i < _imageFormats.Length; i++)
                {
                    _itemImageFormat.Add(_imageFormats[i].Trim().ToLower());
                }
            }
            else
            {
                _itemImageFormat.Add("jpg");
                _itemImageFormat.Add("jpeg");
                _itemImageFormat.Add("png");
                _itemImageFormat.Add("gif");
            }            

	}

    /// <summary>
    /// Initialize a new instance of the ImageEngine class, If required.
    /// </summary>
    public static ImageEngine Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ImageEngine();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Load the Image for the given Tag.
    /// </summary>
    /// <param name="category">The input Tag.</param>
    /// <returns>The path to the image.</returns>
    public string LoadIconLocation(string tag)
    {
        string toReturn = string.Empty;
        if (_iconsHashTable.ContainsKey(tag))
        {
            string iconFile = Convert.ToString(_iconsHashTable[tag]) + _iconsExt;
            toReturn = _iconsPath + iconFile;           
        }
        return toReturn;
    }


    /// <summary>
    /// Load the Static Icon Image for the given iconName.
    /// </summary>
    /// <param name="iconName">The input iconName.</param>
    /// <returns>The path to the Icon.</returns>
    public string LoadStaticIconLocation(string iconName)
    {
        string toReturn = string.Empty;
        if (_staticIconsHashTable.ContainsKey(iconName))
        {
            string iconFile = Convert.ToString(_staticIconsHashTable[iconName]) + _iconsExt;
            toReturn = _iconsPath + iconFile;
        }
        return toReturn;
    }


    /// <summary>
    /// Load Icons from Icon Sprite (Uses sprite.css) for loading Icons. This method is quicker as compared to using the LoadIconLocation() Method of ImageEngine.cs.
    /// </summary>
    /// <param name="tag">The input Category.</param>
    /// <returns>An HTML Span containing the Icon.</returns>
    public string LoadSpriteIcon(string tag)
    {
        //  Using Sprites
        string iconSpan = "<span class='ss_sprite ss_" + _iconsHashTable[tag] + " ' > </span>";
        return iconSpan;        
    }

    /// <summary>
    /// Load Icons from Icon Sprite (Uses sprite.css) for loading Icons. This method is quicker as compared to using the LoadStaticIconLocation() Method of ImageEngine.cs.
    /// </summary>
    /// <param name="tag">The input tag.</param>
    /// <returns>An HTML Span containing the Static Icon.</returns>
    public string LoadSpriteStaticIcon(string tag)
    {
        //  Using Sprites
        string iconSpan = "<span class='ss_sprite ss_" + _staticIconsHashTable[tag] + " ' > </span>";
        return iconSpan;
    }
    



    ///
    /// Creates a resized bitmap from an existing image on disk.
    /// Call Dispose on the returned Bitmap object
    ///
    /// Bitmap or null
    public Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
    {
        System.Drawing.Bitmap bmpOut = null;
        try
        {
            Bitmap loBMP = new Bitmap(lcFilename);
            ImageFormat loFormat = loBMP.RawFormat;
            decimal lnRatio;
            int lnNewWidth = 0;
            int lnNewHeight = 0;

            //  The image has to be atleast somewhat big (Done so as to avoid icons.)
            if (loBMP.PhysicalDimension.Height < 200 && loBMP.PhysicalDimension.Width < 200)
                return null;

            if (loBMP.Height < 150 || loBMP.Width < 150)
            {
                if ((loBMP.Height < loBMP.Width / 3) || (loBMP.Width < loBMP.Height / 3))
                {
                    return null;
                }
            }

            if ((loBMP.Height < loBMP.Width / 2) || (loBMP.Width < loBMP.Height / 2))
            {
                return null;
            }

            //  If the image is smaller than a thumbnail just return it                        
            //if (loBMP.Width < 100 && loBMP.Height < 100)
            //    return loBMP;

            if (loBMP.Width.Equals(loBMP.Height))
                return null;


            lnNewWidth = lnWidth;
            lnNewHeight = lnHeight;

            // System.Drawing.Image imgOut = loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight, null,IntPtr.Zero);

            // This code creates cleaner (though bigger) thumbnails and properly and handles GIF files better by generating a white background for transparent images (as opposed to black)
            
            bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
            Graphics g = Graphics.FromImage(bmpOut);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
            g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
            loBMP.Dispose();
        }
        catch (Exception ex)
        {
            return null;
        }
        return bmpOut;
    }



    ///
    /// Creates a resized bitmap from an existing image on disk.
    /// Call Dispose on the returned Bitmap object
    ///
    /// Bitmap or null
    public Bitmap CreateThumbnail(byte[] byteArrayIn, int lnWidth, int lnHeight)
    {
        System.Drawing.Bitmap bmpOut = null;

        try
        {
            //  Only store the image if its size is greater than 1KB.
            //  if (imageData.Length > 1024 && imageData.Length < 15000)    
            if (byteArrayIn != null && byteArrayIn.Length > 2048)
            {

                //  Bitmap loBMP = (Bitmap)ByteArrayToImage(byteArrayIn);                              
                
                //  using (MemoryStream ms = new MemoryStream(byteArrayIn))
                {
                    MemoryStream ms = new MemoryStream(byteArrayIn);
                    Bitmap loBMP = (Bitmap)System.Drawing.Image.FromStream(ms);
                    
                    if (loBMP != null)
                    {
                        ImageFormat loFormat = loBMP.RawFormat;
                        decimal lnRatio;
                        
                        int lnNewWidth = 0;
                        int lnNewHeight = 0;

                        //  The image has to be atleast somewhat big (Done so as to avoid icons.)
                        if (loBMP.PhysicalDimension.Height < 200 && loBMP.PhysicalDimension.Width < 200)
                            return null;

                        if (loBMP.Height < 150 || loBMP.Width < 150)
                        {
                            if ((loBMP.Height < loBMP.Width / 3) || (loBMP.Width < loBMP.Height / 3))
                            {
                                return null;
                            }
                        }

                        if ((loBMP.Height < loBMP.Width / 2) || (loBMP.Width < loBMP.Height / 2))
                        {
                            return null;
                        }

                        //  If the image is smaller than a thumbnail just return it                        
                        //if (loBMP.Width < 100 && loBMP.Height < 100)
                        //    return loBMP;

                        if (loBMP.Width.Equals(loBMP.Height))
                            return null;
                        
                        lnNewWidth = lnWidth;
                        lnNewHeight = lnHeight;

                        ////  Don't think I need this kind of scaling, I just need an exact square image (100 X 100)
                        //if (loBMP.Width > loBMP.Height)
                        //{
                        //    lnRatio = (decimal)lnWidth / loBMP.Width;
                        //    lnNewWidth = lnWidth;
                        //    decimal lnTemp = loBMP.Height * lnRatio;
                        //    lnNewHeight = (int)lnTemp;
                        //}
                        //else
                        //{
                        //    lnRatio = (decimal)lnHeight / loBMP.Height;
                        //    lnNewHeight = lnHeight;
                        //    decimal lnTemp = loBMP.Width * lnRatio;
                        //    lnNewWidth = (int)lnTemp;
                        //}

                        // System.Drawing.Image imgOut = loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight, null,IntPtr.Zero);

                        // This code creates cleaner (though bigger) thumbnails and properly and handles GIF files better by generating a white background for transparent images (as opposed to black)

                        bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                        Graphics g = Graphics.FromImage(bmpOut);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                        g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
                        
                        g.Dispose();
                        loBMP.Dispose();
                    }
                    else
                    {
                        bmpOut = null;
                    }
                }
            }
            else
            {
                bmpOut = null;
            }
        }
        catch (Exception ex)
        {
            bmpOut = null;
        }
        return bmpOut;
    }



    ///
    /// Creates a Bitmap from an image.
    /// Call Dispose on the returned Bitmap object
    ///
    /// Bitmap or null
    public Bitmap CreateThumbnail(byte[] byteArrayIn)
    {
        System.Drawing.Bitmap bmpOut = null;

        try
        {
            //  Only store the image if its size is greater than 1KB.
            //  if (imageData.Length > 1024 && imageData.Length < 15000)    
            if (byteArrayIn != null && byteArrayIn.Length > 2048)
            {

                //  Bitmap loBMP = (Bitmap)ByteArrayToImage(byteArrayIn);                              

                //  using (MemoryStream ms = new MemoryStream(byteArrayIn))
                {
                    MemoryStream ms = new MemoryStream(byteArrayIn);
                    Bitmap loBMP = (Bitmap)System.Drawing.Image.FromStream(ms);

                    if (loBMP != null)
                    {
                        ImageFormat loFormat = loBMP.RawFormat;
                                                
                        //  The image has to be atleast somewhat big (Done so as to avoid icons.)
                        if (loBMP.PhysicalDimension.Height < 200 && loBMP.PhysicalDimension.Width < 200)
                            return null;

                        if (loBMP.Height < 150 || loBMP.Width < 150)
                        {
                            if ((loBMP.Height < loBMP.Width / 3) || (loBMP.Width < loBMP.Height / 3))
                            {
                                return null;
                            }
                        }

                        if ((loBMP.Height < loBMP.Width / 2) || (loBMP.Width < loBMP.Height / 2))
                        {
                            return null;
                        }

                        //  If the image is smaller than a thumbnail just return it                        
                        //if (loBMP.Width < 100 && loBMP.Height < 100)
                        //    return loBMP;

                        if (loBMP.Width.Equals(loBMP.Height))
                            return null;
                        

                        
                        ////  Don't think I need this kind of scaling, I just need an exact square image (100 X 100)
                        //if (loBMP.Width > loBMP.Height)
                        //{
                        //    lnRatio = (decimal)lnWidth / loBMP.Width;
                        //    lnNewWidth = lnWidth;
                        //    decimal lnTemp = loBMP.Height * lnRatio;
                        //    lnNewHeight = (int)lnTemp;
                        //}
                        //else
                        //{
                        //    lnRatio = (decimal)lnHeight / loBMP.Height;
                        //    lnNewHeight = lnHeight;
                        //    decimal lnTemp = loBMP.Width * lnRatio;
                        //    lnNewWidth = (int)lnTemp;
                        //}

                        // System.Drawing.Image imgOut = loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight, null,IntPtr.Zero);

                        // This code creates cleaner (though bigger) thumbnails and properly and handles GIF files better by generating a white background for transparent images (as opposed to black)

                        bmpOut = new Bitmap(loBMP.Width, loBMP.Height);
                        Graphics g = Graphics.FromImage(bmpOut);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(Brushes.White, 0, 0, loBMP.Width, loBMP.Height);
                        g.DrawImage(loBMP, 0, 0, loBMP.Width, loBMP.Height);

                        g.Dispose();
                        loBMP.Dispose();
                    }
                    else
                    {
                        bmpOut = null;
                    }
                }
            }
            else
            {
                bmpOut = null;
            }
        }
        catch (Exception ex)
        {
            bmpOut = null;
        }
        return bmpOut;
    }



    public System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
    {
        System.Drawing.Image returnImage = null;
        using (MemoryStream ms = new MemoryStream(byteArrayIn))
        {
            returnImage = System.Drawing.Image.FromStream(ms);            
        }
        return returnImage;
    }



    public Bitmap ScaleBitmap(Bitmap Bitmap, int scaleWidth, int scaleHeight)
    {
        Bitmap scaledBitmap;

        try
        {
            //  int scaleWidth = (int)Math.Max(Bitmap.Width * ScaleFactorX, 1.0f);
            //  int scaleHeight = (int)Math.Max(Bitmap.Height * ScaleFactorY, 1.0f);

            scaledBitmap = new Bitmap(scaleWidth, scaleHeight);

            // Scale the bitmap in high quality mode.
            using (Graphics gr = Graphics.FromImage(scaledBitmap))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                gr.DrawImage(Bitmap, new Rectangle(0, 0, scaleWidth, scaleHeight), new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);
            }

            // Copy original Bitmap's EXIF tags to new bitmap.
            foreach (PropertyItem propertyItem in Bitmap.PropertyItems)
            {
                scaledBitmap.SetPropertyItem(propertyItem);
            }

            
        }
        catch (Exception ex)
        {
            scaledBitmap = null;
        }
        return scaledBitmap;
    }




    /// <summary>
    /// Look for Image inside the web-page in the following locations.
    /// </summary>
    public enum HTMLImageLocation
    {
        //  HTMLBodyTitle = "First look into the Body. If n Good Images Found, Don't look into the Title, Else look into the Title.
        HTMLNone = 0,
        HTMLTitle = 1,
        HTMLBody = 2,
        HTMLBodyTitle = 4,
        HTMLAll = 8
    }

}



