using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.BACnet;
using IoTClient.Demo.Helper;

namespace IoTClient.Demo
{
    public partial class BACnetControl : UserControl
    {
        public BACnetControl()
        {
            InitializeComponent();
            txt_msgList.ScrollBars = ScrollBars.Vertical;
            Size = new Size(880, 450);
        }
        private static List<BacNode> devicesList = new List<BacNode>();
        private BacnetClient Bacnet_client;

        private void BACnetControl_Load(object sender, EventArgs e)
        {
            Bacnet_client = new BacnetClient(new BacnetIpUdpProtocolTransport(47808, false));
            Bacnet_client.OnIam -= new BacnetClient.IamHandler(handler_OnIam);
            Bacnet_client.OnIam += new BacnetClient.IamHandler(handler_OnIam);
            Bacnet_client.Start();
            Bacnet_client.WhoIs();
            Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(100);
                    Log($"等待扫描...[{9 - i}]");
                }
                Scan();
            });
        }

        private void handler_OnIam(BacnetClient sender, BacnetAddress adr, uint deviceId, uint maxAPDU, BacnetSegmentations segmentation, ushort vendorId)
        {
            BeginInvoke(new Action(() =>
            {
                lock (devicesList)
                {
                    foreach (BacNode bn in devicesList)
                        if (bn.GetAdd(deviceId) != null) return;   // Yes

                    devicesList.Add(new BacNode(adr, deviceId));   // add it 
                    listBox1.Items.Add(adr.ToString() + " " + deviceId);
                }
            }));
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            txt_msgList.Text = string.Empty;
            Task.Run(() =>
            {
                Log("准备扫描...");
                Scan();
            });
        }

        /// <summary>
        /// 扫描
        /// </summary>
        private void Scan()
        {
            Log("开始扫描设备");
            foreach (var device in devicesList)
            {
                //获取子节点个数
                var deviceCount = GetDeviceArrayIndexCount(device) + 1;
                //TODO 50 可设置 配置
                ScanPointsBatch(device, 50, deviceCount);
            }
            Log("开始扫描属性");
            foreach (var device in devicesList)
            {
                ScanSubProperties(device);
            }
            Log("扫描完成");
        }

        /// <summary>
        /// 扫描设备
        /// </summary>
        public void ScanPointsBatch(BacNode device, uint deviceCount, uint count)
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
                for (uint i = 0; i < count; i++)
                {
                    rList.Add(new BacnetPropertyReference((uint)pid, i));
                    if ((i != 0 && i % deviceCount == 0) || i == count - 1)//不要超了 MaxAPDU
                    {
                        IList<BacnetReadAccessResult> lstAccessRst;
                        var bRst = Bacnet_client.ReadPropertyMultipleRequest(adr, bobj, rList, out lstAccessRst, this.GetCurrentInvokeId());
                        if (bRst)
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
                                        //Log(pid + " , " + strBValue + " , " + bValue.Tag);

                                        var strs = strBValue.Split(':');
                                        if (strs.Length < 2) continue;
                                        var strType = strs[0];
                                        var strObjId = strs[1];
                                        var subNode = new BacProperty();
                                        BacnetObjectTypes otype;
                                        Enum.TryParse(strType, out otype);
                                        if (otype == BacnetObjectTypes.OBJECT_NOTIFICATION_CLASS || otype == BacnetObjectTypes.OBJECT_DEVICE) continue;
                                        subNode.ObjectId = new BacnetObjectId(otype, Convert.ToUInt32(strObjId));
                                        //添加属性
                                        device.Properties.Add(subNode);
                                    }
                                }
                            }
                        }
                        rList.Clear();
                    }
                }
            }
            catch (Exception exp)
            {
                Log("Err:" + exp.Message);
            }
        }

        //获取子节点个数
        public uint GetDeviceArrayIndexCount(BacNode device)
        {
            try
            {
                var adr = device.Address;
                if (adr == null) return 0;
                var list = ReadScalarValue(adr,
                    new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device.DeviceId),
                    BacnetPropertyIds.PROP_OBJECT_LIST, 0, 0);
                var rst = Convert.ToUInt32(list.FirstOrDefault().Value);
                return rst;
            }
            catch
            { }
            return 0;
        }

        IList<BacnetValue> ReadScalarValue(BacnetAddress adr, BacnetObjectId oid,
            BacnetPropertyIds pid, byte invokeId = 0, uint arrayIndex = uint.MaxValue)
        {
            try
            {
                IList<BacnetValue> NoScalarValue;
                var rst = Bacnet_client.ReadPropertyRequest(adr, oid, pid, out NoScalarValue, invokeId, arrayIndex);
                if (!rst) return null;
                return NoScalarValue;
            }
            catch { }
            return null;
        }

        byte InvokeId = 0x00;
        public byte GetCurrentInvokeId()
        {
            InvokeId = (byte)((InvokeId + 1) % 256);
            return InvokeId;
        }

        /// <summary>
        /// 扫描属性
        /// </summary>
        /// <param name="device"></param>
        private void ScanSubProperties(BacNode device)
        {
            var adr = device.Address;
            if (adr == null) return;
            if (device.Properties == null) return;
            foreach (BacProperty subNode in device.Properties)
            {
                try
                {
                    List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();
                    rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_DESCRIPTION, uint.MaxValue));
                    rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_REQUIRED, uint.MaxValue));
                    rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_OBJECT_NAME, uint.MaxValue));
                    rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_PRESENT_VALUE, uint.MaxValue));
                    IList<BacnetReadAccessResult> lstAccessRst;
                    var bRst = Bacnet_client.ReadPropertyMultipleRequest(adr, subNode.ObjectId, rList, out lstAccessRst, this.GetCurrentInvokeId());
                    if (bRst)
                    {
                        foreach (var aRst in lstAccessRst)
                        {
                            if (aRst.values == null) continue;
                            foreach (var bPValue in aRst.values)
                            {
                                if (bPValue.value == null || bPValue.value.Count == 0) continue;
                                var pid = (BacnetPropertyIds)(bPValue.property.propertyIdentifier);
                                var bValue = bPValue.value.First();
                                var strBValue = "" + bValue.Value;
                                //Log(pid + " , " + strBValue + " , " + bValue.Tag);
                                switch (pid)
                                {
                                    case BacnetPropertyIds.PROP_DESCRIPTION://描述
                                        {
                                            subNode.PROP_DESCRIPTION = bValue + "";
                                        }
                                        break;
                                    case BacnetPropertyIds.PROP_OBJECT_NAME://点名
                                        {
                                            subNode.PROP_OBJECT_NAME = bValue + "";
                                        }
                                        break;
                                    case BacnetPropertyIds.PROP_PRESENT_VALUE://值
                                        {
                                            subNode.PROP_PRESENT_VALUE = bValue.Value;
                                            subNode.DataType = bValue.Value.GetType();
                                        }
                                        break;
                                }
                            }
                        }
                        ShwoText(string.Format("点名:{0,-20} 值:{1,-10} 类型:{3,-15} 描述:{2} ", subNode.PROP_OBJECT_NAME, subNode.PROP_PRESENT_VALUE, subNode.PROP_DESCRIPTION, subNode.PROP_PRESENT_VALUE.GetType()));
                    }
                }
                catch (Exception exp)
                {
                    Log("Error: " + exp.Message);
                }
            }
        }

        private void Log(string str)
        {
            BeginInvoke(new Action(() =>
            {
                txt_msgList.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}]:{str} \r\n");
            }));
        }

        private void ShwoText(string str)
        {
            BeginInvoke(new Action(() =>
            {
                txt_msgList.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {str} \r\n");
            }));
        }

        private async void button3_ClickAsync(object sender, EventArgs e)
        {
            var key = textBox3.Text?.Trim();
            var rpop = devicesList.SelectMany(t => t.Properties)
                .Where(t => t.PROP_OBJECT_NAME == key)
                .FirstOrDefault();
            var bacnet = devicesList.Where(t => t.Properties.Any(p => p.PROP_OBJECT_NAME == key))
                .FirstOrDefault();
            if (rpop == null)
            {
                Log("没有找到对应的点");
                return;
            }
            int retry = 0;//重试
            tag_retry:
            var isOk = Bacnet_client.ReadPropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE, out IList<BacnetValue> NoScalarValue);
            if (isOk)
            {
                await Task.Delay(retry * 200);
                try
                {
                    var value = NoScalarValue[0].Value;
                    ShwoText(string.Format("[读取成功][{3}] 点:{0,-15} 值:{1,-10} 类型:{2}", key, value?.ToString(), value?.GetType().ToString(), retry));
                }
                catch
                {
                    Log("Err:读取失败.");
                }
            }
            else
            {
                retry++;
                if (retry < 4) goto tag_retry;
                Log($"Err:读取失败[{retry - 1}]");
            }
        }

        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            var key = textBox3.Text?.Trim();
            var value = textBox2.Text?.Trim();
            var rpop = devicesList.SelectMany(t => t.Properties)
                .Where(t => t.PROP_OBJECT_NAME == key)
                .FirstOrDefault();
            var bacnet = devicesList.Where(t => t.Properties.Any(p => p.PROP_OBJECT_NAME == key))
                .FirstOrDefault();
            if (rpop == null)
            {
                Log("没有找到对应的点");
                return;
            }

            BacnetValue[] NoScalarValue = { new BacnetValue(value.ToDataFormType(rpop.DataType)) };
            int retry = 0;//重试
            tag_retry:
            try
            {
                await Task.Delay(retry * 200);
                Bacnet_client.WritePropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE, NoScalarValue, 0);
                //Bacnet_client.WritePropertyRequest(bacnet.Address, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, rpop.ObjectId.instance), BacnetPropertyIds.PROP_PRESENT_VALUE, NoScalarValue, 0);
                ShwoText(string.Format("[写入成功][{2}] 点:{0,-15} 值:{1}", key, value, retry));
            }
            catch (Exception ex)
            {
                retry++;
                if (retry < 4) goto tag_retry;//强行重试
                Log($"写入失败:{ex.Message} [{retry - 1}]");
            }
        }
    }
}
