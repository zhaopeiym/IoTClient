using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using IoTClient.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Xunit;

namespace IoTClient.Tests.PLCTests
{
    public class SiemensClient_Tests
    {
        private SiemensClient client;
        public SiemensClient_Tests()
        {
            var ip = IPAddress.Parse("SiemensClientIp".GetConfig());
            var port = int.Parse("SiemensClientPort".GetConfig());
            ip = IPAddress.Parse("127.0.0.1");//20.205.243.166
            port = 102;
            client = new SiemensClient(SiemensVersion.S7_200Smart, new IPEndPoint(ip, port));
        }

        [Fact]
        public void 短连接自动开关()
        {
            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for (int i = 0; i < 100; i++)
            {
                short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
                ushort short_number_1 = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);

                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                uint int_number_1 = (uint)rnd.Next(0, int.MaxValue);

                float float_number = int_number / 100;
                var bool_value = short_number % 2 == 1;

                string value_string = "BennyZhao" + float_number;

                client.Write("Q1.3", bool_value);
                Assert.True(client.ReadBoolean("Q1.3").Value == bool_value);
                client.Write("Q1.4", bool_value);
                Assert.True(client.ReadBoolean("Q1.4").Value == bool_value);
                client.Write("Q1.5", !bool_value);
                Assert.True(client.ReadBoolean("Q1.5").Value == !bool_value);

                client.Write("V100", short_number);
                Assert.True(client.ReadInt16("V100").Value == short_number);
                client.Write("V100", short_number_1);
                Assert.True(client.ReadUInt16("V100").Value == short_number_1);

                client.Write("V100", int_number);
                Assert.True(client.ReadInt32("V100").Value == int_number);
                client.Write("V100", int_number_1);
                Assert.True(client.ReadUInt32("V100").Value == int_number_1);

                client.Write("V100", Convert.ToInt64(int_number));
                Assert.True(client.ReadInt64("V100").Value == Convert.ToInt64(int_number));
                client.Write("V100", Convert.ToUInt64(int_number_1));
                Assert.True(client.ReadUInt64("V100").Value == Convert.ToUInt64(int_number_1));

                client.Write("V200", float_number);
                Assert.True(client.ReadFloat("V200").Value == float_number);
                client.Write("V300", Convert.ToDouble(float_number));
                Assert.True(client.ReadDouble("V300").Value == Convert.ToDouble(float_number));

                client.Write("V2205", value_string);
                Assert.True(client.ReadString("V2205").Value == value_string);
            }
        }

        [Fact]
        public void 长连接主动开关()
        {
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

                string value_string = "BennyZhao" + float_number;

                client.Write("Q1.3", bool_value);
                Assert.True(client.ReadBoolean("Q1.3").Value == bool_value);
                client.Write("Q1.4", bool_value);
                Assert.True(client.ReadBoolean("Q1.4").Value == bool_value);
                client.Write("Q1.5", !bool_value);
                Assert.True(client.ReadBoolean("Q1.5").Value == !bool_value);

                client.Write("V100", short_number);
                Assert.True(client.ReadInt16("V100").Value == short_number);
                client.Write("V100", short_number_1);
                Assert.True(client.ReadUInt16("V100").Value == short_number_1);

                client.Write("V100", int_number);
                Assert.True(client.ReadInt32("V100").Value == int_number);
                client.Write("V100", int_number_1);
                Assert.True(client.ReadUInt32("V100").Value == int_number_1);

                client.Write("V100", Convert.ToInt64(int_number));
                Assert.True(client.ReadInt64("V100").Value == Convert.ToInt64(int_number));
                client.Write("V100", Convert.ToUInt64(int_number_1));
                Assert.True(client.ReadUInt64("V100").Value == Convert.ToUInt64(int_number_1));

                client.Write("V200", float_number);
                Assert.True(client.ReadFloat("V200").Value == float_number);
                client.Write("V300", Convert.ToDouble(float_number));
                Assert.True(client.ReadDouble("V300").Value == Convert.ToDouble(float_number));

                client.Write("V2205", value_string);
                Assert.True(client.ReadString("V2205").Value == value_string);
            }

