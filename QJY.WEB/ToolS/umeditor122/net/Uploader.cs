using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Collections;
using System.Net;
using System.Text;
using QJY.Data;
using QJY.API;
using System.Diagnostics;

/// <summary>
/// UEditor�༭��ͨ���ϴ���
/// </summary>
public class UploaderV5
{
    string state = "SUCCESS";

    string URL = null;
    string currentType = null;
    string uploadpath = null;
    string filename = null;
    string originalName = null;
    HttpPostedFile uploadFile = null;

    /**
  * �ϴ��ļ�����������
  * @param HttpContext
  * @param string
  * @param  string[]
  *@param int
  * @return Hashtable
  */
    public Hashtable upFile(HttpContext cxt, string pathbase, string[] filetype, int size)
    {
        // pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
        // uploadpath = cxt.Server.MapPath(pathbase);//��ȡ�ļ��ϴ�·��

        try
        {
            uploadFile = cxt.Request.Files[0];
            originalName = uploadFile.FileName;

            //Ŀ¼����
            //createFolder();

            //��ʽ��֤
            if (checkType(filetype))
            {
                state = "��������ļ�����";
            }
            //��С��֤
            if (checkSize(size))
            {
                state = "�ļ���С������վ����";
            }
            //����ͼƬ
            if (state == "SUCCESS")
            {
                filename = reName();

                if (cxt.Request.Cookies["szhlcode"] != null && cxt.Request.Cookies["szhlcode"].ToString() != "")
                {
                    JH_Auth_UserB.UserInfo usermodel = new JH_Auth_UserB().GetUserInfo(cxt.Request.Cookies["szhlcode"].Value.ToString());
                    URL = usermodel.QYinfo.FileServerUrl + "fileupload?qycode=" + usermodel.QYinfo.QYCode;
                    string md5 = SaveFile(URL, filename);
                    FT_File newfile = new FT_File();
                    newfile.ComId = usermodel.User.ComId;
                    newfile.Name = uploadFile.FileName;
                    newfile.FileMD5 = md5.Replace("\"", "");
                    newfile.FileSize = uploadFile.InputStream.Length.ToString();
                    newfile.FileVersin = 0;
                    newfile.CRDate = DateTime.Now;
                    newfile.CRUser = usermodel.User.UserName;
                    newfile.UPDDate = DateTime.Now;
                    newfile.UPUser = usermodel.User.UserName;
                    newfile.FileExtendName = "jpg";
                    newfile.FolderID = 3;
                    newfile.ISYL = "Y"; 
                    new FT_FileB().Insert(newfile);
                    int filesize = 0;
                    int.TryParse(newfile.FileSize, out filesize);
                    new FT_FileB().AddSpace(usermodel.User.ComId.Value, filesize);
                    URL = "?fileId=" + newfile.ID;
                } 
            }
        }
        catch (Exception e)
        {
            state = "δ֪����";
            URL = "";
        }

        return getUploadInfo();
    }
    public string SaveFile(string uploadUrl, string fileName)
    { 
        try
        {
            string result = "";
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uploadUrl);
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file");
            sb.Append("\"; filename=\"" + fileName + "\"");
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            webrequest.ContentLength = uploadFile.InputStream.Length + postHeaderBytes.Length + boundaryBytes.Length;
            Stream requestStream = webrequest.GetRequestStream();
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            byte[] buffer = new Byte[(int)uploadFile.InputStream.Length]; //�����ļ����ȵĶ���������
            uploadFile.InputStream.Read(buffer, 0, buffer.Length); //���ļ�ת�ɶ�����
            requestStream.Write(buffer, 0, buffer.Length); //��ֵ���������� 
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            webrequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            WebResponse responce = webrequest.GetResponse();
            requestStream.Close();
            using (Stream s = responce.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    result = sr.ReadToEnd();
                }
            }
            responce.Close();

           
            return result;
        }
        catch (Exception ex)
        {
            return "";
        }
         
    }
    /**
 * �ϴ�Ϳѻ����������
  * @param HttpContext
  * @param string
  * @param  string[]
  *@param string
  * @return Hashtable
 */
    public Hashtable upScrawl(HttpContext cxt, string pathbase, string tmppath, string base64Data)
    {
        pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
        uploadpath = cxt.Server.MapPath(pathbase);//��ȡ�ļ��ϴ�·��
        FileStream fs = null;
        try
        {
            //����Ŀ¼
            createFolder();
            //����ͼƬ
            filename = System.Guid.NewGuid() + ".png";
            fs = File.Create(uploadpath + filename);
            byte[] bytes = Convert.FromBase64String(base64Data);
            fs.Write(bytes, 0, bytes.Length);

            URL = pathbase + filename;
        }
        catch (Exception e)
        {
            state = "δ֪����";
            URL = "";
        }
        finally
        {
            fs.Close();
            deleteFolder(cxt.Server.MapPath(tmppath));
        }
        return getUploadInfo();
    }

    /**
* ��ȡ�ļ���Ϣ
* @param context
* @param string
* @return string
*/
    public string getOtherInfo(HttpContext cxt, string field)
    {
        string info = null;
        if (cxt.Request.Form[field] != null && !String.IsNullOrEmpty(cxt.Request.Form[field]))
        {
            info = field == "fileName" ? cxt.Request.Form[field].Split(',')[1] : cxt.Request.Form[field];
        }
        return info;
    }

    /**
     * ��ȡ�ϴ���Ϣ
     * @return Hashtable
     */
    private Hashtable getUploadInfo()
    {
        Hashtable infoList = new Hashtable();

        infoList.Add("state", state);
        infoList.Add("url", URL);
        infoList.Add("originalName", originalName);
        infoList.Add("name", Path.GetFileName(URL));
        infoList.Add("size", uploadFile.ContentLength);
        infoList.Add("type", Path.GetExtension(originalName));

        return infoList;
    }

    /**
     * �������ļ�
     * @return string
     */
    private string reName()
    {
        return System.Guid.NewGuid() + getFileExt();
    }

    /**
     * �ļ����ͼ��
     * @return bool
     */
    private bool checkType(string[] filetype)
    {
        currentType = getFileExt();
        return Array.IndexOf(filetype, currentType) == -1;
    }

    /**
     * �ļ���С���
     * @param int
     * @return bool
     */
    private bool checkSize(int size)
    {
        return uploadFile.ContentLength >= (size * 1024 * 1024);
    }

    /**
     * ��ȡ�ļ���չ��
     * @return string
     */
    private string getFileExt()
    {
        string[] temp = uploadFile.FileName.Split('.');
        return "." + temp[temp.Length - 1].ToLower();
    }

    /**
     * ���������Զ������洢�ļ���
     */
    private void createFolder()
    {
        if (!Directory.Exists(uploadpath))
        {
            Directory.CreateDirectory(uploadpath);
        }
    }

    /**
     * ɾ���洢�ļ���
     * @param string
     */
    public void deleteFolder(string path)
    {
        //if (Directory.Exists(path))
        //{
        //    Directory.Delete(path, true);
        //}
    }
}