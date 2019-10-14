using IoTServer.Common;
using System;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace IoTClient.Tests
{
    public class SocketTempTests
    {
        /// <summary>
        /// 分批缓冲区大小
        /// </summary>
        int BufferSize = 4096;
        /// <summary>
        /// 第一次握手报文
        /// </summary>
        byte[] Head1 = new byte[22]
        {
            0x03,  // 01 RFC1006 Header             
            0x00,  // 02 通常为 0             
            0x00,  // 03 数据长度，高位            
            0x16,  // 04 数据长度，地位           
            0x11,  // 05 连接类型0x11:tcp  0x12 ISO-on-TCP               
            0xE0,  // 06 主动建立连接              
            0x00,  // 07 本地接口ID               
            0x00,  // 08 主动连接时为0              
            0x00,  // 09 该参数未使用              
            0x01,  // 10            
            0x00,  // 11          
            0xC1,  // 12           
            0x02,  // 13          
            0x10,  // 14             
            0x00,  // 15            
            0xC2,  // 16             
            0x02,  // 17           
            0x03,  // 18            
            0x00,  // 19 指示cpu     
            0xC0,  // 20              
            0x01,  // 21            
            0x0A   // 22       
        };
        /// <summary>
        /// 第二次握手报文
        /// </summary>
        byte[] Head2 = new byte[25]
        {
            0x03,
            0x00,
            0x00,
            0x19,
            0x02,
            0xF0,
            0x80,
            0x32,
            0x01,
            0x00,
            0x00,
            0xCC,
            0xC1,
            0x00,
            0x08,
            0x00,
            0x00,
            0xF0,  // 设置通讯      
            0x00,
            0x00,
            0x01,
            0x00,
            0x01,
            0x03,
            0xC0
        };

        [Fact]
        public void SiemensS7_200smart_Test1()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var ip = IPAddress.Parse("ip".GetConfig());
                var port = int.Parse("port".GetConfig());

                //第一次握手
                socket.Connect(new IPEndPoint(ip, port));
                socket.Send(Head1);
                var head1 = SocketRead(socket, 4);
                var data1 = SocketRead(socket, GetContentLength(head1));

                //第二次握手
                socket.Send(Head2);
                var head2 = SocketRead(socket, 4);
                var data2 = SocketRead(socket, GetContentLength(head2));

                //发送读取信息
                var plccommand = GetCommand(0x84, 5012 * 8, 1, 4);
                socket.Send(plccommand);
                var head3 = SocketRead(socket, 4);
                var data3 = SocketRead(socket, GetContentLength(head3));

                byte[] buffer = new byte[4];
                Array.Copy(data3, 21, buffer, 0, 4);

                byte[] buffer2 = new byte[4];
                buffer2[0] = buffer[0 + 3];
                buffer2[1] = buffer[0 + 2];
                buffer2[2] = buffer[0 + 1];
                buffer2[3] = buffer[0 + 0];
                var obj = BitConverter.ToSingle(buffer2, 0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取命令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="beginAddress"></param>
        /// <param name="dbAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] GetCommand(byte type, int beginAddress, ushort dbAddress, ushort count)
        {
            byte[] command = new byte[31];
            command[0] = 0x03;
            command[1] = 0x00; 
            command[2] = 0x00;
            command[3] = 0x1F; 
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;
            command[9] = 0x00;
            command[10] = 0x00; 
            command[11] = 0x00;
            command[12] = 0x01; 
            command[13] = 0x00;
            command[14] = 0x0E;
            command[15] = 0x00;
            command[16] = 0x00;
            command[17] = 0x04;
            command[18] = 0x01;
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x02; 
            command[23] = (byte)(count / 256);
            command[24] = (byte)(count % 256); 
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256); 
            command[27] = type; 
            command[28] = (byte)(beginAddress / 256 / 256);
            command[29] = (byte)(beginAddress / 256);
            command[30] = (byte)(beginAddress % 256);
            return command;
        }
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="receiveCount"></param>
        /// <returns></returns>
        public byte[] SocketRead(Socket socket, int receiveCount)
        {
            byte[] receiveBytes = new byte[receiveCount];
            int receiveFinish = 0;
            while (receiveFinish < receiveCount)
            {
                // 分批读取
                int receiveLength = (receiveCount - receiveFinish) >= BufferSize ? BufferSize : (receiveCount - receiveFinish);
                receiveFinish += socket.Receive(receiveBytes, receiveFinish, receiveLength, SocketFlags.None);
            }
            return receiveBytes;
        }
        /// <summary>
        /// 获取需要读取的长度
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public int GetContentLength(byte[] head)
        {
            if (head?.Length >= 4) 
                return head[2] * 256 + head[3] - 4; 
            else 
                throw new ArgumentException("请传入正确的参数"); 
        }
    }
}
