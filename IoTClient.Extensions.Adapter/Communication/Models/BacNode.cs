using System.Collections.Generic;
using System.IO.BACnet;

namespace IoTClient.Extensions.Adapter.Communication.Models
{
    public class BacNode
    {
        public BacnetAddress Address;
        public uint DeviceId;

        public List<BacProperty> Properties = new List<BacProperty>();

        public BacNode(BacnetAddress adr, uint deviceId)
        {
            this.Address = adr;
            this.DeviceId = deviceId;
        }

        public BacnetAddress GetAdd(uint deviceId)
        {
            if (this.DeviceId == deviceId)
                return Address;
            else
                return null;
        }
    }
}
