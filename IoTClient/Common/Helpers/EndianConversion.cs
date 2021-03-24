using IoTClient.Enums;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// 大小端转换
    /// </summary>
    public static class EndianConversion
    {
        /// <summary>
        /// 字节格式转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] ByteFormatting(this byte[] value, EndianFormat format = EndianFormat.ABCD)
        {
            byte[] buffer = value;
            if (value.Length == 4)
            {
                buffer = new byte[4];
                switch (format)
                {
                    case EndianFormat.ABCD:
                        buffer[0] = value[0];
                        buffer[1] = value[1];
                        buffer[2] = value[2];
                        buffer[3] = value[3];
                        break;
                    case EndianFormat.BADC:
                        buffer[0] = value[1];
                        buffer[1] = value[0];
                        buffer[2] = value[3];
                        buffer[3] = value[2];
                        break;
                    case EndianFormat.CDAB:
                        buffer[0] = value[2];
                        buffer[1] = value[3];
                        buffer[2] = value[0];
                        buffer[3] = value[1];
                        break;
                    case EndianFormat.DCBA:
                        buffer[0] = value[3];
                        buffer[1] = value[2];
                        buffer[2] = value[1];
                        buffer[3] = value[0];
                        break;
                }
            }
            else if (value.Length == 8)
            {
                buffer = new byte[8];
                switch (format)
                {
                    case EndianFormat.ABCD:
                        buffer[0] = value[0];
                        buffer[1] = value[1];
                        buffer[2] = value[2];
                        buffer[3] = value[3];
                        buffer[4] = value[4];
                        buffer[5] = value[5];
                        buffer[6] = value[6];
                        buffer[7] = value[7];
                        break;
                    case EndianFormat.BADC:
                        buffer[0] = value[3];
                        buffer[1] = value[4];
                        buffer[2] = value[0];
                        buffer[3] = value[1];
                        buffer[4] = value[6];
                        buffer[5] = value[7];
                        buffer[6] = value[4];
                        buffer[7] = value[5];
                        break;
                    case EndianFormat.CDAB:
                        buffer[0] = value[4];
                        buffer[1] = value[5];
                        buffer[2] = value[6];
                        buffer[3] = value[7];
                        buffer[4] = value[0];
                        buffer[5] = value[1];
                        buffer[6] = value[2];
                        buffer[7] = value[3];
                        break;
                    case EndianFormat.DCBA:
                        buffer[0] = value[7];
                        buffer[1] = value[6];
                        buffer[2] = value[5];
                        buffer[3] = value[4];
                        buffer[4] = value[3];
                        buffer[5] = value[2];
                        buffer[6] = value[1];
                        buffer[7] = value[0];
                        break;
                }
            }
            return buffer;
        }
    }
}
