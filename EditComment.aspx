<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="EditComment.aspx.cs" Inherits="EditComment" Title="getputs - EditComment" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
    <br />
    <br />
    <asp:Label ID="MessageLabel" runat="server" Text=""></asp:Label>
       <br />
       <br />
       <table id="EditTable" style="width:80%">
       
        <tr>            
            <td align="left">
                <asp:Label ID="CurrentCommentLabel" runat="server" Text=""></asp:Label>
                <br />
                <br />                
            </td>        
        </tr>
       
        <tr>
            <td align="lect">
                <asp:TextBox ID="EditCommentTB" runat="server" Width="400px" Height="200px"></asp:TextBox>
            </td>
        </tr>
       
        <tr>            
            <td align="right">
                <asp:Button ID="EditCommentButton" runat="server" OnClick="EditCommentButton_Click"  Text="Append to your Comment" /> 
            </td>
        </tr>       
       </table>
    
</asp:Content>

