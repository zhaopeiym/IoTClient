using IoTClient.Clients.PLC;
using IoTClient.Enums;
using IoTClient.Interfaces;
using System.Collections.Generic;

namespace IoTClient.Extensions.Adapter
{
    public class AllenBradleyClientCommunication : IIoTClientCommon
    {
        private IEthernetClient client;

        public string DeviceVersion => EthernetDeviceVersion.AllenBradley.ToString();

        public bool IsConnected => client.Connected;

        public string ConnectionInfo => client.IpEndPoint.ToString();

        public AllenBradleyClientCommunication(string ip, int port, int timeout = 1500)
        {
            client = new AllenBradleyClient(ip, port, timeout: timeout);
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
