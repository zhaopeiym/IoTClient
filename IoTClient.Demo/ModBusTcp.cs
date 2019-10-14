using IoTClient.Clients.ModBus;
using IoTServer.Servers.ModBus;
using System;
using System.Windows.Forms;

namespace IoTClient.Demo
{
    public partial class ModBusTcp : Form
    {
        ModBusTcpClient client;
        public ModBusTcp()
        {
            InitializeComponent();
            button3.Enabled = false;
            button4.Enabled = false;
            toolTip1.SetToolTip(button1, "开启本地ModBusTcp服务端仿真模拟服务");
            toolTip1.SetToolTip(but_open, "点击打开连接");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModBusTcpServer server = new ModBusTcpServer("127.0.0.1", 502);
            server.Start();
            button1.Text = "模拟服务已启动";
            button1.Enabled = false;
            but_open.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client = new ModBusTcpClient(txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()));
            if (but_open.Text == "连接")
            {
                if (client.Open())
                {
                    but_open.Text = "已连接";
                    button3.Enabled = true;
                    button4.Enabled = true;
                    toolTip1.SetToolTip(but_open, "点击关闭连接");
                }
                else
                {
                    MessageBox.Show("连接失败");
                }
            }
            else
            {
                client.Close();
                but_open.Text = "连接";
                toolTip1.SetToolTip(but_open, "点击打开连接");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte.TryParse(txt_stationNumber.Text?.Trim(), out byte stationNumber);
            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("请输入地址");
                return;
            }
            var result = client.ReadInt16(ushort.Parse(txt_address.Text), stationNumber);
            if (result.IsSucceed)
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][读取 {txt_address.Text?.Trim()} 成功]：{result.Value}\r\n");
            else
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\r\n");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte.TryParse(txt_stationNumber.Text?.Trim(), out byte stationNumber);
            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("请输入地址");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_value.Text))
            {
                MessageBox.Show("请输入值");
                return;
            }
            var result = client.Write(ushort.Parse(txt_address.Text), short.Parse(txt_value.Text?.Trim()), stationNumber);
            if (result.IsSucceed)
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][写入 {txt_address.Text?.Trim()} 成功]：OK\r\n");
            else
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][写入 {txt_address.Text?.Trim()} 失败]：{result.Err}\r\n");
        }
    }
}
