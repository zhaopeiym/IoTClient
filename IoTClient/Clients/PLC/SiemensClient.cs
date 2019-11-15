using IoTClient.Common.Constants;
using IoTClient.Common.Enums;
using IoTClient.Core;
using IoTClient.Core.Models;
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
    /// 西门子客户端
    /// </summary>
    public class SiemensClient : SocketBase
    {
        private SiemensVersion version;
        private IPEndPoint ipAndPoint;
        private IEnumerable<byte> dataPackage;
        private int timeout;

        public SiemensClient(SiemensVersion version, IPEndPoint ipAndPoint, int timeout = 1500)
        {
            this.version = version;
            this.ipAndPoint = ipAndPoint;
            this.timeout = timeout;
        }

        public SiemensClient(SiemensVersion version, string ip, int port, int timeout = 1500)
        {
            this.version = version;
            ipAndPoint = new IPEndPoint(IPAddress.Parse(ip), port); ;
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

                //连接
                socket.Connect(ipAndPoint);

                var Command1 = SiemensConstant.Command1;
                var Command2 = SiemensConstant.Command2;
                if (version == SiemensVersion.S7_200Smart)
                {
                    Command1 = SiemensConstant.Command1_200Smart;
                    Command2 = SiemensConstant.Command2_200Smart;
                }
                else if (version == SiemensVersion.S7_300)
                {
                    Command1 = SiemensConstant.Command1_300;
                }

                //第一次初始化指令交互
                socket.Send(Command1);
                var head1 = SocketRead(socket, SiemensConstant.InitHeadLength);
                SocketRead(socket, GetContentLength(head1));

                //第二次初始化指令交互
                socket.Send(Command2);
                var head2 = SocketRead(socket, SiemensConstant.InitHeadLength);
                SocketRead(socket, GetContentLength(head2));
                return result;
            }
            catch (Exception ex)
            {
                //TODO
                if (socket?.Connected ?? false) socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                result.IsSucceed = false;
                result.Err = ex.Message;
                return result;
            }
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
            var headPackage = SocketRead(socket, SiemensConstant.InitHeadLength);
            var dataPackage = SocketRead(socket, GetContentLength(headPackage));
            return headPackage.Concat(dataPackage).ToArray();
        }
        #endregion

        #region Read 
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length"></param>
        /// <param name="isBit"></param>
        /// <returns></returns>
        public Result<byte[]> Read(string address, ushort length, bool isBit = false)
        {
            if (!socket?.Connected ?? true) Connect();
            var result = new Result<byte[]>();
            try
            {
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString()))
                    address = address.Remove(1, 1);
                //发送读取信息
                var arg = ConvertArg(address);
                byte[] command;
                if (isBit)
                    command = GetReadBitCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock);
                else
                    command = GetReadCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, length);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //发送命令 并获取响应报文
                var dataPackage = SendPackage(command);
                var bufferLength = length == 8 ? 8 : 4;
                byte[] temp = new byte[bufferLength];
                byte[] response = new byte[bufferLength];
                Array.Copy(dataPackage, dataPackage.Length - bufferLength, temp, 0, bufferLength);
                switch (length)
                {
                    case 8:
                        response[0] = temp[7];
                        response[1] = temp[6];
                        response[2] = temp[5];
                        response[3] = temp[4];
                        response[4] = temp[3];
                        response[5] = temp[2];
                        response[6] = temp[1];
                        response[7] = temp[0];
                        break;
                    default:
                        response[0] = temp[3];
                        response[1] = temp[2];
                        response[2] = temp[1];
                        response[3] = temp[0];
                        break;
                }
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));
                result.Value = response;
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
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Result<byte[]> ReadString(string address, ushort length)
        {
            if (!socket?.Connected ?? true) Connect();
            var result = new Result<byte[]>();
            try
            {
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString()))
                    address = address.Remove(1, 1);
                //发送读取信息
                var arg = ConvertArg(address);
                byte[] command = GetReadCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, length);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));              
                var dataPackage = SendPackage(command);
                byte[] requst = new byte[length];
                Array.Copy(dataPackage, 25, requst, 0, length);
                //Array.Copy(dataPackage, dataPackage.Length - length, requst, 0, length);
                result.Response = string.Join(" ", dataPackage.Select(t => t.ToString("X2")));
                result.Value = requst;
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
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
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
            //return BitConverter.ToBoolean(Read(address, 4, isBit: true), 0);
            var readResut = Read(address, 4, isBit: true);
            var result = new Result<bool>()
            {
                IsSucceed = readResut.IsSucceed,
                Err = readResut.Err,
                ErrList = readResut.ErrList,
                Requst = readResut.Requst,
                Response = readResut.Response,
            };
            if (result.IsSucceed)
                result.Value = BitConverter.ToBoolean(readResut.Value, 0);
            return result;
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<short> ReadInt16(string address)
        {
            //return BitConverter.ToInt16(Read(address, 2), 0);
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
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<ushort> ReadUInt16(string address)
        {
            //return BitConverter.ToUInt16(Read(address, 2), 0);
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
            //return BitConverter.ToInt32(Read(address, 4), 0);
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
            //return BitConverter.ToUInt32(Read(address, 4), 0);
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
            // return BitConverter.ToInt64(Read(address, 8), 0);
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
            //return BitConverter.ToUInt64(Read(address, 8), 0);
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
            //return BitConverter.ToSingle(Read(address, 4), 0);
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
            //return BitConverter.ToDouble(Read(address, 8), 0);
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

        /// <summary>
        /// 读取String
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public Result<string> ReadString(string address)
        {
            //先获取字符串的长度
            var readResut1 = ReadString(address, 1);
            if (readResut1.IsSucceed)
            {
                var readResut2 = ReadString(address, (ushort)(readResut1.Value[0] + 1));
                var result = new Result<string>()
                {
                    IsSucceed = readResut2.IsSucceed,
                    Err = readResut2.Err,
                    ErrList = readResut2.ErrList,
                    Requst = readResut2.Requst,
                    Response = readResut2.Response,
                };
                if (result.IsSucceed)
                    result.Value = Encoding.ASCII.GetString(readResut2.Value, 1, readResut1.Value[0]);
                return result;
            }
            else
            {
                var result = new Result<string>()
                {
                    IsSucceed = readResut1.IsSucceed,
                    Err = readResut1.Err,
                    ErrList = readResut1.ErrList,
                    Requst = readResut1.Requst,
                    Response = readResut1.Response,
                };
                return result;
            }
            //return Encoding.ASCII.GetString(, 1, length[0]);
        }
        #endregion

        #region Write

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, bool value)
        {
            if (!socket?.Connected ?? true) Connect();
            Result result = new Result();
            try
            {
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString())) address = address.Remove(1, 1);
                //发送写入信息
                var arg = ConvertArg(address);
                byte[] command = GetWriteByteCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, value);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                var dataPackage = SendPackage(command);
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
                    result.ErrList.Add(ex.Message);
                }
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Result Write(string address, byte[] data)
        {
            if (!socket?.Connected ?? true) Connect();
            Result result = new Result();
            try
            {
                Array.Reverse(data);
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString())) address = address.Remove(1, 1);
                //发送写入信息
                var arg = ConvertArg(address);
                byte[] command = GetWriteCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, data);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                var dataPackage = SendPackage(command);
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
                    result.ErrList.Add(ex.Message);
                }
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
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

        #region private
        /// <summary>
        /// 获取区域类型代码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private SiemensData ConvertArg(string address)
        {
            var data = new SiemensData()
            {
                DbBlock = 0,
                BeginAddress = GetBeingAddress(address),
            };
            switch (address[0].ToString().ToUpper())
            {
                case "I":
                    data.TypeCode = 0x81;
                    break;
                case "Q":
                    data.TypeCode = 0x82;
                    break;
                case "M":
                    data.TypeCode = 0x83;
                    break;
                case "D":
                    //TODO DB DbBlock AddressStart
                    data.TypeCode = 0x84;
                    break;
                case "T":
                    data.TypeCode = 0x1D;
                    break;
                case "C":
                    data.TypeCode = 0x1C;
                    break;
                case "V":
                    data.TypeCode = 0x84;
                    data.DbBlock = 1;
                    break;
            }
            return data;
        }
        #endregion

        #region 获取指令
        /// <summary>
        /// 获取指令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="beginAddress"></param>
        /// <param name="dbAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected byte[] GetReadCommand(byte type, int beginAddress, ushort dbAddress, ushort length)
        {
            byte[] command = new byte[31];
            command[0] = 0x03;
            command[1] = 0x00;//[0][1]固定报文头
            command[2] = (byte)(command.Length / 256);
            command[3] = (byte)(command.Length % 256);//[2][3]整个读取请求长度为0x1F= 31 
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;//1  客户端发送命令 3 服务器回复命令
            command[9] = 0x00;
            command[10] = 0x00;//[4]-[10]固定6个字节
            command[11] = 0x00;
            command[12] = 0x01;//[11][12]两个字节，标识序列号，回复报文相同位置和这个完全一样；范围是0~65535
            command[13] = 0x00;
            command[14] = 0x0E;
            command[15] = 0x00;
            command[16] = 0x00;
            command[17] = 0x04;//04读 05写
            command[18] = 0x01;//读取数据块个数
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x02;
            command[23] = (byte)(length / 256);
            command[24] = (byte)(length % 256);//[23][24]两个字节,访问数据的个数，以byte为单位；
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);//[25][26]DB块的编号
            command[27] = type;//访问数据块的类型
            command[28] = (byte)(beginAddress / 256 / 256);
            command[29] = (byte)(beginAddress / 256);
            command[30] = (byte)(beginAddress % 256);//[28][29][30]访问DB块的偏移量
            return command;
        }

        protected byte[] GetReadBitCommand(byte type, int beginAddress, ushort dbAddress)
        {
            byte[] command = new byte[31];
            command[0] = 0x03;
            command[1] = 0x00;//[0][1]固定报文头
            command[2] = (byte)(command.Length / 256);
            command[3] = (byte)(command.Length % 256);//[2][3]整个读取请求长度
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;//1  客户端发送命令 3 服务器回复命令
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;
            command[13] = (byte)((command.Length - 17) / 256);
            command[14] = (byte)((command.Length - 17) % 256);
            command[15] = 0x00;
            command[16] = 0x00;
            command[17] = 0x04;//04读 05写
            command[18] = 0x01;//读取数据块个数
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x01;
            command[23] = 0x00;
            command[24] = 0x01;
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);//[25][26]DB块的编号
            command[27] = type;//访问数据块的类型
            command[28] = (byte)(beginAddress / 256 / 256 % 256);
            command[29] = (byte)(beginAddress / 256 % 256);
            command[30] = (byte)(beginAddress % 256);//[28][29][30]访问DB块的偏移量
            return command;
        }

        protected byte[] GetWriteByteCommand(byte type, int beginAddress, ushort dbAddress, bool data)
        {
            var length = 1;
            byte[] command = new byte[35 + length];
            command[0] = 0x03;
            command[1] = 0x00;//[0][1]固定报文头
            command[2] = (byte)((35 + length) / 256);
            command[3] = (byte)((35 + length) % 256);//[2][3]整个读取请求长度
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;//1  客户端发送命令 3 服务器回复命令
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;
            command[13] = 0x00;
            command[14] = 0x0E;//[13][14]固定2个字节
            command[15] = (byte)((4 + length) / 256);
            command[16] = (byte)((4 + length) % 256);//[15][16]写入长度+4
            command[17] = 0x05;//04读 05写
            command[18] = 0x01;
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x01;//写入方式，1是按位，2是按字
            command[23] = (byte)(length / 256);
            command[24] = (byte)(length % 256);
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);
            command[27] = type;
            command[28] = (byte)(beginAddress / 256 / 256);
            command[29] = (byte)(beginAddress / 256);
            command[30] = (byte)(beginAddress % 256);//[28][29][30]访问DB块的偏移量
            command[31] = 0x00;
            command[32] = 0x03;//04 byte(字节) 03bit（位）
            command[33] = (byte)(length / 256);
            command[34] = (byte)(length % 256);//按位计算出的长度
            command[35] = data ? (byte)0x01 : (byte)0x00;
            return command;
        }

        protected byte[] GetWriteCommand(byte type, int beginAddress, ushort dbAddress, byte[] data)
        {
            byte[] command = new byte[35 + data.Length];
            command[0] = 0x03;
            command[1] = 0x00;//[0][1]固定报文头
            command[2] = (byte)((35 + data.Length) / 256);
            command[3] = (byte)((35 + data.Length) % 256);//[2][3]整个读取请求长度
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;//[4]-[7]固定数据
            command[8] = 0x01;//1  客户端发送命令 3 服务器回复命令
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;//[9]-[12]标识序列号
            command[13] = 0x00;
            command[14] = 0x0E;
            command[15] = (byte)((4 + data.Length) / 256);
            command[16] = (byte)((4 + data.Length) % 256);//[15][16]写入长度+4
            command[17] = 0x05;//04读 05写
            command[18] = 0x01;//写入数据块个数
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;//[19]-[21]固定
            command[22] = 0x02;//写入方式，1是按位，2是按字
            command[23] = (byte)(data.Length / 256);
            command[24] = (byte)(data.Length % 256);//写入数据个数
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);//DB块的编号
            command[27] = type;
            command[28] = (byte)(beginAddress / 256 / 256 % 256); ;
            command[29] = (byte)(beginAddress / 256 % 256);
            command[30] = (byte)(beginAddress % 256);//[28][29][30]访问DB块的偏移量
            command[31] = 0x00;
            command[32] = 0x04;//04 byte(字节) 03bit（位）
            command[33] = (byte)(data.Length * 8 / 256);
            command[34] = (byte)(data.Length * 8 % 256);//按位计算出的长度
            data.CopyTo(command, 35);
            return command;
        }
        #endregion

        #region protected

        /// <summary>
        /// 获取需要读取的长度
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        protected int GetContentLength(byte[] head)
        {
            if (head?.Length >= 4)
                return head[2] * 256 + head[3] - 4;
            else
                throw new ArgumentException("请传入正确的参数");
        }

        /// <summary>
        /// 获取读取PLC地址的开始位置
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected int GetBeingAddress(string address)
        {
            //去掉V1025 前面的V
            address = address.Substring(1);
            //I1.3地址的情况
            if (address.IndexOf('.') < 0)
                return int.Parse(address) * 8;
            else
            {
                string[] temp = address.Split('.');
                return Convert.ToInt32(temp[0]) * 8 + Convert.ToInt32(temp[1]);
            }
        }
        #endregion
    }
}
