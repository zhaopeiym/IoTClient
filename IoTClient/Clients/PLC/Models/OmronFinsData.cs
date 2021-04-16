namespace IoTClient.Clients.PLC.Models
{
    public class OmronFinsData
    {
        public OmronFinsType OmronFinsType { get; set; }
        public byte[] Content { get; set; }
    }
}
