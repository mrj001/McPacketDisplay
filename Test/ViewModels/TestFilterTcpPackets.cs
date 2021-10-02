using System;
using System.ComponentModel;
using System.Net;
using McPacketDisplay.ViewModels;
using PacketDotNet;
using Xunit;

namespace Test.ViewModels
{
   public class TestFilterTcpPackets
   {
      [Fact]
      public void ctor_Test()
      {
         FilterTcpPackets filter = new FilterTcpPackets();

         Assert.Equal(IPAddress.Loopback, filter.ServerAddress);
         Assert.False(filter.ApplyServerAddressFilter);

         Assert.Equal(25565, filter.ServerPort);
         Assert.False(filter.ApplyServerPortFilter);

         Assert.Equal(IPAddress.Loopback, filter.ClientAddress);
         Assert.False(filter.ApplyClientAddressFilter);

         Assert.Equal(ushort.MaxValue, filter.ClientPort);
         Assert.False(filter.ApplyClientPortFilter);
      }

      [Fact]
      public void ServerAddress_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         string updated = "192.168.0.21";
         filter.ServerAddress = IPAddress.Parse(updated);
         Assert.True(called);

         // Note: Using a separate instance of IPAddress to assert that referential
         // equality is not used when testing equality.
         called = false;
         filter.ServerAddress = IPAddress.Parse(updated);
         Assert.False(called);
      }

      [Fact]
      public void ServerAddressFilterApplied_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         filter.ApplyServerAddressFilter = true;
         Assert.True(called);

