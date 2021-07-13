using System.ComponentModel;

namespace IoTClient.Extensions.Adapter
{
    public enum EthernetDeviceVersion
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None,
        [Description("ModbusTcp")]
        ModbusTcp,
        [Description("ModbusRtuOverTcp")]
        ModbusRtuOverTcp,
        [Description("Siemens_S7_200")]
        Siemens_S7_200,
        [Description("Siemens_S7_200Smart")]
        Siemens_S7_200Smart,
        [Description("Siemens_S7_300")]
        Siemens_S7_300,
        [Description("Siemens_S7_400")]
        Siemens_S7_400,
        [Description("Siemens_S7_1200")]
        Siemens_S7_1200,
        [Description("Siemens_S7_1500")]
        Siemens_S7_1500,
        [Description("Mitsubishi_A_1E")]
        Mitsubishi_A_1E,
        [Description("Mitsubishi_Qna_3E")]
        Mitsubishi_Qna_3E,
        [Description("AllenBradley")]
        AllenBradley,
        [Description("OmronFins")]
        OmronFins
    }
}
