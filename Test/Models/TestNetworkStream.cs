using System;
using System.Linq;
using McPacketDisplay.Models;
using Xunit;

namespace Test.Models
{
   public class TestNetworkStream
   {
         private readonly int[] _expected = new int[]
         {
         0x14, 0x00, 0x3b, 0xff, 0xff, 0xd8, 0x00, 0x00,
         0x60, 0x00, 0x04, 0x00, 0x02, 0xb2, 0xad, 0xf1, 
         0x07, 0xa2, 0xaf, 0x89, 0x0d, 0x0c, 0x00, 0x0b, 
         0xff, 0xff, 0xe8, 0x00, 0x00, 0x07, 0x50, 0x43, 
         0xb7, 0x1e, 0x0a, 0x00, 0x46, 0xb3, 0x0e, 0xfb, 
         0x53, 0x00, 0x0b, 0x05, 0x1c,
         -1
         };

      [Fact]
      public void Test_ReadByte()
      {
         TcpPacketList packets = TcpPacketList.GetList("Files/SomeEmptyPackets.pcap");
         NetworkStream actual = new NetworkStream(packets);

         int readByte;
         int index = 0;
         do
         {
            readByte = actual.ReadByte();
            Assert.True(_expected[index] == readByte, $"Expected 0x{_expected[index]:x2} ,but received 0x{readByte:x2} at index {index}.");
            index++;
         } while (readByte != -1);
      }

      [Fact]
      public void Test_Read()
      {
         int expectedReadBytes = _expected.Length - 1;
         byte[] buffer = new byte[2 * expectedReadBytes];
         int offset = 3;

         for (int j = 0; j < 2 * expectedReadBytes; j ++)
            buffer[j] = 0;

         TcpPacketList packets = TcpPacketList.GetList("Files/SomeEmptyPackets.pcap");
         NetworkStream actual = new NetworkStream(packets);

         int actualReadBytes = actual.Read(buffer, offset, buffer.Length);

         Assert.Equal(expectedReadBytes, actualReadBytes);

         for (int j = 0; j < 2 * expectedReadBytes; j ++)
         {
            if (j < offset || j >= offset + expectedReadBytes)
               Assert.Equal(0, buffer[j]);
            else
               Assert.Equal(_expected[j - offset], buffer[j]);
         }
      }
   }
}