            client?.Close();
        }

        [Fact]
        public void 批量读写()
        {
            client.Open();

            client.WarningLog = (msg, ex) =>
            {
                string aa = msg;
            };

            var re = new Random(DateTime.Now.Second);

            var number0 = re.Next(0, 255) % 2 == 1;
            var number1 = re.Next(0, 255) % 2 == 1;
            var number2 = re.Next(0, 255) % 2 == 1;
            var number3 = re.Next(0, 255) % 2 == 1;
            var number4 = re.Next(0, 255) % 2 == 1;
            var number5 = re.Next(0, 255) % 2 == 1;
            var number6 = re.Next(0, 255) % 2 == 1;
            var number7 = re.Next(0, 255) % 2 == 1;
            byte byte1 = (byte)re.Next(0, 255);
            byte byte2 = (byte)re.Next(0, 255);
            byte byte3 = (byte)re.Next(0, 255);
            var V2642 = (float)re.Next(0, 255);
            var V2646 = (float)re.Next(0, 255);
            var V2650 = (float)re.Next(0, 255);

            Dictionary<string, object> writeAddresses = new Dictionary<string, object>();
            writeAddresses.Add("V2634.0", number0);
            writeAddresses.Add("V2634.1", number1);
            writeAddresses.Add("V2634.2", number2);
            writeAddresses.Add("V2634.3", number3);
            writeAddresses.Add("V2634.4", number4);
            writeAddresses.Add("V2634.5", number5);
            writeAddresses.Add("V2634.6", number6);
            writeAddresses.Add("V2634.7", number7);
            writeAddresses.Add("V2642", V2642);
            writeAddresses.Add("V2646", V2646);
            writeAddresses.Add("V2650", V2650);
            writeAddresses.Add("V2654", V2650);
            writeAddresses.Add("V2658", V2650);
            writeAddresses.Add("V2662", V2650);
            writeAddresses.Add("V2666", V2650);
            writeAddresses.Add("V2670", V2650);
            writeAddresses.Add("V2674", V2650);
            writeAddresses.Add("V1650", byte1);
            writeAddresses.Add("V1651", byte2);
            writeAddresses.Add("V1652", byte3);
            client.BatchWrite(writeAddresses);

            Dictionary<string, DataTypeEnum> readAddresses = new Dictionary<string, DataTypeEnum>();
            readAddresses.Add("V2634.0", DataTypeEnum.Bool);
            readAddresses.Add("V2634.1", DataTypeEnum.Bool);
            readAddresses.Add("V2634.2", DataTypeEnum.Bool);
            readAddresses.Add("V2634.3", DataTypeEnum.Bool);
            readAddresses.Add("V2634.4", DataTypeEnum.Bool);
            readAddresses.Add("V2634.5", DataTypeEnum.Bool);
            readAddresses.Add("V2634.6", DataTypeEnum.Bool);
            readAddresses.Add("V2634.7", DataTypeEnum.Bool);
            readAddresses.Add("V2642", DataTypeEnum.Float);
            readAddresses.Add("V2646", DataTypeEnum.Float);
            readAddresses.Add("V2650", DataTypeEnum.Float);
            readAddresses.Add("V2654", DataTypeEnum.Float);
            readAddresses.Add("V2658", DataTypeEnum.Float);
            readAddresses.Add("V2662", DataTypeEnum.Float);
            readAddresses.Add("V2666", DataTypeEnum.Float);
            readAddresses.Add("V2670", DataTypeEnum.Float);
            readAddresses.Add("V2674", DataTypeEnum.Float);
            readAddresses.Add("V1650", DataTypeEnum.Byte);
            readAddresses.Add("V1651", DataTypeEnum.Byte);
            readAddresses.Add("V1652", DataTypeEnum.Byte);

            var result = client.BatchRead(readAddresses);

            Assert.True(Convert.ToBoolean(result.Value["V2634.0"]) == number0);
            Assert.True(Convert.ToBoolean(result.Value["V2634.1"]) == number1);
            Assert.True(Convert.ToBoolean(result.Value["V2634.2"]) == number2);
            Assert.True(Convert.ToBoolean(result.Value["V2634.3"]) == number3);
            Assert.True(Convert.ToBoolean(result.Value["V2634.4"]) == number4);
            Assert.True(Convert.ToBoolean(result.Value["V2634.5"]) == number5);
            Assert.True(Convert.ToBoolean(result.Value["V2634.6"]) == number6);
            Assert.True(Convert.ToBoolean(result.Value["V2634.7"]) == number7);
            Assert.True(Convert.ToSingle(result.Value["V2642"]) == V2642);
            Assert.True(Convert.ToSingle(result.Value["V2646"]) == V2646);
            Assert.True(Convert.ToSingle(result.Value["V2650"]) == V2650);
            Assert.True(Convert.ToSingle(result.Value["V2654"]) == V2650);
            Assert.True(Convert.ToSingle(result.Value["V2658"]) == V2650);
            Assert.True(Convert.ToSingle(result.Value["V2662"]) == V2650);
            Assert.True(Convert.ToSingle(result.Value["V2666"]) == V2650);
            Assert.True(Convert.ToSingle(result.Value["V2670"]) == V2650);
            Assert.True(Convert.ToSingle(result.Value["V2674"]) == V2650);
            Assert.True(Convert.ToByte(result.Value["V1650"]) == byte1);
            Assert.True(Convert.ToByte(result.Value["V1651"]) == byte2);
            Assert.True(Convert.ToByte(result.Value["V1652"]) == byte3);
            client?.Close();
        }

        [Fact]
        public void test()
        {
            //string address = "I1.1";
            //ushort readNumber = 20;
            //test2(address, readNumber);

            //TODO 最多只能批量读取 19个？
            Dictionary<string, DataTypeEnum> addresses = new Dictionary<string, DataTypeEnum>();

            //addresses.Add("V1000", DataTypeEnum.Float);
            //addresses.Add("I0.0", DataTypeEnum.Bool);
            //addresses.Add("V4109", DataTypeEnum.Byte);
            //addresses.Add("V1004", DataTypeEnum.Float);

            //addresses.Add("V1000", DataTypeEnum.Float);
            //addresses.Add("V1004", DataTypeEnum.Float);
            //addresses.Add("V1008", DataTypeEnum.Float);
            //addresses.Add("V1012", DataTypeEnum.Float);
            //addresses.Add("V1016", DataTypeEnum.Float);
            //addresses.Add("V1020", DataTypeEnum.Float);
            //addresses.Add("V1024", DataTypeEnum.Float);
            //addresses.Add("V1032", DataTypeEnum.Float);
            //addresses.Add("V1036", DataTypeEnum.Float);
            //addresses.Add("V1040", DataTypeEnum.Float);
            //addresses.Add("V1044", DataTypeEnum.Float);
            //addresses.Add("V1048", DataTypeEnum.Float);
            //addresses.Add("V1052", DataTypeEnum.Float);
            //addresses.Add("V1230", DataTypeEnum.Float);
            //addresses.Add("V1234", DataTypeEnum.Float);
            //addresses.Add("V1238", DataTypeEnum.Float);
            //addresses.Add("V1242", DataTypeEnum.Float);
            //addresses.Add("V1246", DataTypeEnum.Float);
            //addresses.Add("V1250", DataTypeEnum.Float);

            //addresses.Add("V1254", DataTypeEnum.Float);
            //addresses.Add("V1258", DataTypeEnum.Float);


            //addresses.Add("V1012", DataTypeEnum.Float);
            //addresses.Add("V1076 ", DataTypeEnum.UInt32);
            //addresses.Add("V5056 ", DataTypeEnum.Float);
            //addresses.Add("V5232 ", DataTypeEnum.Float);         

            //addresses.Add("I0.0 ", DataTypeEnum.Bool);
            //addresses.Add("I0.1 ", DataTypeEnum.Bool);
            //addresses.Add("I0.2 ", DataTypeEnum.Bool);
            //addresses.Add("I0.3 ", DataTypeEnum.Bool);
            //addresses.Add("I0.4 ", DataTypeEnum.Bool);
            //addresses.Add("I0.5 ", DataTypeEnum.Bool);
            //addresses.Add("I0.6 ", DataTypeEnum.Bool);
            //addresses.Add("I0.7 ", DataTypeEnum.Bool);

            //addresses.Add("I1.0 ", DataTypeEnum.Bool);
            //addresses.Add("I1.1 ", DataTypeEnum.Bool);
            //addresses.Add("I1.2 ", DataTypeEnum.Bool);
            //addresses.Add("I1.3 ", DataTypeEnum.Bool);
            //addresses.Add("I1.4 ", DataTypeEnum.Bool);
            //addresses.Add("I1.5 ", DataTypeEnum.Bool);
            //addresses.Add("I1.6 ", DataTypeEnum.Bool);
            //addresses.Add("I1.7 ", DataTypeEnum.Bool);


            //client.Write("DB4.0", (float)6);
            //client.Write("DB4.12", (float)9);
            //client.Write("DB1.410.0", false);
            //client.Write("DB1.410.0", true);

            var result = client.BatchRead(addresses);

            Dictionary<string, object> newAddresses = new Dictionary<string, object>();
            newAddresses.Add("DB4.24", (float)1);
            newAddresses.Add("DB4.0", (float)2);
            newAddresses.Add("DB1.434.0", true);
            newAddresses.Add("DB1.482.0", true);
            newAddresses.Add("DB4.12", (float)3);
            newAddresses.Add("DB1.410.0", true);
            var result1 = client.BatchWrite(newAddresses);

            var r3 = client.Write("DB1.482.0", false);
            var result2 = client.Write("DB1.434.0", false);
            client.Write("DB1.434.0", true);
        }

        [Fact]
        public void 定时更新()
        {
            client.ReadInt16("V100", (value,isSucceed,err) =>
            {
                Debug.WriteLine($"V100:{value}  err:{err}");
            });

            client.ReadInt16("V102", (value, isSucceed, err) =>
            {
                Debug.WriteLine($"V102:{value}  err:{err}");
            });

            Thread.Sleep(1000 * 30);
        }
    }
}
