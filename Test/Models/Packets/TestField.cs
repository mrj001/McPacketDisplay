using System;
using System.Collections;
using System.IO;
using McPacketDisplay.Models.Packets;
using Moq;
using Xunit;

namespace Test.Models.Packets
{
   public class TestField
   {
      public static TheoryData<Type, object, string, FieldDataType, byte[]> GetField_Scalar_TestData
      {
         get
         {
            var rv = new TheoryData<Type, object, string, FieldDataType, byte[]>();

            rv.Add(typeof(ByteField), (sbyte)5, "Fred", FieldDataType.Byte, new byte[] { 0x05 });
            rv.Add(typeof(ShortField), (short)322, "Barney", FieldDataType.Short, new byte[] { 0x01, 0x42 });
            rv.Add(typeof(IntegerField), 67_305_985, "Wilma", FieldDataType.Integer, new byte[] { 0x04, 0x03, 0x02, 0x01 });
            rv.Add(typeof(LongField), 5_446_742_853_292_462_124, "Betty", FieldDataType.Long, new byte[] { 0x4b, 0x96, 0xb7, 0xc4, 0x59, 0x65, 0x10, 0x2c });
            rv.Add(typeof(FloatField), 0.0625f, "Pebbles", FieldDataType.Float, new byte[] { 0x3d, 0x80, 0x00, 0x00});
            rv.Add(typeof(DoubleField), 0.1, "BamBam", FieldDataType.Double, new byte[] { 0x3F, 0xB9, 0x99, 0x99, 0x99, 0x99, 0x99, 0x9A });
            rv.Add(typeof(StringField), "Furnace", "Dino", FieldDataType.String8, new byte[] { 0x00, 0x07, 0x46, 0x75, 0x72, 0x6e, 0x61, 0x63, 0x65 });
            rv.Add(typeof(StringField), "This is a Sign", "Text1", FieldDataType.String16,
                     new byte[] { 0x00, 0x0e, 0x00, 0x54, 0x00, 0x68, 0x00, 0x69, 0x00, 0x73, 0x00, 0x20, 0x00, 0x69, 0x00, 0x73, 
                     0x00, 0x20, 0x00, 0x61, 0x00, 0x20, 0x00, 0x53, 0x00, 0x69, 0x00, 0x67, 0x00, 0x6e });
            rv.Add(typeof(BoolField), true, "Accepted", FieldDataType.Bool, new byte[] { 0x01 });
            rv.Add(typeof(ItemStackField), ItemStack.Empty, "Item", FieldDataType.ItemStack, new byte[] { 0xff, 0xff });
            rv.Add(typeof(ItemStackField), new ItemStack(0x0064, 0x40, 0x0012), "TheItem", FieldDataType.ItemStack, new byte[] { 0x00, 0x64, 0x40, 0x00, 0x12 });

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(GetField_Scalar_TestData))]
      public void GetField_Scalar_Test(Type expectedType, object expectedValue, string fieldName, FieldDataType fieldDataType, byte[] streamData)
      {
         Mock<IFieldDefinition> mockFieldDefinition = new Mock<IFieldDefinition>(MockBehavior.Strict);
         mockFieldDefinition.Setup<string>((m) => m.Name).Returns(fieldName);
         mockFieldDefinition.Setup<FieldDataType>((m) => m.FieldType).Returns(fieldDataType);

         IField actual;
         using (MemoryStream ms = new MemoryStream(streamData))
            actual = Field.GetField(mockFieldDefinition.Object, ms);

         Assert.Equal(expectedType, actual.GetType());
         Assert.Equal(fieldName, actual.Name);
         Assert.False(object.ReferenceEquals(expectedValue, actual.Value));  // Must not be testing referential equality.
         Assert.Equal(expectedValue, actual.Value);
      }

      [Fact]
      public void GetField_Scalar_Throws()
      {
         Mock<IFieldDefinition> mockFieldDefinition = new Mock<IFieldDefinition>(MockBehavior.Strict);
         mockFieldDefinition.Setup<string>((m) => m.Name).Returns("SomeFieldOrOther");
         mockFieldDefinition.SetupSequence<FieldDataType>((m) => m.FieldType)
                  .Returns(FieldDataType.ByteArray)
                  .Returns(FieldDataType.ShortArray);

         byte[] bytes = new byte[] { 0x01, 0x02, 0x03 };
         using (MemoryStream ms = new MemoryStream(bytes))
         {
            Assert.Throws<ArgumentException>(() => Field.GetField(mockFieldDefinition.Object, ms));
            Assert.Throws<ArgumentException>(() => Field.GetField(mockFieldDefinition.Object, ms));
         }

      }

