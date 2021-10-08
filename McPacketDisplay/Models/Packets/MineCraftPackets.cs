﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftPackets : IEnumerable<IMineCraftPacket>
   {
      private readonly List<IMineCraftPacket> _packets;

      private MineCraftPackets(IMineCraftProtocol protocol, NetworkStream strm)
      {
         _packets = new List<IMineCraftPacket>();
         IMineCraftPacket packet;
         do
         {
            packet = MineCraftPacket.GetPacket(protocol, strm);
            if (packet is not null)
               _packets.Add(packet);
         } while (packet is not null);
      }

      public static MineCraftPackets GetPackets(IMineCraftProtocol protocol, string filename)
      {
         TcpPacketList tcpPackets = TcpPacketList.GetList(filename);
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
