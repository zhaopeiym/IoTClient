using IoTClient.Enums;
using IoTClient.Extensions.Adapter.Communication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;
using System.Net;
using System.Threading;

namespace IoTClient.Extensions.Adapter
{
    /// <summary>
    /// BACnet
    /// </summary>
    public class BACnetCommunication : IIoTClientCommon
    {
        private IPEndPoint IpEndPoint;

        public bool IsConnected => true;

        private string deviceId;

        public string ConnectionInfo => $"{IpEndPoint}:{deviceId}";

        public string DeviceVersion => "BACnet";

        private List<BacNode> devicesList = new List<BacNode>();
        private BacnetClient client;

        public BACnetCommunication(string ip, string deviceId, int port = 47808)
        {
            this.deviceId = deviceId;
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public Result Close()
        {
            try
            {
                client?.Dispose();
            }
            catch { }
            return new Result();
        }

        public Result Open()
        {
            lock (devicesList)
            {
                //Logger?.Debug($"Open-1 IP:{IpEndPoint.Address.ToString()} {IpEndPoint.Port.ToString()}", tag: "BACnetCommunication_Debug");

                if (client == null || devicesList == null || !devicesList.Any())
                {
                    //Logger?.Debug($"Open-2 IP:{IpEndPoint.Address.ToString()} {IpEndPoint.Port.ToString()}", tag: "BACnetCommunication_Debug");

                    try
                    {
                        client?.Dispose();
                    }
                    catch { }
                    //BACnet的默认端口47808 //, localEndpointIp: "192.168.10.4"
                    client = new BacnetClient(new BacnetIpUdpProtocolTransport(IpEndPoint.Port, false));
                    client.OnIam -= new BacnetClient.IamHandler(handler_OnIam);
                    client.OnIam += new BacnetClient.IamHandler(handler_OnIam);
                    client.Start();
                    client.WhoIs();
                }
            }
            return new Result();
        }

        #region private

        private void handler_OnIam(BacnetClient sender, BacnetAddress address, uint deviceId, uint maxAPDU, BacnetSegmentations segmentation, ushort vendorId)
        {
            lock (devicesList)
            {
                var adr = address.adr;

                //Logger?.Debug($"adr:{address.ToString()} IP:{IpEndPoint.Address.ToString()} {IpEndPoint.Port.ToString()}", tag: "BACnetCommunication_Debug");

                foreach (BacNode bn in devicesList)
                {
                    if (bn.GetAdd(deviceId) != null)
                    {
                        //Logger?.Debug($"adr:{address.ToString()} IP:{IpEndPoint.Address.ToString()} {IpEndPoint.Port.ToString()} deviceId:{devicesList}", tag: "BACnetCommunication_Debug");
                        return;   // Yes
                    }
                }

                if (IpEndPoint.Address.ToString() == $"{adr[0]}.{adr[1]}.{adr[2]}.{adr[3]}")
                {
                    devicesList.Add(new BacNode(address, deviceId));   // add it     
                    //Logger?.Debug("开始扫描，Scan()", tag: "BACnetCommunication_Debug");
                    Scan();//开始扫描
                }
            }
        }

        /// <summary>
        /// 扫描
        /// </summary>
        private void Scan()
        {
            foreach (var device in devicesList)
            {
                //获取子节点个数
                var deviceCount = GetDeviceArrayIndexCount(device) + 1;
                //扫描子节点(50可设置 配置 - 批量扫描)
                ScanPointsBatch(device, 50, deviceCount);
            }
        }

        //获取子节点个数
        private uint GetDeviceArrayIndexCount(BacNode device)
        {
            try
            {
                if (device.Address == null) return 0;
                var objectId = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device.DeviceId);
                var list = ReadScalarValue(device.Address, objectId, BacnetPropertyIds.PROP_OBJECT_LIST, 0);
                var rst = Convert.ToUInt32(list.Value);
                return rst;
            }
            catch (Exception ex)
            {
                //Logger?.Error(ex, ex.Message + "-2", tag: "BACnetCommunication_Debug");
            }
            return 0;
        }