         called = false;
         filter.ApplyServerAddressFilter = true;
         Assert.False(called);
      }

      [Fact]
      public void ServerPort_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         ushort newPort = 25568;
         filter.ServerPort = newPort;
         Assert.True(called);

         called = false;
         filter.ServerPort = newPort;
         Assert.False(called);
      }

      [Fact]
      public void ClientAddress_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         string newClient = "192.168.0.65";
         filter.ClientAddress = IPAddress.Parse(newClient);
         Assert.True(called);

         called = false;
         filter.ClientAddress = IPAddress.Parse(newClient);
         Assert.False(called);
      }

      [Fact]
      public void ApplyClientAddressFilter_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         filter.ApplyClientAddressFilter = true;
         Assert.True(called);

         called = false;
         filter.ApplyClientAddressFilter = true;
         Assert.False(called);
      }

      [Fact]
      public void ClientPort_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         ushort newPort = 5432;
         filter.ClientPort = newPort;
         Assert.True(called);

         called = false;
         filter.ClientPort = newPort;
         Assert.False(called);
      }

      [Fact]
      public void ApplyClientPortFilter_PropertyChange_Fires()
      {
         FilterTcpPackets filter = new FilterTcpPackets();
         bool called = false;

         (filter as INotifyPropertyChanged).PropertyChanged += (s, e) => { called = true; };

         filter.ApplyClientPortFilter = true;
         Assert.True(called);

         called = false;
         filter.ApplyClientPortFilter = true;
         Assert.False(called);
      }

      // Note: TheoryData is limited to 10 fields.
      //       So we combine the boolean flags to specify the filter.
      [Flags]
      public enum ApplyFields
      {
         None = 0,
         ServerAddress = 1,
         ServerPort = 2,
         ClientAddress = 4,
         ClientPort = 8,
         All = 15
      }

      public static TheoryData<bool, ApplyFields, string, ushort, string, ushort, string, ushort, string, ushort> PassPacket_TestData
      {
         get
         {
            var rv = new TheoryData<bool, ApplyFields, string, ushort, string, ushort, string, ushort, string, ushort>();

            // Test that the packet passes when no filter is applied.
            rv.Add(true, ApplyFields.None, "127.0.0.1", 25565, "127.0.0.1", 56645, "192.168.0.21", 25565, "192.168.0.65", 64457);

            // Test that the packet passes when the server matches the source address.
            // The server ports do not match, and are not included in the filter.
            rv.Add(true, ApplyFields.ServerAddress, "127.0.0.1", 25565, "192.168.0.5", 5432, "127.0.0.1", 25566, "192.168.0.6", 2345);

            // Test that the packet passes when the server matches the destination address.
            // The server ports do not match, and are not included in the filter.
            rv.Add(true, ApplyFields.ServerAddress, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.6", 2345, "127.0.0.1", 25566);

            // Test that the packet passes when the server matches the source address and port.
            rv.Add(true, ApplyFields.ServerAddress | ApplyFields.ServerPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "127.0.0.1", 25565, "192.168.0.6", 2345);

            // Test that the packet passes when the server matches the destination address and port.
            rv.Add(true, ApplyFields.ServerAddress | ApplyFields.ServerPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.6", 2345, "127.0.0.1", 25565);

            // Test that the packet fails when the server matches the source address.
            // The server ports do not match, and are included in the filter.
            rv.Add(false, ApplyFields.ServerAddress | ApplyFields.ServerPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "127.0.0.1", 25566, "192.168.0.6", 2345);

            // Test that the packet fails when the server matches the destination address.
            // The server ports do not match, and are included in the filter.
            rv.Add(false, ApplyFields.ServerAddress | ApplyFields.ServerPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.6", 2345, "127.0.0.1", 25566);

            // Test that the packet passes when the client matches the source address
            // The ports do not match, and are not included in the filter.
            rv.Add(true, ApplyFields.ClientAddress, "127.0.0.1", 25565, "192.168.0.6", 54321, "192.168.0.6", 5432, "192.168.0.5", 2345);

            // Test that the packet passes when the client matches the destination address
            // The ports do not match, and are not included in the filter.
            rv.Add(true, ApplyFields.ClientAddress, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.21", 25566, "192.168.0.5", 2345);

            // Test that the packet passes when the client matches the source address and port.
            rv.Add(true, ApplyFields.ClientAddress | ApplyFields.ClientPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.5", 5432, "192.168.0.6", 2345);

            // Test that the packet passes when the client matches the destination address and port.
            rv.Add(true, ApplyFields.ClientAddress | ApplyFields.ClientPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.6", 2345, "192.168.0.5", 5432);

            // Test that the packet fails when the client matches the source address.
            // The ports do not match, and are included in the filter.
            rv.Add(false, ApplyFields.ClientAddress | ApplyFields.ClientPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "192.168.0.5", 2345, "127.0.0.1", 25566);

            // Test that the packet fails when the client matches the destination address.
            // The ports do not match, and are included in the filter.
            rv.Add(false, ApplyFields.ServerAddress | ApplyFields.ServerPort, "127.0.0.1", 25565, "192.168.0.5", 5432, "127.0.0.1", 25566, "192.168.0.5", 2345);

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(PassPacket_TestData))]
      public void PassPacket_Test(bool expectedPass,
         ApplyFields fields,
         string filterServerAddr, ushort filterServerPort,
         string filterClientAddr, ushort filterClientPort,
         string packetSourceAddr, ushort packetSourcePort,
         string packetDestAddr, ushort packetDestPort)
      {
         IPAddress srcAddr = IPAddress.Parse(packetSourceAddr);
         IPAddress dstAddr = IPAddress.Parse(packetDestAddr);
         IPv4Packet ipPacket = new IPv4Packet(srcAddr, dstAddr);

         TcpPacket tcpPacket = new TcpPacket(packetSourcePort, packetDestPort);
         tcpPacket.ParentPacket = ipPacket;

         IFilterTcpPackets filter = new FilterTcpPackets();
         filter.ServerAddress = IPAddress.Parse(filterServerAddr);
         filter.ApplyServerAddressFilter = (fields & ApplyFields.ServerAddress) != ApplyFields.None;
         filter.ServerPort = filterServerPort;
         filter.ApplyServerPortFilter = (fields & ApplyFields.ServerPort) != ApplyFields.None;
         filter.ClientAddress = IPAddress.Parse(filterClientAddr);
         filter.ApplyClientAddressFilter = (fields & ApplyFields.ClientAddress) != ApplyFields.None;
         filter.ClientPort = filterClientPort;
         filter.ApplyClientPortFilter = (fields & ApplyFields.ClientPort) != ApplyFields.None;

         bool actualPass = filter.PassPacket(tcpPacket);

         Assert.Equal(expectedPass, actualPass);
      }
   }
}
