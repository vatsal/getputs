<%@ Page Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="Submit.aspx.cs" Inherits="Submit" Title="getputs - Submit" ValidateRequest="false"%>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">    
    <asp:Label ID="ExplanationLabel" runat="server"></asp:Label><br />    
    <br />
    <asp:Label ID="MessageLabel" runat="server" Width="500px"></asp:Label>
    <br />
    <br />    
    <table id="SubmitTable">
    
       <tr>
            <td style="width: 70px">
                <asp:Label ID="LinkLabel" runat="server" Text="Link"></asp:Label></td>
            <td style="width: auto;">
                <asp:TextBox ID="LinkTB" runat="server" Width="388px"></asp:TextBox></td>
        </tr>
        
        
        <tr>
            <td style="width: 70px">
            
            </td>
            <td>
                <asp:Button id="GetTitleButton" runat="server" Text="Get Title" OnClick="GetTitleButton_Click"/> 
                <br />
                <asp:Literal ID="GetTitleOutputLiteral" Text="" runat="server"></asp:Literal>    
                <br />       
                <br />                                 
            </td>
        </tr>
    
        <tr>
            <td style="width: 70px">
                <asp:Label ID="TitleLabel" runat="server" Text="Title"></asp:Label></td>
            <td style="width: auto;">
                
                <asp:TextBox ID="TitleTB" TextMode="MultiLine" runat="server" Width="388px"></asp:TextBox>
                            
            </td>
                
        </tr>
      
        
         <tr>
            <td style="width: 70px; height: 26px">
                <asp:Label ID="CategoryLabel" runat="server" Text="Category"></asp:Label></td>
            <td style="width: auto; height: 26px">
                <asp:DropDownList ID="CategoryDDL" runat="server">
                </asp:DropDownList></td>
        </tr>
        
        
        <tr>
            <td style="width: 70px">
                <asp:Label ID="TextLabel" runat="server" Text="Text"></asp:Label></td>
            <td style="width: auto;">
                <asp:TextBox ID="TextTB" runat="server" MaxLength="500" TextMode="MultiLine" Height="100px" Width="388px"></asp:TextBox></td>
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
            
               
            <!--                                   
            <td>
            
            </td>
            
            <td>
                <img src="JpegImage.aspx" alt="Captcha" />
                <br />
                <asp:Label ID="ReCaptchaLabel_X" runat="server" Text="Enter the Digits: "></asp:Label>
                <asp:TextBox ID="CodeNumberTextBox" runat="server" Width="50px"></asp:TextBox>
            </td>
            -->
            
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
            <td style="width: 70px">
            </td>
            <td align="right" style="width: 400px">
                <asp:Button ID="SubmitButton" runat="server" OnClick="SubmitButton_Click"
                    Text="Submit" />
            </td>
        </tr>
    </table>
   


    <br />
    <br />
    
    
    <div id="GuideLinesDiv">
    
        Guidelines for Posting:
        <br />
        <br />
        A few things to consider while posting:
        <br />
            &nbsp;&nbsp;
            * Post anything interesting: 
                <br />
                &nbsp;&nbsp; &nbsp;&nbsp; News <br />
                &nbsp;&nbsp; &nbsp;&nbsp; Classifieds <br />
                &nbsp;&nbsp; &nbsp;&nbsp; Questions that you want the getputs Community to Answer <br />
                &nbsp;&nbsp; &nbsp;&nbsp; Articles <br />
                &nbsp;&nbsp; &nbsp;&nbsp; Blogs <br />
                &nbsp;&nbsp; &nbsp;&nbsp; Video <br />
                &nbsp;&nbsp; &nbsp;&nbsp; Pictures <br />            
            &nbsp;&nbsp;
            * Post things you think are worth reading.
            <br />
            &nbsp;&nbsp;
            * Link to the original source, not a site that is copying someone else's work.
            <br />
            &nbsp;&nbsp;
            * Do not post links that contain spam, pornography, or profanity.
            <br />
    
    </div>


</asp:Content>

