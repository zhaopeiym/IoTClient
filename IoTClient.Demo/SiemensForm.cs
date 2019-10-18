using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using IoTServer.Servers.PLC;
using System;
using System.Windows.Forms;

namespace IoTClient.Demo
{
    public partial class SiemensForm : Form
    {
        SiemensClient client;
        SiemensServer server;
        public SiemensForm()
        {
            InitializeComponent();
            but_read.Enabled = false;
            but_write.Enabled = false;
            rd_bit.Enabled = false;
        }

        private void SiemensForm_Load(object sender, EventArgs e)
        {
            //but_server.Enabled = false;
        }

        private void but_open_Click(object sender, EventArgs e)
        {
            client?.Close();
            if (but_open.Text == "连接")
            {
                client = new SiemensClient(SiemensVersion.S7_200Smart, txt_ip.Text?.Trim(), int.Parse(txt_port.Text.Trim()));
                if (!client.Open())
                    MessageBox.Show("连接失败");
                else
                {
                    but_open.Text = "已连接";
                    but_read.Enabled = true;
                    but_write.Enabled = true;
                }
            }
            else
            {
                but_open.Text = "连接";
                client?.Close();
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_address.Text))
                {
                    MessageBox.Show("请输入地址");
                    return;
                }
                dynamic result = null;
                if (rd_bit.Checked)
                {
                    result = client.ReadBoolean(txt_address.Text);
                }
                else if (rd_short.Checked)
                {
                    result = client.ReadInt16(txt_address.Text);
                }
                else if (rd_ushort.Checked)
                {
                    result = client.ReadUInt16(txt_address.Text);
                }
                else if (rd_int.Checked)
                {
                    result = client.ReadInt32(txt_address.Text);
                }
                else if (rd_uint.Checked)
                {
                    result = client.ReadUInt32(txt_address.Text);
                }
                else if (rd_long.Checked)
                {
                    result = client.ReadInt64(txt_address.Text);
                }
                else if (rd_ulong.Checked)
                {
                    result = client.ReadUInt64(txt_address.Text);
                }
                else if (rd_float.Checked)
                {
                    result = client.ReadFloat(txt_address.Text);
                }
                else if (rd_double.Checked)
                {
                    result = client.ReadDouble(txt_address.Text);
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
            catch (Exception ex)
            {
                //client?.Close();
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///  写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
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
                    result = client.Write(txt_address.Text, coil);
                }
                else if (rd_short.Checked)
                {
                    result = client.Write(txt_address.Text, short.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_ushort.Checked)
                {
                    result = client.Write(txt_address.Text, ushort.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_int.Checked)
                {
                    result = client.Write(txt_address.Text, int.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_uint.Checked)
                {
                    result = client.Write(txt_address.Text, uint.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_long.Checked)
                {
                    result = client.Write(txt_address.Text, long.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_ulong.Checked)
                {
                    result = client.Write(txt_address.Text, ulong.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_float.Checked)
                {
                    result = client.Write(txt_address.Text, float.Parse(txt_value.Text?.Trim()));
                }
                else if (rd_double.Checked)
                {
                    result = client.Write(txt_address.Text, double.Parse(txt_value.Text?.Trim()));
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void but_server_Click(object sender, EventArgs e)
        {
            if (but_server.Text == "本地模拟服务")
            {
                but_server.Text = "已开启服务";
                server = new SiemensServer(txt_ip.Text?.Trim(), int.Parse(txt_port.Text.Trim()));
                server.Start();
            }
            else
            {
                but_server.Text = "本地模拟服务";
                server?.Close();
            }
        }
    }
}
