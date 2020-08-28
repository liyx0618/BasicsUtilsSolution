using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Util.Basics
{
    public static class KeyValueHelper
    {
        //Key和Value是用“=”
        private static string _matchkey = "=";
        public static string matchKey 
        {
            set { _matchkey = value; }
            get { return _matchkey; }
        }
        //键值之间是“&”隔开的
        private static string _matchvalue = "&";
        private static string matchValue
        {
            set { _matchvalue = value; }
            get { return _matchvalue; }
        }
        /// <summary>
        /// URL数据流转字典
        /// </summary>
        /// <param name="data">url数据流</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetConentByBytes(this byte[] data)
        {
            return getContentByString(Encoding.Default.GetString(data));
        }
        /// <summary>
        /// URL字符串转字典
        /// </summary>
        /// <param name="data">url字符串</param>
        /// <returns></returns>
        public static Dictionary<string, string> getContentByString(this string data)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (data.Substring(data.Length - 1) != matchValue)
            {
                data = data + matchValue;
            }
            try
            {
                int pos = 0;
                int startIndex = 0;
                while (true)
                {
                    //TODO: GET KEY
                    pos = data.IndexOf(matchKey, startIndex);
                    string key = data.Substring(startIndex, pos - startIndex);
                    startIndex = pos + 1;

                    //TODO:GET VALUE
                    pos = data.IndexOf(matchValue, startIndex);
                    string value = data.Substring(startIndex, pos - startIndex);
                    startIndex = pos + 1;

                    dic.Add(key, value);
                    if (startIndex >= data.Length)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Info: " + ex.ToString());
            }
            return dic;
        }
        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out Dictionary<string, string> nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            nvc = new Dictionary<string, string>();
            baseUrl = "";
            if (url == "")
                return;
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);
            // 开始分析参数对  
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
        public static void test()
        {
            string data = "payDetail=[{\"payWayId\":\"0003\",\"payWayName\":\"支付宝\",\"payOrderId\":\"7ee1031a4e3141a7a2bfbbf194896aa6\",\"payAmount\":0.01,\"payDate\":\"20170807143854\"}]&dtChargeNow=20170807143854&payTransNo=71a3872ba8da49ac80f5788742bf131f&orderId=0001201708071433148142";
            Dictionary<string, string> dic = getContentByString(data);
            foreach (KeyValuePair<string, string> item in dic)
            {
                Console.WriteLine(string.Format("KEY:{0} VALUE:{1}", item.Key, item.Value));
            }

            //string ss = Newtonsoft.Json.JsonConvert.SerializeObject(dic).Replace("\"[", "[").Replace("\"]", "]");
            Console.ReadKey();
        }
    }
}
