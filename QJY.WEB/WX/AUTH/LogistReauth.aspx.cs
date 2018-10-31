using QJY.API;
using QJY.Data;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Web;

namespace QJY.WEB.WX.AUTH
{
    public partial class LogistReauth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //客户零售系统的授权页面
            string reauthriseurl = CommonHelp.GetConfig("HostUrl") + "/wx/auth/logistredirect.aspx";
            if (reauthriseurl.Trim().Length > 0)
            {
                string authurl = OAuthApi.GetAuthorizeUrl(CommonHelp.GetConfig("AppId"), reauthriseurl, "reload", OAuthScope.snsapi_userinfo);
                if (authurl.Length > 0)
                {
                    Response.Redirect(HttpUtility.UrlDecode(authurl));
                }
            }
        }
    }
}