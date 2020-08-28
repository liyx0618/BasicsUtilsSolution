using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ServiceProcess;

namespace FatalErrorRestart
{
    public class FatalErrorSet
    {
        #region 软件未处理异常相关的系统调用
        static UnhandledExceptionFilter filter;
        public static int MyUnhandledExceptionFilter(IntPtr pExceptionInfo)
        {
            //软件未处理异常的处理，直接退出，等待外部重启
            Application.Exit();
            Environment.Exit(1);
            return 1;
        }

        private delegate int UnhandledExceptionFilter(IntPtr lpTopLevelExceptionFilter);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr SetUnhandledExceptionFilter(UnhandledExceptionFilter filter);

        private static uint SEM_FAILCRITICALERRORS = 0x1; //无严重错误的弹出框
        private static uint SEM_NOGPFAULTERRORBOX = 0x2; //无一般性错误的弹出框
        private static uint SEM_NOOPENFILEERRORBOX = 0x8000; //无打开文件异常弹出框
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SetErrorMode(uint uMode);
        #endregion

        public static void SetErrorRestart()
        {
            //设置程序的异常处理
            filter = new UnhandledExceptionFilter(MyUnhandledExceptionFilter);
            SetUnhandledExceptionFilter(filter);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            SetErrorMode(SEM_FAILCRITICALERRORS + SEM_NOOPENFILEERRORBOX + SEM_NOGPFAULTERRORBOX);
        }

    }
}
