using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.Basics.Extension;

namespace Util.Basics.Helper
{
    /// <summary>
    /// 特性扩展类
    /// </summary>
    public static class AttributeHelper<T> where T : class
    {
        private static readonly ConcurrentDictionary<Type, T> AttributeDic =
            new ConcurrentDictionary<Type, T>();

        /// <summary>
        /// 获取实体特性信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetAttribute(Type type)
        {
            return AttributeDic.GetOrAdd(type, item =>
            {
                var customAttribute = type.GetCustomAttributes(typeof(T), false).FirstOrDefault();

                if (customAttribute.IsNotNull())
                    return customAttribute as T;

                return null;
            });

        }
    }
}
