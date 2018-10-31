﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using QJY.Data;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Configuration;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System.Net.Http;
using System.Security.Cryptography;

namespace QJY.API
{
    public class CommonHelp
    {
        /// <summary>
        /// 从html中提取纯文本
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public string StripHT(string strHtml)
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string strOutput = regex.Replace(strHtml, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");
            return strOutput;
        }

        /// <summary>
        /// 移除html标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;

            Regex regex = new Regex("<.+?>");
            var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                html = html.Replace(match.Value, "");
            }
            return html;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 生成PCCode
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static string CreatePCCode(JH_Auth_User user)
        {
            string strPCCode = EncrpytHelper.Encrypt(user.UserName + "@" + user.UserPass + "@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            return strPCCode;
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        public static HttpWebResponse CreateHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies, string strType = "POST")
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = strType;
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies, string strType = "POST")
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = strType;
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.Default);
                return reader.ReadToEnd();

            }
        }

        /// <summary>
        /// DataTable导出成Excel
        /// </summary>
        public static MemoryStream RenderToExcel(DataTable table)
        {
            MemoryStream ms = new MemoryStream();

            using (table)
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow headerRow = sheet.CreateRow(0);

                // handling header.
                foreach (DataColumn column in table.Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in table.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                    rowIndex++;
                }

                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }

            //using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            //{
            //    byte[] data = ms.ToArray();

            //    fs.Write(data, 0, data.Length);
            //    fs.Flush();
            //    data = null;
            //}
            return ms;
        }

        /// <summary>
        /// excel转换为DataTable
        /// </summary>
        public DataTable ExcelToTable(HttpPostedFile upfile, int headrow)
        {
            DataTable dt = new DataTable();

            IWorkbook workbook = null;

            Stream stream = upfile.InputStream;

            string suffix = upfile.FileName.Substring(upfile.FileName.LastIndexOf(".") + 1).ToLower();
            if (suffix == "xlsx") // 2007版本
            {
                workbook = new XSSFWorkbook(stream);
            }
            else if (suffix == "xls") // 2003版本
            {
                workbook = new HSSFWorkbook(stream);
            }

            //获取excel的第一个sheet
            ISheet sheet = workbook.GetSheetAt(0);

            //获取sheet的第一行
            IRow headerRow = sheet.GetRow(headrow);

            //一行最后一个方格的编号 即总的列数
            int cellCount = headerRow.LastCellNum;
            //最后一列的标号  即总的行数
            int rowCount = sheet.LastRowNum;
            //列名
            for (int i = 0; i < cellCount; i++)
            {
                dt.Columns.Add(headerRow.GetCell(i).ToString());
            }

            for (int i = (sheet.FirstRowNum + headrow + 1); i <= sheet.LastRowNum; i++)
            {
                DataRow dr = dt.NewRow();

                IRow row = sheet.GetRow(i);
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        dr[j] = row.GetCell(j).ToString();
                    }
                }

                dt.Rows.Add(dr);
            }

            sheet = null;
            workbook = null;

            return dt;
        }

        static void RenderToBrowser(MemoryStream ms, HttpContext context, string fileName)
        {
            if (context.Request.Browser.Browser == "IE")
                fileName = HttpUtility.UrlEncode(fileName);
            context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
            context.Response.BinaryWrite(ms.ToArray());
        }

        public static bool HasData(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workbook = new HSSFWorkbook(excelFileStream);
                if (workbook.NumberOfSheets > 0)
                {
                    ISheet sheet = workbook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// String强制转换成Int，默认为空字符串
        /// </summary>
        public static int ParseInt(string p)
        {
            int result = 0;
            try
            {
                result = int.Parse(p);
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// 验证手机号码
        /// </summary>
        public static bool MarchPhoneNumber(string phone)
        {
            Regex rx = new Regex(@"^0{0,1}(13[4-9]|15[7-9]|15[0-2]|18[7-8])[0-9]{8}$");
            if (!rx.IsMatch(phone)) //不匹配
            {
                return false;
            }
            else //匹配
            {
                return true;
            }
        }

        /// <summary>
        /// Post文件
        /// </summary>
        public static string PostFile(string uploadUrl, string fileToUpload, string poststr = "")
        {
            string result = "";

            try
            {
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uploadUrl);
                webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webrequest.Method = "POST";
                StringBuilder sb = new StringBuilder();
                if (poststr != "")
                {
                    foreach (string c in poststr.Split('&'))
                    {
                        string[] item = c.Split('=');
                        if (item.Length != 2)
                        {
                            break;
                        }
                        string name = item[0];
                        string value = item[1];
                        sb.Append("–" + boundary);
                        sb.Append("\r\n");
                        sb.Append("Content-Disposition: form-data; name=\"" + name + "\"");
                        sb.Append("\r\n\r\n");
                        sb.Append(value);
                        sb.Append("\r\n");
                    }
                }
                sb.Append("--");
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"file");
                //sb.Append(fileFormName);
                sb.Append("\"; filename=\"");
                sb.Append(Path.GetFileName(fileToUpload));
                sb.Append("\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: application/octet-stream");
                //sb.Append(contenttype);
                sb.Append("\r\n");
                sb.Append("\r\n");
                string postHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                FileStream fileStream = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read);
                long length = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
                webrequest.ContentLength = length;
                Stream requestStream = webrequest.GetRequestStream();
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                byte[] buffer = new Byte[(int)fileStream.Length];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                fileStream.Close();
                WebResponse responce = webrequest.GetResponse();
                requestStream.Close();
                using (Stream s = responce.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        result = sr.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                CommonHelp.WriteLOG(uploadUrl + "|||" + fileToUpload + "|||" + ex.ToString());
            }
            return result;
        }

        public static string ProcessWxIMG(string mediaIds, string strCode, JH_Auth_UserB.UserInfo UserInfo, string strType = ".jpg")
        {
            try
            {
                WXHelp wx = new WXHelp(UserInfo.QYinfo);
                string ids = "";
                foreach (var mediaId in mediaIds.Split(','))
                {
                    string fileToUpload = wx.GetMediaFile(mediaId, strType);
                    string md5 = PostFile(UserInfo.QYinfo.FileServerUrl.TrimEnd('/') + "/fileupload?qycode=" + UserInfo.QYinfo.QYCode, fileToUpload);
                    System.IO.FileInfo f = new FileInfo(fileToUpload);
                    FT_File newfile = new FT_File();
                    newfile.ComId = UserInfo.User.ComId;
                    newfile.Name = f.Name;
                    newfile.FileMD5 = md5.Replace("\"", "");
                    newfile.FileSize = f.Length.ToString();
                    newfile.FileVersin = 0;
                    newfile.CRDate = DateTime.Now;
                    newfile.CRUser = UserInfo.User.UserName;
                    newfile.UPDDate = DateTime.Now;
                    newfile.UPUser = UserInfo.User.UserName;
                    newfile.FolderID = 3;
                    newfile.FileExtendName = f.Extension.Substring(1);
                    newfile.ISYL = "Y";
                    new FT_FileB().Insert(newfile);

                    if (ids == "")
                    {
                        ids = newfile.ID.ToString();
                    }
                    else
                    {
                        ids += "," + newfile.ID.ToString();
                    }
                }

                return ids;
            }
            catch (Exception ex)
            {
                CommonHelp.WriteLOG(ex.ToString());
                return "";
            }
        }

        public static string ProcessWxIMGUrl(string url, JH_Auth_UserB.UserInfo UserInfo, string strType = ".jpg")
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream reader = response.GetResponseStream();

                string path = HttpContext.Current.Server.MapPath("\\temp\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileToUpload = path + Guid.NewGuid().ToString() + strType;

                FileStream writer = new FileStream(fileToUpload, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                writer.Dispose();
                reader.Close();
                reader.Dispose();
                response.Close();

                string URL = UserInfo.QYinfo.FileServerUrl + "fileupload?qycode=" + UserInfo.QYinfo.QYCode;
                string md5 = PostFile(URL, fileToUpload);

                System.IO.FileInfo f = new FileInfo(fileToUpload);
                FT_File newfile = new FT_File();
                newfile.ComId = UserInfo.User.ComId;
                newfile.Name = f.Name;
                newfile.FileMD5 = md5.Replace("\"", "");
                newfile.FileSize = f.Length.ToString();
                newfile.FileVersin = 0;
                newfile.CRDate = DateTime.Now;
                newfile.CRUser = UserInfo.User.UserName;
                newfile.UPDDate = DateTime.Now;
                newfile.UPUser = UserInfo.User.UserName;
                newfile.FolderID = 3;
                newfile.FileExtendName = f.Extension.Substring(1);
                newfile.ISYL = "Y";
                new FT_FileB().Insert(newfile);



                return newfile.ID.ToString();
            }
            catch (Exception ex)
            {
                CommonHelp.WriteLOG(ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public static string SendDX(string Mobile, string Content, string SendTime)
        {
            try
            {
                string url = CommonHelp.GetConfig("DXURL") + "&Mobile=" + Mobile + "&Content=" + Content;
                WebClient WC = new WebClient();
                WC.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                int p = url.IndexOf("?");
                string sData = url.Substring(p + 1);
                url = url.Substring(0, p);
                byte[] postData = Encoding.GetEncoding("gb2312").GetBytes(sData);
                byte[] responseData = WC.UploadData(url, "POST", postData);
                string returnData = Encoding.GetEncoding("gb2312").GetString(responseData);
                return returnData;

            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        /// <summary>
        /// 通过新浪api生成短链接
        /// </summary>
        public static string GetShortUrl(string long_url)
        {
            string short_url = "";
            try
            {
                var result = HttpGet("http://api.t.sina.com.cn/short_url/shorten.json?source=1681459862&url_long=" + long_url, "");
                List<Msg_ShortUrl> jsonresult = JsonConvert.DeserializeObject<List<Msg_ShortUrl>>(result);
                short_url = jsonresult[0].url_short;
            }
            catch
            {

            }
            return short_url;
        }

        /// <summary>
        /// 移动云MAS 发送短信接口
        /// </summary>
        /// <param name="Mobiles">手机号以,隔开</param>
        /// <param name="Content">发送内容，建议400字以内</param>
        /// <returns></returns>
        public static string SendMAS(string Mobiles, string Content)
        {
            string authurl = "http://112.35.1.155:1992/sms/norsubmit"; //正式环境
            string ecName = "四川省烟草公司凉山州烟草公司"; //集团客户名称
            string apId = "lsyc01"; //用户名
            string secretKey = "CS08122998032"; //密码
            string mobiles = Mobiles;
            string content = Content;
            string sign = "WUghy8diA"; //签名编码
            string addSerial = ""; //扩展码，根据向移动公司申请的通道填写，如果申请的精确匹配通道，则填写空字符串("")，否则添加移动公司允许的扩展码

            string ConvertString = ecName + apId + secretKey + mobiles + content + sign + addSerial;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string mac = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(ConvertString)));
            mac = mac.Replace("-", "").ToLower();
            
            var authRequest = new
            {
                ecName = ecName,
                apId = apId,
                secretKey = secretKey,
                mobiles = mobiles,
                content = content,
                sign = sign,
                addSerial = addSerial,
                mac = mac
            };
            string sData = Base64Encode(Encoding.UTF8, JsonConvert.SerializeObject(authRequest));
            WebClient WC = new WebClient();
            WC.Headers.Add("Content-Type", "application/json");
            byte[] postData = Encoding.UTF8.GetBytes(sData);
            byte[] responseData = WC.UploadData(authurl, "POST", postData);
            string returnData = Encoding.UTF8.GetString(responseData);
            return returnData;
        }

        /// <summary>
        /// 发送MAS短信使用的字符编码
        /// </summary>
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// 请求Url，发送数据
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 迅捷科技发送短信方法，现已不用
        /// </summary>
        /// <param name="telephone"></param>
        /// <param name="content"></param>
        /// <param name="ComId"></param>
        /// <returns></returns>
        public static string SendSMS(string telephone, string content, int ComId = 0)
        {
            string err = "";
            try
            {
                string dxqz = "企捷科技";
                decimal amcountmoney = 0;
                var qy = new JH_Auth_QYB().GetEntity(d => d.ComId == ComId);
                if (qy != null)
                {
                    dxqz = qy.DXQZ;
                    amcountmoney = qy.AccountMoney.HasValue ? qy.AccountMoney.Value : 0;
                }

                string[] tels = telephone.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace('，', ',').Split(',');

                //判断金额是否够
                decimal DXCost = decimal.Parse(CommonHelp.GetConfig("DXCost"));
                decimal amount = tels.Length * DXCost;
                if (ComId != 0 && amcountmoney < amount) //短信余额不足
                {
                    err = "短信余额不足";
                }
                else
                {
                    string re = "";
                    foreach (string tel in tels)
                    {
                        re = SendDX(tel, content + "【" + dxqz + "】", "");
                    }

                    err = "发送成功";

                    //扣款
                    if (ComId != 0 && qy != null)
                    {
                        qy.AccountMoney = qy.AccountMoney - amount;
                        new JH_Auth_QYB().Update(qy);

                        //记录明细
                        //SZHL_DDGL_ITEM im = new SZHL_DDGL_ITEM();
                        //im.ComId = qy.ComId;
                        //im.SerialID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        //im.Change = -amount;
                        //im.Balance = amcountmoney - amount;
                        //im.Memo = "短信消费";
                        //im.Status = "已消费";
                        //im.CRDate = DateTime.Now;

                        //new SZHL_DDGL_ITEMB().Insert(im);
                    }

                }
            }
            catch { }
            return err;
        }

        private int rep = 0;

        /// <summary>
        /// 生成随机不重复的字符串（分享码用）
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetMD5(string content)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(content, "md5");
        }

        /// <summary>
        /// 获取url中参数
        /// </summary>
        public static string GetQueryString(string QueryName)
        {
            string tmp = "";
            try
            {
                tmp = HttpContext.Current.Request.QueryString[QueryName].ToString();
            }
            catch
            {

            }
            return tmp;
        }

        /// <summary>
        /// 获取url中参数并转换成整数，默认为0
        /// </summary>
        public static int GetQueryInt(string QueryName)
        {
            int tmp = 0;
            try
            {
                tmp = int.Parse(HttpContext.Current.Request.QueryString[QueryName].ToString());
            }
            catch
            {

            }
            return tmp;
        }

        /// <summary>
        /// 设置缓存，时间为永久
        /// </summary>
        public static void SetCookie(string key, string value)
        {
            try
            {
                HttpContext.Current.Response.Cookies.Remove(key);
            }
            catch
            {

            }
            HttpCookie cookie = new HttpCookie(key, value);
        }

        /// <summary>
        /// 设置缓存以及过期时间
        /// </summary>
        public static void SetCookie(string key, string value, DateTime expires)
        {
            try
            {
                HttpContext.Current.Response.Cookies.Remove(key);
            }
            catch
            {

            }
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = expires;
            HttpContext.Current.Response.SetCookie(cookie);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        public static string GetCookieString(string key)
        {
            string tmp = "";
            try
            {
                tmp = HttpContext.Current.Request.Cookies[key].Value.ToString();
            }
            catch
            {

            }
            if (tmp.ToLowerInvariant() == "null")
                tmp = "";
            return tmp;
        }

        /// <summary>
        /// 获取szhlcode，即pccode
        /// </summary>
        public static string Getszhlcode()
        {
            return GetCookieString("szhlcode");
        }

        /// <summary>
        /// 根据szhlcode/pccode返回用户名
        /// </summary>
        public static string GetUserNameByszhlcode()
        {
            string _username = "System";
            string _szhlcode = Getszhlcode();
            if (_szhlcode.Trim().Length > 0)
            {
                var UserInfo = new JH_Auth_UserB().GetUserByPCCode(_szhlcode);
                if (UserInfo != null)
                {
                    _username = UserInfo.UserName;
                }
            }
            return _username;
        }

        /// <summary>
        /// 获取web.config中配置值
        /// </summary>
        public static string GetConfig(string strKey, string strDefault = "")
        {
            return ConfigurationManager.AppSettings[strKey] ?? strDefault;
        }

        /// <summary>
        /// 获取数据库中APPConfig表的配置值
        /// </summary>
        public static string AppConfig(string ConfigName)
        {
            APPConfig model = new APPConfigB().GetEntity(d => d.ConfigName == ConfigName);
            if (model != null)
                return model.ConfigValue;
            else
                return "";
        }

        /// <summary>
        /// 获取数据库中APPConfig表的配置值并转换成整型
        /// </summary>
        public static int AppConfigInt(string ConfigName)
        {
            APPConfig model = new APPConfigB().GetEntity(d => d.ConfigName == ConfigName);
            try
            {
                if (model != null)
                {
                    return int.Parse(model.ConfigValue);
                }

                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 更新数据库中APPConfig表的配置值
        /// </summary>
        public static void UpdateAppConfig(string ConfigName, string ConfigValue)
        {
            string _username = GetUserNameByszhlcode();
            APPConfig model = new APPConfigB().GetEntity(d => d.ConfigName == ConfigName);
            if (model != null)
            {
                model.ConfigValue = ConfigValue;
                model.CRDate = DateTime.Now;
                model.CRUser = _username;
                new APPConfigB().Update(model);
            }
            else
            {
                model.ConfigName = ConfigName;
                model.ConfigValue = ConfigValue;
                model.CRDate = DateTime.Now;
                model.CRUser = _username;
                new APPConfigB().Insert(model);
            }
        }

        /// <summary>
        /// 所有取AccessToken值的地方都在这里
        /// </summary>
        public static string GetAccessToken()
        {
            APPConfig model = new APPConfigB().GetEntity(d => d.ConfigName == "AccessToken");
            if (model != null)
            {
                double expires = DateTime.Now.Subtract(DateTime.Parse(model.CRDate.ToString())).TotalSeconds;
                if (expires > 6000)
                {
                    return WXFWHelp.GetToken();
                }
                else
                {
                    return model.ConfigValue;
                }
            }
            else
                return "";
        }

        /// <summary>
        /// 获取数字验证码
        /// </summary>
        public static string numcode(int codenum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string[] stray = new string[codenum];
            Random random = new Random();
            for (int i = 0; i < codenum; i++)
            {
                int iNum = 0;
                while ((iNum = Convert.ToInt32(VcArray.Length * random.NextDouble())) == VcArray.Length)
                {
                    iNum = Convert.ToInt32(VcArray.Length * random.NextDouble());
                }
                stray[i] = VcArray[iNum];
            }

            string identifycode = string.Empty;
            foreach (string s in stray)
            {
                identifycode += s;
            }
            return identifycode;
        }

        /// <summary>
        /// 登录验证码
        /// </summary>
        public static string yzmcode(int codenum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
            string[] VcArray = Vchar.Split(',');
            string[] stray = new string[codenum];
            Random random = new Random();
            for (int i = 0; i < codenum; i++)
            {
                int iNum = 0;
                while ((iNum = Convert.ToInt32(VcArray.Length * random.NextDouble())) == VcArray.Length)
                {
                    iNum = Convert.ToInt32(VcArray.Length * random.NextDouble());
                }
                stray[i] = VcArray[iNum];
            }

            string identifycode = string.Empty;
            foreach (string s in stray)
            {
                identifycode += s;
            }
            return identifycode;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        public static string getIPAddress()
        {
            string result = "";
            try
            {

                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                // 如果使用代理，获取真实IP 
                if (result != null && result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
                    result = null;
                else if (result != null)
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有“,”，估计多个代理。取第一个不是内网的IP。 
                        result = result.Replace(" ", "").Replace("'", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i])
                                && temparyip[i].Substring(0, 3) != "10."
                                && temparyip[i].Substring(0, 7) != "192.168"
                                && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i];    //找到不是内网的地址 
                            }
                        }
                    }
                    else if (IsIPAddress(result)) //代理即是IP格式 
                        return result;
                    else
                        result = null;    //代理中的内容 非IP，取IP 
                }
                if (null == result || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (result == null || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.UserHostAddress;

            }
            catch (Exception)
            {
                result = "";
            }
            return result;

        }

        /// <summary>
        /// 验证IP地址格式
        /// </summary>
        private static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }

        /// <summary>
        /// 生成IP信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string getIpAddr(string ip = "")
        {
            string ipAddr = "";
            try
            {
                string url = "http://1212.ip138.com/ic.asp";
                System.Net.WebClient webClient = new System.Net.WebClient();
                string strSource = webClient.DownloadString(url);

                string regex = @"<center>.+</center>";
                ipAddr = System.Text.RegularExpressions.Regex.Match(strSource, regex).ToString();
                ipAddr = ipAddr.Replace("<center>您的IP是：", "");
                ipAddr = ipAddr.Replace("</center>", "");
                ipAddr = ipAddr.Replace("来自", "");
                ipAddr = ipAddr.Split('：')[1];
                ipAddr = ipAddr.Split(' ')[0];

            }
            catch (Exception ex)
            {
                return "";
            }
            return ipAddr;
        }

        /// <summary>
        /// 生成日志文件
        /// </summary>
        /// <param name="err"></param>
        public static void WriteLOG(string err)
        {
            try
            {
                string path = HttpContext.Current.Request.MapPath("/");
                if (!Directory.Exists(path + "/log/"))
                {
                    Directory.CreateDirectory(path + "/log/");
                }

                string name = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (!File.Exists(path + "/log/" + name))
                {
                    FileInfo myfile = new FileInfo(path + "/log/" + name);
                    FileStream fs = myfile.Create();
                    fs.Close();
                }

                StreamWriter sw = File.AppendText(path + "/log/" + name);
                sw.WriteLine(err + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {

            }


        }

        /// <summary>
        /// 获取导入excel的字段
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<IMPORTYZ> GetTable(string code, int comid)
        {
            string json = string.Empty;
            switch (code)
            {
                case "KHGL":

                    json = "[{\"Name\":\"客户名称\",\"Length\":\"50\",\"IsNull\":\"1\",\"IsRepeat\":\"SZHL_CRM_KHGL|KHName\"},{\"Name\":\"客户类型\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                            + "{\"Name\":\"电话\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"邮箱\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                            + "{\"Name\":\"传真\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"网址\",\"Length\":\"50\",\"IsNull\":\"0\"},"

                            + "{\"Name\":\"地址\",\"Length\":\"500\",\"IsNull\":\"0\"},"
                            + "{\"Name\":\"邮编\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"跟进状态\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                            + "{\"Name\":\"客户来源\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"所属行业\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                            + "{\"Name\":\"人员规模\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"负责人\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                            + "{\"Name\":\"备注\",\"Length\":\"0\",\"IsNull\":\"0\"}]";
                    break;
                case "KHLXR":

                    json = "[{\"Name\":\"姓名\",\"Length\":\"50\",\"IsNull\":\"1\"},{\"Name\":\"对应客户\",\"Length\":\"50\",\"IsNull\":\"0\",\"IsExist\":\"SZHL_CRM_KHGL|KHName\"},"
                        + "{\"Name\":\"手机\",\"Length\":\"11\",\"IsNull\":\"1\"},{\"Name\":\"邮箱\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"传真\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"网址\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"电话\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"分机\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"QQ\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"微信\",\"Length\":\"100\",\"IsNull\":\"0\"},"
                        //+ "{\"Name\":\"学历\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"公司\",\"Length\":\"100\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"部门\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"职位\",\"Length\":\"100\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"地址\",\"Length\":\"200\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"邮编\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"性别\",\"Length\":\"10\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"生日\",\"Length\":\"10\",\"IsNull\":\"0\"},{\"Name\":\"备注\",\"Length\":\"0\",\"IsNull\":\"0\"}]";
                    break;
                case "HTGL":
                    json = "[{\"Name\":\"合同标题\",\"Length\":\"2500\",\"IsNull\":\"1\"},{\"Name\":\"合同类型\",\"Length\":\"50\",\"IsNull\":\"1\"},"
                        + "{\"Name\":\"合同总金额\",\"Length\":\"100\",\"IsNull\":\"1\"},{\"Name\":\"签约日期\",\"Length\":\"10\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"对应客户\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"开始时间\",\"Length\":\"10\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"结束时间\",\"Length\":\"10\",\"IsNull\":\"0\"},{\"Name\":\"合同状态\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"关联产品\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"付款方式\",\"Length\":\"100\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"付款说明\",\"Length\":\"1500\",\"IsNull\":\"0\"},{\"Name\":\"有效期\",\"Length\":\"100\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"我方签约人\",\"Length\":\"50\",\"IsNull\":\"0\"},{\"Name\":\"客户方签约人\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"合同编号\",\"Length\":\"100\",\"IsNull\":\"0\"},{\"Name\":\"负责人\",\"Length\":\"50\",\"IsNull\":\"0\"},"
                        + "{\"Name\":\"备注\",\"Length\":\"0\",\"IsNull\":\"0\"}]";
                    break;
            }

            if (comid != 0)
            {
                json = json.Substring(0, json.Length - 1);

                DataTable dtExtColumn = new JH_Auth_ExtendModeB().GetExtColumnAll(comid, code);
                foreach (DataRow drExt in dtExtColumn.Rows)
                {
                    json = json + ",{\"Name\":\"" + drExt["TableFiledName"].ToString() + "\",\"Length\":\"0\",\"IsNull\":\"0\"}";
                }

                json = json + "]";
            }

            List<IMPORTYZ> cls = JsonConvert.DeserializeObject<List<IMPORTYZ>>(json);
            return cls;

        }

        /// <summary>
        /// 获取模板中的默认数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetExcelData(string str)
        {
            string json = string.Empty;
            switch (str)
            {
                case "KHGL":
                    json = "贸易公司, 普通客户,010-65881997,123@qq.com, 010-65881998, http://www.baidu.com/,"
                        + "东三环中路101号,100000,初访,广告,服务,小于10人,13312345678,主营外贸销售，代理国外一线品牌";
                    break;
                case "KHLXR":
                    json = "张三,贸易公司,13667894321,123@qq.com,010-65881997 ,http://www.baidu.com/,010-65881997,61601 ,1123213213,fassfd21421,"
                        + "客户部,经理,东三环中路101号,100000,男,1983-09-13,负责XX项目的实施";
                    break;
                case "HTGL":
                    json = "XX项目,项目合同,12300,2015-07-08,贸易公司,2015-07-08,2015-12-08,未开始,企业号项目, 银行转账,"
                        + "服务,6个月,张经理,王经理,AC2001243251002,13312345678,合同备注";
                    break;
            }

            return json;
        }

        public class IMPORTYZ
        {
            public string Name { get; set; }
            public int Length { get; set; }
            public int IsNull { get; set; }
            public string IsRepeat { get; set; }
            public string IsExist { get; set; }
        }

        public string ExportToExcel(string Name, DataTable dt)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    HSSFWorkbook workbook = new HSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("Sheet1");

                    ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                    HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.Alignment = HorizontalAlignment.Center;
                    HeadercellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
                    HeadercellStyle.FillPattern = FillPattern.SolidForeground;
                    HeadercellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;

                    //字体
                    NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                    headerfont.Boldweight = (short)FontBoldWeight.Bold;
                    headerfont.FontHeightInPoints = 12;
                    HeadercellStyle.SetFont(headerfont);


                    //用column name 作为列名
                    int icolIndex = 0;
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(dc.ColumnName);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }

                    ICellStyle cellStyle = workbook.CreateCellStyle();

                    //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                    NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                    cellfont.Boldweight = (short)FontBoldWeight.Normal;
                    cellStyle.SetFont(cellfont);

                    //建立内容行
                    int iRowIndex = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int iCellIndex = 0;
                        IRow irow = sheet.CreateRow(iRowIndex + 1);
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string strsj = string.Empty;
                            if (dr[i] != null)
                            {
                                strsj = dr[i].ToString();
                            }
                            ICell cell = irow.CreateCell(iCellIndex);
                            cell.SetCellValue(strsj);
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                        iRowIndex++;
                    }

                    //自适应列宽度
                    for (int i = 0; i < icolIndex; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        workbook.Write(ms);

                        HttpContext curContext = HttpContext.Current;


                        // 设置编码和附件格式
                        curContext.Response.ContentType = "application/vnd.ms-excel";
                        curContext.Response.ContentEncoding = Encoding.UTF8;
                        curContext.Response.Charset = "";
                        curContext.Response.AppendHeader("Content-Disposition",
                            "attachment;filename=" + HttpUtility.UrlEncode(Name + "_导出文件_" + DateTime.Now.Ticks + ".xls", Encoding.UTF8));

                        curContext.Response.BinaryWrite(ms.GetBuffer());

                        workbook = null;
                        ms.Close();
                        ms.Dispose();

                        curContext.Response.End();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "导出失败！";
            }
        }

        public static string getIP(HttpContext context)
        {
            string ipAddr = "";
            try
            {
                HttpRequest Request = context.Request;
                // 如果使用代理，获取真实IP  
                if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                    ipAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                else
                    ipAddr = Request.ServerVariables["REMOTE_ADDR"];
                if (ipAddr == null || ipAddr == "")
                    ipAddr = Request.UserHostAddress;
                return ipAddr;

            }
            catch (Exception ex)
            {
                return "";
            }
            return ipAddr;
        }

        public static bool ProcessSqlStr(string Str, int type)
        {
            string SqlStr = "";
            if (type == 1)  //Post方法提交  
            {
                SqlStr = "script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|substring(|mid(|master|truncate|char(|declare|replace(|varchar(|cast(";
            }
            else if (type == 2) //Get方法提交  
            {
                SqlStr = "'|script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|*|asc(|chr(|substring(|mid(|master|truncate|char(|declare|replace(|;|varchar(|cast(";
            }
            else if (type == 3) //Cookie提交  
            {
                SqlStr = "script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|asc(|chr(|substring(|mid(|master|truncate|char(|declare";
            }
            else  //默认Post方法提交  
            {
                SqlStr = "script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|asc(|chr(|substring(|mid(|master|truncate|char(|declare|replace(";
            }

            bool ReturnValue = true;
            try
            {
                if (Str != "")
                {
                    string[] anySqlStr = SqlStr.ToUpper().Split('|'); ;
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.ToUpper().IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }

        public string checkconetst(HttpContext context)
        {
            string strCheckString = "";
            string getkeys = "";
            if (context.Request.Form != null)
            {
                for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                {
                    getkeys = context.Request.Form.Keys[i];
                    if (!CommonHelp.ProcessSqlStr(context.Request.Form[getkeys], 1))
                    {
                        if (context.Request.Form[getkeys].ToString().Length > 20)
                        {
                            strCheckString = context.Request.Form[getkeys].ToString();

                        }
                    }
                }
            }
            if (context.Request.QueryString != null)
            {
                for (int i = 0; i < HttpContext.Current.Request.QueryString.Count; i++)
                {
                    getkeys = context.Request.QueryString.Keys[i];
                    if (!CommonHelp.ProcessSqlStr(context.Request.QueryString[getkeys], 1))
                    {
                        if (context.Request.QueryString[getkeys].ToString().Length > 20)
                        {
                            strCheckString = context.Request.QueryString[getkeys].ToString();
                        }
                    }
                }

            }
            if (context.Request.Cookies != null)
            {
                for (int i = 0; i < context.Request.Cookies.Count; i++)
                {
                    getkeys = context.Request.Cookies.AllKeys[i];
                    if (!ProcessSqlStr(context.Request.Cookies[getkeys].Value, 3))
                    {
                        if (context.Request.Cookies[getkeys].ToString().Length > 20)
                        {
                            //System.Web.HttpContext.Current.Response.Redirect("/ViewV5/APP_WARN.html?type=2&msg=检测到敏感字符");
                            strCheckString = context.Request.Cookies[getkeys].ToString();
                        }
                    }
                }
            }
            return strCheckString;
        }
    }
}