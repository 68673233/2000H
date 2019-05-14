using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using WindowsServiceClient.Manage;

namespace WindowsServiceClient.service
{
    class ServiceOpt
    {
        public ServiceOpt(string serviceName) {
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName;


        Action<int> _serviceState = null;
        /// <summary>
        /// 服务状态
        /// </summary>
        public Action<int> OnServiceState {
            set { _serviceState = value; }
        }

        public void serviceStateThreadCheck() {
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), new WaitOrTimerCallback(checkServiceState), null, 3000, false);
        }

        private void checkServiceState(object state, bool timedout) {
           int index= getServiceState(ServiceName);
            if (_serviceState != null) _serviceState(index);
        }

        public int getServiceState(string serviceName) {
            var serviceControllers = ServiceController.GetServices();
            var server = serviceControllers.FirstOrDefault(service => service.ServiceName==serviceName);
            if (server == null) return 0;
            return (int)server.Status;
        }
        //引用“System.ServiceProcess”及“System.Configuration.Install”
        //判断服务是否存在
        public bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName.ToLower() == serviceName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //安装服务
        public void InstallService(string serviceFilePath)
        {
            bool b= IsServiceExisted(PathManage.ServiceName);
            if (b == true) return;
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                IDictionary savedState = new Hashtable();
                installer.Install(savedState);
                installer.Commit(savedState);
            }
        }

        //卸载服务
        public void UninstallService(string serviceFilePath)
        {
            bool b = IsServiceExisted(PathManage.ServiceName);
            if (b == false) return;
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                installer.Uninstall(null);
            }
        }
        //启动服务
        public void ServiceStart(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Stopped)
                {
                    control.Start();
                }
            }
        }

        //停止服务
        public void ServiceStop(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Running)
                {
                    control.Stop();
                }
            }
        }
    }
}
