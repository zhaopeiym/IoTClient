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
                    byte[] requetData2 = new byte[requetData1[2] * 256 + requetData1[3] - 4];
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
                                //数据块个数
                                var dbNumber = requetData[18];
                                //读取数据总长度
                                var readLength = 0;

                                for (int i = 0; i < dbNumber; i++)
                                {
                                    //访问数据的个数，以byte为单位
                                    var byteLength = requetData[23 + i * 12] * 256 + requetData[24 + i * 12];
                                    if (byteLength == 1 && i < dbNumber - 1)//非最后一个bit/byte，需要补全。
                                        byteLength += 1;
                                    readLength += byteLength;
                                }

                                var dataContent = new byte[4 * dbNumber + readLength];//数据报文总长度
                                var cursor = 0;
                                for (int i = 0; i < dbNumber; i++)
                                {
                                    //DB块的偏移量
                                    var beginAddress = requetData[28 + i * 12] * 256 * 256 + requetData[29 + i * 12] * 256 + requetData[30 + i * 12];
                                    //访问数据的个数
                                    var byteLength = requetData[23 + i * 12] * 256 + requetData[24 + i * 12];
                                    if (byteLength == 1 && i < dbNumber - 1)//非最后一个bit/byte，需要补全。
                                        byteLength += 1;
                                    //是否是bit类型
                                    var isBit = requetData[22 + i * 12] == 0x01;
                                    //访问数据块的类型（V、I、DB...）
                                    var dbType = requetData[27 + i * 12];
                                    var dataKey = $"s200-{dbType}";
                                    var dataValue = dataPersist.Read(dataKey);
                                    var byteArray = JsonConvert.DeserializeObject<byte[]>(dataValue) ?? new byte[65536];

                                    //DataConvert.StringToByteArray("FF 09 00 04").CopyTo(dataContent, cursor);
                                    dataContent[0 + cursor] = 0xFF;
                                    //dataContent[1 + cursor] = readLength == 1 ? (byte)0x03 : (byte)0x04;//04 byte(字节) 03bit（位）
                                    dataContent[1 + cursor] = isBit ? (byte)0x03 : (byte)0x04;//04 byte(字节) 03bit（位）
                                    dataContent[2 + cursor] = (byte)(byteLength / 256);//后半截数据数的Length
                                    dataContent[3 + cursor] = (byte)(byteLength % 256);//后半截数据数的Length
                                    if (isBit)//按bit
                                    {
                                        var bitOffset = beginAddress % 8;
                                        byte bitOffsetValue = (byte)Math.Pow(2, bitOffset);
                                        var oldBitValue = byteArray[beginAddress / 8];
                                        //转成bit的形式所需的返回值
                                        var bitValue = (oldBitValue & bitOffsetValue) != 0 ? 0x01 : 0x00;
                                        //[4 + cursor]返回给客户端的数据
                                        new byte[] { (byte)bitValue }.CopyTo(dataContent, 4 + cursor);
                                    }
                                    else
                                        //[4 + cursor]返回给客户端的数据
                                        Buffer.BlockCopy(byteArray, beginAddress / 8, dataContent, 4 + cursor, byteLength);

                                    cursor += 4 + byteLength;
                                }

                                byte[] responseData = new byte[21 + dataContent.Length];
                                DataConvert.StringToByteArray("03 00 00 1A 02 F0 80 32 03 00 00 00 01 00 02 00 00 00 00 04 01").CopyTo(responseData, 0);
                                responseData[8] = 0x03;//1  客户端发送命令 3 服务器回复命令 
                                responseData[2] = (byte)(responseData.Length / 256);//返回数据长度
                                responseData[3] = (byte)(responseData.Length % 256);
                                responseData[15] = (byte)(requetData.Length / 256);//读取数据长度
                                responseData[16] = (byte)(requetData.Length % 256);
                                responseData[20] = requetData[18];//读取数据块个数
                                dataContent.CopyTo(responseData, 21);
                                newSocket.Send(responseData);
                            }
                            break;
                        //写
                        case 5:
                            {
                                var writesLength = requetData[18];//写如数据的个数 Item count
                                int cursor = 0;
                                for (int i = 0; i < writesLength; i++)
                                {
                                    //DB块的偏移量（存储数据的地址）
                                    var beginAddress = requetData[28 + i * 12] * 256 * 256 + requetData[29 + i * 12] * 256 + requetData[30 + i * 12];
                                    //访问数据块的类型（V、I、DB...）
                                    var dbType = requetData[27 + i * 12];
                                    var dataKey = $"s200-{dbType}";

                                    //Data之前的下标
                                    var dataBeforeIndex = 18 + writesLength * 12;
                                    //[2 + index]是否是bit类型
                                    var isBit = requetData[dataBeforeIndex + 4 * (i + 1) - 2 + cursor] == 0x03;
                                    var coefficient = isBit ? 1 : 8;
                                    //初始化要写入的字节数组长度
                                    var writeValue = new byte[requetData[dataBeforeIndex + 4 * (i + 1) + cursor] / coefficient];
                                    //开始写入的地址（报文中的数据的位置）
                                    var requetBeginLocation = dataBeforeIndex + 4 * (i + 1) + cursor + 1;

                                    //非最后一个bit，需要补全。
                                    if (writeValue.Length == 1 && i < writesLength - 1)
                                        cursor++;
                                    cursor += writeValue.Length;
                                    //数据赋值到writeValue中
                                    Buffer.BlockCopy(requetData, requetBeginLocation, writeValue, 0, writeValue.Length);

                                    var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(dataKey)) ?? new byte[65536];
                                    if (isBit)
                                    {
                                        var oldBitValue = byteArray[beginAddress / 8];
                                        var bitOffset = beginAddress % 8;
                                        byte bitOffsetValue = (byte)Math.Pow(2, bitOffset);
                                        if (writeValue[0] == 0x01)//true
                                            oldBitValue = (byte)(oldBitValue | bitOffsetValue);//组合bitOffsetValue
                                        else//false
                                            oldBitValue = (byte)(oldBitValue & ~bitOffsetValue);//去掉bitOffsetValue
                                        new byte[] { oldBitValue }.CopyTo(byteArray, beginAddress / 8);
                                    }
                                    else
                                        writeValue.CopyTo(byteArray, beginAddress / 8);

                                    //存储字节数据到内存
                                    dataPersist.Write(dataKey, JsonConvert.SerializeObject(byteArray));
                                }

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

