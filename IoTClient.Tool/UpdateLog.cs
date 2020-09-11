using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IoTClient.Tool
{
    public partial class UpdateLog : Form
    {
        public UpdateLog(bool hasNew)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            var logs = new List<string>();
            logs.Add("日志记录\r\n");
            logs.Add("版本：[0.4.0]");
            logs.Add($"时间：{DateTime.Now.ToString("yyyy-MM-dd")}");
            logs.Add("内容");
            logs.Add("1.西门子PLC批量读写，大幅提高读写性能");
            logs.Add("2.手动检查更新");
            logs.Add("3.显示历史更新日志");
            textBox1.Text = string.Join("\r\n", logs);
            if (hasNew)
            {
                button1.Enabled = true;
                button1.Text = "自动更新";
            }
            else
            {
                button1.Enabled = false;
                button1.Text = "已是最新版本";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/zhaopeiym/IoTClient/releases");
            }
            catch (Exception) { }
        }
    }
}