        private BacnetValue ReadScalarValue(BacnetAddress address, BacnetObjectId oid, BacnetPropertyIds propertyId, uint arrayIndex)
        {
            try
            {
                return client.ReadPropertyRequest(address, oid, propertyId, arrayIndex);
            }
            catch (Exception ex)
            {
                //Logger?.Error(ex, ex.Message + "-1", tag: "BACnetCommunication_Debug");
            }
            return new BacnetValue();
        }

        /// <summary>
        /// 扫描设备
        /// </summary>
        private void ScanPointsBatch(BacNode device, uint partDeviceCount, uint deviceCount)
        {
            try
            {
                if (device == null) return;
                var pid = BacnetPropertyIds.PROP_OBJECT_LIST;
                var device_id = device.DeviceId;
                var bobj = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device_id);
                var adr = device.Address;
                if (adr == null) return;

                device.Properties.Clear();
                List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();
                for (uint i = 0; i < deviceCount; i++)
                {
                    rList.Add(new BacnetPropertyReference((uint)pid, i));
                    if ((i != 0 && i % partDeviceCount == 0) || i == deviceCount - 1)//不要超了 MaxAPDU
                    {
                        IList<BacnetReadAccessResult> lstAccessRst = client.ReadPropertyMultipleRequest(adr, bobj, rList);
                        if (lstAccessRst?.Any() ?? false)
                        {
                            foreach (var aRst in lstAccessRst)
                            {
                                if (aRst.values == null) continue;
                                foreach (var bPValue in aRst.values)
                                {
                                    if (bPValue.value == null) continue;
                                    foreach (var bValue in bPValue.value)
                                    {
                                        var strBValue = "" + bValue.Value;
                                        var strs = strBValue.Split(':');
                                        if (strs.Length < 2) continue;
                                        var strType = strs[0];
                                        var strObjId = strs[1];
                                        var subNode = new BacProperty();
                                        Enum.TryParse(strType, out BacnetObjectTypes otype);
                                        if (otype == BacnetObjectTypes.OBJECT_NOTIFICATION_CLASS || otype == BacnetObjectTypes.OBJECT_DEVICE) continue;
                                        subNode.ObjectId = new BacnetObjectId(otype, Convert.ToUInt32(strObjId));
                                        device.Properties.Add(subNode); //添加属性
                                    }
                                }
                            }
                        }
                        rList.Clear();//清空
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger?.Error(ex, ex.Message + "-3", tag: "BACnetCommunication_Debug");
            }
        }
        #endregion

        public Result<object> Read(string address)
        {
            var instance = int.Parse(address.Split('_')[0]);
            var objectType = int.Parse(address.Split('_')[1]);
            var result = new Result<object>();
            var rpop = devicesList.SelectMany(t => t.Properties).Where(t => t.ObjectId.Instance == instance && t.ObjectId.Type == (BacnetObjectTypes)objectType).FirstOrDefault();
            if (rpop == null)
            {
                result.Err = $"没有找到对应的点:{address},devicesList:{JsonConvert.SerializeObject(devicesList)},adr:{devicesList.FirstOrDefault()?.Address.ToString()}";
                result.IsSucceed = false;
                return result;
            }

            var bacnet = devicesList.Where(t => t.DeviceId.ToString() == deviceId && t.Properties.Any(p => p.ObjectId.Instance == instance && p.ObjectId.Type == (BacnetObjectTypes)objectType)).FirstOrDefault();
            int retry = 0;//重试
            tag_retry:
            try
            {
                Thread.Sleep(retry * 200);
                IList<BacnetValue> NoScalarValue = client.ReadPropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE);

                if (NoScalarValue?.Any() ?? false)
                {
                    result.Value = NoScalarValue[0].Value;
                }
                else
                {
                    retry++;
                    //Logger?.Warning($"读取:{address}重试次数[{retry}]", tag: "BACnetCommunication_Debug");
                    if (retry < 4) goto tag_retry;
                    result.Err = $"读取失败:{address}";
                    result.IsSucceed = false;
                }
            }
            catch (Exception ex)
            {
                retry++;
                //Logger?.Warning($"读取:{address}重试次数[{retry}]", tag: "BACnetCommunication_Debug");
                if (retry < 4) goto tag_retry;
                result.Err = $"读取失败:{address},{ex.Message}";
                result.IsSucceed = false;
            }
            return result;
        }

