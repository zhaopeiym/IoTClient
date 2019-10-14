using System;
using System.Collections.Generic;
using System.Text;

namespace IoTClient.Core.Models
{
    public class SiemensData
    {
        /// <summary>
        /// 区域类型
        /// </summary>
        public byte TypeCode { get; set; }
        /// <summary>
        /// DB块编号
        /// </summary>
        public ushort DbBlock { get; set; }
        ///// <summary>
        ///// 读取的数据长度
        ///// </summary>
        //public ushort Length { get; set; }
        /// <summary>
        /// 开始地址
        /// </summary>
        public int BeginAddress { get; set; }
    }
}
