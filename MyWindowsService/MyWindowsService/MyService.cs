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
using System.Windows.Forms;

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
        string ip = "192.168.1.10";
        int port = 10080;
        byte[] commBytes = new byte[0];
        /// <summary>
        /// 当前服务的状态，使用到网络断开是否自动连接。
        /// </summary>
        bool serverState = false;
        /// <summary>
        /// 与界面通信是否正常
        /// </summary>
        bool uiCommState = false;

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
            tcpHelper.onSendError =onSendError;
            tcpHelper.onReceiveError =onReceiveError;
            tcpHelper.onSendSuccess = onSendSuccess;
            tcpHelper.openSockect();
            string isConn = tcpHelper.IsConn().ToString();
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState,  isConn).toBytes());


            serverState = true;


            Logger.Instance.i(Tag, "服务启动");

        }

        

        protected override void OnStop()
        {
            serverState = false;
            string isConn = "False";
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState, isConn).toBytes());
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SerialState, isConn).toBytes());

            commHelper?.unCheckUsbState();
            commHelper?.closePort();
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
               if (uiCommState==false)
                  Logger.Instance.i(Tag,"尝试连接界面正常！");
                uiCommState = true;
            }
            catch (Exception e) {
                if (uiCommState)
                Logger.Instance.i(Tag,"尝试连接界面失败！");
                uiCommState = false;
            }
        }

        /// <summary>
        /// 接收串口的数据,由网口转发
        /// </summary>
        /// <param name="bytes"></param>
        private void relayData(byte[] bytes) {
            Logger.Instance.i(Tag, "串口接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
            //string s = "串口接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes);
            if (bytes.Length > 16) {
                string s = " 串口接收到的数据长度：" + bytes.Length;
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_Record, s).toBytes());
            }
            else
            {
                string s = " 串口接收到的数据长度：" + bytes.Length + "  bytes:" + Common.Utils.CommonUtils.ToHexString(bytes);
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_Record, s).toBytes());
            }

            commBytes = new byte[bytes.Length];
            Array.Copy(bytes, commBytes, bytes.Length);
            if (serverState && (tcpHelper == null || tcpHelper.IsConn() == false)) {
                OnConnected(null);
            }
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
                Logger.Instance.i(Tag, "网络接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes));
                //string s = "网络接收到的数据：" + Common.Utils.CommonUtils.ToHexString(bytes);
                string s = " 网口接收到的数据长度：" + bytes.Length;
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_Record, s).toBytes());
                commHelper?.sendMessage(bytes);
            }
            catch (Exception e) {
                Logger.Instance.i(Tag,"网络接收到的数据转发给串口时异常："+e.ToString());
            }
            return 0;
        }
        private void onSendError(string s) {
            string isConn = "False";
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_SocketState, isConn).toBytes());
            Logger.Instance.i(Tag,"网络发送失败，与服务器连接已断开 e:"+s);
        }
        private void onReceiveError(string s) {
            Logger.Instance.i(Tag, "网络接收出错：" + s);
            Thread thread = new Thread(OnConnected);
            thread.Start();
        }
        private void onSendSuccess() {

            string ss = "网络发送成功！";
            Logger.Instance.i(Tag, ss);
            Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_Record, ss).toBytes());
        }

        private void OnConnected(object state)
        {
            if (serverState)
            {
                tcpHelper?.closeSocket();
                Thread.Sleep(200);
                tcpHelper = new TcpHelper(ip, port);
                tcpHelper.onUncompressData = tcpReceiveData;
                tcpHelper.onSendError = onSendError;
                tcpHelper.onReceiveError = onReceiveError;
                tcpHelper.onSendSuccess = onSendSuccess;
                tcpHelper.openSockect();
                tcpHelper.sendMessage(commBytes);
                Logger.Instance.i(Tag, "重新发送数据：" + Common.Utils.CommonUtils.ToHexString(commBytes));
                string ss = "重新发送数据长度：" + commBytes.Length;
                Logger.Instance.sendMessageToService(new LoggerInfoBean(LoggerInfoBean.TYPE_Record, ss).toBytes());
            }
        }

    }
}
