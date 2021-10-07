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

      public static TheoryData<Type, byte[]> GetPacket_TestData
      {
         get
         {
            var rv = new TheoryData<Type, byte[]>();

            rv.Add(typeof(MineCraftPacket), new byte[] { 0x0a, 0x01 });

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(GetPacket_TestData))]
      public void GetPacket_Test(Type expectedType, byte[] streamData)
      {
         PacketID expectedID;
         IMineCraftPacket actual;

         using (MemoryStream strm = new MemoryStream(streamData))
         {
            actual = MineCraftPacket.GetPacket(_protocol, strm);

            strm.Seek(0, SeekOrigin.Begin);
            int n = strm.ReadByte();
            expectedID = new PacketID(n);
         }

         IMineCraftPacketDefinition expectedDefinition = GetDefinition(expectedID);

         Assert.Equal(expectedType, actual.GetType());
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
   }
}