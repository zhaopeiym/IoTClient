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
    /// 三菱MC Qna-3E  服务端模拟
    /// </summary>
    public class MitsubishiQna3EServer : ServerSocketBase, IIoTServer
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
        public MitsubishiQna3EServer(int port, string ip = null)
        {
            this.ip = ip;
            this.port = port;
            dataPersist = new DataPersist("MitsubishiQna3EServer");
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
                    byte[] requetData1 = new byte[21];
                    //读取客户端发送过来的数据                   
                    requetData1 = SocketRead(newSocket, requetData1.Length);
                    byte[] requetData = null;
                    //[12] 0x14 写 0x04 读
                    if (requetData1[12] == 0x14)
                    {
                        var lenght = requetData1[20] * 2 * 256 + requetData1[19] * 2;
                        //如果是按Bit存储
                        if (requetData1[13] == 0x01) lenght = 1;
                        byte[] requetData2 = new byte[lenght];
                        requetData2 = SocketRead(newSocket, requetData2.Length);
                        requetData = requetData1.Concat(requetData2).ToArray();
                    }
                    else
                        requetData = requetData1;
                    //地址
                    //var address = $"{requetData[18]}-{requetData[17] * 256 * 256 + requetData[16] * 256 + requetData[15]}";

                    var dataKey = $"{requetData[18]}";
                    var beginAddress = requetData[17] * 256 * 256 + requetData[16] * 256 + requetData[15];
                    //[13]0x01 位   0x00 字节
                    var isBit = requetData1[13] == 0x01;
                    var addressLenght = isBit ? 0.5 : 2;

                    switch (requetData[12])
                    {
                        //读
                        case 0x04:
                            {
                                //数据存储长度
                                var lenght = requetData[19] + requetData[20] * 256;
                                //var value = dataPersist.Read(address);
                                //把存储的数据转为字节数组                              
                                var dataValue = dataPersist.Read(dataKey);
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataValue) ?? new byte[65536];

                                //数据的字节长度
                                var dataLenght = isBit ? (int)Math.Ceiling(lenght * addressLenght) : lenght * 2;

                                // 响应报文总长度
                                byte[] responseData = new byte[11 + dataLenght];

                                DataConvert.StringToByteArray("D0 00 00 FF FF 03 00 06 00 00 00").CopyTo(responseData, 0);
                                //responseData1[7][8]存储的是后面还有多少长度
                                responseData[7] = BitConverter.GetBytes(2 + dataLenght)[0];
                                responseData[8] = BitConverter.GetBytes(2 + dataLenght)[1];
                                if (isBit && lenght == 1)
                                {
                                    var oldBitValue = byteArray[beginAddress / 2];
                                    //一个字节存储两个点位数据
                                    var bitOffset = beginAddress % 2;
                                    if (bitOffset == 0)
                                        responseData[11] = (oldBitValue & 0b00010000) != 0 ? (byte)0b00010000 : (byte)0b00000000;
                                    else
                                        responseData[11] = (oldBitValue & 0b00000001) != 0 ? (byte)0b00010000 : (byte)0b00000000;
                                }
                                else
                                    Buffer.BlockCopy(byteArray, (int)Math.Floor(beginAddress * addressLenght), responseData, 11, dataLenght);

                                newSocket.Send(responseData);
                            }
                            break;
                        //写
                        case 0x14:
                            {
                                var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(dataKey)) ?? new byte[65536];
                                //[21]后面存的是数据
                                var valueByte = new byte[requetData.Length - 21];
                                Buffer.BlockCopy(requetData, 21, valueByte, 0, valueByte.Length);
                                //存储字节数据到内存
                                //dataPersist.Write(address, JsonConvert.SerializeObject(valueByte));

                                if (isBit)
                                {
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
                                    new byte[] { oldBitValue }.CopyTo(byteArray, (int)Math.Floor(beginAddress * addressLenght));
                                }
                                else
                                    valueByte.CopyTo(byteArray, (int)Math.Floor(beginAddress * addressLenght));
                                dataPersist.Write(dataKey, JsonConvert.SerializeObject(byteArray));

                                byte[] responseData1 = DataConvert.StringToByteArray("D0 00 00 FF FF 03 00 02 00 00 00");
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
