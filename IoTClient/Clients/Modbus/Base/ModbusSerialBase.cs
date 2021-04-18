using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace IoTClient.Clients.Modbus
{
    public abstract class ModbusSerialBase : SerialPortBase, IModbusClient
    {
        protected EndianFormat format;

        /// <summary>
        /// 警告日志委托        
        /// </summary>
        public LoggerDelegate WarningLog { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="portName">COM端口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="format">大小端设置</param>
        public ModbusSerialBase(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout = 1500, EndianFormat format = EndianFormat.ABCD)
        {
            if (serialPort == null) serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = dataBits;
            serialPort.StopBits = stopBits;
            serialPort.Encoding = Encoding.ASCII;
            serialPort.Parity = parity;

            serialPort.ReadTimeout = timeout;
            serialPort.WriteTimeout = timeout;

            this.format = format;
#if DEBUG
            serialPort.ReadTimeout *= 10;
            serialPort.WriteTimeout *= 10;
#endif

        }

        #region 发送报文，并获取响应报文
        /// <summary>
        /// 发送报文，并获取响应报文
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public byte[] SendPackage(byte[] command)
        {
            //从发送命令到读取响应为最小单元，避免多线程执行串数据（可线程安全执行）
            lock (this)
            {
                //发送命令
                serialPort.Write(command, 0, command.Length);
                //获取响应报文
                return SerialPortRead(serialPort);
            }
        }
        #endregion

        #region  Read 读取
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="readLength">读取长度</param>
        /// <returns></returns>
        public abstract Result<byte[]> Read(string address, byte stationNumber = 1, byte functionCode = 3, ushort readLength = 1, bool byteFormatting = true);

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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
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
            return result.EndTime();
        }

        /// <summary>
        /// 读取离散
        /// </summary>
        /// <param name="address"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public Result<bool> ReadDiscrete(string address, byte stationNumber = 1, byte functionCode = 2)
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
            return result.EndTime();
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<short> ReadInt16(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
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

        public Result<short> ReadInt16(int beginAddress, int address, byte[] values)
        {
            return ReadInt16(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<ushort> ReadUInt16(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
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

        public Result<ushort> ReadUInt16(int beginAddress, int address, byte[] values)
        {
            return ReadUInt16(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<int> ReadInt32(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).Reverse().ToArray().ByteFormatting(format);
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

        public Result<int> ReadInt32(int beginAddress, int address, byte[] values)
        {
            return ReadInt32(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<uint> ReadUInt32(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).Reverse().ToArray().ByteFormatting(format);
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

        public Result<uint> ReadUInt32(int beginAddress, int address, byte[] values)
        {
            return ReadUInt32(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<long> ReadInt64(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).Reverse().ToArray().ByteFormatting(format);
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

        public Result<long> ReadInt64(int beginAddress, int address, byte[] values)
        {
            return ReadInt64(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<ulong> ReadUInt64(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).Reverse().ToArray().ByteFormatting(format);
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

        public Result<ulong> ReadUInt64(int beginAddress, int address, byte[] values)
        {
            return ReadUInt64(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<float> ReadFloat(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).Reverse().ToArray().ByteFormatting(format);
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

        public Result<float> ReadFloat(int beginAddress, int address, byte[] values)
        {
            return ReadFloat(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<double> ReadDouble(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).Reverse().ToArray().ByteFormatting(format);
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

        public Result<double> ReadDouble(int beginAddress, int address, byte[] values)
        {
            return ReadDouble(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<bool> ReadCoil(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
                var index = (interval + 1) % 8 == 0 ? (interval + 1) / 8 : (interval + 1) / 8 + 1;
                var binaryArray = Convert.ToInt32(values[index - 1]).IntToBinaryArray().ToArray().Reverse().ToArray();
                var isBit = false;
                if ((index - 1) * 8 + binaryArray.Length > interval)
                    isBit = binaryArray[interval - (index - 1) * 8].ToString() == 1.ToString();
                return new Result<bool>()
                {
                    Value = isBit
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

        public Result<bool> ReadCoil(int beginAddress, int address, byte[] values)
        {
            return ReadCoil(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<bool> ReadDiscrete(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
                var index = (interval + 1) % 8 == 0 ? (interval + 1) / 8 : (interval + 1) / 8 + 1;
                var binaryArray = Convert.ToInt32(values[index - 1]).IntToBinaryArray().ToArray().Reverse().ToArray();
                var isBit = false;
                if ((index - 1) * 8 + binaryArray.Length > interval)
                    isBit = binaryArray[interval - (index - 1) * 8].ToString() == 1.ToString();
                return new Result<bool>()
                {
                    Value = isBit
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

        public Result<bool> ReadDiscrete(int beginAddress, int address, byte[] values)
        {
            return ReadDiscrete(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 分批读取（批量读取，内部进行批量计算读取）
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public Result<List<ModbusOutput>> BatchRead(List<ModbusInput> addresses)
        {
            var result = new Result<List<ModbusOutput>>();
            result.Value = new List<ModbusOutput>();
            var functionCodes = addresses.Select(t => t.FunctionCode).Distinct();
            foreach (var functionCode in functionCodes)
            {
                var stationNumbers = addresses.Where(t => t.FunctionCode == functionCode).Select(t => t.StationNumber).Distinct();
                foreach (var stationNumber in stationNumbers)
                {
                    var addressList = addresses.Where(t => t.FunctionCode == functionCode && t.StationNumber == t.StationNumber)
                        .DistinctBy(t => t.Address)
                        .ToDictionary(t => t.Address, t => t.DataType);
                    var tempResult = BatchRead(addressList, stationNumber, functionCode);
                    if (tempResult.IsSucceed)
                    {
                        foreach (var item in tempResult.Value)
                        {
                            result.Value.Add(new ModbusOutput()
                            {
                                Address = item.Key,
                                FunctionCode = functionCode,
                                StationNumber = stationNumber,
                                Value = item.Value
                            });
                        }
                    }
                    else
                    {
                        result.IsSucceed = tempResult.IsSucceed;
                        result.Err = tempResult.Err;
                        result.ErrList.AddRange(tempResult.ErrList);
                    }
                }
            }
            return result.EndTime();
        }

        private Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addressList, byte stationNumber, byte functionCode)
        {
            var result = new Result<Dictionary<string, object>>();
            result.Value = new Dictionary<string, object>();

            var addresses = addressList.Select(t => new KeyValuePair<int, DataTypeEnum>(int.Parse(t.Key), t.Value)).ToList();

            var minAddress = addresses.Select(t => t.Key).Min();
            var maxAddress = addresses.Select(t => t.Key).Max();
            while (maxAddress >= minAddress)
            {
                int readLength = 121;//125 - 4 = 121

                var tempAddress = addresses.Where(t => t.Key >= minAddress && t.Key <= minAddress + readLength).ToList();
                //如果范围内没有数据。按正确逻辑不存在这种情况。
                if (!tempAddress.Any())
                {
                    minAddress = minAddress + readLength;
                    continue;
                }

                var tempMax = tempAddress.OrderByDescending(t => t.Key).FirstOrDefault();
                switch (tempMax.Value)
                {
                    case DataTypeEnum.Bool:
                    case DataTypeEnum.Byte:
                    case DataTypeEnum.Int16:
                    case DataTypeEnum.UInt16:
                        readLength = tempMax.Key + 1 - minAddress;
                        break;
                    case DataTypeEnum.Int32:
                    case DataTypeEnum.UInt32:
                    case DataTypeEnum.Float:
                        readLength = tempMax.Key + 2 - minAddress;
                        break;
                    case DataTypeEnum.Int64:
                    case DataTypeEnum.UInt64:
                    case DataTypeEnum.Double:
                        readLength = tempMax.Key + 4 - minAddress;
                        break;
                    default:
                        throw new Exception("Err BatchRead 未定义类型 -1");
                }

                var tempResult = Read(minAddress.ToString(), stationNumber, functionCode, Convert.ToUInt16(readLength), false);

                if (!tempResult.IsSucceed)
                {
                    result.IsSucceed = tempResult.IsSucceed;
                    result.Exception = tempResult.Exception;
                    result.Err = tempResult.Err;
                    result.ErrList.AddRange(tempResult.ErrList);
                    return result.EndTime();
                }

                var rValue = tempResult.Value.Reverse().ToArray();
                foreach (var item in tempAddress)
                {
                    object tempVaue = null;

                    switch (item.Value)
                    {
                        case DataTypeEnum.Bool:
                            tempVaue = ReadCoil(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.Byte:
                            throw new Exception("Err BatchRead 未定义类型 -2");
                        case DataTypeEnum.Int16:
                            tempVaue = ReadInt16(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.UInt16:
                            tempVaue = ReadUInt16(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.Int32:
                            tempVaue = ReadInt32(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.UInt32:
                            tempVaue = ReadUInt32(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.Int64:
                            tempVaue = ReadInt64(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.UInt64:
                            tempVaue = ReadUInt64(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.Float:
                            tempVaue = ReadFloat(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        case DataTypeEnum.Double:
                            tempVaue = ReadDouble(minAddress.ToString(), item.Key.ToString(), rValue).Value;
                            break;
                        default:
                            throw new Exception("Err BatchRead 未定义类型 -3");
                    }

                    result.Value.Add(item.Key.ToString(), tempVaue);
                }
                minAddress = minAddress + readLength;

                if (addresses.Any(t => t.Key >= minAddress))
                    minAddress = addresses.Where(t => t.Key >= minAddress).OrderBy(t => t.Key).FirstOrDefault().Key;
                else
                    return result.EndTime();
            }
            return result.EndTime();
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
        public abstract Result Write(string address, bool value, byte stationNumber = 1, byte functionCode = 5);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public abstract Result Write(string address, byte[] values, byte stationNumber = 1, byte functionCode = 16);

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
            byte[] buffer = new byte[6];
            buffer[0] = stationNumber;  //站号
            buffer[1] = functionCode;   //功能码
            buffer[2] = BitConverter.GetBytes(readAddress)[1];
            buffer[3] = BitConverter.GetBytes(readAddress)[0];//寄存器地址
            buffer[4] = BitConverter.GetBytes(length)[1];
            buffer[5] = BitConverter.GetBytes(length)[0];//表示request 寄存器的长度(寄存器个数)
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
            byte[] buffer = new byte[7 + values.Length];
            buffer[0] = stationNumber; //站号
            buffer[1] = functionCode;  //功能码
            buffer[2] = BitConverter.GetBytes(writeAddress)[1];
            buffer[3] = BitConverter.GetBytes(writeAddress)[0];//寄存器地址
            buffer[4] = (byte)(values.Length / 2 / 256);
            buffer[5] = (byte)(values.Length / 2 % 256);//写寄存器数量(除2是两个字节一个寄存器，寄存器16位。除以256是byte最大存储255。)              
            buffer[6] = (byte)(values.Length);          //写字节的个数
            values.CopyTo(buffer, 7);                   //把目标值附加到数组后面
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
            byte[] buffer = new byte[6];
            buffer[0] = stationNumber;//站号
            buffer[1] = functionCode; //功能码
            buffer[2] = BitConverter.GetBytes(writeAddress)[1];
            buffer[3] = BitConverter.GetBytes(writeAddress)[0];//寄存器地址
            buffer[4] = (byte)(value ? 0xFF : 0x00);     //此处只可以是FF表示闭合00表示断开，其他数值非法
            buffer[5] = 0x00;
            return buffer;
        }

        #endregion
    }
}
