using QJY.API;
using QJY.Data;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Web;

namespace QJY.WEB.WX.AUTH
{
    public partial class LogistRedirect : System.Web.UI.Page
    {
        string code = CommonHelp.GetQueryString("code");
        protected void Page_Load(object sender, EventArgs e)
        {
            //客户零售系统的授权页面
            if (!string.IsNullOrWhiteSpace(code))
            {
                //OAuthAccessTokenResult u = WXFWHelp.GetWXUserOAuth(code);
                WX_User u = WXFWHelp.GetWXUserInfoWithUpdateLocal(code);
                if (u != null)
                {
                    JH_Auth_User userInfo = new JH_Auth_UserB().GetEntity(d => d.WXopenid == u.Openid && d.IsWX == 1);
                    if (userInfo != null)//已绑定手机号和姓名
                    {
                        //如果姓名、身份证、手机号、专卖证号其中一项为空，则跳转到绑定页面
                        if (string.IsNullOrEmpty(userInfo.IDCard) || string.IsNullOrEmpty(userInfo.UserRealName) || string.IsNullOrEmpty(userInfo.mobphone) || string.IsNullOrEmpty(userInfo.ToMonoLicense))
                        {
                            RedirectTomo(u.Openid);
                        }
                        else
                        {
                            string redirectpage = "http://order.lstobacco.com:5222/tobacco_logist?licenseCode=" + userInfo.ToMonoLicense; 
                            Response.Redirect(redirectpage);
                        }
                    }
                    else
                    {
                        RedirectTomo(u.Openid);
                    }
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

        /// <summary>
        /// 跳转到绑定专卖证页面
        /// </summary>
        protected void RedirectTomo(string openid)
        {
            DateTime expires = DateTime.Now.AddMinutes(30);
            CommonHelp.SetCookie("openid", openid, expires);
            Response.Redirect("/WX/AUTH/BindTomo.html");
        }
    }
}