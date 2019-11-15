using System;
using System.Linq;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// CRC16验证
    /// </summary>
    public class CRC16
    {
        /// <summary>
        /// 验证CRC16校验码
        /// </summary>
        /// <param name="value">校验数据</param>
        /// <param name="poly">多项式码</param>
        /// <param name="crcInit">校验码初始值</param>
        /// <returns></returns>
        public static bool CheckCRC16(byte[] value, ushort poly = 0xA001, ushort crcInit = 0xFFFF)
        {
            if (value == null || !value.Any())
                throw new ArgumentException("生成CRC16的入参有误");

            var crc16 = GetCRC16(value, poly, crcInit);
            if (crc16[crc16.Length - 2] == crc16[crc16.Length - 1] && crc16[crc16.Length - 1] == 0)
                return true;
            return false;
        }

        /// <summary>
        /// 计算CRC16校验码
        /// </summary>
        /// <param name="value">校验数据</param>
        /// <param name="poly">多项式码</param>
        /// <param name="crcInit">校验码初始值</param>
        /// <returns></returns>
        public static byte[] GetCRC16(byte[] value, ushort poly = 0xA001, ushort crcInit = 0xFFFF)
        {
            if (value == null || !value.Any())
                throw new ArgumentException("生成CRC16的入参有误");

            //运算
            ushort crc = crcInit;
            for (int i = 0; i < value.Length; i++)
            {
                crc = (ushort)(crc ^ (value[i]));
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ poly) : (ushort)(crc >> 1);
                }
            }
            byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc & 0x00FF);         //低位置

            byte[] buffer = new byte[value.Length + 2];
            value.CopyTo(buffer, 0);
            buffer[buffer.Length - 1] = hi;
            buffer[buffer.Length - 2] = lo;
            return buffer;
        }
    }
}
