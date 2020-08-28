using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Util.Log
{
    public class MyLogManager
    {
        private static ILoggerRepository _loggerRepository;

        private static ILoggerRepository LoggerRepository
        {
            get
            {
                if (_loggerRepository != null)
                {
                    return _loggerRepository;
                }
                _loggerRepository = LogManager.CreateRepository("MyLogManager");
                XmlConfigurator.ConfigureAndWatch(_loggerRepository, new FileInfo("log4net.config"));
                return _loggerRepository;
            }
        }

        public static ILog GetMyLog<T>(T t)
        {
            return LogManager.GetLogger(LoggerRepository.Name, t.GetType());
        }

        public static ILog GetMyLog(object obj)
        {
            return LogManager.GetLogger(LoggerRepository.Name, obj.GetType());
        }

        public static ILog GetMyLog(Type type)
        {
            return LogManager.GetLogger(LoggerRepository.Name, type);
        }

        public static ILog GetMyLog()
        {
            return LogManager.GetLogger(LoggerRepository.Name, "GetMyLog");
        }

        public static void Info(string msg)
        {
            GetMyLog().Info(msg);
        }
    }
}
