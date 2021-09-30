using System;
using System.Linq;
using McPacketDisplay.Models;
using Xunit;

namespace Test.Models
{
   public class TestTcpPacketList
   {
      [Fact]
      public void Factory()
      {
         TcpPacketList actual = TcpPacketList.GetList("Files/FourPackets.pcap");

         Assert.Equal(4, actual.Count());
      }
   }
}