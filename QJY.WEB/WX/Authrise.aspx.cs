using QJY.API;
using QJY.Data;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Web;

namespace QJY.WEB.WX
{
    public partial class Authrise : System.Web.UI.Page
    {
        string szhlcode = CommonHelp.Getszhlcode();
        string commit = CommonHelp.GetQueryString("commit");
        protected void Page_Load(object sender, EventArgs e)
        {
            //重新跳转授权页面
            if (commit == "reauthrise")
            {
                string reauthriseurl = CommonHelp.GetConfig("HostUrl") + "/wx/Accomplish.aspx";
                if (reauthriseurl.Trim().Length > 0)
                {
                    string authurl = OAuthApi.GetAuthorizeUrl(CommonHelp.GetConfig("AppId"), reauthriseurl, "reload", OAuthScope.snsapi_userinfo);
                    if (authurl.Length > 0)
                    {
                        Response.Redirect(authurl);
                    }
                }
            }
        }
    }
}