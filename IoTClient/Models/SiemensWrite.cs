using IoTClient.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTClient.Models
{
    public class SiemensWrite : SiemensData
    {

        public SiemensWrite()
        {

        }

        public SiemensWrite(SiemensData data)
        {
            Assignment(data);
        }

        /// <summary>
        /// 要写入的数据
        /// </summary>
        public byte[] WriteData { get; set; }

        /// <summary>
        /// 赋值
        /// </summary>
        private void Assignment(SiemensData data)
        {
            Address = data.Address;
            DataType = data.DataType;
            TypeCode = data.TypeCode;
            DbBlock = data.DbBlock;
            BeginAddress = data.BeginAddress;
            ReadWriteLength = data.ReadWriteLength;
            ReadWriteBit = data.ReadWriteBit;
        }
    }
}
