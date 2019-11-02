using IoTClient.Models;
using System;
using System.Net.Sockets;

namespace IoTClient.Core
{
    /// <summary>
    /// Socket基类
    /// </summary>
    public abstract class SocketBase
    {
        /// <summary>
        /// 分批缓冲区大小
        /// </summary>
        protected const int BufferSize = 4096;
        /// <summary>
        /// Socket实例
        /// </summary>
        protected Socket socket;
        /// <summary>
        /// 是否自动打开关闭
        /// </summary>
        protected bool isAutoOpen = true;
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public Result Open()
        {
            isAutoOpen = false;
            return Connect();
        }

        protected abstract Result Connect();

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            isAutoOpen = true;
            return Dispose();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        protected bool Dispose()
        {
            try
            {
                if (socket.Connected) socket?.Shutdown(SocketShutdown.Both);//正常关闭连接
                socket?.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="receiveCount"></param>
        /// <returns></returns>
        protected byte[] SocketRead(Socket socket, int receiveCount)
        {
            byte[] receiveBytes = new byte[receiveCount];
            int receiveFinish = 0;
            while (receiveFinish < receiveCount)
            {
                // 分批读取
                int receiveLength = (receiveCount - receiveFinish) >= BufferSize ? BufferSize : (receiveCount - receiveFinish);
                receiveFinish += socket.Receive(receiveBytes, receiveFinish, receiveLength, SocketFlags.None);
                if (receiveFinish == 0)
                {
                    if (socket.Connected) socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    throw new Exception("连接已断开");
                }
            }
            return receiveBytes;
        }

    }
}
