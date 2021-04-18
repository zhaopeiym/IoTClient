using IoTClient.Clients.PLC.Models;
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Interfaces;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace IoTClient.Clients.PLC
{
    /// <summary>
    /// 欧姆龙PLC 客户端 - Beta
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
            0x46, 0x49, 0x4E, 0x53, 0x00, 0x00, 0x00, 0x0C,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x01
        };

        /// <summary>
        /// 版本
        /// </summary>
        public string Version => "OmronFins";

        /// <summary>
        /// 连接地址
        /// </summary>
        public IPEndPoint IpAndPoint { get; private set; }

        /// <summary>
        /// 是否是连接的
        /// </summary>
        public bool Connected => socket?.Connected ?? false;

        /// <summary>
        /// 
        /// </summary>
        public LoggerDelegate WarningLog { get; set; }

        /// <summary>
        /// 单元号地址
        /// </summary>
        public byte UnitNumber { get; set; } = 0x00;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <param name="endianFormat"></param>
        public OmronFinsClient(string ip, int port, int timeout = 1500, EndianFormat endianFormat = EndianFormat.CDAB)
        {
            IpAndPoint = new IPEndPoint(IPAddress.Parse(ip), port); ;
            this.timeout = timeout;
            this.endianFormat = endianFormat;
        }

        /// <summary>
        /// 打开长连接
        /// </summary>
        /// <returns></returns>
        protected override Result Connect()
        {
            var result = new Result();
            socket?.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                #region 超时时间设置
#if !DEBUG
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;
#endif
                #endregion

                socket.Connect(IpAndPoint);

                socket.Send(BasicCommand);

                var head = SocketRead(socket, 8);

                byte[] buffer = new byte[4];
                buffer[0] = head[7];
                buffer[1] = head[6];
                buffer[2] = head[5];
                buffer[3] = head[4];
                var length = BitConverter.ToInt32(buffer, 0);

                var content = SocketRead(socket, length);

                var requst = string.Join(" ", head.Concat(content).Select(t => t.ToString("X2")));
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            return result.EndTime(); ;
        }

        #region Read
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length"></param>
        /// <param name="isBit"></param>
        /// <param name="setEndian"></param>
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
                var arg = ConvertArg(address, isBit);
                byte[] command = GetReadCommand(arg, length, isBit);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //发送命令 并获取响应报文
                var sendResult = SendPackage(command);
                if (!sendResult.IsSucceed)
                    return sendResult;
                var dataPackage = sendResult.Value;

                byte[] responseData = new byte[length];
                Array.Copy(dataPackage, dataPackage.Length - length, responseData, 0, length);
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));
                if (setEndian)
                    result.Value = responseData.Reverse().ToArray().ByteFormatting(endianFormat);
                else
                    result.Value = responseData.Reverse().ToArray();
            }
            catch (SocketException ex)
            {
                result.IsSucceed = false;
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    result.Err = "连接超时";
                    result.ErrList.Add("连接超时");
                }
                else
                {
                    result.Err = ex.Message;
                    result.Exception = ex;
                    result.ErrList.Add(ex.Message);
                }
                socket?.SafeClose();
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.Exception = ex;
                result.ErrList.Add(ex.Message);
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
                data = data.Reverse().ToArray().ByteFormatting(endianFormat);
                //发送写入信息
                var arg = ConvertArg(address, isBit);
                byte[] command = GetWriteCommand(arg, data, isBit);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                var sendResult = SendPackage(command);
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
                    result.ErrList.Add("连接超时");
                }
                else
                {
                    result.Err = ex.Message;
                    result.Exception = ex;
                    result.ErrList.Add(ex.Message);
                }
                socket?.SafeClose();
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.Exception = ex;
                result.ErrList.Add(ex.Message);
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

        #endregion

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

        private OmronFinsData ConvertArg(string address, bool isBit)
        {
            address = address.ToUpper();
            var data = new OmronFinsData();
            switch (address[0])
            {
                case 'D':
                    {
                        data.OmronFinsType = OmronFinsType.DM;
                        break;
                    }
                case 'C':
                    {
                        data.OmronFinsType = OmronFinsType.CIO;
                        break;
                    }
                case 'W':
                    {
                        data.OmronFinsType = OmronFinsType.WR;
                        break;
                    }
                case 'H':
                    {
                        data.OmronFinsType = OmronFinsType.HR;
                        break;
                    }
                case 'A':
                    {
                        data.OmronFinsType = OmronFinsType.AR;
                        break;
                    }
                case 'E':
                    {
                        //TODO
                        break;
                    }
                default: throw new Exception("Address解析异常");
            }

            if (address[0] == 'E')
            {
                //TODO
            }
            else
            {
                if (isBit)
                {
                    // 位操作
                    string[] splits = address.Substring(1).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    ushort addr = ushort.Parse(splits[0]);
                    data.Content = new byte[3];
                    data.Content[0] = BitConverter.GetBytes(addr)[1];
                    data.Content[1] = BitConverter.GetBytes(addr)[0];

                    if (splits.Length > 1)
                    {
                        data.Content[2] = byte.Parse(splits[1]);
                        if (data.Content[2] > 15)
                        {
                            //输入的位地址只能在0-15之间
                            throw new Exception("位地址数据异常");
                        }
                    }
                }
                else
                {
                    // 字操作
                    ushort addr = ushort.Parse(address.Substring(1));
                    data.Content = new byte[3];
                    data.Content[0] = BitConverter.GetBytes(addr)[1];
                    data.Content[1] = BitConverter.GetBytes(addr)[0];
                }
            }

            return data;
        }

        protected byte[] GetReadCommand(OmronFinsData arg, ushort length, bool isBit)
        {
            if (!isBit) length = (ushort)(length / 2);

            byte[] command = new byte[26 + 8];

            Array.Copy(BasicCommand, 0, command, 0, 4);
            byte[] tmp = BitConverter.GetBytes(command.Length - 8);
            Array.Reverse(tmp);
            tmp.CopyTo(command, 4);
            command[11] = 0x02;

            command[16] = 0x80; //信息控制字段
            command[17] = 0x00;
            command[18] = 0x02;
            command[19] = 0x00;
            command[20] = 0x13; //0x13; //01(节点地址:Ip地址的最后一位)
            command[21] = UnitNumber; //单元号地址(传过来)
            command[22] = 0x00;
            command[23] = 0x0B; //0x0B; //01(节点地址:Ip地址的最后一位)
            command[24] = 0x00;
            command[25] = 0x00;

            command[26] = 0x01;
            command[27] = 0x01; //读
            command[28] = isBit ? arg.OmronFinsType.BitCode : arg.OmronFinsType.WordCode;
            arg.Content.CopyTo(command, 29);
            command[32] = (byte)(length / 256);
            command[33] = (byte)(length % 256);

            return command;
        }

        protected byte[] GetWriteCommand(OmronFinsData arg, byte[] value, bool isBit)
        {
            byte[] command = new byte[26 + 8 + value.Length];

            Array.Copy(BasicCommand, 0, command, 0, 4);
            byte[] tmp = BitConverter.GetBytes(command.Length - 8);
            Array.Reverse(tmp);
            tmp.CopyTo(command, 4);
            command[11] = 0x02;

            command[16] = 0x80; //信息控制字段
            command[17] = 0x00;
            command[18] = 0x02;
            command[19] = 0x00;
            command[20] = 0x13; //0x13; //01(节点地址:Ip地址的最后一位)
            command[21] = UnitNumber; //单元号地址(传过来)
            command[22] = 0x00;
            command[23] = 0x0B; //0x0B; //01(节点地址:Ip地址的最后一位)
            command[24] = 0x00;
            command[25] = 0x00;

            command[26] = 0x01;
            command[27] = 0x02; //写
            command[28] = isBit ? arg.OmronFinsType.BitCode : arg.OmronFinsType.WordCode;
            arg.Content.CopyTo(command, 29);
            command[32] = isBit ? (byte)(value.Length / 256) : (byte)(value.Length / 2 / 256);
            command[33] = isBit ? (byte)(value.Length % 256) : (byte)(value.Length / 2 % 256);
            value.CopyTo(command, 34);

            return command;
        }

        #region 发送报文，并获取响应报文
        public Result<byte[]> SendPackage(byte[] command)
        {
            //从发送命令到读取响应为最小单元，避免多线程执行串数据（可线程安全执行）
            lock (this)
            {
                Result<byte[]> result = new Result<byte[]>();

                void _sendPackage()
                {
                    socket.Send(command);
                    var head = SocketRead(socket, 8);
                    byte[] buffer = new byte[4];
                    buffer[0] = head[7];
                    buffer[1] = head[6];
                    buffer[2] = head[5];
                    buffer[3] = head[4];
                    var length = BitConverter.ToInt32(buffer, 0);
                    var dataPackage = SocketRead(socket, length);
                    result.Value = head.Concat(dataPackage).ToArray();
                }

                try
                {
                    _sendPackage();
                }
                catch (Exception ex)
                {
                    WarningLog?.Invoke(ex.Message, ex);
                    //如果出现异常，则进行一次重试
                    //重新打开连接
                    var conentResult = Connect();
                    if (!conentResult.IsSucceed)
                        return new Result<byte[]>(conentResult);

                    _sendPackage();
                }
                return result.EndTime();
            }
        }
        #endregion


        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            throw new NotImplementedException();
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
