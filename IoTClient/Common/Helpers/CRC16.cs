using System;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// CRC16验证
    /// </summary>
    public class CRC16
    {

        /// <summary>
        /// 校验CRC校验码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ch">多项式码地位</param>
        /// <param name="cl">多项式码高位</param>
        /// <returns></returns>
        public static bool CheckCRC16(byte[] value, byte ch = 0xA0, byte cl = 0x01)
        {
            if (value == null) return false;
            if (value.Length < 2) return false;

            int length = value.Length;
            byte[] buffer = new byte[length - 2];
            Array.Copy(value, 0, buffer, 0, buffer.Length);

            byte[] CRCbuf = GetCRC16(buffer, ch, cl);
            if (CRCbuf[length - 2] == value[length - 2] && CRCbuf[length - 1] == value[length - 1])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取CRC校验码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ch">多项式码地位</param>
        /// <param name="cl">多项式码高位</param>
        /// <returns>返回带CRC校验码的字节数组</returns>
        public static byte[] GetCRC16(byte[] value, byte ch = 0xA0, byte cl = 0x01)
        {
            byte[] buffer = new byte[value.Length + 2];
            value.CopyTo(buffer, 0);
            byte[] tempData = value;
            byte crc16Lo = 0xFF, crc16Hi = 0xFF, saveHi, saveLo;
            for (int i = 0; i < tempData.Length; i++)
            {
                crc16Lo = (byte)(crc16Lo ^ tempData[i]);
                for (int flag = 0; flag <= 7; flag++)
                {
                    saveHi = crc16Hi;
                    saveLo = crc16Lo;
                    crc16Hi = (byte)(crc16Hi >> 1);
                    crc16Lo = (byte)(crc16Lo >> 1);
                    if ((saveHi & 0x01) == 0x01)
                    {
                        crc16Lo = (byte)(crc16Lo | 0x80);
                    }
                    if ((saveLo & 0x01) == 0x01)
                    {
                        crc16Hi = (byte)(crc16Hi ^ ch);
                        crc16Lo = (byte)(crc16Lo ^ cl);
                    }
                }
            }
            buffer[buffer.Length - 2] = crc16Lo;
            buffer[buffer.Length - 1] = crc16Hi;
            return buffer;
        }
    }
}
