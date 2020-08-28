using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Util.Basics
{
    /// <summary>
    /// RSA 加密工具类
    /// </summary>
    public class RSAHelper : AsymmetricAlgorithmHelper<RSACryptoServiceProvider>
    {
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey">公钥</param>
        /// <param name="content">加密前的原始数据</param>
        /// <param name="fOAEP">如果为 true，则使用 OAEP 填充（仅在运行 Microsoft Windows XP 或更高版本的计算机上可用）执行直接的 System.Security.Cryptography.RSA加密；否则，如果为 false，则使用 PKCS#1 1.5 版填充。</param>
        /// <returns>加密后的结果（base64格式）</returns>
        public static string Encrypt(string publickey, string content, bool fOAEP = false)
        {
            return Execute(publickey,
                algorithm => Convert.ToBase64String(algorithm.Encrypt(Encoding.UTF8.GetBytes(content), fOAEP)));
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey">私钥</param>
        /// <param name="content">加密后的内容(base64格式)</param>
        /// <param name="fOAEP">如果为 true，则使用 OAEP 填充（仅在运行 Microsoft Windows XP 或更高版本的计算机上可用）执行直接的 System.Security.Cryptography.RSA加密；否则，如果为 false，则使用 PKCS#1 1.5 版填充。</param>
        /// <returns></returns>
        public static string Decrypt(string privatekey, string content, bool fOAEP = false)
        {
            return Execute(privatekey,
                algorithm => Encoding.UTF8.GetString(algorithm.Decrypt(Convert.FromBase64String(content), fOAEP)));
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="privatekey">私钥</param>
        /// <param name="content">需签名的原始数据(utf-8)</param>
        /// <param name="halg">签名采用的算法，如果传null，则采用MD5算法  </param>
        /// <returns>签名后的值(base64格式)</returns>
        public static string SignData(string privatekey, string content, object halg = null)
        {
            try
            {
                return Execute(privatekey,
                    algorithm => Convert.ToBase64String(algorithm.SignData(Encoding.UTF8.GetBytes(content), GetHalg(halg))));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="content">需验证签名的数据(utf-8)</param>
        /// <param name="signature">需验证的签名字符串(base64格式)</param>
        /// <param name="halg">签名采用的算法，如果传null，则采用MD5算法</param>
        /// <returns></returns>
        public static bool VerifyData(string publicKey, string content, string signature, object halg = null)
        {
            return Execute(publicKey,
                algorithm => algorithm.VerifyData(Encoding.UTF8.GetBytes(content), GetHalg(halg), Convert.FromBase64String(signature)));
        }

        private static object GetHalg(object halg)
        {
            if (halg == null)
            {
                halg = "MD5";
            }
            return halg;
        }

        /// <summary>
        /// 生成公钥、私钥
        /// </summary>
        /// <param name="publicKey">公钥（Xml格式）</param>
        /// <param name="privateKey">私钥（Xml格式）</param>
        /// <param name="keySize">要生成的KeySize，支持的MinSize:384 MaxSize:16384 SkipSize:8</param>
        public static void Create(out string publicKey, out string privateKey, int keySize = 1024)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize);
            KeyGenerator.CreateAsymmetricAlgorithmKey(out publicKey, out privateKey, provider);
        }


        static void Main(string[] args)
        {
            string publicJavaKey, privateJavaKey, content, publicCSharpKey, privateCSharpKey, signData;
            //java的base64格式秘钥  
            privateJavaKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAMsNFZUmKwtwFH271JMbjuMslL2C4Dw5iYps/jkXu5vxhYzNaKMA5Bu2Adlhw2FzA/fapglWDN68/8OG1veX0M9C/5YhigsIdhzixi6WrkMFfqp/nZhcRgcm900P0UM1wAceemPEvU7+uq9bSrMJWxwSmcryP63KKtCZcFfW05u9AgMBAAECgYBIiisctp8IHglkBddimqTIaePVdE0RluiZKGkGEoF2q6kvbS6llSro73PnqjJ3vPQ89sL8cN52MIUa4DAqEfWJgtl/cy2RKK64ajESZadIQnW2F2Whsuob/T1wEv05jQeej4qUdVp5yz1lhWvwlwoB3BLWPwhnReg87OS8jFFQLQJBAPEA0kv1OIhlQx1ew1WEqd8mK2VbtmLDOwZL4uZvDkx7/dtthVxNw25cVfJM6V9AET2j9Jbyt2pHibfjf1fICmMCQQDXr6+yfl7yLOVDiM4lQFH7if03GsJsfvizvUyoI6suumyTFhjfkDyL3JM4flyWDpFPN76b9TDpjaz4pY1KfItfAkBG0ZD6VRLJscfpB4GqzZMFSbgSzsJnfysHDKGeSSAQhZbxNdusZgV5lpSC4Orq3G60iEtFWAlhp6fma2luKBA1AkEArS3OR+Y5u/+aUcBtrPFZIjvoia89vrmwXTk4bKS/FPTwqqUKca8xPidsOecT1hR6Tf33WOflTxHHeZoLuWwIxwJAZQ4fS9Gatxyn+3ubXxltbyKxpYhfwEP1JvIQqOACtKLXBjWoYzdnUO2jr6WZ2UBeaDCqzA30VGmjG7pud94ZhA==";
            publicJavaKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDLDRWVJisLcBR9u9STG47jLJS9guA8OYmKbP45F7ub8YWMzWijAOQbtgHZYcNhcwP32qYJVgzevP/Dhtb3l9DPQv+WIYoLCHYc4sYulq5DBX6qf52YXEYHJvdND9FDNcAHHnpjxL1O/rqvW0qzCVscEpnK8j+tyirQmXBX1tObvQIDAQAB";
            content = "测试数据你好";
            //content = "appId=dbdfc701e2a11b5924cd2daba367f1f9&bizContent={\"parkingCode\":\"1ac02914bb3e4fad9c23d7012f1e1c11\",\"carCode\":\"carCode\",\"inTime\":\"2018-06-01 11:20:00\",\"inChannelName\":\"进口通道\",\"inType\":1,\"inCarType\":1,\"inImageArray\":\"[{\\\"plateImage\\\":\\\"http://192.168.10.125:9092/Capture_Images/20180524/浙C6699P/浙C6699P_20180524155711411.jpg\\\",\\\"carImage\\\":\\\"http://192.168.10.125:9092/Capture_Images/20180524/浙C6699P/浙C6699P_20180524155711411.jpg\\\"}]\",\"parkCode\":\"SZ001\",\"parkProviderCode\":\"LF\"}&orgCode=G001Z002C0013&timestamp=1528354063";
            //转成C#的xml格式  
            publicCSharpKey = RSAKeyConvert.RSAPublicKeyJava2DotNet(publicJavaKey);
            privateCSharpKey = RSAKeyConvert.RSAPrivateKeyJava2DotNet(privateJavaKey);
            Console.WriteLine("转换得到的C#公钥：" + publicCSharpKey);
            Console.WriteLine("转换得到的C#私钥：" + privateCSharpKey);

            //RSAHelper.Create(out publicCSharpKey, out privateCSharpKey, 1024);  
            string encData = RSAHelper.Encrypt(publicCSharpKey, content);
            Console.WriteLine("公钥加密结果：" + encData);
            Console.WriteLine("私钥解密结果：" + RSAHelper.Decrypt(privateCSharpKey, encData));
            //下面是java通过SHA1WithRSA生成的签名  
            //Dv67xT5SgGQ9q+bKVWuyyxljx28cxNkIMDk5ro8cMopsiPf7Z8/n/02yaN/SVUQPmWJk/f+cjwydikVStwjkll49/D4PrTW+nd4XWr5hea8n7c6JTdRvaOGwFG3Do1n8Sndj7aqxuUWUmlLiC1dYEHeZhSwm9BCMJJSvF8n34CY=  
            //下面是JAVA通过MD5withRSA生成的签名  
            //MUXPVxxNZOlzDY03hOXQgQLQnJ/SrJa0lxQAx8Kl+H+pLBcL6cqdLupVwK6mwKZ1mRP2CCwGaQC8wHkOVRafPdkOSRsnKnkAjRv1iqHBxJtPCG83XlrB7AofzqHi/VULCA9KdWqmvnarVCV+lVwwUVCXP5cK1nwEJN258T/eV8M=  
            //下面是JAVA通过SHA256WithRSA生成的签名  
            //qPfkIAITcKW452/NacSQHjNbBUtJNhel4SpTMp1T/nGaY0Z4I3Xx13/aVl001ZKwBfdFf7cIPAKlbqmywm3sqEzVpBQlVOYMZBARlHAoOexTCZk50tgrCFUlXXa2pWt+jRS2lGUX5esbo6cKS0Yk1fdkYlm+4S4NRKYgEAXO+lY=  
            string halg = "SHA1";//SHA1 MD5 SHA256  
            signData = RSAHelper.SignData(privateCSharpKey, content, halg);//SHA1  
            Console.WriteLine("生成签名：" + signData);
            Console.WriteLine("签名一致：" + RSAHelper.VerifyData(publicCSharpKey, content, signData, halg));
        }
    }
}
