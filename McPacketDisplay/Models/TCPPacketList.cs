using System;
using System.Collections;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.LibPcap;

namespace McPacketDisplay.Models
{
   public class TcpPacketList : IEnumerable<ITcpPacket>
   {
      private readonly List<ITcpPacket> _lst;
      #region Construction
      private TcpPacketList()
      {
         _lst = new List<ITcpPacket>(1);
      }

      private TcpPacketList(List<ITcpPacket> lst)
      {
         _lst = lst;
      }

      public static TcpPacketList GetList(string filename)
      {
         if (string.IsNullOrEmpty(filename))
            return new TcpPacketList();

         Factory factory = new Factory();
         return factory.GetPacketList(filename);
      }

      private class Factory
      {
         private List<ITcpPacket> _lst = new List<ITcpPacket>();
         private int serial = 0;
         private PosixTimeval? _baseTime = null;

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
            PacketDotNet.Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            PacketDotNet.TcpPacket tcp = packet.Extract<PacketDotNet.TcpPacket>();
            if (tcp is not null)
            {
               serial++;
               PosixTimeval relativeTime = GetRelativeTime(rawPacket.Timeval);
               _lst.Add(new TcpPacket(serial, relativeTime, tcp));
            }
         }

         private PosixTimeval GetRelativeTime(PosixTimeval posixTimeval)
         {
            ulong relmicro;
            ulong relsec;

            if (_baseTime is not null)
            {
               if (posixTimeval.MicroSeconds <= _baseTime.MicroSeconds)
               {
                  relmicro = _baseTime.MicroSeconds - posixTimeval.MicroSeconds;
                  relsec = _baseTime.Seconds - posixTimeval.Seconds;
               }
               else
               {
                  relmicro = 1_000_000 + _baseTime.MicroSeconds - posixTimeval.MicroSeconds;
                  relsec = _baseTime.Seconds - 1 - posixTimeval.Seconds;
               }
            }
            else
            {
               _baseTime = posixTimeval;
               relsec = 0;
               relmicro = 0;
            }

            return new PosixTimeval(relsec, relmicro);
         }
      }
      #endregion

      public IEnumerator<ITcpPacket> GetEnumerator()
      {
         return _lst.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _lst.GetEnumerator();
      }
   }
}