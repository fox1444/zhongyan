using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QJY.API
{
    /// <summary>
    /// 返回消息类
    /// </summary>
    public class Msg_Result
    {
        public string Action { get; set; }
        public string ErrorMsg { get; set; }
        public int DataLength { get; set; }
        public string ResultType { get; set; }
        public dynamic Result { get; set; }
        public dynamic Result1 { get; set; }
        public dynamic Result2 { get; set; }
        public dynamic Result3 { get; set; }
        public dynamic Result4 { get; set; }
        public dynamic Result5 { get; set; }
        public dynamic Result6 { get; set; }
    }

    /// <summary>
    /// 云MAS短信返回类
    /// </summary>
    public class Msg_SMSendResult
    {
        public string msgGroup { get; set; }
        public string rspcod { get; set; }
        public bool success { get; set; }
        public string IllegalMac { get; set; }
        public string InvalidMessage { get; set; }
        public string InvalidUsrOrPwd { get; set; }
        public string NoSignId { get; set; }
        public string IllegalSignId { get; set; }
        public string TooManyMobiles { get; set; }
    }

    /// <summary>
    /// 微博短链接返回类
    /// </summary>
    public class Msg_ShortUrl
    {
        public string url_short { get; set; }
        public string url_long { get; set; }
        public int type { get; set; }
    }

}
