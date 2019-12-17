using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace IoTClient.Tool.Controls
{
    public partial class PortsControl : UserControl
    {
        /// <summary>
        /// 串行端口对象
        /// </summary>
        private SerialPort serialPort;
        int[] BaudRateArr = new int[] { 9600, 4800, 2400, 1200, 300, 110 };
        int[] DataBitArr = new int[] { 8, 7, 6 };
        float[] StopBitArr = new float[] { 1, 1.5f, 2 };
        string[] Encodings = new string[] { "ASCII", "UTF8", "UTF32", "UTF7", "Unicode" };
        object[] CheckBitArr = new object[] { "None", "Odd", "Even", "Mark", "Space" };
        Encoding encoding = Encoding.ASCII;
        public PortsControl()
        {
            InitializeComponent();
            Size = new Size(880, 450);

            cb_portNameSend.DataSource = SerialPort.GetPortNames();
            cb_baudRate.DataSource = BaudRateArr;
            cb_dataBit.DataSource = DataBitArr;
            cb_stopBit.DataSource = StopBitArr;
            cb_parity.DataSource = CheckBitArr;
            cb_encoding.DataSource = Encodings;

            cb_portNameSend.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_encoding.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_stopBit.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_parity.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_dataBit.DropDownStyle = ComboBoxStyle.DropDownList;

            SetEncoding();
            serialPort = new SerialPort();
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }

        /// <summary>
        /// 更新串口名
        /// </summary>
        public void UpdatePortNames()
        {
            cb_portNameSend.DataSource = SerialPort.GetPortNames();
        }

        private void butopen_Click(object sender, EventArgs e)
        {
            try
            {
                var parity = cb_parity.SelectedIndex == 0 ? Parity.None : (cb_parity.SelectedIndex == 1 ? Parity.Odd : Parity.Even);
                serialPort.PortName = cb_portNameSend.Text.ToString();
                serialPort.BaudRate = int.Parse(cb_baudRate.Text.ToString());
                serialPort.DataBits = int.Parse(cb_dataBit.Text.ToString());
                serialPort.StopBits = cb_stopBit.Text == "1.5" ? StopBits.OnePointFive : (StopBits)int.Parse(cb_stopBit.Text.ToString());
                serialPort.Parity = parity;
                serialPort.ReadTimeout = 1000;//1秒
                serialPort.Open();
                but_close.Enabled = true;
                but_open.Enabled = false;
                cb_baudRate.Enabled = false;
                cb_dataBit.Enabled = false;
                cb_stopBit.Enabled = false;
                cb_parity.Enabled = false;
                cb_portNameSend.Enabled = false;
                UpdatePortNames();
                AppendText("打开连接");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butwrite_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort.Encoding = encoding;
                serialPort.Write(txt_msg.Text);
                AppendText($"[发送数据]{txt_msg.Text}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butclose_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort.Close();
                but_open.Enabled = true;
                cb_baudRate.Enabled = true;
                cb_dataBit.Enabled = true;
                cb_stopBit.Enabled = true;
                cb_parity.Enabled = true;
                cb_portNameSend.Enabled = true;
                but_close.Enabled = false;
                UpdatePortNames();
                AppendText("关闭连接");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 接收数据回调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);
            string str = encoding.GetString(buffer);
            AppendText($"[接收数据]{str}");
        }

        private void cb_encoding_SelectedValueChanged(object sender, EventArgs e)
        {
            SetEncoding();
        }

        /// <summary>
        /// 设置编码
        /// </summary>
        /// <returns></returns>
        private void SetEncoding()
        {
            switch (cb_encoding.SelectedValue)
            {
                case "ASCII":
                    encoding = Encoding.ASCII;
                    break;
                case "UTF8":
                    encoding = Encoding.UTF8;
                    break;
                case "UTF32":
                    encoding = Encoding.UTF32;
                    break;
                case "UTF7":
                    encoding = Encoding.UTF7;
                    break;
                case "Unicode":
                    encoding = Encoding.Unicode;
                    break;
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
