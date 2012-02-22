<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Search" Title="getputs - Search Page" %>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">

    <table width="100%" class="NoDesign">
        <tr>
            <td style="width:100%" align="left">  
                <asp:Panel ID="SearchPanel" DefaultButton="SearchButton" runat="server">
                    <table id="SearchTable" class="NoDesign" width="80%">
                        <tr>
                            <td align="left">
                                <asp:TextBox ID="SearchTB" runat="server" Width="100%"></asp:TextBox>                                                                                                
                            </td>
                        </tr>    
            
                        <tr>
                            <td align="left">    
                                <!-- Have kept the table invisible for now -->
                                <table class="NoDesign" width="100%" visible="false" >
                                    <tr>
                                        <td style="width:25%" align="center">
                                            <asp:DropDownList ID="SearchTypeDDL" runat="server">
                                                <asp:ListItem Value="Title">Title</asp:ListItem>                                                
                                                <asp:ListItem Value="Links">Links</asp:ListItem>
                                                <%--<asp:ListItem Value="Comments">Comments</asp:ListItem>--%>
                                                
                                            </asp:DropDownList>                                
                                        </td>
                    
                                        <td style="width:25%" align="center">
                                            <asp:DropDownList ID="SearchCategoryDDL" runat="server"></asp:DropDownList>                                                                
                                        </td>
                                        
                                        <td style="width:25%" align="center">
                                            <asp:DropDownList ID="SearchTimeDDL" runat="server">                                            
                                                <asp:ListItem Value="Month">This Month</asp:ListItem>
                                                <asp:ListItem Value="Today">Today</asp:ListItem>
                                                <asp:ListItem Value="Week">This Week</asp:ListItem>                                                
                                                <asp:ListItem Value="Year" Selected="True">This Year</asp:ListItem>
                                            </asp:DropDownList>                                                                
                                        </td>
                    
                                        <td style="width:25%" align="center">
                                            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
                                        </td>                                    
                                    </tr>
                                </table>
                            </td>
                        </tr>  

                        <tr>
                            <td>
                                <asp:Label ID="MessageLabel" runat="server" Width="100%" Text=""></asp:Label>              
                            </td>
                        </tr>    

                        

            
                        <tr>
                            <td>
            
                            </td>
                        </tr>    
            
                    </table>
                </asp:Panel>
        
            </td>
        </tr>
        
        <tr>
        <td>
            <br />
            <br />
        </td>
        </tr>
        
        <tr>
            <td>
                <div id="ItemDiv" class="NoDesign" runat="server">
                
                </div>                
            </td>
        </tr>               
        
    </table>

</asp:Content>

