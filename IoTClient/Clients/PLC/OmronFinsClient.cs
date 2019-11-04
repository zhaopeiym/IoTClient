using IoTClient.Core;
using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IoTClient.Clients.PLC
{
    /// <summary>
    /// 欧姆龙PLC 客户端
    /// </summary>
    public class OmronFinsClient : SocketBase
    {
        private IPEndPoint ipAndPoint;
        private int timeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public OmronFinsClient(string ip, int port, int timeout = 1500)
        {
            ipAndPoint = new IPEndPoint(IPAddress.Parse(ip), port); ;
            this.timeout = timeout;
        }

        /// <summary>
        /// 打开长连接
        /// </summary>
        /// <returns></returns>
        protected override Result Connect()
        {
            var result = new Result();
            socket?.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                #region 超时时间设置
#if !DEBUG
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;
#endif
                #endregion

                socket.Connect(ipAndPoint);
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                return result;
            }
            return result;
        }
    }
}
