using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IoTServer.Servers.ModBus
{
    public class ModBusTcpServer
    {
        private Socket socketServer;
        private string ip;
        private int port;
        DataPersist dataPersist;
        List<Socket> sockets = new List<Socket>();
        public ModBusTcpServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("ModBusTcpServer");
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
            if (socketServer?.Connected ?? false)
                socketServer.Shutdown(SocketShutdown.Both);
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
                    int readLeng = newSocket.Receive(requetData1, 0, requetData1.Length, SocketFlags.None);
                    if (readLeng == 0)//客户端断开连接
                    {
                        if (newSocket.Connected) newSocket.Shutdown(SocketShutdown.Both);
                        newSocket.Close();
                        return;
                    }
                    byte[] requetData2 = new byte[requetData1[5] - 2];
                    readLeng = newSocket.Receive(requetData2, 0, requetData2.Length, SocketFlags.None);
                    if (readLeng == 0)//客户端断开连接
                    {
                        if (newSocket.Connected) newSocket.Shutdown(SocketShutdown.Both);
                        newSocket.Close();
                        return;
                    }
                    var requetData = requetData1.Concat(requetData2).ToArray();

                    byte[] responseData1 = new byte[8];
                    //复制请求报文中的报文头
                    Buffer.BlockCopy(requetData, 0, responseData1, 0, responseData1.Length);
                    var address = requetData[8] * 256 + requetData[9];
                    switch (requetData[7])
                    {
                        //读取线圈
                        case 1:
                            {
                                var value = dataPersist.Read(address);// 数据存在 8、9                            
                                var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                                //当前位置到最后的长度
                                responseData1[5] = (byte)(2 + bytes.Length);
                                //TODO
                                byte[] responseData2 = new byte[2] { bytes[1], bytes[0] };
                                var responseData = responseData1.Concat(responseData2).ToArray();
                                newSocket.Send(responseData);
                            }
                            break;
                        //写入线圈
                        case 5:
                            {
                                var value = new byte[2];
                                Buffer.BlockCopy(requetData, 10, value, 0, value.Length);
                                dataPersist.Write(address, JsonConvert.SerializeObject(value));
                                var responseData = new byte[10];
                                Buffer.BlockCopy(requetData, 0, responseData, 0, responseData.Length);
                                responseData[5] = 4;//后面的长度
                                newSocket.Send(responseData);
                            }
                            break;
                        //读取
                        case 3:
                            {
                                var value = dataPersist.Read(address);// 数据存在 8、9                            
                                var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                                if (bytes == null)
                                {
                                    var length = requetData[requetData.Length - 2] * 256 + requetData[requetData.Length - 1];
                                    bytes = new byte[length * 2];
                                }
                                //当前位置到最后的长度
                                responseData1[5] = (byte)(3 + bytes.Length);
                                byte[] responseData2 = new byte[1 + bytes.Length];
                                responseData2[0] = (byte)bytes.Length;
                                bytes.CopyTo(responseData2, 1);
                                var responseData = responseData1.Concat(responseData2).ToArray();
                                newSocket.Send(responseData);
                            }
                            break;
                        //写入
                        case 16:
                            {
                                var value = new byte[requetData[12]];
                                Buffer.BlockCopy(requetData, 13, value, 0, value.Length);
                                dataPersist.Write(address, JsonConvert.SerializeObject(value));
                                var responseData = new byte[12];
                                Buffer.BlockCopy(requetData, 0, responseData, 0, responseData.Length);
                                responseData[5] = 6;//后面的长度
                                newSocket.Send(responseData);
                            }
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    if (newSocket?.Connected ?? false) newSocket?.Shutdown(SocketShutdown.Both);
                    newSocket?.Close();
                }
                catch (Exception ex)
                {
                    if (newSocket?.Connected ?? false) newSocket?.Shutdown(SocketShutdown.Both);
                    newSocket?.Close();
                }
            }
        }
    }
}
