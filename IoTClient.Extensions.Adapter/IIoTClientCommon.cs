using IoTClient.Enums;
using System.Collections.Generic;

namespace IoTClient.Extensions.Adapter
{
    /// <summary>
    /// IoT连接适配接口
    /// </summary>
    public interface IIoTClientCommon
    {
        /// <summary>
        /// 设备型号
        /// </summary>
        string DeviceVersion { get; }

        /// <summary>
        /// 是否是连接的
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 连接信息
        /// </summary>
        string ConnectionInfo { get; }

        /// <summary>
        /// 打开连接（如果已经是连接状态会先关闭再打开）
        /// </summary>
        /// <returns></returns>
        Result Open();

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        Result Close();

        #region Read 
        /// <summary>
        /// 分批读取
        /// </summary>
        /// <param name="addresses">地址集合</param>
        /// <param name="batchNumber">批量读取数量</param>
        /// <returns></returns>
        Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber);

        /// <summary>
        /// 读取Boolean
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<bool> ReadBoolean(string address);

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<ushort> ReadUInt16(string address);

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<short> ReadInt16(string address);

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<uint> ReadUInt32(string address);

        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<int> ReadInt32(string address);

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<ulong> ReadUInt64(string address);

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<long> ReadInt64(string address);

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<float> ReadFloat(string address);

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Result<double> ReadDouble(string address);

        #endregion

        #region Write

        /// <summary>
        /// 分批写入 
        /// </summary>
        /// <param name="addresses">地址集合</param>
        /// <param name="batchNumber">批量读取数量</param>
        /// <returns></returns>
        Result BatchWrite(Dictionary<string, object> addresses, int batchNumber);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, byte value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, bool value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, sbyte value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, ushort value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, short value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, uint value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, int value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, ulong value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, long value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, float value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Result Write(string address, double value);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        Result Write(string address, object value, DataTypeEnum type);
        #endregion
    }
}
