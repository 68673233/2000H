using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Common.Communication
{
    public class TcpProtocol
    {
        //重发次数
        public int TimeOutResendTimes = 1;

        public int deal(byte[] bytes, int len) {
            StringBuilder sb = new StringBuilder();
            for (int i=0;i<len;i++) {
                sb.Append(bytes[i].ToString("x")+" ");
            }
            Debug.WriteLine(sb.ToString());
            return 0;
        }
    }
}
