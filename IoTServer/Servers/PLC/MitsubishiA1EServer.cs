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
    /// 三菱MC A-1E 服务端模拟
    /// </summary>
    public class MitsubishiA1EServer : ServerSocketBase, IIoTServer
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
        public MitsubishiA1EServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("MitsubishiA1EServer");
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
                    byte[] temp_requetData1 = new byte[12];
                    //读取客户端发送过来的数据                   
                    temp_requetData1 = SocketRead(newSocket, temp_requetData1.Length);
                    byte[] requetData = null;
                    //0x03写字 - 0x00 读位 0x01 读字  0x02写位 0x03写字
                    if (temp_requetData1[0] == 0x03)
                    {
                        var lenght = temp_requetData1[11] * 2 * 256 + temp_requetData1[10] * 2;
                        byte[] temp_requetData2 = new byte[lenght];
                        temp_requetData2 = SocketRead(newSocket, temp_requetData2.Length);
                        requetData = temp_requetData1.Concat(temp_requetData2).ToArray();
                    }
                    //0x02写位
                    else if (temp_requetData1[0] == 0x02)
                    {
                        var lenght = temp_requetData1[11] * 256 + temp_requetData1[10];
                        byte[] temp_requetData2 = new byte[lenght];
                        temp_requetData2 = SocketRead(newSocket, temp_requetData2.Length);
                        requetData = temp_requetData1.Concat(temp_requetData2).ToArray();
                    }
                    else
                        requetData = temp_requetData1;
                    //地址
                    var address = $"{requetData[8]}{requetData[9]}-{ requetData[5] * 256 + requetData[4]}";
                    var dataKey = $"{requetData[8]}{requetData[9]}";
                    var beginAddress = requetData[5] * 256 + requetData[4];
                    var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(dataKey)) ?? new byte[65536];

                    switch (requetData[0])
                    {
                        //读位 0x01 位  0x00 字节                      
                        case 0x00:
                            {
                                //数据存储长度
                                var lenght = requetData[10] + requetData[11] * 256;
                                var value = dataPersist.Read(address);
                                //把存储的数据转为字节数组
                                var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                                if (string.IsNullOrWhiteSpace(value))
                                    bytes = BitConverter.GetBytes(0);
                                //数据的字节长度
                                var dataLenght = (int)Math.Ceiling(lenght * 0.5);
                                // 响应报文总长度
                                byte[] responseData = new byte[2 + dataLenght];
                                DataConvert.StringToByteArray("80 00").CopyTo(responseData, 0);

                                if (lenght == 1)
                                {
                                    var oldBitValue = byteArray[beginAddress / 2];
                                    //一个字节存储两个点位数据
                                    var bitOffset = beginAddress % 2;
                                    if (bitOffset == 0)
                                        responseData[2] = (oldBitValue & 0b00010000) != 0 ? (byte)0b00010000 : (byte)0b00000000;
                                    else
                                        responseData[2] = (oldBitValue & 0b00000001) != 0 ? (byte)0b00010000 : (byte)0b00000000;
                                }
                                else
                                    Buffer.BlockCopy(byteArray, beginAddress / 2, responseData, 2, dataLenght);
                                newSocket.Send(responseData);
                            }
                            break;
                        //读字
                        case 0x01:
                            {
                                var addressLenght = 2;

                                //数据存储长度
                                var lenght = requetData[10] + requetData[11] * 256;
                                var value = dataPersist.Read(address);
                                //数据的字节长度
                                var dataLenght = lenght * 2;
                                // 响应报文总长度
                                byte[] responseData = new byte[2 + dataLenght];
                                DataConvert.StringToByteArray("81 00").CopyTo(responseData, 0);
                                Buffer.BlockCopy(byteArray, beginAddress * addressLenght, responseData, 2, dataLenght);
                                newSocket.Send(responseData);
                            }
                            break;
                        //写位 0x02写位 0x03写字
                        case 0x02:
                            {
                                ////[12]后面存的是数据
                                var valueByte = new byte[requetData.Length - 12];
                                Buffer.BlockCopy(requetData, 12, valueByte, 0, valueByte.Length);
                                ////一个字节存储两个点位数据
                                //valueByte.CopyTo(byteArray, beginAddress / 2);

                                var oldBitValue = byteArray[beginAddress / 2];
                                var bitOffset = beginAddress % 2;//一个字节存储两个点位数据
                                if (valueByte[0] == 0x10)//true
                                {
                                    if (bitOffset == 0)
                                        oldBitValue = (byte)(oldBitValue | 0b00010000);
                                    else
                                        oldBitValue = (byte)(oldBitValue | 0b00000001);
                                }
                                else//false
                                {
                                    if (bitOffset == 0)
                                        oldBitValue = (byte)(oldBitValue & ~0b11111110);//修改成0b00000001或0b00000000
                                    else
                                        oldBitValue = (byte)(oldBitValue & ~0b11101111);//修改成0b00010000或0b00000000
                                }
                                new byte[] { oldBitValue }.CopyTo(byteArray, beginAddress / 2);

                                //存储字节数据到内存
                                dataPersist.Write(dataKey, JsonConvert.SerializeObject(byteArray));
                                byte[] responseData1 = DataConvert.StringToByteArray("82 00");
                                newSocket.Send(responseData1);
                            }
                            break;
                        //写字
                        case 0x03:
                            {
                                var addressLenght = 2;

                                //[12]后面存的是数据
                                var valueByte = new byte[requetData.Length - 12];
                                Buffer.BlockCopy(requetData, 12, valueByte, 0, valueByte.Length);
                                valueByte.CopyTo(byteArray, beginAddress * addressLenght);

                                //存储字节数据到内存
                                dataPersist.Write(dataKey, JsonConvert.SerializeObject(byteArray));

                                byte[] responseData1 = DataConvert.StringToByteArray("83 00");
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
