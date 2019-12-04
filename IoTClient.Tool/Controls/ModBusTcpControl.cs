using IoTClient.Clients.ModBus;
using IoTClient.Common.Helpers;
using IoTServer.Common;
using IoTServer.Servers.ModBus;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IoTClient.Tool
{
    public partial class ModBusTcpControl : UserControl
    {
        IModBusClient client;
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

            lab_address.Location = new Point(9, 22);
            txt_address.Location = new Point(39, 18);
            txt_address.Size = new Size(88, 21);
            but_read.Location = new Point(132, 17);

            lab_value.Location = new Point(227, 22);
            txt_value.Location = new Point(249, 18);
            txt_value.Size = new Size(74, 21);
            but_write.Location = new Point(328, 17);

            txt_dataPackage.Location = new Point(430, 18);
            txt_dataPackage.Size = new Size(186, 21);
            but_sendData.Location = new Point(620, 17);

            chb_show_package.Location = new Point(776, 19);

            but_read.Enabled = false;
            but_write.Enabled = false;
            button2.Enabled = false;
            but_close.Enabled = false;
            but_sendData.Enabled = false;
            toolTip1.SetToolTip(button1, "开启本地ModBusTcp服务端仿真模拟服务");
            toolTip1.SetToolTip(but_open, "点击打开连接");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                server?.Stop();
                server = new ModBusTcpServer(502);
                server.Start();
                button1.Enabled = false;
                button2.Enabled = true;
                AppendText($"开启仿真模拟服务");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server?.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
            AppendText($"关闭仿真模拟服务");
        }

        private void but_open_Click(object sender, EventArgs e)
        {
            try
            {
                client?.Close();
                if (chb_rtudata.Checked)
                    client = new ModBusTcpRtuClient(txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()));
                else
                    client = new ModBusTcpClient(txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()));
                var result = client.Open();
                if (result.IsSucceed)
                {
                    but_read.Enabled = true;
                    but_write.Enabled = true;
                    but_open.Enabled = false;
                    but_close.Enabled = true;
                    but_sendData.Enabled = true;
                    AppendText($"连接成功");
                }
                else
                    MessageBox.Show($"连接失败：{result.Err}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void but_close_Click(object sender, EventArgs e)
        {
            client?.Close();
            but_open.Enabled = true;
            but_close.Enabled = false;
            but_sendData.Enabled = false;
            AppendText($"连接关闭");
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
                else if (rd_discrete.Checked)
                {
                    result = client.ReadDiscrete(txt_address.Text, stationNumber);
                }

                if (result.IsSucceed)
                    AppendText($"[读取 {txt_address.Text?.Trim()} 成功]：{result.Value}");
                else
                    AppendText($"[读取 {txt_address.Text?.Trim()} 失败]：{result.Err}");
                if (chb_show_package.Checked || (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    AppendText($"[请求报文]{result.Requst}");
                    AppendText($"[响应报文]{result.Response}\r\n");
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
                else if (rd_discrete.Checked)
                {
                    AppendText($"离散类型只读");
                    return;
                }

                if (result.IsSucceed)
                    AppendText($"[写入 {txt_address.Text?.Trim()} 成功]：{txt_value.Text?.Trim()} OK");
                else
                    AppendText($"[写入 {txt_address.Text?.Trim()} 失败]：{result.Err}");
                if (chb_show_package.Checked || (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    AppendText($"[请求报文]{result.Requst}");
                    AppendText($"[响应报文]{result.Response}\r\n");
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
            AppendText($"数据清空成功\r\n");
        }

        private void but_sendData_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_dataPackage.Text))
                {
                    MessageBox.Show("请输入要发送的报文");
                    return;
                }
                var dataPackageString = txt_dataPackage.Text.Replace(" ", "");
                if (dataPackageString.Length % 2 != 0)
                {
                    MessageBox.Show("请输入正确的的报文");
                    return;
                }

                var dataPackage = DataConvert.StringToByteArray(txt_dataPackage.Text?.Trim(), false);
                var msg = client.SendPackage(dataPackage);
                AppendText($"[请求报文]{string.Join(" ", dataPackage.Select(t => t.ToString("X2")))}");
                AppendText($"[响应报文]{string.Join(" ", msg.Select(t => t.ToString("X2")))}\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                client.Close();
                client.Open();
            }
        }

        private void AppendText(string content)
        {
            txt_content.Invoke((Action)(() =>
            {
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]{content}\r\n");
            }));
        }
    }
}
