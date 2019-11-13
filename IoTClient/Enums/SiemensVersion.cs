using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IoTClient.Common.Enums
{
    /// <summary>
    /// 西门子型号版本
    /// </summary>
    [Flags]
    public enum SiemensVersion
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None = 0,
        /// <summary>
        /// 西门子S7-200Smar
        /// </summary>
        [Description("西门子S7-200Smar")]
        S7_200Smart = 1,
        /// <summary>
        /// 西门子S7-300
        /// </summary>
        [Description("西门子S7-300")]
        S7_300 = 2,
    }
}
