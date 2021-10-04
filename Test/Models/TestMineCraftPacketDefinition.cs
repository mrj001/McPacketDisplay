using System;
using System.Xml;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;
using Xunit;

namespace Test.Models
{
   public class TestMineCraftPacketDefinition
   {
      public static TheoryData<int, PacketSource, string, string[],FieldDataType[], string> ctor_TestData
      {
         get
         {
            var rv = new TheoryData<int, PacketSource, string, string[], FieldDataType[], string>();

            rv.Add(10, PacketSource.Client, "PlayerGroundedPacket",
               new string[] { "OnGround" },
               new FieldDataType[] { FieldDataType.Bool },
               "<packet><name>PlayerGroundedPacket</name><from>client</from><id>0a</id><fields><field><name>OnGround</name><type>bool</type></field></fields></packet>");

            rv.Add(0, PacketSource.Client, "KeepAlivePacket",
               Array.Empty<string>(), Array.Empty<FieldDataType>(),
               "<packet><name>KeepAlivePacket</name><from>client</from><id>00</id><fields></fields></packet>");

            rv.Add(0x6a,
               PacketSource.Server, "TransactionStatusPacket",
               new string[] { "WindowID", "ActionNumber", "Accepted" },
               new FieldDataType[] { FieldDataType.Byte, FieldDataType.Short, FieldDataType.Bool },
               "<packet><name>TransactionStatusPacket</name><from>server</from><id>6a</id><fields><field><name>WindowID</name><type>byte</type></field><field><name>ActionNumber</name><type>short</type></field><field><name>Accepted</name><type>bool</type></field></fields></packet>");

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(ctor_TestData))]
      public void ctor(int expectedId, PacketSource expectedSource,
         string expectedName, string[] expectedFieldNames,
         FieldDataType[] expectedFieldDataTypes,
         string xml)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml($"<id>{expectedId:x2}</id>");
         PacketID expectedPacketID = new PacketID(doc.FirstChild);

         doc = new XmlDocument();
         doc.LoadXml(xml);
         MineCraftPacketDefinition actual = new MineCraftPacketDefinition(doc.FirstChild);

         Assert.Equal(expectedPacketID, actual.ID);
         Assert.Equal(expectedSource, actual.From);
         Assert.Equal(expectedName, actual.Name);

         if (expectedFieldNames.Length != expectedFieldDataTypes.Length)
            throw new ArgumentException($"{expectedFieldNames} and {expectedFieldDataTypes} must have the same Length.");

         int jul = expectedFieldNames.Length;
         for (int j = 0; j < jul; j ++)
         {
            Assert.Equal(expectedFieldNames[j], actual[j].Name);
            Assert.Equal(expectedFieldDataTypes[j], actual[j].FieldType);
         }

         Assert.Throws<ArgumentOutOfRangeException>(() => { FieldDefinition fd = actual[-1]; });
         Assert.Throws<ArgumentOutOfRangeException>(() => { FieldDefinition fd = actual[jul]; });
      }

      [Fact]
      public void ctor_Throws_ArgumentException()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<wrong><node>This doesn't belong here</node></wrong>");

         Assert.Throws<ArgumentException>(() => new MineCraftPacketDefinition(doc.FirstChild));
      }

      [Fact]
      public void ctor_Throws_InvalidCastException()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<packet><name>KeepAlivePacket</name><from>InvalidValue</from><id>00</id><fields></fields></packet>");

         Assert.Throws<InvalidCastException>(() => new MineCraftPacketDefinition(doc.FirstChild));
      }
   }
}
