//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

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

public partial class Feedback : System.Web.UI.Page
{
    General general;
    Logger log;
    GUIVariables gui;

    string getputsMail = ConfigurationManager.AppSettings["getputsMail"];

    protected void Page_Load(object sender, EventArgs e)
    {
        general = General.Instance;
        log = Logger.Instance;
        gui = GUIVariables.Instance;

        MessageLabel.Text = "Your Feedback is so very important that we dedicated an entire page just for getting your feedback!";

        //Uncomment to Test If File Read/Write is working Fine.
        //string logPath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["logPath"];
        //string feedbackPath = HttpRuntime.AppDomainAppPath + ConfigurationManager.AppSettings["feedbackPath"];
        //OutputLabel.Text = "Log: " + logPath + gui.LineBreak + "Feedback: " + feedbackPath;

        if (!IsPostBack)
        {
            HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        }
    }

    protected void SubmitFeedbackButton_Click(object sender, EventArgs e)
    {
        string name = NameTB.Text.Trim();
        string eMail = EMailTB.Text.Trim();
        string topic = TopicDDL.SelectedItem.Value;
        string feedback = FeedbackTB.Text.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(eMail) || string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(feedback))
        {
            OutputLabel.Text = gui.RedFontStart + "Please fill out all the fields." + gui.RedFontEnd;
        }
        else
        {
            //  if (Page.IsValid)
            if(IsCaptchaValid())
            {
                if (general.IsValidEMail(eMail))
                {
                    log.LogFeedback(name, eMail, topic, feedback);
                    OutputLabel.Text = gui.GreenFontStart + "You feedback has been submitted. Thank You for helping us improve getputs." + gui.GreenFontEnd;

                    NameTB.Text = string.Empty;
                    EMailTB.Text = string.Empty;
                    FeedbackTB.Text = string.Empty;
                }
            }
            else    //  Invalid Captcha was entered.
            {
                OutputLabel.Text = gui.RedFontStart + "Invalid Captcha. Please Try Again." + gui.RedFontEnd;
            }
        }
        
        HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
        CodeNumberTextBox.Text = string.Empty;
    }

    private bool IsCaptchaValid()
    {
        bool isValid = false;

        #region Captcha

        // On a postback, check the user input.
        if (HttpContext.Current.Session != null)
        {
            if (CodeNumberTextBox.Text == HttpContext.Current.Session["CaptchaImageText"].ToString())
            //  if(CodeNumberTextBox.Text == general.CaptchaImageText)
            {
                //  Display an informational message.                
                //  MessageLabel.Text = "Correct.";

                isValid = true;
            }
            else
            {
                isValid = false;

                //  Display an error message.                
                //  MessageLabel.Text = "Incorrect, Try Again.";

                // Clear the input and create a new random code.
                CodeNumberTextBox.Text = string.Empty;
                HttpContext.Current.Session["CaptchaImageText"] = general.GenerateRandomCode();
                //  general.CaptchaImageText = general.GenerateRandomCode();
            }
        }
        else
        {
            isValid = true;
        }

        #endregion Captcha

        return isValid;
    }


}
