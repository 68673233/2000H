using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Common.Communication
{
    public delegate int OnUncompressData(byte[] bytes, int len);

    public class TCPSocket
    {
        /// <summary>
        /// 处理接收数据
        /// </summary>
        public OnUncompressData uncompressData = null;
        public System.Timers.ElapsedEventHandler recieveTimeOut = null;


        private TcpClient client;
        private Thread client_th;
        private int _isConn;

        protected System.Timers.Timer timer;
        protected string _hostname;
        protected int _port;
        /// <summary>
        /// 
        /// </summary>
        protected int _timeout_milliseconds;
        // protected TcpClient connection;
        protected bool connected;
        protected Exception exception;



        public int IsConn { get { return _isConn; } }

        public TCPSocket(string ip, int port, OnUncompressData uncompressData, System.Timers.ElapsedEventHandler recieveTimeOut)
        {
            this._hostname = ip;
            this._port = port;
            this._timeout_milliseconds = 2000;
            this.uncompressData += uncompressData;
            this.timer = new System.Timers.Timer();
            timer.Elapsed += recieveTimeOut;
            timer.Interval = 10000;
            //执行一次，就退出。
            timer.AutoReset = false;
            //this.connect_s(ip, port);
            Connect();

        }



        public TcpClient Connect()
        {
            // kick off the thread that tries to connect
            connected = false;
            exception = null;
            Thread thread = new Thread(new ThreadStart(BeginConnect));
            thread.IsBackground = true; // 作为后台线程处理    
            // 不会占用机器太长的时间    
            thread.Start();

            // 等待如下的时间    
            for (int i = 0; i < 200; i++)
            {
                //Application.DoEvents();
                thread.Join(5);
            }



            if (connected == true)
            {
                // 如果成功就返回TcpClient对象
                thread.Abort();
                return client;
            }
            if (exception != null)
            {
                // 如果失败就抛出错误
                thread.Abort();
                // throw exception;
                _isConn = -1;

            }
            else
            {
                // 同样地抛出错误
                thread.Abort();
                //string message = string.Format("TcpClient connection to {0}:{1} timed out",
                //  _hostname, _port);
                //throw new TimeoutException(message);
                _isConn = -1;
            }
            return null;
        }


        protected void BeginConnect()
        {
            try
            {
                client = new TcpClient(_hostname, _port);
                // 标记成功，返回调用者
                connected = true;
                //ThreadStart threadStart = new ThreadStart(AcceptMsg);
                //client_th = new Thread(threadStart);
                //client_th.Start();
                //textBox2.Text = "连接成功";
                _isConn = 0;
            }
            catch (Exception ex)
            {
                // 标记失败
                exception = ex;
            }
        }

        //开始数据接收工作，在一个线程中处理
        public void startAcceptMsg()
        {
            if (client_th != null) return;

            ThreadStart threadStart = new ThreadStart(AcceptMsg);
            client_th = new Thread(threadStart);
            client_th.Start();
        }

        //停止数据接收工作
        public void stopAcceptMsg()
        {
            timer.Enabled = false;
            if (client_th != null)
                client_th.Abort();
            client_th = null;
            GC.Collect();
        }

        private void connect_s(string ip, int port)
        {
            try
            {
                client = new TcpClient(ip, port);
                ThreadStart threadStart = new ThreadStart(AcceptMsg);
                client_th = new Thread(threadStart);
                client_th.Start();
                //textBox2.Text = "连接成功";
                _isConn = 0;
            }
            catch (System.Exception e)
            {
                //textBox2.Text = e.ToString(); ;
                _isConn = -1;
            }
        }

        public void setTimerEnable(bool b)
        {
            timer.Enabled = b;
        }


        /// <summary>
        /// 线程接收
        /// </summary>
        private void AcceptMsg()
        {
            if (client == null) return;
            NetworkStream ns = client.GetStream();
            byte[] bytes = new byte[1024];
            int bytesread = 0;
            //StreamReader sr = new StreamReader(ns);//流读写器
            //字组处理
            while (true)
            {
                try
                {
                    timer.Enabled = true;
                    if (ns.CanRead)
                    {
                        do
                        {
                            bytesread = ns.Read(bytes, 0, bytes.Length);
                            // string msg = Encoding.Default.GetString(bytes, 0, bytesread);
                            //显示
                            if (bytesread > 0)
                            {
                                timer.Enabled = false;
                                if (uncompressData != null) uncompressData(bytes, bytesread);

                            }
                        } while (ns.DataAvailable);
                    }
                    //  ns.Flush();

                    //ns.Close();
                }
                catch (Exception e)
                {
                    // MessageBox.Show("与服务器断开连接了");
                    // MessageBox.Show(e.Message);
                    ns.Close();
                    break;

                }
            }
            ns.Close();
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="info"></param>
        public void sendInfo(String info)
        {
            if (client == null)
                return;
            NetworkStream sendStream = client.GetStream();
            String msg = info;
            Byte[] sendBytes = Encoding.Default.GetBytes(msg);
            sendStream.Write(sendBytes, 0, sendBytes.Length);
            sendStream.Flush();
            //sendStream.Close();
        }

        public void sendInfo(byte[] info)
        {
            if (client == null)
                return;
            try
            {
                NetworkStream sendStream = client.GetStream();
                sendStream.Write(info, 0, info.Length);
                sendStream.Flush();
            }
            catch (Exception e)
            {

            }
        }

        public void closeSocket()
        {
            timer.Enabled = false;
            this._isConn = -1;
            if (client_th != null)
                client_th.Abort();
            if (client != null)
                client.Close();

            client_th = null;
            client = null;
            GC.Collect();

            //System.Threading.Thread.Sleep(1000);
        }


        /// <summary>
        /// 添加接收超时
        /// </summary>
        /// <param name="recieveTimeOut"></param>
        public void AddRecieveTimeout(System.Timers.ElapsedEventHandler recieveTimeOut)
        {
            timer.Interval = 30000;
            //Delegate[] invokeList = GetObjectEventList(this.timer, "ElapsedEventHandler");
            //if (invokeList.Length == 0)
            //    timer.Elapsed += recieveTimeOut;
            //else if (invokeList.Length>1)
            //{
            //    clearRecieveTimeout();
            //    timer.Elapsed += recieveTimeOut;
            //}
        }

        /// <summary>
        /// 清楚接收超时
        /// </summary>
        public void clearRecieveTimeout()
        {
            timer.Interval = 1000 * 60 * 60;
            //System.Timers.Timer b = this.timer;
            //FieldInfo fi = (typeof(System.Timers.Timer)).GetField("Events", BindingFlags.Static | BindingFlags.NonPublic);
            //object obj = fi.GetValue(b);
            //PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            //EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            //list.RemoveHandler(obj, list[obj]);


            ////Delegate[] invokeList = GetObjectEventList(this.timer, "ElapsedEventHandler");
            //Delegate[] invokeList = GetObjectEventList(this.timer, "ElapsedEventHandler");
            //if (invokeList != null)
            //{
            //    foreach (Delegate del in invokeList)
            //    {  // 我已经测试，事件被全部取消了，没有问题。  
            //        typeof(System.Timers.Timer).GetEvent("Elapsed").RemoveEventHandler(this.timer, del);
            //    }
            //}
        }


        //private Delegate[] GetObjectEventList(System.Timers.Timer p_Control, string p_EventName)
        //{
        //    //PropertyInfo _PropertyInfo = p_Control.GetType().GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
        //    PropertyInfo _PropertyInfo = p_Control.GetType().GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (_PropertyInfo != null)
        //    {
        //        object _EventList = _PropertyInfo.GetValue(p_Control, null);
        //        if (_EventList != null && _EventList is EventHandlerList)
        //        {
        //            EventHandlerList _list = (EventHandlerList)_EventList;
        //            //FieldInfo _FieldInfo = (typeof(System.Timers.Timer)).GetField(p_EventName, BindingFlags.Static | BindingFlags.NonPublic);
        //            FieldInfo _FieldInfo = (typeof(System.Timers.Timer)).GetField(p_EventName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        //            if (_FieldInfo == null) return null;
        //            Delegate _ObjectDelegate = _list[_FieldInfo.GetValue(p_Control)];
        //            if (_ObjectDelegate == null) return null;
        //            return _ObjectDelegate.GetInvocationList();
        //        }
        //    }
        //    return null;
        //}

        //public static Delegate[] GetObjectEventList(object p_Object, string p_EventName)
        //{
        //    FieldInfo _Field = p_Object.GetType().GetField(p_EventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        //    if (_Field == null) { return null; }
        //    object _FieldValue = _Field.GetValue(p_Object);
        //    if (_FieldValue != null && _FieldValue is Delegate)
        //    {
        //        Delegate _ObjectDelegate = (Delegate)_FieldValue;
        //        return _ObjectDelegate.GetInvocationList();
        //    }
        //    return null;
        //}
    }
}
