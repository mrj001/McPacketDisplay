using System;
using System.Collections;
using System.Collections.Generic;
using PacketDotNet;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftPackets : IEnumerable<IMineCraftPacket>
   {
      private readonly List<IMineCraftPacket> _packets;

      private MineCraftPackets(IMineCraftProtocol protocol, NetworkStream strm)
      {
         _packets = new List<IMineCraftPacket>();
         int packetNumber = 1;
         IMineCraftPacket packet;
         do
         {
            packet = MineCraftPacket.GetPacket(packetNumber, protocol, strm);
            if (packet is not null)
               _packets.Add(packet);
            packetNumber++;
         } while (packet is not null);
      }

      public static MineCraftPackets GetPackets(IMineCraftProtocol protocol, string filename)
      {
         TcpPacketList tcpPackets = TcpPacketList.GetList(filename);
         using (NetworkStream strm = new NetworkStream(tcpPackets))
            return new MineCraftPackets(protocol, strm);
      }

      public static MineCraftPackets GetPackets(IMineCraftProtocol protocol, IEnumerable<TcpPacket> tcpPackets)
      {
         using (NetworkStream strm = new NetworkStream(tcpPackets))
            return new MineCraftPackets(protocol, strm);
      }

      public IEnumerator<IMineCraftPacket> GetEnumerator()
      {
         return _packets.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _packets.GetEnumerator();
      }
   }
}
