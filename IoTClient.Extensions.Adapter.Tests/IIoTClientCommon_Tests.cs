using System;
using Xunit;

namespace IoTClient.Extensions.Adapter.Tests
{
    public class IIoTClientCommon_Tests
    {
        private IIoTClientCommon ioTClientCommon;

        public IIoTClientCommon_Tests()
        {

        }
        [Fact]
        public void Test1()
        {
            ioTClientCommon = new ModbusTcpCommunication("", "127.0.0.1", 502);

            var result = ioTClientCommon.ReadInt16("1_12_3");
        }
    }
}
