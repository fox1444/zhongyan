using FastReflectionLib;
using Newtonsoft.Json;
using QJY.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web;

namespace QJY.API
{
    public class ZCGLManage : IWsService
    {
        public void ProcessRequest(HttpContext context, ref Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            MethodInfo methodInfo = typeof(ZCGLManage).GetMethod(msg.Action.ToUpper());
            ZCGLManage model = new ZCGLManage();
            methodInfo.FastInvoke(model, new object[] { context, msg, P1, P2, UserInfo });
        }

        #region 资产管理
        /// <summary>
        /// 资产列表
        /// </summary>
        public void GETZCGLLIST_PAGE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int page = 0;
            int pagecount = 10;
            int.TryParse(context.Request["p"] ?? "1", out page);
            int.TryParse(context.Request["pagecount"] ?? "10", out pagecount);//页数
            page = page == 0 ? 1 : page;
            int total = 0;

            string strWhere = " z.IsDel=0 ";

            string typeid = context.Request["typeid"] ?? "";
            string branchcode = context.Request["branchcode"] ?? "";
            string usergw = context.Request["usergw"] ?? "";
            string locationid = context.Request["locationid"] ?? "";
            string status = context.Request["status"] ?? "";
            string lifecycleid = context.Request["lifecycleid"] ?? "";
            string searchstr = context.Request["searchstr"] ?? "";
            searchstr = searchstr.TrimEnd();

            if (!string.IsNullOrEmpty(typeid))
            {
                strWhere += string.Format(" And z.TypeID='{0}' ", typeid);
            }
            if (!string.IsNullOrEmpty(branchcode))
            {
                strWhere += string.Format(" And z.BranchCode='{0}' ", branchcode);
            }
            if (!string.IsNullOrEmpty(usergw))
            {
                strWhere += string.Format(" And z.UserGW='{0}' ", usergw);
            }
            if (!string.IsNullOrEmpty(locationid))
            {
                strWhere += string.Format(" And z.LocationID='{0}' ", locationid);
            }
            if (!string.IsNullOrEmpty(status))
            {
                strWhere += string.Format(" And z.Status='{0}' ", status);
            }
            if (!string.IsNullOrEmpty(searchstr))
            {
                strWhere += string.Format(" And ( z.Name like '%{0}%'  or z.Code like '%{0}%')", searchstr);
            }
            if (!string.IsNullOrEmpty(lifecycleid))
            {
                strWhere += string.Format("  and z.ID in ( select ZCGLID from SZHL_ZCGL_LifeCycle where TypeID= '{0}' and IsDel=0 )", lifecycleid);
            }
            //int DataID = -1;
            //int.TryParse(context.Request.QueryString["ID"] ?? "-1", out DataID);//记录Id
            //if (DataID != -1)
            //{
            //    string strIsHasDataQX = new JH_Auth_QY_ModelB().ISHASDATAREADQX("ZCGL", DataID, UserInfo);
            //    if (strIsHasDataQX == "Y")
            //    {
            //        strWhere += string.Format(" And z.ID = '{0}'", DataID);
            //    }
            //}

            DataTable dt = new DataTable();

            dt = new SZHL_ZCGLB().GetDataPager("SZHL_ZCGL z left join SZHL_ZCGL_Type t on z.TypeID=t.ID left join SZHL_ZCGL_Location l on z.LocationID=l.ID left join JH_Auth_User u on z.UserName=u.UserName left join  JH_Auth_Branch b on b.DeptCode=z.BranchCode", "z.*, u.UserRealName, t.Title, l.Title as LocTitle, ISNULL(t.AllowLifeCycle,0) as AllowLifeCycle", pagecount, page, "b.DeptShort desc, u.UserRealName asc, z.CRDate desc", strWhere, ref total);

