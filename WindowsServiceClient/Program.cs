using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WindowsServiceClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Process[] processes = System.Diagnostics.Process.GetProcessesByName("WindowsServiceClient");
            if (processes.Length > 1) {
                MessageBox.Show("应用程序已经在运行中。。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Thread.Sleep(1000);
                System.Environment.Exit(1);
            }
            else {
                beginRun(true);
                Application.Run(new Form1());
            }
            
        }

        private static void beginRun(bool flag) {
            if (flag) //设置开机自启动  
            {
                //MessageBox.Show("设置开机自启动，需要修改注册表", "提示");
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("WindowsServiceClient", path);
                rk2.Close();
                rk.Close();

            }
            else //取消开机自启动  
            {
                // MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue("WindowsServiceClient", false);
                rk2.Close();
                rk.Close();
            }
        }
    }
}
