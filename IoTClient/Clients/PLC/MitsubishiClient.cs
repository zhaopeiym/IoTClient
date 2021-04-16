using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Interfaces;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IoTClient.Clients.PLC
{
    /// <summary>
    /// 三菱plc客户端 - Beta
    /// </summary>
    public class MitsubishiClient : SocketBase, IEthernetClient
    {
        private int timeout;

        public string Version => version.ToString();
        private MitsubishiVersion version;

        public IPEndPoint IpAndPoint { get; }

        public bool Connected => socket?.Connected ?? false;

        public LoggerDelegate WarningLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public MitsubishiClient(MitsubishiVersion version, string ip, int port, int timeout = 1500)
        {
            this.version = version;
            IpAndPoint = new IPEndPoint(IPAddress.Parse(ip), port); ;
            this.timeout = timeout;
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
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                return result;
            }
            return result;
        }

        #region 发送报文，并获取响应报文
        /// <summary>
        /// 发送报文，并获取响应报文
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public byte[] SendPackage(byte[] command)
        {
            socket.Send(command);
            var headPackage = SocketRead(socket, 9);
            var dataPackage = SocketRead(socket, GetContentLength(headPackage));
            return headPackage.Concat(dataPackage).ToArray();
        }

        public byte[] SendPackage(byte[] command, int receiveCount)
        {
            socket.Send(command);
            var dataPackage = SocketRead(socket, receiveCount);
            return dataPackage.ToArray();
        }
        #endregion

        #region 读
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
            if (!socket?.Connected ?? true) Connect();
            var result = new Result<byte[]>();
            try
            {
                //发送读取信息
                MitsubishiMCData arg = null;
                byte[] command = null;

                switch (version)
                {
                    case MitsubishiVersion.A_1E:
                        arg = ConvertAddress_A_1E(address);
                        command = GetReadCommand_A_1E(arg.BeginAddress, arg.MitsubishiMCType.TypeCode, length, isBit);
                        break;
                    case MitsubishiVersion.Qna_3E:
                        arg = ConvertAddress_Qna_3E(address);
                        command = GetReadCommand_Qna_3E(arg.BeginAddress, arg.MitsubishiMCType.TypeCode, length, isBit);
                        break;
                }
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                byte[] dataPackage = null;
                switch (version)
                {
                    case MitsubishiVersion.A_1E:
                        var lenght = command[10] + command[11] * 256;
                        if (isBit)
                            dataPackage = SendPackage(command, (int)Math.Ceiling(lenght * 0.5) + 2);
                        else
                            dataPackage = SendPackage(command, lenght * 2 + 2);
                        break;
                    case MitsubishiVersion.Qna_3E:
                        dataPackage = SendPackage(command);
                        break;
                }
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));

                var bufferLength = length;
                byte[] responseValue = null;

                switch (version)
                {
                    case MitsubishiVersion.A_1E:
                        responseValue = new byte[dataPackage.Length - 2];
                        Array.Copy(dataPackage, 2, responseValue, 0, responseValue.Length);
                        break;
                    case MitsubishiVersion.Qna_3E:

                        if (isBit)
                        {
                            bufferLength = (ushort)Math.Ceiling(bufferLength * 0.5);
                        }
                        responseValue = new byte[bufferLength];
                        Array.Copy(dataPackage, dataPackage.Length - bufferLength, responseValue, 0, bufferLength);
                        break;
                }

                result.Value = responseValue;
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
                    result.ErrList.Add(ex.Message);
                }
                socket?.SafeClose();
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
        }

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<bool> ReadBoolean(string address)
        {
            var readResut = Read(address, 1, isBit: true);
            var result = new Result<bool>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = (readResut.Value[0] & 0b00010000) != 0;
            return result;
        }

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address"></param>
        /// <param name="readNumber"></param>
        /// <returns></returns>
        public Result<List<KeyValuePair<string, bool>>> ReadBoolean(string address, ushort readNumber)
        {
            var length = 1;
            var readResut = Read(address, Convert.ToUInt16(length * readNumber), isBit: true);
            var result = new Result<List<KeyValuePair<string, bool>>>(readResut);
            var dbAddress = decimal.Parse(address.Substring(1));
            var dbType = address.Substring(0, 1);
            if (result.IsSucceed)
            {
                var values = new List<KeyValuePair<string, bool>>();
                for (ushort i = 0; i < readNumber; i++)
                {
                    var index = i / 2;
                    var isoffset = i % 2 == 0;
                    bool value;
                    if (isoffset)
                        value = (readResut.Value[index] & 0b00010000) != 0;
                    else
                        value = (readResut.Value[index] & 0b00000001) != 0;
                    values.Add(new KeyValuePair<string, bool>($"{dbType}{dbAddress + i * length }", value));
                }
                result.Value = values;
            }
            return result.EndTime();
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<short> ReadInt16(string address)
        {
            var readResut = Read(address, 2);
            var result = new Result<short>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt16(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address"></param>
        /// <param name="readNumber"></param>
        /// <returns></returns>
        public Result<List<KeyValuePair<string, short>>> ReadInt16(string address, ushort readNumber)
        {
            var length = 2;
            var readResut = Read(address, Convert.ToUInt16(length * readNumber));
            var dbAddress = int.Parse(address.Substring(1));
            var dbType = address.Substring(0, 1);
            var result = new Result<List<KeyValuePair<string, short>>>(readResut);
            if (result.IsSucceed)
            {
                var values = new List<KeyValuePair<string, short>>();
                for (int i = 0; i < readNumber; i++)
                {
                    values.Add(new KeyValuePair<string, short>($"{dbType}{dbAddress + i * length}", BitConverter.ToInt16(readResut.Value, (readNumber - 1 - i) * length)));
                }
                result.Value = values;
            }
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
            var result = new Result<ushort>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt16(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<int> ReadInt32(string address)
        {
            var readResut = Read(address, 4);
            var result = new Result<int>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt32(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<uint> ReadUInt32(string address)
        {
            var readResut = Read(address, 4);
            var result = new Result<uint>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt32(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<long> ReadInt64(string address)
        {
            var readResut = Read(address, 8);
            var result = new Result<long>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt64(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<ulong> ReadUInt64(string address)
        {
            var readResut = Read(address, 8);
            var result = new Result<ulong>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt64(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<float> ReadFloat(string address)
        {
            var readResut = Read(address, 4);
            var result = new Result<float>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToSingle(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<double> ReadDouble(string address)
        {
            var readResut = Read(address, 8);
            var result = new Result<double>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToDouble(readResut.Value, 0);
            return result;
        }
        #endregion

        #region 写
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, bool value)
        {
            byte[] valueByte = new byte[1];
            if (value) valueByte[0] = 16;
            return Write(address, valueByte, true);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <param name="isBit"></param>
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
                Array.Reverse(data);

                //发送写入信息
                MitsubishiMCData arg = null;
                byte[] command = null;
                switch (version)
                {
                    case MitsubishiVersion.A_1E:
                        arg = ConvertAddress_A_1E(address);
                        command = GetWriteCommand_A_1E(arg.BeginAddress, arg.MitsubishiMCType.TypeCode, data, isBit);
                        break;
                    case MitsubishiVersion.Qna_3E:
                        arg = ConvertAddress_Qna_3E(address);
                        command = GetWriteCommand_Qna_3E(arg.BeginAddress, arg.MitsubishiMCType.TypeCode, data, isBit);
                        break;
                }
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                byte[] dataPackage = null;
                switch (version)
                {
                    case MitsubishiVersion.A_1E:
                        dataPackage = SendPackage(command, 2);
                        break;
                    case MitsubishiVersion.Qna_3E:
                        dataPackage = SendPackage(command);
                        break;
                }
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

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, byte value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, sbyte value)
        {
            return Write(address, BitConverter.GetBytes(value));
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
        /// <returns></returns>
        public Result Write(string address, string value)
        {
            var valueBytes = Encoding.ASCII.GetBytes(value);
            var bytes = new byte[valueBytes.Length + 1];
            bytes[0] = (byte)valueBytes.Length;
            valueBytes.CopyTo(bytes, 1);
            Array.Reverse(bytes);
            return Write(address, bytes);
        }
        #endregion

        #region 生成报文命令
        /// <summary>
        /// 获取Qna_3E读取命令
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="typeCode"></param>
        /// <param name="length"></param>
        /// <param name="isBit"></param>
        /// <returns></returns>
        protected byte[] GetReadCommand_Qna_3E(int beginAddress, byte[] typeCode, ushort length, bool isBit)
        {
            if (!isBit) length = (ushort)(length / 2);

            byte[] command = new byte[21];
            command[0] = 0x50;
            command[1] = 0x00; //副头部
            command[2] = 0x00; //网络编号
            command[3] = 0xFF; //PLC编号
            command[4] = 0xFF;
            command[5] = 0x03; //IO编号
            command[6] = 0x00; //模块站号
            command[7] = (byte)((command.Length - 9) % 256);
            command[8] = (byte)((command.Length - 9) / 256); // 请求数据长度
            command[9] = 0x0A;
            command[10] = 0x00; //时钟
            command[11] = 0x01;
            command[12] = 0x04;//指令（0x01 0x04读 0x01 0x14写）
            command[13] = isBit ? (byte)0x01 : (byte)0x00;//子指令（位 或 字节为单位）
            command[14] = 0x00;
            command[15] = BitConverter.GetBytes(beginAddress)[0];// 起始地址的地位
            command[16] = BitConverter.GetBytes(beginAddress)[1];
            command[17] = BitConverter.GetBytes(beginAddress)[2];
            command[18] = typeCode[0]; //数据类型
            command[19] = (byte)(length % 256);
            command[20] = (byte)(length / 256); //长度
            return command;
        }

        /// <summary>
        /// 获取A_1E读取命令
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="typeCode"></param>
        /// <param name="length"></param>
        /// <param name="isBit"></param>
        /// <returns></returns>
        protected byte[] GetReadCommand_A_1E(int beginAddress, byte[] typeCode, ushort length, bool isBit)
        {
            if (!isBit)
                length = (ushort)(length / 2);
            byte[] command = new byte[12];
            command[0] = isBit ? (byte)0x00 : (byte)0x01;//副头部
            command[1] = 0xFF; //PLC编号
            command[2] = 0x0A;
            command[3] = 0x00;
            command[4] = BitConverter.GetBytes(beginAddress)[0]; // 
            command[5] = BitConverter.GetBytes(beginAddress)[1]; // 开始读取的地址
            command[6] = 0x00;
            command[7] = 0x00;
            command[8] = typeCode[1];
            command[9] = typeCode[0];
            command[10] = (byte)(length % 256);//长度
            command[11] = (byte)(length / 256);
            return command;
        }

        /// <summary>
        /// 获取Qna_3E写入命令
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="typeCode"></param>
        /// <param name="data"></param>
        /// <param name="isBit"></param>
        /// <returns></returns>
        protected byte[] GetWriteCommand_Qna_3E(int beginAddress, byte[] typeCode, byte[] data, bool isBit)
        {
            var length = data.Length / 2;
            if (isBit) length = 1;

            byte[] command = new byte[21 + data.Length];
            command[0] = 0x50;
            command[1] = 0x00; //副头部
            command[2] = 0x00; //网络编号
            command[3] = 0xFF; //PLC编号
            command[4] = 0xFF;
            command[5] = 0x03; //IO编号
            command[6] = 0x00; //模块站号
            command[7] = (byte)((command.Length - 9) % 256);// 请求数据长度
            command[8] = (byte)((command.Length - 9) / 256);
            command[9] = 0x0A;
            command[10] = 0x00; //时钟
            command[11] = 0x01;
            command[12] = 0x14;//指令（0x01 0x04读 0x01 0x14写）
            command[13] = isBit ? (byte)0x01 : (byte)0x00;//子指令（位 或 字节为单位）
            command[14] = 0x00;
            command[15] = BitConverter.GetBytes(beginAddress)[0];// 起始地址的地位
            command[16] = BitConverter.GetBytes(beginAddress)[1];
            command[17] = BitConverter.GetBytes(beginAddress)[2];
            command[18] = typeCode[0];//数据类型
            command[19] = (byte)(length % 256);
            command[20] = (byte)(length / 256); //长度
            data.Reverse().ToArray().CopyTo(command, 21);
            return command;
        }

        /// <summary>
        /// 获取A_1E写入命令
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="typeCode"></param>
        /// <param name="data"></param>
        /// <param name="isBit"></param>
        /// <returns></returns>
        protected byte[] GetWriteCommand_A_1E(int beginAddress, byte[] typeCode, byte[] data, bool isBit)
        {
            var length = data.Length / 2;
            if (isBit) length = data.Length;

            byte[] command = new byte[12 + data.Length];
            command[0] = isBit ? (byte)0x02 : (byte)0x03;     //副标题
            command[1] = 0xFF;                             // PLC号
            command[2] = 0x0A;
            command[3] = 0x00;
            command[4] = BitConverter.GetBytes(beginAddress)[0];        //
            command[5] = BitConverter.GetBytes(beginAddress)[1];        //起始地址的地位
            command[6] = 0x00;
            command[7] = 0x00;
            command[8] = typeCode[1];        //
            command[9] = typeCode[0];        //数据类型
            command[10] = (byte)(length % 256);
            command[11] = (byte)(length / 256);
            data.Reverse().ToArray().CopyTo(command, 12);
            return command;
        }
        #endregion

        #region private
        /// <summary>
        /// 获取内容长度
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        private int GetContentLength(byte[] head)
        {
            return BitConverter.ToUInt16(head, 7);
        }

        /// <summary>
        /// 地址转换
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private MitsubishiMCData ConvertAddress_Qna_3E(string address)
        {
            address = address.ToUpper();
            var addressData = new MitsubishiMCData();
            switch (address[0])
            {
                case 'M':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.M;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.M.Format);
                    }
                    break;
                case 'X':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.X;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.X.Format);
                    }
                    break;
                case 'Y':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.Y;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.Y.Format);
                    }
                    break;
                case 'D':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.D;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.D.Format);
                    }
                    break;
                case 'W':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.W;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.W.Format);
                    }
                    break;
                case 'L':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.L;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.L.Format);
                    }
                    break;
                case 'F':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.F;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.F.Format);
                    }
                    break;
                case 'V':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.V;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.V.Format);
                    }
                    break;
                case 'B':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.B;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.B.Format);
                    }
                    break;
                case 'R':
                    {
                        addressData.MitsubishiMCType = MitsubishiMCType.R;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.R.Format);
                    }
                    break;
                case 'S':
                    {
                        if (address[1] == 'C')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.SC;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.SC.Format);
                        }
                        else if (address[1] == 'S')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.SS;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.SS.Format);
                        }
                        else if (address[1] == 'N')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.SN;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.SN.Format);
                        }
                        else
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.S;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.S.Format);
                        }
                        break;
                    }
                case 'Z':
                    {
                        if (address[1] == 'R')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.ZR;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.ZR.Format);
                        }
                        else
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.Z;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.Z.Format);
                        }
                        break;
                    }
                case 'T':
                    {
                        if (address[1] == 'N')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.TN;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.TN.Format);
                        }
                        else if (address[1] == 'S')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.TS;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.TS.Format);
                        }
                        else if (address[1] == 'C')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.TC;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.TC.Format);
                        }
                        break;
                    }
                case 'C':
                    {
                        if (address[1] == 'N')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.CN;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.CN.Format);
                        }
                        else if (address[1] == 'S')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.CS;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.CS.Format);
                        }
                        else if (address[1] == 'C')
                        {
                            addressData.MitsubishiMCType = MitsubishiMCType.CC;
                            addressData.BeginAddress = Convert.ToInt32(address.Substring(2), MitsubishiMCType.CC.Format);
                        }
                        break;
                    }
            }
            return addressData;
        }

        private MitsubishiMCData ConvertAddress_A_1E(string address)
        {
            address = address.ToUpper();
            var addressData = new MitsubishiMCData();
            switch (address[0])
            {
                case 'X':
                    {
                        addressData.MitsubishiMCType = MitsubishiA1Type.X;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.X.Format);
                    }
                    break;
                case 'Y':
                    {
                        addressData.MitsubishiMCType = MitsubishiA1Type.Y;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.Y.Format);
                    }
                    break;
                case 'M':
                    {
                        addressData.MitsubishiMCType = MitsubishiA1Type.M;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.M.Format);
                    }
                    break;
                case 'S':
                    {
                        addressData.MitsubishiMCType = MitsubishiA1Type.S;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.S.Format);
                    }
                    break;
                case 'D':
                    {
                        addressData.MitsubishiMCType = MitsubishiA1Type.D;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.D.Format);
                    }
                    break;
                case 'R':
                    {
                        addressData.MitsubishiMCType = MitsubishiA1Type.R;
                        addressData.BeginAddress = Convert.ToInt32(address.Substring(1), MitsubishiMCType.R.Format);
                    }
                    break;
            }
            return addressData;
        }

        private MitsubishiMCWrite ConvertWriteArg_Qna_3E(string address, byte[] writeData)
        {
            MitsubishiMCWrite arg = new MitsubishiMCWrite(ConvertAddress_Qna_3E(address));
            arg.WriteData = writeData;
            //arg.ReadWriteBit = bit;
            return arg;
        }

        #region TODO
        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            throw new NotImplementedException();
        }

        public Result<byte> ReadByte(string address)
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

        #endregion
        #endregion
    }
}
