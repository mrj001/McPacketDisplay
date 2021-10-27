using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;
using Moq;
using Xunit;

namespace Test.Models.Packets
{
   public class TestMineCraftPacket
   {
      private static IMineCraftProtocol _protocol;
      static TestMineCraftPacket()
      {
         Assembly assy = AppDomain.CurrentDomain.GetAssemblies().
                  Where<Assembly>(a => !a.IsDynamic && a.Location.EndsWith("McPacketDisplay.dll")).
                  First<Assembly>();

         XmlDocument doc = new XmlDocument();
         using (Stream xml = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xml"))
         using (XmlReader rdr = XmlReader.Create(xml))
            doc.Load(rdr);

         _protocol = new MineCraftProtocol(doc.FirstChild.NextSibling);
      }

      private static IMineCraftPacketDefinition GetDefinition(PacketID packetID)
      {
         int j = 0;
         int jul = _protocol.Count;
         while (j < jul && _protocol[j].ID != packetID)
            j++;
         if (j == jul)
            return null;

         return _protocol[j];
      }

      public static TheoryData<Type, int, byte[]> GetPacket_TestData
      {
         get
         {
            var rv = new TheoryData<Type, int, byte[]>();

            rv.Add(typeof(MineCraftPacket), 5, new byte[] { 0x0a, 0x01 });
            // Set Slot to empty
            rv.Add(typeof(MineCraftPacket), 7, new byte[] { 0x67, 0xff, 0xff, 0xff, 0xff, 0xff });
            // Set Slot to an item.
            rv.Add(typeof(MineCraftPacket), 11, new byte[] { 0x67, 0x01, 0x00, 0x20, 0x01, 0x0d, 0x01, 0x00, 0x16 });
            // Window Click Packet with non-empty slot
            rv.Add(typeof(MineCraftPacket), 3, new byte[] { 0x66, 0x01, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x07, 0x11, 0x00, 0x00 });
            // Window Click Packet with empty slot.
            rv.Add(typeof(MineCraftPacket), 19, new byte[] { 0x66, 0x01, 0x00, 0x07, 0x00, 0x00, 0x02, 0x00, 0xff, 0xff });

            // Player Block Placement with no item.
            rv.Add(typeof(MineCraftPacket), 7, new byte[] { 0x0f, 0x00, 0x00, 0x00, 0x22, 0x40, 0xff, 0xff, 0xff, 0xeb, 0x01, 0xff, 0xff });
            // Player Block Placement with a block
            rv.Add(typeof(MineCraftPacket), 3, new byte[] { 0x0f, 0x00, 0x00, 0x00, 0x20, 0x3f, 0xff, 0xff, 0xff, 0xec, 0x01, 0x01, 0x43, 0x01, 0x00, 0x00 });
            // Mob Spawn with metadata
            rv.Add(typeof(MineCraftPacket), 3, new byte[] { 0x18, 0x00, 0x00, 0x00, 0x3d, 0x5a, 0x00, 0x00, 0x00, 0xd0, 0x00, 0x00, 0x08, 0xa0, 0xff, 0xff, 0xec, 0xd0, 0x9a, 0x00, 0x00, 0x00, 0x10, 0x00, 0x7f });

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(GetPacket_TestData))]
      public void GetPacket_Test(Type expectedType, int expectedNumber, byte[] streamData)
      {
         PacketID expectedID = new PacketID(streamData[0]);
         IMineCraftPacket actual;

         using (MemoryStream strm = new MemoryStream(streamData))
         {
            actual = MineCraftPacket.GetPacket(expectedNumber, _protocol, strm);

            Assert.Equal(streamData.Length, strm.Position);
         }

         IMineCraftPacketDefinition expectedDefinition = GetDefinition(expectedID);

         Assert.Equal(expectedType, actual.GetType());
         Assert.Equal(expectedNumber, actual.PacketNumber);
         Assert.Equal(expectedID, actual.ID);
         Assert.Equal(expectedDefinition.Name, actual.Name);
         Assert.Equal(expectedDefinition.From, actual.From);
         Assert.Equal(expectedDefinition.Count, actual.Count);
         int j;
         for (j = 0; j < expectedDefinition.Count; j ++)
            Assert.Equal(expectedDefinition[j].Name, actual[j].Name);

         j = 0;
         foreach (IField k in actual)
         {
            Assert.Equal(expectedDefinition[j].Name, k.Name);
            j++;
         }
      }

      [Fact]
      public void GetPacket_WindowItems_Test()
      {
         int expectedID = 0x68;
         int expectedNumber = 13;
         IMineCraftPacket actual;
         byte[] streamData = new byte[] 
         {
            0x68, 0x01, 0x00, 0x27, 0xff, 0xff, 0x01,
            0x07, 0x11, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff,
            0x01, 0x18, 0x02, 0x00, 0x00, 0x00, 0x05, 0x02,
            0x00, 0x00, 0x00, 0x11, 0x03, 0x00, 0x00, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0x01, 0x12, 0x01,
            0x00, 0x33, 0x00, 0x03, 0x03, 0x00, 0x00, 0x01,
            0x0d, 0x01, 0x00, 0x16, 0x00, 0x04, 0x40, 0x00,
            0x00, 0x00, 0x0d, 0x0f, 0x00, 0x00, 0x00, 0x06,
            0x05, 0x00, 0x00, 0x00, 0x0f, 0x01, 0x00, 0x00,
            0x00, 0x04, 0x0c, 0x00, 0x00, 0x00, 0x32, 0x0b,
            0x00, 0x00
         };

         using (MemoryStream strm = new MemoryStream(streamData))
         {
            actual = MineCraftPacket.GetPacket(expectedNumber, _protocol, strm);

            // Assert that the entire stream is exactly used up.
            Assert.Equal(streamData.Length, strm.Position);
         }

         IMineCraftPacketDefinition expectedDefinition = GetDefinition(new PacketID(expectedID));

         Assert.Equal(typeof(MineCraftWindowItemsPacket), actual.GetType());
         Assert.Equal(expectedNumber, actual.PacketNumber);
         Assert.Equal(expectedID, actual.ID.ID);
         Assert.Equal(expectedDefinition.Name, actual.Name);
         Assert.Equal(expectedDefinition.From, actual.From);
         Assert.Equal(expectedDefinition.Count, actual.Count);

         int j;
         for (j = 0; j < expectedDefinition.Count; j++)
            Assert.Equal(expectedDefinition[j].Name, actual[j].Name);

         j = 0;
         foreach (IField k in actual)
         {
            Assert.Equal(expectedDefinition[j].Name, k.Name);
            j++;
         }

         Assert.Equal(39, ((ItemArrayField)actual["Items"]).Count);
      }
   }
}