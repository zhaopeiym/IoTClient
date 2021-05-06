using IoTClient.Clients.Modbus;
using IoTClient.Enums;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.Modbus_Tests
{
    public class ModbusRtuOverTcpClient_tests
    {
        ModbusRtuOverTcpClient client;
        byte stationNumber = 2;//站号
        public ModbusRtuOverTcpClient_tests()
        {
            client = new ModbusRtuOverTcpClient("127.0.0.1", 502);
        }

        [Fact]
        public async Task 短连接自动开关()
        {
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
                client.Write("0", short_number, stationNumber, 16);
                client.Write("4", ushort_number, stationNumber, 16);
                client.Write("8", int_number, stationNumber, 16);
                client.Write("12", uint_number, stationNumber, 16);
                client.Write("16", long_number, stationNumber, 16);
                client.Write("20", ulong_number, stationNumber, 16);
                client.Write("24", float_number, stationNumber, 16);
                client.Write("28", double_number, stationNumber, 16);

                client.Write("32", coil, stationNumber, 5);

                //写入可能有一定的延时，500毫秒后检验
                await Task.Delay(500);

                //读取地址:0 站号:stationNumber 功能码:默认16(也可以自己传入对应的功能码)
                var read_short_number = client.ReadInt16("0", stationNumber, 3).Value;
                Assert.True(read_short_number == short_number);
                Assert.True(client.ReadUInt16("4", stationNumber, 3).Value == ushort_number);
                Assert.True(client.ReadInt32("8", stationNumber, 3).Value == int_number);
                Assert.True(client.ReadUInt32("12", stationNumber, 3).Value == uint_number);
                Assert.True(client.ReadInt64("16", stationNumber, 3).Value == long_number);
                Assert.True(client.ReadUInt64("20", stationNumber, 3).Value == ulong_number);
                Assert.True(client.ReadFloat("24", stationNumber, 3).Value == float_number);
                Assert.True(client.ReadDouble("28", stationNumber, 3).Value == double_number);

                Assert.True(client.ReadCoil("32", stationNumber, 1).Value == coil);
            }
        }

        [Fact]
        public async Task 长连接主动开关()
        {
            client.Open();

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
                client.Write("0", short_number, stationNumber, 16);
                client.Write("4", ushort_number, stationNumber, 16);
                client.Write("8", int_number, stationNumber, 16);
                client.Write("12", uint_number, stationNumber, 16);
                client.Write("16", long_number, stationNumber, 16);
                client.Write("20", ulong_number, stationNumber, 16);
                client.Write("24", float_number, stationNumber, 16);
                client.Write("28", double_number, stationNumber, 16);

                client.Write("32", coil, stationNumber, 5);

                //写入可能有一定的延时，500毫秒后检验
                await Task.Delay(500);

                //读取地址:0 站号:stationNumber 功能码:默认16(也可以自己传入对应的功能码)
                var read_short_number = client.ReadInt16("0", stationNumber, 3).Value;
                Assert.True(read_short_number == short_number);
                Assert.True(client.ReadUInt16("4", stationNumber, 3).Value == ushort_number);
                Assert.True(client.ReadInt32("8", stationNumber, 3).Value == int_number);
                Assert.True(client.ReadUInt32("12", stationNumber, 3).Value == uint_number);
                Assert.True(client.ReadInt64("16", stationNumber, 3).Value == long_number);
                Assert.True(client.ReadUInt64("20", stationNumber, 3).Value == ulong_number);
                Assert.True(client.ReadFloat("24", stationNumber, 3).Value == float_number);
                Assert.True(client.ReadDouble("28", stationNumber, 3).Value == double_number);

                Assert.True(client.ReadCoil("32", stationNumber, 1).Value == coil);
            }

            client.Close();
        }

        [Fact]
        public void 批量读取()
        {          
            var list = new List<ModbusInput>();
            list.Add(new ModbusInput()
            {
                Address = "2",
                DataType = DataTypeEnum.Int16,
                FunctionCode = 3,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "2",
                DataType = DataTypeEnum.Int16,
                FunctionCode = 4,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "5",
                DataType = DataTypeEnum.Int16,
                FunctionCode = 3,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "199",
                DataType = DataTypeEnum.Int16,
                FunctionCode = 3,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "200",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "201",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "202",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "203",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "204",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "205",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "206",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "207",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            list.Add(new ModbusInput()
            {
                Address = "208",
                DataType = DataTypeEnum.Bool,
                FunctionCode = 2,
                StationNumber = 1
            });
            var result = client.BatchRead(list);
        }
    }
}
