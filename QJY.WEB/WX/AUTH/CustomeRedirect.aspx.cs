using QJY.API;
using QJY.Data;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Web;

namespace QJY.WEB.WX.AUTH
{
    public partial class CustomeRedirect : System.Web.UI.Page
    {
        string code = CommonHelp.GetQueryString("code");
        protected void Page_Load(object sender, EventArgs e)
        {
            //零售系统的消费者授权页面
            if (!string.IsNullOrWhiteSpace(code))
            {
                //OAuthAccessTokenResult u = WXFWHelp.GetWXUserOAuth(code);
                WX_User u = WXFWHelp.GetWXUserInfoWithUpdateLocal(code);
                if (u != null)
                {
                    string redirectpage = "http://order.lstobacco.com:5222/tobacco_customer?openId=" + u.Openid;
                    Response.Redirect(redirectpage);
                }
                else
                {
                    Response.Write("获取用户信息失败！");
                }
            }
            else
            {
                Response.Write("参数错误！");
            }
        }
    }
}