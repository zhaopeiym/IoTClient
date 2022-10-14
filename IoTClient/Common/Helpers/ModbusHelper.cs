using System;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public class ModbusHelper
    {
        /// <summary>
        /// 是否为异常功能码
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public static bool VerifyFunctionCode(byte resultCode, byte responseCode)
        {
            return responseCode - resultCode == 128;
        }

        /// <summary>
        /// 异常码描述
        /// https://www.likecs.com/show-204655077.html?sc=5546
        /// </summary>
        /// <param name="errCode"></param>
        public static string ErrMsg(byte errCode)
        {
            var err = "未知异常";
            switch (errCode)
            {
                case 0x01:
                    err = $"异常码{errCode}：⾮法功能";
                    break;
                case 0x02:
                    err = $"异常码{errCode}：⾮法数据地址";
                    break;
                case 0x03:
                    err = $"异常码{errCode}：⾮法数据值";
                    break;
                case 0x04:
                    err = $"异常码{errCode}：从站设备故障";
                    break;
                case 0x05:
                    err = $"异常码{errCode}：确认";
                    break;
                case 0x06:
                    err = $"异常码{errCode}：从属设备忙";
                    break;
                case 0x08:
                    err = $"异常码{errCode}：存储奇偶性差错";
                    break;
                case 0x0A:
                    err = $"异常码{errCode}：不可⽤⽹关路径";
                    break;
                case 0x0B:
                    err = $"异常码{errCode}：⽹关⽬标设备响应失败";
                    break;
            }
            return err;
        }
    }
}
