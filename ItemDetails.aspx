<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="ItemDetails.aspx.cs" Inherits="ItemDetails" Title="getputs - Item Details" ValidateRequest="false"%>
<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">
        <div id="ItemDiv" runat="server">
        </div>
    
        <table width="500px">
        <tr>
        <td style="width: 100%" align="center">
            <asp:TextBox ID="CommentTB" runat="server" Height="180px" TextMode="MultiLine" Width="600px"></asp:TextBox>            
        </td>
        </tr>
        
        <tr>
        <td style="width: 100%;" align="center">
            <table class="NoDesign" width="100%">            
                <tr>                
                    <td style="width: 70%" align="left">
                        <asp:Button ID="DirectContactButton" runat="server" Text="Send Message by E-Mail" OnClick="DirectContactButton_Click"
                            ToolTip="Click if you need to send a private e-mail message to the person who submitted this item." />
                    </td>  
                    <td style="width: 30%" align="right">
                        <asp:Button ID="CommentButton" runat="server" Text="Comment" OnClick="CommentButton_Click" />
                    </td>                          
                </tr>        
            </table>
        </td>
        </tr>
        
        </table>
        <asp:Label ID="MessageLabel" runat="server"></asp:Label>
        
        
        <br />
        <br />
        <br />
        
        
        
        
        
        <span id="AdSpan" class="AdSpan">
        
            <!-- /* getputs_HorizontalAdLinks_728x15_Ads */ -->
            <script type="text/javascript"><!--
                google_ad_client = "pub-6493032152362679";
                /* getputs_HorizontalAdLinks_728x15_Ads */
                google_ad_slot = "2382540695";
                google_ad_width = 600;
                google_ad_height = 15;
            //-->
            </script>
            <script type="text/javascript"
            src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
            </script>
        
            <!-- getputs_LeaderBoard_728x90_ads -->
            <%--
            <script type="text/javascript"><!--
                google_ad_client = "pub-6493032152362679";
                /* getputs_LeaderBoard_728x90_ads */
                google_ad_slot = "4791087586";
                google_ad_width = 728;
                google_ad_height = 90;
            //-->
            </script>
            <script type="text/javascript"
            src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
            </script>
            --%>
        </span>
        
        <br />
        <br />
                
        <table id="CommentDiv" class="NoDesign" runat="server">
        
        </table>
                
        <br />
                
       
        
        
        
        
</asp:Content>

