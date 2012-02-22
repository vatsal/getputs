<%@ Page Title="getputs - Tools " Language="C#" MasterPageFile="~/getputs_A.master" AutoEventWireup="true" CodeFile="Tools.aspx.cs" Inherits="Tools" %>

<asp:Content ID="Main" ContentPlaceHolderID="Main" Runat="Server">

<br />

    <b>getputs Bookmarklet:</b> 
<br />
<br />
The getputs Bookmarklet allows you to Post News/Articles/Links directly from your Browser.
<br />
When you click on the bookmarklet, it will submit the page you are currently viewing. 
<br />
To install, drag the following link to your browser toolbar: 
<br />
<br />
<a href="javascript:location.href='http://www.getputs.com/Submit.aspx?url='+encodeURIComponent(location.href)+'&amp;title='+encodeURIComponent(document.title)" onclick="alert('Drag this link to your Toolbar.'); return false;">getputs</a>

<br />
<br />

</asp:Content>

