using IoTClient.Clients.PLC;
using IoTClient.Enums;
using System;
using System.Collections.Generic;
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
                client.Write("M101", bool_value);
                Assert.True(client.ReadBoolean("M101").Value == bool_value);
                client.Write("M102", bool_value);
                Assert.True(client.ReadBoolean("M102").Value == bool_value);
                client.Write("M103", !bool_value);
                Assert.True(client.ReadBoolean("M103").Value == !bool_value);
                client.Write("M104", bool_value);
                Assert.True(client.ReadBoolean("M104").Value == bool_value);
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

        [Theory]
        [InlineData(MitsubishiVersion.Qna_3E, 6000)]
        [InlineData(MitsubishiVersion.A_1E, 6001)]
        public void 批量读写(MitsubishiVersion version, int port)
        {
            client = new MitsubishiClient(version, ip, port);

            client.Open();

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            short short_number1 = (short)rnd.Next(short.MinValue, short.MaxValue);
            short short_number2 = (short)rnd.Next(short.MinValue, short.MaxValue);
            short short_number3 = (short)rnd.Next(short.MinValue, short.MaxValue);
            short short_number4 = (short)rnd.Next(short.MinValue, short.MaxValue);
            short short_number5 = (short)rnd.Next(short.MinValue, short.MaxValue);
            var bool_value = short_number1 % 2 == 1;

            client.Write("M100", !bool_value);
            client.Write("M101", !bool_value);
            client.Write("M102", bool_value);
            client.Write("M103", !bool_value);
            client.Write("M104", bool_value);

            var result = client.ReadBoolean("M100", 5);
            foreach (var item in result.Value)
            {
                if (item.Key == "M100" || item.Key == "M101" || item.Key == "M103")
                {
                    Assert.True(item.Value == !bool_value);
                }
                else
                {
                    Assert.True(item.Value == bool_value);
                }
            }

            client.Write("D100", short_number1);
            client.Write("D101", short_number2);
            client.Write("D102", short_number3);
            client.Write("D103", short_number4);
            client.Write("D104", short_number5);

            Assert.True(client.ReadInt16("D100").Value == short_number1);
            Assert.True(client.ReadInt16("D101").Value == short_number2);
            Assert.True(client.ReadInt16("D102").Value == short_number3);
            Assert.True(client.ReadInt16("D103").Value == short_number4);
            Assert.True(client.ReadInt16("D104").Value == short_number5);

            client?.Close();
        }

        [Theory]
        [InlineData(MitsubishiVersion.Qna_3E, 6000)]
        public void 批量读取(MitsubishiVersion version, int port)
        {
            client = new MitsubishiClient(version, ip, port);

            Dictionary<string, DataTypeEnum> readAddresses = new Dictionary<string, DataTypeEnum>();
            //readAddresses.Add("V2634.0", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.1", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.2", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.3", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.4", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.5", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.6", DataTypeEnum.Bool);
            //readAddresses.Add("V2634.7", DataTypeEnum.Bool);
            //readAddresses.Add("V2642", DataTypeEnum.Float);
            //readAddresses.Add("V2646", DataTypeEnum.Float);
            //readAddresses.Add("V2650", DataTypeEnum.Float);
            readAddresses.Add("D100", DataTypeEnum.Float);
            readAddresses.Add("D102", DataTypeEnum.Float);
            readAddresses.Add("D104", DataTypeEnum.Float);
            readAddresses.Add("D263", DataTypeEnum.Int16);
            readAddresses.Add("D265", DataTypeEnum.Int16);
            //readAddresses.Add("V2670", DataTypeEnum.Float);
            //readAddresses.Add("V2674", DataTypeEnum.Float);
            //readAddresses.Add("V1650", DataTypeEnum.Byte);
            //readAddresses.Add("V1651", DataTypeEnum.Byte);
            //readAddresses.Add("V1652", DataTypeEnum.Byte);

            var result = client.BatchRead(readAddresses, 10);
        }
    }
}
