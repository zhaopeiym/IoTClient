using System;
using System.ComponentModel;

namespace IoTClient.Enums
{
    /// <summary>
    /// 三菱型号版本
    /// </summary>
    [Flags]
    public enum MitsubishiVersion
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None = 0,
        /// <summary>
        /// 三菱 MC A-1E帧
        /// </summary>
        [Description("三菱MC_A-1E帧")]
        A_1E = 1,
        /// <summary>
        /// 三菱 MC Qna-3E帧
        /// </summary>
        [Description("三菱MC_Qna-3E帧")]
        Qna_3E = 2,
    }
}
