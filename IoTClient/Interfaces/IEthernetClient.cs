using System.Net;

namespace IoTClient.Interfaces
{
    /// <summary>
    /// 以太网形式
    /// </summary>
    public interface IEthernetClient : IIoTClient
    {
        /// <summary>
        /// IPEndPoint
        /// </summary>
        IPEndPoint IpEndPoint { get; }
    }
}
