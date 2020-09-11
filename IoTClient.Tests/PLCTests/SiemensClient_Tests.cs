using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using System.Net;
using Xunit;
using IoTServer.Common;
using System.Collections.Generic;
using IoTClient.Enums;

namespace IoTClient.Tests.PLCTests
{
    public class SiemensClient_Tests
    {
        private SiemensClient client;
        public SiemensClient_Tests()
        {
            var ip = IPAddress.Parse("SiemensClientIp".GetConfig());
            var port = int.Parse("SiemensClientPort".GetConfig());
            client = new SiemensClient(SiemensVersion.S7_1500, new IPEndPoint(ip, port));
        }

        [Fact]
        public void 短连接自动开关()
        {
            var value = true;
            var result = client.Write("Q1.3", value);
            Assert.True(client.ReadBoolean("Q1.3").Value);
            value = false;
            client.Write("Q1.3", value);
            Assert.False(client.ReadBoolean("Q1.3").Value);

            short value_short = 11;
            client.Write("V2205", value_short);
            Assert.True(client.ReadInt16("V2205").Value == value_short);

            short value_short_1 = -11;
            client.Write("V2205", value_short_1);
            Assert.True(client.ReadInt16("V2205").Value == value_short_1);

            int value_int = 33;
            client.Write("V2205", value_int);
            Assert.True(client.ReadInt32("V2205").Value == value_int);

            long value_long = 44;
            client.Write("V2205", value_long);
            Assert.True(client.ReadInt64("V2205").Value == value_long);

            float value_float = 44.5f;
            client.Write("V2205", value_float);
            Assert.True(client.ReadFloat("V2205").Value == value_float);

            double value_double = 44.5d;
            client.Write("V2205", value_double);
            Assert.True(client.ReadDouble("V2205").Value == value_double);

            string value_string = "BennyZhao";
            client.Write("V2205", value_string);
            Assert.True(client.ReadString("V2205").Value == value_string);
        }

        [Fact]
        public void 长连接主动开关()
        {
            client.Open();
            var value = true;
            client.Write("Q1.3", value);
            Assert.True(client.ReadBoolean("Q1.3").Value);
            value = false;
            client.Write("Q1.3", value);
            Assert.False(client.ReadBoolean("Q1.3").Value);

            short value_short = 11;
            client.Write("V2205", value_short);
            Assert.True(client.ReadInt16("V2205").Value == value_short);

            short value_short_1 = -11;
            client.Write("V2205", value_short_1);
            Assert.True(client.ReadInt16("V2205").Value == value_short_1);

            int value_int = 33;
            client.Write("V2205", value_int);
            Assert.True(client.ReadInt32("V2205").Value == value_int);

            long value_long = 44;
            client.Write("V2205", value_long);
            Assert.True(client.ReadInt64("V2205").Value == value_long);

            float value_float = 44.5f;
            client.Write("V2205", value_float);
            Assert.True(client.ReadFloat("V2205").Value == value_float);

            double value_double = 44.5d;
            client.Write("V2205", value_double);
            Assert.True(client.ReadDouble("V2205").Value == value_double);

            string value_string = "BennyZhao";
            client.Write("V2205", value_string);
            Assert.True(client.ReadString("V2205").Value == value_string);

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

        private void test2(string address, ushort readNumber)
        {

            var reuslt2 = client.ReadBoolean(address, readNumber);

            var reuslt = client.ReadFloat("V1088", 4);
            var reuslt3 = client.ReadUInt16("V1", 5);

            var reuslt4 = client.ReadByte("V10", 5);
        }
    }
}
