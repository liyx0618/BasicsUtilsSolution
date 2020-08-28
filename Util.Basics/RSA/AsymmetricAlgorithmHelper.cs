using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Util.Basics
{
    /// <summary>
    /// 非对称算法辅助器帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsymmetricAlgorithmHelper<T>
        where T : AsymmetricAlgorithm, new()
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="key">秘钥</param>
        /// <param name="func">执行方法</param>
        /// <returns></returns>
        protected static TResult Execute<TResult>(string key, Func<T, TResult> func)
        {
            using (T algorithm = new T())
            {
                algorithm.FromXmlString(key);
                return func(algorithm);
            }
        }

        /// <summary>
        /// 按默认规则生成公钥、私钥
        /// </summary>
        /// <param name="publicKey">公钥（Xml格式）</param>
        /// <param name="privateKey">私钥（Xml格式）</param>
        public static void Create(out string publicKey, out string privateKey)
        {
            KeyGenerator.CreateAsymmetricAlgorithmKey<T>(out publicKey, out privateKey);
        }
    }
}
