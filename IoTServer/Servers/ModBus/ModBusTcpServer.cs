using IoTServer.Common;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IoTServer.Servers.ModBus
{
    public class ModBusTcpServer
    {
        private Socket socketServer;
        private string redisConfig;
        private string ip;
        private int port;
        public ModBusTcpServer(string ip, int port, string redisConfig = null)
        {
            this.redisConfig = redisConfig;
            this.ip = ip;
            this.port = port;
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
        /// 客户端连接到服务端
        /// </summary>
        /// <param name="socket"></param>
        void Accept(Socket socket)
        {
            while (true)
            {
                try
                {
                    //阻塞等待客户端连接
                    Socket newSocket = socket.Accept();
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
                        newSocket.Shutdown(SocketShutdown.Both);
                        newSocket.Close();
                        return;
                    }
                    byte[] requetData2 = new byte[requetData1[5] - 2];
                    readLeng = newSocket.Receive(requetData2, 0, requetData2.Length, SocketFlags.None);
                    if (readLeng == 0)//客户端断开连接
                    {
                        newSocket.Shutdown(SocketShutdown.Both);
                        newSocket.Close();
                        return;
                    }
                    var requetData = requetData1.Concat(requetData2).ToArray();

                    byte[] responseData1 = new byte[8];
                    //复制请求报文中的报文头
                    Buffer.BlockCopy(requetData, 0, responseData1, 0, responseData1.Length);
                    DataPersist data = new DataPersist(redisConfig);

                    switch (requetData[7])
                    {
                        //读取线圈
                        case 1:
                            //TODO
                            break;
                        //写入线圈
                        case 5:
                            //TODO
                            break;
                        //读取
                        case 3:
                            {
                                var value = data.Read(requetData[9]);//TODO 数据存在 8、9
                                short.TryParse(value, out short resultValue);
                                var bytes = BitConverter.GetBytes(resultValue);
                                //当前位置到最后的长度
                                responseData1[5] = (byte)(3 + bytes.Length);
                                //TODO
                                byte[] responseData2 = new byte[3] { (byte)bytes.Length, bytes[1], bytes[0] };
                                var responseData = responseData1.Concat(responseData2).ToArray();
                                newSocket.Send(responseData);
                            }
                            break;
                        //写入
                        case 16:
                            {
                                //TODO
                                var value = requetData[requetData.Length - 2] * 256 + requetData[requetData.Length - 1];
                                data.Write(requetData[9], value.ToString());
                                var responseData = new byte[12];
                                Buffer.BlockCopy(requetData, 0, responseData, 0, responseData.Length);
                                responseData[5] = 6;
                                newSocket.Send(responseData);
                            }
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    //todo
                    if (ex.SocketErrorCode != SocketError.ConnectionRefused)
                        throw ex;
                }
            }
        }
    }
}
