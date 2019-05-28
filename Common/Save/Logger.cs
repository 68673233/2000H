using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Common.Save
{
    public class Logger
    {
        private const string TAG = "Logger";

        #region  网络通信相关
        ///网络传送协议  
        ///<int>type</int>
        ///<int>len</int>
        ///<string>content</string>



       

        /// <summary>
        /// 服务端将数据更新到界面
        /// </summary>
        public Action<int, string> OnParamToUI = null;

        TcpListener tcpServer;
        TcpClient tcpClient;
        /// <summary>
        /// 线程结束标记
        /// </summary>
        private volatile bool _isAction = false;
        public bool IsAction {
            set { _isAction = value; }
            get { return _isAction; }
        }
        #endregion

        public static Logger logger = new Logger();
        public static Logger Instance { get { return logger; } }

        public string FilePath = null;

        private string fileName;

        public Action<string> OnLoggerI = null;


        public void init(string filePath, Action<string> _onLoggerI) {
            this.FilePath = filePath;
            OnLoggerI = _onLoggerI;
        }

        /// <summary>
        /// 显示及保存
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public void i(string tag, string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            if (FilePath != null) {
                fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string path = FilePath + "\\" + fileName;
                try
                {
                    File.AppendAllText(path, content + Environment.NewLine);
                    //File.AppendAllText(path, Environment.NewLine);
                }
                catch (Exception e) {

                }
                OnLoggerI?.Invoke(content);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public void infoToFile(string tag, string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            if (FilePath != null)
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string path = FilePath + "\\" + fileName;
                File.AppendAllText(path, content);
                File.AppendAllText(path, Environment.NewLine);
            }
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public void infoToShow(string tag, string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            OnLoggerI?.Invoke(content);
        }


        /// <summary>
        /// 开启网络服务
        /// </summary>
        public void startSocketService(Action<int, string> onParamToUI) {
            OnParamToUI = onParamToUI;
            _isAction = true;
            tcpServer = new TcpListener(IPAddress.Any, 10101);
            tcpServer.Start();
            
            Thread t = new Thread(server);
            t.IsBackground = true;
            t.Start(tcpServer);
        }

        public void stopSocketService() {
            _isAction = false;
            if (tcpServer != null) tcpServer.Stop();
            tcpServer = null;
        }

        //private void ListenClientConnect() {
        //    while (true) {
        //        Socket clientSocket = serverSocket.Accept();
        //        clientSockets.Add(clientSocket);
        //        Thread receiveThread = new Thread(ReceiveMessage);
        //        receiveThread.Start(clientSocket);
        //    }
        //}

        private void server(object o)
        {
            TcpListener list = o as TcpListener;
            TcpClient client = list.AcceptTcpClient();

            while (_isAction)
            {
                const int buffer = 81920;
                byte[] b = new byte[buffer];
                {
                    
                        try
                        {
                        client.Client.Receive(b);
                        int i = 0;
                        while (b[i]!=0) {
                            //解析
                            if (b.Length > 8 && OnParamToUI != null)
                            {
                                int type = b[i+0] | b[i+1] << 8 | b[i+2] << 16 | b[i+3] << 24;
                                int len = b[i+4] | b[i + 5] << 8 | b[i + 6] << 16 | b[i + 7] << 24;
                                byte[] bytes = new byte[len];
                                Array.Copy(b, i+8, bytes, 0, len);
                                string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                                OnParamToUI?.Invoke(type, str);
                                i = i + 8+len;
                            }
                         }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.i(TAG,"recieve error:"+e);
                            client = list.AcceptTcpClient();
                    }
                }
            }
        }

        public void startSocketClient() {
            tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 10101);

        }

        public void stopSocketClient() {
            try
            {
                tcpClient.Close();
                tcpClient = null;
            }
            catch (Exception e) {
                Logger.Instance.i(TAG,"stopSocketClient is error:"+e.ToString());
            }
        }
        public bool TcpClientConnAction() {
            bool b = false;
            if (tcpClient != null) b = tcpClient.Connected ;
            return b;
        }
        public void sendMessageToService(byte[] bytes) {
            try
            {   if (tcpClient!=null)
                {
                   tcpClient.Client.Send(bytes);
                    //NetworkStream strem = tcpClient.GetStream();
                    ////byte[] b = Encoding.UTF8.GetBytes(str);
                    //strem.Write(bytes, 0, bytes.Length);
                    //strem.Close();
                }
            }
            catch (Exception e) {
                if (bytes[0] == LoggerInfoBean.TYPE_NULL) return;
                Logger.Instance.i(TAG,"sendMessageToService error: "+e.ToString());
            }
        }


    }

    public class LoggerInfoBean{
        /// <summary>
        /// 串口连接状态
        /// </summary>
        public const int TYPE_SerialState = 0x01;
        /// <summary>
        /// 网络连接服务器状态
        /// </summary>
        public const int TYPE_SocketState = 0x02;
        /// <summary>
        /// 记录信息
        /// </summary>
        public const int TYPE_Record = 0x03;
        /// <summary>
        /// 用来检测是否在连接状态
        /// </summary>
        public const int TYPE_NULL = 0x04;

        public int type;
        public int len;
        public string content;
        public LoggerInfoBean(int type,string content) {
            this.type = type;
            this.content = content;
        }

        public byte[] toBytes() {
            byte[] c = Encoding.UTF8.GetBytes(content);
            len = c.Length;

            List<byte> list = new List<byte>();
            list.Add((byte)type);
            list.Add( (byte)(type >> 8));
            list.Add( (byte)(type>>16));
            list.Add((byte)(type>>24));

            list.Add((byte)len);
            list.Add((byte)(len>>8));
            list.Add((byte)(len>>16));
            list.Add((byte)(len>>24));
            

            list.AddRange(c);
            return list.ToArray();
        }
    }

}
