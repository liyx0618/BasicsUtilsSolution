using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reformer.Utils
{
    public class SingleBase<T> where T :class, new()
    {
        private static readonly T _instance;
        private static readonly object _singleLock = new object();

        /// <summary>
        /// 获取单例句柄
        /// </summary>
        public static T Instance
        {
            get { return _instance; }
        }

        static SingleBase()
        {
            if (_instance == null)
            {
                lock (_singleLock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
        }

        public SingleBase()
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("单例模式设计，不允许重复实例化对象");
            }
        }
    }
}
