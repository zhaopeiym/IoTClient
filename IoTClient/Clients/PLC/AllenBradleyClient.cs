using IoTClient.Clients.PLC.Models;
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IoTClient.Clients.PLC
{
    /// <summary>
    /// (AB)罗克韦尔客户端 Beta
    /// https://blog.csdn.net/lishiming0308/article/details/85243041
    /// </summary>
    public class AllenBradleyClient : SocketBase, IEthernetClient
    {
        public string Version => "AllenBradley";

        /// <summary>
        /// 连接地址
        /// </summary>
        public IPEndPoint IpEndPoint { get; }

        /// <summary>
        /// 是否是连接的
        /// </summary>
        public bool Connected => socket?.Connected ?? false;

        /// <summary>
        /// 超时时间
        /// </summary>
        private readonly int timeout;
        /// <summary>
        /// 插槽
        /// </summary>
        private readonly byte slot;

        public AllenBradleyClient(string ip, int port, byte slot = 0, int timeout = 1500)
        {
            if (!IPAddress.TryParse(ip, out IPAddress address))
                address = Dns.GetHostEntry(ip).AddressList?.FirstOrDefault();
            IpEndPoint = new IPEndPoint(address, port);
            this.timeout = timeout;
            this.slot = slot;
        }

        /// <summary>
        /// 会话句柄(由AB PLC生成)
        /// </summary>
        public uint Session { get; private set; }

        /// <summary>
        /// 注册命令
        /// </summary>
        private byte[] RegisteredCommand = new byte[28] {
            0x65,0x00,                              //注册请求
            0x04,0x00,                              //命令数据长度(单位字节)
            0x00,0x00,0x00,0x00,                    //会话句柄,初始值为0x00000000
            0x00,0x00,0x00,0x00,                    //状态，初始值为0x00000000（状态好）
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,//请求通信一方的说明
            0x00,0x00,0x00,0x00,                    //选项，默认为0x00000000
            0x01,0x00,                              //协议版本（0x0001）
            0x00,0x00                               //选项标记（0x0000
        };

        /// <summary>
        /// 打开连接（如果已经是连接状态会先关闭再打开）
        /// </summary>
        /// <returns></returns>
        protected override Result Connect()
        {
            var result = new Result();
            socket?.SafeClose();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //超时时间设置
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;

                //连接
                //socket.Connect(IpEndPoint);
                IAsyncResult connectResult = socket.BeginConnect(IpEndPoint, null, null);
                //阻塞当前线程           
                if (!connectResult.AsyncWaitHandle.WaitOne(timeout))
                    throw new TimeoutException("连接超时");
                socket.EndConnect(connectResult);

                result.Requst = string.Join(" ", RegisteredCommand.Select(t => t.ToString("X2")));
                socket.Send(RegisteredCommand);

                var socketReadResul = SocketRead(socket, 24);
                if (!socketReadResul.IsSucceed)
                    return socketReadResul;
                var head = socketReadResul.Value;

                socketReadResul = SocketRead(socket, GetContentLength(head));
                if (!socketReadResul.IsSucceed)
                    return socketReadResul;
                var content = socketReadResul.Value;

                var response = head.Concat(content).ToArray();
                result.Response = string.Join(" ", response.Select(t => t.ToString("X2")));

                byte[] buffer = new byte[4];
                buffer[0] = response[4];
                buffer[1] = response[5];
                buffer[2] = response[6];
                buffer[3] = response[7];
                //会话句柄
                Session = BitConverter.ToUInt32(buffer, 0);
            }
            catch (Exception ex)
            {
                socket?.SafeClose();
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.ErrCode = 408;
                result.Exception = ex;
            }
            return result.EndTime();
        }

        /// <summary>
        /// 发送报文，并获取响应报文（建议使用SendPackageReliable，如果异常会自动重试一次）
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override Result<byte[]> SendPackageSingle(byte[] command)
        {
            //从发送命令到读取响应为最小单元，避免多线程执行串数据（可线程安全执行）
            lock (this)
            {
                Result<byte[]> result = new Result<byte[]>();
                try
                {
                    socket.Send(command);
                    var socketReadResul = SocketRead(socket, 24);
                    if (!socketReadResul.IsSucceed)
                        return socketReadResul;
                    var head = socketReadResul.Value;

                    socketReadResul = SocketRead(socket, GetContentLength(head));
                    if (!socketReadResul.IsSucceed)
                        return socketReadResul;
                    var content = socketReadResul.Value;

                    result.Value = head.Concat(content).ToArray();
                    return result.EndTime();
                }
                catch (Exception ex)
                {
                    result.IsSucceed = false;
                    result.Err = ex.Message;
                    result.AddErr2List();
                    return result.EndTime();
                }
            }
        }

        #region Read
        public Result<byte[]> Read(string address, ushort length, bool isBit = false, bool setEndian = true)
        {
            if (!socket?.Connected ?? true)
            {
                var connectResult = Connect();
                if (!connectResult.IsSucceed)
                {
                    return new Result<byte[]>(connectResult);
                }
            }
            var result = new Result<byte[]>();
            try
            {
                var command = GetReadCommand(address, length);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //发送命令 并获取响应报文
                var sendResult = SendPackageReliable(command);
                if (!sendResult.IsSucceed)
                    return sendResult;
                var dataPackage = sendResult.Value;
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));

                ushort count = BitConverter.ToUInt16(dataPackage, 38);
                byte[] data = new byte[count - 6];
                Buffer.BlockCopy(dataPackage, 46, data, 0, data.Length);

                result.Value = data;
            }
            catch (SocketException ex)
            {
                result.IsSucceed = false;
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    result.Err = "连接超时";
                }
                else
                {
                    result.Err = ex.Message;
                    result.Exception = ex;
                }
                socket?.SafeClose();
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.Exception = ex;
                socket?.SafeClose();
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<bool> ReadBoolean(string address)
        {
            var readResut = Read(address, 1, isBit: true);
            var result = new Result<bool>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToBoolean(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取byte
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Result<byte> ReadByte(string address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Result<short> ReadInt16(string address)
        {
            var readResut = Read(address, 2);
            var result = new Result<short>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt16(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<ushort> ReadUInt16(string address)
        {
            var readResut = Read(address, 2);
            var result = new Result<ushort>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt16(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<int> ReadInt32(string address)
        {
            var readResut = Read(address, 4);
            var result = new Result<int>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt32(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<uint> ReadUInt32(string address)
        {
            var readResut = Read(address, 4);
            var result = new Result<uint>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt32(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<long> ReadInt64(string address)
        {
            var readResut = Read(address, 8);
            var result = new Result<long>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt64(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<ulong> ReadUInt64(string address)
        {
            var readResut = Read(address, 8);
            var result = new Result<ulong>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt64(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<float> ReadFloat(string address)
        {
            var readResut = Read(address, 4);
            var result = new Result<float>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToSingle(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<double> ReadDouble(string address)
        {
            var readResut = Read(address, 8);
            var result = new Result<double>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToDouble(readResut.Value, 0);
            return result.EndTime();
        }
        #endregion

        #region Write
        public Result Write(string address, ushort typeCode, byte[] data, bool isBit = false)
        {
            if (!socket?.Connected ?? true)
            {
                var connectResult = Connect();
                if (!connectResult.IsSucceed)
                {
                    return connectResult;
                }
            }
            Result result = new Result();
            try
            {
                //Array.Reverse(data);
                //发送写入信息
                //var arg = ConvertWriteArg(address, data, false);
                byte[] command = GetWriteCommand(address, typeCode, data, 1);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                var sendResult = SendPackageReliable(command);
                if (!sendResult.IsSucceed)
                    return sendResult;

                var dataPackage = sendResult.Value;
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));
            }
            catch (SocketException ex)
            {
                result.IsSucceed = false;
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    result.Err = "连接超时";
                }
                else
                {
                    result.Err = ex.Message;
                    result.Exception = ex;
                }
                socket?.SafeClose();
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.Exception = ex;
                socket?.SafeClose();
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

        public Result Write(string address, bool value)
        {
            return Write(address, 0xC1, value ? new byte[] { 0xFF, 0xFF } : new byte[] { 0x00, 0x00 });
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, byte value)
        {
            return Write(address, 0xC2, new byte[] { value, 0x00 });// new byte[1] { value }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, sbyte value)
        {
            return Write(address, 0xC2, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, short value)
        {
            return Write(address, 0xC3, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, ushort value)
        {
            return Write(address, 0xC3, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, int value)
        {
            return Write(address, 0xC4, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, uint value)
        {
            return Write(address, 0xC4, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, long value)
        {
            return Write(address, 0xC5, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, ulong value)
        {
            return Write(address, 0xC5, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, float value)
        {
            return Write(address, 0xCA, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, double value)
        {
            return Write(address, 0xCB, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, string value)
        {
            var valueBytes = Encoding.ASCII.GetBytes(value);
            var bytes = new byte[valueBytes.Length + 1];
            bytes[0] = (byte)valueBytes.Length;
            valueBytes.CopyTo(bytes, 1);
            Array.Reverse(bytes);
            return Write(address, 0xC4, bytes);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public Result Write(string address, object value, DataTypeEnum type)
        {
            var result = new Result() { IsSucceed = false };
            switch (type)
            {
                case DataTypeEnum.Bool:
                    result = Write(address, Convert.ToBoolean(value));
                    break;
                case DataTypeEnum.Byte:
                    result = Write(address, Convert.ToByte(value));
                    break;
                case DataTypeEnum.Int16:
                    result = Write(address, Convert.ToInt16(value));
                    break;
                case DataTypeEnum.UInt16:
                    result = Write(address, Convert.ToUInt16(value));
                    break;
                case DataTypeEnum.Int32:
                    result = Write(address, Convert.ToInt32(value));
                    break;
                case DataTypeEnum.UInt32:
                    result = Write(address, Convert.ToUInt32(value));
                    break;
                case DataTypeEnum.Int64:
                    result = Write(address, Convert.ToInt64(value));
                    break;
                case DataTypeEnum.UInt64:
                    result = Write(address, Convert.ToUInt64(value));
                    break;
                case DataTypeEnum.Float:
                    result = Write(address, Convert.ToSingle(value));
                    break;
                case DataTypeEnum.Double:
                    result = Write(address, Convert.ToDouble(value));
                    break;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 地址信息解析
        /// </summary>
        /// <param name="address"></param>        
        /// <param name="isBit"></param> 
        /// <returns></returns>
        private AllenBradleyAddress ConvertArg(string address, bool isBit)
        {
            return new AllenBradleyAddress();
        }

        /// <summary>
        /// 获取Read命令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="slot"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected byte[] GetReadCommand(string address, ushort length)
        {
            //if (!isBit)
            //length = (ushort)(length / 2);

            var address_ASCII = Encoding.ASCII.GetBytes(address);
            if (address_ASCII.Length % 2 == 1)
            {
                address_ASCII = new byte[address_ASCII.Length + 1];
                Encoding.ASCII.GetBytes(address).CopyTo(address_ASCII, 0);
            }

            byte[] command = new byte[9 + 26 + address_ASCII.Length + 1 + 24];

            command[0] = 0x6F;//命令
            command[2] = BitConverter.GetBytes((ushort)(command.Length - 24))[0];
            command[3] = BitConverter.GetBytes((ushort)(command.Length - 24))[1];//长度
            command[4] = BitConverter.GetBytes(Session)[0];
            command[5] = BitConverter.GetBytes(Session)[1];
            command[6] = BitConverter.GetBytes(Session)[2];
            command[7] = BitConverter.GetBytes(Session)[3];//会话句柄

            command[0 + 24] = 0x00;
            command[1 + 24] = 0x00;
            command[2 + 24] = 0x00;
            command[3 + 24] = 0x00;//接口句柄，默认为0x00000000（CIP）
            command[4 + 24] = 0x01;
            command[5 + 24] = 0x00;//超时（0x0001）
            command[6 + 24] = 0x02;
            command[7 + 24] = 0x00;//项数（0x0002）
            command[8 + 24] = 0x00;
            command[9 + 24] = 0x00;//空地址项（0x0000）
            command[10 + 24] = 0x00;
            command[11 + 24] = 0x00;//长度（0x0000）
            command[12 + 24] = 0xB2;
            command[13 + 24] = 0x00;//未连接数据项（0x00b2）
            command[14 + 24] = BitConverter.GetBytes((short)(command.Length - 16 - 24))[0]; // 后面数据包的长度，等全部生成后在赋值
            command[15 + 24] = BitConverter.GetBytes((short)(command.Length - 16 - 24))[1];
            command[16 + 24] = 0x52;//服务类型（0x03请求服务列表，0x52请求标签数据）
            command[17 + 24] = 0x02;//请求路径大小
            command[18 + 24] = 0x20;
            command[19 + 24] = 0x06;//请求路径(0x0620)
            command[20 + 24] = 0x24;
            command[21 + 24] = 0x01;//请求路径(0x0124)
            command[22 + 24] = 0x0A;
            command[23 + 24] = 0xF0;
            command[24 + 24] = BitConverter.GetBytes((short)(6 + address_ASCII.Length))[0];     // CIP指令长度
            command[25 + 24] = BitConverter.GetBytes((short)(6 + address_ASCII.Length))[1];

            command[0 + 24 + 26] = 0x4C;//读取数据
            command[1 + 24 + 26] = (byte)((address_ASCII.Length + 2) / 2);
            command[2 + 24 + 26] = 0x91;
            command[3 + 24 + 26] = (byte)address.Length;
            address_ASCII.CopyTo(command, 4 + 24 + 26);
            command[4 + 24 + 26 + address_ASCII.Length] = BitConverter.GetBytes(length)[0];
            command[5 + 24 + 26 + address_ASCII.Length] = BitConverter.GetBytes(length)[1];

            command[6 + 24 + 26 + address_ASCII.Length] = 0x01;
            command[7 + 24 + 26 + address_ASCII.Length] = 0x00;
            command[8 + 24 + 26 + address_ASCII.Length] = 0x01;
            command[9 + 24 + 26 + address_ASCII.Length] = slot;

            return command;
        }

        /// <summary>
        /// 获取Write命令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="typeCode"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected byte[] GetWriteCommand(string address, ushort typeCode, byte[] value, int length)
        {
            var address_ASCII = Encoding.ASCII.GetBytes(address);
            if (address_ASCII.Length % 2 == 1)
            {
                address_ASCII = new byte[address_ASCII.Length + 1];
                Encoding.ASCII.GetBytes(address).CopyTo(address_ASCII, 0);
            }
            byte[] command = new byte[8 + 26 + address_ASCII.Length + value.Length + 4 + 24];

            command[0] = 0x6F;//命令
            command[2] = BitConverter.GetBytes((ushort)(command.Length - 24))[0];
            command[3] = BitConverter.GetBytes((ushort)(command.Length - 24))[1];//长度
            command[4] = BitConverter.GetBytes(Session)[0];
            command[5] = BitConverter.GetBytes(Session)[1];
            command[6] = BitConverter.GetBytes(Session)[2];
            command[7] = BitConverter.GetBytes(Session)[3];//会话句柄

            command[0 + 24] = 0x00;
            command[1 + 24] = 0x00;
            command[2 + 24] = 0x00;
            command[3 + 24] = 0x00;//接口句柄，默认为0x00000000（CIP）
            command[4 + 24] = 0x01;
            command[5 + 24] = 0x00;//超时（0x0001）
            command[6 + 24] = 0x02;
            command[7 + 24] = 0x00;//项数（0x0002）
            command[8 + 24] = 0x00;
            command[9 + 24] = 0x00;
            command[10 + 24] = 0x00;
            command[11 + 24] = 0x00;//空地址项（0x0000）
            command[12 + 24] = 0xB2;
            command[13 + 24] = 0x00;//未连接数据项（0x00b2）
            command[14 + 24] = BitConverter.GetBytes((short)(command.Length - 16 - 24))[0]; // 后面数据包的长度，等全部生成后在赋值
            command[15 + 24] = BitConverter.GetBytes((short)(command.Length - 16 - 24))[1];
            command[16 + 24] = 0x52;//服务类型（0x03请求服务列表，0x52请求标签数据）
            command[17 + 24] = 0x02;//请求路径大小
            command[18 + 24] = 0x20;
            command[19 + 24] = 0x06;//请求路径(0x0620)
            command[20 + 24] = 0x24;
            command[21 + 24] = 0x01;//请求路径(0x0124)
            command[22 + 24] = 0x0A;
            command[23 + 24] = 0xF0;
            command[24 + 24] = BitConverter.GetBytes((short)(8 + value.Length + address_ASCII.Length))[0];     // CIP指令长度
            command[25 + 24] = BitConverter.GetBytes((short)(8 + value.Length + address_ASCII.Length))[1];

            command[0 + 26 + 24] = 0x4D;//写数据
            command[1 + 26 + 24] = (byte)((address_ASCII.Length + 2) / 2);
            command[2 + 26 + 24] = 0x91;
            command[3 + 26 + 24] = (byte)address.Length;
            address_ASCII.CopyTo(command, 4 + 26 + 24);
            command[4 + 26 + 24 + address_ASCII.Length] = BitConverter.GetBytes(typeCode)[0];
            command[5 + 26 + 24 + address_ASCII.Length] = BitConverter.GetBytes(typeCode)[1];
            command[6 + 26 + 24 + address_ASCII.Length] = BitConverter.GetBytes(length)[0];//TODO length ??
            command[7 + 26 + 24 + address_ASCII.Length] = BitConverter.GetBytes(length)[1];
            value.CopyTo(command, 8 + 26 + 24 + address_ASCII.Length);

            command[8 + 26 + 24 + address_ASCII.Length + value.Length] = 0x01;
            command[9 + 26 + 24 + address_ASCII.Length + value.Length] = 0x00;
            command[10 + 26 + 24 + address_ASCII.Length + value.Length] = 0x01;
            command[11 + 26 + 24 + address_ASCII.Length + value.Length] = slot;
            return command;

        }

        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            throw new System.NotImplementedException();
        }

        public Result BatchWrite(Dictionary<string, object> addresses, int batchNumber)
        {
            throw new System.NotImplementedException();
        }

        public Result<string> ReadString(string address)
        {
            throw new NotImplementedException();
        }

        public Result Write(string address, byte[] data, bool isBit = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 后面内容长度
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        private ushort GetContentLength(byte[] head)
        {
            return BitConverter.ToUInt16(head, 2);
        }
    }
}
