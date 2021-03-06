using System;
using System.Collections.Generic;

namespace McPacketDisplay.Models.Packets
{
   public interface IMineCraftPacket : IEnumerable<IField>
   {
      /// <summary>
      /// Gets the position of this MineCraft Packet within the stream of MineCraft Packets.
      /// </summary>
      int PacketNumber { get; }

      /// <summary>
      /// Gets the Packet ID of the MineCraftPacket.
      /// </summary>
      /// <value>Returns the Packet ID.</value>
      PacketID ID { get; }

      /// <summary>
      /// Gets the name of the MineCraftPacket.
      /// </summary>
      /// <value></value>
      string Name { get; }

      /// <summary>
      /// Gets which of Client or Server originated the Packet.
      /// </summary>
      /// <value></value>
      PacketSource From { get; }

      /// <summary>
      /// Gets the index'th Field of the MineCraftPacket
      /// </summary>
      /// <value></value>
      IField this[int index] { get; }

      /// <summary>
      /// Gets the Field specified by Name.
      /// </summary>
      /// <value></value>
      IField? this[string name] { get; }

      /// <summary>
      /// Gets the number of Fields that this Packet contains.
      /// </summary>
      /// <value></value>
      int Count { get; }
   }
}