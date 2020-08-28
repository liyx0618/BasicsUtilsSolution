using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace Util.Log
{
    /// <summary>
    /// 日志通用接口ILogger的实现类,使用Log4net书写日志.
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        #region PrivateMembers
        private ILog log;
        #endregion

        #region ILogger Members

        /// <summary>
        /// ILogger接口CreateLogger实现方法:根据类型创建Logger
        /// </summary>
        /// <param name="loggerType">Class Type</param>
        public void CreateLogger(Type loggerType)
        {
            //log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(loggerType);
        }
        /// <summary>
        /// ILogger接口CreateLogger实现方法:根据字符串创建Logger
        /// </summary>
        /// <param name="name"></param>
        public void CreateLogger(string name)
        {
            //log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(name);
        }


        /// <summary>
        /// 日志属性存取器
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            set { ThreadContext.Properties[propertyName] = value; }
        }

        /// <summary>
        /// Debug级别日志处理
        /// </summary>
        /// <param name="message">Messege for logging</param>
        public void Debug(string message)
        {
            if (log.IsDebugEnabled)
                log.Debug(message);
        }

        /// <summary>
        /// Fatal级别日志处理
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message)
        {
            if (log.IsFatalEnabled)
                log.Fatal(message);
        }

        /// <summary>
        /// Info级别日志处理
        /// </summary>
        /// <param name="message">Message</param>
        public void Info(string message)
        {
            if (log.IsInfoEnabled)
                log.Info(message);
        }

        /// <summary>
        /// Error级别日志处理
        /// </summary>
        /// <param name="message">Message</param>
        public void Error(string message)
        {
            if (log.IsErrorEnabled)
                log.Error(message);
        }

        /// <summary>
        /// Warn级别日志处理
        /// </summary>
        /// <param name="message">Message</param>
        public void Warn(string message)
        {
            if (log.IsWarnEnabled)
                log.Warn(message);
        }

        #endregion
    }
}
