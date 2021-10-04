using System;
using System.Xml;
using McPacketDisplay.Models.Packets;
using Xunit;

namespace Test.Models
{
   public class TestFieldDefinition
   {
      public static TheoryData<string, FieldDataType, string> ctor_TestData
      {
         get
         {
            var rv = new TheoryData<string, FieldDataType, string>();
            rv.Add("TheName", FieldDataType.String8, "<field><name>TheName</name><type>string8</type></field>");
            rv.Add("EntityID", FieldDataType.Integer, "<field><name>EntityID</name><type>int</type></field>");
            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(ctor_TestData))]
      public void ctor_Test(string expectedName, FieldDataType expectedType, string xml)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(xml);

         FieldDefinition actual = new FieldDefinition(doc.FirstChild);

         Assert.Equal(expectedName, actual.Name);
         Assert.Equal(expectedType, actual.FieldType);
      }

      [Fact]
      public void ctor_Throws()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<field><name>FriedEgg</name><type>UnknownType</type></field>");

         Assert.Throws<InvalidCastException>(() => new FieldDefinition(doc.FirstChild));
      }
   }
}
