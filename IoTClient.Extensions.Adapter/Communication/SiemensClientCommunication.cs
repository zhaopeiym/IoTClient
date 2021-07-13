using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using IoTClient.Enums;
using IoTClient.Interfaces;
using System.Collections.Generic;

namespace IoTClient.Extensions.Adapter
{
    /// <summary>
    /// SiemensClient
    /// </summary>
    public class SiemensClientCommunication : IIoTClientCommon
    {
        private IEthernetClient client;

        public string DeviceVersion { get; }

        public bool IsConnected => client.Connected;

        public string ConnectionInfo => client.IpEndPoint.ToString();

        public SiemensClientCommunication(SiemensVersion version, string ip, int port, byte slot = 0, byte rack = 0, int timeout = 1500)
        {
            switch (version)
            {
                case SiemensVersion.S7_200:
                    DeviceVersion = EthernetDeviceVersion.Siemens_S7_200.ToString();
                    break;
                case SiemensVersion.S7_200Smart:
                    DeviceVersion = EthernetDeviceVersion.Siemens_S7_200Smart.ToString();
                    break;
                case SiemensVersion.S7_300:
                    DeviceVersion = EthernetDeviceVersion.Siemens_S7_300.ToString();
                    break;
                case SiemensVersion.S7_400:
                    DeviceVersion = EthernetDeviceVersion.Siemens_S7_400.ToString();
                    break;
                case SiemensVersion.S7_1200:
                    DeviceVersion = EthernetDeviceVersion.Siemens_S7_1200.ToString();
                    break;
                case SiemensVersion.S7_1500:
                    DeviceVersion = EthernetDeviceVersion.Siemens_S7_1500.ToString();
                    break;
            }
            client = new SiemensClient(version, ip, port, slot, rack, timeout: timeout);
        }

        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            return client.BatchRead(addresses, batchNumber);
        }

        public Result BatchWrite(Dictionary<string, object> addresses, int batchNumber)
        {
            return client.BatchWrite(addresses, batchNumber);
        }

        public Result Close()
        {
            return client.Close();
        }

        public Result Open()
        {
            return client.Open();
        }

        public Result<bool> ReadBoolean(string address)
        {
            return client.ReadBoolean(address);
        }

        public Result<double> ReadDouble(string address)
        {
            return client.ReadDouble(address);
        }

        public Result<float> ReadFloat(string address)
        {
            return client.ReadFloat(address);
        }

        public Result<short> ReadInt16(string address)
        {
            return client.ReadInt16(address);
        }

        public Result<int> ReadInt32(string address)
        {
            return client.ReadInt32(address);
        }

        public Result<long> ReadInt64(string address)
        {
            return client.ReadInt64(address);
        }

        public Result<ushort> ReadUInt16(string address)
        {
            return client.ReadUInt16(address);
        }

        public Result<uint> ReadUInt32(string address)
        {
            return client.ReadUInt32(address);
        }

        public Result<ulong> ReadUInt64(string address)
        {
            return client.ReadUInt64(address);
        }

        public Result Write(string address, byte value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, bool value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, sbyte value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, ushort value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, short value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, uint value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, int value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, ulong value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, long value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, float value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, double value)
        {
            return client.Write(address, value);
        }

        public Result Write(string address, object value, DataTypeEnum dataType)
        {
            return client.Write(address, value, dataType);
        }
    }
}
