using IoTClient.Clients.PLC.Models;
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IoTClient.Clients.PLC
{
    /// <summary>
    /// 欧姆龙PLC 客户端
    /// https://flat2010.github.io/2020/02/23/Omron-Fins%E5%8D%8F%E8%AE%AE/
    /// </summary>
    public class OmronFinsClient : SocketBase, IEthernetClient
    {
        private EndianFormat endianFormat;
        private int timeout;

        /// <summary>
        /// 基础命令
        /// </summary>
        private byte[] BasicCommand = new byte[]
        {
            0x46, 0x49, 0x4E, 0x53,//Magic字段  0x46494E53 对应的ASCII码，即FINS
            0x00, 0x00, 0x00, 0x0C,//Length字段 表示其后所有字段的总长度
            0x00, 0x00, 0x00, 0x00,//Command字段 
            0x00, 0x00, 0x00, 0x00,//Error Code字段
            0x00, 0x00, 0x00, 0x0B //Client/Server Node Address字段
        };

        /// <summary>
        /// 版本
        /// </summary>
        public string Version => "OmronFins";

        /// <summary>
        /// 连接地址
        /// </summary>
        public IPEndPoint IpEndPoint { get; private set; }

        /// <summary>
        /// 是否是连接的
        /// </summary>
        public bool Connected => socket?.Connected ?? false;

        /// <summary>
        /// DA2(即Destination unit address，目标单元地址)
        /// 0x00：PC(CPU)
        /// 0xFE： SYSMAC NET Link Unit or SYSMAC LINK Unit connected to network；
        /// 0x10~0x1F：CPU总线单元 ，其值等于10 + 单元号(前端面板中配置的单元号)
        /// </summary>
        public byte UnitAddress { get; set; } = 0x00;

        /// <summary>
        /// SA1 客户端节点编号
        /// </summary>
        public byte SA1 { get; set; } = 0x0B;

        /// <summary>
        /// DA1 服务器节点编号
        /// </summary>
        private byte DA1 { get; set; } = 0x01;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <param name="endianFormat"></param>
        public OmronFinsClient(string ip, int port = 9600, int timeout = 1500, EndianFormat endianFormat = EndianFormat.CDAB)
        {
            if (!IPAddress.TryParse(ip, out IPAddress address))
                address = Dns.GetHostEntry(ip).AddressList?.FirstOrDefault();
            IpEndPoint = new IPEndPoint(address, port);
            this.timeout = timeout;
            this.endianFormat = endianFormat;
        }

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

                //socket.Connect(IpEndPoint);
                IAsyncResult connectResult = socket.BeginConnect(IpEndPoint, null, null);
                //阻塞当前线程           
                if (!connectResult.AsyncWaitHandle.WaitOne(timeout))
                    throw new TimeoutException("连接超时");
                socket.EndConnect(connectResult);

                BasicCommand[19] = SA1;
                result.Requst = string.Join(" ", BasicCommand.Select(t => t.ToString("X2")));
                socket.Send(BasicCommand);

                var socketReadResul = SocketRead(socket, 8);
                if (!socketReadResul.IsSucceed)
                    return socketReadResul;
                var head = socketReadResul.Value;

                byte[] buffer = new byte[4];
                buffer[0] = head[7];
                buffer[1] = head[6];
                buffer[2] = head[5];
                buffer[3] = head[4];
                var length = BitConverter.ToInt32(buffer, 0);

                socketReadResul = SocketRead(socket, length);
                if (!socketReadResul.IsSucceed)
                    return socketReadResul;
                var content = socketReadResul.Value;

                var headContent = head.Concat(content).ToArray();
                result.Response = string.Join(" ", headContent.Select(t => t.ToString("X2")));
                // 服务器节点编号
                if (headContent.Length >= 24) DA1 = headContent[23];
                else DA1 = Convert.ToByte(IpEndPoint.Address.ToString().Substring(IpEndPoint.Address.ToString().LastIndexOf(".") + 1)); ;
            }
            catch (Exception ex)
            {
                socket?.SafeClose();
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.ErrCode = 408;
                result.Exception = ex;
            }
            return result.EndTime(); ;
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
                    var socketReadResul = SocketRead(socket, 8);
                    if (!socketReadResul.IsSucceed)
                        return socketReadResul;
                    var head = socketReadResul.Value;

                    byte[] buffer = new byte[4];
                    buffer[0] = head[7];
                    buffer[1] = head[6];
                    buffer[2] = head[5];
                    buffer[3] = head[4];
                    //4-7是Length字段 表示其后所有字段的总长度
                    var contentLength = BitConverter.ToInt32(buffer, 0);
                    socketReadResul = SocketRead(socket, contentLength);
                    if (!socketReadResul.IsSucceed)
                        return socketReadResul;
                    var dataPackage = socketReadResul.Value;

                    result.Value = head.Concat(dataPackage).ToArray();
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
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length"></param>
        /// <param name="isBit"></param>
        /// <param name="setEndian">返回值是否设置大小端</param>
        /// <returns></returns>
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
                //发送读取信息
                var arg = ConvertArg(address, isBit: isBit);
                byte[] command = GetReadCommand(arg, length);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //发送命令 并获取响应报文
                var sendResult = SendPackageReliable(command);
                if (!sendResult.IsSucceed)
                    return sendResult;
                var dataPackage = sendResult.Value;

                byte[] responseData = new byte[length];
                Array.Copy(dataPackage, dataPackage.Length - length, responseData, 0, length);
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));
                if (setEndian)
                    result.Value = responseData.ToArray().ByteFormatting2(endianFormat, false);
                else
                    result.Value = responseData.ToArray();
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

        private Result<bool> ReadBoolean(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = addressInt - startAddressInt;
                var byteArry = values.Skip(interval * 1).Take(1).ToArray();
                return new Result<bool>
                {
                    Value = BitConverter.ToBoolean(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        private Result<short> ReadInt16(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = addressInt - startAddressInt;
                var byteArry = values.Skip(interval * 2).Take(2).Reverse().ToArray();
                return new Result<short>
                {
                    Value = BitConverter.ToInt16(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<short>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        private Result<ushort> ReadUInt16(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = addressInt - startAddressInt;
                var byteArry = values.Skip(interval * 2).Take(2).Reverse().ToArray();
                return new Result<ushort>
                {
                    Value = BitConverter.ToUInt16(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<ushort>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        private Result<int> ReadInt32(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 2;
                var offset = (addressInt - startAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).ToArray().ByteFormatting2(endianFormat, false);
                return new Result<int>
                {
                    Value = BitConverter.ToInt32(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<int>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        private Result<uint> ReadUInt32(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 2;
                var offset = (addressInt - startAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).ToArray().ByteFormatting2(endianFormat, false);
                return new Result<uint>
                {
                    Value = BitConverter.ToUInt32(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<uint>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        private Result<long> ReadInt64(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 4;
                var offset = (addressInt - startAddressInt) % 4 * 2;
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).ToArray().ByteFormatting2(endianFormat, false);
                return new Result<long>
                {
                    Value = BitConverter.ToInt64(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<long>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        private Result<ulong> ReadUInt64(int startAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - startAddressInt) / 4;
                var offset = (addressInt - startAddressInt) % 4 * 2;
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).ToArray().ByteFormatting2(endianFormat, false);
                return new Result<ulong>
                {
                    Value = BitConverter.ToUInt64(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<ulong>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        public Result<float> ReadFloat(int beginAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).ToArray().ByteFormatting2(endianFormat, false);
                return new Result<float>
                {
                    Value = BitConverter.ToSingle(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<float>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
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

        public Result<double> ReadDouble(int beginAddressInt, int addressInt, byte[] values)
        {
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).ToArray().ByteFormatting2(endianFormat, false);
                return new Result<double>
                {
                    Value = BitConverter.ToDouble(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<double>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }
        #endregion

        #region Write

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="data">值</param>
        /// <param name="isBit">值</param>
        /// <returns></returns>
        public Result Write(string address, byte[] data, bool isBit = false)
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
                data = data.Reverse().ToArray().ByteFormatting2(endianFormat);
                //发送写入信息
                var arg = ConvertArg(address, isBit: isBit);
                byte[] command = GetWriteCommand(arg, data);
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

        public Result Write(string address, byte[] data)
        {
            return Write(address, data, false);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, bool value)
        {
            return Write(address, value ? new byte[] { 0x01 } : new byte[] { 0x00 }, true);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, byte value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, sbyte value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, short value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, ushort value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, int value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, uint value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, long value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, ulong value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, float value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, double value)
        {
            return Write(address, BitConverter.GetBytes(value));
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
        /// <param name="dataType"></param> 
        /// <param name="isBit"></param> 
        /// <returns></returns>
        private OmronFinsAddress ConvertArg(string address, DataTypeEnum dataType = DataTypeEnum.None, bool isBit = false)
        {
            address = address.ToUpper();
            var addressInfo = new OmronFinsAddress()
            {
                DataTypeEnum = dataType,
                IsBit = isBit
            };
            switch (address[0])
            {
                case 'D'://DM区
                    {
                        addressInfo.BitCode = 0x02;
                        addressInfo.WordCode = 0x82;
                        addressInfo.TypeChar = address.Substring(0, 1);
                        addressInfo.BeginAddress = Convert.ToInt32(address.Substring(1).Split('.')[0]);
                        break;
                    }
                case 'C'://CIO区
                    {
                        addressInfo.BitCode = 0x30;
                        addressInfo.WordCode = 0xB0;
                        addressInfo.TypeChar = address.Substring(0, 1);
                        addressInfo.BeginAddress = Convert.ToInt32(address.Substring(1).Split('.')[0]);
                        break;
                    }
                case 'W'://WR区
                    {
                        addressInfo.BitCode = 0x31;
                        addressInfo.WordCode = 0xB1;
                        addressInfo.TypeChar = address.Substring(0, 1);
                        addressInfo.BeginAddress = Convert.ToInt32(address.Substring(1).Split('.')[0]);
                        break;
                    }
                case 'H'://HR区
                    {
                        addressInfo.BitCode = 0x32;
                        addressInfo.WordCode = 0xB2;
                        addressInfo.TypeChar = address.Substring(0, 1);
                        addressInfo.BeginAddress = Convert.ToInt32(address.Substring(1).Split('.')[0]);
                        break;
                    }
                case 'A'://AR区
                    {
                        addressInfo.BitCode = 0x33;
                        addressInfo.WordCode = 0xB3;
                        addressInfo.TypeChar = address.Substring(0, 1);
                        addressInfo.BeginAddress = Convert.ToInt32(address.Substring(1).Split('.')[0]);
                        break;
                    }
                case 'E':
                    {
                        string[] address_split = address.Split('.');
                        int block_length = Convert.ToInt32(address_split[0].Substring(1), 16);
                        if (block_length < 16)
                        {
                            addressInfo.BitCode = (byte)(0x20 + block_length);
                            addressInfo.WordCode = (byte)(0xA0 + block_length);
                        }
                        else
                        {
                            addressInfo.BitCode = (byte)(0xE0 + block_length - 16);
                            addressInfo.WordCode = (byte)(0x60 + block_length - 16);
                        }

                        if (isBit)
                        {
                            // 位操作
                            ushort address_location = ushort.Parse(address_split[1]);
                            addressInfo.BitAddress = new byte[3];
                            addressInfo.BitAddress[0] = BitConverter.GetBytes(address_location)[1];
                            addressInfo.BitAddress[1] = BitConverter.GetBytes(address_location)[0];

                            if (address_split.Length > 2)
                            {
                                addressInfo.BitAddress[2] = byte.Parse(address_split[2]);
                                if (addressInfo.BitAddress[2] > 15)
                                    //输入的位地址只能在0-15之间
                                    throw new Exception("位地址数据异常");
                            }
                        }
                        else
                        {
                            // 字操作
                            ushort address_location = ushort.Parse(address_split[1]);
                            addressInfo.BitAddress = new byte[3];
                            addressInfo.BitAddress[0] = BitConverter.GetBytes(address_location)[1];
                            addressInfo.BitAddress[1] = BitConverter.GetBytes(address_location)[0];
                        }
                        break;
                    }
                default:
                    //类型不支持
                    throw new Exception("Address解析异常");
            }

            if (address[0] != 'E')
            {
                if (isBit)
                {
                    // 位操作
                    string[] address_split = address.Substring(1).Split('.');
                    ushort address_location = ushort.Parse(address_split[0]);
                    addressInfo.BitAddress = new byte[3];
                    addressInfo.BitAddress[0] = BitConverter.GetBytes(address_location)[1];
                    addressInfo.BitAddress[1] = BitConverter.GetBytes(address_location)[0];

                    if (address_split.Length > 1)
                    {
                        addressInfo.BitAddress[2] = byte.Parse(address_split[1]);
                        if (addressInfo.BitAddress[2] > 15)
                            //输入的位地址只能在0-15之间
                            throw new Exception("位地址数据异常");
                    }
                }
                else
                {
                    // 字操作
                    ushort address_location = ushort.Parse(address.Substring(1));
                    addressInfo.BitAddress = new byte[3];
                    addressInfo.BitAddress[0] = BitConverter.GetBytes(address_location)[1];
                    addressInfo.BitAddress[1] = BitConverter.GetBytes(address_location)[0];
                }
            }

            return addressInfo;
        }

        /// <summary>
        /// 获取Read命令
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected byte[] GetReadCommand(OmronFinsAddress arg, ushort length)
        {
            bool isBit = arg.IsBit;

            if (!isBit) length = (ushort)(length / 2);

            byte[] command = new byte[26 + 8];

            Array.Copy(BasicCommand, 0, command, 0, 4);
            byte[] tmp = BitConverter.GetBytes(command.Length - 8);
            Array.Reverse(tmp);
            tmp.CopyTo(command, 4);
            command[11] = 0x02;

            command[16] = 0x80; //ICF 信息控制字段
            command[17] = 0x00; //RSV 保留字段
            command[18] = 0x02; //GCT 网关计数
            command[19] = 0x00; //DNA 目标网络地址 00:表示本地网络  0x01~0x7F:表示远程网络
            command[20] = DA1; //DA1 目标节点编号 0x01~0x3E:SYSMAC LINK网络中的节点号 0x01~0x7E:YSMAC NET网络中的节点号 0xFF:广播传输
            command[21] = UnitAddress; //DA2 目标单元地址
            command[22] = 0x00; //SNA 源网络地址 取值及含义同DNA字段
            command[23] = SA1; //SA1 源节点编号 取值及含义同DA1字段
            command[24] = 0x00; //SA2 源单元地址 取值及含义同DA2字段
            command[25] = 0x00; //SID Service ID 取值0x00~0xFF，产生会话的进程的唯一标识

            command[26] = 0x01;
            command[27] = 0x01; //Command Code 内存区域读取
            command[28] = isBit ? arg.BitCode : arg.WordCode;
            arg.BitAddress.CopyTo(command, 29);
            command[32] = (byte)(length / 256);
            command[33] = (byte)(length % 256);

            return command;
        }

        /// <summary>
        /// 获取Write命令
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected byte[] GetWriteCommand(OmronFinsAddress arg, byte[] value)
        {
            bool isBit = arg.IsBit;
            byte[] command = new byte[26 + 8 + value.Length];

            Array.Copy(BasicCommand, 0, command, 0, 4);
            byte[] tmp = BitConverter.GetBytes(command.Length - 8);
            Array.Reverse(tmp);
            tmp.CopyTo(command, 4);
            command[11] = 0x02;

            command[16] = 0x80; //ICF 信息控制字段
            command[17] = 0x00; //RSV 保留字段
            command[18] = 0x02; //GCT 网关计数
            command[19] = 0x00; //DNA 目标网络地址 00:表示本地网络  0x01~0x7F:表示远程网络
            command[20] = DA1; //DA1 目标节点编号 0x01~0x3E:SYSMAC LINK网络中的节点号 0x01~0x7E:YSMAC NET网络中的节点号 0xFF:广播传输
            command[21] = UnitAddress; //DA2 目标单元地址
            command[22] = 0x00; //SNA 源网络地址 取值及含义同DNA字段
            command[23] = SA1; //SA1 源节点编号 取值及含义同DA1字段
            command[24] = 0x00; //SA2 源单元地址 取值及含义同DA2字段
            command[25] = 0x00; //SID Service ID 取值0x00~0xFF，产生会话的进程的唯一标识

            command[26] = 0x01;
            command[27] = 0x02; //Command Code 内存区域写入
            command[28] = isBit ? arg.BitCode : arg.WordCode;
            arg.BitAddress.CopyTo(command, 29);
            command[32] = isBit ? (byte)(value.Length / 256) : (byte)(value.Length / 2 / 256);
            command[33] = isBit ? (byte)(value.Length % 256) : (byte)(value.Length / 2 % 256);
            value.CopyTo(command, 34);

            return command;
        }

        /// <summary>
        /// 批量读取
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="batchNumber">此参数设置无实际效果</param>
        /// <returns></returns>
        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            var result = new Result<Dictionary<string, object>>();
            result.Value = new Dictionary<string, object>();

            var omronFinsAddresses = addresses.Select(t => ConvertArg(t.Key, t.Value)).ToList();
            var typeChars = omronFinsAddresses.Select(t => t.TypeChar).Distinct();
            foreach (var typeChar in typeChars)
            {
                var tempAddresses = omronFinsAddresses.Where(t => t.TypeChar == typeChar).ToList();
                var minAddress = tempAddresses.Select(t => t.BeginAddress).Min();
                var maxAddress = tempAddresses.Select(t => t.BeginAddress).Max();

                while (maxAddress >= minAddress)
                {
                    int readLength = 121;//TODO 分批读取的长度还可以继续调大

                    var tempAddress = tempAddresses.Where(t => t.BeginAddress >= minAddress && t.BeginAddress <= minAddress + readLength).ToList();
                    //如果范围内没有数据。按正确逻辑不存在这种情况。
                    if (!tempAddress.Any())
                    {
                        minAddress = minAddress + readLength;
                        continue;
                    }

                    var tempMax = tempAddress.OrderByDescending(t => t.BeginAddress).FirstOrDefault();
                    switch (tempMax.DataTypeEnum)
                    {
                        case DataTypeEnum.Bool:
                            throw new Exception("暂时不支持Bool类型批量读取");
                        case DataTypeEnum.Byte:
                            throw new Exception("暂时不支持Byte类型批量读取");
                        //readLength = tempMax.BeginAddress + 1 - minAddress;
                        //break;
                        case DataTypeEnum.Int16:
                        case DataTypeEnum.UInt16:
                            readLength = tempMax.BeginAddress * 2 + 2 - minAddress * 2;
                            break;
                        case DataTypeEnum.Int32:
                        case DataTypeEnum.UInt32:
                        case DataTypeEnum.Float:
                            readLength = tempMax.BeginAddress * 2 + 4 - minAddress * 2;
                            break;
                        case DataTypeEnum.Int64:
                        case DataTypeEnum.UInt64:
                        case DataTypeEnum.Double:
                            readLength = tempMax.BeginAddress * 2 + 8 - minAddress * 2;
                            break;
                        default:
                            throw new Exception("Err BatchRead 未定义类型 -1");
                    }

                    var tempResult = Read(typeChar + minAddress.ToString(), Convert.ToUInt16(readLength), false, setEndian: false);

                    if (!tempResult.IsSucceed)
                    {
                        result.IsSucceed = tempResult.IsSucceed;
                        result.Exception = tempResult.Exception;
                        result.Err = tempResult.Err;
                        result.ErrCode = tempResult.ErrCode;
                        return result.EndTime();
                    }

                    var rValue = tempResult.Value.ToArray();
                    foreach (var item in tempAddress)
                    {
                        object tempVaue = null;

                        switch (item.DataTypeEnum)
                        {
                            case DataTypeEnum.Bool:
                                tempVaue = ReadBoolean(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.Byte:
                                throw new Exception("Err BatchRead 未定义类型 -2");
                            case DataTypeEnum.Int16:
                                tempVaue = ReadInt16(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.UInt16:
                                tempVaue = ReadUInt16(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.Int32:
                                tempVaue = ReadInt32(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.UInt32:
                                tempVaue = ReadUInt32(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.Int64:
                                tempVaue = ReadInt64(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.UInt64:
                                tempVaue = ReadUInt64(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.Float:
                                tempVaue = ReadFloat(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            case DataTypeEnum.Double:
                                tempVaue = ReadDouble(minAddress, item.BeginAddress, rValue).Value;
                                break;
                            default:
                                throw new Exception("Err BatchRead 未定义类型 -3");
                        }

                        result.Value.Add(item.TypeChar + item.BeginAddress.ToString(), tempVaue);
                    }
                    minAddress = minAddress + readLength / 2;

                    if (tempAddresses.Any(t => t.BeginAddress >= minAddress))
                        minAddress = tempAddresses.Where(t => t.BeginAddress >= minAddress).OrderBy(t => t.BeginAddress).FirstOrDefault().BeginAddress;
                }
            }
            return result.EndTime(); ;
        }

        public Result<string> ReadString(string address)
        {
            throw new NotImplementedException();
        }

        public Result BatchWrite(Dictionary<string, object> addresses, int batchNumber)
        {
            throw new NotImplementedException();
        }

        public Result Write(string address, string value)
        {
            throw new NotImplementedException();
        }
    }
}
