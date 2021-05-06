using IoTClient.Enums;

namespace IoTClient.Core.Models
{
    /// <summary>
    /// 西门子解析后的地址信息
    /// </summary>
    public class SiemensAddress
    {
        /// <summary>
        /// 原地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataTypeEnum DataType { get; set; }

        /// <summary>
        /// 区域类型
        /// </summary>
        public byte TypeCode { get; set; }
        /// <summary>
        /// DB块编号
        /// </summary>
        public ushort DbBlock { get; set; }
        /// <summary>
        /// 开始地址(西门子plc地址为8个位的长度，这里展开实际的开始地址。)
        /// </summary>
        public int BeginAddress { get; set; }
        /// <summary>
        /// 读取或写入长度
        /// </summary>
        public ushort ReadWriteLength { get; set; }
        /// <summary>
        /// 是否读取或写入bit类型
        /// </summary>
        public bool ReadWriteBit { get; set; } = false;       
    }
}
