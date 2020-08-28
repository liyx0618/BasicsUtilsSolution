using System;
using System.Collections.Generic;
using System.Text;
using log4net.Layout;
using log4net.Core;

namespace Util.Log
{
    public class Log4NetSqlLayout : LayoutSkeleton
    {
        private PatternLayout m_nestedLayout;
        public PatternLayout PatternLayout
        {
            get { return m_nestedLayout; }
            set { m_nestedLayout = value; }
        }

        /// <summary>
        /// 日志选项
        /// </summary>
        public override void ActivateOptions()
        {
            this.IgnoresException = true;
            this.Footer = "";
            this.Header = "";
        }


        /// <summary>
        /// 形成日志格式
        /// </summary>
        /// <param name="writer">日志书写器</param>
        /// <param name="loggingEvent">日志事件</param>
        public override void Format(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            string msg = loggingEvent.MessageObject.ToString();
            writer.WriteLine(msg);
        }
    }
}
