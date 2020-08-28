using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Basics
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 泛型添加数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (items == null)
                throw new ArgumentNullException("items");

            target.AddRange(items);

            //foreach (var item in items)
            //    target.Add(item);
        }
        /// <summary>
        /// 创建字典键值并返回键值
        /// </summary>
        /// <typeparam name="TKey">键值KEY 类型</typeparam>
        /// <typeparam name="TValue">键值Value 类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键值名</param>
        /// <param name="createValueCallback">创建键值value</param>
        /// <returns></returns>
        public static TValue GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createValueCallback)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (!dictionary.ContainsKey(key))
                lock (dictionary)
                    if (!dictionary.ContainsKey(key))
                        dictionary[key] = createValueCallback();

            return dictionary[key];
        }
        /// <summary>
        /// 键值对泛型集合转字典
        /// </summary>
        /// <typeparam name="TKey">键值KEY 类型</typeparam>
        /// <typeparam name="TValue">键值Value 类型</typeparam>
        /// <param name="source">键值对集合</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.ToDictionary(m => m.Key, m => m.Value);
        }
        /// <summary>
        /// 判断泛型集合是否为空
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public static bool Empty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        /// <summary>
        /// An order independent version of <see cref="Enumerable.SequenceEqual{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Collections.Generic.IEnumerable{TSource})"/>.
        /// </summary>
        public static bool SetEqual<T>(this IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");

            var objectsInX = x.ToList();
            var objectsInY = y.ToList();

            if (objectsInX.Count() != objectsInY.Count())
                return false;

            foreach (var objectInY in objectsInY)
            {
                if (!objectsInX.Contains(objectInY))
                    return false;

                objectsInX.Remove(objectInY);
            }

            return objectsInX.Empty();
        }
    }
}
