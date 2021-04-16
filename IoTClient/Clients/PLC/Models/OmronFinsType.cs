namespace IoTClient.Clients.PLC.Models
{
    public class OmronFinsType
    {

        public OmronFinsType(byte bitCode, byte wordCode)
        {
            BitCode = bitCode;
            WordCode = wordCode;
        } 

        /// <summary>
        /// 位操作
        /// </summary>
        public byte BitCode { get; private set; }

        /// <summary>
        /// 字操作
        /// </summary>
        public byte WordCode { get; private set; } 

        public static readonly OmronFinsType DM = new OmronFinsType(0x02, 0x82);

        public static readonly OmronFinsType CIO = new OmronFinsType(0x30, 0xB0);

        public static readonly OmronFinsType WR = new OmronFinsType(0x31, 0xB1);

        public static readonly OmronFinsType HR = new OmronFinsType(0x32, 0xB2);

        public static readonly OmronFinsType AR = new OmronFinsType(0x33, 0xB3);
    }
}
