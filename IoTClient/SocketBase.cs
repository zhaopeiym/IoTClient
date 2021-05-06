using IoTClient.Common.Helpers;
using IoTClient.Models;
using System;
using System.Net.Sockets;

namespace IoTClient
{
    /// <summary>
    /// 日记记录委托定义
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ex"></param>
    public delegate void LoggerDelegate(string name, Exception ex = null);

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
        /// 连接（如果已经是连接状态会先关闭再打开）
        /// </summary>
        /// <returns></returns>
        protected abstract Result Connect();

        /// <summary>
        /// 打开连接（如果已经是连接状态会先关闭再打开）
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
                socket?.SafeClose();
                return result;
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.Exception = ex;
                result.ErrList.Add(ex.Message);
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
        /// Socket读取
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="receiveCount">读取长度</param>
        /// <param name="warningLog">日记委托记录</param>     
        /// <returns></returns>
        protected byte[] SocketRead(Socket socket, int receiveCount, LoggerDelegate warningLog = null)
        {
            byte[] receiveBytes = SocketTryRead(socket, receiveCount, warningLog);
            if (receiveBytes == null)
            {
                socket?.SafeClose();
                throw new Exception("连接被断开");
            }
            return receiveBytes;
        }

        /// <summary>
        /// Socket读取
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="receiveCount">读取长度</param>
        /// <param name="warningLog">日记委托记录</param>        
        /// <returns>读到的数据，如果内部出现异常则返回null</returns>
        protected byte[] SocketTryRead(Socket socket, int receiveCount, LoggerDelegate warningLog = null)
        {
            byte[] receiveBytes = new byte[receiveCount];
            int receiveFinish = 0;
            while (receiveFinish < receiveCount)
            {
                // 分批读取
                int receiveLength = (receiveCount - receiveFinish) >= BufferSize ? BufferSize : (receiveCount - receiveFinish);
                try
                {
                    var readLeng = socket.Receive(receiveBytes, receiveFinish, receiveLength, SocketFlags.None);
                    if (readLeng == 0)
                    {
                        return null;
                    }
                    receiveFinish += readLeng;
                }
                catch (SocketException ex)
                {
                    warningLog?.Invoke(ex.Message, ex);
                    return null;
                }
            }
            return receiveBytes;
        }
    }
}
