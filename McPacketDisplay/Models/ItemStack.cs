using System;
using System.Text;

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

      public override string ToString()
      {
         StringBuilder rv = new StringBuilder();

         rv.Append("{");
         rv.Append(ID);
         if (ID >= 0)
         {
            rv.Append("; ");
            rv.Append(Count);
            rv.Append("; ");
            rv.Append(Uses);
         }
         rv.Append("}");

         return rv.ToString();
      }
   }
}