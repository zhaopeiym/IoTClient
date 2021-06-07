using IoTClient.Enums;

namespace IoTClient.Models
{
    /// <summary>
    /// 三菱解析后的地址信息
    /// </summary>
    public class MitsubishiMCAddress
    {
        /// <summary>
        /// 开始地址
        /// </summary>
        public int BeginAddress { get; set; }

        /// <summary>
        /// 类型的代号
        /// </summary>
        public byte[] TypeCode { get; set; }

        /// <summary>
        /// 类型的代号
        /// </summary>
        public string TypeChar { get; set; }

        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        public byte BitType { get; set; }

        /// <summary>
        /// 指示地址是10进制，还是16进制的
        /// </summary>
        public int Format { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataTypeEnum DataTypeEnum { get; set; }
    }
}
