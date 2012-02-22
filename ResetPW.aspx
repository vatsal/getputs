<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="ResetPW.aspx.cs" Inherits="ResetPW" Title="getputs - Reset Password" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">

    <table id="ResetPWTable">
        <tr>
        <td>
            <asp:Label ID="CurrentPasswordLabel"  runat="server" Text="Current Password"></asp:Label>            
        </td>
        
        <td>
            <asp:TextBox ID="CurrentPasswordTB" runat="server" TextMode="Password"></asp:TextBox>
        </td>
        </tr>

        <tr>
        <td>
            <asp:Label ID="NewPasswordLabel"  runat="server" Text="New Password"></asp:Label>             
        </td>
       
        <td>
            <asp:TextBox ID="NewPasswordTB" runat="server" TextMode="Password"></asp:TextBox>
        </td>
        </tr>

        <tr>
        <td>
            <asp:Label ID="ConfirmNewPasswordLabel"  runat="server" Text="Confirm New Password"></asp:Label>                         
        </td>
        
        <td>
            <asp:TextBox ID="ConfirmNewPasswordTB" runat="server" TextMode="Password"></asp:TextBox>
        </td>
        </tr>


        <tr>
        <td>
                    
        </td>
       
        <td>
            <asp:Button ID="ResetPasswordButton" runat="server" Text="Reset Password" OnClick="ResetPasswordButton_Click" />    
        </td>
        </tr>
        
        
    </table>
    
    <asp:Label ID="MessageLabel" runat="server" Width="500px"></asp:Label>
    
</asp:Content>

