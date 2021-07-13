using System.ComponentModel;

namespace IoTClient.Extensions.Adapter
{
    public enum SerialDeviceVersion
    {
        [Description("ModbusRtu")]
        ModbusRtu,
        [Description("ModBusAscii")]
        ModBusAscii,
    }
}
