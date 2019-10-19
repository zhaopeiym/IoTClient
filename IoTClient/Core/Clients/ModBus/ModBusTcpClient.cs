using IoTClient.Core;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace IoTClient.Clients.ModBus
{
    /// <summary>
    /// ModBusRtu协议客户端
    /// </summary>
    public class ModBusTcpClient : SocketBase
    {
        private IPEndPoint ipAndPoint;
        private int timeout = 1500;

        public ModBusTcpClient(IPEndPoint ipAndPoint, int? timeout = null)
        {
            if (timeout.HasValue) this.timeout = timeout.Value;
            this.ipAndPoint = ipAndPoint;
        }

        public ModBusTcpClient(string ip, int port, int? timeout = null)
        {
            if (timeout.HasValue) this.timeout = timeout.Value;
            this.ipAndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        protected override bool Connect()
        {
            socket?.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
#if DEBUG
                //调试模式增加超时时间
                socket.ReceiveTimeout = timeout * 100;
                socket.SendTimeout = timeout * 100;
#else
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;
#endif

                //连接
                socket.Connect(ipAndPoint);
                return true;
            }
            catch (Exception)
            {
                if (socket.Connected) socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                return false;
            }
        }

        #region Read 读取
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="readLength">读取长度</param>
        /// <returns></returns>
        public Result<byte[]> Read(string address, byte stationNumber = 1, byte functionCode = 3, ushort readLength = 1)
        {
            if (!socket?.Connected ?? true) Connect();
            var result = new Result<byte[]>();
            try
            {
                //1 获取命令（组装报文）
                byte[] command = GetReadCommand(address, stationNumber, functionCode, readLength);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //2 发送命令
                socket.Send(command);
                //3 获取响应报文
                var headPackage = SocketRead(socket, 8);
                int length = headPackage[4] * 256 + headPackage[5] - 2;
                var dataPackage = SocketRead(socket, length);

                byte[] resultBuffer = new byte[dataPackage.Length - 1];
                Array.Copy(dataPackage, 1, resultBuffer, 0, resultBuffer.Length);

                result.Response = string.Join(" ", headPackage.Concat(dataPackage).Select(t => t.ToString("X2")));
                //4 获取响应报文数据（字节数组形式）                
                result.Value = resultBuffer.Reverse().ToArray();
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
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
        }

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<short> ReadInt16(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<ushort> ReadUInt16(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<int> ReadInt32(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode, readLength: 2);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<uint> ReadUInt32(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode, readLength: 2);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<long> ReadInt64(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode, readLength: 4);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<ulong> ReadUInt64(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode, readLength: 4);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<float> ReadFloat(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode, readLength: 2);
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
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<double> ReadDouble(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = Read(address, stationNumber, functionCode, readLength: 4);
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
        /// 读取线圈
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public Result<bool> ReadCoil(string address, byte stationNumber = 1, byte functionCode = 1)
        {
            var readResut = Read(address, stationNumber, functionCode);
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
        #endregion

        #region Write 写入

        /// <summary>
        /// 线圈写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        public Result Write(string address, bool value, byte stationNumber = 1, byte functionCode = 5)
        {
            if (!socket?.Connected ?? true) Connect();
            var result = new Result();
            try
            {
                var command = GetWriteCoilCommand(address, value, stationNumber, functionCode);
                socket.Send(command);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //获取响应报文
                var headBytes = SocketRead(socket, 8);
                int length = headBytes[4] * 256 + headBytes[5] - 2;
                var dataBytes = SocketRead(socket, length);
                result.Response = string.Join(" ", headBytes.Concat(dataBytes).Select(t => t.ToString("X2")));
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
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public Result Write(string address, byte[] values, byte stationNumber = 1, byte functionCode = 16)
        {
            if (!socket?.Connected ?? true) Connect();

            var result = new Result();
            try
            {
                var command = GetWriteCommand(address, values, stationNumber, functionCode);
                socket.Send(command);
                result.Requst = string.Join(" ", command.Select(t => t.ToString("X2")));
                //获取响应报文
                var headBytes = SocketRead(socket, 8);
                int length = headBytes[4] * 256 + headBytes[5] - 2;
                var dataBytes = SocketRead(socket, length);
                result.Response = string.Join(" ", headBytes.Concat(dataBytes).Select(t => t.ToString("X2")));
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
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, short value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, ushort value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, int value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, uint value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, long value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, ulong value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, float value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public Result Write(string address, double value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return Write(address, values, stationNumber, functionCode);
        }
        #endregion

        #region 获取命令

        /// <summary>
        /// 获取读取命令
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="length">读取长度</param>
        /// <returns></returns>
        public byte[] GetReadCommand(string address, byte stationNumber, byte functionCode, ushort length)
        {
            var readAddress = ushort.Parse(address?.Trim());
            byte[] buffer = new byte[12];
            buffer[0] = 0x19;
            buffer[1] = 0xB2;//Client发出的检验信息
            buffer[2] = 0x00;
            buffer[3] = 0x00;//表示tcp/ip 的协议的modbus的协议
            buffer[4] = 0x00;
            buffer[5] = 0x06;//表示的是该字节以后的字节长度

            buffer[6] = stationNumber;  //站号
            buffer[7] = functionCode;   //功能码
            buffer[8] = BitConverter.GetBytes(readAddress)[1];
            buffer[9] = BitConverter.GetBytes(readAddress)[0];//寄存器地址
            buffer[10] = BitConverter.GetBytes(length)[1];
            buffer[11] = BitConverter.GetBytes(length)[0];//表示request 寄存器的长度(寄存器个数)
            return buffer;
        }

        /// <summary>
        /// 获取写入命令
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="values"></param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public byte[] GetWriteCommand(string address, byte[] values, byte stationNumber, byte functionCode)
        {
            var writeAddress = ushort.Parse(address?.Trim());
            byte[] buffer = new byte[13 + values.Length];
            buffer[0] = 0x19;
            buffer[1] = 0xB2;//检验信息，用来验证response是否串数据了           
            buffer[4] = BitConverter.GetBytes(7 + values.Length)[1];
            buffer[5] = BitConverter.GetBytes(7 + values.Length)[0];//表示的是header handle后面还有多长的字节

            buffer[6] = stationNumber; //站号
            buffer[7] = functionCode;  //功能码
            buffer[8] = BitConverter.GetBytes(writeAddress)[1];
            buffer[9] = BitConverter.GetBytes(writeAddress)[0];//寄存器地址
            buffer[10] = (byte)(values.Length / 2 / 256);
            buffer[11] = (byte)(values.Length / 2 % 256);//写寄存器数量(除2是两个字节一个寄存器，寄存器16位。除以256是byte最大存储255。)              
            buffer[12] = (byte)(values.Length);          //写字节的个数
            values.CopyTo(buffer, 13);                   //把目标值附加到数组后面
            return buffer;
        }

        /// <summary>
        /// 获取线圈写入命令
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value"></param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public byte[] GetWriteCoilCommand(string address, bool value, byte stationNumber, byte functionCode)
        {
            var writeAddress = ushort.Parse(address?.Trim());
            byte[] buffer = new byte[12];
            buffer[0] = 0x19;
            buffer[1] = 0xB2;//Client发出的检验信息     
            buffer[4] = 0x00;
            buffer[5] = 0x06;//表示的是该字节以后的字节长度

            buffer[6] = stationNumber;//站号
            buffer[7] = functionCode; //功能码
            buffer[8] = BitConverter.GetBytes(writeAddress)[1];
            buffer[9] = BitConverter.GetBytes(writeAddress)[0];//寄存器地址
            buffer[10] = (byte)(value ? 0xFF : 0x00);     //此处只可以是FF表示闭合00表示断开，其他数值非法
            buffer[11] = 0x00;
            return buffer;
        }

        #endregion      
    }
}
