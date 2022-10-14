using IoTClient.Clients.PLC;
using System;
using System.Collections.Generic;
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
            ip = "OmronFinsIp".GetConfig();
        }

        [Theory]
        [InlineData(11336)]
        public void 读写测试(int port)
        {
            client = new OmronFinsClient(ip, port);

            var reuslt = client.Open();

            client.ReadInt16("D6402");

            Random rnd = new Random((int)Stopwatch.GetTimestamp());

            short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
            ushort short_number_1 = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);

            int int_number = rnd.Next(int.MinValue, int.MaxValue);
            uint int_number_1 = (uint)rnd.Next(0, int.MaxValue);

            float float_number = int_number / 100;
            var bool_value = short_number % 2 == 1;

            client.Write("D300", short_number);
            client.Write("D301", short_number_1);
            client.Write("D100", (short)100);

            Assert.True(client.ReadInt16("D100").Value == 100);
            Assert.True(client.ReadInt16("D300").Value == short_number);
            Assert.True(client.ReadUInt16("D301").Value == short_number_1);

            client.Write("D310", int_number);
            client.Write("D312", int_number_1);

            client.Write("W320", float_number);
            client.Write("W322", float_number);
            client.Write("W324", float_number);
            client.Write("W326", float_number);
            client.Write("W328", float_number);

            client.Write("W330", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W334", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W338", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W342", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W346", int_number, Enums.DataTypeEnum.Int64);

            client.Write("W400", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W408", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W412", int_number, Enums.DataTypeEnum.Int64);
            client.Write("W442", int_number, Enums.DataTypeEnum.Int64);

            client.Write("D354", int_number_1, Enums.DataTypeEnum.UInt64);
            client.Write("D358", float_number, Enums.DataTypeEnum.Double);


            var dic = new Dictionary<string, Enums.DataTypeEnum>();
            dic.Add("D300", Enums.DataTypeEnum.Int16);
            dic.Add("D301", Enums.DataTypeEnum.UInt16);

            dic.Add("D310", Enums.DataTypeEnum.Int32);
            dic.Add("D312", Enums.DataTypeEnum.UInt32);

            dic.Add("W320", Enums.DataTypeEnum.Float);
            dic.Add("W322", Enums.DataTypeEnum.Float);
            dic.Add("W324", Enums.DataTypeEnum.Float);
            dic.Add("W326", Enums.DataTypeEnum.Float);
            dic.Add("W328", Enums.DataTypeEnum.Float);

            dic.Add("W330", Enums.DataTypeEnum.Int64);
            dic.Add("W334", Enums.DataTypeEnum.Int64);
            dic.Add("W338", Enums.DataTypeEnum.Int64);
            dic.Add("W342", Enums.DataTypeEnum.Int64);
            dic.Add("W346", Enums.DataTypeEnum.Int64);
            dic.Add("W400", Enums.DataTypeEnum.Int64);
            dic.Add("W408", Enums.DataTypeEnum.Int64);
            dic.Add("W412", Enums.DataTypeEnum.Int64);
            dic.Add("W442", Enums.DataTypeEnum.Int64);

            dic.Add("D354", Enums.DataTypeEnum.UInt64);
            dic.Add("D358", Enums.DataTypeEnum.Double);

            var result = client.BatchRead(dic, 10);

            Assert.True(result.IsSucceed);

            Assert.True(Convert.ToInt16(result.Value["D300"]) == short_number);
            Assert.True(Convert.ToUInt16(result.Value["D301"]) == short_number_1);

            Assert.True(Convert.ToInt32(result.Value["D310"]) == int_number);
            Assert.True(Convert.ToUInt32(result.Value["D312"]) == int_number_1);

            Assert.True(Convert.ToSingle(result.Value["W320"]) == float_number);
            Assert.True(Convert.ToSingle(result.Value["W322"]) == float_number);
            Assert.True(Convert.ToSingle(result.Value["W324"]) == float_number);
            Assert.True(Convert.ToSingle(result.Value["W326"]) == float_number);
            Assert.True(Convert.ToSingle(result.Value["W328"]) == float_number);

            Assert.True(Convert.ToInt64(result.Value["W330"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W334"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W338"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W342"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W346"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W400"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W408"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W412"]) == Convert.ToInt64(int_number));
            Assert.True(Convert.ToInt64(result.Value["W442"]) == Convert.ToInt64(int_number));

            Assert.True(Convert.ToUInt64(result.Value["D354"]) == Convert.ToUInt64(int_number_1));
            Assert.True(Convert.ToDouble(result.Value["D358"]) == Convert.ToDouble(float_number));

            client.Write("H400.15", false);
            Assert.True(client.ReadBoolean("H400.15").Value == false);
            client.Write("H400.15", true);
            Assert.True(client.ReadBoolean("H400.15").Value == true);

            client.Close();
        }

        [Theory]
        [InlineData(11336)]
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
