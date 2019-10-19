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
    public class SiemensServer
    {
        private Socket socketServer;
        private string redisConfig;
        private string ip;
        private int port;
        List<Socket> sockets = new List<Socket>();
        DataPersist dataPersist;
        public SiemensServer(string ip, int port, string redisConfig = null)
        {
            this.redisConfig = redisConfig;
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist(redisConfig);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            //1 创建Socket对象
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //2 绑定ip和端口 
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socketServer.Bind(ipEndPoint);

            //3、开启侦听(等待客户机发出的连接),并设置最大客户端连接数为10
            socketServer.Listen(10); 

            Task.Run(() => { Accept(socketServer); });
        }

        public void Close()
        {
            if (socketServer?.Connected ?? false)
                socketServer.Shutdown(SocketShutdown.Both);
            socketServer?.Close();
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            dataPersist?.Clear();
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
                    int readLeng = newSocket.Receive(requetData1, 0, requetData1.Length, SocketFlags.None);
                    if (readLeng == 0)//客户端断开连接
                    {
                        newSocket.Shutdown(SocketShutdown.Both);
                        newSocket.Close();
                        return;
                    }
                    byte[] requetData2 = new byte[requetData1[3] - 4];
                    readLeng = newSocket.Receive(requetData2, 0, requetData2.Length, SocketFlags.None);
                    if (readLeng == 0)//客户端断开连接
                    {
                        if (newSocket?.Connected ?? false) newSocket.Shutdown(SocketShutdown.Both);
                        newSocket?.Close();
                        return;
                    }
                    var requetData = requetData1.Concat(requetData2).ToArray();

                    //如果是连接的时候发送的两次初始化指令 则直接跳过
                    if (requetData[3] == SiemensConstant.Command1_200Smart.Length || requetData[3] == SiemensConstant.Command2_200Smart.Length)
                    {
                        newSocket.Send(requetData);
                        continue;
                    }
                    var address = requetData[28] * 256 * 256 + requetData[29] * 256 + requetData[30];
                    switch (requetData[17])
                    {
                        //读
                        case 4:
                            {
                                var value = dataPersist.Read(address);//TODO 数据存在 25、26   
                                //把存储的数据转为字节数组
                                var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                                if (string.IsNullOrWhiteSpace(value))
                                    bytes = BitConverter.GetBytes(0);
                                var dataContent = new byte[4 + bytes.Length];//可以从报文中获取Length？
                                DataConvert.StringToByteArray("FF 09 00 04").CopyTo(dataContent, 0);
                                dataContent[1] = bytes.Length == 1 ? (byte)0x03 : (byte)0x04;//04 byte(字节) 03bit（位）
                                dataContent[2] = (byte)(bytes.Length / 256);
                                dataContent[3] = (byte)(bytes.Length % 256);
                                //dataContent[0] 固定
                                //dataContent[2] dataContent[3] 后半截数据数组的Length
                                //dataContent[4]-[7] 返回给客户端的数据
                                byte[] responseData1 = new byte[21 + dataContent.Length];
                                DataConvert.StringToByteArray("03 00 00 1A 02 F0 80 32 03 00 00 00 01 00 02 00 00 00 00 04 01").CopyTo(responseData1, 0);
                                //responseData1[0] = 0x03;
                                //responseData1[1] = 0x00; //固定协议头                                
                                //responseData1[8] = 0x03;//1  客户端发送命令 3 服务器回复命令 
                                responseData1[2] = (byte)(responseData1.Length / 256);
                                responseData1[3] = (byte)(responseData1.Length % 256);
                                responseData1[15] = (byte)(requetData.Length / 256);
                                responseData1[16] = (byte)(requetData.Length % 256);
                                responseData1[20] = requetData[18];
                                dataContent.CopyTo(responseData1, 21);
                                Buffer.BlockCopy(bytes, 0, responseData1, responseData1.Length - bytes.Length, bytes.Length);
                                newSocket.Send(responseData1);
                            }
                            break;
                        //写
                        case 5:
                            {
                                var valueByte = new byte[requetData.Length - 35];
                                Buffer.BlockCopy(requetData, 35, valueByte, 0, valueByte.Length);
                                //存储字节数据到内存
                                dataPersist.Write(address, JsonConvert.SerializeObject(valueByte));

                                byte[] responseData1 = new byte[22];
                                DataConvert.StringToByteArray("03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 FF").CopyTo(responseData1, 0);
                                //responseData1[15] = (byte)(responseData1.Length / 256);
                                //responseData1[16] = (byte)(responseData1.Length % 256);
                                newSocket.Send(responseData1);
                            }
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    //todo
                    //if (ex.SocketErrorCode != SocketError.ConnectionRefused &&
                    //    ex.SocketErrorCode != SocketError.ConnectionReset)
                    //    throw ex;
                }
            }
        }
    }
}

