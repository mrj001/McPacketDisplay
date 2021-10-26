using System;
using System.Collections;
using System.Collections.Generic;

namespace McPacketDisplay.Models.Packets
{
   public interface IMineCraftProtocol : IEnumerable<IMineCraftPacketDefinition>
   {
      string MineCraftVersion { get; }

      int ProtocolVersion { get; }

      public IMineCraftPacketDefinition this[int index] { get; }

      int Count { get; }
      }
}