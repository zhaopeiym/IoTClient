namespace IoTClient.Models
{

    public class MitsubishiA1Type : IMitsubishiMCType
    {
        public MitsubishiA1Type(byte[] code, byte type, int fromBase)
        {
            TypeCode = code;
            Format = fromBase;
            if (type < 2) DataType = type;
        }

        public byte[] TypeCode { get; private set; } = { 0x00, 0x00 };
        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        public byte DataType { get; private set; } = 0x00;

        /// <summary>
        /// 指示地址是10进制，还是16进制的
        /// </summary>
        public int Format { get; private set; }

        /// <summary>
        /// X输入寄存器
        /// </summary>
        public readonly static MitsubishiA1Type X = new MitsubishiA1Type(new byte[] { 0x58, 0x20 }, 0x01, 8);
        /// <summary>
        /// Y输出寄存器
        /// </summary>
        public readonly static MitsubishiA1Type Y = new MitsubishiA1Type(new byte[] { 0x59, 0x20 }, 0x01, 8);
        /// <summary>
        /// M中间寄存器
        /// </summary>
        public readonly static MitsubishiA1Type M = new MitsubishiA1Type(new byte[] { 0x4D, 0x20 }, 0x01, 10);
        /// <summary>
        /// S状态寄存器
        /// </summary>
        public readonly static MitsubishiA1Type S = new MitsubishiA1Type(new byte[] { 0x53, 0x20 }, 0x01, 10);
        /// <summary>
        /// D数据寄存器
        /// </summary>
        public readonly static MitsubishiA1Type D = new MitsubishiA1Type(new byte[] { 0x44, 0x20 }, 0x00, 10);
        /// <summary>
        /// R文件寄存器
        /// </summary>
        public readonly static MitsubishiA1Type R = new MitsubishiA1Type(new byte[] { 0x52, 0x20 }, 0x00, 10);
    }
}
