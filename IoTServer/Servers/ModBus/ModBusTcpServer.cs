using IoTClient.Common.Helpers;
using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IoTServer.Servers.Modbus
{
    /// <summary>
    /// ModbusTcp 服务端模拟
    /// </summary>
    public class ModbusTcpServer : ServerSocketBase
    {
        private Socket socketServer;
        private string ip;
        private int port;
        DataPersist dataPersist;
        List<Socket> sockets = new List<Socket>();
        public ModbusTcpServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("ModbusTcpServer");
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

            //3、开启侦听(等待客户机发出的连接),并设置最大客户k端连接数为10
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
                    }
                    catch (SocketException ex)
                    {
                        foreach (var item in sockets)
                        {
                            if (item.Connected) item.Shutdown(SocketShutdown.Both);
                            item.Close();
                        }
                    }
                    Task.Run(() => { Receive(newSocket); });
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
                    byte[] requetData1 = new byte[8];
                    //读取客户端发送过来的数据                  
                    requetData1 = SocketRead(newSocket, requetData1.Length);
                    byte[] requetData2 = new byte[requetData1[5] - 2];
                    requetData2 = SocketRead(newSocket, requetData2.Length);
                    var requetData = requetData1.Concat(requetData2).ToArray();
                    byte[] responseData1 = new byte[8];
                    //复制请求报文中的报文头
                    Buffer.BlockCopy(requetData, 0, responseData1, 0, responseData1.Length);
                    var addressKey = $"{requetData[6]}-{requetData[8] * 256 + requetData[9]}";//站号-寄存器地址
                    var stationNumberKey = $"{requetData[6]}-key";//站号
                    var address = requetData[8] * 256 + requetData[9];//寄存器地址
                    var registerLenght = requetData[10] * 256 + requetData[11];//寄存器的长度
                    switch (requetData[7])
                    {
                        //读取线圈
                        case 1:
                            {
                                var value = dataPersist.Read(stationNumberKey + "-Coil");// 数据存在 8、9                            
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(value) ?? new byte[65536];

                                var blenght = (byte)Math.Ceiling(registerLenght / 8f);
                                //当前位置到最后的长度
                                responseData1[5] = (byte)(3 + blenght);
                                byte[] responseData2 = new byte[1 + blenght];

                                var tempData = new byte[registerLenght];
                                Buffer.BlockCopy(byteArray, address, tempData, 0, tempData.Length);
                                responseData2[0] = blenght;//显示数据所占长度
                                for (int i = 0; i < blenght; i++)
                                {
                                    responseData2[1 + i] = (byte)DataConvert.BinaryArrayToInt(string.Join("", tempData.Skip(i * 8).Take(8).Reverse()));
                                }
                                var responseData = responseData1.Concat(responseData2).ToArray();
                                newSocket.Send(responseData);
                            }
                            break;
                        //写入线圈
                        case 5:
                            {
                                var value = new byte[2];
                                Buffer.BlockCopy(requetData, 10, value, 0, value.Length);
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(stationNumberKey + "-Coil")) ?? new byte[65536];
                                if (value[0] == 0 && value[1] == 0)
                                    byteArray[address] = 0;
                                else
                                    byteArray[address] = 1;
                                dataPersist.Write(stationNumberKey + "-Coil", JsonConvert.SerializeObject(byteArray));
                                var responseData = new byte[10];
                                Buffer.BlockCopy(requetData, 0, responseData, 0, responseData.Length);
                                responseData[5] = 4;//后面的长度
                                newSocket.Send(responseData);
                            }
                            break;
                        //读取
                        case 3:
                            {
                                var value = dataPersist.Read(stationNumberKey);// 数据存在 8、9                            
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(value) ?? new byte[65536];
                                //当前位置到最后的长度
                                responseData1[4] = (byte)((3 + registerLenght * 2) / 256);
                                responseData1[5] = (byte)((3 + registerLenght * 2) % 256);
                                byte[] responseData2 = new byte[1 + registerLenght * 2];
                                responseData2[0] = (byte)(registerLenght * 2);
                                Buffer.BlockCopy(byteArray, address * 2, responseData2, 1, registerLenght * 2);
                                var responseData = responseData1.Concat(responseData2).ToArray();
                                newSocket.Send(responseData);
                            }
                            break;
                        //写入
                        case 16:
                            {
                                var value = new byte[requetData[12]];
                                Buffer.BlockCopy(requetData, 13, value, 0, value.Length);
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(stationNumberKey)) ?? new byte[65536];
                                value.CopyTo(byteArray, address * 2);
                                dataPersist.Write(stationNumberKey, JsonConvert.SerializeObject(byteArray));
                                var responseData = new byte[12];
                                Buffer.BlockCopy(requetData, 0, responseData, 0, responseData.Length);
                                responseData[5] = 6;//后面的长度
                                newSocket.Send(responseData);
                            }
                            break;
                        //读离散
                        case 2:
                            {
                                //只返回false
                                var bytes = new byte[2];
                                //当前位置到最后的长度
                                responseData1[5] = (byte)(2 + bytes.Length);
                                byte[] responseData2 = new byte[2] { bytes[1], bytes[0] };
                                var responseData = responseData1.Concat(responseData2).ToArray();
                                newSocket.Send(responseData);
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
