using IoTClient.Common.Helpers;
using IoTClient.Enums;
using System;
using System.IO.Ports;
using System.Linq;

namespace IoTClient.Clients.Modbus
{
    /// <summary>
    /// ModbusRtu协议客户端
    /// </summary>
    public class ModbusRtuClient : ModbusSerialBase, IModbusClient
    {
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
        /// <param name="plcAddresses">PLC地址</param>
        public ModbusRtuClient(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout = 1500, EndianFormat format = EndianFormat.ABCD, bool plcAddresses = false)
            : base(portName, baudRate, dataBits, stopBits, parity, timeout, format, plcAddresses)
        { }

        #region  Read 读取
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="readLength">读取长度</param>
        /// <param name="byteFormatting"></param>
        /// <returns></returns>
        public override Result<byte[]> Read(string address, byte stationNumber = 1, byte functionCode = 3, ushort readLength = 1, bool byteFormatting = true)
        {
            if (isAutoOpen) Connect();

            var result = new Result<byte[]>();
            try
            {
                //获取命令（组装报文）
                byte[] command = GetReadCommand(address, stationNumber, functionCode, readLength);
                var commandCRC16 = CRC16.GetCRC16(command);
                result.Requst = string.Join(" ", commandCRC16.Select(t => t.ToString("X2")));

                //发送命令并获取响应报文
                var sendResult = SendPackageReliable(commandCRC16);
                if (!sendResult.IsSucceed)
                    return result.SetErrInfo(sendResult).EndTime();
                var responsePackage = sendResult.Value;
                if (!responsePackage.Any())
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果为空";
                    return result.EndTime();
                }
                else if (!CRC16.CheckCRC16(responsePackage))
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果CRC16验证失败";
                    //return result.EndTime();
                }
                else if (ModbusHelper.VerifyFunctionCode(functionCode, responsePackage[1]))
                {
                    result.IsSucceed = false;
                    result.Err = ModbusHelper.ErrMsg(responsePackage[2]);
                }

                byte[] resultData = new byte[responsePackage.Length - 2 - 3];
                Array.Copy(responsePackage, 3, resultData, 0, resultData.Length);
                result.Response = string.Join(" ", responsePackage.Select(t => t.ToString("X2")));
                //4 获取响应报文数据（字节数组形式）                
                if (byteFormatting)
                    result.Value = resultData.Reverse().ToArray().ByteFormatting(format);
                else
                    result.Value = resultData.Reverse().ToArray();
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            finally
            {
                if (isAutoOpen) Dispose();
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
        public override Result Write(string address, bool value, byte stationNumber = 1, byte functionCode = 5)
        {
            if (isAutoOpen) Connect();
            var result = new Result();
            try
            {
                var command = GetWriteCoilCommand(address, value, stationNumber, functionCode);
                var commandCRC16 = CRC16.GetCRC16(command);
                result.Requst = string.Join(" ", commandCRC16.Select(t => t.ToString("X2")));
                //发送命令并获取响应报文
                var sendResult = SendPackageReliable(commandCRC16);
                if (!sendResult.IsSucceed)
                    return result.SetErrInfo(sendResult).EndTime();
                var responsePackage = sendResult.Value;

                if (!responsePackage.Any())
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果为空";
                    return result.EndTime();
                }
                else if (!CRC16.CheckCRC16(responsePackage))
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果CRC16验证失败";
                    //return result.EndTime();
                }
                else if (ModbusHelper.VerifyFunctionCode(functionCode, responsePackage[1]))
                {
                    result.IsSucceed = false;
                    result.Err = ModbusHelper.ErrMsg(responsePackage[2]);
                }
                byte[] resultBuffer = new byte[responsePackage.Length - 2];
                Buffer.BlockCopy(responsePackage, 0, resultBuffer, 0, resultBuffer.Length);
                result.Response = string.Join(" ", responsePackage.Select(t => t.ToString("X2")));
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public override Result Write(string address, byte[] values, byte stationNumber = 1, byte functionCode = 16, bool byteFormatting = true)
        {
            if (isAutoOpen) Connect();

            var result = new Result();
            try
            {
                values = values.ByteFormatting(format);
                var command = GetWriteCommand(address, values, stationNumber, functionCode);

                var commandCRC16 = CRC16.GetCRC16(command);
                result.Requst = string.Join(" ", commandCRC16.Select(t => t.ToString("X2")));
                var sendResult = SendPackageReliable(commandCRC16);
                if (!sendResult.IsSucceed)
                    return result.SetErrInfo(sendResult).EndTime();
                var responsePackage = sendResult.Value;
                if (!responsePackage.Any())
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果为空";
                    return result.EndTime();
                }
                else if (!CRC16.CheckCRC16(responsePackage))
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果CRC16验证失败";
                    //return result.EndTime();
                }
                else if (ModbusHelper.VerifyFunctionCode(functionCode, responsePackage[1]))
                {
                    result.IsSucceed = false;
                    result.Err = ModbusHelper.ErrMsg(responsePackage[2]);
                }
                byte[] resultBuffer = new byte[responsePackage.Length - 2];
                Array.Copy(responsePackage, 0, resultBuffer, 0, resultBuffer.Length);
                result.Response = string.Join(" ", responsePackage.Select(t => t.ToString("X2")));
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

        #endregion       
    }
}
