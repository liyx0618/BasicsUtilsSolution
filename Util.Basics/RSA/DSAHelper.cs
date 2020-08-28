using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Util.Basics
{
    /// <summary>
    /// DSA工具类
    /// </summary>
    public class DSAHelper : AsymmetricAlgorithmHelper<DSACryptoServiceProvider>
    {
        /// <summary>
        /// DSA签名
        /// </summary>
        /// <param name="privatekey">私钥</param>
        /// <param name="content">需签名的原始数据(utf-8)</param>
        /// <returns>签名后的值(base64格式)</returns>
        public static string SignData(string privatekey, string content)
        {
            return Execute(privatekey,
                algorithm => Convert.ToBase64String(algorithm.SignData(Encoding.UTF8.GetBytes(content))));
        }

        /// <summary>
        /// DSA验签
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="content">需验证签名的数据(utf-8)</param>
        /// <param name="signature">需验证的签名字符串(base64格式)</param>
        /// <returns></returns>
        public static bool VerifyData(string publicKey, string content, string signature)
        {
            return Execute(publicKey,
                algorithm => algorithm.VerifyData(Encoding.UTF8.GetBytes(content), Convert.FromBase64String(signature)));
        }

        /// <summary>
        /// 生成公钥、私钥
        /// </summary>
        /// <param name="publicKey">公钥（Xml格式）</param>
        /// <param name="privateKey">私钥（Xml格式）</param>
        /// <param name="keySize">要生成的KeySize，支持的MinSize:512 MaxSize:1024 SkipSize:64</param>
        public static void Create(out string publicKey, out string privateKey, int keySize = 1024)
        {
            DSACryptoServiceProvider provider = new DSACryptoServiceProvider(keySize);
            KeyGenerator.CreateAsymmetricAlgorithmKey(out publicKey, out privateKey, provider);
        }
    }
}
