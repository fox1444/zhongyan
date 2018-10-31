using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QJY.WEB.WX
{
    public partial class UploadImg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            HttpFileCollection Files = HttpContext.Current.Request.Files;
            for (int i = 0; i < Files.Count; i++)
            {
                HttpPostedFile PostedFile = Files[i];
                
                if (PostedFile.ContentLength > 0)
                {
                    string FileName = PostedFile.FileName;
                    Response.Write(FileName);
                    string strExPrentFile = FileName.Substring(FileName.LastIndexOf(".") + 1);

                    string sFilePath = "/uploadfile/11" + i + "."+strExPrentFile;
                    PostedFile.SaveAs(Server.MapPath(sFilePath));

                    
                }
                else
                {
                    //this.LabMessage.Text = "不能上传空文件";
                }
            }
        }
    }
}