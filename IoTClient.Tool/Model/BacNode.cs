using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTClient.Tool
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
