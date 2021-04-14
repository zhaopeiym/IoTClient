namespace IoTClient.Models
{
    public class MitsubishiMCWrite : MitsubishiMCData
    {
        public MitsubishiMCWrite()
        {

        }

        public MitsubishiMCWrite(MitsubishiMCData data)
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
        private void Assignment(MitsubishiMCData data)
        {
            BeginAddress = data.BeginAddress;
            MitsubishiMCType = data.MitsubishiMCType;
        }
    }
}
