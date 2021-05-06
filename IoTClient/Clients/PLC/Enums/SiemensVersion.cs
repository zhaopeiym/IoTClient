using System;
using System.ComponentModel;

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
        /// 西门子S7-200 【需要配置网络模块】
        /// </summary>
        [Description("西门子S7-200")]
        S7_200 = 1,
        /// <summary>
        /// 西门子S7-200Smar
        /// </summary>
        [Description("西门子S7-200Smar")]
        S7_200Smart = 2,
        /// <summary>
        /// 西门子S7-300
        /// </summary>
        [Description("西门子S7-300")]
        S7_300 = 3,
        /// <summary>
        /// 西门子S7-400
        /// </summary>
        [Description("西门子S7-400")]
        S7_400 = 4,
        /// <summary>
        /// 西门子S7-1200
        /// </summary>
        [Description("西门子S7-1200")]
        S7_1200 = 5,
        /// <summary>
        /// 西门子S7-1500
        /// </summary>
        [Description("西门子S7-1500")]
        S7_1500 = 6,
    }
}
