using System;
using System.Collections.Generic;
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
        /// <param name="strict">严格模式（严格按两个字母间隔一个空格）</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str, bool strict = true)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Trim().Replace(" ", "").Length % 2 != 0)
                throw new ArgumentException("请传入有效的参数");

            if (strict)
            {
                return str.Split(' ').Where(t => t?.Length == 2).Select(t => Convert.ToByte(t, 16)).ToArray();
            }
            else
            {
                str = str.Trim().Replace(" ", "");
                var list = new List<byte>();
                for (int i = 0; i < str.Length; i++)
                {
                    var string16 = str[i].ToString() + str[++i].ToString();
                    list.Add(Convert.ToByte(string16, 16));
                }
                return list.ToArray();
            }
        }
    }
}
