namespace IoTClient.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMitsubishiMCType
    {
        /// <summary>
        /// 类型的代号值
        /// </summary>
        byte[] TypeCode { get; }

        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        byte DataType { get; }

        /// <summary>
        /// 指示地址是10进制，还是16进制的
        /// </summary>
        int Format { get; }
    }
}
