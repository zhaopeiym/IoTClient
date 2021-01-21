using System;
using System.Collections.Generic;
using System.Text;

namespace IoTClient.Models
{
    public class ModbusOutput
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 站号
        /// </summary>
        public byte StationNumber { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode { get; set; }
        public object Value { get; set; }
    }
}
