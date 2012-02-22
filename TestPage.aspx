<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestPage.aspx.cs" Inherits="TestPage" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Test Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <!-- <script language="javascript" src="http://www.quotedb.com/quote/quote.php?action=random_quote"></script> -->
        <br />        
        
        <asp:Button ID="StoreQuotesButton" runat="server" OnClick="StoreQuotesButton_Click" Text="Store Quotes" />        
        <asp:Button ID="GetRandomQuoteButton" runat="server" OnClick="GetRandomQuoteButton_Click" Text="Get Random Quote" />
        <asp:Label ID="ShowRandomQuoteLabel" runat="server"></asp:Label>
        
        <br />
        <br />
                
        <asp:Button ID="StoreFactsButton" runat="server" OnClick="StoreFactsButton_Click" Text="Store Facts" />        
        <asp:Button ID="GetRandomFactButton" runat="server" OnClick="GetRandomFactButton_Click" Text="Get Random Fact" />
        <asp:Label ID="ShowRandomFactLabel" runat="server"></asp:Label>
        
        <br />
        <br />
        
        <asp:Button ID="ReloadRandomTipButton" runat="server" OnClick="ReloadRandomTipButton_Click" Text="Reload Random Tip File" />
        <asp:Label ID="ShowRandomTipLabel" runat="server"></asp:Label>
        
        <br />
        <br />
            
        <asp:Button ID="TestButton" runat="server" OnClick="TestButton_Click" Text="Test" /><br />
        <asp:TextBox ID="OutputTB" runat="server" Height="300px" Width="500px" TextMode="MultiLine"></asp:TextBox>
        
        <br />
        <br />
        
        <a id="TestLink" href="About.aspx" title="Test Link" runat="server" onclick="TestLinkClick()">Test Link</a>
        <asp:LinkButton runat="server" ID="TestLinkButton" Text="Test Link Button" OnClick="TestLinkButton_Click"></asp:LinkButton>
        <br />
        <br />
        <br />
        <asp:TextBox ID="RegExpInputTB" runat="server" Height="75px" TextMode="MultiLine"
            Width="500px"></asp:TextBox><br />
        <br />
        <asp:LinkButton ID="TestHTMLLinkRegExpLB" runat="server" OnClick="TestHTMLLinkRegExpLB_Click">TestHTMLLinkRegExp</asp:LinkButton>
        <br />
        <br />
        <asp:Label ID="OutputLabel" runat="server"></asp:Label>
        <br />
        <br />
        <asp:Label ID="InputIIDLabel" runat="server" Text="Input IID"></asp:Label>
        <asp:TextBox ID="InputIIDTB" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="ImageStoreButton" runat="server" 
            onclick="ImageStoreButton_Click" Text="Store Image" />
        <asp:Label ID="ImageStoreLabel" runat="server" Text="..."></asp:Label>
        <br />
        <br />
        <br />
        <div id="CrawlAndParseDiv">
            <table>
            <tr>
            <td>
                <asp:Label ID="InputURLLabel" runat="server" Text="Input URL"></asp:Label>
                <asp:TextBox ID="InputURLTB" runat="server"></asp:TextBox>
            </td>
            </tr>
            
            <tr>
            <td align="right">
                <asp:Button id="URLButton" runat="server" Text="Get Title" OnClick="URLButton_Click"/>                
                <asp:Button id="IMGButton" runat="server" Text="Get Images" OnClick="IMGButton_Click"/>                
            </td>
            </tr>
            
            <tr>
            <td>
                <asp:Label ID="OutputURLLabel" runat="server"></asp:Label>
            </td>
            </tr>
            
            <tr>
            <td>
                <img runat="server" id="PageImage" alt="No Image Found" src="Images/Icons/accept.png"/>
            </td>
            </tr>
            
            </table>        
        </div>
    </div>
        
    </form>
</body>
</html>
