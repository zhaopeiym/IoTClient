using IoTClient.Common.Helpers;
using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace IoTServer.Servers.ModBus
{
    /// <summary>
    /// ModBusRtu 服务端模拟
    /// </summary>
    public class ModBusRtuServer
    {
        private SerialPort serialPort;
        DataPersist dataPersist;
        public ModBusRtuServer(string portName, int baudRate, int dataBits, StopBits stopBits)
        {
            if (serialPort == null) serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = dataBits;
            serialPort.StopBits = stopBits;
            serialPort.Encoding = Encoding.ASCII;
#if !DEBUG
            serialPort.ReadTimeout = 1000;//1秒
#endif
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            dataPersist = new DataPersist("ModBusTcpServer");
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
                switch (requetData[1])
                {
                    //读取线圈
                    case 1:
                        {
                            var value = dataPersist.Read(address);
                            var bytes = JsonConvert.DeserializeObject<byte[]>(value).Reverse().ToArray();
                            var tempData = new byte[4];
                            tempData[0] = 1;
                            tempData[1] = 1;
                            Buffer.BlockCopy(bytes, 0, tempData, 2, 2);
                            var responseData = CRC16.GetCRC16(tempData);
                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                    //写入线圈
                    case 5:
                        {
                            var value = new byte[2];
                            Buffer.BlockCopy(requetData, requetData.Length - 4, value, 0, value.Length);
                            dataPersist.Write(address, JsonConvert.SerializeObject(value));
                            serialPort.Write(requetData, 0, requetData.Length);
                        }
                        break;
                    //读取
                    case 3:
                        {
                            var value = dataPersist.Read(address);
                            var bytes = JsonConvert.DeserializeObject<byte[]>(value);
                            if (bytes == null)
                            {
                                var length = requetData[4] * 256 + requetData[5];
                                bytes = new byte[length * 2];
                            }
                            var dataHead = new byte[] { 1, 3, (byte)bytes.Length };
                            var tempData = dataHead.Concat(bytes).ToArray();
                            var responseData = CRC16.GetCRC16(tempData);
                            serialPort.Write(responseData, 0, responseData.Length);
                        }
                        break;
                    //写入
                    case 16:
                        {
                            var value = new byte[requetData[6]];
                            Buffer.BlockCopy(requetData, 7, value, 0, value.Length);
                            dataPersist.Write(address, JsonConvert.SerializeObject(value));
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
