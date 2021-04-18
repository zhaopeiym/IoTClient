using IoTClient.Clients.PLC;
using System;
using System.Diagnostics;
using Xunit;

namespace IoTClient.Tests.PLCTests
{
    public class OmronFinsClient_Tests
    {
        private OmronFinsClient client;
        string ip = string.Empty;

        public OmronFinsClient_Tests()
        {
            ip = "OmronFinsClientIp".GetConfig();
        }

        [Theory]
        [InlineData(9600)]
        [InlineData(9601)]
        public void 长连接主动开关(int port)
        {
            client = new OmronFinsClient(ip, port);

            client.Open();

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for (int i = 0; i < 100; i++)
            {               
                short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
                ushort short_number_1 = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);

                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                uint int_number_1 = (uint)rnd.Next(0, int.MaxValue);

                float float_number = int_number / 100;
                var bool_value = short_number % 2 == 1;

                client.Write("D100", bool_value);
                Assert.True(client.ReadBoolean("D100").Value == bool_value);
                client.Write("D101", bool_value);
                Assert.True(client.ReadBoolean("D101").Value == bool_value);
                client.Write("D102", !bool_value);
                Assert.True(client.ReadBoolean("D102").Value == !bool_value);

                client.Write("D100", short_number);
                Assert.True(client.ReadInt16("D100").Value == short_number);
                client.Write("D100", short_number_1);
                Assert.True(client.ReadUInt16("D100").Value == short_number_1);

                client.Write("D100", int_number);
                Assert.True(client.ReadInt32("D100").Value == int_number);
                client.Write("D100", int_number_1);
                Assert.True(client.ReadUInt32("D100").Value == int_number_1);

                client.Write("D100", Convert.ToInt64(int_number));
                Assert.True(client.ReadInt64("D100").Value == Convert.ToInt64(int_number));
                client.Write("D100", Convert.ToUInt64(int_number_1));
                Assert.True(client.ReadUInt64("D100").Value == Convert.ToUInt64(int_number_1));

                client.Write("D200", float_number);
                Assert.True(client.ReadFloat("D200").Value == float_number);
                client.Write("D300", Convert.ToDouble(float_number));
                Assert.True(client.ReadDouble("D300").Value == Convert.ToDouble(float_number));
            }

            client.Close();
        }
    }
}
