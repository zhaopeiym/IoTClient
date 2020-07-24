# IoTClient
- 这是一个物联网设备通讯协议实现客户端，将包括主流PLC通信读取、ModBus协议、Bacnet协议等常用工业通讯协议。
- 本组件基于.NET Standard 2.0，可用于.Net的跨平台开发，如Windows、Linux甚至可运行于树莓派上。
- 本组件终身开源免费，采用最宽松MIT协议，您也可以随意修改和商业使用（商业使用请做好评估和测试）。  
- 开发工具：Visual Studio 2019 
- QQ交流群：[995475200](https://jq.qq.com/?_wv=1027&k=5bz0ne5)  
- IoTClient Tool [下载1](https://github.com/zhaopeiym/IoTClient/releases) [下载2](https://files-cdn.cnblogs.com/files/zhaopei/IoTClient.rar) 

# 使用说明
## 引用组件
[Nuget安装](https://www.nuget.org/packages/IoTClient/) ```Install-Package IoTClient ```  
或图形化安装   
![image](https://user-images.githubusercontent.com/5820324/68722366-2fc5bf00-05f0-11ea-8282-f2b0a58a9f9d.png)  

## ModBusTcp读写操作

```
//1、实例化客户端 - 输入正确的IP和端口
ModBusTcpClient client = new ModBusTcpClient("127.0.0.1", 502);

//2、写操作 - 参数依次是：地址 、值 、站号 、功能码
client.Write("4", (short)33, 2, 16);
client.Write("4", (short)3344, 2, 16);

//3、读操作 - 参数依次是：地址 、站号 、功能码
var value = client.ReadInt16("4", 2, 3).Value;
var value2 = client.ReadInt32("4", 2, 3).Value;

//4、如果没有主动Open，则会每次读写操作的时候自动打开自动和关闭连接，这样会使读写效率大大减低。所以建议手动Open和Close。
client.Open();

//5、读写操作都会返回操作结果对象Result
var result = client.ReadInt16("4", 2, 3);
//5.1 读取是否成功（true或false）
var isSucceed = result.IsSucceed;
//5.2 读取失败的异常信息
var errMsg = result.Err;
//5.3 读取操作实际发送的请求报文
var requst  = result.Requst;
//5.4 读取操作服务端响应的报文
var response = result.Response;
//5.5 读取到的值
var value3 = result.Value;
``` 

## ModBusRtu读写操作
```
//实例化客户端 - [COM端口名称,波特率,数据位,停止位,奇偶校验]
ModBusRtuClient client = new ModBusRtuClient("COM3", 9600, 8, StopBits.One, Parity.None);

//其他读写操作和ModBusTcpClient的读写操作一致
```

## SiemensClient读写操作
```
//1、实例化客户端 - 输入正确的IP和端口
SiemensClient client = new SiemensClient(SiemensVersion.S7_200Smart, "127.0.0.1",102);

//2、写操作
client.Write("Q1.3", true);
client.Write("V2205", (short)11);
client.Write("V2209", 33);

//3、读操作
var value1 = client.ReadBoolean("Q1.3").Value;
var value2 = client.ReadInt16("V2205").Value;
var value3 = client.ReadInt32("V2209").Value;

//4、如果没有主动Open，则会每次读写操作的时候自动打开自动和关闭连接，这样会使读写效率大大减低。所以建议手动Open和Close。
client.Open();

//5、读写操作都会返回操作结果对象Result
var result = client.ReadInt16("V2205");
//5.1 读取是否成功（true或false）
var isSucceed = result.IsSucceed;
//5.2 读取失败的异常信息
var errMsg = result.Err;
//5.3 读取操作实际发送的请求报文
var requst  = result.Requst;
//5.4 读取操作服务端响应的报文
var response = result.Response;
//5.5 读取到的值
var value4 = result.Value;
```

## 其他更多详细使用请[参考](https://github.com/zhaopeiym/IoTClient/tree/master/IoTClient.Tool/Controls)

# IoTClient Tool效果图   
![image](https://user-images.githubusercontent.com/5820324/68926947-9c43e800-07c1-11ea-9da7-f431ec52f2fb.png)  

![image](https://user-images.githubusercontent.com/5820324/68926546-c052f980-07c0-11ea-86ec-8ae36cc9aa3a.png)    

![image](https://user-images.githubusercontent.com/5820324/68926383-5a667200-07c0-11ea-905c-42a391f2300f.png)

![image](https://user-images.githubusercontent.com/5820324/68068805-3c4a4c00-fd94-11e9-899e-cec0b4b70fa8.png)  

![image](https://user-images.githubusercontent.com/5820324/68068874-bf6ba200-fd94-11e9-817d-62ed251e258f.png)  

![image](https://user-images.githubusercontent.com/5820324/68068818-63a11900-fd94-11e9-932e-fa0bd5941861.png)  

![image](https://user-images.githubusercontent.com/5820324/68068890-e75b0580-fd94-11e9-9370-b914e5af9590.png)  
