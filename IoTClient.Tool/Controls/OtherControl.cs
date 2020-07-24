using IoTClient.Common.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Talk.Redis;

namespace IoTClient.Tool.Controls
{
    public partial class OtherControl : UserControl
    {
        private Socket socketTcp;
        private UdpClient udpClient;
        public OtherControl()
        {
            InitializeComponent();
            but_tcpclose.Enabled = false;
            but_tcpsend.Enabled = false;
            but_udpclose.Enabled = false;
            but_udpsend.Enabled = false;
        }

        private void but_crc16calculate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_crcstr.Text) || txt_crcstr.Text.Trim().Replace(" ", "").Length % 2 != 0)
            {
                MessageBox.Show("请传入有效的参数");
                return;
            }

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
            if (string.IsNullOrWhiteSpace(txt_crcstr.Text) || txt_crcstr.Text.Trim().Replace(" ", "").Length % 2 != 0)
            {
                MessageBox.Show("请传入有效的参数");
                return;
            }

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
            if (string.IsNullOrWhiteSpace(txt_tcpip.Text) || string.IsNullOrWhiteSpace(txt_tcpport.Text))
            {
                MessageBox.Show("请传入有效的IP和端口");
                return;
            }

            socketTcp?.Close();
            socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socketTcp.SendTimeout = 1000 * 2;
                socketTcp.ReceiveTimeout = 1000 * 2;
                socketTcp.Connect(new IPEndPoint(IPAddress.Parse(txt_tcpip.Text?.Trim()), int.Parse(txt_tcpport.Text?.Trim())));
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
            socketTcp?.Close();
            AppendText("关闭连接成功");
            but_tcpopen.Enabled = true;
        }

        private void but_tcpSend_Click(object sender, EventArgs e)
        {
            try
            {
                var command = DataConvert.StringToByteArray(txt_tcpmsg.Text, false);
                socketTcp.Send(command);
                var msg = SocketRead(socketTcp, 4096);
                AppendText(msg.ByteArrayToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //重新连接
                socketTcp?.Close();
                socketTcp.Connect(new IPEndPoint(IPAddress.Parse(txt_tcpip.Text?.Trim()), int.Parse(txt_tcpport.Text?.Trim())));
            }
        }


        protected byte[] SocketRead(Socket socket, int receiveCount)
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

        private void but_udpOpen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_tcpip.Text) || string.IsNullOrWhiteSpace(txt_tcpport.Text))
            {
                MessageBox.Show("请传入有效的IP和端口");
                return;
            }

            try
            {
                udpClient = new UdpClient();
                udpClient.Connect(new IPEndPoint(IPAddress.Parse(txt_udpip.Text?.Trim()), int.Parse(txt_udpport.Text?.Trim())));
                AppendText("打开连接成功");
                but_udpopen.Enabled = false;
                but_udpclose.Enabled = true;
                but_udpsend.Enabled = true;
            }
            catch (Exception ex)
            {
                AppendText("打开连接失败" + ex.Message);
            }
        }

        private void but_udpSend_Click(object sender, EventArgs e)
        {
            try
            {
                var command = DataConvert.StringToByteArray(txt_udpmsg.Text, false);
                udpClient.Send(command, command.Length);

                var ep = new IPEndPoint(IPAddress.Any, 0);
                var msg = udpClient.Receive(ref ep);
                AppendText(msg.ByteArrayToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                udpClient?.Close();
                udpClient.Connect(new IPEndPoint(IPAddress.Parse(txt_udpip.Text?.Trim()), int.Parse(txt_udpport.Text?.Trim())));
            }
        }

        private void but_udpClose_Click(object sender, EventArgs e)
        {
            udpClient?.Close();
            AppendText("关闭连接成功");
            but_udpopen.Enabled = true;
            but_udpclose.Enabled = false;
            but_udpsend.Enabled = false;
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
        private RedisManager redis;
        private void redisOpen_Click(object sender, EventArgs e)
        {
            var address = txt_address.Text?.Trim().Split(':');
            var ip = address[0];
            var port = address.Length >= 2 ? address[1] : "6379";
            var dbindex = address.Length >= 3 ? address[2] : "1";
            var config = $"{ip}:{port},allowAdmin=true,password=,syncTimeout=15000,defaultdatabase={dbindex}";
            var initialTime = DateTime.Now;
            redis = new RedisManager(int.Parse(dbindex), config);
            var timeConsuming = (DateTime.Now - initialTime).TotalMilliseconds;
            AppendText($"连接成功\t\t\t\t耗时：{timeConsuming}ms");
        }

        private void redisRead_Click(object sender, EventArgs e)
        {
            if (redis == null)
            {
                MessageBox.Show("请先打开连接");
                return;
            }
            var key = txt_redisKey.Text?.Trim();
            var initialTime = DateTime.Now;
            var value = redis.GetString(key);
            var timeConsuming = (DateTime.Now - initialTime).TotalMilliseconds;
            AppendText($"[读取 {key} 成功]：{value}\t\t耗时：{timeConsuming}ms");
        }

        private void redisSet_Click(object sender, EventArgs e)
        {
            if (redis == null)
            {
                MessageBox.Show("请先打开连接");
                return;
            }
            var key = txt_redisKey.Text?.Trim();
            var value = txt_redisValue.Text?.Trim();
            var initialTime = DateTime.Now;
            var isOK = redis.Set(key, value);
            var timeConsuming = (DateTime.Now - initialTime).TotalMilliseconds;
            if (isOK)
                AppendText($"[写入 {key} 成功]：{value}\t\t耗时：{timeConsuming}ms");
            else
                AppendText($"[写入 {key} 失败]：{value}\t\t耗时：{timeConsuming}ms");
        }
    }
}
