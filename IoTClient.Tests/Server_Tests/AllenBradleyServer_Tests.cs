using IoTServer.Servers.PLC;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.Server_Tests
{
    public class AllenBradleyServer_Tests
    {
        [Fact]
        public async Task StartAsync()
        {
            AllenBradleyServer server = new AllenBradleyServer(44818);

            server.Start();

            await Task.Delay(1000 * 1000);
        }
    }
}
