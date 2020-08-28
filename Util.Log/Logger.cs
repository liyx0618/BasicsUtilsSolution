using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;
using log4net.Layout;
using log4net.Core;

namespace Util.Log
{
    /// <summary>
    /// 封装log4net的ILog，外部调时不必再引用log4net
    /// </summary>
    public class UtilLog 
    {
        /// <summary>
        /// 保存log实例
        /// </summary>
        private ILog log;
        //private static ILog log;

        /// <summary>
        /// 指定日志文件的配置名称
        /// </summary>
        /// <param name="LogName"></param>
        public UtilLog(string LogName)
        {
            LogConfig();
            log = LogManager.GetLogger(LogName);
        }
        public UtilLog(Type type)
        {
            LogConfig();
            log = LogManager.GetLogger(type);
        }
        /// <summary>
        /// 无参数创建时，默认使用"log"
        /// </summary>
        public UtilLog()
        {
            LogConfig();
            log = LogManager.GetLogger("log");
        }

        /// <summary>
        /// 定位日志配置文件的路径
        /// </summary>
        private void LogConfig()
        {
            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "log4net.config");
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));
        }
        public void Info(string msg)
        {
            log.Info(msg);
        }
        public void Error(string msg)
        {
            log.Error(msg);
        }
        public void Error(string msg, Exception ex)
        {
            log.Error(msg, ex);
        }
        public void Debug(string msg)
        {
            log.Debug(msg);
        }
        public void Debug(string msg, Exception ex)
        {
            log.Debug(msg, ex);
        }
    }

    public static class Log
    {
        /// <summary>
        /// 保存log实例
        /// </summary>
        private static UtilLog log = new UtilLog();
        
        public static void Info(string msg)
        {
            log.Info(msg);
        }
        public static void Error(string msg)
        {
            log.Error(msg);
        }
        public static void Error(string msg, Exception ex)
        {
            log.Error(msg, ex);
        }
    }
}
