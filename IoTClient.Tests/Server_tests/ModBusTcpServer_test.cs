using IoTServer.Common;
using IoTServer.Servers.ModBus;
using IoTServer.Servers.PLC;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.Server_tests
{
    public class ModBusTcpServer_test
    {
        public ModBusTcpServer_test()
        {
            DataPersist.LoadData();
        }

        [Fact]
        public async Task StartAsync()
        {
            ModBusTcpServer server = new ModBusTcpServer(int.Parse("LocalPort".GetConfig()));

            server.Start();

            await Task.Delay(1000 * 1000);
        }

        [Fact]
        public async Task SiemensServerAsync()
        {
            SiemensServer server = new SiemensServer(102);
            server.Start();

            await Task.Delay(1000 * 1000);
        }
    }
}
