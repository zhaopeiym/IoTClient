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
    /// ModbusAscii 服务端模拟
    /// </summary>
    public class ModbusAsciiServer
    {
        private SerialPort serialPort;
        DataPersist dataPersist;
        public ModbusAsciiServer(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout = 1500)
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
                var address = $"{requetData[5]}-{requetData[6]}-{requetData[7]}-{requetData[8]}";

                byte[] tempData = new byte[requetData.Length - 3];
                Buffer.BlockCopy(requetData, 1, tempData, 0, tempData.Length);
                var requetAsciiData = DataConvert.AsciiArrayToByteArray(tempData);

                var type = $"{requetData[3].ToString("X2")}{requetData[4].ToString("X2")}";
                switch (type)
                {
                    //读取线圈
                    case "3031":
                        {
                            var value = dataPersist.Read(address);
                            var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                            byte[] data = null;
                            //没有存储过的数据的，默认响应false
                            if (bytes == null)
                            {
                                data = DataConvert.StringToByteArray("01 01 01 00");
                            }
                            else
                            {
                                if (bytes[0].ToString("X2") != "3A" ||
                                    bytes[bytes.Length - 2].ToString("X2") != "0D" ||
                                    bytes[bytes.Length - 1].ToString("X2") != "0A")
                                {
                                    throw new Exception("标记验证失败");
                                }

                                byte[] asciiData = new byte[bytes.Length - 3];
                                Buffer.BlockCopy(bytes, 1, asciiData, 0, asciiData.Length);
                                var byteDataArray = DataConvert.AsciiArrayToByteArray(asciiData);

                                //true
                                if (byteDataArray[4].ToString("X2") == "FF" && byteDataArray[5].ToString("X2") == "00")
                                    data = DataConvert.StringToByteArray("01 01 01 01");
                                else//false
                                    data = DataConvert.StringToByteArray("01 01 01 00");
                            }

                            var dataString = string.Join("", LRC.GetLRC(data).Select(t => t.ToString("X2")));
                            var databyte = Encoding.ASCII.GetBytes(dataString);
                            var responseData = new byte[databyte.Length + 3];
                            Buffer.BlockCopy(databyte, 0, responseData, 1, databyte.Length);
                            responseData[0] = 0x3A;
                            responseData[responseData.Length - 2] = 0x0D;
                            responseData[responseData.Length - 1] = 0x0A;

                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                    //写入线圈
                    case "3035":
                        {
                            dataPersist.Write(address, JsonConvert.SerializeObject(requetData));
                            serialPort.Write(requetData, 0, requetData.Length);
                        }
                        break;
                    //读取
                    case "3033":
                        {
                            var value = dataPersist.Read(address);
                            var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                            byte[] data = null;
                            if (bytes == null)
                            {
                                data = new byte[2 + 3];
                                data[2] = 2;//数据长度
                            }
                            else
                            {
                                if (bytes[0].ToString("X2") != "3A" ||
                                    bytes[bytes.Length - 2].ToString("X2") != "0D" ||
                                    bytes[bytes.Length - 1].ToString("X2") != "0A")
                                {
                                    throw new Exception("标记验证失败");
                                }

                                byte[] asciiData = new byte[bytes.Length - 3];
                                Buffer.BlockCopy(bytes, 1, asciiData, 0, asciiData.Length);
                                var byteDataArray = DataConvert.AsciiArrayToByteArray(asciiData);
                                data = new byte[byteDataArray[6] + 3];
                                data[2] = byteDataArray[6];//数据长度
                                Buffer.BlockCopy(byteDataArray, 7, data, 3, data.Length - 3);
                            }
                            data[0] = requetAsciiData[0];//站号
                            data[1] = 0x03;//功能码                           

                            var dataString = string.Join("", LRC.GetLRC(data).Select(t => t.ToString("X2")));
                            var databyte = Encoding.ASCII.GetBytes(dataString);
                            var responseData = new byte[databyte.Length + 3];
                            Buffer.BlockCopy(databyte, 0, responseData, 1, databyte.Length);
                            responseData[0] = 0x3A;
                            responseData[responseData.Length - 2] = 0x0D;
                            responseData[responseData.Length - 1] = 0x0A;

                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                    //写入3130 对应十进制16 十六进制 10
                    case "3130":
                        {
                            dataPersist.Write(address, JsonConvert.SerializeObject(requetData));
                            serialPort.Write(requetData, 0, requetData.Length);
                        }
                        break;
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
