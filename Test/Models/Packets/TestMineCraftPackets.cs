using System;
using System.Linq;
using McPacketDisplay.Models.Packets;
using Xunit;

namespace Test.Models.Packets
{
   public class TestMineCraftPackets
   {
      [Fact]
      public void GetPackets()
      {
         IMineCraftProtocol protocol = MineCraftTestingProtocol.GetProtocol();

         MineCraftPackets actual = MineCraftPackets.GetPackets(protocol, "Files/FourPackets.pcap");

         int[] expectedIDs = new int[]
         {
            0x0a,         // first TCP Packet
            0x04, 0x1f,   // second TCP Packet
            0x21, 0x1f, 0x1f, 0x1f, 0x1c, 0x1f, 0x21, 0x1f, 0x1f, 0x21, 0x1f, 0x21, 0x1f, 0x21, 0x1f, 0x21, 0x21, 0x21, 0x1f, 0x21, 0x21, 0x21,
            0x0a          // fourth TCP Packet
         };

         Assert.Equal(expectedIDs.Count(), actual.Count());

         int j = 0;
         foreach(IMineCraftPacket p in actual)
         {
            Assert.Equal(expectedIDs[j], p.ID.ID);
            j++;
         }
      }
   }
}