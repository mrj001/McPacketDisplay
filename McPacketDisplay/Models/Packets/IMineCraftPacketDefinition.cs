using System;
using McPacketDisplay.Models;

namespace McPacketDisplay.Models.Packets
{

   public interface IMineCraftPacketDefinition
   {
      /// <summary>
      /// Gets the PacketID for the MineCraft Packet defined by this object.
      /// </summary>
      PacketID ID { get; }

      /// <summary>
      /// Gets the Source (sender) of the MineCraft Packet defined by this object.
      /// </summary>
      PacketSource From { get; }

      /// <summary>
      /// Gets the Name of the MineCraft Packet defined by this object.
      /// </summary>
      string Name { get; }

      /// <summary>
      /// Gets the FieldDefinition at the given index.
      /// </summary>
      /// <param name="index">The index of the FieldDefinition to return</param>
      /// <returns></returns>
      FieldDefinition this[int index] { get; }

      /// <summary>
      /// Gets the number of Field Definitions in this IMineCraftPacketDefinition.
      /// </summary>
      /// <value></value>
      int Count { get; }
   }
}