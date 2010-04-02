<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RewriterError._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox runat="server" ID="txtTest" />
        <asp:LinkButton runat="server" ID="btnTest" Text="Click Me" />
        <asp:RequiredFieldValidator EnableClientScript="true" runat="server" ID="rfvTest" ControlToValidate="txtTest" Text="The text box must have a value." />
    </div>
    
    </form>
</body>
</html>
