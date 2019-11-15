using IoTClient.Models;
using System;
using System.Net.Sockets;

namespace IoTClient
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
        /// 连接
        /// </summary>
        /// <returns></returns>
        protected abstract Result Connect();

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public Result Open()
        {
            isAutoOpen = false;
            return Connect();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        protected Result Dispose()
        {
            Result result = new Result();
            try
            {
                if (socket.Connected) socket?.Shutdown(SocketShutdown.Both);//正常关闭连接
                socket?.Close();
                return result;
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public Result Close()
        {
            isAutoOpen = true;
            return Dispose();
        }

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
