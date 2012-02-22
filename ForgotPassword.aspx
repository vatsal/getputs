<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="ForgotPassword.aspx.cs" Inherits="ForgotPassword" Title="getputs - Forgot Password" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">

<table id="ForgotPasswordTable">
        <tr>
            <td style="width: 60px">
                <asp:Label ID="UsernameLabel" runat="server" Text="Username"></asp:Label></td>
            <td style="width: 90px">
                <asp:TextBox ID="UsernameTB" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 60px">
                <asp:Label ID="EMailLabel" runat="server" Text="EMail"></asp:Label></td>
            <td style="width: 90px">
                <asp:TextBox ID="EMailTB" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 60px">
            </td>
            <td align="right" style="width: 90px">
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
            </td>
        </tr>
    </table>
    <asp:Label ID="MessageLabel" runat="server" Width="500px"></asp:Label><br />
    
    <br />    
    <br />
    
    <asp:HyperLink ID="LoginHL" runat="server" Font-Underline="false" NavigateUrl="~/SLogin.aspx">Already have a getputs Account?</asp:HyperLink>
    <br />
    <br />    
    <asp:HyperLink ID="NewUserHL" runat="server" Font-Underline="false" NavigateUrl="~/Registration.aspx" Width="225px">New to getputs? Sign Up.</asp:HyperLink>

</asp:Content>

