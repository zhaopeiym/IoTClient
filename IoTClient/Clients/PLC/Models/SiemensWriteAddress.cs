using IoTClient.Core.Models;

namespace IoTClient.Models
{
    /// <summary>
    /// 西门子[写]解析后的地址信息
    /// </summary>
    public class SiemensWriteAddress : SiemensAddress
    {
        public SiemensWriteAddress(SiemensAddress data)
        {
            Assignment(data);
        }

        /// <summary>
        /// 要写入的数据
        /// </summary>
        public byte[] WriteData { get; set; }

        /// <summary>
        /// 赋值
        /// </summary>
        private void Assignment(SiemensAddress data)
        {
            Address = data.Address;
            DataType = data.DataType;
            TypeCode = data.TypeCode;
            DbBlock = data.DbBlock;
            BeginAddress = data.BeginAddress;
            ReadWriteLength = data.ReadWriteLength;
            ReadWriteBit = data.ReadWriteBit;
        }
    }
}
