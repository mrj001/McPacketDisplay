using System;
using System.Xml;
using McPacketDisplay.Models;
using Xunit;

namespace Test.Models
{
   public class TestPacketID
   {
      public static TheoryData<int, string> PacketID_TestData
      {
         get
         {
            TheoryData<int, string> rv = new TheoryData<int, string>();
            rv.Add(5, "<id>05</id>");
            rv.Add(10, "<id>0a</id>");
            rv.Add(130, "<id>82</id>");
            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(PacketID_TestData))]
      public void PacketID(int expectedID, string xml)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(xml);

         PacketID actual = new PacketID(doc.FirstChild);

         Assert.Equal(expectedID, actual.ID);
      }

      [Fact]
      public void ctor_Int()
      {
         int id = 5;
         PacketID actual = new PacketID(id);

         Assert.Equal(id, actual.ID);
      }

      public static TheoryData<string, string> ToString_TestData
      {
         get
         {
            TheoryData<string, string> rv = new TheoryData<string, string>();
            rv.Add("0x05", "<id>05</id>");
            rv.Add("0x0a", "<id>0a</id>");
            rv.Add("0x6f", "<id>6f</id>");
            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(ToString_TestData))]
      public void ToString_Test(string expected, string xml)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(xml);
         PacketID id = new PacketID(doc.FirstChild);

         string actual = id.ToString();

         Assert.Equal(expected, actual);
      }

      public static TheoryData<bool, string, string> Equals_TestData
      {
         get
         {
            TheoryData<bool, string, string> rv = new TheoryData<bool, string, string>();
            rv.Add(true, "<id>07</id>", "<id>07</id>");
            rv.Add(false, "<id>42</id>", "<id>24</id>");
            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(Equals_TestData))]
      public void Equals_Test(bool expected, string xml1, string xml2)
      {
         XmlDocument doc1 = new XmlDocument();
         doc1.LoadXml(xml1);
         PacketID id1 = new PacketID(doc1.FirstChild);
         XmlDocument doc2 = new XmlDocument();
         doc2.LoadXml(xml2);
         PacketID id2 = new PacketID(doc2.FirstChild);

         Assert.Equal(expected, id1.Equals((object)id2));
         Assert.Equal(expected, id2.Equals((object)id1));
         Assert.Equal(expected, id1.Equals(id2));
         Assert.Equal(expected, id2.Equals(id1));

         Assert.Equal(expected, id1 == id2);
         Assert.Equal(expected, id2 == id1);
         Assert.Equal(!expected, id1 != id2);
         Assert.Equal(!expected, id2 != id1);
      }

      [Fact]
      public void Not_Equal_Null()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<id>1f</id>");
         PacketID id = new PacketID(doc.FirstChild);

         Assert.False(id.Equals(null));
         Assert.False(id.Equals((PacketID)null));

         Assert.False(null == id);
         Assert.False(id == null);

         Assert.True(null != id);
         Assert.True(id != null);
      }

      [Fact]
      public void Not_Equal_Invalid()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<id>21</id>");
         PacketID id = new PacketID(doc.FirstChild);

         Assert.False(id.Equals("invalid cast"));
      }
   }
}
