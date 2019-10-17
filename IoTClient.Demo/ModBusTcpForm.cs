using IoTClient.Clients.ModBus;
using IoTServer.Servers.ModBus;
using System;
using System.Windows.Forms;

namespace IoTClient.Demo
{
    public partial class ModBusTcpForm : Form
    {
        ModBusTcpClient client;
        ModBusTcpServer server;
        public ModBusTcpForm()
        {
            InitializeComponent();
            button3.Enabled = false;
            button4.Enabled = false;
            toolTip1.SetToolTip(button1, "开启本地ModBusTcp服务端仿真模拟服务");
            toolTip1.SetToolTip(but_open, "点击打开连接");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (server == null)
            {
                server = new ModBusTcpServer("127.0.0.1", 502);

            }
            if (button1.Text == "本地模拟服务")
            {
                server.Start();
                button1.Text = "模拟服务已启动";
            }
            else
            {
                server.Close();
                button1.Text = "本地模拟服务";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (but_open.Text == "连接")
            {
                client?.Close();
                client = new ModBusTcpClient(txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()));
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
                client?.Close();
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
            dynamic result = null;
            if (rd_bit.Checked)
            {
                result = client.ReadCoil(txt_address.Text, stationNumber);
            }
            else if (rd_short.Checked)
            {
                result = client.ReadInt16(txt_address.Text, stationNumber);
            }
            else if (rd_ushort.Checked)
            {
                result = client.ReadUInt16(txt_address.Text, stationNumber);
            }
            else if (rd_int.Checked)
            {
                result = client.ReadInt32(txt_address.Text, stationNumber);
            }
            else if (rd_uint.Checked)
            {
                result = client.ReadUInt32(txt_address.Text, stationNumber);
            }
            else if (rd_long.Checked)
            {
                result = client.ReadInt64(txt_address.Text, stationNumber);
            }
            else if (rd_ulong.Checked)
            {
                result = client.ReadUInt64(txt_address.Text, stationNumber);
            }
            else if (rd_float.Checked)
            {
                result = client.ReadFloat(txt_address.Text, stationNumber);
            }
            else if (rd_double.Checked)
            {
                result = client.ReadDouble(txt_address.Text, stationNumber);
            }

            if (result.IsSucceed)
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][读取 {txt_address.Text?.Trim()} 成功]：{result.Value}\r\n");
            else
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\r\n");
            if (checkBox1.Checked)
            {
                txt_content.AppendText($"[请求报文]{result.Requst}\r\n");
                txt_content.AppendText($"[响应报文]{result.Response}\r\n");
            }
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

            dynamic result = null;
            if (rd_bit.Checked)
            {
                if (!bool.TryParse(txt_value.Text?.Trim(), out bool coil))
                {
                    if (txt_value.Text?.Trim() == "0")
                        coil = false;
                    else if (txt_value.Text?.Trim() == "1")
                        coil = true;
                    else
                    {
                        MessageBox.Show("请输入 True 或 False");
                        return;
                    }
                }
                result = client.Write(txt_address.Text, coil, stationNumber);
            }
            else if (rd_short.Checked)
            {
                result = client.Write(txt_address.Text, short.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_ushort.Checked)
            {
                result = client.Write(txt_address.Text, ushort.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_int.Checked)
            {
                result = client.Write(txt_address.Text, int.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_uint.Checked)
            {
                result = client.Write(txt_address.Text, uint.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_long.Checked)
            {
                result = client.Write(txt_address.Text, long.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_ulong.Checked)
            {
                result = client.Write(txt_address.Text, ulong.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_float.Checked)
            {
                result = client.Write(txt_address.Text, float.Parse(txt_value.Text?.Trim()), stationNumber);
            }
            else if (rd_double.Checked)
            {
                result = client.Write(txt_address.Text, double.Parse(txt_value.Text?.Trim()), stationNumber);
            }


            if (result.IsSucceed)
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][写入 {txt_address.Text?.Trim()} 成功]：{txt_value.Text?.Trim()} OK\r\n");
            else
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][写入 {txt_address.Text?.Trim()} 失败]：{result.Err}\r\n");
            if (checkBox1.Checked)
            {
                txt_content.AppendText($"[请求报文]{result.Requst}\r\n");
                txt_content.AppendText($"[响应报文]{result.Response}\r\n");
            }
        }

        private void ModBusTcp_Load(object sender, EventArgs e)
        {

        }

        private void ModBusTcpForm_FormClosed(object sender, FormClosedEventArgs e)
        {            
            //new IndexForm().Show();
        }
    }
}
