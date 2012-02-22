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


/// <summary>
/// Summary description for Links
/// </summary>
public class Links
{
    private static volatile Links instance;
    private static object syncRoot = new Object();

    string _loginLink = ConfigurationManager.AppSettings["loginLink"];
    string _registerLink = ConfigurationManager.AppSettings["registerLink"];
    string _forgotPasswordLink = ConfigurationManager.AppSettings["forgotPasswordLink"];
    string _resetPasswordLink = ConfigurationManager.AppSettings["resetPasswordLink"];
    
    string _frontPageLink = ConfigurationManager.AppSettings["frontPageLink"];
    string _newestPageLink = ConfigurationManager.AppSettings["newestPageLink"];
    string _savedPageLink = ConfigurationManager.AppSettings["savedPageLink"];
    string _ratedPageLink = ConfigurationManager.AppSettings["ratedPageLink"];
    
    
    string _categoryPageLink = ConfigurationManager.AppSettings["categoryPageLink"];
    

    string _userDetailsPageLink = ConfigurationManager.AppSettings["userDetailsPageLink"];
    string _itemDetailsPageLink = ConfigurationManager.AppSettings["itemDetailsPageLink"];

    string _submitPageLink = ConfigurationManager.AppSettings["SubmitPageLink"];
    

    string _logoutPageLink = ConfigurationManager.AppSettings["logoutPageLink"];
    string _sessionExpiredPageLink = ConfigurationManager.AppSettings["sessionExpiredPageLink"];
    string _errorPageLink = ConfigurationManager.AppSettings["errorPageLink"];

    string _aboutPageLink = ConfigurationManager.AppSettings["aboutPageLink"];
    string _blogPageLink = ConfigurationManager.AppSettings["blogPageLink"];

    string _contactUsPageLink = ConfigurationManager.AppSettings["contactUsPageLink"];
    string _privacyPolicyPageLink = ConfigurationManager.AppSettings["privacyPolicyPageLink"];
    string _feedbackPageLink = ConfigurationManager.AppSettings["feedbackPageLink"];

    string _rssPageLink = ConfigurationManager.AppSettings["rssPageLink"];
    string _rssFeedLink = ConfigurationManager.AppSettings["rssFeedLink"];
    
    string _domainLink = ConfigurationManager.AppSettings["domainLink"];

    string _sendItemMailPageLink = ConfigurationManager.AppSettings["sendItemMailPageLink"];

    string _searchPageLink = ConfigurationManager.AppSettings["searchPageLink"];

    string _mynewsPageLink = ConfigurationManager.AppSettings["mynewsPageLink"];
    
    string _submittedPageLink = ConfigurationManager.AppSettings["submittedPageLink"];
    string _commentedPageLink = ConfigurationManager.AppSettings["commentedPageLink"];

    string _faqPageLink = ConfigurationManager.AppSettings["faqPageLink"];

    string _rPageLink = ConfigurationManager.AppSettings["rPageLink"];    
    string _savePageLink = ConfigurationManager.AppSettings["savePageLink"];
    
    string _autoTagPageLink = ConfigurationManager.AppSettings["autoTagPageLink"];

    string _editCommentPageLink = ConfigurationManager.AppSettings["editCommentPageLink"];


    public Links()
    {
        
    }

    /// <summary>
    /// Initialize a new instance of the Links class, If required.
    /// </summary>
    public static Links Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new Links();
                }
            }
            return instance;
        }
    }

    public string LoginLink
    {
        get { return _loginLink; }
    }

    public string RegisterLink
    {
        get { return _registerLink; }
    }

    public string ForgotPasswordLink
    {
        get { return _forgotPasswordLink; }
    }


    public string ResetPasswordLink
    {
        get { return _resetPasswordLink; }
    }
    
    
    
    public string FrontPageLink
    {
        get { return _frontPageLink; }
    }

    public string NewestPageLink
    {
        get { return _newestPageLink; }
    }
        
    public string SavedPageLink
    {
        get { return _savedPageLink; }
    }

    public string RatedPageLink
    {
        get { return _ratedPageLink; }
    }

    

    public string CategoryPageLink
    {
        get { return _categoryPageLink; }
    }
    

    public string UserDetailsPageLink
    {
        get { return _userDetailsPageLink; }
    }

    public string ItemDetailsPageLink
    {
        get { return _itemDetailsPageLink; }
    }

    public string SubmitPageLink
    {
        get { return _submitPageLink; }
    }

    
    
    public string LogoutPageLink
    {
        get { return _logoutPageLink; }
    }
    public string SessionExpiredPageLink
    {
        get { return _sessionExpiredPageLink; }
    }
    public string ErrorPageLink
    {
        get { return _errorPageLink; }
    }

    public string AboutPageLink
    {
        get { return _aboutPageLink; }
    }

    public string BlogPageLink
    {
        get { return _blogPageLink; }
    }

    public string ContactUsPageLink
    {
        get { return _contactUsPageLink; }
    }
    public string PrivacyPolicyPageLink
    {
        get { return _privacyPolicyPageLink; }
    }
    public string FeedbackPageLink
    {
        get { return _feedbackPageLink; }
    }

    public string RssPageLink
    {
        get { return _rssPageLink; }
    }

    public string RssFeedLink
    {
        get { return _rssFeedLink; }
    }
        
    public string DomainLink
    {
        get { return _domainLink; }
    }

    public string SendItemMailPageLink
    {
        get { return _sendItemMailPageLink; }
    }

    public string SearchPageLink
    {
        get { return _searchPageLink; }
    }

    public string MyNewsPageLink
    {
        get { return _mynewsPageLink; }
    }

    public string SubmittedPageLink
    {
        get { return _submittedPageLink; }
    }

    public string CommentedPageLink
    {
        get { return _commentedPageLink; }
    }

    public string FAQPageLink
    {
        get { return _faqPageLink; }
    }
    
    public string RPageLink
    {
        get { return _rPageLink; }
    }

    public string SavePageLink
    {
        get { return _savePageLink; }
    }

    public string AutoTagPageLink
    {
        get { return _autoTagPageLink; }
    }

    public string EditCommentPageLink
    {
        get { return _editCommentPageLink; } 
    }
    


}
