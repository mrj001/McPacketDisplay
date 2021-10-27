using System;
using System.Xml;
using McPacketDisplay.Models.Packets;
using Xunit;

namespace Test.Models
{
   public class TestFieldDefinition
   {
      public static TheoryData<string, FieldDataType, string, int, string> ctor_TestData
      {
         get
         {
            var rv = new TheoryData<string, FieldDataType, string, int, string>();
            rv.Add("TheName", FieldDataType.String8, "", 1, "<field><name>TheName</name><type>string8</type></field>");
            rv.Add("EntityID", FieldDataType.Integer, null, 1, "<field><name>EntityID</name><type>int</type></field>");
            rv.Add("CompressedData", FieldDataType.ByteArray, "CompressedSize", 1,
                   "<field><name>CompressedData</name><type>byte[]</type><length><field>CompressedSize</field><multiplier>1</multiplier></length></field>");
            rv.Add("AffectedBlocks", FieldDataType.ByteArray, "RecordCount", 3,
                   "<field><name>AffectedBlocks</name><type>byte[]</type><length><field>RecordCount</field><multiplier>3</multiplier></length></field>");

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(ctor_TestData))]
      public void ctor_Test(string expectedName, FieldDataType expectedType,
               string expectedLengthField, int expectedMultiplier, string xml)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(xml);

         FieldDefinition actual = new FieldDefinition(doc.FirstChild);

         Assert.Equal(expectedName, actual.Name);
         Assert.Equal(expectedType, actual.FieldType);
         if (string.IsNullOrEmpty(expectedLengthField))
            Assert.True(string.IsNullOrEmpty(actual.ArrayLengthField));
         else
            Assert.Equal(expectedLengthField, actual.ArrayLengthField);
         Assert.Equal(expectedMultiplier, actual.Multiplier);
      }

      [Fact]
      public void ctor_Throws()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<field><name>FriedEgg</name><type>UnknownType</type></field>");

         Assert.Throws<InvalidCastException>(() => new FieldDefinition(doc.FirstChild));
      }

      [Fact]
      public void ctor_Missing_Length_Throws()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<field><name>AffectedBlocks</name><type>byte[]</type></field>");

         Assert.Throws<ArgumentException>(() => new FieldDefinition(doc.FirstChild));
      }

      [Fact]
      public void ctor_Extraneous_Length_Node_Throws()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<field><name>TheName</name><type>string8</type><length><field>RecordCount</field><multiplier>3</multiplier></length></field>");

         Assert.Throws<ArgumentException>(() => new FieldDefinition(doc.FirstChild));
      }
   }
}
