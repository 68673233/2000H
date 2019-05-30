using Common.Save;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace Common.Communication
{
    public class CommProtocol
    {
        /// <summary>
        /// 端口检测错误 
        /// </summary>
        public Action OnPortCheckError=null;
        private Action onPortCheckSuccess = null;
        public Action OnPortCheckSuccess {
            set { onPortCheckSuccess = value; }
        }
        public Action<byte[]> onRelayDate = null;
        public Action<byte[]> OnRelayDate {
            set { onRelayDate = value; }
        }
        
        //当前是否是探测串口状态
        private bool isCheckCommState = false;
        //当前指令
        private int CurrCommand;
        //心跳未连接成功次数
        private volatile int NoHeartBeatCount = 0;
        //心跳连接最大次数
        private const int HeartBeatMaxConnCount = 4;
        //连接状态
        private bool CurrConnState = false;
        private System.Timers.Timer heartbeatTimer;

        /// <summary>
        /// 串口通信状态
        /// </summary>
        private bool commState = false;

        //返回连接状态
        public bool IsConn {
            get { return CurrConnState; }
        }
        //串口的数量由外面控制
        public string[] PortNames = getSerialNames();

        private CommPort comm;

        public CommProtocol() {

            heartbeatTimer = new System.Timers.Timer();
            heartbeatTimer.AutoReset = true;
            heartbeatTimer.Elapsed += new ElapsedEventHandler(heartbeatElapsedEvent);
            //heartbeatTimer.Elapsed += new ElapsedEventHandler((s, e) => heartbeatElapsedEvent(s, e,ref NoHeartBeatCount));
            heartbeatTimer.Interval = 1000;
            heartbeatTimer.Enabled = false;

        }

        public void setAutoCheckCom(bool enabled) {
            heartbeatTimer.Enabled = enabled;
        }
        private void heartbeatElapsedEvent(object sender, ElapsedEventArgs e)
        {
            NoHeartBeatCount++;
            if (NoHeartBeatCount >= HeartBeatMaxConnCount)
            {
                NoHeartBeatCount = 0;
                CurrConnState = false;
                OnPortCheckError?.Invoke();
                Logger.Instance.i("comm","连接设备心跳断开!");
                return;
            }
            if (CurrConnState)
            {
                if (commState==false) Logger.Instance.i("comm", "心跳连接！");
                //发心跳包
                this.connDev();
                commState = true;
                
            }
            else
            {
                if (commState == true) Logger.Instance.i("comm","查找连接设备端口！");
                commState = false;
                //未连接，查找连接设备的端口
                this.checkDevComm();
            }
        }

        public bool IsNoExistPort()
        {
            bool flag = true;
            string[] portNames = getSerialNames(); 
            for (int i = 0; i < portNames.Length; i++)
            {
                if (comm!=null && portNames[i] == comm.Port) { flag = false; return flag; }
            }
            NoHeartBeatCount = HeartBeatMaxConnCount;
            comm?.close();
            return flag;

        }
        public void closePort() {
             comm?.close();
        }

        public void checkDevComm()
        {
            string port = getDevComm();
            if (port == null)
            {
                //提示切退出
                OnPortCheckError?.Invoke();
                return;
            }
            if (comm == null)
            {
                OnPortCheckError?.Invoke();
                return;
            }

            comm.open();
        }
        private string getDevComm()
        {
            //string[] PortNames = SerialPort.GetPortNames();
            System.Threading.Thread.Sleep(20);
            for (int i = 0; i < PortNames.Length; i++)
            {
                isCheckCommState = true;
                if (comm != null)
                {
                    comm.receiveData -= receivedData;
                    comm.close();
                    System.Threading.Thread.Sleep(10);
                }
                comm = new CommPort(PortNames[i], 115200);

                comm.receiveData += receivedData;
                bool b;
                if (b = comm.open()) { this.connDev(); Logger.Instance.i("comm","探测串口！连接"); }


                System.Threading.Thread.Sleep(300);
                if (isCheckCommState == false)
                {
                    return PortNames[i];
                }
                comm.close();

            }
            return null;
        }

        public void receivedData(byte[] recData) {
            //Logger.Instance.i("comm", "接收到数据 len:"+recData.Length +"  bytes:" + Common.Utils.CommonUtils.ToHexString(recData));
            if (recData.Length < 12) return;
            int star = 0;
            while (star<recData.Length) {
                int type = recData[8 + star] | recData[9 + star] << 8 | recData[10 + star] << 16 | recData[11 + star] << 24;
                int len= recData[4+star] | recData[5 + star] << 8 | recData[6 + star] << 16 | recData[7 + star] << 24;
                
                //int type = recData[11] | recData[10] << 8 | recData[9] << 16 | recData[8] << 24;

                switch (type) {
                    case 0xA030: {
                            //连接状态
                            isCheckCommState = false;
                            NoHeartBeatCount = 0;
                            CurrConnState = true;
                            onPortCheckSuccess?.Invoke();

                        } break;
                    default: {
                            //其它数据转发，转给串网络发送
                            byte[] bytes = new byte[8+len];
                            Array.Copy(recData,star, bytes,0, bytes.Length);
                            onRelayDate?.Invoke(bytes);
                        } break;
                }
                star =star+ 4 + 4 + len;
            }
            
        }
        public void connDev()
        {
            byte[] bytes = { 0xAA,0x55,0x55,0xAA,0x04,0x00,0x00,0x00, 0x30, 0, 0,0x00 };
            CurrCommand = bytes[8] | bytes[9] << 8 | bytes[10] << 16 | bytes[11] << 24;
            //Logger.Instance.i("comm","发送连接数据");
            if (comm != null)
            {
                comm.sendMessage(bytes,false);
            }
        }

        public void sendMessage(byte[] bytes) {
            comm?.sendMessage(bytes,true);
        }



        #region  串口工具

        /// <summary>
        /// 枚举win32 api
        /// </summary>
        public enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }


        public class MulGetHardwareInfo_Comm
        {
            public HardwareEnum hardType;
            public string propKey;
            public string[] ss;
            /// <summary>
            /// WMI取硬件信息
            /// </summary>
            /// <param name="hardType"></param>
            /// <param name="propKey"></param>
            /// <returns></returns>

            public void MulGetHardwareInfo()
            {


                List<string> strs = new List<string>();
                try
                {
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                    {
                        var hardInfos = searcher.Get();
                        foreach (var hardInfo in hardInfos)
                        {
                            if (hardInfo.Properties[propKey].Value == null) continue;
                            string s = hardInfo.Properties[propKey].Value.ToString();
                            //MessageBox.Show(s);
                            Regex reg = new Regex("\\(COM(\\d{1,3})\\)");
                            Match match = reg.Match(s);
                            if (match.Success)
                            {
                                strs.Add(s);
                                //MessageBox.Show("找到："+s);
                                //strs.Add(hardInfo.Properties[propKey].Value.ToString());
                            }

                        }
                        searcher.Dispose();
                    }

                    ss = strs.ToArray();
                }
                catch
                {
                    ss = strs.ToArray();
                }
                finally
                { strs = null; }
            }
        }




        // 获得串口名称
        public static string[] getSerialNames()
        {
            //通过WMI获取COM端口
            MulGetHardwareInfo_Comm comm = new MulGetHardwareInfo_Comm();
            comm.hardType = HardwareEnum.Win32_PnPEntity;
            comm.propKey = "Name";

            Thread thread = new Thread(new ThreadStart(comm.MulGetHardwareInfo));
            thread.Start();
            thread.Join();
            //string[] ss = MulGetHardwareInfo(/*HardwareEnum.Win32_PnPEntity*/HardwareEnum.Win32_SerialPort, "Name");
            string[] ss = comm.ss;
            for (int i = 0; i < ss.Length; i++)
            {
                ss[i] = ss[i].Split(new char[1] { '(' })[1];
                ss[i] = ss[i].Split(new char[1] { ')' })[0];
                //MessageBox.Show("串口号："+ss[i]);
            }

            return comm.ss;
        }
        #endregion
    }
}
