using IoTServer.Common;
using IoTServer.Servers.Modbus;
using IoTServer.Servers.PLC;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.Server_tests
{
    public class ModbusTcpServer_test
    {
        public ModbusTcpServer_test()
        {
            DataPersist.LoadData();
        }

        [Fact]
        public async Task StartAsync()
        {
            ModbusTcpServer server = new ModbusTcpServer(int.Parse("LocalPort".GetConfig()));

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
