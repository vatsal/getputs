<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FB_Connect_1.aspx.cs" Inherits="FB_Connect_1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<head runat="server">
    <title>FB Connect 1</title>
    
    <script src="http://static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php"  type="text/javascript">
    </script>  
    
</head>
<body>
    <form id="form1" runat="server">
    <div>
           <fb:login-button onlogin="alert('Authenticated!');"></fb:login-button>  
    </div>
    </form>
    
    <script type="text/javascript">
        FB_RequireFeatures(["XFBML"], function() {
            FB.Facebook.init("207bc09b7ee8b20371920b9dc5d52c5c", "xd_receiver.htm");
            });
    </script>
        
</body>
</html>
