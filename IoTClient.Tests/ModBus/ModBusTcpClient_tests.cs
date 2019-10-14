using IoTClient.Clients.ModBus;
using IoTServer.Common;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.ModBus
{
    public class ModBusTcpClient_tests
    {
        ModBusTcpClient client;
        byte stationNumber = 2;//站号
        public ModBusTcpClient_tests()
        {
            var ip = IPAddress.Parse("ip".GetConfig());
            var port = int.Parse("port".GetConfig());
            client = new ModBusTcpClient(new IPEndPoint(ip, port));
        }

        /// <summary>
        /// 值的写入有一定的延时，修改500毫秒后检验
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task 短连接自动开关()
        {
            short Number = 33;
            client.Write(4, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(4, stationNumber) == Number);

            Number = 34;
            client.Write(4, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(4, stationNumber) == Number);

            Number = 1;
            client.Write(12, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(12, stationNumber) == 1);
            Number = 0;
            client.Write(12, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(12, stationNumber) == 0);
        }

        [Fact]
        public async Task 长连接主动开关()
        {
            client.Open();

            short Number = 33;
            client.Write(4, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(4, stationNumber) == Number);

            Number = 34;
            client.Write(4, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(4, stationNumber) == Number);

            Number = 1;
            client.Write(12, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(12, stationNumber) == 1);
            Number = 0;
            client.Write(12, Number, stationNumber);
            await Task.Delay(500);
            Assert.True(client.ReadInt16(12, stationNumber) == 0);

            client.Close();
        }
    }
}
