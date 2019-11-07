using System;
using System.Linq;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// LRC验证
    /// </summary>
    public class LRC
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetLRC(byte[] value)
        {
            if (value == null) return null;

            int sum = 0;
            for (int i = 0; i < value.Length; i++)
            {
                sum += value[i];
            }

            sum = sum % 256;
            sum = 256 - sum;

            byte[] LRC = new byte[] { (byte)sum };
            return value.Concat(LRC).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CheckLRC(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("参数为null");

            int length = value.Length;
            byte[] buffer = new byte[length - 1];
            Array.Copy(value, 0, buffer, 0, buffer.Length);

            byte[] LRCbuf = GetLRC(buffer);
            if (LRCbuf[length - 1] == value[length - 1])
            {
                return true;
            }
            return false;
        }
    }
}
