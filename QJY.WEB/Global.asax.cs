using QJY.API;
using System;
using System.Web;

namespace QJY.WEB
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            //t.Interval = (CommonHelp.AppConfigInt("AccessTokenExpireTime") * 60 * 1000);
            t.Interval = 20 * 60 * 1000;
            t.Elapsed += new System.Timers.ElapsedEventHandler(TimerNow);
            t.AutoReset = true;
            t.Enabled = true;
            t.Start();
        }

        public void TimerNow(object source, System.Timers.ElapsedEventArgs e)
        {
            //new JH_Auth_LogB().InsertLog("Application_Start", "Application启动", "Global.asax", "System", "www.lstobacco.com", 0, "");
            WXFWHelp.UpdateTokenByTimer();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            // 在应用程序关闭时运行的代码 
            //解决应用池回收问题 
            //System.Threading.Thread.Sleep(5000);
            //Random rd = new Random();
            //string strUrl = CommonHelp.GetConfig("APITX") + "&r=" + rd.Next();
            //HttpWebResponse ResponseDataXS = CommonHelp.CreateHttpResponse(strUrl, null, 0, "", null, "GET");
            //string Returndata = new StreamReader(ResponseDataXS.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            //更新AccessToken
            new JH_Auth_LogB().InsertLog("Application_End", "Application关闭", "Global.asax", "System", "www.lstobacco.com", 0, "");
            WXFWHelp.UpdateTokenByTimer();
        }
    }
}