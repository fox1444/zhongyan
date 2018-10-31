using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Web.SessionState;
using QJY.API;
using QJY.Data;
using System.Diagnostics;

namespace QJY.WEB
{
    /// <summary>
    /// VIEWAPINOLOGIN 的摘要说明
    /// </summary>
    public class VIEWAPINOLOGIN : IHttpHandler, IRequiresSessionState
    {
        public string ComId { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Methods", "POST,GET,OPTIONS,DELETE"); //支持的http 动作
            context.Response.AddHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type,authorization");
            context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            string strAction = context.Request["Action"] ?? "";
            string P1 = context.Request["P1"] ?? "";
            string P2 = context.Request["P2"] ?? "";
            string P3 = context.Request["P3"] ?? "";
            string UserName = context.Request["UserName"] ?? "";
            string szhlcode = context.Request["szhlcode"] ?? "";
            string authcode = context.Request.Headers["Authorization"] ?? "";

            string strIP = CommonHelp.getIP(context);//用户IP
            int intTimeOut = 60;//用户超时间隔时间即szhlcode失效时间
            Msg_Result Model = new Msg_Result() { Action = strAction.ToUpper(), ErrorMsg = "" };

            if (!string.IsNullOrEmpty(strAction))
            {
                try
                {
                    string strCheckString = new CommonHelp().checkconetst(context);
                    if (strCheckString != "")
                    {
                        Model.ErrorMsg = strAction + "有敏感字符串";
                        new JH_Auth_LogB().InsertLog(strAction, Model.ErrorMsg, strCheckString, UserName, "", 0, strIP);
                    }
                    else
                    {
                        #region 必须登录执行接口
                        Model.ErrorMsg = "";

                        var bl = true;
                        var acs = Model.Action.Split('_');
                        //if (Model.Action.IndexOf("_") > 0)
                        //{
                        //    if (acs[0].ToUpper() == "Commanage".ToUpper())
                        //    {
                        //        bl = false;
                        //        var container = ServiceContainerV.Current().Resolve<IWsService>(acs[0].ToUpper());
                        //        Model.Action = acs[1];
                        //        container.ProcessRequest(context, ref Model, P1.TrimEnd(), P2.TrimEnd(), new JH_Auth_UserB.UserInfo());
                        //        int cid = 0;
                        //        string un = string.Empty;
                        //        if (Model.Result4 != null)
                        //        {
                        //            JH_Auth_User UserInfo = Model.Result4;
                        //            cid = UserInfo.ComId.Value;
                        //            un = UserInfo.UserRealName;
                        //        }

                        //    }
                        //}
                        if (bl)
                        {
                            var container = ServiceContainerV.Current().Resolve<IWsService>(acs[0].ToUpper());
                            Model.Action = Model.Action.Substring(acs[0].Length + 1);
                            container.ProcessRequest(context, ref Model, P1.TrimEnd(), P2.TrimEnd(), new JH_Auth_UserB.UserInfo());
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Model.ErrorMsg = strAction + "接口调用失败,请检查日志";
                    Model.Result = ex.ToString();
                    new JH_Auth_LogB().InsertLog(strAction, Model.ErrorMsg + ex.StackTrace.ToString(), ex.ToString(), UserName, "", 0, strIP);

                }
            }
            string jsonpcallback = context.Request["jsonpcallback"] ?? "";
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string Result = JsonConvert.SerializeObject(Model, Formatting.Indented, timeConverter).Replace("null", "\"\"");
            if (jsonpcallback != "")
            {
                Result = jsonpcallback + "(" + Result + ")";//支持跨域
            }

            context.Response.Write(Result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}