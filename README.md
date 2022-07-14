<h1 align="center">
IoTClient 
</h1>

## English | [中文文档](README-zh_CN.md)

[![image](https://img.shields.io/nuget/v/IoTClient.svg)](https://www.nuget.org/packages/IoTClient/) [![image](https://img.shields.io/nuget/dt/IoTClient.svg)](https://www.nuget.org/packages/IoTClient/) ![image](https://img.shields.io/github/license/alienwow/SnowLeopard.svg)

- This is an IoT device communication protocol realization client, which will include mainstream PLC communication reading, ModBus protocol, Bacnet protocol and other common industrial communication protocols.
- This component is based on .NET Standard 2.0 and can be used for cross-platform development of .Net, such as Windows, Linux and even run on Raspberry Pi.
- This component is open source and free for life, and adopts the most relaxed MIT protocol. You can also modify and use it for commercial use (commercial use please evaluate and test).
- Development tools：Visual Studio 2019 
- QQ exchange group：[700324594](https://jq.qq.com/?_wv=1027&k=tIRmmGbt)  

## Document directory
<!-- TOC -->
    
- [Instructions for use](#instructions-for-use)
    - [Reference component](#reference-component)
    - [ModBusTcp read and write operations](#modbustcp-read-and-write-operations)
    - [ModBusRtu read and write operations](#modbusrtu-read-and-write-operations)
    - [ModBusAscii read and write operations](#modbusascii-read-and-write-operations)
    - [ModbusRtuOverTcp read and write operations](#modbusrtuovertcp-read-and-write-operations)
    - [SiemensClient (Siemens) read and write operations](#siemensclient-siemens-read-and-write-operations)
    - [Note: About Siemens PLC address](#note-about-siemens-plc-address)
    - [SiemensClient best practices](#siemensclient-best-practices)
    - [MitsubishiClient (Mitsubishi) read and write operations](#mitsubishiclient-mitsubishi-read-and-write-operations)
    - [OmronFinsClient (Omron) read and write operations](#omronfinsclient-omron-read-and-write-operations)
    - [AllenBradleyClient read and write operations](#allenbradleyclient-read-and-write-operations)
- [Some projects based on IoTClient library](#some-projects-based-on-iotclient-library)
    - [IoTClient Tool Desktop program tool (open source)](#iotclient-tool-desktop-program-tool-open-source)        
    - [Energy Management System (Commercial)](#energy-management-system-commercial)
        - [能源管理-现场-单项目](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86-%E7%8E%B0%E5%9C%BA-%E5%8D%95%E9%A1%B9%E7%9B%AE)
        - [能源管理-云端-多项目](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86-%E4%BA%91%E7%AB%AF-%E5%A4%9A%E9%A1%B9%E7%9B%AE)
        - [能源管理-移动端](#%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86-%E7%A7%BB%E5%8A%A8%E7%AB%AF)
    - [Haidilao terminal control (commercial)](#haidilao-terminal-control-commercial)
        - [海底捞末端控制-web](#%E6%B5%B7%E5%BA%95%E6%8D%9E%E6%9C%AB%E7%AB%AF%E6%8E%A7%E5%88%B6-web)
        - [海底捞末端控制-移动端](#%E6%B5%B7%E5%BA%95%E6%8D%9E%E6%9C%AB%E7%AB%AF%E6%8E%A7%E5%88%B6-%E7%A7%BB%E5%8A%A8%E7%AB%AF)

<!-- /TOC -->

# Instructions for use

## Reference component
[Nuget installation](https://www.nuget.org/packages/IoTClient/) ```Install-Package IoTClient ```  
Or graphical installation   
![image](https://user-images.githubusercontent.com/5820324/68722366-2fc5bf00-05f0-11ea-8282-f2b0a58a9f9d.png)  

## ModBusTcp read and write operations
```
//1、Instantiate the client-enter the correct IP and port
ModBusTcpClient client = new ModBusTcpClient("127.0.0.1", 502);

//2、Write operation-parameters are: address, value, station number, function code
client.Write("4", (short)33, 2, 16);

//2.1、[Note] When writing data, you need to clarify the data type
client.Write("0", (short)33, 2, 16);    //Write short type value
client.Write("4", (ushort)33, 2, 16);   //Write ushort type value
client.Write("8", (int)33, 2, 16);      //Write int type value
client.Write("12", (uint)33, 2, 16);    //Write uint type value
client.Write("16", (long)33, 2, 16);    //Write long type value
client.Write("20", (ulong)33, 2, 16);   //Write ulong type value
client.Write("24", (float)33, 2, 16);   //Write float type value
client.Write("28", (double)33, 2, 16);  //Write double type value
client.Write("32", true, 2, 5);         //Write Coil type value
client.Write("100", "orderCode", stationNumber);  //Write string

//3、Read operation-the parameters are: address, station number, function code
var value = client.ReadInt16("4", 2, 3).Value;

//3.1、Other types of data reading
client.ReadInt16("0", stationNumber, 3);    //short type data read
client.ReadUInt16("4", stationNumber, 3);   //ushort type data read
client.ReadInt32("8", stationNumber, 3);    //int type data read
client.ReadUInt32("12", stationNumber, 3);  //uint type data read
client.ReadInt64("16", stationNumber, 3);   //long type data read
client.ReadUInt64("20", stationNumber, 3);  //ulong type data read
client.ReadFloat("24", stationNumber, 3);   //float type data read
client.ReadDouble("28", stationNumber, 3);  //double type data read
client.ReadCoil("32", stationNumber, 1);    //Coil type data read
client.ReadDiscrete("32", stationNumber, 2);//Discrete type data read
client.ReadString("100", stationNumber,readLength:10); //Read string

//4、If there is no active Open, it will automatically open and close the connection every time you read and write operations, which will greatly reduce the efficiency of reading and writing. So it is recommended to open and close manually.
client.Open();

//5、Read and write operations will return the operation result object Result
var result = client.ReadInt16("4", 2, 3);
//5.1 Whether the reading is successful (true or false)
var isSucceed = result.IsSucceed;
//5.2 Exception information for failed reading
var errMsg = result.Err;
//5.3 Read the request message actually sent by the operation
var requst  = result.Requst;
//5.4 Read the response message from the server
var response = result.Response;
//5.5 Read value
var value3 = result.Value;

//6、Batch read
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

//7、Other parameters of the constructor
//IP, port, timeout time, big and small end settings
ModBusTcpClient client = new ModBusTcpClient("127.0.0.1", 502, 1500, EndianFormat.ABCD);
``` 
For more usage of ModBusTcp, please refer to [Unit Test](https://github.com/zhaopeiym/IoTClient/blob/master/IoTClient.Tests/Modbus_Tests/ModBusTcpClient_tests.cs)  

## ModBusRtu read and write operations
```
//Instantiate the client-[COM port name, baud rate, data bits, stop bits, parity]
ModBusRtuClient client = new ModBusRtuClient("COM3", 9600, 8, StopBits.One, Parity.None);

//Other read and write operations are the same as ModBusTcpClient's read and write operations
```

## ModBusAscii read and write operations
```
//Instantiate the client-[COM port name, baud rate, data bits, stop bits, parity]
ModbusAsciiClient client = new ModbusAsciiClient("COM3", 9600, 8, StopBits.One, Parity.None);

//Other read and write operations are the same as ModBusTcpClient's read and write operations
```

## ModbusRtuOverTcp read and write operations
```
//Serial port transparent transmission i.e.: send Rtu format messages in Tcp mode

//Instantiate the client-IP, port, timeout, big and small end settings
ModbusRtuOverTcpClient client = new ModbusRtuOverTcpClient("127.0.0.1", 502, 1500, EndianFormat.ABCD);

//Other read and write operations are the same as ModBusTcpClient's read and write operations
```

## SiemensClient (Siemens) read and write operations
```
//1、Instantiate the client-enter the model, IP and port
//Other models：SiemensVersion.S7_200、SiemensVersion.S7_300、SiemensVersion.S7_400、SiemensVersion.S7_1200、SiemensVersion.S7_1500
SiemensClient client = new SiemensClient(SiemensVersion.S7_200Smart, "127.0.0.1",102);

//2、Write operation
client.Write("Q1.3", true);
client.Write("V2205", (short)11);
client.Write("V2209", 33);

//3、Read operation
var value1 = client.ReadBoolean("Q1.3").Value;
var value2 = client.ReadInt16("V2205").Value;
var value3 = client.ReadInt32("V2209").Value;

//4、If there is no active Open, it will automatically open and close the connection every time you read and write operations, which will greatly reduce the efficiency of reading and writing. So it is recommended to open and close manually.
client.Open();

//5、Read and write operations will return the operation result object Result
var result = client.ReadInt16("V2205");
//5.1 Whether the reading is successful (true or false)
var isSucceed = result.IsSucceed;
//5.2 Exception information for failed reading
var errMsg = result.Err;
//5.3 Read the request message actually sent by the operation
var requst  = result.Requst;
//5.4 Read the response message from the server
var response = result.Response;
//5.5 Read value
var value4 = result.Value;

```

## Note: About Siemens PLC address
```
VB263、VW263、VD263中的B、W、D分别表示：byte型(8位)、word型(16位)、doubleword型(32位)。

When this component passes in the address, there is no need to carry the data type, just use the corresponding method to read the corresponding type, such as:
VB263       - client.ReadByte("V263")
VD263       - client.ReadFloat("V263")
VD263       - client.ReadInt32("V263")
DB108.DBW4  - client.ReadUInt16("DB108.4")
DB1.DBX0.0  - client.ReadBoolean("DB1.0.0")
DB1.DBD0    - client.ReadFloat("DB1.0")
```
|C# data type | smart200 | 1200/1500/300
|---|---|---
|bit | V1.0 | DB1.DBX1.0
|byte | VB1 | DB1.DBB1
|shor <br> ushort  | VW2 | DB1.DBW2
|int <br> uint <br> float | VD4 | DB1.DBD4

## SiemensClient best practices
```
1、When not to take the initiative to open
Siemens plc generally allows up to 8 long connections. So when the number of connections is not enough or when doing testing, do not take the initiative to open, so that the component will automatically open and close immediately.

2、When to take the initiative to open
When the number of long connections is enough, and you want to improve the read and write performance.

3、In addition to active Open connections, batch read and write can also greatly improve read and write performance.
//Batch read
Dictionary<string, DataTypeEnum> addresses = new Dictionary<string, DataTypeEnum>();
addresses.Add("DB4.24", DataTypeEnum.Float);
addresses.Add("DB1.434.0", DataTypeEnum.Bool);
addresses.Add("V4109", DataTypeEnum.Byte);
...
var result = client.BatchRead(addresses);

//Batch write
Dictionary<string, object> addresses = new Dictionary<string, object>();
addresses.Add("DB4.24", (float)1);
addresses.Add("DB4.0", (float)2);
addresses.Add("DB1.434.0", true);
...
var result = client.BatchWrite(addresses);

4、[Note] When writing data, you need to clarify the data type
client.Write("DB4.12", 9);          //What is written is of type int
client.Write("DB4.12", (float)9);   //What is written is a float type

5、SiemensClient is a thread safe class
Due to limited long PLC connections, SiemensClient is designed as a thread-safe class. You can set SiemensClient as a singleton, and use the instance of SiemensClient to read and write PLC between multiple threads.
```

## MitsubishiClient (Mitsubishi) read and write operations
```
//1、Instantiate the client-enter the correct IP and port
MitsubishiClient client = new MitsubishiClient(MitsubishiVersion.Qna_3E, "127.0.0.1",6000);

//2、Write operation
client.Write("M100", true);
client.Write("D200", (short)11);
client.Write("D210", 33);

//3、Read operation
var value1 = client.ReadBoolean("M100").Value;
var value2 = client.ReadInt16("D200").Value;
var value3 = client.ReadInt32("D210").Value;

//4、If there is no active Open, it will automatically open and close the connection every time you read and write operations, which will greatly reduce the efficiency of reading and writing. So it is recommended to open and close manually.
client.Open();

//5、Read and write operations will return the operation result object Result
var result = client.ReadInt16("D210");
//5.1 Whether the reading is successful (true or false)
var isSucceed = result.IsSucceed;
//5.2 Exception information for failed reading
var errMsg = result.Err;
//5.3 Read the request message actually sent by the operation
var requst  = result.Requst;
//5.4 Read the response message from the server
var response = result.Response;
//5.5 Read value
var value4 = result.Value;
```

## OmronFinsClient (Omron) read and write operations
```
//1、Instantiate the client-enter the correct IP and port
OmronFinsClient client = new OmronFinsClient("127.0.0.1",6000);

//2、Write operation
client.Write("M100", true);
client.Write("D200", (short)11);
client.Write("D210", 33);

//3、Read operation
var value1 = client.ReadBoolean("M100").Value;
var value2 = client.ReadInt16("D200").Value;
var value3 = client.ReadInt32("D210").Value;

//4、If there is no active Open, it will automatically open and close the connection every time you read and write operations, which will greatly reduce the efficiency of reading and writing. So it is recommended to open and close manually.
client.Open();

//5、Read and write operations will return the operation result object Result
var result = client.ReadInt16("D210");
//5.1 Whether the reading is successful (true or false)
var isSucceed = result.IsSucceed;
//5.2 Exception information for failed reading
var errMsg = result.Err;
//5.3 Read the request message actually sent by the operation
var requst  = result.Requst;
//5.4 Read the response message from the server
var response = result.Response;
//5.5 Read value
var value4 = result.Value;
```

## AllenBradleyClient read and write operations
```
//1、Instantiate the client-enter the correct IP and port
AllenBradleyClient client = new AllenBradleyClient("127.0.0.1",44818);

//2、Write operation 
client.Write("A1", (short)11); 

//3、Read operation
var value = client.ReadInt16("A1").Value;

//4、If there is no active Open, it will automatically open and close the connection every time you read and write operations, which will greatly reduce the efficiency of reading and writing. So it is recommended to open and close manually.
client.Open();

//5、Read and write operations will return the operation result object Result
var result = client.ReadInt16("A1");
//5.1 Whether the reading is successful (true or false)
var isSucceed = result.IsSucceed;
//5.2 Exception information for failed reading
var errMsg = result.Err;
//5.3 Read the request message actually sent by the operation
var requst  = result.Requst;
//5.4 Read the response message from the server
var response = result.Response;
//5.5 Read value
var value4 = result.Value;
```

# Some projects based on IoTClient library

## IoTClient Tool Desktop program tool (open source)

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

## Energy Management System (Commercial) 

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

## Haidilao terminal control (commercial)

### 海底捞末端控制-web
![image](https://user-images.githubusercontent.com/5820324/117001939-87d90f80-ad15-11eb-8848-7a4956ba1ce9.png)  
![image](https://user-images.githubusercontent.com/5820324/117001942-87d90f80-ad15-11eb-85b2-778cadaf85ad.png)  
![image](https://user-images.githubusercontent.com/5820324/117001947-890a3c80-ad15-11eb-9e28-57e8b05cd04c.png)  
![image](https://user-images.githubusercontent.com/5820324/117001949-89a2d300-ad15-11eb-9226-2e2683e2cc7f.png)  

### 海底捞末端控制-移动端
![image](https://user-images.githubusercontent.com/5820324/116964517-5002a580-acde-11eb-9bfb-c859a57307c7.png)![image](https://user-images.githubusercontent.com/5820324/116964519-509b3c00-acde-11eb-8245-573ac3fa7f16.png)![image](https://user-images.githubusercontent.com/5820324/116964521-5133d280-acde-11eb-85de-b09dde1ca41e.png)![image](https://user-images.githubusercontent.com/5820324/116964525-51cc6900-acde-11eb-924f-f3320e4a179c.png)
