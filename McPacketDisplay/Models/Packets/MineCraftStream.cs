using System;
using System.IO;

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