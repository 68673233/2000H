using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsServiceClient.controls
{
    public partial class IpAddrText : UserControl
    {

        bool textBoxValid = true;
        bool EventEnable = true;

        public IpAddrText()
        {
            InitializeComponent();
            initControl();
        }
        private void initControl() {
            textBox1.KeyPress += new KeyPressEventHandler(textBoxOnKeyPress);
            textBox2.KeyPress += new KeyPressEventHandler(textBoxOnKeyPress);
            textBox3.KeyPress += new KeyPressEventHandler(textBoxOnKeyPress);
            textBox4.KeyPress += new KeyPressEventHandler(textBox4OnKeyPress);

            textBox1.KeyDown += new KeyEventHandler(textBoxOnKeyDown);
            textBox2.KeyDown += new KeyEventHandler(textBoxOnKeyDown);
            textBox3.KeyDown += new KeyEventHandler(textBoxOnKeyDown);
            textBox4.KeyDown += new KeyEventHandler(textBoxOnKeyDown);

            textBox1.TextChanged += new EventHandler(textBox1OnTextChanged);
            textBox2.TextChanged += new EventHandler(textBoxOnTextChanged);
            textBox3.TextChanged += new EventHandler(textBoxOnTextChanged);
            textBox4.TextChanged += new EventHandler(textBoxOnTextChanged);
        }

        private void textBoxOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                if (e.KeyChar == (char)8)
                {
                    e.Handled = false;
                    return;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox4OnKeyPress(object sender,KeyPressEventArgs e) {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                if (e.KeyChar == (char)8)
                {
                    e.Handled = false;
                    return;
                }
                if (textBox4.TextLength == 3)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (e.KeyValue == 37 || e.KeyValue == 38 || e.KeyValue == 8)
            {
                if (textbox.SelectionStart == 0)
                {
                    SendKeys.Send("{Tab}");
                }
            }
            else if (e.KeyValue == 39 || e.KeyValue == 40)
            {
                string currentStr = textbox.Text;
                if (textbox.SelectionStart == currentStr.Length)
                {
                    SendKeys.Send("+{Tab}{End}");
                }
            }
        }

        private void textBox1OnTextChanged(object sender,EventArgs e) {
            if (EventEnable == false) return;

            int len = textBox1.Text.Length;
            if (len == 3)
            {
                string currentStr = textBox1.Text;
                int currentNum = Convert.ToInt32(currentStr);
                if (currentNum > 223)
                {
                    MessageBox.Show(currentNum + "Is Not Valid. Please Enter A Value Between 1 And223.", "Error");
                    textBoxValid = false;
                    textBox1.Text = "223";
                }
                else if (currentNum == 0)
                {
                    MessageBox.Show(currentNum + "Is Not Valid. Please Enter A Value Between 1 And223.", "Error");
                    textBoxValid = false;
                    textBox1.Text = "1";
                }
                else
                {
                    if (textBoxValid)
                    { SendKeys.Send("{Tab}"); }
                    else
                    {
                        textBox1.SelectionStart = 0;
                        textBoxValid = true;
                    }
                }
            }
           
        }
        private void textBoxOnTextChanged(object sender, EventArgs e)
        {
            if (EventEnable == false) return;

            TextBox textBox = sender as TextBox;
            int len = textBox.Text.Length;
            if (len == 3)
            {
                string currentStr = textBox.Text;
                int currentNum = Convert.ToInt32(currentStr);
                if (currentNum > 255)
                {
                    MessageBox.Show(currentNum + "Is Not Valid. Please Enter A Value Between 1 And223.", "Error");
                    textBoxValid = false;
                    textBox.Text = "255";
                }
                else
                {
                    if (textBoxValid)
                    {
                        if (textBox == textBox4) { return; }
                        else
                        { SendKeys.Send("{Tab}"); }
                    }
                    else
                    {
                        textBox.SelectionStart = 0;
                        textBoxValid = true;
                    }
                }
            }
        }


        #region 接口
        public string ipAddressString()
        {
            string ipAddress = textBox1.Text.Trim() + "." + textBox2.Text.Trim() + "." + textBox3.Text.Trim() + "." + textBox4.Text.Trim();
            return ipAddress;
        }
        public bool ipAddressNotNull()
        {
            if (textBox1.Text.Trim() != string.Empty && textBox2.Text.Trim() != string.Empty &&
                textBox3.Text.Trim() != string.Empty && textBox4.Text.Trim() != string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void setIpAddress(string ipAddressStr)
        {
            string[] textIpAddress = ipAddressStr.Split('.');
            try
            {
                EventEnable = false;
                textBox1.Text = textIpAddress[0];
                textBox2.Text = textIpAddress[1];
                textBox3.Text = textIpAddress[2];
                textBox4.Text = textIpAddress[3];

            }
            catch (Exception e)
            {

            }
            finally {
                EventEnable = true;
            }
        } 
        #endregion
    }
}
