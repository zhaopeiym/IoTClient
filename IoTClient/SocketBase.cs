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
        /// 警告日志委托
        /// 为了可用性，会对异常网络进行重试。此类日志通过委托接口给出去。
        /// </summary>
        public LoggerDelegate WarningLog { get; set; }

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
        /// <returns></returns>
        protected Result<byte[]> SocketRead(Socket socket, int receiveCount)
        {
            var result = new Result<byte[]>();
            if (receiveCount < 0)
            {
                result.IsSucceed = false;
                result.Err = $"读取长度[receiveCount]为{receiveCount}";
                result.AddErr2List();
                return result;
            }

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
                        socket?.SafeClose();
                        result.IsSucceed = false;
                        result.Err = $"连接被断开";
                        result.AddErr2List();
                        return result;
                    }
                    receiveFinish += readLeng;
                }
                catch (SocketException ex)
                {
                    socket?.SafeClose();
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                        result.Err = $"连接超时：{ex.Message}";
                    else
                        result.Err = $"连接被断开，{ex.Message}";
                    result.IsSucceed = false;
                    result.AddErr2List();
                    result.Exception = ex;
                    return result;
                }
            }
            result.Value = receiveBytes;
            return result.EndTime();
        }

        /// <summary>
        /// 发送报文，并获取响应报文（建议使用SendPackageReliable，如果异常会自动重试一次）
        /// </summary>
        /// <param name="command">发送命令</param>
        /// <returns></returns>
        public abstract Result<byte[]> SendPackageSingle(byte[] command);

        /// <summary>
        /// 发送报文，并获取响应报文（如果网络异常，会自动进行一次重试）
        /// TODO 重试机制应改成用户主动设置
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Result<byte[]> SendPackageReliable(byte[] command)
        {
            try
            {
                var result = SendPackageSingle(command);
                if (!result.IsSucceed)
                {
                    WarningLog?.Invoke(result.Err, result.Exception);
                    //如果出现异常，则进行一次重试         
                    var conentResult = Connect();
                    if (!conentResult.IsSucceed)
                        return new Result<byte[]>(conentResult);

                    return SendPackageSingle(command);
                }
                else
                    return result;
            }
            catch (Exception ex)
            {
                try
                {
                    WarningLog?.Invoke(ex.Message, ex);
                    //如果出现异常，则进行一次重试                
                    var conentResult = Connect();
                    if (!conentResult.IsSucceed)
                        return new Result<byte[]>(conentResult);

                    return SendPackageSingle(command);
                }
                catch (Exception ex2)
                {
                    Result<byte[]> result = new Result<byte[]>();
                    result.IsSucceed = false;
                    result.Err = ex2.Message;
                    result.AddErr2List();
                    return result.EndTime();
                }
            }
        }
    }
}
