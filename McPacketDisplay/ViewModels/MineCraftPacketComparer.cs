using System;
using System.Collections.Generic;
using McPacketDisplay.Models.Packets;

namespace McPacketDisplay.ViewModels
{
   public class MineCraftPacketComparer : IComparer<IMineCraftPacket>
   {
      private static MineCraftPacketComparer _comparer = new MineCraftPacketComparer();

      public static IComparer<IMineCraftPacket> Comparer => _comparer;

      public int Compare(IMineCraftPacket? x, IMineCraftPacket? y)
      {
         return (x?.PacketNumber ?? 0).CompareTo(y?.PacketNumber ?? 0);
      }
   }
}
