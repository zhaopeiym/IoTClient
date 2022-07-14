using System.ComponentModel;

namespace IoTClient.Enums
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataTypeEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None = 0,
        /// <summary>
        /// Bool
        /// </summary>
        [Description("Bool")]
        Bool = 1,
        /// <summary>
        /// Byte
        /// </summary>
        [Description("Byte")]
        Byte = 2,
        /// <summary>
        /// Int16
        /// </summary>
        [Description("Int16")]
        Int16 = 3,
        /// <summary>
        /// UInt16
        /// </summary>
        [Description("UInt16")]
        UInt16 = 4,
        /// <summary>
        /// Int32
        /// </summary>
        [Description("Int32")]
        Int32 = 5,
        /// <summary>
        /// UInt32
        /// </summary>
        [Description("UInt32")]
        UInt32 = 6,
        /// <summary>
        /// Int64
        /// </summary>
        [Description("Int64")]
        Int64 = 7,
        /// <summary>
        /// UInt64
        /// </summary>
        [Description("UInt64")]
        UInt64 = 8,
        /// <summary>
        /// Float
        /// </summary>
        [Description("Float")]
        Float = 9,
        /// <summary>
        /// Double
        /// </summary>
        [Description("Double")]
        Double = 10,
        /// <summary>
        /// String
        /// </summary>
        [Description("String")]
        String = 11,
    }
}
