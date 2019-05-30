using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace Common.Communication
{
    public class CommHelper
    {

        /// <summary>
        /// USB连接设备失败
        /// </summary>
        public Action OnPortCheckError=null;
        /// <summary>
        /// USB连接上设备
        /// </summary>
        private Action onPortCheckSuccess;
        public Action OnPortCheckSuccess {
            set {
                onPortCheckSuccess = value;
                if (comm!=null ) comm.OnPortCheckSuccess = onPortCheckSuccess;
                }
        }

        private Action<byte[]> onRelayDate = null;
        public Action<byte[]> OnRelayDate {
            set {
                onRelayDate = value;
                if (comm != null) comm.OnRelayDate = onRelayDate;
            }
        }

        public bool IsConn {
            get
            {
                bool conn = false;
                if (comm != null) conn=comm.IsConn;
                return conn;
            }
        }

        RegisteredWaitHandle registeredWaitHandle;

        private CommProtocol comm;
        public CommHelper() {
            comm = new CommProtocol();
            comm.OnPortCheckSuccess = onPortCheckSuccess;
            comm.setAutoCheckCom(true);
        }
        public void closePort() {
            comm?.closePort();
        }
        public void checkUsbState() {
            registeredWaitHandle= ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), new WaitOrTimerCallback(checkUsbStateMethod), null, 3000, false);
        }
        public void unCheckUsbState() {
            registeredWaitHandle.Unregister(new AutoResetEvent(true));
        }

        public void sendMessage(byte[] bytes) {
            comm?.sendMessage(bytes);
        }

        private void checkUsbStateMethod(object state, bool timedout)
        {
            string[] ss = CommProtocol.getSerialNames();
            if (comm.PortNames.Length != ss.Length) {
                if (ss.Length > comm.PortNames.Length)
                {
                    //相当于插入USB
                    comm.PortNames = ss;
                    Debug.WriteLine("插入USB");
                }
                else
                {
                    //相当于拔出USB
                    if (comm.IsNoExistPort() == true)
                    {
                        OnPortCheckError?.Invoke();
                        comm.PortNames = ss;
                    }
                    Debug.WriteLine("拔出USB");
                }
            }
        }
    }
}
