using Common;
using Common.Communication;
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

        service.ServiceOpt server;
        IniFileOpt ini = null;

        private void initClass()
        {
            if (!Directory.Exists(PathManage.LogDir)) Directory.CreateDirectory(PathManage.LogDir);
            Logger.Instance.init(PathManage.LogDir, OnLoggerI);
            Logger.Instance.startSocketService(OnLoggerParamToUI);
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

            txt_port.KeyPress += new KeyPressEventHandler(txt_port_KeyPress);

            menuitem_showUI.Click += new EventHandler(menuItem_Click);
            menuitem_hideUI.Click += new EventHandler(menuItem_Click);
            menuitem_exit.Click += new EventHandler(menuItem_Click);

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }


        private void OnLoggerParamToUI(int type,string content) {
            if (this.InvokeRequired) {
                Invoke(new Action<int, string>(OnLoggerParamToUI), type, content);
            }
            else
            {
                switch (type)
                {
                    case LoggerInfoBean.TYPE_SerialState: {
                            //if (listBox1.Items.Count > 3000) listBox1.Items.RemoveAt(0);
                            //listBox1.Items.Add("serialstate conn:"+content);
                            addUIList("serialstate conn:" + content);
                            bool b = "True" == content;
                            if (b) imageDevice.Image = global::WindowsServiceClient.Properties.Resources.conn;
                            else imageDevice.Image = global::WindowsServiceClient.Properties.Resources.noconn;
                        } break;
                    case LoggerInfoBean.TYPE_SocketState: {
                            //if (listBox1.Items.Count > 3000) listBox1.Items.RemoveAt(0);
                            //listBox1.Items.Add("socketstate conn:" + content);
                            addUIList("socketstate conn:" + content);
                            bool b = "True" == content;
                            if (b) imageServer.Image = global::WindowsServiceClient.Properties.Resources.conn;
                            else imageServer.Image = global::WindowsServiceClient.Properties.Resources.noconn;
                        } break;
                    case LoggerInfoBean.TYPE_Record: {
                            //if (listBox1.Items.Count > 3000) listBox1.Items.RemoveAt(0);
                            //listBox1.Items.Add("" + content);
                            addUIList(""+content);
                        } break;
                }
            }
        }
        private void OnLoggerI(string content) {
            if (this.InvokeRequired) {
                Invoke(new Action<string>(OnLoggerI), content);
            }
            else {
                addUIList(content);
            }

        }

        private void addUIList(string content) {
            if (listBox1.Items.Count > 3000) listBox1.Items.RemoveAt(0);
            listBox1.Items.Add(content);
            listBox1.SelectedIndex = listBox1.Items.Count-1;
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
            Logger.Instance.i("btn_click",(sender as Button).Text+" 服务完成！");
        }

        private void btn_SetAndRun_Click(object sender,EventArgs e) {
            if (ini == null)
            {
                ini = new IniFileOpt(PathManage.IniFilePath);
            }
            txt_port.Text= int.Parse(txt_port.Text) > 65535 ? "10080" : txt_port.Text;
            ini.write(IniFileOpt.IP, ipAddrText1.ipAddressString());
            ini.write(IniFileOpt.Port,txt_port.Text);
            Logger.Instance.i("btn_setAndRun_click", "ip:"+ipAddrText1.ipAddressString()+",port:"+txt_port.Text);
            
            //重新启动服务

            //TcpHelper tcp = new TcpHelper(ipAddrText1.ipAddressString(), int.Parse(txt_port.Text));
            //tcp.openSockect();
            //byte[] bytes = new byte[] { 0xaa, 0x55, 0x55, 0xaa, 0x04, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00 };
            //tcp.sendMessage(bytes);
        }

        private void txt_port_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
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
                case "menuitem_exit":
                    {
                        Application.Exit();
                    } break;
            }
            Logger.Instance.i("menuItem",menuitem.Name+"_Click");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible == false) {
                e.Cancel = false;
                return;
            }
            e.Cancel = true;
            this.Hide();
            notifyIcon1.Visible = true;
            Logger.Instance.i("formClosing","formClosing");
        }
    }
}
