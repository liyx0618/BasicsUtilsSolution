using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Runtime.InteropServices;

namespace FatalErrorRestart
{
    /// <summary>
    /// 服务程序安装、卸载、配置类
    /// </summary>
    public class ServiceHelper
    {
        /// <summary>
        ///  安装和运行，使用win32api
        /// </summary>
        /// <param name="svcPath">C#安装程序路径</param>
        /// <param name="svcName">服务名</param>
        /// <param name="svcDispName"> 服务显示名称</param>
        /// <returns>服务安装是否成功</returns>
        public bool InstallService(string svcPath, string svcName, string svcDispName)
        {
            #region Constants declaration.
            int SC_MANAGER_CREATE_SERVICE = 0x0002;
            int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            //int SERVICE_DEMAND_START = 0x00000003;   
            int SERVICE_ERROR_NORMAL = 0x00000001;
            int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            int SERVICE_QUERY_CONFIG = 0x0001;
            int SERVICE_CHANGE_CONFIG = 0x0002;
            int SERVICE_QUERY_STATUS = 0x0004;
            int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            int SERVICE_START = 0x0010;
            int SERVICE_STOP = 0x0020;
            int SERVICE_PAUSE_CONTINUE = 0x0040;
            int SERVICE_INTERROGATE = 0x0080;
            int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL);
            int SERVICE_AUTO_START = 0x00000002;
            #endregion Constants declaration.

            try
            {
                IntPtr sc_handle = modAPI.OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0)
                {
                    IntPtr sv_handle = modAPI.CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() == 0)
                    {
                        modAPI.CloseServiceHandle(sc_handle);
                        return false;
                    }
                    else
                    {
                        //试尝启动服务   
                        int i = modAPI.StartService(sv_handle, 0, null);
                        if (i == 0)
                        {
                            return false;
                        }
                        modAPI.CloseServiceHandle(sc_handle);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// 删除安装服务。使用win32api
        /// </summary>
        /// <param name="svcName">服务名</param>
        /// <returns></returns>
        public bool UnInstallServiceEx(string svcName)
        {
            int GENERIC_WRITE = 0x40000000;
            IntPtr sc_hndl = modAPI.OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0)
            {
                int DELETE = 0x10000;
                IntPtr svc_hndl = modAPI.OpenService(sc_hndl, svcName, DELETE);
                if (svc_hndl.ToInt32() != 0)
                {
                    int i = modAPI.DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        modAPI.CloseServiceHandle(sc_hndl);
                        return true;
                    }
                    else
                    {
                        modAPI.CloseServiceHandle(sc_hndl);
                        return false;
                    }
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 安装服务，使用.net
        /// </summary>
        /// <param name="stateSaver">服务名</param>
        /// <param name="filepath">安装服务路径</param>
        private void InstallService(IDictionary stateSaver, string filepath)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("ServiceName");
                if (!ServiceIsExisted("ServiceName"))
                {
                    //Install Service   
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Install(stateSaver);
                    myAssemblyInstaller.Commit(stateSaver);
                    myAssemblyInstaller.Dispose();
                    //--Start Service   
                    service.Start();
                }
                else
                {
                    if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running && service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                    {
                        service.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("installServiceError/n" + ex.Message);
            }
        }
        
        /// <summary>
        /// 卸载windows服务，使用.net
        /// </summary>
        /// <param name="filepath">服务路径</param>
        private void UnInstallService(string filepath)
        {
            try
            {
                if (ServiceIsExisted("ServiceName"))
                {
                    //UnInstall Service   
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Uninstall(null);
                    myAssemblyInstaller.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unInstallServiceError/n" + ex.Message);
            }
        }
        
        /// <summary>
        /// 检查服务是否存在
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns></returns>
        private bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 服务程序的ProjectInstall界面上，在AfterInstall事件中调用本函数，设置成自动重启
        /// </summary>
        public static void SeviceConfig_AfterInstall(string ServiceName)
        {
            IntPtr iSCManagerHandle = IntPtr.Zero;
            IntPtr iSCManagerLockHandle = IntPtr.Zero;
            IntPtr iServiceHandle = IntPtr.Zero;
            bool bChangeServiceConfig2 = false;
            modAPI.SERVICE_FAILURE_ACTIONS ServiceFailureActions;
            modAPI.SC_ACTION[] ScActions = new modAPI.SC_ACTION[3];
            bool bCloseService = false;
            bool bUnlockSCManager = false;
            bool bCloseSCManager = false;
            IntPtr iScActionsPointer = new IntPtr();

            try
            {
                //打开服务控制台
                iSCManagerHandle = modAPI.OpenSCManagerA(null, null,
                modAPI.ServiceControlManagerType.SC_MANAGER_ALL_ACCESS);

                if (iSCManagerHandle.ToInt32() < 1)
                {
                    throw new Exception("不能打开服务管理器.");
                }

                iSCManagerLockHandle = modAPI.LockServiceDatabase(iSCManagerHandle);

                if (iSCManagerLockHandle.ToInt32() < 1)
                {
                    throw new Exception("不能锁定服务管理器.");
                }
                //服务名
                iServiceHandle = modAPI.OpenServiceA(iSCManagerHandle, ServiceName,
                modAPI.ACCESS_TYPE.SERVICE_ALL_ACCESS);

                if (iServiceHandle.ToInt32() < 1)
                {
                    throw new Exception("不能打开服务进行修改.");
                }

                ServiceFailureActions.dwResetPeriod = 0; //0天后复位
                ServiceFailureActions.lpRebootMsg = "服务运行出错，正在重启 ...";
                // ServiceFailureActions.lpCommand = "SomeCommand.exe Param1 Param2";
                ServiceFailureActions.lpCommand = "";
                ServiceFailureActions.cActions = ScActions.Length;
                //故障恢复设置，0天后复位
                ScActions[0].Delay = 0;
                ScActions[0].SCActionType = modAPI.SC_ACTION_TYPE.SC_ACTION_RESTART; //重启服务
                ScActions[1].Delay = 0;
                ScActions[1].SCActionType = modAPI.SC_ACTION_TYPE.SC_ACTION_RESTART;
                ScActions[2].Delay = 0;
                ScActions[2].SCActionType = modAPI.SC_ACTION_TYPE.SC_ACTION_RESTART;

                iScActionsPointer = Marshal.AllocHGlobal(Marshal.SizeOf(new modAPI.SC_ACTION()) * 3);

                modAPI.CopyMemory(iScActionsPointer, ScActions, Marshal.SizeOf(new modAPI.SC_ACTION()) * 3);

                ServiceFailureActions.lpsaActions = iScActionsPointer;

                bChangeServiceConfig2 = modAPI.ChangeServiceConfig2A(iServiceHandle,
                modAPI.InfoLevel.SERVICE_CONFIG_FAILURE_ACTIONS, ref ServiceFailureActions);

                if (bChangeServiceConfig2 == false)
                {
                    throw new Exception("不能设置服务的故障恢复设置.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(iScActionsPointer);

                if (iServiceHandle.ToInt32() > 0)
                {
                    bCloseService = modAPI.CloseServiceHandle(iServiceHandle);
                }

                if (iSCManagerLockHandle.ToInt32() > 0)
                {
                    bUnlockSCManager = modAPI.UnlockServiceDatabase(iSCManagerLockHandle);
                }

                if (iSCManagerHandle.ToInt32() != 0)
                {
                    bCloseSCManager = modAPI.CloseServiceHandle(iSCManagerHandle);
                }
            }
        }

    }
}
