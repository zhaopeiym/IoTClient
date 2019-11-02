using System;

namespace IoTClient.Tool.Helper
{
    public static class StringExtension
    {
        /// <summary>
        /// 转出对应数据类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToDataFormType(this string str, Type type)
        {
            str = str?.Trim();
            try
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return bool.Parse(str);
                    case TypeCode.Byte:
                        return byte.Parse(str);
                    case TypeCode.Char:
                        return char.Parse(str);
                    case TypeCode.DateTime:
                        return DateTime.Parse(str);
                    case TypeCode.Decimal:
                        return decimal.Parse(str);
                    case TypeCode.Double:
                        return double.Parse(str);
                    case TypeCode.Int16:
                        return short.Parse(str);
                    case TypeCode.Int32:
                        return int.Parse(str);
                    case TypeCode.Int64:
                        return long.Parse(str);
                    case TypeCode.SByte:
                        return sbyte.Parse(str);
                    case TypeCode.Single:
                        return float.Parse(str);
                    case TypeCode.UInt16:
                        return ushort.Parse(str);
                    case TypeCode.UInt32:
                        return uint.Parse(str);
                    case TypeCode.UInt64:
                        return ulong.Parse(str);
                }
            }
            catch
            { }
            return str;
        }
    }
}
