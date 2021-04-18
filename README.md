
# IoTClient 
[![image](https://img.shields.io/nuget/v/IoTClient.svg)](https://www.nuget.org/packages/IoTClient/) [![image](https://img.shields.io/nuget/dt/IoTClient.svg)](https://www.nuget.org/packages/IoTClient/) ![image](https://img.shields.io/github/license/alienwow/SnowLeopard.svg)
- 这是一个物联网设备通讯协议实现客户端，将包括主流PLC通信读取、ModBus协议、Bacnet协议等常用工业通讯协议。
- 本组件基于.NET Standard 2.0，可用于.Net的跨平台开发，如Windows、Linux甚至可运行于树莓派上。
- 本组件终身开源免费，采用最宽松MIT协议，您也可以随意修改和商业使用（商业使用请做好评估和测试）。  
- 开发工具：Visual Studio 2019 
- QQ交流群：[995475200](https://jq.qq.com/?_wv=1027&k=5bz0ne5)  
- IoTClient Tool [下载1](https://github.com/zhaopeiym/IoTClient/releases/download/0.4.0/IoTClient.0.4.0.exe) [下载2](https://download.haojima.net/api/IoTClient/Download) 

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

//6、批量读取
var list = new List<ModBusInput>();
list.Add(new ModBusInput()
{
    Address = "2",
    DataType = DataTypeEnum.Int16,
    FunctionCode = 3,
    StationNumber = 1
});
list.Add(new ModBusInput()
{
    Address = "2",
    DataType = DataTypeEnum.Int16,
    FunctionCode = 4,
    StationNumber = 1
});
list.Add(new ModBusInput()
{
    Address = "199",
    DataType = DataTypeEnum.Int16,
    FunctionCode = 3,
    StationNumber = 1
});
var result = client.BatchRead(list);

//7、构造函数其他参数
//IP、端口、超时时间、大小端设置
ModBusTcpClient client = new ModBusTcpClient("127.0.0.1", 502, 1500, EndianFormat.ABCD);
``` 

## ModBusRtu读写操作
```
//实例化客户端 - [COM端口名称,波特率,数据位,停止位,奇偶校验]
ModBusRtuClient client = new ModBusRtuClient("COM3", 9600, 8, StopBits.One, Parity.None);

//其他读写操作和ModBusTcpClient的读写操作一致
```

## SiemensClient(西门子)读写操作
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
## 注意：关于Siemens的PLC地址
```
VB263、VW263、VD263中的B、W、D分别表示：byte型(8位)、word型(16位)、doubleword型(32位)。

在本组件传入地址的时候不需要带数据类型，直接使用对应方法读取对应类型即可，如：
VB263       - client.ReadByte("V263")
VD263       - client.ReadFloat("V263")
VD263       - client.ReadInt32("V263")
DB108.DBW4  - client.ReadUInt16("DB108.4")
DB1.DBX0.0  - client.ReadBoolean("DB1.0.0")
DB1.DBD0    - client.ReadFloat("DB1.0")
```
|C#数据类型 | smart200 | 1200/1500/300
|---|---|---
|bit | V1.0 | DB1.DBX1.0
|byte | VB1 | DB1.DBB1
|shor <br> ushort  | VW2 | DB1.DBW2
|int <br> uint <br> float | VD4 | DB1.DBD4

## SiemensClient最佳实践
```
1、什么时候不要主动Open
西门子plc一般最多允许8个长连接。所以当连接数不够用的时候或者做测试的时候就不要主动Open，这样组件会自动Open并即时Close。

2、什么时候主动Open
当长连接数量还够用，且想要提升读写性能。

3、除了主动Open连接，还可以通过批量读写，大幅提升读写性能。
//批量读取
Dictionary<string, DataTypeEnum> addresses = new Dictionary<string, DataTypeEnum>();
addresses.Add("DB4.24", DataTypeEnum.Float);
addresses.Add("DB1.434.0", DataTypeEnum.Bool);
addresses.Add("V4109", DataTypeEnum.Byte);
...
var result = client.BatchRead(addresses);

//批量写入
Dictionary<string, object> addresses = new Dictionary<string, object>();
addresses.Add("DB4.24", (float)1);
addresses.Add("DB4.0", (float)2);
addresses.Add("DB1.434.0", true);
...
var result = client.BatchWrite(addresses);

4、【注意】写入数据的时候需要明确数据类型
client.Write("DB4.12", 9);          //写入的是int类型
client.Write("DB4.12", (float)9);   //写入的是float类型

5、SiemensClient是线程安全类
由于plc长连接有限，SiemensClient被设计成线程安全类。可以把SiemensClient设置成单例，在多个线程之间使用SiemensClient的实例读写操作plc。
```

## MitsubishiClient(三菱)读写操作
```
//1、实例化客户端 - 输入正确的IP和端口
MitsubishiClient client = new MitsubishiClient(MitsubishiVersion.Qna_3E, "127.0.0.1",6000);

//2、写操作
client.Write("M100", true);
client.Write("D200", (short)11);
client.Write("D210", 33);

//3、读操作
var value1 = client.ReadBoolean("M100").Value;
var value2 = client.ReadInt16("D200").Value;
var value3 = client.ReadInt32("D210").Value;

//4、如果没有主动Open，则会每次读写操作的时候自动打开自动和关闭连接，这样会使读写效率大大减低。所以建议手动Open和Close。
client.Open();

//5、读写操作都会返回操作结果对象Result
var result = client.ReadInt16("D210");
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

## OmronFinsClient(欧姆龙)读写操作
```
//1、实例化客户端 - 输入正确的IP和端口
OmronFinsClient client = new OmronFinsClient("127.0.0.1",6000);

//2、写操作
client.Write("M100", true);
client.Write("D200", (short)11);
client.Write("D210", 33);

//3、读操作
var value1 = client.ReadBoolean("M100").Value;
var value2 = client.ReadInt16("D200").Value;
var value3 = client.ReadInt32("D210").Value;

//4、如果没有主动Open，则会每次读写操作的时候自动打开自动和关闭连接，这样会使读写效率大大减低。所以建议手动Open和Close。
client.Open();

//5、读写操作都会返回操作结果对象Result
var result = client.ReadInt16("D210");
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

## 其他更多详细使用请[参考](https://github.com/zhaopeiym/IoTClient.Examples)

# [IoTClient Tool](https://github.com/zhaopeiym/IoTClient.Examples)效果图   
![image](https://user-images.githubusercontent.com/5820324/115138587-b7bebc80-a05f-11eb-9f7c-720a88bdca6e.png)  

![image](https://user-images.githubusercontent.com/5820324/115138592-bbeada00-a05f-11eb-9fc4-4b15a426cdb3.png)    

![image](https://user-images.githubusercontent.com/5820324/115138594-bd1c0700-a05f-11eb-8d4b-34a567669e3d.png)

![image](https://user-images.githubusercontent.com/5820324/115138596-bee5ca80-a05f-11eb-9878-9b05a4cfbc0b.png)  

![image](https://user-images.githubusercontent.com/5820324/115138597-c016f780-a05f-11eb-9d09-298a54f55266.png)  

![image](https://user-images.githubusercontent.com/5820324/115138600-c2795180-a05f-11eb-92b0-1a1d278c20c8.png)  

![image](https://user-images.githubusercontent.com/5820324/115138602-c3aa7e80-a05f-11eb-9cd7-be876735a26f.png)  

![image](https://user-images.githubusercontent.com/5820324/115138603-c5744200-a05f-11eb-9cdb-a222aa9b7b25.png)  

![image](https://user-images.githubusercontent.com/5820324/115138606-c73e0580-a05f-11eb-9ca1-5ece1bae8e71.png)  

![image](https://user-images.githubusercontent.com/5820324/115138607-c86f3280-a05f-11eb-83f1-d1706331406a.png)  
