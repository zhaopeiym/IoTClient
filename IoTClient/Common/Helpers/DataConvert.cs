using System;
using System.Linq;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// 数据转换
    /// </summary>
    public static class DataConvert
    {
        /// <summary>
        /// 字节数组转16进制字符
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] byteArray)
        {
            return string.Join(" ", byteArray.Select(t => t.ToString("X2")));
        }

        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str)
        {
            return str.Split(' ').Where(t => t?.Length == 2).Select(t => Convert.ToByte(t, 16)).ToArray();
        }
    }
}
