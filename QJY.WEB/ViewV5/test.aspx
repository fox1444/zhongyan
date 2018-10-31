<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="QJY.WEB.ViewV5.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
</head>
<body>
    <form runat="server">
        <div>
            <asp:Button ID="btn" runat="server" Text="提交" OnClick="btn_Click" Visible="false" />
            <asp:Button ID="shotbtn" runat="server" Text="短链接" OnClick="shotbtn_Click" />
        </div>
    </form>
</body>
</html>
