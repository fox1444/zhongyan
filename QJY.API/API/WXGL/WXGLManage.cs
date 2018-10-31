using QJY.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using FastReflectionLib;
using System.Data;
using QJY.Data;
using Newtonsoft.Json;
using Senparc.Weixin.QY.Entities;
using System.Collections;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace QJY.API
{
    public class WXGLManage : IWsService
    {
        public void ProcessRequest(HttpContext context, ref Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            MethodInfo methodInfo = typeof(WXGLManage).GetMethod(msg.Action.ToUpper());
            WXGLManage model = new WXGLManage();
            methodInfo.FastInvoke(model, new object[] { context, msg, P1, P2, UserInfo });
        }

        #region 微信管理系统

        public void ADDGROUP(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            WX_Group GP = JsonConvert.DeserializeObject<WX_Group>(P1);
            if (GP == null)
            {
                msg.ErrorMsg = "添加失败";
                return;
            }

            if (string.IsNullOrWhiteSpace(GP.GroupName))
            {
                msg.ErrorMsg = "小组名称不能为空";
                return;
            }

            if (GP.GroupCode == 0)
            {
                GP.CRDate = DateTime.Now;
                GP.CRUser = UserInfo.User.UserName;
                GP.IsDel = 0;
                new WX_GroupB().Insert(GP);
            }
            else
            {
                new WX_GroupB().Update(GP);
            }
            msg.Result = GP;
        }
        public void GETGROUPMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int _GroupCode = int.Parse(P1);
            WX_Group GP = new WX_GroupB().GetEntity(d => d.GroupCode == _GroupCode);

            msg.Result = GP;
        }
        public void GETGROUPLIST(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            DataTable dt = new DataTable();
            int total = 0;
            dt = new WX_GroupB().GetDataPager("WX_Group g left join JH_Auth_User u on g.CRUser=u.UserName", " g.*, u.UserRealName ", 99999, 1, " g.CRDate desc", " g.isDel=0 and g.CRUser='" + UserInfo.User.UserName + "'", ref total);

            if (dt.Rows.Count > 0)
            {

            }
            msg.Result = dt;
            msg.Result1 = total;
        }
        /// <summary>
        /// 获取我所在的自律小组信息
        /// </summary>
        public void GETMYGROUPTEAM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            JH_Auth_User thisuser = new JH_Auth_UserB().GetEntity(d => d.ID == UserInfo.User.ID);
            string viewname = "select U.* , wu.nickName from JH_Auth_User U LEFT JOIN WX_User wu on u.WxOpenid = wu.openid  " +
                "where u.ZiLvXiaoZu is not null and u.ZiLvXiaoZu <> '' and  u.ZiLvXiaoZu = '" + thisuser.ZiLvXiaoZu + "' "
                + "or  ('" + thisuser.ZiLvXiaoZu + "' like (select Items from dbo.Split(REPLACE(u.jianduxiaozu,' ',''),';') where items='" + thisuser.ZiLvXiaoZu + "'))"
                + " order by u.UserOrder, u.IsZuZhang desc, u.UserRealName asc";
            DataTable dt = new JH_Auth_UserB().GetDTByCommand(viewname);

            msg.Result = dt;
            msg.Result1 = thisuser.ZiLvXiaoZu;
        }
        /// <summary>
        /// 根据小组名称获取自律小组信息，包含我所监督的小组
        /// </summary>
        public void GETGROUPTEAMBYCODE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            P1 = P1.Trim();
            string viewname = "select U.* , wu.nickName from JH_Auth_User U LEFT JOIN WX_User wu on u.WxOpenid = wu.openid  " +
                "where u.ZiLvXiaoZu is not null and u.ZiLvXiaoZu <> '' and  u.ZiLvXiaoZu = '" + P1 + "' "
                + "or  ('" + P1 + "' like (select Items from dbo.Split(REPLACE(u.jianduxiaozu,' ',''),';') where items='" + P1 + "'))"
                + " order by u.UserOrder, u.IsZuZhang desc, u.UserRealName asc";
            DataTable dt = new JH_Auth_UserB().GetDTByCommand(viewname);
            msg.Result = dt;
        }
        /// <summary>
        /// 获取我所监督的自律小组列表
        /// </summary>
        public void GETMYJIANDUGROUPTEAM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            //我的自律小组名称
            JH_Auth_User thisuser = new JH_Auth_UserB().GetEntity(d => d.ID == UserInfo.User.ID);
            string viewname = "select Items as XiaoZu from dbo.Split(REPLACE('" + thisuser.JianDuXiaoZu + "',' ',''),';') ";
            DataTable dt = new JH_Auth_UserB().GetDTByCommand(viewname);
            msg.Result = dt;
            msg.Result1 = thisuser.ZiLvXiaoZu;
        }
        /// <summary>
        /// 绑定手机号和姓名
        /// </summary>
        public void BINDPHONE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            JH_Auth_User j = JsonConvert.DeserializeObject<JH_Auth_User>(P1);
            if (j == null)
            {
                msg.ErrorMsg = "绑定失败";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.UserRealName.Trim()))
            {
                msg.ErrorMsg = "姓名不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.mobphone.Trim()))
            {
                msg.ErrorMsg = "手机号不能为空";
                return;
            }
            string _openid = CommonHelp.GetCookieString("openid");
            WX_User u = new WX_UserB().GetEntity(d => d.Openid == _openid);
            msg.Result = u;
            if (u != null)
            {
                JH_Auth_User localuser = new JH_Auth_UserB().GetEntity(d => d.mobphone == j.mobphone.Trim());
                if (localuser == null)
                {
                    //新用户，随机生成
                    localuser = new JH_Auth_User();
                    localuser.UserName = "wx" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                    localuser.UserRealName = j.UserRealName;
                    localuser.UserPass = CommonHelp.GetMD5("a123456");
                    localuser.pccode = EncrpytHelper.Encrypt(localuser.UserName + "@" + localuser.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    localuser.ComId = 10334;
                    localuser.Sex = u.Sex;
                    localuser.mobphone = j.mobphone;
                    localuser.BranchCode = 0;
                    localuser.CRDate = DateTime.Now;
                    localuser.CRUser = "System";
                    localuser.logindate = DateTime.Now;
                    localuser.IsUse = "Y";
                    localuser.IsWX = 1;
                    localuser.WXopenid = _openid;
                    localuser.weixinCard = j.weixinCard;
                    new JH_Auth_UserB().Insert(localuser);

                    WXFWHelp.UpdateCookieAfterSignIn(localuser);
                    //msg.ErrorMsg = "手机号不存在，请联系管理员";
                    return;
                }
                else
                {
                    //老用户
                    if (localuser.UserRealName == j.UserRealName)
                    {
                        new JH_Auth_UserB().ExsSql("update JH_Auth_User set WXopenid='', IsWX=0  where WXopenid='" + _openid + "'");//清除以前绑定的用户

                        localuser.WXopenid = _openid;
                        localuser.IsWX = 1;
                        localuser.weixinCard = j.weixinCard;
                        localuser.pccode = EncrpytHelper.Encrypt(localuser.UserName + "@" + localuser.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        localuser.logindate = DateTime.Now;
                        new JH_Auth_UserB().Update(localuser);//更新logindate  pccode不能更新

                        WXFWHelp.UpdateCookieAfterSignIn(localuser);
                        msg.Result = localuser;
                    }
                    else
                    {
                        msg.ErrorMsg = "姓名与手机号不匹配";
                        return;
                    }
                }
            }
            else
            {
                msg.ErrorMsg = "登录异常";
                return;
            }
        }
        /// <summary>
        /// 绑定手机、姓名、身份证、专卖许可证
        /// </summary>
        public void BINDTOMONOLICENSE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            JH_Auth_User j = JsonConvert.DeserializeObject<JH_Auth_User>(P1);
            if (j == null)
            {
                msg.ErrorMsg = "绑定失败";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.UserRealName.Trim()))
            {
                msg.ErrorMsg = "姓名不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.mobphone.Trim()))
            {
                msg.ErrorMsg = "手机号不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.IDCard.Trim()))
            {
                msg.ErrorMsg = "身份证号不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.ToMonoLicense.Trim()))
            {
                msg.ErrorMsg = "专卖许可证号不能为空";
                return;
            }
            string _openid = CommonHelp.GetCookieString("openid");
            WX_User u = new WX_UserB().GetEntity(d => d.Openid == _openid);
            msg.Result = u;
            if (u != null)
            {
                JH_Auth_User localuser = new JH_Auth_UserB().GetEntity(d => d.mobphone == j.mobphone.Trim());
                if (localuser == null)
                {
                    new JH_Auth_UserB().ExsSql("update JH_Auth_User set WXopenid='', IsWX=0, IDCard='',ToMonoLicense='' where WXopenid='" + _openid + "'");//清除以前绑定的用户
                    //新用户，随机生成
                    localuser = new JH_Auth_User();
                    localuser.UserName = "wx" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                    localuser.UserRealName = j.UserRealName.Trim();
                    localuser.UserPass = CommonHelp.GetMD5("a123456");
                    localuser.pccode = EncrpytHelper.Encrypt(localuser.UserName + "@" + localuser.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    localuser.ComId = 10334;
                    localuser.Sex = u.Sex;
                    localuser.mobphone = j.mobphone.Trim();
                    localuser.BranchCode = 0;
                    localuser.CRDate = localuser.logindate = DateTime.Now;
                    localuser.CRUser = "System";
                    localuser.IsUse = "Y";
                    localuser.IsWX = 1;
                    localuser.WXopenid = _openid;
                    localuser.weixinCard = j.weixinCard.Trim();
                    localuser.IDCard = j.IDCard.Trim();
                    localuser.ToMonoLicense = j.ToMonoLicense.Trim();

                    new JH_Auth_UserB().Insert(localuser);
                    WXFWHelp.UpdateCookieAfterSignIn(localuser);
                    msg.Result = localuser;
                    //msg.ErrorMsg = "手机号不存在，请联系管理员";
                    return;
                }
                else
                {
                    //老用户
                    if (localuser.UserRealName == j.UserRealName.Trim())
                    {
                        new JH_Auth_UserB().ExsSql("update JH_Auth_User set WXopenid='', IsWX=0, IDCard='',ToMonoLicense='' where WXopenid='" + _openid + "'");//清除以前绑定的用户

                        localuser.WXopenid = _openid;
                        localuser.IsWX = 1;
                        localuser.weixinCard = j.weixinCard.Trim();
                        //localuser.pccode = EncrpytHelper.Encrypt(localuser.UserName + "@" + localuser.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        localuser.logindate = DateTime.Now;
                        localuser.IDCard = j.IDCard.Trim();
                        localuser.ToMonoLicense = j.ToMonoLicense.Trim();

                        new JH_Auth_UserB().Update(localuser);//更新logindate，pccode不能更新
                        WXFWHelp.UpdateCookieAfterSignIn(localuser);
                        msg.Result = localuser;
                    }
                    else
                    {
                        msg.ErrorMsg = "姓名与手机号不匹配";
                        return;
                    }
                }
            }
            else
            {
                msg.ErrorMsg = "微信登录异常";
                return;
            }
        }
        public void VALIDATETOMONOLICENSE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            JH_Auth_User j = JsonConvert.DeserializeObject<JH_Auth_User>(P1);
            if (j == null)
            {
                msg.ErrorMsg = "绑定失败";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.UserRealName.Trim()))
            {
                msg.ErrorMsg = "姓名不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.mobphone.Trim()))
            {
                msg.ErrorMsg = "手机号不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.IDCard.Trim()))
            {
                msg.ErrorMsg = "身份证号不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(j.ToMonoLicense.Trim()))
            {
                msg.ErrorMsg = "专卖许可证号不能为空";
                return;
            }
            string url = "http://order.lstobacco.com:5222/tabacco/logistic/validateCustInfo";
            Dictionary<String, String> DATA = new Dictionary<String, String>();
            DATA.Add("licenseCode", j.ToMonoLicense.Trim());
            DATA.Add("idCard", j.IDCard.Trim());
            DATA.Add("userName", j.UserRealName.Trim());
            try
            {
                HttpWebResponse ResponseData = CommonHelp.CreatePostHttpResponse(url, DATA, 0, "", null);
                string Returndata = CommonHelp.GetResponseString(ResponseData);
                JObject json = (JObject)JsonConvert.DeserializeObject(Returndata);
                msg.Result = json;
            }
            catch (Exception e)
            {
                msg.ErrorMsg = "验证失败，请检查您的信息！";
            }
        }
        public void GETUSERINFOBYOPENID(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string _openid = CommonHelp.GetCookieString("openid");
            WX_User u = new WX_UserB().GetEntity(d => d.Openid == _openid);
            if (u != null)
            {
                JH_Auth_User localuser = new JH_Auth_UserB().GetEntity(d => d.WXopenid == _openid && d.IsWX == 1);
                if (localuser != null)
                {
                    msg.Result = localuser;
                }
            }
        }
        /// <summary>
        /// 我的群，根据科室/职务来划分
        /// </summary>
        public void GETMYCROWD(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            //我的自律小组名称
            JH_Auth_User thisuser = new JH_Auth_UserB().GetEntity(d => d.ID == UserInfo.User.ID);
            string viewname = "select u.* , wu.nickName, b.DeptName from JH_Auth_User U LEFT JOIN WX_User wu on u.WxOpenid = wu.openid  "
                + " left join JH_Auth_Branch b on u.BranchCode=b.DeptCode "
                + " where u.BranchCode=" + thisuser.BranchCode + " and  u.UserGW = '" + thisuser.UserGW + "' "
                + " order by u.UserOrder, u.UserRealName asc";

            DataTable dt = new JH_Auth_UserB().GetDTByCommand(viewname);

            msg.Result = dt;
            msg.Result1 = thisuser.UserGW;
            if (dt.Rows.Count > 0)
                msg.Result2 = thisuser.UserGW + "-" + dt.Rows[0]["DeptName"].ToString();
        }
        public void ADDRYMODELWX(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            WX_RY model = JsonConvert.DeserializeObject<WX_RY>(P2);
            if (model == null)
            {
                msg.ErrorMsg = "添加失败";
                return;
            }
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                msg.ErrorMsg = "标题不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                msg.ErrorMsg = "内容不能为空";
                return;
            }
            if (model.ID == 0)
            {
                model.CRDate = DateTime.Now;
                model.CRUser = UserInfo.User.UserName;
                model.ZiLvXiaoZu = UserInfo.User.ZiLvXiaoZu;
                new WX_RYB().Insert(model);
            }
            else
            {
                new WX_RYB().Update(model);
            }

        }
        public void GETRYLIST(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            DataTable dt = new DataTable();
            int total = 0;
            string whestr = " ZiLvXiaoZu='" + UserInfo.User.ZiLvXiaoZu + "' " +
                "or  ZiLvXiaoZu in (select Items from dbo.Split(REPLACE('" + UserInfo.User.JianDuXiaoZu + "',' ',''),';'))";
            dt = new WX_GroupB().GetDataPager("WX_RY ", " * ", 99999, 1, " CRDate desc", whestr, ref total);

            msg.Result = dt;
            msg.Result1 = total;
            msg.Result2 = UserInfo.User.IsZuZhang;
        }
        public void GETRYMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);

            WX_RY model = new WX_RYB().GetEntity(p => p.ID == Id);
            msg.Result = model;
            if (UserInfo.User.UserName == model.CRUser)
                msg.Result1 = 1;
            else
                msg.Result1 = 0;
        }
        public void DELRYMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            WX_RY model = new WX_RYB().GetEntity(p => p.ID == Id);
            if (UserInfo.User.UserName == model.CRUser)
            {
                new WX_RYB().Delete(model);
            }
            else
            {
                msg.ErrorMsg = "您没有权限删除";
            }
        }
        public void ADDHUODONGMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            WX_HD model = JsonConvert.DeserializeObject<WX_HD>(P2);
            if (model == null)
            {
                msg.ErrorMsg = "添加失败";
                return;
            }
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                msg.ErrorMsg = "标题不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                msg.ErrorMsg = "内容不能为空";
                return;
            }
            if (model.ID == 0)
            {
                model.CRDate = DateTime.Now;
                model.CRUser = UserInfo.User.UserName;
                model.ZiLvXiaoZu = UserInfo.User.ZiLvXiaoZu;
                new WX_HDB().Insert(model);
            }
            else
            {
                new WX_HDB().Update(model);
            }

        }
        public void GETHUODONGLIST(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            DataTable dt = new DataTable();
            int total = 0;
            string whestr = " ZiLvXiaoZu='" + UserInfo.User.ZiLvXiaoZu + "' " +
                "or  ZiLvXiaoZu in (select Items from dbo.Split(REPLACE('" + UserInfo.User.JianDuXiaoZu + "',' ',''),';'))";
            dt = new WX_GroupB().GetDataPager("WX_HD ", " * ", 99999, 1, " CRDate desc", whestr, ref total);

            msg.Result = dt;
            msg.Result1 = total;
            msg.Result2 = UserInfo.User.IsZuZhang;

        }
        public void GETHUODONGMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);

            WX_HD model = new WX_HDB().GetEntity(p => p.ID == Id);
            msg.Result = model;
            if (UserInfo.User.UserName == model.CRUser)
                msg.Result1 = 1;
            else
                msg.Result1 = 0;
        }
        public void GETMESSAGEHISTORY(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            DataTable dt = new DataTable();
            int total = 0;
            int page = 0;
            int pagecount = 50;
            int.TryParse(context.Request.QueryString["p"] ?? "1", out page);
            int.TryParse(context.Request.QueryString["pagecount"] ?? "8", out pagecount);//页数
            string whestr = "GroupName='" + P1 + "'" + " and id in (select top 100 id from[Message] where groupname = '" + P1 + "' order by CRDate desc )";
            dt = new WX_GroupB().GetDataPager("MESSAGE ", " * ", 99999, page, " CRDate", whestr, ref total);

            msg.Result = dt;
            msg.Result1 = total;
        }
        public void INSERTMESSAGE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {

            if (string.IsNullOrWhiteSpace(P1))
            {
                msg.ErrorMsg = "内容不能为空";
                return;
            }
            if (string.IsNullOrWhiteSpace(P2))
            {
                msg.ErrorMsg = "小组不存在";
                return;
            }
            Message model = new Message();
            if (model.ID == 0)
            {
                model.Content = P1;
                model.CRDate = DateTime.Now;
                model.CRUser = UserInfo.User.UserName;
                model.CRUserRealName = UserInfo.User.UserRealName;
                model.GroupName = P2;
                model.Status = 0;
                new MessageB().Insert(model);
            }
            msg.Result = model;
        }
        public void DELHUODONGMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            WX_HD model = new WX_HDB().GetEntity(p => p.ID == Id);
            if (UserInfo.User.UserName == model.CRUser)
            {
                new WX_HDB().Delete(model);
            }
            else
            {
                msg.ErrorMsg = "您没有权限删除";
            }
        }
        #endregion

        #region 获取微信初始化参数
        public void GETWXCONFIG(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
            long timestamp = Convert.ToInt64(ts.TotalSeconds);
            string appId = CommonHelp.GetConfig("AppId");
            string jsapi_ticket = CommonHelp.AppConfig("jsapi_ticket");
            string nonceStr = CommonHelp.GetRandomString(15);
            string url = P1;

            string TOP_FIELD_SIGN = "";
            param.Add("timestamp", timestamp.ToString());
            param.Add("jsapi_ticket", jsapi_ticket);
            param.Add("noncestr", nonceStr);
            param.Add("url", url);

            SortedDictionary<string, string> dic = new SortedDictionary<string, string>(param);
            IEnumerator<KeyValuePair<string, string>> em = dic.GetEnumerator();
            // 第1步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            while (em.MoveNext())
            {
                string key = em.Current.Key;
                if (!TOP_FIELD_SIGN.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    string value = em.Current.Value;
                    if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(key))
                    {
                        query.Append("&").Append(key).Append("=").Append(value);
                    }
                }
            }
            query = new StringBuilder(query.ToString().TrimStart('&'));
            string querystring = HttpUtility.HtmlDecode(query.ToString());
            // 第2步：使用sha1加密
            string signature = WXFWHelp.EnSha1(querystring);
            msg.Result = new
            {
                appId = appId,
                //jsapi_ticket = jsapi_ticket,
                timestamp = timestamp,
                noncestr = nonceStr,
                url = url,
                //query = querystring,
                signature = signature
            };
        }
        #endregion
    }
}