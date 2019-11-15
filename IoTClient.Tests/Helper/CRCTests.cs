using IoTClient.Common.Helpers;
using System;
using System.Linq;
using Xunit;

namespace IoTClient.Tests.Helper
{
    /// <summary>
    /// https://www.cnblogs.com/mjoin/p/11607114.html
    /// https://en.wikipedia.org/wiki/Cyclic_redundancy_check
    /// https://www.cnblogs.com/esestt/archive/2007/08/09/848856.html
    /// </summary>
    public class CRCTests
    {
        [Fact]
        public void test()
        {

            //var b3 = 0x53A1; 
            //string bstr = "101001110100001";
            //var b2 = Convert.ToInt32(bstr, 2);          
            //BitConverter.GetBytes(0b10010110);
            int value1 = 0b111111110100100;
            int value2 = 0b1101001110110110;

            //var value3 = Convert.ToString((value1 | value2), 2);
            //var value4 = Convert.ToString((value1 & value2), 2);
            var value5 = Convert.ToString((value1 ^ value2), 2);

            var oo = CRC16.GetCRC16(BitConverter.GetBytes((ushort)0xD3B6));
            var oo3 = DataConvert.ByteArrayToString(oo);
            var oo4 = GetCRC16_2(BitConverter.GetBytes((ushort)0xD3B6), 0x8005);
            var oo5 = DataConvert.ByteArrayToString(oo4);

            CalcOnCrc16 crc16 = new CalcOnCrc16();
            var crc1 = crc16.CalcNoemalCrc16(BitConverter.GetBytes((ushort)0xD3B6), 0x8005, 0xFFFF);
            var crcSring1 = DataConvert.ByteArrayToString(BitConverter.GetBytes(crc1));

            var crc2 = crc16.CalcReversedCrc16(BitConverter.GetBytes((ushort)0xD3B6), 0xA001, 0xFFFF);
            var crcSring2 = DataConvert.ByteArrayToString(BitConverter.GetBytes(crc2));
        }


        public static byte[] GetCRC16_2(byte[] value, ushort h = 0xA001)
        {
            if (value == null || !value.Any())
                throw new ArgumentException("生成CRC16的入参有误");

            //运算
            ushort crc = 0xFFFF;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                crc = (ushort)(crc ^ (value[i] << 8));
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ h) : (ushort)(crc << 1);
                }
            }
            byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc & 0x00FF);         //低位置

            byte[] buffer = new byte[value.Length + 2];
            value.CopyTo(buffer, 0);
            buffer[buffer.Length - 1] = hi;
            buffer[buffer.Length - 2] = lo;
            return buffer;
        }
    }
   
    class CalcOnCrc16
    {
        private ushort[] Crc16NormalTable;

        private ushort[] Crc16ReversedTable;

        private void CreateNormalCrc16Table(ushort ploy)
        {
            ushort data;
            Crc16NormalTable = new ushort[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                data = (ushort)(i << 8);
                for (j = 0; j < 8; j++)
                {
                    if ((data & 0x8000) == 0x8000)
                        data = Convert.ToUInt16((ushort)(data << 1) ^ ploy);
                    else
                        data <<= 1;
                }
                Crc16NormalTable[i] = data;
            }
        }

        private void CreateReversedCrc16Table(ushort ploy)
        {
            ushort data;
            Crc16ReversedTable = new ushort[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                data = (ushort)i;
                for (j = 0; j < 8; j++)
                {
                    if ((data & 1) == 1)
                        data = Convert.ToUInt16((ushort)(data >> 1) ^ ploy);
                    else
                        data >>= 1;
                }
                Crc16ReversedTable[i] = data;
            }
        }

        /// <summary>
        /// 正向计算CRC16校验码
        /// </summary>
        /// <param name="bytes">校验数据</param>
        /// <param name="poly">生成多项式</param>
        /// <param name="crcInit">校验码初始值</param>
        /// <returns></returns>
        public ushort CalcNoemalCrc16(byte[] bytes, ushort poly, ushort crcInit)
        {
            CreateNormalCrc16Table(poly);

            ushort crc = crcInit;
            for (int i = 0; i < bytes.Length; i++)
            {
                crc = Convert.ToUInt16((ushort)(crc << 8) ^ Crc16NormalTable[((crc >> 8) & 0xff) ^ bytes[i]]);
            }
            return crc;
        }

        /// <summary>
        /// 反向计算CRC16校验码
        /// </summary>
        /// <param name="bytes">校验数据</param>
        /// <param name="poly">反向生成多项式</param>
        /// <param name="crcInit">校验码初始值</param>
        /// <returns></returns>
        public ushort CalcReversedCrc16(byte[] bytes, ushort poly, ushort crcInit)
        {
            CreateReversedCrc16Table(poly);

            ushort crc = crcInit;
            for (int i = 0; i < bytes.Length; i++)
            {
                crc = Convert.ToUInt16((ushort)(crc >> 8) ^ Crc16ReversedTable[(crc & 0xff) ^ bytes[i]]);
            }
            return crc;
        }
    }
}
