using Common;
using Common.Communication;
using Common.Save;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace MyWindowsService
{
    public partial class MyService : ServiceBase
    {
        private const string Tag = "MyService";

        CommHelper commHelper;
        TcpHelper tcpHelper;
        //string filePath = @"D:\MyServiceLog.txt";
        string logDir;
        string iniFilePath;

        public MyService()
        {
            InitializeComponent();
        }

        
        protected override void OnStart(string[] args)
        {
            //using (FileStream stream = new FileStream(filePath, FileMode.Append))
            //using (StreamWriter writer = new StreamWriter(stream))
            //{
            //    writer.WriteLine($"{DateTime.Now},服务启动！");
            //}
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.ProcessName == "MyWindowsService")
                {
                    logDir = prc.MainModule.FileName.Replace("MyWindowsService.exe","Log");
                    iniFilePath = prc.MainModule.FileName.Replace("MyWindowsService.exe", "config.ini");
                    Logger.Instance.init(logDir, null);
                }
                
            }
            IniFileOpt ini = new IniFileOpt(iniFilePath);


            commHelper = new CommHelper();
            commHelper.checkUsbState();
            commHelper.OnRelayDate = relayData;

            string ip = "192.168.1.10";
            int port = 10080;
            try
            {
                ip = ini.read(IniFileOpt.IP);
                port = int.Parse(ini.read(IniFileOpt.Port));
                Logger.Instance.i(Tag,"连接服务器IP："+ip+"  端口："+port.ToString());
            }
            catch (Exception e) {
                Logger.Instance.i(Tag,"ini配置文件出错！");
                Logger.Instance.i(Tag,e.ToString());
            }
            tcpHelper = new TcpHelper(ip,port);
            tcpHelper.onUncompressData = tcpReceiveData;
            tcpHelper.openSockect();

            Logger.Instance.i(Tag, "服务启动");

        }

        

        protected override void OnStop()
        {
            //using (FileStream stream = new FileStream(filePath, FileMode.Append))
            //using (StreamWriter writer = new StreamWriter(stream))
            //{
            //    writer.WriteLine($"{DateTime.Now},服务停止！");
            //}

            commHelper?.unCheckUsbState();
            commHelper = null;
            tcpHelper?.closeSocket();
            tcpHelper = null;

            Logger.Instance.i(Tag, "服务停止");
        }

        /// <summary>
        /// 接收串口的数据,由网口转发
        /// </summary>
        /// <param name="bytes"></param>
        private void relayData(byte[] bytes) {
            Logger.Instance.i(Tag, "串口接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
            tcpHelper?.sendMessage(bytes);
            
        }
        private int tcpReceiveData(byte[] data,int len ) {
            byte[] bytes = new byte[len];
            Array.Copy(data, bytes, len);
            Logger.Instance.i(Tag, "网络接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
            commHelper?.sendMessage(bytes);
            
            return 0;
        }


    }
}
