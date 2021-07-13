using IoTClient.Enums;

namespace IoTClient.Clients.PLC.Models
{
    /// <summary>
    /// Omron解析后的地址信息
    /// </summary>
    public class OmronFinsAddress
    {
        /// <summary>
        /// 开始地址
        /// </summary>
        public int BeginAddress { get; set; }

        /// <summary>
        /// 类型的代号
        /// </summary>
        public string TypeChar { get; set; }

        /// <summary>
        /// 位操作
        /// </summary>
        public byte BitCode { get; set; }

        /// <summary>
        /// 字操作
        /// </summary>
        public byte WordCode { get; set; }

        /// <summary>
        /// 位操作 解析地址
        /// </summary>
        public byte[] BitAddress { get; set; }

        /// <summary>
        /// 是否是bit
        /// </summary>
        public bool IsBit { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DataTypeEnum DataTypeEnum { get; set; }
    }
}
