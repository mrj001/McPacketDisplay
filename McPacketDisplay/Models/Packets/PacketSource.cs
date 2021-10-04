using System;

namespace McPacketDisplay.Models.Packets
{
   /// <summary>
   /// Specifies the source of a MineCraft Packet.
   /// </summary>
   /// <remarks>
   /// The values in this enum correspond to the values listed in
   /// DirectionType in packets.xsd.
   /// </remarks>
   public enum PacketSource
   {
      Server,
      Client
   }

}