            msg.Result = dt;
            msg.Result1 = total;

        }
        /// <summary>
        /// 当前用户的资产列表
        /// </summary>
        public void GETZCGLLISTTHISUSER(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string userName = UserInfo.User.UserName;
            string strWhere = " z.IsDel=0 and z.ComId=" + UserInfo.User.ComId + " and z.UserName='" + userName + "'";

            string typeid = context.Request["typeid"] ?? "";
            if (typeid != "")
            {
                strWhere += string.Format(" And z.TypeID='{0}' ", typeid);
            }
            string searchstr = context.Request["searchstr"] ?? "";
            searchstr = searchstr.TrimEnd();
            if (searchstr != "")
            {
                strWhere += string.Format(" And ( z.Name like '%{0}%'  or z.Code like '%{0}%')", searchstr);
            }
            int DataID = -1;
            int.TryParse(context.Request.QueryString["ID"] ?? "-1", out DataID);//记录Id
            if (DataID != -1)
            {
                string strIsHasDataQX = new JH_Auth_QY_ModelB().ISHASDATAREADQX("ZCGL", DataID, UserInfo);
                if (strIsHasDataQX == "Y")
                {
                    strWhere += string.Format(" And z.ID = '{0}'", DataID);
                }
            }

            int page = 0;
            int pagecount = 10;
            int.TryParse(context.Request.QueryString["p"] ?? "1", out page);
            int.TryParse(context.Request.QueryString["pagecount"] ?? "10", out pagecount);//页数
            page = page == 0 ? 1 : page;
            int total = 0;

            DataTable dt = new DataTable();

            dt = new SZHL_ZCGLB().GetDataPager("SZHL_ZCGL z left join SZHL_ZCGL_Type t on z.TypeID=t.ID left join SZHL_ZCGL_Location l on z.LocationID=l.ID left join JH_Auth_User u on z.UserName=u.UserName left join JH_Auth_Branch b on z.BranchCode=b.DeptCode", "z.*, b.DeptName,  u.UserRealName, t.Title, l.Title as LocTitle", pagecount, page, " z.CRDate desc", strWhere, ref total);

            msg.Result = dt;
            msg.Result1 = total;

        }
        /// <summary>
        /// 资产详细信息
        /// </summary>
        public void GETZCGLMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            string strWhere = " z.IsDel=0  and z.ID=" + Id;
            string colNme = @"z.*, t.Title, l.Title as LocTitle, b.DeptName,u.UserRealName ";
            string tableName = string.Format(@" SZHL_ZCGL z left join SZHL_ZCGL_Type t on z.TypeID=t.ID left join SZHL_ZCGL_Location l on z.LocationID=l.ID left join JH_Auth_Branch b on z.BranchCode=b.DeptCode left join JH_Auth_User u on z.UserName=u.UserName");

