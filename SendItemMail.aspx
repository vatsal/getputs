<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="SendItemMail.aspx.cs" Inherits="SendItemMail" Title="getputs - Send Mail" ValidateRequest="false"%>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">

        <asp:Label ID="ItemTitleLabel" runat="server"></asp:Label>
        <br />
        <br />

        <a id="EMailer"></a>
        <div id="EMailDiv" runat="server">
        
        <asp:Label ID="EMailTableDescriptionLabel" Text="EMail this Item." runat="server"></asp:Label>
        <br />                        
        <br />
        <asp:Label ID="EMailOutputMessageLabel" Text="" runat="server"></asp:Label>
        <br />
        <br />
        <table id="EMailTable" style="width:100%;">
        <tr>
            <td style="width: 20%">
                <asp:Label ID="FromLabel" Text="From" runat="server"></asp:Label>        
            </td>
            <td style="width:80%">
                <asp:TextBox ID="FromTB" runat="server" Width="95%" ></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td style="width: 20%">
                <asp:Label ID="ToLabel" Text="To" runat="server"></asp:Label>
            </td>
            <td style="width:80%">            
                <asp:Label ID="MultipleAddressesLabel" Text="Seperate EMails Addresses by a Comma (,)" runat="server"></asp:Label>
                <br />
                <asp:TextBox ID="ToTB" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td style="height: 44px; width: 20%;">
                <asp:Label ID="EMailMessageLabel" Text="Message" runat="server"></asp:Label>
            </td>
            <td style="width: 80%">  
                <asp:TextBox ID="EMailMessageTB" runat="server" Height="100px" TextMode="MultiLine" Width="95%"></asp:TextBox>
            </td>
        </tr>
        
        
        <%--<tr>
            <td>
                <br />
            </td>
            <td>
                <br />
            </td>
        </tr>
        --%>
        <tr>
            <td>
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
            </td>
            
           <%-- <td style="width: 80px">
            
            </td>
            
            <td>
                <img src="JpegImage.aspx" alt="Captcha" />
                <br />
                <asp:Label ID="ReCaptchaLabel" runat="server" Text="Enter the Digits: "></asp:Label>
                <asp:TextBox ID="CodeNumberTextBox" runat="server" Width="50px"></asp:TextBox>
            </td>--%>
        </tr>
        
        
        <tr>
            <td style="width: 80px">
                <br />
            </td>
            <td>
                <br />
            </td>
        </tr>
        
        <tr>
            <td style="width: 80px">
                           
            </td>
            <td align="left">  
                <asp:Button ID="EMailButton" Text="Send" runat="server" OnClick="EMailButton_Click"></asp:Button> 
            </td>
        </tr>       
        
        </table>
        </div>

</asp:Content>

