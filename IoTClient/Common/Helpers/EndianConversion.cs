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
        /// <param name="reverse">是否经过了反转</param>
        /// <returns></returns>
        public static byte[] ByteFormatting(this byte[] value, EndianFormat format = EndianFormat.ABCD, bool reverse = true)
        {
            if (!reverse)
            {
                switch (format)
                {
                    case EndianFormat.ABCD:
                        format = EndianFormat.DCBA;
                        break;
                    case EndianFormat.BADC:
                        format = EndianFormat.CDAB;
                        break;
                    case EndianFormat.CDAB:
                        format = EndianFormat.BADC;
                        break;
                    case EndianFormat.DCBA:
                        format = EndianFormat.ABCD;
                        break;
                }
            }

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
                        buffer[0] = value[1];
                        buffer[1] = value[0];
                        buffer[2] = value[3];
                        buffer[3] = value[2];
                        buffer[4] = value[5];
                        buffer[5] = value[4];
                        buffer[6] = value[7];
                        buffer[7] = value[6];
                        break;
                    case EndianFormat.CDAB:
                        buffer[0] = value[6];
                        buffer[1] = value[7];
                        buffer[2] = value[4];
                        buffer[3] = value[5];
                        buffer[4] = value[2];
                        buffer[5] = value[3];
                        buffer[6] = value[0];
                        buffer[7] = value[1];
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

        /// <summary>
        /// 字节格式转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="reverse">是否经过了反转</param>
        /// <returns></returns>
        public static byte[] ByteFormatting2(this byte[] value, EndianFormat format = EndianFormat.ABCD, bool reverse = true)
        {
            if (!reverse)
            {
                switch (format)
                {
                    case EndianFormat.ABCD:
                        format = EndianFormat.DCBA;
                        break;
                    case EndianFormat.BADC:
                        format = EndianFormat.CDAB;
                        break;
                    case EndianFormat.CDAB:
                        format = EndianFormat.BADC;
                        break;
                    case EndianFormat.DCBA:
                        format = EndianFormat.ABCD;
                        break;
                }
            }

            byte[] buffer;
            if (value.Length == 2)
            {
                buffer = new byte[2];
                switch (format)
                {
                    case EndianFormat.BADC:
                        buffer[0] = value[1];
                        buffer[1] = value[0];
                        break;
                    default:
                        buffer = value;
                        break;
                }
            }
            else
                return ByteFormatting(value, format, true);
            return buffer;
        }
    }
}
