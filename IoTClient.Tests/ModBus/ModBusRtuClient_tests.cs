using IoTClient.Clients.ModBus;
using System.IO.Ports;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.ModBus
{
    public class ModBusRtuClient_tests
    {
        private ModBusRtuClient client;
        byte stationNumber = 1;//站号
        public ModBusRtuClient_tests()
        {
            client = new ModBusRtuClient("COM3", 9600, 8, StopBits.One, Parity.None);
        }

        [Fact]
        public async Task 短连接自动开关()
        {
            short Number = 33;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 34;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 1;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 1);

            Number = 0;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 0);

            int numberInt32 = -12;
            client.Write("4", numberInt32, stationNumber);
            Assert.True(client.ReadInt32("4", stationNumber).Value == numberInt32);

            float numberFloat = 112;
            client.Write("4", numberFloat, stationNumber);
            Assert.True(client.ReadFloat("4", stationNumber).Value == numberFloat);

            double numberDouble = 32;
            client.Write("4", numberDouble, stationNumber);
            Assert.True(client.ReadDouble("4", stationNumber).Value == numberDouble);
        }

        [Fact]
        public async Task 长连接主动开关()
        {
            client.Open();

            short Number = 33;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 34;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 1;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 1);

            Number = 0;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 0);

            int numberInt32 = -12;
            client.Write("4", numberInt32, stationNumber);
            Assert.True(client.ReadInt32("4", stationNumber).Value == numberInt32);

            float numberFloat = 112;
            client.Write("4", numberFloat, stationNumber);
            Assert.True(client.ReadFloat("4", stationNumber).Value == numberFloat);

            double numberDouble = 32;
            client.Write("4", numberDouble, stationNumber);
            Assert.True(client.ReadDouble("4", stationNumber).Value == numberDouble);

            client.Close();
        }
    }
}
