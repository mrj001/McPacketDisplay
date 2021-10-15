using System;
using System.IO;
using System.Text;

namespace McPacketDisplay.Models.Packets
{
   public abstract class Field : IField
   {
      private readonly string _name;

      protected Field(string name)
      {
         _name = name;
      }

      public static IField GetField(IFieldDefinition definition, Stream strm)
      {
         switch(definition.FieldType)
         {
            case FieldDataType.Byte:
               return new ByteField(definition.Name, strm);

            case FieldDataType.Short:
               return new ShortField(definition.Name, strm);

            case FieldDataType.ByteArray:
            case FieldDataType.ShortArray:
               throw new ArgumentException("Reading an array-valued field requires a length.");

            case FieldDataType.Integer:
               return new IntegerField(definition.Name, strm);

            case FieldDataType.Long:
               return new LongField(definition.Name, strm);

            case FieldDataType.Float:
               return new FloatField(definition.Name, strm);

            case FieldDataType.Double:
               return new DoubleField(definition.Name, strm);

            case FieldDataType.String8:
               return StringField.GetString8Field(definition.Name, strm);

            case FieldDataType.String16:
               return StringField.GetString16Field(definition.Name, strm);

            case FieldDataType.Bool:
               return new BoolField(definition.Name, strm);

            case FieldDataType.Metadata:
               throw new NotImplementedException();

            default:
               throw new ArgumentException($"{nameof(definition)} contains an unknown value for the Field Data Type.");
         }
      }

      public static IArrayField GetField(IFieldDefinition definition, Stream strm, int count)
      {
         switch(definition.FieldType)
         {
            case FieldDataType.ByteArray:
               return new ByteArrayField(definition.Name, strm, count);

            case FieldDataType.ShortArray:
               return new ShortArrayField(definition.Name, strm, count);

            default:
               throw new ArgumentException($"{nameof(definition)} contains an unknown value for the Field Data Type.");
         }
      }

      protected static byte[] ReadBytes(Stream strm, int count)
      {
         byte[] rv = new byte[count];
         int value;

         for (int j = 0; j < count; j ++)
         {
            value = strm.ReadByte();
            if (value < 0)
               throw new EndOfStreamException();
            rv[j] = (byte)value;
         }

         if (BitConverter.IsLittleEndian)
            Array.Reverse(rv);

         return rv;
      }

      protected static short ReadShort(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 2);
         return BitConverter.ToInt16(bytes, 0);
      }

      public string Name { get => _name; }

      public abstract object Value { get; }
   }


   public class ByteField : Field
   {
      private readonly sbyte _value;
      internal ByteField(string name, Stream strm) : base(name)
      {
         int value = strm.ReadByte();
         if (value < 0)
            throw new EndOfStreamException();

         _value = (sbyte)value;
      }

      public override object Value { get => _value; }
   }

   public class ShortField : Field
   {
      private readonly short _value;

      internal ShortField(string name, Stream strm) : base(name)
      {
         _value = ReadShort(strm);
      }

      public override object Value { get => _value; }
   }

   public class IntegerField : Field
   {
      private readonly int _value;

      internal IntegerField(string name, Stream strm) : base(name)
      {
         _value = ReadInteger(strm);
      }

      protected static int ReadInteger(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 4);
         return BitConverter.ToInt32(bytes, 0);
      }

      public override object Value { get => _value; }
   }

   public class LongField : Field
   {
      private readonly long _value;

      internal LongField(string name, Stream strm) : base(name)
      {
         _value = ReadLong(strm);
      }

      protected static long ReadLong(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 8);
         return BitConverter.ToInt64(bytes, 0);
      }

      public override object Value { get => _value; }
   }

   public class FloatField : Field
   {
      private readonly float _value;

      internal FloatField(string name, Stream strm) : base(name)
      {
         _value = ReadFloat(strm);
      }

      protected static float ReadFloat(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 4);
         return BitConverter.ToSingle(bytes, 0);
      }

      public override object Value { get => _value; }
   }

   public class DoubleField : Field
   {
      private readonly double _value;

      internal DoubleField(string name, Stream strm) : base(name)
      {
         _value = ReadDouble(strm);
      }

      protected static double ReadDouble(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 8);
         return BitConverter.ToDouble(bytes, 0);
      }

      public override object Value { get => _value; }
   }

   public class StringField : Field
   {
      private readonly string _value;

      private StringField(string name, string value) : base(name)
      {
         _value = value;
      }

      internal static StringField GetString8Field(string name, Stream strm)
      {
         short count = ReadShort(strm);
         StringBuilder value = new StringBuilder(count);

         // NOTE: The BinaryReader encodes a string's length as an
         //   integer encoded 7 bits at a time.
         //   The string8 data type encodes the string's length as a
         //   big-endian short.
         //   Thus, the BinaryReader's ReadString method cannot be used.
         // NOTE:  The string8 data type is encoded as "Modified UTF-8".
         //   By reading it as UTF-8, there is some risk that null characters
         //   will be mis-interpreted.  However, .NET has no support for
         //    Modified UTF-8 as that is a Java concept.
         using (BinaryReader rdr = new BinaryReader(strm, Encoding.UTF8, true))
            for (int j = 0; j < count; j ++)
               value.Append(rdr.ReadChar());

         return new StringField(name, value.ToString());
      }

      internal static StringField GetString16Field(string name, Stream strm)
      {
         short count = ReadShort(strm);
         int codePoint;
         StringBuilder value = new StringBuilder(count);

         // NOTE: The string16 data type is encoded as "UCS-2", which is an
         //   archaic (and no longer supported) encoding.  UCS-2 is a sequence
         //   of 16-bit code points.  In string16, it is prefixed by a short
         //   specifying the length of the string in characters.
         for (int j = 0; j < count; j ++)
         {
            codePoint = ReadShort(strm);
            // Despite the name, ConvertFromUtf32 actually takes a code
            // point as an argument, not a UTF-32 encoded character.
            value.Append(Char.ConvertFromUtf32(codePoint));
         }

         return new StringField(name, value.ToString());
      }

      public override object Value { get => _value; }
   }

   public class BoolField : Field
   {
      private readonly bool _value;

      internal BoolField(string name, Stream strm) : base(name)
      {
         int b = strm.ReadByte();
         if (b < 0)
            throw new EndOfStreamException();

         _value = (b != 0);
      }

      public override object Value { get => _value; }
   }

   public abstract class ArrayField : Field, IArrayField
   {
      private readonly int _count;

      protected ArrayField(string name, int count) : base(name)
      {
         _count = count;
      }

      public int Count { get => _count; }
   }

   public class ByteArrayField : ArrayField
   {
      private readonly byte[] _value;

      internal ByteArrayField(string name, Stream strm, int count) : base(name, count)
      {
         _value = new byte[count];
         for (int j = 0; j < count; j ++)
         {

            int val = strm.ReadByte();
            if (val < 0)
               throw new EndOfStreamException();
            _value[j] = (byte)val;
         }
      }

      public override object Value { get => _value; }
   }

   public class ShortArrayField : ArrayField
   {
      private readonly short[] _value;

      internal ShortArrayField(string name, Stream strm, int count) : base(name, count)
      {
         _value = new short[count];
         for (int j = 0; j < count; j ++)
            _value[j] = ReadShort(strm);
      }

      public override object Value { get => _value; }
   }
}