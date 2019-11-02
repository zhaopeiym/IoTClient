using IoTClient.Clients.ModBus;
using IoTServer.Servers.ModBus;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace IoTClient.Tool.Controls
{
    public partial class ModBusRtuControl : UserControl
    {
        private ModBusRtuClient client;
        private ModBusRtuServer server;
        public ModBusRtuControl()
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
            but_read.Enabled = false;
            but_write.Enabled = false;
            but_server_close.Enabled = false;
            but_close.Enabled = false;
            UpdatePortNames();
            cb_portNameSend.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_portNameSend_server.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// 更新串口名
        /// </summary>
        public void UpdatePortNames()
        {
            cb_portNameSend.DataSource = ModBusRtuClient.GetPortNames();
            cb_portNameSend_server.DataSource = ModBusRtuClient.GetPortNames();
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_open_Click(object sender, EventArgs e)
        {
            try
            {
                var PortName = cb_portNameSend.Text.ToString();
                var BaudRate = int.Parse(txt_baudRate.Text.ToString());
                var DataBits = int.Parse(txt_dataBit.Text.ToString());
                var StopBits = (StopBits)int.Parse(txt_stopBit.Text.ToString());
                client?.Close();
                client = new ModBusRtuClient(PortName, BaudRate, DataBits, StopBits);
                var result = client.Open();
                if (result.IsSucceed)
                {
                    but_open.Enabled = false;
                    cb_portNameSend.Enabled = false;
                    but_read.Enabled = true;
                    but_write.Enabled = true;
                    but_open.Enabled = false;
                    but_close.Enabled = true;
                    AppendText("连接成功");
                }
                else
                    AppendText($"连接失败：{result.Err}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butClose_Click(object sender, EventArgs e)
        {
            client?.Close();
            AppendText("关闭连接");
            but_open.Enabled = true;
            but_close.Enabled = false;
            cb_portNameSend.Enabled = true;
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_read_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_write_Click(object sender, EventArgs e)
        {
            var address = txt_address.Text?.Trim();
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
                    result = client.Write(address, coil, stationNumber);
                }
                else if (rd_short.Checked)
                {
                    result = client.Write(address, short.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_ushort.Checked)
                {
                    result = client.Write(address, ushort.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_int.Checked)
                {
                    result = client.Write(address, int.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_uint.Checked)
                {
                    result = client.Write(address, uint.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_long.Checked)
                {
                    result = client.Write(address, long.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_ulong.Checked)
                {
                    result = client.Write(address, ulong.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_float.Checked)
                {
                    result = client.Write(address, float.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_double.Checked)
                {
                    result = client.Write(address, double.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_discrete.Checked)
                {
                    txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]离散类型只读\r\n");
                    return;
                }

                if (result.IsSucceed)
                    txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][写入 {address?.Trim()} 成功]：{txt_value.Text?.Trim()} OK\r\n");
                else
                    txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}][写入 {address?.Trim()} 失败]：{result.Err}\r\n");
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

        /// <summary>
        /// 启动仿真服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_open_server_Click(object sender, EventArgs e)
        {
            try
            {
                var PortName = cb_portNameSend_server.Text.ToString();
                var BaudRate = int.Parse(txt_baudRate.Text.ToString());
                var DataBits = int.Parse(txt_dataBit.Text.ToString());
                var StopBits = (StopBits)int.Parse(txt_stopBit.Text.ToString());
                server?.Stop();
                server = new ModBusRtuServer(PortName, BaudRate, DataBits, StopBits);
                server.Start();
                AppendText("开启仿真服务");
                but_server_open.Enabled = false;
                but_server_close.Enabled = true;
                cb_portNameSend_server.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 关闭仿真服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_close_server_Click(object sender, EventArgs e)
        {
            server?.Stop();
            AppendText("关闭仿真服务");
            but_server_open.Enabled = true;
            but_server_close.Enabled = false;
            cb_portNameSend_server.Enabled = true;
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
