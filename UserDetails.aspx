<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="UserDetails.aspx.cs" Inherits="UserDetails" Title="getputs - User Details" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
    <asp:Label ID="UserDetailsLabel" runat="server"></asp:Label>
    
    
<div id="UpdateUserDetailsTable" runat="server">
    <table>    
        <tr>
        <td>
        <asp:Label ID="UserLabelL" runat="server" Text="User:"></asp:Label>
        </td>
        <td>
        <asp:Label ID="UserLabelR" runat="server"></asp:Label>
        </td>
        </tr>
        
        <tr>
        <td>
        <asp:Label ID="CreatedLabelL" runat="server" Text="Created:"></asp:Label>
        </td>
        <td>
        <asp:Label ID="CreatedLabelR" runat="server"></asp:Label>
        </td>
        </tr>
        
        <tr>
        <td>
        <asp:Label ID="PasswordLabelL" runat="server" Text="Password:"></asp:Label>
        </td>
        <td>
        <asp:HyperLink ID="PasswordLink" runat="server" Text="Reset Password" NavigateUrl="~/ResetPW.aspx" Font-Underline="false"></asp:HyperLink>
        </td>
        </tr>
        
        
        <tr>        
        <td>
            <asp:Label ID="EMailLabel" runat="server" Text="EMail:"></asp:Label>            
        </td>
        <td>
            <asp:TextBox ID="EMailTB" runat="server"></asp:TextBox>
        </td>
        </tr>
        
        <tr>        
        <td>
            <asp:Label ID="AboutLabel" runat="server" Text="About:"></asp:Label>            
        </td>
        <td>
            <asp:TextBox ID="AboutTB" runat="server" TextMode="MultiLine"></asp:TextBox>
        </td>        
        </tr>
        
         
        <tr>
        <td>
            <asp:Button ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click"></asp:Button>        
        </td>
        <td>
             <asp:Label ID="MessageLabel" runat="server" Text=""></asp:Label>
        </td>
        </tr>      
       
    </table>
    </div>
    
    <br />
    <br />
    
    <asp:LinkButton ID="SubmittedLB" runat="server" Text="Submissions" CssClass="CSS_NavigationTableLinkButtons" OnClick="SubmittedLB_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="CommentedLB" runat="server" Text="Comments" CssClass="CSS_NavigationTableLinkButtons" OnClick="CommentedLB_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="SavedLB" runat="server" Text="Saved" CssClass="CSS_NavigationTableLinkButtons" OnClick="SavedLB_Click"></asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="RatedLB" runat="server" Text="Rated" CssClass="CSS_NavigationTableLinkButtons" OnClick="RatedLB_Click"></asp:LinkButton>
    <br />    
    <br />
    <br />
        
    <asp:CheckBoxList ID="InterestsCBL" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"></asp:CheckBoxList>    
    <asp:Button ID="AddButton" runat="server" Text="Save" Width="50px" OnClick="AddButton_Click" />
    &nbsp;&nbsp;
    <asp:Label ID="SaveChangesLabel" runat="server"></asp:Label>
    
    
</asp:Content>
