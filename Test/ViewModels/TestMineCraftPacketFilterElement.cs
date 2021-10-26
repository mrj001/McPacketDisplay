using System;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;
using McPacketDisplay.ViewModels;
using Moq;
using Xunit;

namespace Test.ViewModels
{
   public class TestMineCraftPacketFilterElement
   {
      private static Mock<IMineCraftPacket> GetMock(PacketID id, PacketSource source, string name)
      {
         Mock<IMineCraftPacket> rv = new Mock<IMineCraftPacket>(MockBehavior.Strict);

         rv.Setup(a => a.ID).Returns(id);
         rv.Setup(a => a.From).Returns(source);
         rv.Setup(a => a.Name).Returns(name);

         return rv;
      }

      [Fact]
      public void ctor()
      {
         PacketID id = new PacketID(0x33);
         string name = "MapChunk";

         Mock<IMineCraftPacket> mock = GetMock(id, PacketSource.Server, name);
         MineCraftPacketFilterElement actual = new MineCraftPacketFilterElement(mock.Object);

         Assert.Equal(id, actual.ID);
         Assert.Equal(PacketSource.Server, actual.Source);
         Assert.Equal(name, actual.Name);
         Assert.Equal(0, actual.PacketCount);
         Assert.True(actual.Pass);
      }

      [Fact]
      public void PacketCount_PropertyChange_Fires()
      {
         Mock<IMineCraftPacket> mock = GetMock(new PacketID(0x67), PacketSource.Server, "SetSlot");
         MineCraftPacketFilterElement actual = new MineCraftPacketFilterElement(mock.Object);
         int changingCallCount = 0;
         actual.PropertyChanging += ((s, e) =>
         {
            changingCallCount++;
            Assert.Equal(nameof(MineCraftPacketFilterElement.PacketCount), e.PropertyName);
         });
         int changedCallCount = 0;
         actual.PropertyChanged += ((s, e) =>
         {
            changedCallCount++;
            Assert.Equal(nameof(MineCraftPacketFilterElement.PacketCount), e.PropertyName);
         });

         actual.PacketCount = 5;

         Assert.Equal(1, changingCallCount);
         Assert.Equal(1, changedCallCount);
         Assert.Equal(5, actual.PacketCount);
      }

      [Fact]
      public void Pass_PropertyChange_Fires()
      {
         Mock<IMineCraftPacket> mock = GetMock(new PacketID(0x67), PacketSource.Server, "SetSlot");
         MineCraftPacketFilterElement actual = new MineCraftPacketFilterElement(mock.Object);
         int changingCallCount = 0;
         actual.PropertyChanging += ((s, e) =>
         {
            changingCallCount++;
            Assert.Equal("Pass", e.PropertyName);
         });
         int changedCallCount = 0;
         actual.PropertyChanged += ((s, e) =>
         {
            changedCallCount++;
            Assert.Equal("Pass", e.PropertyName);
         });

         Assert.True(actual.Pass);
         actual.Pass = false;

         Assert.Equal(1, changingCallCount);
         Assert.Equal(1, changedCallCount);
         Assert.False(actual.Pass);
      }

      public static TheoryData<bool, int, PacketSource, string, int, PacketSource> IsMatch_TestData
      {
         get
         {
            var rv = new TheoryData<bool, int, PacketSource, string, int, PacketSource>();

            rv.Add(true, 0x68, PacketSource.Server, "WindowItems", 0x68, PacketSource.Server);
            rv.Add(false, 0x67, PacketSource.Server, "SetSlot", 0x68, PacketSource.Server);

            rv.Add(true, 0x0d, PacketSource.Client, "PlayerPositionAndLook", 0x0d, PacketSource.Client);
            rv.Add(false, 0x0d, PacketSource.Client, "PlayerPositionAndLook", 0x0d, PacketSource.Server);

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(IsMatch_TestData))]
      public void IsMatch(bool expected, int filterId, PacketSource filterSource, string filterName,
               int packetId, PacketSource packetSource)
      {
         Mock<IMineCraftPacket> mock = GetMock(new PacketID(filterId), filterSource, filterName);
         MineCraftPacketFilterElement filterElement = new MineCraftPacketFilterElement(mock.Object);

         Mock<IMineCraftPacket> mockPacket = new Mock<IMineCraftPacket>(MockBehavior.Strict);
         mockPacket.Setup(a => a.ID).Returns(new PacketID(packetId));
         mockPacket.Setup(a => a.From).Returns(packetSource);

         bool actual = filterElement.IsMatch(mockPacket.Object);

         Assert.Equal(expected, actual);
      }
   }
}
