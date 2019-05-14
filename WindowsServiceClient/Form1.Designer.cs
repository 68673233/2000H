namespace WindowsServiceClient
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitem_showUI = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitem_hideUI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitem_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_SetAndRun = new System.Windows.Forms.Button();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.imageDevice = new System.Windows.Forms.PictureBox();
            this.imageServer = new System.Windows.Forms.PictureBox();
            this.ipAddrText1 = new WindowsServiceClient.controls.IpAddrText();
            this.listbox1_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitem_clear = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServer)).BeginInit();
            this.listbox1_menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(67, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "安装";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(167, 73);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "启动";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(262, 73);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "停止";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(363, 73);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "卸载";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前服务状态：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitem_showUI,
            this.menuitem_hideUI,
            this.toolStripMenuItem1,
            this.menuitem_exit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 76);
            // 
            // menuitem_showUI
            // 
            this.menuitem_showUI.Name = "menuitem_showUI";
            this.menuitem_showUI.Size = new System.Drawing.Size(124, 22);
            this.menuitem_showUI.Text = "显示界面";
            // 
            // menuitem_hideUI
            // 
            this.menuitem_hideUI.Name = "menuitem_hideUI";
            this.menuitem_hideUI.Size = new System.Drawing.Size(124, 22);
            this.menuitem_hideUI.Text = "隐藏界面";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(121, 6);
            // 
            // menuitem_exit
            // 
            this.menuitem_exit.Name = "menuitem_exit";
            this.menuitem_exit.Size = new System.Drawing.Size(124, 22);
            this.menuitem_exit.Text = "退出";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_SetAndRun);
            this.groupBox1.Controls.Add(this.txt_port);
            this.groupBox1.Controls.Add(this.ipAddrText1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(39, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 116);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "网络设置";
            // 
            // btn_SetAndRun
            // 
            this.btn_SetAndRun.Location = new System.Drawing.Point(325, 23);
            this.btn_SetAndRun.Name = "btn_SetAndRun";
            this.btn_SetAndRun.Size = new System.Drawing.Size(86, 35);
            this.btn_SetAndRun.TabIndex = 3;
            this.btn_SetAndRun.Text = "设置并启用";
            this.btn_SetAndRun.UseVisualStyleBackColor = true;
            // 
            // txt_port
            // 
            this.txt_port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_port.Location = new System.Drawing.Point(75, 76);
            this.txt_port.MaxLength = 5;
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(100, 21);
            this.txt_port.TabIndex = 2;
            this.txt_port.Text = "10080";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ip";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 270);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(524, 206);
            this.panel1.TabIndex = 5;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(524, 206);
            this.listBox1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(400, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "设备连接：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(387, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "服务器连接：";
            // 
            // imageDevice
            // 
            this.imageDevice.Image = global::WindowsServiceClient.Properties.Resources.noconn;
            this.imageDevice.Location = new System.Drawing.Point(475, 9);
            this.imageDevice.Name = "imageDevice";
            this.imageDevice.Size = new System.Drawing.Size(25, 25);
            this.imageDevice.TabIndex = 7;
            this.imageDevice.TabStop = false;
            // 
            // imageServer
            // 
            this.imageServer.Image = global::WindowsServiceClient.Properties.Resources.noconn;
            this.imageServer.Location = new System.Drawing.Point(475, 38);
            this.imageServer.Name = "imageServer";
            this.imageServer.Size = new System.Drawing.Size(25, 25);
            this.imageServer.TabIndex = 7;
            this.imageServer.TabStop = false;
            // 
            // ipAddrText1
            // 
            this.ipAddrText1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipAddrText1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipAddrText1.Location = new System.Drawing.Point(75, 27);
            this.ipAddrText1.Margin = new System.Windows.Forms.Padding(5);
            this.ipAddrText1.Name = "ipAddrText1";
            this.ipAddrText1.Size = new System.Drawing.Size(170, 27);
            this.ipAddrText1.TabIndex = 1;
            // 
            // listbox1_menu
            // 
            this.listbox1_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitem_clear});
            this.listbox1_menu.Name = "listbox1_menu";
            this.listbox1_menu.Size = new System.Drawing.Size(153, 48);
            // 
            // menuitem_clear
            // 
            this.menuitem_clear.Name = "menuitem_clear";
            this.menuitem_clear.Size = new System.Drawing.Size(152, 22);
            this.menuitem_clear.Text = "清空";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 476);
            this.Controls.Add(this.imageServer);
            this.Controls.Add(this.imageDevice);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "服务状态";
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServer)).EndInit();
            this.listbox1_menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuitem_showUI;
        private System.Windows.Forms.ToolStripMenuItem menuitem_hideUI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_port;
        private controls.IpAddrText ipAddrText1;
        private System.Windows.Forms.Button btn_SetAndRun;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuitem_exit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox imageDevice;
        private System.Windows.Forms.PictureBox imageServer;
        private System.Windows.Forms.ContextMenuStrip listbox1_menu;
        private System.Windows.Forms.ToolStripMenuItem menuitem_clear;
    }
}

