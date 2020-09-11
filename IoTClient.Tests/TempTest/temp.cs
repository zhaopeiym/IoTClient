using IoTClient.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace IoTClient.Tests.TempTest
{
    public class temp
    {
        [Fact]
        public void test()
        {

            var oo = BitConverter.ToSingle(DataConvert.StringToByteArray("20 FF C0 00 00").Reverse().ToArray(),0);

            var str = "0300 00 1D";
            //var b = str.Split(" ").Select(t => Convert.ToByte(t, 16)).ToArray();

            var aa = DataConvert.StringToByteArray(str);
            var aa2 = DataConvert.StringToByteArray(str, false);

            var bb = DataConvert.ByteArrayToString(aa);

            var bb2 = BitConverter.GetBytes(258);


            //"3A 30 31 30 31 30 31 30 31 46 43 0D 0A";
            var c1 = string.Join(" ", LRC.GetLRC(DataConvert.StringToByteArray("3A 30 31 30 31 30 31 30 31")).Select(t => t.ToString("X2")));

            var c2 = string.Join(" ", LRC.GetLRC(DataConvert.StringToByteArray("30 31 30 31 30 31 30 31")).Select(t => t.ToString("X2")));

            var c3 = string.Join(" ", LRC.GetLRC(DataConvert.StringToByteArray("30 31 30 31 30 31")).Select(t => t.ToString("X2")));

            var c4 = string.Join(" ", Encoding.ASCII.GetBytes("3031").Select(t => t.ToString("X2")));

            List<string> sb = new List<string>();
            foreach (var item in "3A 30 31 30 31 30 30 30 31 30 30 30 31 46 43".Split(" "))
            {
                sb.Add(((char)(Convert.ToByte(item, 16))).ToString());
            }
            string.Join(" ", sb);

            var c7= LRC.GetLRC( DataConvert.StringToByteArray("01 01 00 01 00 01"));

            var c5 = DataConvert.AsciiStringToByteArray("30 31 30 31 30 30 30 31 30 30 30 31 46 43");
            var c6 = DataConvert.AsciiStringToByteArray("30 31 30 31 30 30 30 31 30 30 30 31 46 43", true);
        }
    }
}
