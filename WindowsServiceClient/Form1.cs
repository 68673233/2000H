using Common;
using Common.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using WindowsServiceClient.Manage;

namespace WindowsServiceClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            initClass();
            initCtrl();
            
        }

        //string serviceFilePath = $"{Application.StartupPath}\\MyWindowsService.exe";
        //string serviceName = "MyService";
        service.ServiceOpt server;
        IniFileOpt ini = null;

        private void initClass()
        {
            if (!Directory.Exists(PathManage.LogDir)) Directory.CreateDirectory(PathManage.LogDir);
            Logger.Instance.init(PathManage.LogDir, OnLoggerI);


        }
        private void initCtrl() {
            server = new service.ServiceOpt(PathManage.ServiceName);
            server.OnServiceState = new Action<int>(serviceState);
            server.serviceStateThreadCheck();

            button1.Click +=new EventHandler(btn_Click);
            button2.Click += new EventHandler(btn_Click);
            button3.Click += new EventHandler(btn_Click);
            button4.Click += new EventHandler(btn_Click);
            btn_SetAndRun.Click += new EventHandler(btn_SetAndRun_Click);


            menuitem_showUI.Click += new EventHandler(menuItem_Click);
            menuitem_hideUI.Click += new EventHandler(menuItem_Click);

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }


        private void OnLoggerI(string content) {
            MessageBox.Show(content);
        }

        private void serviceState(int index) {
            if (this.InvokeRequired) {
                Invoke(new Action<int>(serviceState), index);
            }
            else {
                string s = "";
                switch (index) {
                    case 0: s = "服务未安装"; break;
                    case (int)ServiceControllerStatus.Stopped: s = "服务未运行";  break;
                    case (int)ServiceControllerStatus.StartPending: s = "服务正在启动";break;
                    case (int)ServiceControllerStatus.StopPending: s = "服务正在停止";break;
                    case (int)ServiceControllerStatus.Running: s = "服务正在运行";break;
                    case (int)ServiceControllerStatus.ContinuePending:s = "服务即将继续";break;
                    case (int)ServiceControllerStatus.PausePending: s = "服务即将停止";break;
                    case (int)ServiceControllerStatus.Paused:s = "服务已停止";break;
                }
                label2.Text = s;
            }
        }
        private void btn_Click(object sender, EventArgs e)
        {
            switch ( (sender as Button).Name ) {
                case "button1":
                    {
                        if (server.IsServiceExisted(PathManage.ServiceName)) server.UninstallService(PathManage.ServiceFilePath);
                        server.InstallService(PathManage.ServiceFilePath);
                    } break;
                case "button2":
                    {
                        if (server.IsServiceExisted(PathManage.ServiceName)) server.ServiceStart(PathManage.ServiceName);
                    } break;
                case "button3":
                    {
                        if (server.IsServiceExisted(PathManage.ServiceName)) server.ServiceStop(PathManage.ServiceName);
                    } break;
                case "button4":
                    {
                        if (server.IsServiceExisted(PathManage.ServiceName))
                        {
                            server.ServiceStop(PathManage.ServiceName);
                            server.UninstallService(PathManage.ServiceFilePath);
                        }
                    } break;
            }
        }

        private void btn_SetAndRun_Click(object sender,EventArgs e) {
            //if (ini==null) {
            //    ini = new IniFileOpt(PathManage.IniFilePath);
            //}
            //ini.write("Ip", "192.168.1.10");
            //MessageBox.Show( ini.read("Ip"));
            Logger.Instance.i(" btn_setAndRun_click", "保存成功");

        }

        private void menuItem_Click(object sender,EventArgs e) {
            ToolStripMenuItem menuitem = sender as ToolStripMenuItem;
            switch (menuitem.Name) {
                case "menuitem_showUI":
                    {
                        this.Show();
                        notifyIcon1.Visible = false;
                    } break;
                case "menuitem_hideUI":
                    {
                        this.Hide();
                        notifyIcon1.Visible = true;
                    } break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            notifyIcon1.Visible = true;
        }
    }
}
