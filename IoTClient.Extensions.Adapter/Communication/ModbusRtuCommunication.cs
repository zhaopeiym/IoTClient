using IoTClient.Clients.Modbus;
using IoTClient.Enums;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace IoTClient.Extensions.Adapter.Communication
{
    public class ModbusRtuCommunication : IIoTClientCommon
    {
        private IModbusClient client;
        private string _ConnectionInfo;

        /// <summary>
        /// 连接信息
        /// </summary>
        public string ConnectionInfo => _ConnectionInfo;

        /// <summary>
        /// 是否是连接的
        /// </summary>
        public bool IsConnected => true;

        public string DeviceVersion { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version">传入"ModbusRtu"或"ModbusAscii"</param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public ModbusRtuCommunication(string version, string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout = 1500, EndianFormat format = EndianFormat.ABCD)
        {
            _ConnectionInfo = $"portName:{portName} baudRate:{baudRate} dataBits:{dataBits} stopBits:{stopBits} parity:{parity}";
            if (version == "ModbusRtu")
            {
                DeviceVersion = SerialDeviceVersion.ModbusRtu.ToString();
                client = new ModbusRtuClient(portName, baudRate, dataBits, stopBits, parity, timeout, format);
            }
            else if (version == "ModbusAscii")
            {
                DeviceVersion = SerialDeviceVersion.ModBusAscii.ToString();
                client = new ModbusAsciiClient(portName, baudRate, dataBits, stopBits, parity, timeout, format);
            }
            else
            {
                throw new Exception("ModbusRtuCommunication构造函version参数需传入ModbusRtu或ModbusAscii");
            }
        }

        public Result Close()
        {
            return client.Close();
        }

        public Result Open()
        {
            return client.Open();
        }

        /// <summary>
        /// 解析富文本地址
        /// </summary>
        /// <param name="addressRich"></param>
        /// <returns></returns>
        private Result<ModbusInput> AddressAnalysis(string addressRich)
        {
            var result = new Result<ModbusInput>();
            var address = addressRich.Split(',');
            if (addressRich.Contains("，"))
            {
                result.IsSucceed = false;
                result.Err = $"addressRich地址[{addressRich}]格式不正确。地址中的中文逗号应该修改成英文逗号。";
                result.AddErr2List();
                return result;
            }
            if (address.Length != 3)
            {
                result.IsSucceed = false;
                result.Err = $"addressRich地址[{addressRich}]格式不正确。富文本地址该依此包含站号、地址、功能码，并以英文逗号','分割，如：1,12,3";
                result.AddErr2List();
                return result;
            }
            try
            {
                result.Value = new ModbusInput()
                {
                    StationNumber = Convert.ToByte(address[0]),
                    Address = address[1],
                    FunctionCode = Convert.ToByte(address[2])
                };
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = $"addressRich地址[{addressRich}]解析异常，{ex.Message}";
                result.Exception = ex;
                result.AddErr2List();
                return result;
            }
            return result;
        }

        /// <summary>
        /// 批量读取
        /// </summary>
        /// <param name="addresses">地址</param>
        /// <param name="batchNumber">mobdus的batchNumber设置无效</param>
        /// <returns></returns>
        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            var inputs = addresses.Select(t => new ModbusInput()
            {
                StationNumber = byte.Parse(t.Key.Split(',')[0]),
                Address = t.Key.Split(',')[1],
                FunctionCode = byte.Parse(t.Key.Split(',')[2]),
                DataType = t.Value
            }).ToList();
            var tempResult = client.BatchRead(inputs);
            var result = new Result<Dictionary<string, object>>(tempResult);
            result.Value = new Dictionary<string, object>();
            if (tempResult.Value?.Any() ?? false)
            {
                foreach (var item in tempResult.Value)
                {
                    var key = $"{item.StationNumber},{item.Address},{item.FunctionCode}";
                    result.Value.Add(key, item.Value);
                }
            }
            return result;
        }

        public Result<bool> ReadBoolean(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<bool>(result);

            return client.ReadCoil(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<ushort> ReadUInt16(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<ushort>(result);

            return client.ReadUInt16(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<short> ReadInt16(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<short>(result);

            return client.ReadInt16(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<uint> ReadUInt32(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<uint>(result);

            return client.ReadUInt32(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<int> ReadInt32(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<int>(result);

            return client.ReadInt32(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<ulong> ReadUInt64(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<ulong>(result);

            return client.ReadUInt64(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<long> ReadInt64(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<long>(result);

            return client.ReadInt64(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<float> ReadFloat(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<float>(result);

            return client.ReadFloat(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result<double> ReadDouble(string addressRich)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return new Result<double>(result);

            return client.ReadDouble(result.Value.Address, result.Value.StationNumber, result.Value.FunctionCode);
        }

        /// <summary>
        /// 假的批量写入（内部循环写入）
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        public Result BatchWrite(Dictionary<string, object> addresses, int batchNumber)
        {
            var result = new Result();
            foreach (var address in addresses)
            {
                DataTypeEnum dataType = DataTypeEnum.None;
                switch (address.Value.GetType().Name)
                {
                    case "Boolean":
                        dataType = DataTypeEnum.Bool; break;
                    case "UInt16":
                        dataType = DataTypeEnum.UInt16; break;
                    case "Int16":
                        dataType = DataTypeEnum.Int16; break;
                    case "UInt32":
                        dataType = DataTypeEnum.UInt32; break;
                    case "Int32":
                        dataType = DataTypeEnum.Int32; break;
                    case "UInt64":
                        dataType = DataTypeEnum.UInt64; break;
                    case "Int64":
                        dataType = DataTypeEnum.Int64; break;
                    case "Single":
                        dataType = DataTypeEnum.Float; break;
                    case "Double":
                        dataType = DataTypeEnum.Double; break;
                    default:
                        throw new Exception($"暂未提供对{address.Value.GetType().Name}类型的写入操作。");
                }
                var tempResult = Write(address.Key, address.Value, dataType);
                if (!tempResult.IsSucceed)
                {
                    result.SetErrInfo(tempResult);
                }
                result.Requst = tempResult.Requst;
                result.Response = tempResult.Response;
            }
            return result;
        }

        public Result Write(string addressRich, byte value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, bool value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, sbyte value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, ushort value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, short value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, uint value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, int value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, ulong value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, long value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, float value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, double value)
        {
            var result = AddressAnalysis(addressRich);
            if (!result.IsSucceed)
                return result;

            return client.Write(result.Value.Address, value, result.Value.StationNumber, result.Value.FunctionCode);
        }

        public Result Write(string addressRich, object value, DataTypeEnum dataType)
        {
            var result = new Result() { IsSucceed = false };
            switch (dataType)
            {
                case DataTypeEnum.Bool:
                    result = Write(addressRich, Convert.ToBoolean(value));
                    break;
                case DataTypeEnum.Byte:
                    result = Write(addressRich, Convert.ToByte(value));
                    break;
                case DataTypeEnum.Int16:
                    result = Write(addressRich, Convert.ToInt16(value));
                    break;
                case DataTypeEnum.UInt16:
                    result = Write(addressRich, Convert.ToUInt16(value));
                    break;
                case DataTypeEnum.Int32:
                    result = Write(addressRich, Convert.ToInt32(value));
                    break;
                case DataTypeEnum.UInt32:
                    result = Write(addressRich, Convert.ToUInt32(value));
                    break;
                case DataTypeEnum.Int64:
                    result = Write(addressRich, Convert.ToInt64(value));
                    break;
                case DataTypeEnum.UInt64:
                    result = Write(addressRich, Convert.ToUInt64(value));
                    break;
                case DataTypeEnum.Float:
                    result = Write(addressRich, Convert.ToSingle(value));
                    break;
                case DataTypeEnum.Double:
                    result = Write(addressRich, Convert.ToDouble(value));
                    break;
            }
            return result;
        }
    }
}
