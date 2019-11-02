using IoTClient.Clients.ModBus;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace IoTClient.Tool.Controls
{
    public partial class ModBusRtuControl : UserControl
    {
        /// <summary>
        /// 串行端口对象
        /// </summary>
        //private SerialPort serialPort;
        private ModBusRtuClient modBusRtuClient;

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
            button2.Enabled = false;
            but_close.Enabled = false;
            UpdatePortNames();
            cb_portNameSend.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// 更新串口名
        /// </summary>
        public void UpdatePortNames()
        {
            cb_portNameSend.DataSource = SerialPort.GetPortNames();
        }

        private void but_open_Click(object sender, EventArgs e)
        {
            try
            {
                var PortName = cb_portNameSend.Text.ToString();
                var BaudRate = int.Parse(txt_baudRate.Text.ToString());
                var DataBits = int.Parse(txt_dataBit.Text.ToString());
                var StopBits = (StopBits)int.Parse(txt_stopBit.Text.ToString());

                if (modBusRtuClient == null) modBusRtuClient = new ModBusRtuClient(PortName, BaudRate, DataBits, StopBits);
                var result = modBusRtuClient.Open();
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
                UpdatePortNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AppendText(string content)
        {
            txt_content.Invoke((Action)(() =>
            {
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]{content}\r\n");
            }));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butWrite_Click(object sender, EventArgs e)
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
                    result = modBusRtuClient.Write(address, coil, stationNumber);
                }
                else if (rd_short.Checked)
                {
                    result = modBusRtuClient.Write(address, short.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_ushort.Checked)
                {
                    result = modBusRtuClient.Write(address, ushort.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_int.Checked)
                {
                    result = modBusRtuClient.Write(address, int.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_uint.Checked)
                {
                    result = modBusRtuClient.Write(address, uint.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_long.Checked)
                {
                    result = modBusRtuClient.Write(address, long.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_ulong.Checked)
                {
                    result = modBusRtuClient.Write(address, ulong.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_float.Checked)
                {
                    result = modBusRtuClient.Write(address, float.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if (rd_double.Checked)
                {
                    result = modBusRtuClient.Write(address, double.Parse(txt_value.Text?.Trim()), stationNumber);
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

        private void butClose_Click(object sender, EventArgs e)
        {
            modBusRtuClient?.Close();
            AppendText("关闭连接");
            but_open.Enabled = true;
            but_close.Enabled = false;
        }

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
                    result = modBusRtuClient.ReadCoil(txt_address.Text, stationNumber);
                }
                else if (rd_short.Checked)
                {
                    result = modBusRtuClient.ReadInt16(txt_address.Text, stationNumber);
                }
                else if (rd_ushort.Checked)
                {
                    result = modBusRtuClient.ReadUInt16(txt_address.Text, stationNumber);
                }
                else if (rd_int.Checked)
                {
                    result = modBusRtuClient.ReadInt32(txt_address.Text, stationNumber);
                }
                else if (rd_uint.Checked)
                {
                    result = modBusRtuClient.ReadUInt32(txt_address.Text, stationNumber);
                }
                else if (rd_long.Checked)
                {
                    result = modBusRtuClient.ReadInt64(txt_address.Text, stationNumber);
                }
                else if (rd_ulong.Checked)
                {
                    result = modBusRtuClient.ReadUInt64(txt_address.Text, stationNumber);
                }
                else if (rd_float.Checked)
                {
                    result = modBusRtuClient.ReadFloat(txt_address.Text, stationNumber);
                }
                else if (rd_double.Checked)
                {
                    result = modBusRtuClient.ReadDouble(txt_address.Text, stationNumber);
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
    }
}
