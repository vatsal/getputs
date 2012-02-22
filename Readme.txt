//  Owner: Vatsal Shah
//  Product: getputs.com
//  Copyright Notice: Copyright Protected. All Rights Reserved.

//	Contains a log of all the Programmatica Changes/Improvements/Additions performed on the getputs Code.

2008-05-21
Added E-Mailing/Sharing capability.

2008-07-08
Polished gp.Files
Merged gp.Files code with getputs
Administrator.aspx now has capability to reload lists
Scheduler now reloads lists every hour
Added Submitted.aspx
Added Commented.aspx
UserDetails.aspx now has links for Submissions, Comments, Saved
Added the RandomArticleLinkButton to getputs.master

2008-07-09

Added BrainJar C# Captcha (CaptchaImage.cs, JpegImage.cs, Submit.aspx, Submit.aspx.cs) Currently only used on Submit Page
FAQ.aspx added. Currently no content on that page.
Activated CustomErrors in Web.config, will see what happens.
ServerGetsFried.aspx set as the default page
Added sitedown.png image to ServerGetsFried.aspx 
Added checks for BlackLists inside Submit.aspx.cs (Checks for Bad Users, IPs, Sites)

Added R.aspx, still incomplete. Will require the following querystring items: [url, iid, uid]
Thinking of preparing a new class: ClickCounter.cs [IID, UID, IP, Date, Clicks] and the corresponding DB

2008-07-10

Added clickdata table to the getputs DB
Added clickdata.cs class
Added code to update a global dictionary of clickdata in the scheduler (See UpdateClickDataDB() in scheduler.cs)
Added an FAQ Page, currently blank.
R.aspx is still incomplete.
Need to work on improving the search functionality (Multiple word search, comments search)

2008-07-11

- For submit.aspx.cs the digit-captcha is reloaded regardless of whether the submit is successful or not.
- There is a generic GetIP(HttpWebRequest Request) method inside General.cs that will be used by all the web-pages.
- Changed the LinkItemClick, TextItemClick, CommentClick inside Default/Category/My News/ItemDetails Pages so as to use the General.UpdateClickDataDB(...) method.
	So now for a given IID, and a given UID/IP only 1 click is considered no matter how many times the UID/IP clicked on that IID.

2008-07-14

- Added PrivacyPolicy.aspx (Content and Page)
- Fixed the null value check inside DBOperations ExecuteScalar Method
- Changed all the getputs.Master bottom level links (About/Contact/Blog/Rss/PrivacyPolicy) from LinkButton to HyperLink
- Added 'Open Link in a New Tab' Icon to all items in the default page.
- Fixed the 'https' problem in submit.aspx.cs

2008-07-15

- Prepared the FAQ Page
- Added Tokenizer.cs
- Added Tags.cs
- SearchEngine.cs changed to user Tokenizer.cs and now supports looking up multiple words inside single query.
- Fixed the CategoryDiv invisibility when user logged in problem inside getputs.master.cs
- The open link in new tab now goes through R.aspx

- Need to add tagging capability

2008-07-16

- Added some more stuff to FAQ Page
- Tried Urlrewriting, but think its not required right now.
- Changed the (website label to a website link)(userlb to userhl)(categorylb to categoryhl) in default.aspx.cs
	Will monitor it for the next 2 days and then will change it for all other pages.
- Added startItemRegex to Hot and New Links in getputs.master.cs

2008-07-17

- Fixed a bug in registration.aspx (A sql insert query was not updated and was not allowing any user to register)
- Updated LoatItemTable for the following pages: Search/Saved/MyNews/ItemDetails/Category/Submitted

- To Do: Add tagging logic. 
- To Do: Add comments search inside search.aspx.cs

2008-07-18

