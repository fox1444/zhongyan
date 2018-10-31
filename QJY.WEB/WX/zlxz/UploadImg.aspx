<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadImg.aspx.cs" Inherits="QJY.WEB.WX.UploadImg" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server"  method="post" enctype="multipart/form-data">
        <div>
            <input id="fileUpload" type="file" multiple runat="server" /><br />
            <asp:Button ID="btnUpload" runat="server" Text="开始上传" OnClick="btnUpload_Click" />
            <div id="image-holder"></div>
            
        </div>
    </form>
    <script src="/View_Mobile/js/layer/layer.m.js"></script>
    <script type='text/javascript' src='//g.alicdn.com/sj/lib/zepto/zepto.min.js' charset='utf-8'></script>
    <script src="/View_Mobile/JS/avalon1.47.js"></script>
    <script src="/View_Mobile/JS/raty/jquery.raty.js?v=1.3"></script>
    <script>
        $(function () {
            $("#fileUpload").on('change', function () {
                //获取上传文件的数量
                var countFiles = $(this)[0].files.length;

                var imgPath = $(this)[0].value;
                var extn = imgPath.substring(imgPath.lastIndexOf('.') + 1).toLowerCase();
                var image_holder = $("#image-holder");
                image_holder.empty();

                if (extn == "gif" || extn == "png" || extn == "jpg" || extn == "jpeg") {
                    if (typeof (FileReader) != "undefined") {

                        // 循环所有要上传的图片
                        for (var i = 0; i < countFiles; i++) {

                            var reader = new FileReader();
                            reader.onload = function (e) {
                                $("<img />", {
                                    "src": e.target.result,
                                    "class": "thumb-image"
                                }).appendTo(image_holder);
                            }

                            image_holder.show();
                            reader.readAsDataURL($(this)[0].files[i]);
                        }

                    } else {
                        alert("你的浏览器不支持FileReader！");
                    }
                } else {
                    alert("请选择图像文件。");
                }
            });
        })
    </script>
</body>
</html>
