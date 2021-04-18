using IoTClient.Common.Helpers;
using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IoTServer.Servers.PLC
{
    /// <summary>
    /// 欧姆龙Fins 服务端模拟
    /// </summary>
    public class OmronFinsServer : ServerSocketBase, IIoTServer
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
        public OmronFinsServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("OmronFinsServer");
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

        private byte[] BasicCommand = new byte[]
        {
            0x46, 0x49, 0x4E, 0x53,
            0x00, 0x00, 0x00, 0x0C,// 后面长度
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x01
        };

        private bool ByteArryThan(byte[] items1, byte[] items2)
        {
            if (items1.Length != items2.Length)
                return false;
            else
            {
                //只判断前16个字节
                for (int i = 0; i < 16; i++)
                {
                    if (items1[i] != items2[i])
                        return false;
                }
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

                    byte[] requetData1 = new byte[20];
                    //读取客户端发送过来的数据                   
                    requetData1 = SocketRead(newSocket, requetData1.Length);
                    if (ByteArryThan(requetData1, BasicCommand))
                    {
                        var response = DataConvert.StringToByteArray("46 49 4E 53 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01");
                        newSocket.Send(response);
                        continue;
                    }

                    byte[] requetData2 = new byte[34 - 20];
                    requetData2 = SocketRead(newSocket, requetData2.Length);

                    completeRequetData = requetData1.Concat(requetData2).ToArray();

                    var isBit = completeRequetData[28] == 0x02 ||
                      completeRequetData[28] == 0x30 ||
                      completeRequetData[28] == 0x31 ||
                      completeRequetData[28] == 0x32 ||
                      completeRequetData[28] == 0x33;

                    var addressLenght = isBit ? 1 : 2;

                    //1 读 2 写
                    var readWriteCode = requetData2[27 - 20];
                    //读写数据长度
                    var readWriteLength = requetData2[32 - 20] * 256 + requetData2[33 - 20];
                    if (readWriteCode == 0x02)//写
                    {
                        byte[] requetData3 = SocketRead(newSocket, readWriteLength * addressLenght);
                        completeRequetData = completeRequetData.Concat(requetData3).ToArray();
                    }

                    var dataKey = $"{completeRequetData[28]}";
                    var beginAddress = completeRequetData[30] + completeRequetData[29] * 256;
                    var bitAddress = completeRequetData[31];

                    switch (completeRequetData[27])
                    {
                        //读
                        case 0x01:
                            {
                                //把存储的数据转为字节数组                              
                                var dataValue = dataPersist.Read(dataKey);
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataValue) ?? new byte[65536];

                                //数据的字节长度
                                var dataLenght = readWriteLength * addressLenght;

                                // 响应报文总长度
                                byte[] responseData = new byte[30 + dataLenght];

                                DataConvert.StringToByteArray("46 49 4E 53 00 00 00 1A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00").CopyTo(responseData, 0);
                                //存储的是后面还有多少长度
                                responseData[4] = BitConverter.GetBytes(22 + dataLenght)[3];
                                responseData[5] = BitConverter.GetBytes(22 + dataLenght)[2];
                                responseData[6] = BitConverter.GetBytes(22 + dataLenght)[1];
                                responseData[7] = BitConverter.GetBytes(22 + dataLenght)[0];
                                if (isBit)
                                    Buffer.BlockCopy(byteArray, beginAddress * 16 + bitAddress, responseData, 30, dataLenght);
                                else
                                    Buffer.BlockCopy(byteArray, beginAddress * addressLenght, responseData, 30, dataLenght);

                                newSocket.Send(responseData);
                            }
                            break;
                        //写
                        case 0x02:
                            {
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(dataKey)) ?? new byte[65536];
                                //[32]后面存的是数据
                                var valueByte = new byte[completeRequetData.Length - 34];
                                Buffer.BlockCopy(completeRequetData, 34, valueByte, 0, valueByte.Length);

                                //var tempValue1 = BitConverter.ToSingle(valueByte.Reverse().ToArray().ByteFormatting(EndianFormat.CDAB), 0);

                                if (isBit)
                                    valueByte.CopyTo(byteArray, beginAddress * 16 + bitAddress);
                                else
                                    valueByte.CopyTo(byteArray, beginAddress * addressLenght);
                                dataPersist.Write(dataKey, JsonConvert.SerializeObject(byteArray));

                                newSocket.Send(DataConvert.StringToByteArray("46 49 4E 53 00 00 00 16 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"));
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    //if (newSocket?.Connected ?? false) newSocket?.Shutdown(SocketShutdown.Both);
                    //newSocket?.Close();
                    //throw ex;
                }
            }
        }
    }
}
