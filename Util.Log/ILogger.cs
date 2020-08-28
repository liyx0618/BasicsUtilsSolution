using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Log
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 为特殊日志类型创建日志
        /// </summary>
        /// <param name="loggerType">日志类型</param>
        void CreateLogger(Type loggerType);
        /// <summary>
        /// 为特殊日志类型创建日志
        /// </summary>
        /// <param name="name">日志名称</param>
        void CreateLogger(string name);

        /// <summary>
        /// 日志属性存取器
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        string this[string propertyName]
        {
            set;
        }


        //根据日志级别分类,写日志函数
        void Debug(string message);
        void Fatal(string message);
        void Info(string message);
        void Error(string message);
        void Warn(string message);
    }

    // 日志级别
    public enum LogLevelType
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }
}
