using IoTClient.Clients.PLC;
using System;
using System.Diagnostics;
using Xunit;

namespace IoTClient.Tests.PLC_Tests
{
    public class AllenBradleyClient_Tests
    {
        private AllenBradleyClient client;
        string ip = string.Empty;

        public AllenBradleyClient_Tests()
        {
            ip = "127.0.0.1";
        }

        [Theory]
        [InlineData(44818)]
        public void 长连接主动开关(int port)
        {
            client = new AllenBradleyClient(ip, port);

            client.Open();

            #region MyRegion

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for (int i = 0; i < 100; i++)
            {
                short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
                ushort short_number_1 = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);

                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                uint int_number_1 = (uint)rnd.Next(0, int.MaxValue);

                float float_number = rnd.Next(-100000, 100000) / 100;

                var bool_value = short_number % 2 == 1;

                client.Write("A1", bool_value);
                Assert.True(client.ReadBoolean("A1").Value == bool_value);
                client.Write("A1", !bool_value);
                Assert.True(client.ReadBoolean("A1").Value == !bool_value);

                client.Write("A1", short_number);
                Assert.True(client.ReadInt16("A1").Value == short_number);
                client.Write("A1", short_number_1);
                Assert.True(client.ReadUInt16("A1").Value == short_number_1);

                client.Write("A1", int_number);
                Assert.True(client.ReadInt32("A1").Value == int_number);
                client.Write("A1", int_number_1);
                Assert.True(client.ReadUInt32("A1").Value == int_number_1);

                client.Write("A1", Convert.ToSingle(float_number));
                Assert.True(client.ReadFloat("A1").Value == Convert.ToSingle(float_number));

            }
            #endregion

            client.Close();
        }
    }
}
