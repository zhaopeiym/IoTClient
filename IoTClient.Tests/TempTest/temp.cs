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
            var str = "03 00 00 1D 02 F0 80 32 03 00 00 00 01 00 02 00 1F 00 00 04 01 FF 04 00 04 00 00 00 00";
            //var b = str.Split(" ").Select(t => Convert.ToByte(t, 16)).ToArray();

            var aa = DataConvert.StringToByteArray(str);
            var bb = DataConvert.ByteArrayToString(aa);

           var bb2 =  BitConverter.GetBytes(258);
        }
    }
}
