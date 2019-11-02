using System;
using System.Collections.Generic;

namespace IoTClient.Tool.Model
{
    public class ResultBase<T>
    { 
        //
        // 摘要:
        //     请求是否成功
        public bool IsSuccess { get; }
        //
        // 摘要:
        //     请求结果code
        public int Code { get; set; }
        //
        // 摘要:
        //     异常消息
        public string ErrorMsg { get; set; } 
        public List<string> ErrorList { get; set; }
        //
        // 摘要:
        //     请求结果
        public T Data { get; set; } 
    }
}
