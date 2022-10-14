using IoTClient.Models;
using System.Collections.Generic;

namespace IoTClient.Clients.Modbus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModbusClient
    {
        /// <summary>
        /// 警告日志委托
        /// 为了可用性，会对异常网络进行重试。此类日志通过委托接口给出去。
        /// </summary>
        LoggerDelegate WarningLog { get; set; }

        /// <summary>
        /// 是否是连接的
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        Result Open();

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        Result Close();

        /// <summary>
        /// 发送报文，并获取响应报文
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Result<byte[]> SendPackageReliable(byte[] command);

        #region  Read 读取
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="readLength">读取长度</param>
        /// <param name="setEndian">设置构造函数中的大小端</param>
        /// <returns></returns>
        Result<byte[]> Read(string address, byte stationNumber, byte functionCode, ushort readLength, bool setEndian);

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<short> ReadInt16(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 按位的方式读取
        /// </summary>
        /// <param name="address">寄存器地址:如1.00 ... 1.14、1.15</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="left">按位取值从左边开始取</param>
        /// <returns></returns>
        Result<short> ReadInt16Bit(string address, byte stationNumber = 1, byte functionCode = 3, bool left = true);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<short> ReadInt16(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<ushort> ReadUInt16(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<ushort> ReadUInt16(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 按位的方式读取
        /// </summary>
        /// <param name="address">寄存器地址:如1.00 ... 1.14、1.15</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="left">按位取值从左边开始取</param>
        /// <returns></returns>
        Result<ushort> ReadUInt16Bit(string address, byte stationNumber = 1, byte functionCode = 3, bool left = true);

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<int> ReadInt32(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<int> ReadInt32(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<uint> ReadUInt32(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<uint> ReadUInt32(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<long> ReadInt64(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<long> ReadInt64(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<ulong> ReadUInt64(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<ulong> ReadUInt64(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<float> ReadFloat(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<float> ReadFloat(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<double> ReadDouble(string address, byte stationNumber = 1, byte functionCode = 3);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<double> ReadDouble(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取线圈
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        Result<bool> ReadCoil(string address, byte stationNumber = 1, byte functionCode = 1);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<bool> ReadCoil(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 读取离散
        /// </summary>
        /// <param name="address"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        Result<bool> ReadDiscrete(string address, byte stationNumber = 1, byte functionCode = 2);

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Result<bool> ReadDiscrete(string beginAddress, string address, byte[] values);

        /// <summary>
        /// 分批读取（批量读取，内部进行批量计算读取）
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="retryCount">如果读取异常，重试次数</param>
        /// <returns></returns>
        Result<List<ModbusOutput>> BatchRead(List<ModbusInput> addresses, uint retryCount = 1);
        #endregion

        #region Write 写入
        /// <summary>
        /// 线圈写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        Result Write(string address, bool value, byte stationNumber = 1, byte functionCode = 5);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        Result Write(string address, byte[] values, byte stationNumber = 1, byte functionCode = 16, bool byteFormatting = true);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, short value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, ushort value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, int value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, uint value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, long value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, ulong value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, float value, byte stationNumber = 1, byte functionCode = 16);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        Result Write(string address, double value, byte stationNumber = 1, byte functionCode = 16);
        #endregion
    }
}
