using IoTClient.Clients.PLC;
using IoTClient.Enums;
using System;
using System.Diagnostics;
using Xunit;

namespace IoTClient.Tests.PLCTests
{
    public class MitsubishiClient_Tests
    {
        private MitsubishiClient client;
        string ip = string.Empty;

        public MitsubishiClient_Tests()
        {
            ip = "MitsubishiClientIp".GetConfig();
        }

        [Theory]
        [InlineData(MitsubishiVersion.Qna_3E, 6000)]
        [InlineData(MitsubishiVersion.A_1E, 6001)]
        public void 短连接自动开关(MitsubishiVersion version, int port)
        {
            client = new MitsubishiClient(version, ip, port);

            ReadWrite();
        }

        [Theory]
        [InlineData(MitsubishiVersion.Qna_3E, 6000)]
        [InlineData(MitsubishiVersion.A_1E, 6001)]
        public void 长连接主动开关(MitsubishiVersion version, int port)
        {
            client = new MitsubishiClient(version, ip, port);

            client.Open();

            ReadWrite();

            client?.Close();
        }

        private void ReadWrite()
        {
            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for (int i = 0; i < 10; i++)
            {
                short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                float float_number = int_number / 100;
                var bool_value = short_number % 2 == 1;
                client.Write("X100", bool_value);
                Assert.True(client.ReadBoolean("X100").Value == bool_value);
                client.Write("Y100", !bool_value);
                Assert.True(client.ReadBoolean("Y100").Value == !bool_value);
                client.Write("M100", !bool_value);
                Assert.True(client.ReadBoolean("M100").Value == !bool_value);
                //client.Write("L100", !bool_value);
                //Assert.True(client.ReadBoolean("L100").Value == !bool_value);
                //client.Write("F100", bool_value);
                //Assert.True(client.ReadBoolean("F100").Value == bool_value);
                //client.Write("V100", !bool_value);
                //Assert.True(client.ReadBoolean("V100").Value == !bool_value);
                //client.Write("B100", bool_value);
                //Assert.True(client.ReadBoolean("B100").Value == bool_value);
                //client.Write("S100", bool_value);
                //Assert.True(client.ReadBoolean("S100").Value == bool_value);

                client.Write("D200", short_number);
                Assert.True(client.ReadInt16("D200").Value == short_number);

                client.Write("D200", int_number);
                Assert.True(client.ReadInt32("D200").Value == int_number);

                client.Write("D200", Convert.ToInt64(int_number));
                Assert.True(client.ReadInt64("D200").Value == Convert.ToInt64(int_number));

                client.Write("D200", float_number);
                Assert.True(client.ReadFloat("D200").Value == float_number);

                client.Write("D200", Convert.ToDouble(float_number));
                Assert.True(client.ReadDouble("D200").Value == Convert.ToDouble(float_number));

                //client.Write("W400", short_number);
                //Assert.True(client.ReadInt16("W400").Value == short_number);

                //client.Write("R200", r_number);
                //Assert.True(client.ReadInt16("R200").Value == r_number);
                Debug.WriteLine(short_number);
            }
        }

        [Fact]
        public void 批量读写()
        {
            client.Open();

            client?.Close();
        }
    }
}
