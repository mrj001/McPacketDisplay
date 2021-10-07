using System;

namespace McPacketDisplay.Models.Packets
{
   public interface IField
   {
      string Name { get; }

      object Value { get; }
   }

   public interface IArrayField : IField
   {
      int Count { get; }
   }
}