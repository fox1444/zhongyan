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
    public class FSGLManage : IWsService
    {
        public void ProcessRequest(HttpContext context, ref Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            MethodInfo methodInfo = typeof(FSGLManage).GetMethod(msg.Action.ToUpper());
            FSGLManage model = new FSGLManage();
            methodInfo.FastInvoke(model, new object[] { context, msg, P1, P2, UserInfo });
        }

        /// <summary>
        /// 粉丝列表分页
        /// </summary>
        public void GETFSLIST_PAGE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string strWhere = " 1=1 ";
            if (P1 != "")
            {
                strWhere += string.Format(" and wu.Nickname like '%{0}%'", P1);
            }
            string colNme = @"wu.*, u.UserRealName, u.IsWX, u.mobphone, u.IDCard, u.ToMonoLicense ";
            string tableName = string.Format(@" WX_User wu left join JH_Auth_User u on wu.Openid=u.WXOpenid ");

            int page = 0;
            int pagecount = 10;
            int.TryParse(context.Request["p"] ?? "1", out page);
            int.TryParse(context.Request["pagecount"] ?? "10", out pagecount);//页数
            page = page == 0 ? 1 : page;
            int total = 0;
            DataTable dt = new WX_UserB().GetDataPager(tableName, colNme, pagecount, page, " wu.CRDate desc", strWhere, ref total);
            msg.Result = dt;
            msg.Result1 = total;
        }

        /// <summary>
        /// 粉丝详情
        /// </summary>
        public void GETFSGLMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            string strWhere = " wu.ID=" + Id;
            string colNme = @"wu.*, u.UserRealName, u.IsWX, u.mobphone, u.IDCard, u.ToMonoLicense ";
            string tableName = string.Format(@" WX_User wu left join JH_Auth_User u on wu.Openid=u.WXOpenid ");

            string strSql = string.Format("Select {0}  From {1} where {2} order by wu.CRDate desc", colNme, tableName, strWhere);
            DataTable dt = new SZHL_ZCGLB().GetDTByCommand(strSql);
            msg.Result = dt;
        }            
    }
}