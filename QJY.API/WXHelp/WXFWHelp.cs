using QJY.Data;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QJY.API
{
    /// <summary>
    /// 微信公众号服务类
    /// </summary>
    public class WXFWHelp
    {
        private static string AppId = CommonHelp.GetConfig("AppId");
        private static string AppSecret = CommonHelp.GetConfig("AppSecret");
        private static string HostUrl = CommonHelp.GetConfig("HostUrl");

        public WXFWHelp()
        {
        }

        /// <summary>
        /// Global中定时更新AccessToken
        /// </summary>
        public static void UpdateTokenByTimer()
        {
            string strIp = CommonHelp.getIP(HttpContext.Current);
            try
            {
                string acc = GetTokenAsync(true);
                if (acc.Length > 0)
                {
                }
                else
                {
                    new JH_Auth_LogB().InsertLog("Application_Start", "更新Access为空", "Global.asax", "System", HostUrl, 0, strIp);
                }
            }
            catch (Exception ex)
            {
                new JH_Auth_LogB().InsertLog("Application_Start", "更新Access错误" + ex.ToString(), "Global.asax", "System", HostUrl, 0, strIp);
            }
        }
        /// <summary>
        /// 立即更新AccessToken
        /// </summary>
        public static string GetToken()
        {
            AccessTokenResult r = CommonApi.GetToken(AppId, AppSecret, "client_credential");
            string _username = CommonHelp.GetUserNameByszhlcode();
            string strIp = CommonHelp.getIP(HttpContext.Current);

            string accesstoken = r.access_token;
            if (accesstoken.Trim().Length > 0)
            {
                CommonHelp.UpdateAppConfig("AccessToken", accesstoken);
                new JH_Auth_LogB().InsertLog("WXFWHelper", "立即更新AccessToken为" + accesstoken, "WXFWHelper", _username, _username, 0, strIp);

                Updatejsapiticket(accesstoken, _username, strIp);
            }
            return accesstoken;
        }
        /// <summary>
        /// 异步更新AccessToken
        /// </summary>
        /// <param name="getNewToken"></param>
        public static string GetTokenAsync(bool getNewToken = false)
        {
            //AccessTokenResult r = CommonApi.GetToken(Qyinfo.corpId, Qyinfo.corpSecret, "client_credential");
            string strIp = CommonHelp.getIP(HttpContext.Current);
            string _username = CommonHelp.GetUserNameByszhlcode();
            var task1 = new Task<string>(() =>
            AccessTokenContainer.TryGetAccessTokenAsync(AppId, AppSecret, getNewToken).Result
            );

            task1.Start();

            string accesstoken = task1.Result;
            if (accesstoken.Trim().Length > 0)
            {
                CommonHelp.UpdateAppConfig("AccessToken", accesstoken);
                new JH_Auth_LogB().InsertLog("WXFWHelper", "更新AccessToken为" + accesstoken, "WXFWHelper", _username, _username, 0, strIp);

                Updatejsapiticket(accesstoken, _username, strIp);
            }
            return accesstoken;
        }
        /// <summary>
        /// 更新jsapi_ticket
        /// </summary>
        public static bool Updatejsapiticket(string accesstoken, string username = "System", string strIp = "")
        {
            bool result = false;
            JsApiTicketResult jsapi_ticket = CommonApi.GetTicketByAccessToken(accesstoken);
            if (jsapi_ticket != null)
            {
                if (jsapi_ticket.ticket.Length > 0)
                {
                    CommonHelp.UpdateAppConfig("jsapi_ticket", jsapi_ticket.ticket);
                    new JH_Auth_LogB().InsertLog("WXFWHelper", "更新jsapi_ticket为" + jsapi_ticket.ticket, "WXFWHelper", username, username, 0, strIp);
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 根据code获取用户OAuth
        /// </summary>
        /// <param name="_code"></param>
        /// <returns></returns>
        public static OAuthAccessTokenResult GetWXUserOAuth(string _code)
        {
            try
            {
                OAuthAccessTokenResult _accresstoken = OAuthApi.GetAccessToken(AppId, AppSecret, _code);
                return _accresstoken;
            }
            catch
            {

            }

            return null;
        }
        /// <summary>
        /// 从公众号中进入获取用户信息
        /// </summary>
        public static UserInfoJson GetWXUserInfo(string _code)
        {
            try
            {
                OAuthAccessTokenResult _accresstoken = OAuthApi.GetAccessToken(AppId, AppSecret, _code);
                return GetUserInfoByOpenid(_accresstoken.openid);
            }
            catch
            {

            }

            return null;
        }
        /// <summary>
        /// 从公众号中进入获取用户信息并更新数据库中账号
        /// </summary>
        public static WX_User GetWXUserInfoWithUpdateLocal(string _code)
        {
            try
            {
                OAuthAccessTokenResult _accresstoken = OAuthApi.GetAccessToken(AppId, AppSecret, _code);
                return GetUserInfoByOpenidWithUpdateLocal(_accresstoken.openid, _accresstoken, _code);
            }
            catch
            {

            }

            return null;
        }
        /// <summary>
        /// 根据公众号openid获取用户信息
        /// </summary>
        public static UserInfoJson GetUserInfoByOpenid(string openid)
        {
            return UserApi.Info(CommonHelp.GetAccessToken(), openid);
        }
        /// <summary>
        /// 直接更新本地数据库中账号 
        /// </summary>
        public static WX_User GetUserInfoByOpenidWithUpdateLocal(string openid, OAuthAccessTokenResult _accresstoken, string _code)
        {
            UserInfoJson u = GetUserInfoByOpenid(openid);
            WX_User wxuser = new WX_User();
            if (u != null && u.errmsg == null)//有用户信息返回
            {
                wxuser = new WX_UserB().GetEntity(d => d.Openid == u.openid);
                return UpdateLocalUserInfo(u, _accresstoken, _code);
            }
            return wxuser;
        }
        /// <summary>
        /// 更新本地数据库中微信用户信息
        /// </summary>
        public static WX_User UpdateLocalUserInfo(UserInfoJson u, OAuthAccessTokenResult _accresstoken, string _code)
        {
            string _accesstokenstr = "";
            int _expires_in = 0;
            string _refreshtokenstr = "";
            string _scopestr = "";
            if (_accresstoken != null && _accresstoken.errmsg == null)
            {
                _accesstokenstr = _accresstoken.access_token;
                _expires_in = _accresstoken.expires_in;
                _refreshtokenstr = _accresstoken.refresh_token;
                _scopestr = _accresstoken.scope;
            }
            WX_User wxuser = new WX_UserB().GetEntity(d => d.Openid == u.openid);
            if (wxuser == null)
            {
                wxuser = new WX_User();
                wxuser.Openid = u.openid;
                wxuser.Nickname = u.nickname;
                wxuser.Sex = u.sex == 1 ? "男" : (u.sex == 2 ? "女" : "未知");
                wxuser.Province = u.province;
                wxuser.City = u.city;
                wxuser.Country = u.country;
                wxuser.HeadImgUrl = u.headimgurl;
                wxuser.Code = _code;
                wxuser.Access_token = _accesstokenstr;
                wxuser.Expires_in = _expires_in;
                wxuser.Refresh_token = _refreshtokenstr;
                wxuser.Scope = _scopestr;
                wxuser.CRDate = DateTime.Now;
                wxuser.LastLoginDate = DateTime.Now;
                wxuser.AuthUserID = 0;
                new WX_UserB().Insert(wxuser);
            }
            else
            {
                wxuser.Nickname = u.nickname;
                wxuser.Sex = u.sex == 1 ? "男" : (u.sex == 2 ? "女" : "未知");
                wxuser.Province = u.province;
                wxuser.City = u.city;
                wxuser.Country = u.country;
                wxuser.HeadImgUrl = u.headimgurl;
                wxuser.Code = _code;
                wxuser.Access_token = _accesstokenstr;
                wxuser.Expires_in = _expires_in;
                wxuser.Refresh_token = _refreshtokenstr;
                wxuser.Scope = _scopestr;
                wxuser.LastLoginDate = DateTime.Now;
                new WX_UserB().Update(wxuser);
            }
            return wxuser;
            //JH_Auth_User localuser = new JH_Auth_UserB().GetEntity(d => d.WXopenid == u.openid && d.IsWX == 1);
            //if (localuser == null)
            //{
            //    localuser = new JH_Auth_User();

            //    localuser.UserName = "wx" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            //    //新用户名随机生成
            //    localuser.UserRealName = u.nickname;
            //    localuser.UserPass = CommonHelp.GetMD5("a123456");
            //    localuser.pccode= EncrpytHelper.Encrypt(localuser.UserName + "@" + localuser.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            //    localuser.ComId = 10334;
            //    localuser.Sex = u.sex == 1 ? "男" : (u.sex == 2 ? "女" : "未知");
            //    localuser.BranchCode = 0;
            //    localuser.CRDate = DateTime.Now;
            //    localuser.CRUser = "administrator";
            //    localuser.logindate = DateTime.Now;
            //    localuser.IsUse = "Y";
            //    localuser.IsWX = 1;
            //    localuser.WXopenid = u.openid;
            //    new JH_Auth_UserB().Insert(localuser);
            //}
            //else
            //{
            //    //localuser.pccode = EncrpytHelper.Encrypt(localuser.UserName + "@" + localuser.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            //    localuser.logindate = DateTime.Now;
            //    new JH_Auth_UserB().Update(localuser);//更新logindate  pccode不能更新
            //}
        }
        /// <summary>
        /// 微信授权登录成功后更新本地账号缓存
        /// </summary>
        public static void UpdateCookieAfterSignIn(JH_Auth_User userInfo)
        {
            DateTime expires = DateTime.Now.AddMinutes(120);
            CommonHelp.SetCookie("szhlcode", userInfo.pccode, expires);
            CommonHelp.SetCookie("username", userInfo.UserName, expires);
            CommonHelp.SetCookie("userphonenumber", userInfo.mobphone, expires);
        }
        /// <summary>
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string EnSha1(string str)
        {
            //大写加密
            //var buffer = Encoding.UTF8.GetBytes(str);
            //var data = SHA1.Create().ComputeHash(buffer);

            //var sb = new StringBuilder();
            //foreach (var t in data)
            //{
            //    sb.Append(t.ToString("X2"));
            //}
            //return sb.ToString();

            //小写加密
            StringBuilder result = new StringBuilder();
            var data = Encoding.UTF8.GetBytes(str);
            SHA1 sha = new SHA1CryptoServiceProvider();
            var resultArr = sha.ComputeHash(data);
            for (int i = 0; i < resultArr.Length; i++)
            {
                var tmp = String.Format("{0:X2}", resultArr[i] & 0xFF);
                if (tmp.Length == 1)
                {
                    result.Append("0");
                }
                result.Append(tmp);
            }
            var passwd = result.ToString();
            StringBuilder lowerstr = new StringBuilder();
            foreach (char c in passwd)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    lowerstr.Append(char.ToLower(c));
                }
                else
                {
                    lowerstr.Append(c);
                }
            }
            string sb = lowerstr.ToString();
            return sb;
        }
    }
}