      public static TheoryData<Type, object, string, FieldDataType, byte[]> GetField_MetaData_TestData
      {
         get
         {
            var rv = new TheoryData<Type, object, string, FieldDataType, byte[]>();

            rv.Add(typeof(MetaDataField), new sbyte[] { 0, 0 }, "Gilligan", FieldDataType.Metadata, new byte[] { 0x00, 0x00, 0x7f });
            rv.Add(typeof(MetaDataField), new sbyte[] { 0, 0, 16, 0 }, "Skipper", FieldDataType.Metadata, new byte[] { 0x00, 0x00, 0x10, 0x00, 0x7f });

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(GetField_MetaData_TestData))]
      public void GetField_MetaData(Type expectedType, object expectedValue, string fieldName, FieldDataType fieldDataType, byte[] streamData)
      {
         Mock<IFieldDefinition> mockFieldDefinition = new Mock<IFieldDefinition>(MockBehavior.Strict);
         mockFieldDefinition.Setup<string>((m) => m.Name).Returns(fieldName);
         mockFieldDefinition.Setup<FieldDataType>((m) => m.FieldType).Returns(fieldDataType);

         IField actual;
         using (MemoryStream ms = new MemoryStream(streamData))
            actual = Field.GetField(mockFieldDefinition.Object, ms);

         Assert.Equal(expectedType, actual.GetType());
         Assert.Equal(fieldName, actual.Name);
         Assert.False(object.ReferenceEquals(expectedValue, actual.Value));  // Must not be testing referential equality.

         IEnumerator j = ((IEnumerable)expectedValue).GetEnumerator();
         IEnumerator k = ((IEnumerable)actual.Value).GetEnumerator();
         bool nextj = j.MoveNext();
         bool nextk = k.MoveNext();
         while (nextj && nextk)
         {
            Assert.Equal(j.Current, k.Current);
            nextj = j.MoveNext();
            nextk = k.MoveNext();
         }

         // Both collections must be the same size
         Assert.False(nextj);
         Assert.False(nextk);
      }

      public static TheoryData<Type, object, string, FieldDataType, byte[]> GetField_Array_TestData
      {
         get
         {
            var rv = new TheoryData<Type, object, string, FieldDataType, byte[]>();

            rv.Add(typeof(ByteArrayField), new byte[] { 3, 1, 4, 1 }, "ThatField", FieldDataType.ByteArray,
                     new byte[] { 0x03, 0x01, 0x04, 0x01 });

            rv.Add(typeof(ShortArrayField), new short[] { 3141, 42, 2718, 258 }, "AnotherField", FieldDataType.ShortArray,
                     new byte[] { 0x0c, 0x45, 0x00, 0x2a, 0x0a, 0x9e, 0x01, 0x02});

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(GetField_Array_TestData))]
      public void GetField_Array_Test(Type expectedType, object expectedValue, string fieldName, FieldDataType fieldDataType, byte[] streamData)
      {
         int count = ((Array)expectedValue).Length;

         Mock<IFieldDefinition> mockFieldDefinition = new Mock<IFieldDefinition>(MockBehavior.Strict);
         mockFieldDefinition.Setup<string>((m) => m.Name).Returns(fieldName);
         mockFieldDefinition.Setup<FieldDataType>((m) => m.FieldType).Returns(fieldDataType);

         IArrayField actual;
         using (MemoryStream ms = new MemoryStream(streamData))
            actual = Field.GetField(mockFieldDefinition.Object, ms, count);

         Assert.Equal(expectedType, actual.GetType());
         Assert.Equal(fieldName, actual.Name);
         Assert.Equal(count, actual.Count);

         for (int j = 0; j < count; j ++)
            Assert.Equal(((Array)expectedValue).GetValue(j), ((Array)actual.Value).GetValue(j));

         Assert.Equal(count, ((Array)actual.Value).Length);
         Assert.Throws<IndexOutOfRangeException>(() => ((Array)actual.Value).GetValue(count));
      }

      [Fact]
      public void GetField_Array_Throws()
      {
         FieldDataType[] checkTypes = new FieldDataType[]
         {
            FieldDataType.Byte,
            FieldDataType.Short,
            FieldDataType.Integer,
            FieldDataType.Long,
            FieldDataType.Float,
            FieldDataType.Double,
            FieldDataType.String8,
            FieldDataType.String16,
            FieldDataType.Bool,
            FieldDataType.Metadata
         };

         Mock<IFieldDefinition> mockFieldDefinition = new Mock<IFieldDefinition>(MockBehavior.Strict);
         mockFieldDefinition.Setup<string>((m) => m.Name).Returns("SomeFieldName");
         var setup = mockFieldDefinition.SetupSequence<FieldDataType>((m) => m.FieldType);
         for (int j = 0; j < checkTypes.Length; j++)
            setup = setup.Returns(checkTypes[j]);

         byte[] bytes = new byte[] { 0x01, 0x02, 0x03 };

         using (MemoryStream ms = new MemoryStream(bytes))
            for (int j = 0; j < checkTypes.Length; j ++)
               Assert.Throws<ArgumentException>(() => Field.GetField(mockFieldDefinition.Object, ms, 5));
      }
   }
}
