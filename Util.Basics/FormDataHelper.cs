using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Util.Basics
{

    /// <summary>
    /// HttpListenner监听Post请求参数值实体
    /// </summary>
    public class HttpListenerPostValue
    {
        /// <summary>
        /// 0=> 参数
        /// 1=> 文件
        /// </summary>
        public int type = 0;
        public string name;
        public byte[] datas;
    }

    /// <summary>
    /// FormData帮助类
    /// 处理 HttpListener 接收到的Form-Data数据  (multipart/form-data;)
    /// </summary>
    public class FormDataHelper
    {
        /// <summary>
        /// 字节比较
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="comparison">比较数据</param>
        /// <returns>比较结果</returns>
        private bool CompareBytes(byte[] source, byte[] comparison)
        {
            try
            {
                int count = source.Length;
                if (source.Length != comparison.Length)
                    return false;
                for (int i = 0; i < count; i++)
                    if (source[i] != comparison[i])
                        return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取行数据转字节流
        /// </summary>
        /// <param name="SourceStream">源数据流</param>
        /// <returns>字节流</returns>
        private byte[] ReadLineAsBytes(Stream SourceStream)
        {
            var resultStream = new MemoryStream();
            while (true)
            {
                int data = SourceStream.ReadByte();
                resultStream.WriteByte((byte)data);
                if (data == 10 || data == -1)
                    break;
            }
            resultStream.Position = 0;
            byte[] dataBytes = new byte[resultStream.Length];
            resultStream.Read(dataBytes, 0, dataBytes.Length);
            return dataBytes;
        }
        /// <summary>
        /// 获取Post过来的参数和数据
        /// <param name="request">请求字段</param>
        /// </summary>
        /// <returns>接收的数据</returns>
        public List<HttpListenerPostValue> GetHttpListenerPostValue(HttpListenerRequest request)
        {
            try
            {
                List<HttpListenerPostValue> HttpListenerPostValueList = new List<HttpListenerPostValue>();
                if (request.ContentType.Length > 20 && string.Compare(request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0)
                {
                    string[] HttpListenerPostValue = request.ContentType.Split(';').Skip(1).ToArray();
                    string boundary = string.Join(";", HttpListenerPostValue).Replace("boundary=", "").Trim();
                    byte[] ChunkBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
                    byte[] EndBoundary = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
                    Stream SourceStream = request.InputStream;
                    var resultStream = new MemoryStream();
                    List<byte> currentByte = new List<byte>();
                    bool CanMoveNext = true;
                    HttpListenerPostValue data = null;
                    int i = 0;
                    while (CanMoveNext)
                    {
                        i++;
                        if (i > 64) break;
                        byte[] currentChunk = ReadLineAsBytes(SourceStream);
                        if (!Encoding.UTF8.GetString(currentChunk).Equals("\r\n"))
                        { 
                            resultStream.Write(currentChunk, 0, currentChunk.Length);
                        }
                        if (CompareBytes(ChunkBoundary, currentChunk))
                        {
                            byte[] result = new byte[resultStream.Length - ChunkBoundary.Length];
                            resultStream.Position = 0;
                            resultStream.Read(result, 0, result.Length);
                            CanMoveNext = true;
                            if (result.Length > 0)
                                data.datas = result;
                            data = new HttpListenerPostValue();
                            HttpListenerPostValueList.Add(data);
                            resultStream.Dispose();
                            resultStream = new MemoryStream();
                            currentByte.AddRange(result);

                        }
                        else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Disposition"))
                        {
                            byte[] result = new byte[resultStream.Length - 2];
                            resultStream.Position = 0;
                            resultStream.Read(result, 0, result.Length);
                            CanMoveNext = true;
                            data.name = Encoding.UTF8.GetString(result).Replace("Content-Disposition: form-data; name=\"", "").Replace("\"", "").Split(';')[0];
                            resultStream.Dispose();
                            resultStream = new MemoryStream();
                            currentByte.AddRange(result);
                        }
                        else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Type"))
                        {
                            CanMoveNext = true;
                            data.type = 1;
                            resultStream.Dispose();
                            resultStream = new MemoryStream();
                        }
                        else if (CompareBytes(EndBoundary, currentChunk))
                        {
                            byte[] result = new byte[resultStream.Length - EndBoundary.Length - 2];
                            resultStream.Position = 0;
                            resultStream.Read(result, 0, result.Length);
                            data.datas = result;
                            resultStream.Dispose();
                            CanMoveNext = false;
                            currentByte.AddRange(result);
                        }
                    }

                    if (currentByte != null && currentByte.Count > 0)
                    {
                        //获取数据中有空白符需要去掉，输出的就是post请求的参数字符串 如：username=linezero
                        string postJson = Encoding.UTF8.GetString(currentByte.ToArray()).Replace("�", "");
                        Debug.WriteLine(string.Format("停车服务路径:{0},接收post原始数据:{1}", request.RawUrl, postJson));
                    }
                }
                return HttpListenerPostValueList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