        /// <summary>
        /// 批量读取
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="batchNumber">经过测试9是最大批量数，可根据实际情况进行调整</param>
        /// <returns></returns>
        public Result<Dictionary<string, object>> BatchRead(Dictionary<string, DataTypeEnum> addresses, int batchNumber)
        {
            var reuslt = new Result<Dictionary<string, object>>();
            reuslt.Value = new Dictionary<string, object>();

            var batchCount = Math.Ceiling((float)addresses.Count / batchNumber);
            //Logger?.Debug($"batchCount:{batchCount}", tag: "temp");

            List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();
            rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_PRESENT_VALUE, uint.MaxValue));
            BacnetAddress address = devicesList.FirstOrDefault()?.Address;
            List<BacnetReadAccessResult> lstAccessRst = new List<BacnetReadAccessResult>();
            for (int i = 0; i < batchCount; i++)
            {
                IList<BacnetReadAccessSpecification> properties = addresses.Skip(i * batchNumber).Take(batchNumber)
                    .Select(t =>
                    {
                        var instance = uint.Parse(t.Key.Split('_')[0]);
                        var objectType = int.Parse(t.Key.Split('_')[1]);
                        return new BacnetReadAccessSpecification(new BacnetObjectId((BacnetObjectTypes)objectType, instance), rList);
                    }).ToList();
                //批量读取
                lstAccessRst.AddRange(client.ReadPropertyMultipleRequest(address, properties));
            }

            foreach (var aRst in lstAccessRst)
            {
                if (aRst.values == null) continue;

                object PROP_PRESENT_VALUE = null;
                foreach (var bPValue in aRst.values)
                {
                    if (bPValue.value == null || bPValue.value.Count == 0) continue;
                    var pid = (BacnetPropertyIds)(bPValue.property.propertyIdentifier);
                    var bValue = bPValue.value.First();
                    switch (pid)
                    {
                        case BacnetPropertyIds.PROP_PRESENT_VALUE://值
                            {
                                PROP_PRESENT_VALUE = bValue.Value;
                            }
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(PROP_PRESENT_VALUE?.ToString()))
                    reuslt.Value.Add($"{aRst.objectIdentifier.Instance}_{(int)aRst.objectIdentifier.Type}", PROP_PRESENT_VALUE);
            }
            return reuslt;
        }

        public Result<bool> ReadBoolean(string address)
        {
            var result = Read(address);
            return new Result<bool>(result, Convert.ToBoolean(result.Value));
        }

        public Result<byte> ReadByte(string address)
        {
            var result = Read(address);
            return new Result<byte>(result, Convert.ToByte(result.Value));
        }

        public Result<ushort> ReadUInt16(string address)
        {
            var result = Read(address);
            return new Result<ushort>(result, Convert.ToUInt16(result.Value));
        }

        public Result<short> ReadInt16(string address)
        {
            var result = Read(address);
            return new Result<short>(result, Convert.ToInt16(result.Value));
        }

        public Result<uint> ReadUInt32(string address)
        {
            var result = Read(address);
            return new Result<uint>(result, Convert.ToUInt32(result.Value));
        }

        public Result<int> ReadInt32(string address)
        {
            var result = Read(address);
            return new Result<int>(result, Convert.ToInt32(result.Value));
        }

        public Result<float> ReadFloat(string address)
        {
            var result = Read(address);
            return new Result<float>(result, Convert.ToSingle(result.Value));
        }

        public Result<ulong> ReadUInt64(string address)
        {
            var result = Read(address);
            return new Result<ulong>(result, Convert.ToUInt64(result.Value));
        }

        public Result<long> ReadInt64(string address)
        {
            var result = Read(address);
            return new Result<long>(result, Convert.ToInt64(result.Value));
        }

        public Result<double> ReadDouble(string address)
        {
            var result = Read(address);
            return new Result<double>(result, Convert.ToDouble(result.Value));
        }

