using IoTClient.Enums;

namespace IoTClient.Core.Models
{
    public class SiemensData
    {
        /// <summary>
        /// 原地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 区域类型
        /// </summary>
        public byte TypeCode { get; set; }
        /// <summary>
        /// DB块编号
        /// </summary>
        public ushort DbBlock { get; set; }
        /// <summary>
        /// 开始地址
        /// </summary>
        public int BeginAddress { get; set; }
        /// <summary>
        /// 读取长度
        /// </summary>
        public ushort ReadLength { get; set; }
        /// <summary>
        /// 是否读取bit类型
        /// </summary>
        public bool ReadBit { get; set; } = false;
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataTypeEnum DataType { get; set; }
    }
}
