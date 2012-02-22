//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

//  namespace CaptchaImage
//  {
	public partial class JpegImage : System.Web.UI.Page
	{
        General general;

		private void Page_Load(object sender, System.EventArgs e)
		{
            general = General.Instance;

			// Create a CAPTCHA image using the text stored in the Session object.
			CaptchaImage ci = new CaptchaImage(HttpContext.Current.Session["CaptchaImageText"].ToString(), 200, 50, "Century Schoolbook");
            //  CaptchaImage ci = new CaptchaImage(general.CaptchaImageText, 200, 50, "Century Schoolbook");

			// Change the response headers to output a JPEG image.
			Response.Clear();
			Response.ContentType = "image/jpeg";

			// Write the image to the response stream in JPEG format.
			ci.Image.Save(Response.OutputStream, ImageFormat.Jpeg);

			// Dispose of the CAPTCHA image object.
			ci.Dispose();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
//  }
