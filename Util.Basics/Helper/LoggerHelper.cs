﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Util.Basics.Helper
{
    public static class LoggerHelper
    {
        private static readonly ILoggerRepository Repository = LogManager.CreateRepository("NETCoreRepository");
        private static readonly ILog Log = LogManager.GetLogger(Repository.Name, "NETCorelog4net");

        static LoggerHelper()
        {
            XmlConfigurator.Configure(Repository, new FileInfo("log4net.config"));
        }

        #region 文本日志

        /// <summary>
        /// 文本日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void WriteToFile(string message, Exception ex)
        {
            if (string.IsNullOrEmpty(message))
                message = ex.Message;

            Log.Error(message, ex);
        }

        /// <summary>
        /// 文本日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void WriteToFile(string message)
        {
            Log.Info(message);
        }
        #endregion
    }
}
