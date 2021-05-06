using IoTClient.Common.Helpers;
using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IoTServer.Servers.PLC
{
    public class AllenBradleyServer : ServerSocketBase, IIoTServer
    {
        private Socket socketServer;
        private string ip;
        private int port;
        List<Socket> sockets = new List<Socket>();
        DataPersist dataPersist;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="ip"></param>
        public AllenBradleyServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("AllenBradley");
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            //1 创建Socket对象
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //2 绑定ip和端口 
            var ipaddress = string.IsNullOrWhiteSpace(ip) ? IPAddress.Any : IPAddress.Parse(ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipaddress, port);
            socketServer.Bind(ipEndPoint);

            //3、开启侦听(等待客户机发出的连接),并设置最大客户端连接数为10
            socketServer.Listen(10);

            Task.Run(() => { Accept(socketServer); });
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (socketServer?.Connected ?? false) socketServer.Shutdown(SocketShutdown.Both);
            socketServer?.Close();
        }

        /// <summary>
        /// 客户端连接到服务端
        /// </summary>
        /// <param name="socket"></param>
        void Accept(Socket socket)
        {
            while (true)
            {
                try
                {
                    Socket newSocket = null;
                    try
                    {
                        //阻塞等待客户端连接
                        newSocket = socket.Accept();
                        sockets.Add(newSocket);
                        Task.Run(() => { Receive(newSocket); });
                    }
                    catch (SocketException)
                    {
                        foreach (var item in sockets)
                        {
                            if (item?.Connected ?? false) item.Shutdown(SocketShutdown.Both);
                            item?.Close();
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.Interrupted)
                        throw ex;
                }

            }
        }

        private byte[] GetBasicCommand()
        {
            byte[] command = new byte[28];
            command[0] = 0x65;
            command[2] = 0x04;
            command[24] = 0x01;
            return command;
        }

        private bool ByteArryThan(byte[] items1, byte[] items2)
        {
            if (items1.Length != items2.Length)
                return false;
            else
            {
                if (items1[0] != items2[0])
                    return false;
                if (items1[2] != items2[2])
                    return false;
                if (items1[24] != items2[24])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 接收客户端发送的消息
        /// </summary>
        /// <param name="newSocket"></param>
        void Receive(Socket newSocket)
        {
            while (newSocket.Connected)
            {
                try
                {
                    byte[] completeRequetData = null;

                    byte[] requetData1 = new byte[28];
                    //读取客户端发送过来的数据                   
                    requetData1 = SocketRead(newSocket, requetData1.Length);
                    if (ByteArryThan(requetData1, GetBasicCommand()))
                    {
                        //连接握手
                        var response = DataConvert.StringToByteArray("65 00 04 00 57 01 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00");
                        newSocket.Send(response);
                        continue;
                    }

                    byte[] requetData2 = new byte[40 - 28];
                    requetData2 = SocketRead(newSocket, requetData2.Length);

                    var requetLength = requetData2[requetData2.Length - 1] * 256 + requetData2[requetData2.Length - 2];
                    byte[] requetData3 = SocketRead(newSocket, requetLength);

                    completeRequetData = requetData1.Concat(requetData2).Concat(requetData3).ToArray();

                    var requst = string.Join(" ", completeRequetData.Select(t => t.ToString("X2")));

                    var address_ASCII_Length = completeRequetData[1 + 26 + 24] * 2 - 2;
                    var address_ASCII = new byte[address_ASCII_Length];
                    Buffer.BlockCopy(completeRequetData, 4 + 26 + 24, address_ASCII, 0, address_ASCII_Length);
                    var address = Encoding.ASCII.GetString(address_ASCII);

                    var CIP_Length = completeRequetData[24 + 24] + completeRequetData[25 + 24] * 256;

                    switch (completeRequetData[26 + 24])
                    {
                        //读
                        case 0x4C:
                            {
                                var value_byte = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(address)) ?? new byte[4];
                                var response = DataConvert.StringToByteArray("66 00 1A 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 02 00 00 00 00 00 B2 00 0A 00 CC 00 00 00 00 00 7B 00 00 00");
                                BitConverter.GetBytes((ushort)(value_byte.Length + 6)).CopyTo(response, 38);
                                value_byte.CopyTo(response, 46);
                                var response_str = string.Join(" ", response.Select(t => t.ToString("X2")));
                                newSocket.Send(response);
                            }
                            break;
                        //写
                        case 0x4D:
                            {
                                var value_Length = CIP_Length - address_ASCII_Length - 8;
                                var value_byte = new byte[value_Length];
                                Buffer.BlockCopy(completeRequetData, 8 + 26 + 24 + address_ASCII.Length, value_byte, 0, value_Length);
                                //var value = BitConverter.ToInt16(value_byte, 0);
                                dataPersist.Write(address, JsonConvert.SerializeObject(value_byte));
                                var response = DataConvert.StringToByteArray("66 00 16 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 02 00 00 00 00 00 B2 00 06 00 CD 00 00 00 00 00");
                                newSocket.Send(response);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
