using IoTServer.Common;
using IoTServer.Servers.ModBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.Server_tests
{
    public class ModBusTcpServer_test
    {
        [Fact]
        public async Task StartAsync()
        {
            ModBusTcpServer server = new ModBusTcpServer("LocalIP".GetConfig(), int.Parse("LocalPort".GetConfig()));

            server.Start();

            await Task.Delay(1000 * 1000);
        }
    }
}
