namespace IoTServer.Servers.PLC
{
    public interface IIoTServer
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
    }
}
