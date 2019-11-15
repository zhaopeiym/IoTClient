using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace IoTServer.Servers
{
    /// <summary>
    /// ServerSocket基类
    /// </summary>
    public class ServerSocketBase
    {
        /// <summary>
        /// 分批缓冲区大小
        /// </summary>
        protected const int BufferSize = 4096;

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="receiveCount">读取长度</param>
        /// <returns></returns>
        protected byte[] SocketRead(Socket socket, int receiveCount)
        {
            byte[] receiveBytes = new byte[receiveCount];
            int receiveFinish = 0;
            while (receiveFinish < receiveCount)
            {
                // 分批读取
                int receiveLength = (receiveCount - receiveFinish) >= BufferSize ? BufferSize : (receiveCount - receiveFinish);
                var readLeng = socket.Receive(receiveBytes, receiveFinish, receiveLength, SocketFlags.None);
                if (readLeng == 0)
                {
                    if (socket.Connected) socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    throw new Exception("连接已断开");
                }
                receiveFinish += readLeng;
            }
            return receiveBytes;
        }
    }
}