- Fixed a bug in ProcessingEngine.LoadItemDB(Sort sortType, string UID) which was messing up submitted.aspx.cs
- Replaced all ReCaptcha with Captcha from JpegImage (Reigstration/SendItemMail/Feedback)
- Other formatting changes (Message Label's are now shown at the top instead of at the bottom of Forms)
- HttpWebRequest changed to WebRequest for all .aspx.cs and for general.IsValidURL()

2008-07-21

- Replaced all the LinkButton in NavigationTable in getputs.Master and getputs.Master.cs with HyperLink
- Added ProcessingEngine.ItemRowCount and updation code for the itemrowcount in Scheduler.cs
- Added Length Checks on the title of the submission. (2<=title word count<=40)
- Added Length Checks on the text of the submission. (5<=text word count)

2008-07-24

- Fixed the bug inside DBOperations ExecuteScalar(), "Object cannot be cast from DBNull to other types.", this happened for queries of the type: "SELECT MAX(CID) FROM comments WHERE IID = 387;"
- Added a tool-tip to the getputs logo.
- Feedback.aspx Form Fields are now cleared after the submission is successful.
- Added original article link to the commented.aspx Page.
- Reverted to R.aspx for click-counting.

2008-07-28

- Converted all Application LoadClasses() Instantiation that happened in Global.asax into Per Class Singleton instance.
- Updated Database Structure
- Code for Automated Tagging added.

2008-07-29

- Improved the AutoTagging Feature immensely.
- AutoTag.aspx Added
- CommentToken limit added (Min=2; Max=500)

2008-07-30

- Small but important bug fixes.
- Bad URL checking IsBadSite() in general.cs now trims http://, https://, www.
- A URL is now considered as a "NN" for tagging
- Words consisting of just symbols are now tagged as "SYM"
- Fixed a bad quote bug arising from GetTokens() Tagger.cs
- Added a try-catch block to UpdateClickDataDictionary() in ProcessingEngine.cs

2008-08-06

- ItemDetails, Saved, Search, Category, MyNews, Submitted, AutoTag .aspx Pages non contain only HyperLinks. All LinkButtons are removed.
- Tags with Length < 3 will not be considered.
- Administrator.aspx now has facility to change Tags for each Item.

2008-08-28

- Added getputs_A.css
- Added general.js
- Provided the User the option to change Theme on the fly (Using getputs_A.css and getputs_A.master)
- Added rating inside the DB
- Provide a Config Option to Turn the Rating On/Off

2008-08-29

- Added getputs_passion.css, getputs_serene.css, getputs_feminine.css
- seperator (" | ") now a part of guivariables.cs
- Changes to general.js, in particular related to switching the logo when the css file is switched.

- To Do: Add Rating Code.
- To Do: Add TagEdit.aspx (For allowing the submitter to edit tags for his submission)
- To Do: Change the code for the SpamButtonClick

2008-08-29

- Got Image Swapping to work for IE (By tweaking general.js)
- Changed the code for the SpamButton Click (Now an item is marked as spam if 3 different users mark the item as spam)
- Changed Tagger to remove ' and ` from Tags.

- To Do: Add Rating Code.
- To Do: Add TagEdit.aspx (For allowing the submitter to edit tags for his submission)

2008-10-27

- Image extraction from Submitted Pages Done.
- Image to ThumbNail conversion.
- ThumbNail retrieval for the Default.aspx Page Done.

2008-11-07

- Image Extraction Algorithm much more better (and stricter)
- Rating System almost completed.
- To Do: Implement Ranking based on Rating System.

2008-11-11

- Image Extraction, some stuff still needs to be fixed especially look at Item.IID = 1008 (http://www-personal.umich.edu/~mejn/election/2008/)
- Good progress with the Star-Rating System
- Ranking based on the Rating System has been implemented. Still need to test it thoroughly.

2008-11-14

- To Do (before ThanksGiving)
	Replace YUI with jquery entirely.
	Replace ASPX Button Controls (save/e-mail/spam) with HTML Entirely.
	Implement Advertisement.
	Store Clicks only as "Number of Views". Store it in ClickData Table only for Logged In Users.
	Generate Quotes Database and Random Quote Retrieval.
	Generate Tips Database and Random Tip Retrieval.

	If Time Permits:

		Strengthen the Rating System and Stress Test it.
		Keep Monitoring Image Extraction.
		Administrator.aspx needs to be strengthened significantly.
		Upload Pictures of the Team on the About Page (brief descriptions also, if possible)		
		Allow Users to Upload Images for their submissions.
		Allow Users to Tag their submissions.		
		Build an Address Book.
		OpenSocial Integration (or Login using Yahoo/Google/Microsoft/AOL ID's).
		A description of each category (Example: NRI - For the Non Resident Indians).
		
	Not Sure:
		User Images
		
2008-11-24

-	Implemented Advertisement (Google Adsense).
-	Currently storing clicks only as "Number of Views". Store it in ClickData Table only for Logged In Users.

2008-12-29

- Previous Database Server Name (Before 2008-12-29) : p50mysql127.secureserver.net
- New Database Server Name       (After 2008-12-29) : getputs.db.2678838.hostedresource.com
- Fixed the Invalid Captcha Error that occurred only on Production Server and not on Localhost.
- "Make getputs your Home Page" implemented. Works only for IE, does not work for Mozilla. So Browser Detection using Javascript implemented.

(1) OpenID Integration (So that people would no longer need to signup with us) Visit http://openid.net/ to learn more
(2) Allowing people to login via their Yahoo/Google/Hotmail/AOL/Facebook ID's
(3) Allowing people to make getputs their HomePage (Should be pretty easy)
(4) A fullfledged recommendation system that makes recommendations to people about what to read. (This is going to be very very hard, but this is the feature that will put getputs into an altogether different league)
(5) Member Images (Allowing users to upload images, not sure if we should have this feature)
(6) Item Images (Allowing submitters to upload images of an item)
(7) Using a CDN (Content Delivery Network) like Amazon AWS to serve static files (Javascript, Images) etc. This will help reduce the load on our servers.
(8) Adding a location option so that the User can select what area does a particular news/classified item belong to (For now we can start with India (Delhi, Mumbai, Kolkata, Chennai and 10-15 other cities) and US (10-15 major cities)) this will help users filter out the news about a particular location (Example: I would like reading classifieds about Ahmedabad but not about Kolkata etc.)
(9) Check Box in My Accounts sections. Send me daily e-mails at this e-mail address. Receive daily digest to your e-mail.
(10) A Tools Section		
		- Bookmarklet 
			Post to getputs (link)
			Firefox users, drag this to your Bookmarks toolbar.
			IE users, right click and add to Favourites.
		- 'getputs Icon'
			Icon and 'Add to getputs' code for blogs
			Use the following code to place an easy posting link to getputs.
			{Code/scripts}
			Please replace <YOUR-POST-URL> and <YOUR-POST-TITLE> with relevant stuff for your page.
			For eg, for blogspot, use <$BlogItemPermalinkURL$> and <$BlogItemTitle$> respectively.				
(11) A report abuse section (The User needs to be logged in, in order to submit a spam report).     	
	Your email address(optional)
    Infringing URL (required):
    Why is this URL abusive (optional):    
		- Submit


2009-01-05

On Submit.aspx, if you want to use the ReCaptcha Library Control, Add this Markup Code.
--------
<!--
            <td>
                <asp:Label ID="ReCaptchaLabel" runat="server" Text="Enter the Words: "></asp:Label>
            </td>
            
            <%-- ReCaptcha --%>
            
            <td>
                <recaptcha:RecaptchaControl  
                    ID="recaptcha"  
                    runat="server"  
                    PublicKey="6LcWGAIAAAAAANfjBkyJ3xZmYXlJweGwspUwXTPW"              
                    PrivateKey="6LcWGAIAAAAAAM1AlblbioeODqAaH2tPdeVUar30"
                    BackColor="White"
                    ForeColor="Black"
                    Theme="white" 
                     
                />
            </td>
            -->
--------  


2009-01-09

- Created ItemDisplayer.cs. It will contain code to display all the items.
- Replaced LoadItemTable() in Default.aspx.cs with LoadItemTable() from ItemDisplayer.cs          
- Created Rated.aspx.cs. A logged in user can see the items that he rated.
- Changed the GUI for the LoadItemTable() The GUI now uses pure HTML instead of ASP.NET Controls
- Removed ReCaptcha entirely from the Submit.aspx Page. Will add it again if we have a spam problem.

2009-01-12

- Improved ItemDisplayer.cs
- All the pages which display items (Default/AutoTag/Rated/Submitted/Saved/MyNews/Search) now use ItemDisplayer.
- New rules added to Tagger.cs
	Example: 2008, 1947, 1900 etc will be tagged as NN
	Example: "Department of Defense" will get tagged as 1 single sequence. (Still need to check and make sure)
	
2009-01-14

- Added the Environment Category
- Removed the R.aspx bug by adding engine.ItemLinkStartSeperator and engine.ItemLinkEndSeperator to ProcessingEngine.cs
	<!-- For Counting Clicks, the URL will be wrapped between these seperators -->
    <!-- Example: ItemLinkStartSeperator + Item.Link + ItemLinkEndSeperator -->
    <!-- The ItemLinkStartSeperator and ItemLinkEndSeperator will be replaced by and Empty String in R.aspx -->
    <add key="ItemLinkStartSeperator" value="(("></add>
    <add key="ItemLinkEndSeperator" value="))"></add>
    
    
2009-01-16

-	Added the Layout Options to getputs_A.master, getputs_A.master.cs, and ItemDisplayer.cs     
	Tried Javascript, Tried DropDownList, but in the end, settled on ImageButtons.
	
    <%--<select id="LayoutDDL_1" class="CSS_ColorTheme">
                            <option value="Flow" onclick="setLayout('flow'); return false;" title="Flow">Flow</option>
                            <option value="Columns" onclick="setLayout('columns'); return false;" title="Columns">Columns</option>
                            <option value="Categorized" onclick="setLayout('categorized'); return false;" title="Categorized">Categorized</option>                    
                        </select>   --%>
                        
                        <asp:DropDownList ID="LayoutDDL" class="CSS_ColorTheme" runat="server" Font-Names="Verdana" Font-Size="Small" ForeColor="Black" 
                    onselectedindexchanged="LayoutDDL_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem>Flow</asp:ListItem>
                            <asp:ListItem>Columns</asp:ListItem>
                            <asp:ListItem>Categorized</asp:ListItem>                        
                        </asp:DropDownList>
                        
                    <br />
                    
                    
2009-01-19

- Converted Layout Options from a QueryString Parameter to a Cookie (Better Code, more reusable)
- Fixed bugs related to R.aspx url parameter (These bugs happened because of the ItemDisplayer.cs)

2009-01-23

- Twitter/LinkedIn/Facebook Pages Integration
- Error/Success Messages Improvement (Show success messages in Green Background. Show error messages in Red Background)
- (Done) Jokes/Quotes
- News Punchlines (All your news belong to us)
- What you can do with getputs (Tips)
- Color Coding Categories
- E-Mail Address Book (Either showing all the e-mails, last 5-25 e-mails or AJAX)
- Threaded Comments, ability to reply to a comment.
- Comment Alerts
- Statistics Package (NEMailed, NSaved, Clicked, Rated (Amazon Style)) on the ItemDetails Page
- Recommendations - People who read this also read
- While parsing the page for looking for images, parse only till 50 images.
- Show more comments (right now only 25 comments from a user are shown on the commented page)
- OpenID and Facebook Connect
- Text/Link merge (Allow people to submit Text as well as Links)
- Allow people to upload Music (Javascript Music Player)
- Allow people to upload Pictures (Tricky!)
- Strengthen Search
- Tag Cloud (Main Page, Tag Cloud for each person)
- Filter news by Person who submitted
- Add/Remove people in your news network


2009-01-27

- Added QuoteDB.txt (in Files Folder) and QuoteDB.cs (in gp.Files)
- Added FactDB.txt (in Files Folder) and FactDB.cs (in gp.Files)
- Added code for retrieval of random Quotes and Facts in getputs_A.master and getputs_A.master.cs

2009-01-30

- Completed the 3rd Layout with OpenClose() JavaScript in general.cs and changes to ItemDisplayer.cs

2009-02-02

- Added Tools.aspx
- Added getputs Bookmarklets
- Fixed a bug in ItemDisplayer.cs where websiteHL was getting a value instead of being empty when TextItemHL was not empty.
- Fixed the display of 3rd Layout (especially for Category.aspx)

2009-03-18

- Remote Database Connection with GoDaddy now solved.
- Increased the Thumbnail Size (Width * Height) from (100 * 100) to (125 * 125)
- Clicks for users who are not logged in will also be stored for now.

2009-04-01

- Added a TipsDB Class to gp.Files. Describes new features and tips on how to use getputs.
- getputs_A.master will now show a Random Tip at the top of the page to all the users.

2009-04-16

- Implemented jQuery Carousel (http://sorgalla.com/projects/jcarousel/) for ItemImages.
- Here is a sample itemImageList (JavaScript) for testing:
             var itemImageList = [
        "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
        "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>",
         "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
        "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>",
        "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
        "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>",
        "<a href='http://money.cnn.com/2009/04/13/technology/gunther_electric.fortune/index.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1893/1893-1.png' style='border: none;'></img></a>",
        "<a href='http://specials.rediff.com/cricket/2009/apr/13slide1-sachin-tendulkar-at-madame-tussauds.htm' target='_blank' style='text-decoration:none; border-color:Gray;'><img src='http://localhost/getputs//Images/ItemThumbNailStore/1891/1891-1.png' style='border: none;'></img></a>"
        ];
        
