<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="Feedback.aspx.cs" Inherits="Feedback" Title="getputs - Feedback" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
    <div>
        <br />        
        <asp:Label ID="OutputLabel" runat="server" Text=""></asp:Label>
        <br />
        <br />
        <asp:Label ID="MessageLabel" runat="server"></asp:Label><br />
        <br />
        <asp:Label ID="RequiredFieldsLabel" runat="server" Text="Please Fill Out All the Fields below."></asp:Label><br />
        <br />
        <table id="FeedbackTable" style="width: 750px">
            <tr>
                <td style="width: 170px">
                    <asp:Label ID="NameLabel" runat="server" Text="Your Name"></asp:Label></td>
                <td style="width: 225px">
                    <asp:TextBox ID="NameTB" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 170px">
                    <asp:Label ID="EmailLabel" runat="server" Text="Your E-Mail Address"></asp:Label></td>
                <td style="width: 225px">
                    <asp:TextBox ID="EMailTB" runat="server"></asp:TextBox>
                    <asp:Label ID="NoSpamLabel" runat="server" Text="[No Spamming, that's a Promise!]"
                        Width="286px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 170px; height: 24px">
                    <asp:Label ID="TopicLabel" runat="server" Text="Regarding" Width="236px"></asp:Label></td>
                <td style="width: 225px; height: 24px">
                    <asp:DropDownList ID="TopicDDL" runat="server" Width="355px">
                        <asp:ListItem>New Feature Requests</asp:ListItem>
                        <asp:ListItem>Help and Support</asp:ListItem>
                        <asp:ListItem>Bugs/Errors </asp:ListItem>
                        <asp:ListItem>Praise and Flowers</asp:ListItem>
                        <asp:ListItem>Rotten Tomatoes and Stones</asp:ListItem>
                        <asp:ListItem>Go On A Date with Vatsal</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="width: 170px; height: 150px">
                    <asp:Label ID="FeedbackLabel" runat="server" Text="Feedback"></asp:Label></td>
                <td style="width: 225px; height: 150px">
                    <asp:TextBox ID="FeedbackTB" runat="server" Height="125px" TextMode="MultiLine" Width="350px"></asp:TextBox></td>
            </tr>
                       
            
            <tr>
            <td>
                <br />
            </td>
            <td>
                <br />
            </td>
        </tr>
        
        <tr>
            <%--<td>
                <asp:Label ID="ReCaptchaLabel" runat="server" Text="Type the two words: "></asp:Label>
            </td>
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
            </td>--%>
            
            <td>
            
            </td>
            
            <td>
                <img src="JpegImage.aspx" alt="Captcha" />
                <br />
                <asp:Label ID="ReCaptchaLabel" runat="server" Text="Enter the Digits: "></asp:Label>
                <asp:TextBox ID="CodeNumberTextBox" runat="server" Width="50px"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td>
                <br />
            </td>
            <td>
                <br />
            </td>
        </tr>
            
            
            <tr>
                <td style="width: 170px; height: 26px">
                </td>
                <td style="width: 190px; height: 26px">
                    <asp:Button ID="SubmitFeedbackButton" runat="server" OnClick="SubmitFeedbackButton_Click"
                        Text="Submit Feedback" /></td>
            </tr>
        </table>
    </div>
    <br />
    <br />
</asp:Content>

