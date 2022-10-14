using IoTClient.Clients.Modbus;
using IoTClient.Enums;
using IoTClient.Models;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using Xunit;

namespace IoTClient.Tests.Modbus
{
    public class ModbusRtuClient_tests
    {
        private ModbusRtuClient client;
        byte stationNumber = 1;//站号
        public ModbusRtuClient_tests()
        {
            client = new ModbusRtuClient("COM2", 9600, 8, StopBits.One, Parity.None);
        }

        [Fact]
        public async Task 短连接自动开关()
        {
            short Number = 33;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 34;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 1;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 1);

            Number = 0;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 0);

            int numberInt32 = -12;
            client.Write("4", numberInt32, stationNumber);
            Assert.True(client.ReadInt32("4", stationNumber).Value == numberInt32);

            float numberFloat = 112;
            client.Write("4", numberFloat, stationNumber);
            Assert.True(client.ReadFloat("4", stationNumber).Value == numberFloat);

            double numberDouble = 32;
            client.Write("4", numberDouble, stationNumber);
            Assert.True(client.ReadDouble("4", stationNumber).Value == numberDouble);
        }

        [Fact]
        public async Task 长连接主动开关()
        {
            client.Open();

            short Number = 33;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 34;
            client.Write("4", Number, stationNumber);
            Assert.True(client.ReadInt16("4", stationNumber).Value == Number);

            Number = 1;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 1);

            Number = 0;
            client.Write("12", Number, stationNumber);
            Assert.True(client.ReadInt16("12", stationNumber).Value == 0);

            int numberInt32 = -12;
            client.Write("4", numberInt32, stationNumber);
            Assert.True(client.ReadInt32("4", stationNumber).Value == numberInt32);

            float numberFloat = 112;
            client.Write("4", numberFloat, stationNumber);
            Assert.True(client.ReadFloat("4", stationNumber).Value == numberFloat);

            double numberDouble = 32;
            client.Write("4", numberDouble, stationNumber);
            Assert.True(client.ReadDouble("4", stationNumber).Value == numberDouble);

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
