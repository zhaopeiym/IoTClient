using IoTClient.Common.Constants;
using IoTClient.Common.Enums;
using IoTClient.Core;
using IoTClient.Core.Models;
using System;
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

        public SiemensClient(SiemensVersion version, IPEndPoint ipAndPoint)
        {
            this.version = version;
            this.ipAndPoint = ipAndPoint;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        protected override bool Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //连接
                socket.Connect(ipAndPoint);

                //第一次握手
                socket.Send(SiemensConstant.Head1_200Smart);
                var head1 = SocketRead(socket, SiemensConstant.InitHeadLength);
                SocketRead(socket, GetContentLength(head1));

                //第二次握手
                socket.Send(SiemensConstant.Head2_200Smart);
                var head2 = SocketRead(socket, SiemensConstant.InitHeadLength);
                SocketRead(socket, GetContentLength(head2));
                return true;
            }
            catch (Exception ex)
            {
                //TODO
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                return false;
            }
        }

        #region Write

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, bool value)
        {
            if (!isAutoOpen.HasValue) isAutoOpen = true;
            if (isAutoOpen.Value) Connect();
            try
            {
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString())) address = address.Remove(1, 1);
                //发送写入信息
                var arg = ConvertArg(address);
                byte[] plccommand = GetWriteByteCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, value);
                try
                {
                    socket.Send(plccommand);
                }
                catch (Exception ex)
                {
                    socket?.Shutdown(SocketShutdown.Both);
                    socket?.Close();
                    throw ex;
                }
                //以下两次请求不能省略，不然在主动管理连接的时候会有问题
                var dataHead = SocketRead(socket, SiemensConstant.InitHeadLength);
                var dataContent = SocketRead(socket, GetContentLength(dataHead));
                return true;
            }
            finally
            {
                if (isAutoOpen.Value) Dispose();
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, byte[] data)
        {
            if (!isAutoOpen.HasValue) isAutoOpen = true;
            if (isAutoOpen.Value) Connect();
            try
            {
                Array.Reverse(data);
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString())) address = address.Remove(1, 1);
                //发送写入信息
                var arg = ConvertArg(address);
                byte[] plccommand = GetWriteCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, data);
                try
                {
                    socket.Send(plccommand);
                }
                catch (Exception ex)
                {
                    socket?.Shutdown(SocketShutdown.Both);
                    socket?.Close();
                    throw ex;
                }
                //以下两次请求不能省略，不然在主动管理连接的时候会有问题
                var dataHead = SocketRead(socket, SiemensConstant.InitHeadLength);
                var dataContent = SocketRead(socket, GetContentLength(dataHead));
                return true;
            }
            finally
            {
                if (isAutoOpen.Value) Dispose();
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, byte value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, sbyte value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, ushort value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, short value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, uint value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, int value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, float value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, double value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, long value)
        {
            return Write(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Write(string address, string value)
        {
            var valueBytes = Encoding.ASCII.GetBytes(value);
            var bytes = new byte[valueBytes.Length + 1];
            bytes[0] = (byte)valueBytes.Length;
            valueBytes.CopyTo(bytes, 1);
            Array.Reverse(bytes);
            return Write(address, bytes);
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
        public byte[] Read(string address, ushort length, bool isBit = false)
        {
            if (!isAutoOpen.HasValue) isAutoOpen = true;
            if (isAutoOpen.Value) Connect();
            try
            {
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString()))
                    address = address.Remove(1, 1);
                //发送读取信息
                var arg = ConvertArg(address);
                byte[] plccommand;
                if (isBit)
                    plccommand = GetReadBitCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock);
                else
                    plccommand = GetReadCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, length);
                try
                {
                    socket.Send(plccommand);
                }
                catch (Exception ex)
                {
                    socket?.Shutdown(SocketShutdown.Both);
                    socket?.Close();
                    throw ex;
                }
                var dataHead = SocketRead(socket, SiemensConstant.InitHeadLength);
                var dataContent = SocketRead(socket, GetContentLength(dataHead));
                var bufferLength = length == 8 ? 8 : 4;
                byte[] temp = new byte[bufferLength];
                byte[] result = new byte[bufferLength];
                Array.Copy(dataContent, dataContent.Length - bufferLength, temp, 0, bufferLength);
                switch (length)
                {
                    case 8:
                        result[0] = temp[0 + 7];
                        result[1] = temp[0 + 6];
                        result[2] = temp[0 + 5];
                        result[3] = temp[0 + 4];
                        result[4] = temp[0 + 3];
                        result[5] = temp[0 + 2];
                        result[6] = temp[0 + 1];
                        result[7] = temp[0 + 0];
                        break;
                    default:
                        result[0] = temp[0 + 3];
                        result[1] = temp[0 + 2];
                        result[2] = temp[0 + 1];
                        result[3] = temp[0 + 0];
                        break;
                }
                return result;
            }
            finally
            {
                if (isAutoOpen.Value) Dispose();
            }
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadString(string address, ushort length)
        {
            if (!isAutoOpen.HasValue) isAutoOpen = true;
            if (isAutoOpen.Value) Connect();
            try
            {
                //兼容地址，如VD5012中的D
                if (address.Length >= 2 && !"0123456789".Contains(address[1].ToString()))
                    address = address.Remove(1, 1);
                //发送读取信息
                var arg = ConvertArg(address);
                byte[] plccommand = GetReadCommand(arg.TypeCode, arg.BeginAddress, arg.DbBlock, length);
                try
                {
                    socket.Send(plccommand);
                }
                catch (Exception ex)
                {
                    socket?.Shutdown(SocketShutdown.Both);
                    socket?.Close();
                    throw ex;
                }
                var dataHead = SocketRead(socket, SiemensConstant.InitHeadLength);
                var dataContent = SocketRead(socket, GetContentLength(dataHead));
                byte[] result = new byte[length];
                Array.Copy(dataContent, dataContent.Length - length, result, 0, length);
                return result;
            }
            finally
            {
                if (isAutoOpen.Value) Dispose();
            }
        }

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public bool ReadBoolean(string address)
        {
            return BitConverter.ToBoolean(Read(address, 4, isBit: true), 0);
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public short ReadInt16(string address)
        {
            return BitConverter.ToInt16(Read(address, 2), 0);
        }

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public ushort ReadUInt16(string address)
        {
            return BitConverter.ToUInt16(Read(address, 2), 0);
        }

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public int ReadInt32(string address)
        {
            return BitConverter.ToInt32(Read(address, 4), 0);
        }

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public uint ReadUInt32(string address)
        {
            return BitConverter.ToUInt32(Read(address, 4), 0);
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public long ReadInt64(string address)
        {
            return BitConverter.ToInt64(Read(address, 8), 0);
        }

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public ulong ReadUInt64(string address)
        {
            return BitConverter.ToUInt64(Read(address, 8), 0);
        }

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public float ReadFloat(string address)
        {
            return BitConverter.ToSingle(Read(address, 4), 0);
        }

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public double ReadDouble(string address)
        {
            return BitConverter.ToDouble(Read(address, 8), 0);
        }

        /// <summary>
        /// 读取String
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public string ReadString(string address)
        {
            //先获取字符串的长度
            var length = ReadString(address, 1);
            return Encoding.ASCII.GetString(ReadString(address, (ushort)(length[0] + 1)), 1, length[0]);
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
            command[1] = 0x00;
            command[2] = 0x00;
            command[3] = 0x1F;
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;
            command[13] = 0x00;
            command[14] = 0x0E;
            command[15] = 0x00;
            command[16] = 0x00;
            command[17] = 0x04;
            command[18] = 0x01;
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x02;
            command[23] = (byte)(length / 256);
            command[24] = (byte)(length % 256);
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);
            command[27] = type;
            command[28] = (byte)(beginAddress / 256 / 256);
            command[29] = (byte)(beginAddress / 256);
            command[30] = (byte)(beginAddress % 256);
            return command;
        }

        protected byte[] GetReadBitCommand(byte type, int beginAddress, ushort dbAddress)
        {
            byte[] command = new byte[31];
            command[0] = 0x03;
            command[1] = 0x00;
            command[2] = (byte)(command.Length / 256);
            command[3] = (byte)(command.Length % 256);
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;
            command[13] = (byte)((command.Length - 17) / 256);
            command[14] = (byte)((command.Length - 17) % 256);
            command[15] = 0x00;
            command[16] = 0x00;
            command[17] = 0x04;
            command[18] = 0x01;
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x01;
            command[23] = 0x00;
            command[24] = 0x01;
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);
            command[27] = type;
            command[28] = (byte)(beginAddress / 256 / 256 % 256);
            command[29] = (byte)(beginAddress / 256 % 256);
            command[30] = (byte)(beginAddress % 256);
            return command;
        }

        protected byte[] GetWriteByteCommand(byte type, int beginAddress, ushort dbAddress, bool data)
        {
            var length = 1;
            byte[] command = new byte[35 + length];
            command[0] = 0x03;
            command[1] = 0x00;
            command[2] = (byte)((35 + length) / 256);
            command[3] = (byte)((35 + length) % 256);
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;
            command[13] = 0x00;
            command[14] = 0x0E;
            command[15] = (byte)((4 + length) / 256);
            command[16] = (byte)((4 + length) % 256);
            command[17] = 0x05;
            command[18] = 0x01;
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x01;
            command[23] = (byte)(length / 256);
            command[24] = (byte)(length % 256);
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);
            command[27] = type;
            command[28] = (byte)(beginAddress / 256 / 256);
            command[29] = (byte)(beginAddress / 256);
            command[30] = (byte)(beginAddress % 256);
            command[31] = 0x00;
            command[32] = 0x03;
            command[33] = (byte)(length / 256);
            command[34] = (byte)(length % 256);
            command[35] = data ? (byte)0x01 : (byte)0x00;
            return command;
        }

        protected byte[] GetWriteCommand(byte type, int beginAddress, ushort dbAddress, byte[] data)
        {
            byte[] command = new byte[35 + data.Length];
            command[0] = 0x03;
            command[1] = 0x00;
            command[2] = (byte)((35 + data.Length) / 256);
            command[3] = (byte)((35 + data.Length) % 256);
            command[4] = 0x02;
            command[5] = 0xF0;
            command[6] = 0x80;
            command[7] = 0x32;
            command[8] = 0x01;
            command[9] = 0x00;
            command[10] = 0x00;
            command[11] = 0x00;
            command[12] = 0x01;
            command[13] = 0x00;
            command[14] = 0x0E;
            command[15] = (byte)((4 + data.Length) / 256);
            command[16] = (byte)((4 + data.Length) % 256);
            command[17] = 0x05;
            command[18] = 0x01;
            command[19] = 0x12;
            command[20] = 0x0A;
            command[21] = 0x10;
            command[22] = 0x02;
            command[23] = (byte)(data.Length / 256);
            command[24] = (byte)(data.Length % 256);
            command[25] = (byte)(dbAddress / 256);
            command[26] = (byte)(dbAddress % 256);
            command[27] = type;
            command[28] = (byte)(beginAddress / 256 / 256 % 256); ;
            command[29] = (byte)(beginAddress / 256 % 256);
            command[30] = (byte)(beginAddress % 256);
            command[31] = 0x00;
            command[32] = 0x04;
            command[33] = (byte)(data.Length * 8 / 256);
            command[34] = (byte)(data.Length * 8 % 256);
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
