using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Communication
{
    public class TcpHelper
    {
        private TCPSocket recieveSocket;
        private TcpProtocol tcpProtocol;

        public OnUncompressData onUncompressData = null;

        private string _ip;
        private int _port;
        public TcpHelper(string ip,int port) {
            _ip = ip;
            _port = port;
        }

        public bool openSockect()
        {
            if (tcpProtocol == null)
            {
                tcpProtocol = new TcpProtocol();
                
            }
            tcpProtocol.TimeOutResendTimes = 1;

            
            if (recieveSocket == null)
            {
                recieveSocket = new TCPSocket(_ip, _port, onUncompressData, this.RecieveTimeOut);
                recieveSocket.AddRecieveTimeout(this.RecieveTimeOut);
                recieveSocket.startAcceptMsg();
            }
            else if (recieveSocket.IsConn == -1)
            {
                recieveSocket = new TCPSocket(_ip, _port, onUncompressData, this.RecieveTimeOut);
                recieveSocket.AddRecieveTimeout(this.RecieveTimeOut);
                recieveSocket.startAcceptMsg();
            }

            //连接失败 
            if ( recieveSocket.IsConn == -1)
            {
                recieveSocket.closeSocket();
                recieveSocket = null;
                //提示连接失败
                return false;
            }
            return true;
        }

        //关闭连接
        public bool closeSocket()
        {
            if (recieveSocket != null)
                recieveSocket.closeSocket();
            recieveSocket = null;
            return true;
        }
        public bool IsConn() {
            bool b = false;
            if (recieveSocket == null) return b;
            b= recieveSocket?.IsConn==-1?false:true;
            return b;
        }
        //超时的处理
        private void RecieveTimeOut(object sender, System.Timers.ElapsedEventArgs e)
        {
            tcpProtocol.TimeOutResendTimes++;
        }

        public void sendMessage(byte[] info) {
            if (recieveSocket!=null && recieveSocket.IsConn!=-1)
                recieveSocket.sendInfo(info);
        }

    }
}
