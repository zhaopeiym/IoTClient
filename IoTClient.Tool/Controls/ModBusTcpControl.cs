using IoTClient.Clients.ModBus;
using IoTServer.Common;
using IoTServer.Servers.ModBus;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IoTClient.Tool
{
    public partial class ModBusTcpControl : UserControl
    {
        ModBusTcpClient client;
        ModBusTcpServer server;
        public ModBusTcpControl()
        {
            InitializeComponent();
            Size = new Size(880, 450);
            groupBox2.Location = new Point(13, 5);
            groupBox2.Size = new Size(855, 50);
            groupBox1.Location = new Point(13, 55);
            groupBox1.Size = new Size(855, 50);
            groupBox3.Location = new Point(13, 105);
            groupBox3.Size = new Size(855, 50);
            txt_content.Location = new Point(13, 160);
            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = false;
            toolTip1.SetToolTip(button1, "开启本地ModBusTcp服务端仿真模拟服务");
            toolTip1.SetToolTip(but_open, "点击打开连接");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server?.Close();
            server = new ModBusTcpServer(502);
            server.Start();
            button1.Enabled = false;
            button2.Enabled = true;
            txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]开启仿真模拟服务\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server?.Close();
            button1.Enabled = true;
            button2.Enabled = false;
            txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]关闭仿真模拟服务\r\n");
        }

        private void but_open_Click(object sender, EventArgs e)
        {
            client?.Close();
            client = new ModBusTcpClient(txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()));
            var result = client.Open();
            if (result.IsSucceed)
            {
                button3.Enabled = true;
                button4.Enabled = true;
                but_open.Enabled = false;
                button5.Enabled = true;
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]连接成功\r\n");
            }
            else
                MessageBox.Show($"连接失败：{result.Err}");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            client?.Close();
            but_open.Enabled = true;
            button5.Enabled = false;
            txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]连接关闭\r\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte.TryParse(txt_stationNumber.Text?.Trim(), out byte stationNumber);
            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("请输入地址");
                return;
            }
            try
            {
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
                    txt_content.AppendText($"[响应报文]{result.Response}\r\n\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            try
            {

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
                    txt_content.AppendText($"[响应报文]{result.Response}\r\n\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataPersist.Clear();
            txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]数据清空成功\r\n\r\n");
        }
    }
}
