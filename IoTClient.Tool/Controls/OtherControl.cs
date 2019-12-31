using IoTClient.Common.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace IoTClient.Tool.Controls
{
    public partial class OtherControl : UserControl
    {
        private Socket socket;
        public OtherControl()
        {
            InitializeComponent();
            but_tcpclose.Enabled = false;
            but_tcpsend.Enabled = false;
        }

        private void but_crc16calculate_Click(object sender, EventArgs e)
        {
            var byteArry = DataConvert.StringToByteArray(txt_crcstr.Text?.Trim(), false);
            var crc16 = CRC16.GetCRC16(byteArry);
            AppendText($"CRC16计算结果：{DataConvert.ByteArrayToString(crc16)}");
        }

        private void AppendText(string content)
        {
            txt_content.Invoke((Action)(() =>
            {
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]{content}\r\n");
            }));
        }

        private void but_crc16validation_Click(object sender, EventArgs e)
        {
            var byteArry = DataConvert.StringToByteArray(txt_crcstr.Text?.Trim(), false);
            var checkCrc16 = CRC16.CheckCRC16(byteArry);
            AppendText($"CRC16验证结果：{txt_crcstr.Text} {checkCrc16}");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void but_tcpOpen_Click(object sender, EventArgs e)
        {
            socket?.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.SendTimeout = 1000 * 2;
                socket.ReceiveTimeout = 1000 * 2;
                socket.Connect(new IPEndPoint(IPAddress.Parse(txt_tcpip.Text?.Trim()), int.Parse(txt_tcpport.Text?.Trim())));
                AppendText("打开连接成功");
                but_tcpopen.Enabled = false;
                but_tcpclose.Enabled = true;
                but_tcpsend.Enabled = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void but_tcpClose_Click(object sender, EventArgs e)
        {
            socket?.Close();
            AppendText("关闭连接成功");
            but_tcpopen.Enabled = true;
        }

        private void but_tcpSend_Click(object sender, EventArgs e)
        {
            try
            {
                var command = DataConvert.StringToByteArray(txt_tcpmsg.Text, false);
                socket.Send(command);
                var msg = SocketRead(socket, 4096);
                AppendText(msg.ByteArrayToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //重新连接
                socket?.Close();
                socket.Connect(new IPEndPoint(IPAddress.Parse(txt_tcpip.Text?.Trim()), int.Parse(txt_tcpport.Text?.Trim())));
            }
        }


        protected byte[] SocketRead(Socket socket, int receiveCount)
        {
            try
            {
                byte[] receiveBytes = new byte[receiveCount];
                int receiveFinish = 0;
                var readLeng = socket.Receive(receiveBytes, receiveFinish, receiveCount, SocketFlags.None);
                if (readLeng == 0)
                {
                    if (socket.Connected) socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    throw new Exception("连接已断开");
                }
                return receiveBytes.Take(readLeng).ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //重新连接
                socket?.Close();
                socket.Connect(new IPEndPoint(IPAddress.Parse(txt_tcpip.Text?.Trim()), int.Parse(txt_tcpport.Text?.Trim())));
                return null;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
