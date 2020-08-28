using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Util.Log;

namespace Util.Ftp
{
    /// <summary>
    /// 提供ftp客户端操作功能
    /// </summary>
    public class FtpClient
    {
        #region 定义
        /// <summary>
        /// FTP连接地址
        /// </summary>
        private string ftpServerIP;
        /// <summary>
        /// 指定FTP连接成功后的当前目录, 如果不指定即默认为根目录
        /// </summary>
        private string ftpRemotePath;
        /// <summary>
        /// 用户名
        /// </summary>
        private string ftpUserID;
        /// <summary>
        /// FtpPassword
        /// </summary>
        private string ftpPassword;
        /// <summary>
        /// Ftp路径
        /// </summary>
        private string ftpURI;
        /// <summary>
        /// 使用passive mode,默认是　false  主动模式
        /// </summary>
        public bool PassiveMode;
        /// <summary>
        /// 
        /// </summary>
        public bool KeepAlive;
        /// <summary>
        /// 
        /// </summary>
        public bool UseBinary;
        /// <summary>
        /// 日志操作文件
        /// </summary>
        private UtilLog ftplog = new UtilLog("ftp");
        #endregion
        /// <summary> 
        /// 连接FTP 
        /// </summary> 
        /// <param name="FtpServerIP">FTP连接地址</param> 
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param> 
        /// <param name="FtpUserID">用户名</param> 
        /// <param name="FtpPassword">密码</param> 
        public FtpClient(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            ftpServerIP = FtpServerIP;
            ftpRemotePath = string.Empty;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            if (string.IsNullOrWhiteSpace(FtpRemotePath)) ftpRemotePath = "";
            else ftpRemotePath = FtpRemotePath + "/";
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath;
            PassiveMode = false; //默认是关闭，使用 PORT 模式
            KeepAlive = true;
            UseBinary = true;
        }
        /// <summary>
        /// ftp文件上传
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="remotefile">远程文件名，含相对路径。不能使用绝对路径。</param>
        /// <returns>是否上传成功</returns>
        public bool Upload(string filename, string remotefile = "")
        {
            bool is_ok = false;
            FileInfo fileInf = new FileInfo(filename);
            string uri = ftpURI + (string.IsNullOrWhiteSpace(remotefile) ? fileInf.Name : Path.Combine(remotefile, fileInf.Name)); //没有指定时，用文件名。否则用指定的文件名，含相对路径
            ftplog.Info("上传文件路径:" + uri);
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = KeepAlive;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = UseBinary;
            reqFTP.UsePassive = PassiveMode; //vsftpd对这个返回不正常,只能false
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            //int buffLength = (int)fileInf.Length;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
                is_ok = true;
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp上传文件失败:" + ex.Message);
            }
            return is_ok;
        }
        //下载文件
        public bool Download(string filename, string remotedir="", string localdir="")
        {
            bool is_ok = false;
            string strfilename = string.IsNullOrEmpty(remotedir) ? filename : Path.Combine(remotedir, filename);
            string uri = Path.Combine(ftpURI, strfilename);
            string localfile = string.IsNullOrWhiteSpace(localdir) ? filename : Path.Combine(localdir, filename);
            ftplog.Info("下载文件路径:" + uri);
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = KeepAlive;
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFTP.UseBinary = UseBinary;
            reqFTP.UsePassive = PassiveMode;
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        FileInfo fileInf = new FileInfo(localfile);
                        FileStream fs = fileInf.OpenWrite();

                        int buffLength = 512;
                        byte[] buff = new byte[buffLength];
                        int contentLen = responseStream.Read(buff, 0, buffLength);
                        if (contentLen != 0)
                        {
                            fs.SetLength(0);
                        }
                        while (contentLen != 0)
                        {
                            fs.Write(buff, 0, contentLen);
                            contentLen = responseStream.Read(buff, 0, buffLength);
                        }
                        fs.Close();
                        response.Close();
                        is_ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp下载文件失败:" + ex.Message);
            }
            return is_ok;
        }
        /// <summary> 
        /// 删除文件 
        /// </summary> 
        /// <param name="fileName"></param> 
        public void Delete(string fileName)
        {
            try
            {
                string uri = ftpURI + fileName;
                ftplog.Info("删除文件路径:" + uri);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = KeepAlive;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.UseBinary = UseBinary;
                reqFTP.UsePassive = PassiveMode;
                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp删除文件失败:" + ex.Message);
            }
        }
        /// <summary> 
        /// 创建文件夹 
        /// </summary> 
        /// <param name="dirName"></param> 
        public void MakeDir(string dirName)
        {
            try
            {
                //dirName = name of the directory to create. 
                string uri = ftpURI + dirName;
                ftplog.Info("上传文件路径:" + uri);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.KeepAlive = KeepAlive;
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = UseBinary;
                reqFTP.UsePassive = PassiveMode;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp创建文件夹失败:" + ex.Message);
            }
        }
        /// <summary> 
        /// 删除文件夹 
        /// </summary> 
        /// <param name="folderName"></param> 
        public void RemoveDirectory(string folderName)
        {
            try
            {
                string uri = ftpURI + folderName;
                ftplog.Info("删除文件路径:" + uri);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = KeepAlive;
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.UseBinary = UseBinary;
                reqFTP.UsePassive = PassiveMode;
                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp删除文件夹失败:" + ex.Message);
            }
        }
        /// <summary> 
        /// 获取当前目录下所有的文件夹列表(仅文件夹) 
        /// </summary> 
        /// <returns></returns> 
        public string[] GetDirectoryList(string dirName="")
        {
            try
            {
                StringBuilder result = new StringBuilder();
                string uri = ftpURI + dirName;
                ftplog.Info("获取文件夹路径:" + uri);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                ftp.KeepAlive = KeepAlive;
                ftp.UseBinary = UseBinary;
                ftp.UsePassive = PassiveMode;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp获取文件夹信息失败:" + ex.Message);
                return null;
            }
        }
        /// <summary> 
        /// 获取当前目录下明细(包含文件和文件夹) 
        /// </summary> 
        /// <returns></returns> 
        public string[] GetFilesDetailList()
        {
            try
            {
                StringBuilder result = new StringBuilder();
                string uri = ftpURI;
                ftplog.Info("获取文件路径:" + uri);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftp.KeepAlive = KeepAlive;
                ftp.UseBinary = UseBinary;
                ftp.UsePassive = PassiveMode;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                ftplog.Error("ftp获取文件夹信息失败:" + ex.Message);
                return null;
            }
        }
    }
}
