using System;
using System.IO.BACnet;

namespace IoTClient.Extensions.Adapter.Communication.Models
{
    public class BacProperty
    {
        public BacnetObjectId ObjectId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string PROP_DESCRIPTION { get; set; }
        /// <summary>
        /// 点名
        /// </summary>
        public string PROP_OBJECT_NAME { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object PROP_PRESENT_VALUE { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public Type DataType { get; set; }
    }
}
