using System;
using System.Collections;
using System.Collections.Generic;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace McPacketDisplay.Models
{
   public class TcpPacketList : IEnumerable<TcpPacket>
   {
      private readonly List<TcpPacket> _lst;
#region Construction
      private TcpPacketList(List<TcpPacket> lst)
      {
         _lst = lst;
      }

      public static TcpPacketList GetList(string filename)
      {
         Factory factory = new Factory();
         return factory.GetPacketList(filename);
      }

      private class Factory
      {
         private List<TcpPacket> _lst = new List<TcpPacket>();

         public TcpPacketList GetPacketList(string filename)
         {
            using (ICaptureDevice device = new CaptureFileReaderDevice(filename))
            {
               device.Open();
               device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
               device.Capture();
            }

            return new TcpPacketList(_lst);
         }

         private void device_OnPacketArrival(object sender, PacketCapture e)
         {
            RawCapture rawPacket = e.GetPacket();
            Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            TcpPacket tcp = packet.Extract<TcpPacket>();
            if (tcp is not null)
               _lst.Add(tcp);
         }
      }
      #endregion

      public IEnumerator<TcpPacket> GetEnumerator()
      {
         return _lst.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _lst.GetEnumerator();
      }
   }
}