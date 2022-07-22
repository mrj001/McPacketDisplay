using System;
using System.Collections;
using System.Collections.Generic;
using McPacketDisplay.ViewModels;
using PacketDotNet;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftPackets : IEnumerable<IMineCraftPacket>
   {
      private readonly List<IMineCraftPacket> _packets;

      private MineCraftPackets(IMineCraftProtocol protocol, IFilterTcpPackets filter, NetworkStream strm)
      {
         _packets = new List<IMineCraftPacket>();
         int packetNumber = 1;
         IMineCraftPacket packet;
         do
         {
            PacketSource packetSource = filter.GetPacketSource(strm.CurrentTcpPacket);
            packet = MineCraftPacket.GetPacket(packetNumber, protocol, packetSource, strm);
            if (packet is not null)
               _packets.Add(packet);
            packetNumber++;
         } while (strm.Position < strm.Length);
      }

      public static MineCraftPackets GetPackets(IMineCraftProtocol protocol, IFilterTcpPackets filter, string filename)
      {
         TcpPacketList tcpPackets = TcpPacketList.GetList(filename);
         using (NetworkStream strm = new NetworkStream(tcpPackets))
            return new MineCraftPackets(protocol, filter, strm);
      }

      public static MineCraftPackets GetPackets(IMineCraftProtocol protocol, IFilterTcpPackets filter, IEnumerable<ITcpPacket> tcpPackets)
      {
         using (NetworkStream strm = new NetworkStream(tcpPackets))
            return new MineCraftPackets(protocol, filter, strm);
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
