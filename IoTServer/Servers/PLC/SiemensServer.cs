using IoTClient.Common.Constants;
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
    /// 西门子PLC 服务端模拟
    /// </summary>
    public class SiemensServer : ServerSocketBase
    {
        private Socket socketServer;
        private string ip;
        private int port;
        List<Socket> sockets = new List<Socket>();
        DataPersist dataPersist;
        public SiemensServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("SiemensServer");
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
            socketServer.Listen(100);

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
                    byte[] requetData1 = new byte[4];
                    //读取客户端发送过来的数据                   
                    requetData1 = SocketRead(newSocket, requetData1.Length);
                    byte[] requetData2 = new byte[requetData1[3] - 4];
                    requetData2 = SocketRead(newSocket, requetData2.Length);
                    var requetData = requetData1.Concat(requetData2).ToArray();

                    //如果是连接的时候发送的两次初始化指令 则直接跳过
                    if (requetData[3] == SiemensConstant.Command1_200Smart.Length || requetData[3] == SiemensConstant.Command2_200Smart.Length)
                    {
                        newSocket.Send(requetData);
                        continue;
                    }

                    switch (requetData[17])
                    {
                        //读
                        case 4:
                            {
                                var dbNumber = requetData[18];


                                //读取数据长度
                                var readLength = 0;
                                for (int i = 0; i < dbNumber; i++)
                                {
                                    var tempLength = requetData[23 + i * 12] * 256 + requetData[24 + i * 12];
                                    if (tempLength == 1 && i < dbNumber - 1)
                                        tempLength += 1;
                                    readLength += tempLength;
                                }

                                var dataContent = new byte[4 * dbNumber + readLength];//可以从报文中获取Length？
                                var cursor = 0;
                                for (int i = 0; i < dbNumber; i++)
                                {
                                    var address = requetData[28 + i * 12] * 256 * 256 + requetData[29 + i * 12] * 256 + requetData[30 + i * 12];
                                    var tempReadLenght = requetData[23 + i * 12] * 256 + requetData[24 + i * 12];
                                    if (tempReadLenght == 1 && i < dbNumber - 1)
                                        tempReadLenght += 1;

                                    var typeDB = requetData[27 + i * 12];
                                    var stationNumberKey = $"s200-{typeDB}";
                                    var value = dataPersist.Read(stationNumberKey);//TODO 数据存在 25、26   
                                    var byteArray = JsonConvert.DeserializeObject<byte[]>(value) ?? new byte[65536];

                                    DataConvert.StringToByteArray("FF 09 00 04").CopyTo(dataContent, 0 + cursor);
                                    dataContent[1 + cursor] = readLength == 1 ? (byte)0x03 : (byte)0x04;//04 byte(字节) 03bit（位）
                                    dataContent[2 + cursor] = (byte)(readLength / 256);
                                    dataContent[3 + cursor] = (byte)(readLength % 256);

                                    Buffer.BlockCopy(byteArray, address / 8, dataContent, 4 + cursor, tempReadLenght);
                                    cursor += 4 + tempReadLenght;
                                    //dataContent[2] dataContent[3] 后半截数据数组的Length
                                    //dataContent[4]-[7] 返回给客户端的数据
                                }


                                byte[] responseData1 = new byte[21 + dataContent.Length];
                                DataConvert.StringToByteArray("03 00 00 1A 02 F0 80 32 03 00 00 00 01 00 02 00 00 00 00 04 01").CopyTo(responseData1, 0);
                                //responseData1[8] = 0x03;//1  客户端发送命令 3 服务器回复命令 
                                responseData1[2] = (byte)(responseData1.Length / 256);
                                responseData1[3] = (byte)(responseData1.Length % 256);
                                responseData1[15] = (byte)(requetData.Length / 256);
                                responseData1[16] = (byte)(requetData.Length % 256);
                                responseData1[20] = requetData[18];
                                dataContent.CopyTo(responseData1, 21);
                                //当读取的是字符串的时候[25]存储的后面的数据长度，参考SiemensClient的Write(string address, string value)方法。
                                //Buffer.BlockCopy(bytes, 0, responseData1, responseData1.Length - bytes.Length, bytes.Length);
                                newSocket.Send(responseData1);

                            }
                            break;
                        //写
                        case 5:
                            {
                                var address = requetData[28] * 256 * 256 + requetData[29] * 256 + requetData[30];
                                var typeDB = requetData[27];
                                var stationNumberKey = $"s200-{typeDB}";

                                var value = new byte[requetData.Length - 35];
                                Buffer.BlockCopy(requetData, 35, value, 0, value.Length);

                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(stationNumberKey)) ?? new byte[65536];
                                value.CopyTo(byteArray, address / 8);
                                //存储字节数据到内存
                                dataPersist.Write(stationNumberKey, JsonConvert.SerializeObject(byteArray));

                                byte[] responseData1 = new byte[22];
                                DataConvert.StringToByteArray("03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 FF").CopyTo(responseData1, 0);
                                newSocket.Send(responseData1);
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

