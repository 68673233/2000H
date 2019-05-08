using System;
using System.Collections.Generic;
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

        private CommProtocol comm;
        public CommHelper() {
            comm = new CommProtocol();
            comm.OnPortCheckSuccess = onPortCheckSuccess;
        }

        public void checkUsbState() {
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), new WaitOrTimerCallback(checkUsbStateMethod), null, 3000, false);
        }
        private void checkUsbStateMethod(object state, bool timedout)
        {
            string[] ss = CommProtocol.getSerialNames();
            if (comm.PortNames.Length != ss.Length) {
                if (ss.Length > comm.PortNames.Length)
                {
                    //相当于插入USB
                    comm.PortNames = ss;
                }
                else
                {
                    //相当于拔出USB
                    if (comm.IsNoExistPort() == true)
                    {
                        OnPortCheckError?.Invoke();
                        comm.PortNames = ss;
                    }
                }
            }
        }
    }
}
