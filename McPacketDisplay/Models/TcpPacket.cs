using System;
using System.Collections.Generic;
using System.Net;
using SharpPcap;

namespace McPacketDisplay.Models
{
   public class TcpPacket : ITcpPacket
   {
      private readonly int _serial;

      private readonly PosixTimeval _timeval;

      private PacketDotNet.TcpPacket _packet;

      public TcpPacket(int serial, PosixTimeval timeval, PacketDotNet.TcpPacket packet)
      {
         _serial = serial;
         _timeval = timeval;
         _packet = packet;
      }

      public int Serial { get => _serial; }

      public PosixTimeval Timeval { get => _timeval; }

      public IPAddress SourceAddress
      {
         get => ((PacketDotNet.IPPacket)_packet.ParentPacket).SourceAddress;
      }

      public ushort SourcePort { get => _packet.SourcePort; }

      public IPAddress DestinationAddress
      {
         get => ((PacketDotNet.IPPacket)_packet.ParentPacket).DestinationAddress;
      }

      public ushort DestinationPort { get => _packet.DestinationPort; }

      public int PayloadDataLength { get => _packet.PayloadData.Length; }

      public byte this[int index] { get => _packet.PayloadData[index]; }

      public int CompareTo(ITcpPacket? other)
      {
         if (other is null) return 1;
         return _serial.CompareTo(other.Serial);
      }
   }

   public class TcpPacketComparer : IComparer<ITcpPacket>
   {
      public int Compare(ITcpPacket? x, ITcpPacket? y)
      {
         return x!.Serial.CompareTo(y!.Serial);
      }
   }
}
