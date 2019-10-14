using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using System.Net;
using Xunit;
using IoTServer.Common;

namespace IoTClient.Tests.PLCTests
{
    public class SiemensClient_Tests
    {
        private SiemensClient client;
        public SiemensClient_Tests()
        {
            var ip = IPAddress.Parse("SiemensClientIp".GetConfig());
            var port = int.Parse("SiemensClientPort".GetConfig());
            client = new SiemensClient(SiemensVersion.S7_200Smart, new IPEndPoint(ip, port));
        }

        [Fact]
        public void 短连接自动开关()
        {
            var value = true;
            client.Write("Q1.3", value);
            Assert.True(client.ReadBoolean("Q1.3"));
            value = false;
            client.Write("Q1.3", value);
            Assert.False(client.ReadBoolean("Q1.3"));

            short value_short = 11;
            client.Write("V2205", value_short);
            Assert.True(client.ReadInt16("V2205") == value_short);

            short value_short_1 = -11;
            client.Write("V2205", value_short_1);
            Assert.True(client.ReadInt16("V2205") == value_short_1);

            int value_int = 33;
            client.Write("V2205", value_int);
            Assert.True(client.ReadInt32("V2205") == value_int);

            long value_long = 44;
            client.Write("V2205", value_long);
            Assert.True(client.ReadInt64("V2205") == value_long);

            float value_float = 44.5f;
            client.Write("V2205", value_float);
            Assert.True(client.ReadFloat("V2205") == value_float);

            double value_double = 44.5d;
            client.Write("V2205", value_double);
            Assert.True(client.ReadDouble("V2205") == value_double);

            string value_string = "BennyZhao";
            client.Write("V2205", value_string);
            Assert.True(client.ReadString("V2205") == value_string);
        }

        [Fact]
        public void 长连接主动开关()
        {
            client.Open();
            var value = true;
            client.Write("Q1.3", value);
            Assert.True(client.ReadBoolean("Q1.3"));
            value = false;
            client.Write("Q1.3", value);
            Assert.False(client.ReadBoolean("Q1.3"));

            short value_short = 11;
            client.Write("V2205", value_short);
            Assert.True(client.ReadInt16("V2205") == value_short);

            short value_short_1 = -11;
            client.Write("V2205", value_short_1);
            Assert.True(client.ReadInt16("V2205") == value_short_1);

            int value_int = 33;
            client.Write("V2205", value_int);
            Assert.True(client.ReadInt32("V2205") == value_int);

            long value_long = 44;
            client.Write("V2205", value_long);
            Assert.True(client.ReadInt64("V2205") == value_long);

            float value_float = 44.5f;
            client.Write("V2205", value_float);
            Assert.True(client.ReadFloat("V2205") == value_float);

            double value_double = 44.5d;
            client.Write("V2205", value_double);
            Assert.True(client.ReadDouble("V2205") == value_double);

            string value_string = "BennyZhao";
            client.Write("V2205", value_string);
            Assert.True(client.ReadString("V2205") == value_string);

            client?.Close();
        }
    }
}
