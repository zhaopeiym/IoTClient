using IoTClient.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace IoTClient.Extensions.Adapter.Tests
{
    public class IoTClientFactory_Tests
    {
        private IIoTClientCommon client = null;

        [Theory]
        [InlineData(EthernetDeviceVersion.ModbusTcp, "127.0.0.1", 502)]
        [InlineData(EthernetDeviceVersion.ModbusRtuOverTcp, "127.0.0.1", 503)]
        [InlineData(EthernetDeviceVersion.Siemens_S7_1200, "127.0.0.1", 102)]
        [InlineData(EthernetDeviceVersion.OmronFins, "127.0.0.1", 9601)]
        [InlineData(EthernetDeviceVersion.Mitsubishi_Qna_3E, "127.0.0.1", 6000)]
        [InlineData(EthernetDeviceVersion.AllenBradley, "127.0.0.1", 44818)]
        public void EthernetDevice_Tests(EthernetDeviceVersion deviceVersion, string ip, int port)
        {
            client = IoTClientFactory.CreateClient2EthernetDevice(deviceVersion, ip, port);
            Assert.True(client.Open().IsSucceed);

            if (deviceVersion.ToString().StartsWith("Modbus"))
                Modbus读写测试();
            else if (deviceVersion.ToString().StartsWith("Siemens_S7_"))
                Siemens读写测试();
            else if (deviceVersion.ToString().StartsWith("OmronFins"))
                OmronFins读写测试();
            else if (deviceVersion.ToString().StartsWith("Mitsubishi"))
                Mitsubishi读写测试();
            else if (deviceVersion.ToString().StartsWith("AllenBradley"))
                AllenBradley读写测试();
        }

        [Theory]
        [InlineData("169.254.150.55", "5121")]
        public void BACnet_Tests(string ip, string deviceId)
        {
            client = IoTClientFactory.CreateClient2BACnetDevice(ip, deviceId);
            Assert.True(client.Open().IsSucceed);

            Thread.Sleep(1000);//等待扫描

            BACnet读写测试();
        }

        [Fact]
        public void ModbusRtu_Test()
        {
            client = IoTClientFactory.CreateClient2SerialDevice(SerialDeviceVersion.ModbusRtu, "COM2", 9600, 8, System.IO.Ports.StopBits.One, System.IO.Ports.Parity.None);
            Assert.True(client.Open().IsSucceed);

            Modbus读写测试();
        }

        private void Modbus读写测试()
        {
            byte stationNumber = 1;//站号

            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for (int i = 0; i < 10; i++)
            {
                #region 生产随机数
                short short_number = (short)rnd.Next(short.MinValue, short.MaxValue);
                ushort ushort_number = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);
                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                uint uint_number = (uint)Math.Abs(rnd.Next(int.MinValue, int.MaxValue));
                long long_number = rnd.Next(int.MinValue, int.MaxValue);
                ulong ulong_number = (ulong)Math.Abs(rnd.Next(int.MinValue, int.MaxValue));
                float float_number = rnd.Next(int.MinValue, int.MaxValue) / 100;
                double double_number = (double)rnd.Next(int.MinValue, int.MaxValue) / 100;
                bool coil = int_number % 2 == 0;
                #endregion

                //写入地址:0 值为:short_number 站号:stationNumber 功能码:默认16(也可以自己传入对应的功能码)
                client.Write($"{stationNumber},0,16", short_number, DataTypeEnum.Int16);
                client.Write($"{stationNumber},4,16", ushort_number, DataTypeEnum.UInt16);
                client.Write($"{stationNumber},8,16", int_number, DataTypeEnum.Int32);
                client.Write($"{stationNumber},12,16", uint_number, DataTypeEnum.UInt32);
                client.Write($"{stationNumber},16,16", long_number, DataTypeEnum.Int64);
                client.Write($"{stationNumber},20,16", ulong_number, DataTypeEnum.UInt64);
                client.Write($"{stationNumber},24,16", float_number, DataTypeEnum.Float);
                client.Write($"{stationNumber},28,16", double_number, DataTypeEnum.Double);
                //如果是modbus slave 模拟，需要开一个function 01 coil的窗体
                client.Write($"{stationNumber},32,5", coil, DataTypeEnum.Bool);


                //写入可能有一定的延时，100毫秒后检验
                //Thread.SpinWait(100);

                //读取地址:0 站号:stationNumber 功能码:默认16(也可以自己传入对应的功能码)
                var read_short_number = client.ReadInt16($"{stationNumber},0, 3").Value;
                Assert.True(read_short_number == short_number);
                Assert.True(client.ReadUInt16($"{stationNumber},4, 3").Value == ushort_number);
                Assert.True(client.ReadInt32($"{stationNumber},8, 3").Value == int_number);
                Assert.True(client.ReadUInt32($"{stationNumber},12, 3").Value == uint_number);
                Assert.True(client.ReadInt64($"{stationNumber},16, 3").Value == long_number);
                Assert.True(client.ReadUInt64($"{stationNumber},20, 3").Value == ulong_number);
                Assert.True(client.ReadFloat($"{stationNumber},24, 3").Value == float_number);
                Assert.True(client.ReadDouble($"{stationNumber},28, 3").Value == double_number);

                Assert.True(client.ReadBoolean($"{stationNumber},32, 1").Value == coil);
            }

            var list = new Dictionary<string, DataTypeEnum>();
            list.Add($"1,2,3", DataTypeEnum.Int16);
            list.Add($"1,2,4", DataTypeEnum.Int16);
            list.Add($"1,5,3", DataTypeEnum.Int16);
            list.Add($"1,199,3", DataTypeEnum.Int16);
            list.Add($"1,200,2", DataTypeEnum.Bool);
            list.Add($"1,201,2", DataTypeEnum.Bool);
            list.Add($"1,202,2", DataTypeEnum.Bool);
            list.Add($"1,203,2", DataTypeEnum.Bool);
            list.Add($"1,204,2", DataTypeEnum.Bool);
            list.Add($"1,205,2", DataTypeEnum.Bool);
            list.Add($"1,206,2", DataTypeEnum.Bool);
            var result = client.BatchRead(list, 10);
        }

        private void Siemens读写测试()
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

            }
        }

        private void OmronFins读写测试()
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

        private void Mitsubishi读写测试()
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

                Debug.WriteLine(short_number);
            }
        }

        private void AllenBradley读写测试()
        {
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
        }

        private void BACnet读写测试()
        {
            Random rnd = new Random((int)Stopwatch.GetTimestamp());

            for (int i = 0; i < 100; i++)
            {
                int int_number = rnd.Next(int.MinValue, int.MaxValue);
                client.Write("1_40", int_number);

                Assert.True(client.ReadInt32("1_40").Value == int_number);
            }
        }
    }
}
