using IoTClient.Common.Helpers;
using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace IoTServer.Servers.Modbus
{
    /// <summary>
    /// ModbusRtu 服务端模拟
    /// </summary>
    public class ModbusRtuServer
    {
        private SerialPort serialPort;
        DataPersist dataPersist;
        public ModbusRtuServer(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout = 1500)
        {
            if (serialPort == null) serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = dataBits;
            serialPort.StopBits = stopBits;
            serialPort.Encoding = Encoding.ASCII;
            serialPort.Parity = parity;

            serialPort.ReadTimeout = timeout;
            serialPort.WriteTimeout = timeout;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            dataPersist = new DataPersist("ModbusTcpServer");
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            serialPort.Open();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            serialPort.Close();
            serialPort.Dispose();
        }

        /// <summary>
        /// 接收数据回调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] requetData = new byte[serialPort.BytesToRead];
                serialPort.Read(requetData, 0, requetData.Length);
                var address = requetData[2] * 256 + requetData[3];

                var stationNumberKey = $"{requetData[0]}-key";//站号
                switch (requetData[1])
                {
                    //读取线圈
                    case 1:
                        {
                            var value = dataPersist.Read(stationNumberKey + "-Coil");
                            var byteArray = JsonConvert.DeserializeObject<byte[]>(value) ?? new byte[65536];
                            var registerLenght = requetData[4] * 256 + requetData[5];
                            var blenght = (byte)Math.Ceiling(registerLenght / 8f);
                            var tempData = new byte[registerLenght];
                            Buffer.BlockCopy(byteArray, address, tempData, 0, tempData.Length);
                            var rData = new byte[3 + registerLenght];
                            rData[0] = 1;
                            rData[1] = 1;
                            rData[2] = (byte)registerLenght;
                            for (int i = 0; i < blenght; i++)
                            {
                                rData[3 + i] = (byte)DataConvert.BinaryArrayToInt(string.Join("", tempData.Skip(i * 8).Take(8).Reverse()));
                            }
                            var responseData = CRC16.GetCRC16(rData);
                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                    //写入线圈
                    case 5:
                        {
                            var value = new byte[2];
                            Buffer.BlockCopy(requetData, requetData.Length - 4, value, 0, value.Length);
                            var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(stationNumberKey + "-Coil")) ?? new byte[65536];
                            if (value[0] == 0 && value[1] == 0)
                                byteArray[address] = 0;
                            else
                                byteArray[address] = 1;
                            dataPersist.Write(stationNumberKey + "-Coil", JsonConvert.SerializeObject(byteArray));
                            serialPort.Write(requetData, 0, requetData.Length);
                        }
                        break;
                    //读取
                    case 3:
                        {
                            var value = dataPersist.Read(stationNumberKey);
                            var byteArray = JsonConvert.DeserializeObject<byte[]>(value) ?? new byte[65536];
                            var dlength = requetData[4] * 256 + requetData[5];
                            var dataHead = new byte[] { 1, 3, (byte)(dlength * 2) };
                            var dataContent = new byte[dlength * 2];
                            Buffer.BlockCopy(byteArray, address * 2, dataContent, 0, dataContent.Length);
                            var tempData = dataHead.Concat(dataContent).ToArray();
                            var responseData = CRC16.GetCRC16(tempData);
                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                    //写入
                    case 16:
                        {
                            var value = new byte[requetData[6]];
                            Buffer.BlockCopy(requetData, 7, value, 0, value.Length);
                            var byteArray = JsonConvert.DeserializeObject<byte[]>(dataPersist.Read(stationNumberKey)) ?? new byte[65536];
                            value.CopyTo(byteArray, address * 2);
                            dataPersist.Write(stationNumberKey, JsonConvert.SerializeObject(byteArray));
                            var tempData = new byte[6];
                            Buffer.BlockCopy(requetData, 0, tempData, 0, tempData.Length);
                            var responseData = CRC16.GetCRC16(tempData);
                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
