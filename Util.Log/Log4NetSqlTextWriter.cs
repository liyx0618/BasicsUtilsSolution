using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Util.Log
{
    public class Log4NetSqlTextWriter : TextWriter
    {
        private ILogger _logger;
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private LogLevelType _logLevelType;
        public LogLevelType LogLevel
        {
            get { return _logLevelType; }
            set { _logLevelType = value; }
        }

        private Encoding _encoding;
        public override Encoding Encoding
        {
            get
            {
                if (_encoding == null)
                {
                    _encoding = new UnicodeEncoding(false, false);
                }
                return _encoding;
            }

        }

        public Log4NetSqlTextWriter()
        {

        }

        public Log4NetSqlTextWriter(ILogger logger, LogLevelType logLevel)
        {
            _logger = logger;
            _logLevelType = logLevel;
        }

        public override void Write(string value)
        {
            // 根据ILogger中电议的日志级别,书写日志

            switch (_logLevelType)
            {
                case LogLevelType.Fatal: _logger.Fatal(value); break;
                case LogLevelType.Error: _logger.Error(value); break;
                case LogLevelType.Warn: _logger.Warn(value); break;
                case LogLevelType.Info: _logger.Info(value); break;
                case LogLevelType.Debug: _logger.Debug(value); break;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }
    }
}
