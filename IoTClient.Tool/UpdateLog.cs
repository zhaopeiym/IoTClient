using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            logs.Add("日志记录");
            logs.Add("\r\n版本：[0.4.0]");
            logs.Add($"时间：2020-09-11");
            logs.Add("内容");
            logs.Add("1.西门子PLC批量读写，大幅提高读写性能");
            logs.Add("2.手动检查更新");
            logs.Add("3.显示历史更新日志");

            logs.Add("\r\n版本：[0.4.1]");
            logs.Add($"时间：2021-01-19");
            logs.Add("内容");
            logs.Add("1.西门子PLC批量写，服务端模拟的实现");
            logs.Add("2.西门子PLC批量写Byte类型bug修复");
            logs.Add("3.ModbusTcp批量读取");
            logs.Add("4.ModbusTcp线程安全读取");

            logs.Add("\r\n版本：[0.4.2]");
            logs.Add($"时间：2021-03-10");
            logs.Add("内容");
            logs.Add("1.修复相关bug");

            logs.Add("\r\n版本：[0.4.4]");
            logs.Add($"时间：2021-03-21");
            logs.Add("内容");
            logs.Add("1.Modbus 大小端设置");

            logs.Add("\r\n版本：[0.4.5]");
            logs.Add($"时间：2021-04-14");
            logs.Add("内容");
            logs.Add("1.三菱MC_Qna-3E帧客户端实现");
            logs.Add("2.三菱MC_A-1E帧客户端实现");
            logs.Add("3.三菱MC_Qna-3E帧模拟服务端实现");
            logs.Add("4.三菱MC_A-1E帧模拟服务端实现");

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
