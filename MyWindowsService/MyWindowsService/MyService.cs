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
using System.Threading;
using System.Threading.Tasks;


namespace MyWindowsService
{
    public partial class MyService : ServiceBase
    {
        private const string Tag = "ME2000HService";
        private const string ProcessName = "ME2000HServiceProcess";
        private const string ProcessExeName = "ME2000HServiceProcess.exe";

        CommHelper commHelper;
        TcpHelper tcpHelper;
        string logDir;
        string iniFilePath;

        public MyService()
        {
            InitializeComponent();
        }

        
        protected override void OnStart(string[] args)
        {
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.ProcessName == ProcessName)
                {
                    logDir = prc.MainModule.FileName.Replace(ProcessExeName, "Log");
                    iniFilePath = prc.MainModule.FileName.Replace(ProcessExeName, "config.ini");
                    Logger.Instance.init(logDir, null);
                }
                
            }
            initLoggerComm();
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
            string isConn = tcpHelper.IsConn().ToString();
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState,  isConn).toBytes());

            

            Logger.Instance.i(Tag, "服务启动");

        }

        

        protected override void OnStop()
        {
            string isConn = "False";
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState, isConn).toBytes());
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SerialState, isConn).toBytes());

            commHelper?.unCheckUsbState();
            commHelper = null;
            tcpHelper?.closeSocket();
            tcpHelper = null;

            Logger.Instance.stopSocketService();
            Logger.Instance.i(Tag, "服务停止");
        }

        private void initLoggerComm() {
            //检验comm,socket的连接状态
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true),new WaitOrTimerCallback(checkLoggerServer),null,3000,false);
        }
        private void checkLoggerServer(object state, bool timedout)
        {
            try
            {
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_NULL,  "1").toBytes());
                bool b = Logger.Instance.TcpClientConnAction();
                if (b == false)
                {   //将tcp的连接状态通过logger发送到界面
                    Logger.Instance.startSocketClient();
                    string isConn = tcpHelper?.IsConn().ToString();
                    Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState, isConn).toBytes());

                    //将串口的连接状态通过logger发送到界面
                    bool bCommConn = commHelper != null ? commHelper.IsConn : false;
                    string isCommConn = bCommConn.ToString();
                    Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SerialState, isCommConn).toBytes());
                    return;
                }

                { 
                string isConn = tcpHelper?.IsConn().ToString();
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState, isConn).toBytes());

                //将串口的连接状态通过logger发送到界面
                bool bCommConn = commHelper != null ? commHelper.IsConn : false;
                string isCommConn = bCommConn.ToString();
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SerialState,  isCommConn).toBytes());
                }

            }
            catch (Exception e) {
                Logger.Instance.i(Tag,"尝试连接界面失败！");
            }
        }

        /// <summary>
        /// 接收串口的数据,由网口转发
        /// </summary>
        /// <param name="bytes"></param>
        private void relayData(byte[] bytes) {
            //Logger.Instance.i(Tag, "串口接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
            //string s = "串口接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes);
            try
            {
                tcpHelper?.sendMessage(bytes);
            }
            catch (Exception e) {
                //Logger.Instance.i(Tag, "数据异常 串口接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
                Logger.Instance.i(Tag, "串口接收到数据转发给网络时数据异常 :"+e.ToString());
            }
            
        }
        private int tcpReceiveData(byte[] data,int len ) {
            try
            {
                byte[] bytes = new byte[len];
                Array.Copy(data, bytes, len);
                //Logger.Instance.i(Tag, "网络接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
                //string s = "网络接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes);
                //Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_Record, s).toBytes());
                commHelper?.sendMessage(bytes);
            }
            catch (Exception e) {
                Logger.Instance.i(Tag,"网络接收到的数据转发给串口时异常："+e.ToString());
            }
            return 0;
        }


    }
}
