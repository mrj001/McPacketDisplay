using System;
using System.IO;
using System.Text;

namespace McPacketDisplay.Models.Packets
{
   /// <summary>
   /// A collection of static methods to aid in reading MineCraft data types
   /// from a Stream
   /// </summary>
   public static class MineCraftStream
   {
      private static byte[] ReadBytes(Stream strm, int count)
      {
         byte[] rv = new byte[count];
         int value;

         for (int j = 0; j < count; j++)
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

      public static sbyte ReadByte(Stream strm)
      {
         int value = strm.ReadByte();
         if (value < 0)
            throw new EndOfStreamException();
         return (sbyte)value;
      }

      public static short ReadShort(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 2);
         return BitConverter.ToInt16(bytes, 0);
      }

      public static int ReadInt(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 4);
         return BitConverter.ToInt32(bytes);
      }

      public static float ReadFloat(Stream strm)
      {
         byte[] bytes = ReadBytes(strm, 4);
         return BitConverter.Int32BitsToSingle(BitConverter.ToInt32(bytes));
      }

      public static string ReadString16(Stream strm)
      {
         short count = ReadShort(strm);
         int codePoint;
         StringBuilder value = new StringBuilder(count);

         // NOTE: The string16 data type is encoded as "UCS-2", which is an
         //   archaic (and no longer supported) encoding.  UCS-2 is a sequence
         //   of 16-bit code points.  In string16, it is prefixed by a short
         //   specifying the length of the string in characters.
         for (int j = 0; j < count; j++)
         {
            codePoint = ReadShort(strm);
            // Despite the name, ConvertFromUtf32 actually takes a code
            // point as an argument, not a UTF-32 encoded character.
            value.Append(Char.ConvertFromUtf32(codePoint));
         }

         return value.ToString();
      }


      public static ItemStack ReadItemStack(Stream strm)
      {
         int itemID = ReadShort(strm);
         if (itemID == -1)
            return ItemStack.Empty;

         int count = ReadByte(strm);
         int uses = ReadShort(strm);

         return new ItemStack(itemID, count, uses);
      }
   }
}