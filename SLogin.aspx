<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" Title="getputs - Login" AutoEventWireup="true" CodeFile="SLogin.aspx.cs" Inherits="SLogin" ValidateRequest="false"%>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
    <br />
    <table id="LoginTable">
        <tr>
            <td style="width: 60px">
                <asp:Label ID="UsernameLabel" runat="server" Text="Username"></asp:Label></td>
            <td style="width: 90px">
                <asp:TextBox ID="UsernameTB" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 60px">
                <asp:Label ID="PasswordLabel" runat="server" Text="Password"></asp:Label></td>
            <td style="width: 90px">
                <asp:TextBox ID="PasswordTB" runat="server" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 60px">
            </td>
            <td align="right" style="width: 90px">
                <asp:Button ID="SLoginButton" runat="server" OnClick="SLoginButton_Click" Text="Login" />
            </td>
        </tr>
    </table>
    <asp:Label ID="MessageLabel" runat="server" Width="200px"></asp:Label><br />
    <br />
    <asp:HyperLink ID="NewUserHL" runat="server" Font-Underline="False" NavigateUrl="~/Registration.aspx" Width="225px">New to getputs? Sign Up.</asp:HyperLink>
    <br />
    <br />
    <asp:HyperLink ID="ForgotPasswordHL" runat="server" Font-Underline="false" NavigateUrl="~/ForgotPassword.aspx">Forgot your Password?</asp:HyperLink>    

        
    <br />
    
</asp:Content>

