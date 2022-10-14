using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static string ByteArrayToString(this byte[] byteArray)
        {
            return string.Join(" ", byteArray.Select(t => t.ToString("X2")));
        }

        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strict">严格模式（严格按两个字母间隔一个空格）</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(this string str, bool strict = true)
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

        /// <summary>
        /// Asciis字符串数组字符串装字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static byte[] AsciiStringToByteArray(this string str, bool strict = true)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Trim().Replace(" ", "").Length % 2 != 0)
                throw new ArgumentException("请传入有效的参数");

            if (strict)
            {
                List<string> stringList = new List<string>();
                foreach (var item in str.Split(' '))
                {
                    stringList.Add(((char)(Convert.ToByte(item, 16))).ToString());
                }
                return StringToByteArray(string.Join("", stringList), false);
            }
            else
            {
                str = str.Trim().Replace(" ", "");
                var stringList = new List<string>();
                for (int i = 0; i < str.Length; i++)
                {
                    var stringAscii = str[i].ToString() + str[++i].ToString();
                    stringList.Add(((char)Convert.ToByte(stringAscii, 16)).ToString());
                }
                return StringToByteArray(string.Join("", stringList), false);
            }
        }

        /// <summary>
        /// Asciis数组字符串装字节数组
        /// 如：30 31 =》 00 01
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] AsciiArrayToByteArray(this byte[] str)
        {
            if (!str?.Any() ?? true)
                throw new ArgumentException("请传入有效的参数");

            List<string> stringList = new List<string>();
            foreach (var item in str)
            {
                stringList.Add(((char)item).ToString());
            }
            return StringToByteArray(string.Join("", stringList), false);
        }

        /// <summary>
        /// 字节数组转换成Ascii字节数组
        /// 如：00 01 => 30 31
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ByteArrayToAsciiArray(this byte[] str)
        {
            return Encoding.ASCII.GetBytes(string.Join("", str.Select(t => t.ToString("X2"))));
        }

        /// <summary>
        /// Int转二进制
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minLength">补0长度</param>
        /// <returns></returns>
        public static string IntToBinaryArray(this int value, int minLength = 0)
        {
            //Convert.ToString(12,2); // 将12转为2进制字符串，结果 “1100”
            return Convert.ToString(value, 2).PadLeft(minLength, '0');
        }

        /// <summary>
        /// 二进制转Int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int BinaryArrayToInt(this string value)
        {
            //Convert.ToInt("1100",2); // 将2进制字符串转为整数，结果 12
            return Convert.ToInt32(value, 2);
        }
    }
}
