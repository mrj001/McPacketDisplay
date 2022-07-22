using System;
using System.Linq;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;
using McPacketDisplay.ViewModels;
using Moq;
using Xunit;

namespace Test.Models.Packets
{
   public class TestMineCraftPackets
   {
      [Fact]
      public void GetPackets()
      {
         IMineCraftProtocol protocol = MineCraftTestingProtocol.GetProtocol();

         Mock<IFilterTcpPackets> mockTcpFilter = new Mock<IFilterTcpPackets>(MockBehavior.Strict);
         mockTcpFilter.Setup(x => x.GetPacketSource(It.Is<ITcpPacket>(p => p.SourcePort == 25565))).Returns(PacketSource.Server);
         mockTcpFilter.Setup(x => x.GetPacketSource(It.Is<ITcpPacket>(p => p.SourcePort != 25565))).Returns(PacketSource.Client);

         MineCraftPackets actual = MineCraftPackets.GetPackets(protocol, mockTcpFilter.Object, "Files/FourPackets.pcap");

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