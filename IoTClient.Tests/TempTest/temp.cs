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
            var str = "0300 00 1D";
            //var b = str.Split(" ").Select(t => Convert.ToByte(t, 16)).ToArray();

            var aa = DataConvert.StringToByteArray(str);
            var aa2 = DataConvert.StringToByteArray(str,false);

            var bb = DataConvert.ByteArrayToString(aa);

           var bb2 =  BitConverter.GetBytes(258);
        }
    }
}