            string strSql = string.Format("Select {0}  From {1} where {2} order by z.CRDate desc", colNme, tableName, strWhere);
            DataTable dt = new SZHL_ZCGLB().GetDTByCommand(strSql);
            msg.Result = dt;
        }
        /// <summary>
        /// 添加资产
        /// </summary>
        public void ADDZCGL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            SZHL_ZCGL ZC = JsonConvert.DeserializeObject<SZHL_ZCGL>(P1);

            if (ZC == null)
            {
                msg.ErrorMsg = "添加失败";
                return;
            }
            if (string.IsNullOrWhiteSpace(ZC.Name))
            {
                msg.ErrorMsg = "名称不能为空！";
                return;
            }
            if (ZC.TypeID <= 0)
            {
                msg.ErrorMsg = "请选择资产类型！";
                return;
            }
            if (ZC.Status < 0)
            {
                msg.ErrorMsg = "请选择物品状态！";
                return;
            }
            if (ZC.Qty <= 0)
            {
                msg.ErrorMsg = "请输入数量！";
                return;
            }
            if (ZC.ID == 0)
            {
                ZC.CRDate = DateTime.Now;
                ZC.CRUser = UserInfo.User.UserName;
                ZC.ComId = UserInfo.User.ComId.Value;
                ZC.IsDel = 0;
                ZC.IndexID = 0;
                new SZHL_ZCGLB().Insert(ZC);
            }
            else
            {
                new SZHL_ZCGLB().Update(ZC);
            }
            msg.Result = ZC;
        }
        /// <summary>
        /// 删除资产
        /// </summary>
        public void DELZCGLMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            SZHL_ZCGL model = new SZHL_ZCGLB().GetEntity(d => d.ID == Id && d.ComId == UserInfo.User.ComId);
            model.IsDel = 1;
            new SZHL_ZCGLB().Update(model);
        }
        /// <summary>
        /// 岗位/部门列表，并包含资产数量
        /// </summary>
        public void GETALLGWBYBRANCHCODE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = "";
            if (!string.IsNullOrEmpty(P2))
            {
                where += " and z.TypeID='" + P2 + "' ";
            }
            string status = context.Request["status"] ?? "";
            if (!string.IsNullOrEmpty(status))
            {
                where += string.Format(" And z.Status='{0}' ", status);
            }
            DataTable dt = new JH_Auth_BranchB().GetDTByCommand("select distinct(UserGW),(select COUNT(0) from dbo.SZHL_ZCGL z where z.IsDel=0 and z.UserGW=u.UserGW " + where + ") as ZCNum from JH_Auth_User u  where branchcode='" + P1 + "' and IsUse ='Y' and (select COUNT(0) from dbo.SZHL_ZCGL z where z.IsDel=0 and z.UserGW=u.UserGW " + where + ")>0 order by UserGW");
            msg.Result = dt;
        }
        #endregion

        #region 资产类型管理
        /// <summary>
        /// 所有资产类型列表
        /// </summary>
        public void GETZCGLTYPELIST(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            //var list = new SZHL_ZCGL_TypeB().GetEntities(p => p.IsDel == 0);
            DataTable dt = new SZHL_ZCGL_TypeB().GetDTByCommand("select * from dbo.SZHL_ZCGL_Type where IsDel=0 and ComId=" + UserInfo.User.ComId + " order by DisplayOrder");
            msg.Result = dt;
        }
        /// <summary>
        /// 所有资产类型列表，并展示资产数量
        /// </summary>
        public void GETZCGLTYPELISTWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = "";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.UserGW='" + P1 + "' ";
            }
            if (!string.IsNullOrEmpty(P2))
            {
                where += " and z.BranchCode='" + P2 + "' ";
            }
            string status = context.Request["status"] ?? "";
            if (!string.IsNullOrEmpty(status))
            {
                where += string.Format(" And z.Status='{0}' ", status);
            }
            DataTable dt = new SZHL_ZCGL_TypeB().GetDTByCommand("select *,(select COUNT(0) from dbo.SZHL_ZCGL z where z.IsDel=0" + where + " and z.TypeID=t.ID) as ZCNum from dbo.SZHL_ZCGL_Type t where IsDel=0 and ComId=" + UserInfo.User.ComId + " and (select COUNT(0) from dbo.SZHL_ZCGL z where z.IsDel=0" + where + " and z.TypeID=t.ID)>0 order by DisplayOrder");
            msg.Result = dt;
        }
        /// <summary>
        /// 根据场地获取所有资产类型列表，并展示资产数量
        /// </summary>
        public void GETZCGLTYPELISTBYLOCATIONWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = " z.IsDel=0 ";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.LocationID='" + P1 + "' ";
            }
            if (!string.IsNullOrEmpty(P2))
            {
                where += " and z.BranchCode= '" + P2 + "' ";
            }
            string status = context.Request["status"] ?? "";
            if (!string.IsNullOrEmpty(status))
            {
                where += string.Format(" And z.Status='{0}' ", status);
            }
            DataTable dt = new SZHL_ZCGL_TypeB().GetDTByCommand("select *,(select COUNT(0) from dbo.SZHL_ZCGL z where " + where + " and z.TypeID=t.ID) as ZCNum from dbo.SZHL_ZCGL_Type t where IsDel=0 and ComId=" + UserInfo.User.ComId + " and (select COUNT(0) from dbo.SZHL_ZCGL z where " + where + " and z.TypeID=t.ID)>0 order by DisplayOrder");
            msg.Result = dt;
        }
        /// <summary>
        /// 根据生命周期获取所有资产类型列表，并展示资产数量
        /// </summary>
        public void GETZCGLTYPELISTBYLIFECYCLEWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = " z.IsDel=0 ";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.ID in ( select ZCGLID from SZHL_ZCGL_LifeCycle where TypeID= '" + P1 + "' and IsDel=0 ) ";
            }
            string status = context.Request["status"] ?? "";
            if (!string.IsNullOrEmpty(status))
            {
                where += string.Format(" And z.Status='{0}' ", status);
            }
            DataTable dt = new SZHL_ZCGL_TypeB().GetDTByCommand("select *,(select COUNT(0) from dbo.SZHL_ZCGL z where " + where + " and z.TypeID=t.ID) as ZCNum from dbo.SZHL_ZCGL_Type t where IsDel=0 and ComId=" + UserInfo.User.ComId + " and (select COUNT(0) from dbo.SZHL_ZCGL z where " + where + " and z.TypeID=t.ID)>0 order by DisplayOrder");
            msg.Result = dt;
        }
        /// <summary>
        /// 资产类型分页列表
        /// </summary>  
        public void GETZCGLTYPELIST_PAGE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string strWhere = " cc.ComId=" + UserInfo.User.ComId;
            if (P1 != "")
            {
                strWhere += string.Format(" And cc.Title like '%{0}%'", P1);
            }

            int page = 0;
            int pagecount = 10;
            int.TryParse(context.Request["p"] ?? "1", out page);
            int.TryParse(context.Request["pagecount"] ?? "10", out pagecount);//页数
            page = page == 0 ? 1 : page;
            int total = 0;
            DataTable dt = new SZHL_ZCGL_TypeB().GetDataPager(" SZHL_ZCGL_Type cc", "cc.*", pagecount, page, " cc.DisplayOrder", strWhere, ref total);

            msg.Result = dt;
            msg.Result1 = total;
        }
        /// <summary>
        /// 资产类型详细信息
        /// </summary>
        public void GETTYPEMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            SZHL_ZCGL_Type model = new SZHL_ZCGL_TypeB().GetEntity(d => d.ID == Id && d.ComId == UserInfo.User.ComId);
            msg.Result = model;
        }

        public void GETTYPEMODELBYZCGLID(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int ZCGLId = int.Parse(P1);
            string strWhere = " t.IsDel=0  and z.ID=" + ZCGLId;
            string colNme = @"t.* ";
            string tableName = string.Format(@" SZHL_ZCGL_Type t left join SZHL_ZCGL z on z.TypeID=t.ID and z.IsDel=0  ");

            string strSql = string.Format("Select {0}  From {1} where {2} ", colNme, tableName, strWhere);
            DataTable dt = new SZHL_ZCGL_TypeB().GetDTByCommand(strSql);
            msg.Result = dt;
        }

        /// <summary>
        /// 添加资产类型
        /// </summary>
        public void ADDTYPE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            SZHL_ZCGL_Type t = JsonConvert.DeserializeObject<SZHL_ZCGL_Type>(P1);

            if (string.IsNullOrEmpty(t.Title))
            {
                msg.ErrorMsg = "类型名称不能为空!";
            }

            if (string.IsNullOrEmpty(msg.ErrorMsg))
            {
                if (t.ID == 0)
                {
                    var t1 = new SZHL_ZCGL_TypeB().GetEntity(p => p.ComId == UserInfo.User.ComId && p.Title == t.Title);
                    if (t1 != null)
                    {
                        msg.ErrorMsg = "系统已经存在此类型名称!";
                    }
                    else
                    {
                        t.CRDate = DateTime.Now;
                        t.CRUser = UserInfo.User.UserName;
                        t.ComId = Convert.ToInt16(UserInfo.User.ComId);
                        t.IsDel = 0;
                        new SZHL_ZCGL_TypeB().Insert(t);
                        msg.Result = t;
                    }
                }
                else
                {
                    var hys1 = new SZHL_ZCGL_TypeB().GetEntity(p => p.ComId == UserInfo.User.ComId && p.Title == t.Title && p.ID != t.ID);
                    if (hys1 != null)
                    {
                        msg.ErrorMsg = "系统已经存在此类型名称";
                    }
                    else
                    {
                        new SZHL_ZCGL_TypeB().Update(t);
                        msg.Result = t;
                    }
                }

            }
        }
        /// <summary>
        /// 删除资产类型
        /// </summary>
        public void DELZCGLTYPE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            int ss = int.Parse(P2);
            SZHL_ZCGL_Type model = new SZHL_ZCGL_TypeB().GetEntity(d => d.ID == Id && d.ComId == UserInfo.User.ComId);
            model.IsDel = ss;
            new SZHL_ZCGL_TypeB().Update(model);

        }
        #endregion

        #region 资产场地管理
        /// <summary>
        /// 所有资产类型列表
        /// </summary>
        public void GETZCGLLOCATIONLIST(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            //var list = new SZHL_ZCGL_TypeB().GetEntities(p => p.IsDel == 0);
            DataTable dt = new SZHL_ZCGL_LocationB().GetDTByCommand("select * from dbo.SZHL_ZCGL_LOCATION where IsDel=0 and ComId=" + UserInfo.User.ComId + " order by DisplayOrder");
            msg.Result = dt;
        }
        /// <summary>
        /// 所有资产类型列表，并展示资产数量
        /// </summary>
        public void GETZCGLLOCATIONLISTWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = " z.IsDel=0 ";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.TypeID='" + P1 + "' ";
            }
            if (!string.IsNullOrEmpty(P2))
            {
                where += " and z.BranchCode='" + P2 + "' ";
            }
            string status = context.Request["status"] ?? "";
            if (!string.IsNullOrEmpty(status))
            {
                where += string.Format(" And z.Status='{0}' ", status);
            }
            DataTable dt = new SZHL_ZCGL_LocationB().GetDTByCommand("select *,(select COUNT(0) from dbo.SZHL_ZCGL z where " + where + " and z.LocationID=l.ID) as ZCNum  from dbo.SZHL_ZCGL_LOCATION l where IsDel=0  and BranchCode='" + P2 + "' and ComId=" + UserInfo.User.ComId + " and (select COUNT(0) from dbo.SZHL_ZCGL z where " + where + " and z.LocationID=l.ID)>0 order by DisplayOrder");
            msg.Result = dt;
        }
        /// <summary>
        /// 所有资产状态列表，并展示资产数量
        /// </summary>
        public void GETZCGLSTATUSLISTBYLOCATIONWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = " z.IsDel=0 ";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.TypeID='" + P1 + "' ";
            }
            if (!string.IsNullOrEmpty(P2))
            {
                where += " and z.BranchCode='" + P2 + "' ";
            }
            string location = context.Request["location"] ?? "";
            if (!string.IsNullOrEmpty(location))
            {
                where += string.Format(" And z.LocationID='{0}' ", location);
            }

            string tmpTable = "#StatusData" + DateTime.Now.ToString("yyMMddhhmmss");
            string statusdata = context.Request["statusdata"] ?? "";
            List<ExtentionData> ExDataList = JsonConvert.DeserializeObject<List<ExtentionData>>(statusdata);

            string tmpS = " create table " + tmpTable + "(ID int NOT NULL, TypeName varchar(50) NOT NULL) ";
            foreach (ExtentionData e in ExDataList)
            {
                tmpS += " insert into " + tmpTable + " values ('" + e.ID + "','" + e.TypeName + "')";
            }
            tmpS += " select s.*, (select count(0) from  dbo.SZHL_ZCGL z where " + where + " and z.Status=s.ID ) as ZCNum from " + tmpTable + " s where (select count(0) from  dbo.SZHL_ZCGL z where " + where + " and z.Status=s.ID )>0 ";
            tmpS += " drop table " + tmpTable;
            DataTable dt = new SZHL_ZCGL_LocationB().GetDTByCommand(tmpS);
            msg.Result = dt;
        }
        /// <summary>
        /// 资产类型分页列表
        /// </summary>  
        public void GETZCGLLOCATIONLIST_PAGE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string strWhere = " cc.ComId=" + UserInfo.User.ComId;
            //strWhere += string.Format(" And cc.CRUser='{0}' ", UserInfo.User.UserName);
            if (P1 != "")
            {
                strWhere += string.Format(" And cc.Title like '%{0}%'", P1);
            }
            string bc = context.Request["branchcode"] ?? "";

            if (bc != "")
            {
                strWhere += string.Format(" And cc.BranchCode = '{0}'", bc);
            }

            int page = 0;
            int pagecount = 10;
            int.TryParse(context.Request["p"] ?? "1", out page);
            int.TryParse(context.Request["pagecount"] ?? "10", out pagecount);//页数
            page = page == 0 ? 1 : page;
            int total = 0;

            DataTable dt = new SZHL_ZCGL_LocationB().GetDataPager(" SZHL_ZCGL_LOCATION cc left join JH_Auth_Branch b on cc.BranchCode=b.DeptCode", "cc.* , b.DeptName", pagecount, page, " cc.DisplayOrder", strWhere, ref total);

            msg.Result = dt;
            msg.Result1 = total;
        }

        /// <summary>
        /// 资产类型详细信息
        /// </summary>
        public void GETLOCATIONMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            SZHL_ZCGL_Location model = new SZHL_ZCGL_LocationB().GetEntity(d => d.ID == Id && d.ComId == UserInfo.User.ComId);
            msg.Result = model;
        }
        /// <summary>
        /// 添加资产类型
        /// </summary>
        public void ADDLOCATION(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            SZHL_ZCGL_Location t = JsonConvert.DeserializeObject<SZHL_ZCGL_Location>(P1);

            if (string.IsNullOrEmpty(t.Title))
            {
                msg.ErrorMsg = "场地名称不能为空!";
            }

            if (t.BranchCode == null || t.BranchCode <= 0)
            {
                msg.ErrorMsg = "公司不能为空!";
            }

            if (string.IsNullOrEmpty(msg.ErrorMsg))
            {
                if (t.ID == 0)
                {
                    var t1 = new SZHL_ZCGL_LocationB().GetEntity(p => p.ComId == UserInfo.User.ComId && p.Title == t.Title);
                    if (t1 != null)
                    {
                        msg.ErrorMsg = "系统已经存在此类型名称!";
                    }
                    else
                    {
                        t.CRDate = DateTime.Now;
                        t.CRUser = UserInfo.User.UserName;
                        t.ComId = Convert.ToInt16(UserInfo.User.ComId);
                        t.IsDel = 0;
                        new SZHL_ZCGL_LocationB().Insert(t);
                        msg.Result = t;
                    }
                }
                else
                {
                    var hys1 = new SZHL_ZCGL_LocationB().GetEntity(p => p.ComId == UserInfo.User.ComId && p.Title == t.Title && p.ID != t.ID);
                    if (hys1 != null)
                    {
                        msg.ErrorMsg = "系统已经存在此类型名称";
                    }
                    else
                    {
                        new SZHL_ZCGL_LocationB().Update(t);
                        msg.Result = t;
                    }
                }

            }
        }
        /// <summary>
        /// 删除资产类型
        /// </summary>
        public void DELZCGLLOCATION(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            int ss = int.Parse(P2);
            SZHL_ZCGL_Location model = new SZHL_ZCGL_LocationB().GetEntity(d => d.ID == Id && d.ComId == UserInfo.User.ComId);
            model.IsDel = ss;
            new SZHL_ZCGL_LocationB().Update(model);

        }
        #endregion

        #region 资产生命周期管理
        /// <summary>
        /// 生命周期信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="P1">LIFECYCLEID</param>
        /// <param name="P2"></param>
        /// <param name="UserInfo"></param>
        public void GETLIFECYCLEMODEL(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            string strWhere = " IsDel=0  and ID=" + Id;
            string colNme = @"* ";
            string tableName = string.Format(@" SZHL_ZCGL_LifeCycle ");

            string strSql = string.Format("Select {0}  From {1} where {2} ", colNme, tableName, strWhere);
            DataTable dt = new SZHL_ZCGLB().GetDTByCommand(strSql);
            msg.Result = dt;
        }

        /// <summary>
        /// 添加或者编辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="UserInfo"></param>
        public void UPDATELIFECYCLE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int ZCGLid = int.Parse(P1);
            SZHL_ZCGL_LifeCycle ZCLC = JsonConvert.DeserializeObject<SZHL_ZCGL_LifeCycle>(P2);
            if (ZCGLid <= 0)
            {
                msg.ErrorMsg = "资产不存在！";
                return;
            }
            if (string.IsNullOrWhiteSpace(ZCLC.Title))
            {
                msg.ErrorMsg = "标题不能为空！";
                return;
            }
            if (ZCLC.FromDate == null)
            {
                msg.ErrorMsg = "日期不能为空！";
                return;
            }
            if (ZCLC.TypeID <= 0)
            {
                msg.ErrorMsg = "请选择类型！";
                return;
            }

            if (ZCLC.ID == 0)
            {
                ZCLC.ZCGLID = ZCGLid;
                ZCLC.CRDate = DateTime.Now;
                ZCLC.CRUser = UserInfo.User.UserName;
                ZCLC.ComId = UserInfo.User.ComId.Value;
                ZCLC.IsDel = 0;
                if (new SZHL_ZCGL_LifeCycleB().Insert(ZCLC))
                {
                    SENDZCGLMSG(ZCGLid);
                }
            }
            else
            {
                ZCLC.UpdateDate = DateTime.Now;
                ZCLC.UpdateUser = UserInfo.User.UserName;
                if (new SZHL_ZCGL_LifeCycleB().Update(ZCLC))
                {
                    SENDZCGLMSG(ZCGLid);
                }
            }
            msg.Result = ZCLC;
        }

        /// <summary>
        /// 生命周期新加或者修改后给使用人发送短信
        /// </summary>
        /// <param name="ZCGLID"></param>
        public void SENDZCGLMSG(int ZCGLID)
        {
            SZHL_ZCGL ZC = new SZHL_ZCGLB().GetEntity(d => d.ID == ZCGLID);
            if (ZC != null)
            {
                JH_Auth_User USER = new JH_Auth_UserB().GetEntity(d => d.UserName == ZC.UserName);
                if (USER != null)
                {
                    string phone = "13980444473";
                    string host = CommonHelp.GetConfig("HostUrl");
                    string url = CommonHelp.GetShortUrl(host + "/WX/bgxt/zc_Detail.html?id=" + ZCGLID);
                    string content = ZC.Name + "的生命周期发生变动，请查看" + url;
                    if (phone.Trim().Length > 0)
                    {
                        string message = "尊敬的" + USER.UserRealName.Trim() + "，" + content;
                        if (CommonHelp.MarchPhoneNumber(phone))
                        {
                            string MASresult = CommonHelp.SendMAS(phone, message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生命周期分类列表，并展示资产数量
        /// </summary>
        public void GETZCGLLIFECYCLELISTWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = " z.IsDel=0 ";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.TypeID='" + P1 + "' ";
            }
            string status = context.Request["status"] ?? "";
            if (!string.IsNullOrEmpty(status))
            {
                where += string.Format(" And z.Status='{0}' ", status);
            }
            where += " and z.ID in ( select ZCGLID from SZHL_ZCGL_LifeCycle where TypeID= s.ID and IsDel=0 ) ";

            string tmpTable = "#LifeCycleData" + DateTime.Now.ToString("yyMMddhhmmss");
            string lifecycledata = context.Request["lifecycledata"] ?? "";
            List<ExtentionData> ExDataList = JsonConvert.DeserializeObject<List<ExtentionData>>(lifecycledata);
            string tmpS = " create table " + tmpTable + "(ID int NOT NULL, TypeName varchar(50) NOT NULL) ";
            foreach (ExtentionData e in ExDataList)
            {
                tmpS += " insert into " + tmpTable + " values ('" + e.ID + "','" + e.TypeName + "')";
            }
            tmpS += " select s.*, (select count(0) from  dbo.SZHL_ZCGL z where " + where + ") as ZCNum from " + tmpTable + " s where (select count(0) from  dbo.SZHL_ZCGL z where " + where + ")>0 ";
            tmpS += " drop table " + tmpTable;
            DataTable dt = new SZHL_ZCGL_LocationB().GetDTByCommand(tmpS);
            msg.Result = dt;
        }
        /// <summary>
        /// 所有资产状态列表，并展示资产数量
        /// </summary>
        public void GETZCGLSTATUSLISTBYLIFECYCLEWITHNUM(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string where = " z.IsDel=0 ";
            if (!string.IsNullOrEmpty(P1))
            {
                where += " and z.TypeID='" + P1 + "' ";
            }
            if (!string.IsNullOrEmpty(P2))
            {
                where += " and z.ID in ( select ZCGLID from SZHL_ZCGL_LifeCycle where TypeID= '" + P2 + "' and IsDel=0 ) ";
            }
            string location = context.Request["location"] ?? "";
            if (!string.IsNullOrEmpty(location))
            {
                where += string.Format(" And z.LocationID='{0}' ", location);
            }

            string tmpTable = "#StatusData" + DateTime.Now.ToString("yyMMddhhmmss");
            string statusdata = context.Request["statusdata"] ?? "";
            List<ExtentionData> ExDataList = JsonConvert.DeserializeObject<List<ExtentionData>>(statusdata);

            string tmpS = " create table " + tmpTable + "(ID int NOT NULL, TypeName varchar(50) NOT NULL) ";
            foreach (ExtentionData e in ExDataList)
            {
                tmpS += " insert into " + tmpTable + " values ('" + e.ID + "','" + e.TypeName + "')";
            }
            tmpS += " select s.*, (select count(0) from  dbo.SZHL_ZCGL z where " + where + " and z.Status=s.ID ) as ZCNum from " + tmpTable + " s where (select count(0) from  dbo.SZHL_ZCGL z where " + where + " and z.Status=s.ID)>0 ";
            tmpS += " drop table " + tmpTable;
            DataTable dt = new SZHL_ZCGL_LocationB().GetDTByCommand(tmpS);
            msg.Result = dt;
        }
        /// <summary>
        /// 获取生命周期列表根据资产ID
        /// </summary>
        public void GETLIFECYCLLISTBYZCGLID(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            msg.Result = new SZHL_ZCGL_LifeCycleB().GetDTByCommand("select * from dbo.SZHL_ZCGL_LifeCycle where IsDel=0 and ZCGLID=" + Id + " order by FromDate desc");
        }
        /// <summary>
        /// 删除生命周期
        /// </summary>
        public void DELLIFECYCLE(HttpContext context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int Id = int.Parse(P1);
            SZHL_ZCGL_LifeCycle model = new SZHL_ZCGL_LifeCycleB().GetEntity(d => d.ID == Id && d.ComId == UserInfo.User.ComId);
            model.IsDel = 1;
            new SZHL_ZCGL_LifeCycleB().Update(model);
        }
        #endregion
    }

    public class ExtentionData
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
    }

    public class ExtentionDataList
    {
        public List<ExtentionData> Items { get; set; }
    }
}