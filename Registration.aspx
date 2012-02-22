<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" Title="getputs - Registration" AutoEventWireup="true" CodeFile="Registration.aspx.cs" Inherits="Registration" ValidateRequest="false"%>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
    <br />
    <br />
    <asp:Label ID="MessageLabel" runat="server" Width="300px"></asp:Label>
    <br />
    <br />
    <table id="RegistrationTable" style="width: 347px; height: 221px;">
        <tr>
            <td style="width: 120px">
                <asp:Label ID="UsernameLabel" runat="server" Text="Username"></asp:Label></td>
            <td style="width: 154px">
                <asp:TextBox ID="UsernameTB" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 120px">
                <asp:Label ID="PasswordLabel" runat="server" Text="Password"></asp:Label></td>
            <td style="width: 154px">
                <asp:TextBox ID="PasswordTB" runat="server" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 120px">
                <asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm Password" Width="113px"></asp:Label></td>
            <td style="width: 154px">
                <asp:TextBox ID="ConfirmPasswordTB" runat="server" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 120px; height: 26px;">
                <asp:Label ID="EMailLabel" runat="server" Text="EMail"></asp:Label></td>
            <td style="width: 154px; height: 26px;">
                <asp:TextBox ID="EMailTB" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 120px; height: 26px">
                <asp:Label ID="AboutLabel" runat="server" Text="About"></asp:Label></td>
            <td style="width: 154px; height: 26px">
                <asp:TextBox ID="AboutTB" runat="server" TextMode="MultiLine" Height="126px" MaxLength="500" Width="200px"></asp:TextBox></td>
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
            
            <%--<td>
            
            </td>
            
            <td>
                <img src="JpegImage.aspx" alt="Captcha" />
                <br />
                <asp:Label ID="ReCaptchaLabel" runat="server" Text="Enter the Digits: "></asp:Label>
                <asp:TextBox ID="CodeNumberTextBox" runat="server" Width="50px"></asp:TextBox>
            </td>--%>
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
            <td style="width: 100px">
            </td>
            <td align="right" style="width: 154px">                
                <asp:Button ID="RegistrationButton" runat="server" OnClick="RegistrationButton_Click" Text="Sign Up" />
            </td>
        </tr>
    </table>
    
    <br />
    <asp:HyperLink ID="LoginLink" runat="server" Font-Underline="false" NavigateUrl="~/SLogin.aspx">Already have a getputs Account?</asp:HyperLink><br />
    <br />
</asp:Content>

