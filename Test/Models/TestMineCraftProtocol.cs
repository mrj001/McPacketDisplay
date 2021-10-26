using System;
using System.Xml;
using McPacketDisplay.Models.Packets;
using Xunit;

namespace Test.Models
{
   public class TestMineCraftProtocol
   {
         private readonly string _xml = @"<packets>
  <packet>
    <name>KeepAlivePacket</name>
    <from>server</from>
    <id>00</id>
    <fields>
    </fields>
  </packet>
  <packet>
    <name>LoginResponsePacket</name>
    <from>server</from>
    <id>01</id>
    <fields>
      <field>
        <name>EntityID</name>
        <type>int</type>
      </field>
      <field>
        <name>Unknown</name>
        <type>string16</type>
      </field>
      <field>
        <name>Seed</name>
        <type>long</type>
      </field>
      <field>
        <name>Dimension</name>
        <type>byte</type>
      </field>
    </fields>
  </packet>
</packets>";

      [Fact]
      public void ctor()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(_xml);

         MineCraftProtocol actual = new MineCraftProtocol(doc.FirstChild);

         Assert.Equal("Beta 1.7.3", actual.MineCraftVersion);
         Assert.Equal(14, actual.ProtocolVersion);
         Assert.Equal(2, actual.Count);
      }

      [Fact]
      public void indexer()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(_xml);

         MineCraftProtocol actual = new MineCraftProtocol(doc.FirstChild);

         Assert.Throws<ArgumentOutOfRangeException>(() => actual[-1]);
         Assert.Throws<ArgumentOutOfRangeException>(() => actual[actual.Count]);

         IMineCraftPacketDefinition def = actual[0];
         Assert.Equal("KeepAlivePacket", def.Name);

         def = actual[1];
         Assert.Equal("LoginResponsePacket", def.Name);
      }
   }
}