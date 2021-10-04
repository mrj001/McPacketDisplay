using System;

namespace McPacketDisplay.Models.Packets
{
   public interface IMineCraftProtocol
   {
      string MineCraftVersion { get; }

      int ProtocolVersion { get; }

      public MineCraftPacketDefinition this[int index] { get; }

      int Count { get; }
      }
}