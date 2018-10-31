using Newtonsoft.Json;
using QJY.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QJY.WEB.ViewV5
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Click(object sender, EventArgs e)
        {
            string Mobiles = "13980444473";
            string Content = "尊敬的向毅，凉山烟草智能会议系统目前已更新至V2.6版本。本次更新如下：1.加入导航回参会人住宿地功能；2.开通宣传片在线播放功能。3.实时短信通知功能。如有异常，请向余坤15196180999反馈。谢谢！会议访问链接：http://lstobacco.com/hy.html?id=271";
            string result = CommonHelp.SendMAS(Mobiles, Content);
            Response.Write(result);
        }

        protected void shotbtn_Click(object sender, EventArgs e)
        {
            var result= CommonHelp.GetShortUrl("http://www.lstobacco.com/view_mobile/ui/HyDetail_index_r.html?id=271");
            Response.Write(result);
        }
    }
}