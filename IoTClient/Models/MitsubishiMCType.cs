namespace IoTClient.Models
{
    /// <summary>
    /// 三菱PLC数据类型
    /// </summary>
    public class MitsubishiMCType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param> 
        /// <param name="format">十进制或十六进制</param>
        public MitsubishiMCType(byte code, byte type, int format)
        {
            TypeCode = code;
            Format = format;
            DataType = type;
        }

        /// <summary>
        /// 类型的代号值
        /// </summary>
        public byte TypeCode { get; set; } = 0x00;

        /// <summary>
        /// 0代表按字，1代表按位
        /// </summary>
        public byte DataType { get; set; } = 0x00;

        /// <summary>
        /// 10进制或16进制
        /// </summary>
        public int Format { get; set; }

        /// <summary>
        /// X输入继电器
        /// </summary>
        public static MitsubishiMCType X = new MitsubishiMCType(0x9C, 0x01, 16);

        /// <summary>
        /// Y输出继电器
        /// </summary>
        public static MitsubishiMCType Y = new MitsubishiMCType(0x9D, 0x01, 16);

        /// <summary>
        /// M中间继电器
        /// </summary>
        public static MitsubishiMCType M = new MitsubishiMCType(0x90, 0x01, 10);

        /// <summary>
        /// D数据寄存器
        /// </summary>
        public static MitsubishiMCType D = new MitsubishiMCType(0xA8, 0x00, 10);

        /// <summary>
        /// W链接寄存器
        /// </summary>
        public static MitsubishiMCType W = new MitsubishiMCType(0xB4, 0x00, 16);

        /// <summary>
        /// L锁存继电器
        /// </summary>
        public static MitsubishiMCType L = new MitsubishiMCType(0x92, 0x01, 10);

        /// <summary>
        /// F报警器
        /// </summary>
        public static MitsubishiMCType F = new MitsubishiMCType(0x93, 0x01, 10);

        /// <summary>
        /// V边沿继电器
        /// </summary>
        public static MitsubishiMCType V = new MitsubishiMCType(0x94, 0x01, 10);

        /// <summary>
        /// B链接继电器
        /// </summary>
        public static MitsubishiMCType B = new MitsubishiMCType(0xA0, 0x01, 16);

        /// <summary>
        /// R文件寄存器
        /// </summary>
        public static MitsubishiMCType R = new MitsubishiMCType(0xAF, 0x00, 10);

        /// <summary>
        /// S步进继电器
        /// </summary>
        public static MitsubishiMCType S = new MitsubishiMCType(0x98, 0x01, 10);

        /// <summary>
        /// 变址寄存器
        /// </summary>
        public static MitsubishiMCType Z = new MitsubishiMCType(0xCC, 0x00, 10);

        /// <summary>
        /// 定时器的当前值
        /// </summary>
        public static MitsubishiMCType TN = new MitsubishiMCType(0xC2, 0x00, 10);

        /// <summary>
        /// 定时器的触点
        /// </summary>
        public static MitsubishiMCType TS = new MitsubishiMCType(0xC1, 0x01, 10);

        /// <summary>
        /// 定时器的线圈
        /// </summary>
        public static MitsubishiMCType TC = new MitsubishiMCType(0xC0, 0x01, 10);

        /// <summary>
        /// 累计定时器的触点
        /// </summary>
        public static MitsubishiMCType SS = new MitsubishiMCType(0xC7, 0x01, 10);

        /// <summary>
        /// 累计定时器的线圈
        /// </summary>
        public static MitsubishiMCType SC = new MitsubishiMCType(0xC6, 0x01, 10);

        /// <summary>
        /// 累计定时器的当前值
        /// </summary>
        public static MitsubishiMCType SN = new MitsubishiMCType(0xC8, 0x00, 100);

        /// <summary>
        /// 计数器的当前值
        /// </summary>
        public static MitsubishiMCType CN = new MitsubishiMCType(0xC5, 0x00, 10);

        /// <summary>
        /// 计数器的触点
        /// </summary>
        public static MitsubishiMCType CS = new MitsubishiMCType(0xC4, 0x01, 10);

        /// <summary>
        /// 计数器的线圈
        /// </summary>
        public static MitsubishiMCType CC = new MitsubishiMCType(0xC3, 0x01, 10);

        /// <summary>
        /// 文件寄存器ZR区
        /// </summary>
        public static MitsubishiMCType ZR = new MitsubishiMCType(0xB0, 0x00, 16);
    }
}
