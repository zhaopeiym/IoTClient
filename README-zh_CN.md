<h1 align="center">
IoTClient 
</h1>

## [English](README.md) | 简体中文

[![image](https://img.shields.io/nuget/v/IoTClient.svg)](https://www.nuget.org/packages/IoTClient/) [![image](https://img.shields.io/nuget/dt/IoTClient.svg)](https://www.nuget.org/packages/IoTClient/) ![image](https://img.shields.io/github/license/alienwow/SnowLeopard.svg)

- 这是一个物联网设备通讯协议实现客户端，将包括主流PLC通信读取、ModBus协议、Bacnet协议等常用工业通讯协议。
- 本组件基于.NET Standard 2.0，可用于.Net的跨平台开发，如Windows、Linux甚至可运行于树莓派上。
- 本组件终身开源免费，采用最宽松MIT协议，您也可以随意修改和商业使用（商业使用请做好评估和测试）。  
- 开发工具：Visual Studio 2019 
- QQ交流群：[700324594](https://jq.qq.com/?_wv=1027&k=tIRmmGbt)  

## 文档目录
<!-- TOC -->

- [使用说明](#%E4%BD%BF%E7%94%A8%E8%AF%B4%E6%98%8E)
    - [引用组件](#%E5%BC%95%E7%94%A8%E7%BB%84%E4%BB%B6)
    - [ModBusTcp读写操作](#modbustcp%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [ModBusRtu读写操作](#modbusrtu%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [ModBusAscii读写操作](#modbusascii%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [ModbusRtuOverTcp读写操作](#modbusrtuovertcp%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [SiemensClient(西门子)读写操作](#siemensclient%E8%A5%BF%E9%97%A8%E5%AD%90%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [注意：关于Siemens的PLC地址](#%E6%B3%A8%E6%84%8F%E5%85%B3%E4%BA%8Esiemens%E7%9A%84plc%E5%9C%B0%E5%9D%80)
    - [SiemensClient最佳实践](#siemensclient%E6%9C%80%E4%BD%B3%E5%AE%9E%E8%B7%B5)
    - [MitsubishiClient(三菱)读写操作](#mitsubishiclient%E4%B8%89%E8%8F%B1%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [OmronFinsClient(欧姆龙)读写操作](#omronfinsclient%E6%AC%A7%E5%A7%86%E9%BE%99%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
    - [AllenBradleyClient(罗克韦尔)读写操作](#allenbradleyclient%E7%BD%97%E5%85%8B%E9%9F%A6%E5%B0%94%E8%AF%BB%E5%86%99%E6%93%8D%E4%BD%9C)
- [基于IoTClient库的一些项目](#%E5%9F%BA%E4%BA%8Eiotclient%E5%BA%93%E7%9A%84%E4%B8%80%E4%BA%9B%E9%A1%B9%E7%9B%AE)
    - [IoTClient Tool 桌面程序工具（开源）](#iotclient-tool-%E6%A1%8C%E9%9D%A2%E7%A8%8B%E5%BA%8F%E5%B7%A5%E5%85%B7%E5%BC%80%E6%BA%90)
    - [iotgateway（开源）](#iotgateway%E5%BC%80%E6%BA%90)
    - [能源管理系统（商用）](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86%E7%B3%BB%E7%BB%9F%E5%95%86%E7%94%A8)
        - [能源管理-现场-单项目](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86-%E7%8E%B0%E5%9C%BA-%E5%8D%95%E9%A1%B9%E7%9B%AE)
        - [能源管理-云端-多项目](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86-%E4%BA%91%E7%AB%AF-%E5%A4%9A%E9%A1%B9%E7%9B%AE)
        - [能源管理-移动端](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86-%E7%A7%BB%E5%8A%A8%E7%AB%AF)
    - [海底捞末端控制（商用）](#%E6%B5%B7%E5%BA%95%E6%8D%9E%E6%9C%AB%E7%AB%AF%E6%8E%A7%E5%88%B6%E5%95%86%E7%94%A8)
        - [海底捞末端控制-web](#%E6%B5%B7%E5%BA%95%E6%8D%9E%E6%9C%AB%E7%AB%AF%E6%8E%A7%E5%88%B6-web)
        - [海底捞末端控制-移动端](#%E6%B5%B7%E5%BA%95%E6%8D%9E%E6%9C%AB%E7%AB%AF%E6%8E%A7%E5%88%B6-%E7%A7%BB%E5%8A%A8%E7%AB%AF)
    - [越邦智能分拣系统（商用）](#%E8%B6%8A%E9%82%A6%E6%99%BA%E8%83%BD%E5%88%86%E6%8B%A3%E7%B3%BB%E7%BB%9F%E5%95%86%E7%94%A8)
    - [电表监控系统（商用）](#%E7%94%B5%E8%A1%A8%E7%9B%91%E6%8E%A7%E7%B3%BB%E7%BB%9F%E5%95%86%E7%94%A8)
    - [人造板行业生产管理软件（商用）](#%E4%BA%BA%E9%80%A0%E6%9D%BF%E8%A1%8C%E4%B8%9A%E7%94%9F%E4%BA%A7%E7%AE%A1%E7%90%86%E8%BD%AF%E4%BB%B6%E5%95%86%E7%94%A8)
- [友情链接](#%E5%8F%8B%E6%83%85%E9%93%BE%E6%8E%A5)
    - [YuebonCore](#yueboncore)
    - [iotgateway](#iotgateway)

<!-- /TOC -->

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

//2.1、【注意】写入数据的时候需要明确数据类型
client.Write("0", (short)33, 2, 16);    //写入short类型数值
client.Write("4", (ushort)33, 2, 16);   //写入ushort类型数值
client.Write("8", (int)33, 2, 16);      //写入int类型数值
client.Write("12", (uint)33, 2, 16);    //写入uint类型数值
client.Write("16", (long)33, 2, 16);    //写入long类型数值
client.Write("20", (ulong)33, 2, 16);   //写入ulong类型数值
client.Write("24", (float)33, 2, 16);   //写入float类型数值
client.Write("28", (double)33, 2, 16);  //写入double类型数值
client.Write("32", true, 2, 5);         //写入线圈类型值
client.Write("100", "orderCode", stationNumber);  //写入字符串

//3、读操作 - 参数依次是：地址 、站号 、功能码
var value = client.ReadInt16("4", 2, 3).Value;

//3.1、其他类型数据读取
client.ReadInt16("0", stationNumber, 3);    //short类型数据读取
client.ReadUInt16("4", stationNumber, 3);   //ushort类型数据读取
client.ReadInt32("8", stationNumber, 3);    //int类型数据读取
client.ReadUInt32("12", stationNumber, 3);  //uint类型数据读取
client.ReadInt64("16", stationNumber, 3);   //long类型数据读取
client.ReadUInt64("20", stationNumber, 3);  //ulong类型数据读取
client.ReadFloat("24", stationNumber, 3);   //float类型数据读取
client.ReadDouble("28", stationNumber, 3);  //double类型数据读取
client.ReadCoil("32", stationNumber, 1);    //线圈类型数据读取
client.ReadDiscrete("32", stationNumber, 2);//离散类型数据读取
client.ReadString("100", stationNumber,10); //读取字符串

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
ModBusTcp更多使用方式，请参考[单元测试](https://github.com/zhaopeiym/IoTClient/blob/master/IoTClient.Tests/Modbus_Tests/ModBusTcpClient_tests.cs)  

## ModBusRtu读写操作
```
//实例化客户端 - [COM端口名称,波特率,数据位,停止位,奇偶校验]
ModBusRtuClient client = new ModBusRtuClient("COM3", 9600, 8, StopBits.One, Parity.None);

//其他读写操作和ModBusTcpClient的读写操作一致
```

## ModBusAscii读写操作
```
//实例化客户端 - [COM端口名称,波特率,数据位,停止位,奇偶校验]
ModbusAsciiClient client = new ModbusAsciiClient("COM3", 9600, 8, StopBits.One, Parity.None);

//其他读写操作和ModBusTcpClient的读写操作一致
```

## ModbusRtuOverTcp读写操作
```
//串口透传 即:用Tcp的方式发送Rtu格式报文

//实例化客户端 - IP、端口、超时时间、大小端设置
ModbusRtuOverTcpClient client = new ModbusRtuOverTcpClient("127.0.0.1", 502, 1500, EndianFormat.ABCD);

//其他读写操作和ModBusTcpClient的读写操作一致
```

## SiemensClient(西门子)读写操作
```
//1、实例化客户端 - 输入型号、IP和端口
//其他型号：SiemensVersion.S7_200、SiemensVersion.S7_300、SiemensVersion.S7_400、SiemensVersion.S7_1200、SiemensVersion.S7_1500
SiemensClient client = new SiemensClient(SiemensVersion.S7_200Smart, "127.0.0.1",102);

//2、写操作
client.Write("Q1.3", true);
client.Write("V2205", (short)11);
client.Write("V2209", 33);
client.Write("V2305", "orderCode");             //写入字符串

//3、读操作
var value1 = client.ReadBoolean("Q1.3").Value;
var value2 = client.ReadInt16("V2205").Value;
var value3 = client.ReadInt32("V2209").Value;
var value4 = client.ReadString("V2305").Value; //读取字符串

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

## AllenBradleyClient(罗克韦尔)读写操作
```
//1、实例化客户端 - 输入正确的IP和端口
AllenBradleyClient client = new AllenBradleyClient("127.0.0.1",44818);

//2、写操作 
client.Write("A1", (short)11); 

//3、读操作
var value = client.ReadInt16("A1").Value;

//4、如果没有主动Open，则会每次读写操作的时候自动打开自动和关闭连接，这样会使读写效率大大减低。所以建议手动Open和Close。
client.Open();

//5、读写操作都会返回操作结果对象Result
var result = client.ReadInt16("A1");
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

# 基于IoTClient库的一些项目

## IoTClient Tool 桌面程序工具（开源）
所属个人：[农码一生](https://www.cnblogs.com/zhaopei)  

### [IoTClient Tool](https://github.com/zhaopeiym/IoTClient.Examples/releases/download/1.0.3/IoTClient.exe) 桌面程序工具，[开源地址](https://github.com/zhaopeiym/IoTClient.Examples)。     

- 1、可用来测试PLC和相关协议的通信 
- 2、可作为IoTClient库使用例子。

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

## iotgateway（开源）
所属个人：[iioter](https://gitee.com/iioter)  

### [iotgateway](https://gitee.com/iioter/iotgateway)
![image](https://user-images.githubusercontent.com/29589505/147056511-14611d19-8498-4a3c-bd67-3749ab75462f.gif)


## 能源管理系统（商用）
所属公司：[擎呐科技](https://qingnakeji.com)   

### 能源管理-现场-单项目
![image](https://user-images.githubusercontent.com/5820324/117001443-f10c5300-ad14-11eb-8597-bcc6e573c542.png)  
![image](https://user-images.githubusercontent.com/5820324/117001444-f1a4e980-ad14-11eb-80ea-0972211e46a1.png)   

### 能源管理-云端-多项目
![image](https://user-images.githubusercontent.com/5820324/117001447-f23d8000-ad14-11eb-9771-1854b13bef4b.png)  
![image](https://user-images.githubusercontent.com/5820324/117001451-f2d61680-ad14-11eb-9507-bf4123e5cbe8.png)  
![image](https://user-images.githubusercontent.com/5820324/117001454-f36ead00-ad14-11eb-8ea1-e993298eca9b.png)  
![image](https://user-images.githubusercontent.com/5820324/117001460-f49fda00-ad14-11eb-8c75-eb88a24983b6.png)  
![image](https://user-images.githubusercontent.com/5820324/117001461-f5d10700-ad14-11eb-9d82-d73a7347ad32.png)  
![image](https://user-images.githubusercontent.com/5820324/117001464-f6699d80-ad14-11eb-8810-50b20f8954ae.png)  
![image](https://img2020.cnblogs.com/blog/208266/202106/208266-20210630094808579-2089270828.svg)   

### 能源管理-移动端
![image](https://user-images.githubusercontent.com/5820324/116964170-796f0180-acdd-11eb-9514-fd9a05c15eae.png)![image](https://user-images.githubusercontent.com/5820324/116964172-7a079800-acdd-11eb-91ac-13c1a321145d.png)![image](https://user-images.githubusercontent.com/5820324/116964174-7aa02e80-acdd-11eb-8051-158f13ed2993.png)![image](https://user-images.githubusercontent.com/5820324/116964175-7b38c500-acdd-11eb-80b4-97827ee03374.png)![image](https://user-images.githubusercontent.com/5820324/116964177-7c69f200-acdd-11eb-94b8-ddbf5081ddaf.png)![image](https://user-images.githubusercontent.com/5820324/116964179-7d028880-acdd-11eb-95c6-601e235e3b6b.png)![image](https://user-images.githubusercontent.com/5820324/116964181-7d9b1f00-acdd-11eb-9914-911167e0af05.png)

## 海底捞末端控制（商用）
所属公司：[擎呐科技](https://qingnakeji.com)   

### 海底捞末端控制-web
![image](https://user-images.githubusercontent.com/5820324/117001939-87d90f80-ad15-11eb-8848-7a4956ba1ce9.png)  
![image](https://user-images.githubusercontent.com/5820324/117001942-87d90f80-ad15-11eb-85b2-778cadaf85ad.png)  
![image](https://user-images.githubusercontent.com/5820324/117001947-890a3c80-ad15-11eb-9e28-57e8b05cd04c.png)  
![image](https://user-images.githubusercontent.com/5820324/117001949-89a2d300-ad15-11eb-9226-2e2683e2cc7f.png)  

### 海底捞末端控制-移动端
![image](https://user-images.githubusercontent.com/5820324/116964517-5002a580-acde-11eb-9bfb-c859a57307c7.png)![image](https://user-images.githubusercontent.com/5820324/116964519-509b3c00-acde-11eb-8245-573ac3fa7f16.png)![image](https://user-images.githubusercontent.com/5820324/116964521-5133d280-acde-11eb-85de-b09dde1ca41e.png)![image](https://user-images.githubusercontent.com/5820324/116964525-51cc6900-acde-11eb-924f-f3320e4a179c.png)

## 越邦智能分拣系统（商用）
所属公司：[越邦科技](http://www.yuebon.com)  
![image](https://user-images.githubusercontent.com/5820324/124534498-fa19bd80-de46-11eb-9c6d-89c6e7323ed5.jpg)  
![image](https://user-images.githubusercontent.com/5820324/124534506-fdad4480-de46-11eb-983c-b074b72e0922.jpg)  

## 电表监控系统（商用）
所属公司：[哈工大机器人南昌智能制造研究院](http://www.hrgrobotics.cn)  
![image](https://user-images.githubusercontent.com/5820324/124694128-3f0b2600-df13-11eb-9266-fa7d2ef7de11.png)  
![image](https://user-images.githubusercontent.com/5820324/124694129-403c5300-df13-11eb-8452-12f73260a7a9.png)  

## 人造板行业生产管理软件（商用）
所属公司：广州赛志科技有限公司  
![image](https://user-images.githubusercontent.com/5820324/127763049-3018fefd-d233-41de-8c8e-e02e03776690.png)  
![image](https://user-images.githubusercontent.com/5820324/127763052-dec0a2e8-bb9f-421f-a72b-08bb5ac50709.png)  
![image](https://user-images.githubusercontent.com/5820324/127763055-068ec3a4-4959-47fd-928b-6bfa12ef7ed7.png)  

# 友情链接

## YuebonCore
开源地址：  
https://gitee.com/yuebon/YuebonNetCore  

概述：  
YuebonCore是基于.Net5.0开发的权限管理及快速开发框架，整合应用最新技术包括Asp.NetCore MVC、Dapper、WebAPI、Swagger、EF、Vue等，核心模块包括：组织机构、角色用户、权限授权、多系统、多应用管理、定时任务、业务单据编码规则、代码生成器等。它的架构易于扩展，规范了一套业务实现的代码结构与操作流程，使YuebonCore框架更易于应用到实际项目开发中。  

## iotgateway
开源地址：  
https://gitee.com/iioter/iotgateway  

概述：  
基于.net5的跨平台物联网网关。通过可视化配置，轻松的连接到你的任何设备和系统(如PLC、扫码枪、CNC、数据库、串口设备、上位机、OPC Server、OPC UA Server、Mqtt Server等)，从而与 Thingsboard、IoTSharp或您自己的物联网平台进行双向数据通讯。提供简单的驱动开发接口；当然也可以进行边缘计算。  