        public Result<bool> WriteValue(string address, object value)
        {
            var result = new Result<bool>();
            var instance = int.Parse(address.Split('_')[0]?.Trim());
            var objectType = int.Parse(address.Split('_')[1]?.Trim());
            var rpop = devicesList.SelectMany(t => t.Properties).Where(t => t.ObjectId.Instance == instance && t.ObjectId.Type == (BacnetObjectTypes)objectType).FirstOrDefault();
            if (rpop == null)
            {
                result.Err = $"没有找到对应的点:{address}";
                result.IsSucceed = false;
                return result;
            }
            var bacnet = devicesList.Where(t => t.DeviceId.ToString() == deviceId && t.Properties.Any(p => p.ObjectId.Instance == instance && p.ObjectId.Type == (BacnetObjectTypes)objectType)).FirstOrDefault();
            BacnetValue[] NoScalarValue = { new BacnetValue(value) };
            int retry = 0;//重试
            tag_retry:
            try
            {
                Thread.Sleep(retry * 200);
                client.WritePropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE, NoScalarValue);
            }
            catch (Exception ex)
            {
                retry++;
                if (retry < 4) goto tag_retry;//强行重试                                            
                result.IsSucceed = false;
                result.Err = $"写入失败:{address},{ex.Message},Type:{value.GetType()}";
            }
            return result;
        }

        public Result Write(string address, int value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, uint value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, short value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, ushort value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, float value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, bool value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, byte value)
        {
            return WriteValue(address, value);
        }

        public Result BatchWrite(Dictionary<string, object> addresses, int batchNumber)
        {
            var result = new Result();
            foreach (var address in addresses)
            {
                DataTypeEnum dataType = DataTypeEnum.None;
                switch (address.Value.GetType().Name)
                {
                    case "Boolean":
                        dataType = DataTypeEnum.Bool; break;
                    case "UInt16":
                        dataType = DataTypeEnum.UInt16; break;
                    case "Int16":
                        dataType = DataTypeEnum.Int16; break;
                    case "UInt32":
                        dataType = DataTypeEnum.UInt32; break;
                    case "Int32":
                        dataType = DataTypeEnum.Int32; break;
                    case "UInt64":
                        dataType = DataTypeEnum.UInt64; break;
                    case "Int64":
                        dataType = DataTypeEnum.Int64; break;
                    case "Single":
                        dataType = DataTypeEnum.Float; break;
                    case "Double":
                        dataType = DataTypeEnum.Double; break;
                    default:
                        throw new Exception($"暂未提供对{address.Value.GetType().Name}类型的写入操作。");
                }
                var tempResult = Write(address.Key, address.Value, dataType);
                if (!tempResult.IsSucceed)
                {
                    result.SetErrInfo(tempResult);
                }
                result.Requst = tempResult.Requst;
                result.Response = tempResult.Response;
            }
            return result;
        }

        public Result Write(string address, sbyte value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, ulong value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, long value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, double value)
        {
            return WriteValue(address, value);
        }

        public Result Write(string address, object value, DataTypeEnum dataType)
        {
            var result = new Result() { IsSucceed = false };
            switch (dataType)
            {
                case DataTypeEnum.Bool:
                    result = Write(address, Convert.ToBoolean(value));
                    break;
                case DataTypeEnum.Byte:
                    result = Write(address, Convert.ToByte(value));
                    break;
                case DataTypeEnum.Int16:
                    result = Write(address, Convert.ToInt16(value));
                    break;
                case DataTypeEnum.UInt16:
                    result = Write(address, Convert.ToUInt16(value));
                    break;
                case DataTypeEnum.Int32:
                    result = Write(address, Convert.ToInt32(value));
                    break;
                case DataTypeEnum.UInt32:
                    result = Write(address, Convert.ToUInt32(value));
                    break;
                case DataTypeEnum.Int64:
                    result = Write(address, Convert.ToInt64(value));
                    break;
                case DataTypeEnum.UInt64:
                    result = Write(address, Convert.ToUInt64(value));
                    break;
                case DataTypeEnum.Float:
                    result = Write(address, Convert.ToSingle(value));
                    break;
                case DataTypeEnum.Double:
                    result = Write(address, Convert.ToDouble(value));
                    break;
            }
            return result;
        }
    }
}
