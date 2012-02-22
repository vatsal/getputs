//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Drawing;
using System.Drawing.Imaging;

public partial class CreateThumbNail : System.Web.UI.Page
{
        private void Page_Load(object sender, System.EventArgs e)
        {
            string _image = Request.QueryString["Image"];
            if (_image == null)
            {
                this.ErrorResult();
                return;
            }
            
            string sSize = Request["Size"];
            int _size = 120;
            if (sSize != null)
            {
                _size = Int32.Parse(sSize);
            }

            string _path = Server.MapPath(Request.ApplicationPath) + "\\" + _image;
            Bitmap _bmp = CreateThumbnail(_path,_size,_size);

            if (_bmp == null)
            {
                this.ErrorResult();
                return;
            }

            string OutputFilename = null;
            OutputFilename = Request.QueryString["OutputFilename"];

            if (OutputFilename != null)            
            {
                if (this.User.Identity.Name == "") 
                {
                    // *** Custom error display here
                    _bmp.Dispose();
                    this.ErrorResult();
                }
                try
                {
                    _bmp.Save(OutputFilename);
                }
                catch(Exception ex)
                {
                    _bmp.Dispose();
                    this.ErrorResult();
                    return;
                }
        }

        // Put user code to initialize the page here

        Response.ContentType = "image/jpeg";
        _bmp.Save(Response.OutputStream,System.Drawing.Imaging.ImageFormat.Jpeg);
        _bmp.Dispose();
    }
    
    private void ErrorResult()
    {
        Response.Clear();
        Response.StatusCode = 404;
        Response.End();
    }
    
    ///
    /// Creates a resized bitmap from an existing image on disk.
    /// Call Dispose on the returned Bitmap object
    ///
    /// Bitmap or null
    public static Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
    {
        System.Drawing.Bitmap bmpOut = null;
        try
        {
            Bitmap loBMP = new Bitmap(lcFilename);
            ImageFormat loFormat = loBMP.RawFormat;
            decimal lnRatio;
            int lnNewWidth = 0;
            int lnNewHeight = 0;

            //*** If the image is smaller than a thumbnail just return it
            if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                return loBMP;

            if (loBMP.Width > loBMP.Height)
            {
                lnRatio = (decimal)lnWidth / loBMP.Width;
                lnNewWidth = lnWidth;
                decimal lnTemp = loBMP.Height * lnRatio;
                lnNewHeight = (int)lnTemp;
            }
            else
            {
                lnRatio = (decimal)lnHeight / loBMP.Height;
                lnNewHeight = lnHeight;
                decimal lnTemp = loBMP.Width * lnRatio;
                lnNewWidth = (int)lnTemp;
            }

            // System.Drawing.Image imgOut =
            //      loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,
            //                              null,IntPtr.Zero);

            // *** This code creates cleaner (though bigger) thumbnails and properly
            // *** and handles GIF files better by generating a white background for
            // *** transparent images (as opposed to black)
            bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
            Graphics g = Graphics.FromImage(bmpOut);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
            g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);
            loBMP.Dispose();
        }
        catch
        {
            return null;
        }
        return bmpOut;
    }

}