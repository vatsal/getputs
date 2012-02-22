<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="MyNews.aspx.cs" Inherits="MyNews" Title="getputs - MyNews" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
    <asp:Label ID="MessageLabel" runat="server"></asp:Label>
    <br />
    <asp:HyperLink ID="MyAccountLink" runat="server" Text="My Account" NavigateUrl="~/UserDetails.aspx" Font-Underline="false"></asp:HyperLink>
    <br />   
    <br />
    <br />
    <div id="ItemDiv" runat="server">
    
    </div>
</asp:Content>

