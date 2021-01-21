using IoTClient.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTClient.Models
{
    public class ModbusInput
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataTypeEnum DataType { get; set; }
        /// <summary>
        /// 站号
        /// </summary>
        public byte StationNumber { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode { get; set; }
    }

}
