using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IoTClient.Common.Helpers;

namespace IoTClient.Tool.Controls
{
    public partial class OtherControl : UserControl
    {
        public OtherControl()
        {
            InitializeComponent();
        }

        private void but_crc16calculate_Click(object sender, EventArgs e)
        {
            var byteArry = DataConvert.StringToByteArray(txt_crcstr.Text?.Trim(), false);
            var crc16 = CRC16.GetCRC16(byteArry);
            AppendText($"CRC16计算结果：{DataConvert.ByteArrayToString(crc16)}");
        }

        private void AppendText(string content)
        {
            txt_content.Invoke((Action)(() =>
            {
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]{content}\r\n");
            }));
        }

        private void but_crc16validation_Click(object sender, EventArgs e)
        {
            var byteArry = DataConvert.StringToByteArray(txt_crcstr.Text?.Trim(), false);
            var checkCrc16 = CRC16.CheckCRC16(byteArry);
            AppendText($"CRC16验证结果：{txt_crcstr.Text} {checkCrc16}");
        }
    }
}
