using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Commons.Utils
{
    /// <summary>
    /// 重新封装Log4Net
    /// 注意点：
    /// 1，Level级别：OFF > FATAL > ERROR > WARN > INFO > DEBUG > ALL
    /// 使用方法如下：
    /// 1， 拷贝配置文件： log4net.xml 到指定项目目录下
    /// 2，配置初始化：在程序启动处调用SetConfig(FileInfo configFile)  
    /// 3，使用LogHelper.Info(xxx)等方法记录日志
    /// </summary>
    public class LogHelper
    {
        public static log4net.ILog logger;
 
        public static void SetConfig(FileInfo configFile)  
        {  
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
        } 
        private static void setLogger(Type type){
            logger = log4net.LogManager.GetLogger(type);
        }
        public static void Debug(string info, Type type)
        {
            setLogger(type);
            logger.Debug(info);
        }
        public static void Debug(string info, Exception e, Type type)
        {
            setLogger(type);
            logger.Debug(info, e);
        }
        public static void Info(string info, Type type)
        {
            setLogger(type);
            logger.Info(info);
        }
        public static void Info(string info, Exception e, Type type)
        {
            setLogger(type);
            logger.Info(info, e);
        }
        public static void Warn(string info, Type type)
        {
            setLogger(type);
            logger.Warn(info);
        }
        public static void Warn(string info, Exception e, Type type)
        {
            setLogger(type);
            logger.Warn(info, e);
        }
        public static void Error(string info, Type type)
        {
            setLogger(type);
            logger.Error(info);
        }
        public static void Error(string info, Exception e, Type type)
        {
            setLogger(type);
            logger.Error(info, e);
        } 

    }  


}
