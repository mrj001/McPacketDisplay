using System;
using System.Net;
using SharpPcap;

namespace McPacketDisplay.Models
{
   public interface ITcpPacket : IComparable<ITcpPacket>
   {
      /// <summary>
      /// Gets the serial number of the TCP Packet within the underlying capture file.
      /// </summary>
      int Serial { get; }

      /// <summary>
      /// Gets the timestamp of the TCP Packet within the underyling capture file relative
      /// to the beginning of the file.
      /// </summary>
      PosixTimeval Timeval { get; }

      IPAddress SourceAddress { get; }

      ushort SourcePort { get; }

      IPAddress DestinationAddress { get; }

      ushort DestinationPort { get; }

      /// <summary>
      /// Gets the length of the TCP Packet's Payload data, in bytes.
      /// </summary>
      int PayloadDataLength { get; }

      /// <summary>
      /// Gets the value of the byte at the given index within the TCP Packet's payload data.
      /// </summary>
      /// <param name="index">the index of the byte.</param>
      /// <returns>The value of the byte at the given index within the TCP Packet's payload data.</returns>
      byte this[int index] { get; }
   }
}
