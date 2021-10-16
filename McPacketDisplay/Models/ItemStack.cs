using System;

namespace McPacketDisplay.Models.Packets
{
   public class ItemStack
   {
      public ItemStack(int itemID, int count, int uses)
      {
         ID = itemID;
         Count = count;
         Uses = uses;
      }

      public int ID { get; }

      public int Count { get; }

      public int Uses { get; }

      public static ItemStack Empty { get => new ItemStack(-1, 0, 0); }
   }
}