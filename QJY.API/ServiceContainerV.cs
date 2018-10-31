using Microsoft.Practices.Unity;

namespace QJY.API
{
    public class ServiceContainerV
    {
        public static IUnityContainer Current()
        {

            IUnityContainer container = new UnityContainer();
           
            container.RegisterType<IWsService, Commanage>("Commanage".ToUpper());//免注册接口类

            #region 基础模块接口
            container.RegisterType<IWsService, AuthManage>("XTGL");//基础接口

            container.RegisterType<IWsService, INITManage>("INIT");//系统配置相关API
            #endregion

            container.RegisterType<IWsService, XXFBManage>("XXFB");//信息发布

            container.RegisterType<IWsService, CCXJManage>("CCXJ");//出差休假

            container.RegisterType<IWsService, LCSPManage>("LCSP");//流程审批

            #region JSAPI
            container.RegisterType<IWsService, JSAPI>("JSSDK");
            #endregion

            container.RegisterType<IWsService, DXGLManage>("DXGL");//短信管理 

            container.RegisterType<IWsService, TXLManage>("QYTX");//通讯录 

            container.RegisterType<IWsService, TXSXManage>("TXSX");//提醒事项 

            container.RegisterType<IWsService, GZBGManage>("GZBG");//工作报告

            container.RegisterType<IWsService, QYWDManage>("QYWD");//文档管理 

            container.RegisterType<IWsService, RWGLManage>("RWGL");//任务管理 

            container.RegisterType<IWsService, XMGLManage>("XMGL");//项目管理

            container.RegisterType<IWsService, NOTEManage>("NOTE");//记事本管理 

            container.RegisterType<IWsService, TSSQManage>("TSSQ");//同事社区

            container.RegisterType<IWsService, JFBXManage>("JFBX");//经费报销

            container.RegisterType<IWsService, KQGLManage>("KQGL");//考勤管理

            container.RegisterType<IWsService, WQQDManage>("WQQD");//外勤签到

            container.RegisterType<IWsService, XZGLManage>("XZGL");//薪资管理

            container.RegisterType<IWsService, DBGLManage>("DBGL");//数据库管理

            container.RegisterType<IWsService, CRMManage>("CRM");//数据库管理

            container.RegisterType<IWsService, YCGLManage>("YCGL");//用车管理

            container.RegisterType<IWsService, HYGLManage>("HYGL");//会议管理

            container.RegisterType<IWsService, WXGLManage>("WXGL");//微信功能管理

            container.RegisterType<IWsService, ZCGLManage>("ZCGL");//资产管理

            container.RegisterType<IWsService, FSGLManage>("FSGL");//微信粉丝管理

            return container;
        }

    }
}
