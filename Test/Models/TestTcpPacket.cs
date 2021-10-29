using System;
using System.Collections.Generic;
using McPacketDisplay.Models;
using SharpPcap;
using SharpPcap.LibPcap;
using Xunit;

namespace Test.Models
{
   public class TestTcpPacket
   {
      private List<ITcpPacket> _lst = new List<ITcpPacket>();
      private int serial = 0;
      private PosixTimeval _baseTime = null;

      public TestTcpPacket()
      {
         using (ICaptureDevice device = new CaptureFileReaderDevice("Files/FourPackets.pcap"))
         {
            device.Open();
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device.Capture();
         }
      }

      private void device_OnPacketArrival(object sender, PacketCapture e)
      {
         RawCapture rawPacket = e.GetPacket();
         PacketDotNet.Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

         PacketDotNet.TcpPacket tcp = packet.Extract<PacketDotNet.TcpPacket>();
         PosixTimeval relativeTime;

         if (_baseTime is not null)
         {
            ulong relmicro;
            ulong relsec;

            if (rawPacket.Timeval.MicroSeconds > _baseTime.MicroSeconds)
            {
               relmicro = rawPacket.Timeval.MicroSeconds - _baseTime.MicroSeconds;
               relsec = rawPacket.Timeval.Seconds - _baseTime.Seconds;
            }
            else
            {
               relmicro = 1_000_000 + rawPacket.Timeval.MicroSeconds - _baseTime.MicroSeconds;
               relsec = rawPacket.Timeval.Seconds - 1 - _baseTime.Seconds;
            }
            relativeTime = new PosixTimeval(relsec, relmicro);
         }
         else
         {
            _baseTime = rawPacket.Timeval;
            relativeTime = new PosixTimeval(0, 0);
         }

         if (tcp is not null)
         {
            serial++;
            _lst.Add(new TcpPacket(serial, relativeTime, tcp));
         }
      }

      [Fact]
      public void properties()
      {
         ITcpPacket actual = _lst[0];

         Assert.Equal(1, actual.Serial);
         Assert.Equal(new PosixTimeval(0, 0), actual.Timeval);
         Assert.Equal("127.0.0.1", actual.SourceAddress.ToString());
         Assert.Equal(58505, actual.SourcePort);
         Assert.Equal("127.0.0.1", actual.DestinationAddress.ToString());
         Assert.Equal(25565, actual.DestinationPort);
         Assert.Equal(2, actual.PayloadDataLength);
         Assert.Equal(10, actual[0]);
         Assert.Equal(1, actual[1]);

         actual = _lst[1];
         Assert.Equal((ulong)0, actual.Timeval.Seconds);
         Assert.Equal((ulong)13_200, actual.Timeval.MicroSeconds);
      }

      [Fact]
      public void CompareTo()
      {
         List<ITcpPacket> lst = new List<ITcpPacket>(_lst.Count);
         for (int j = 0; j < _lst.Count; j++)
            lst.Add(_lst[_lst.Count - 1 - j]);

         // Sort will call ITcpPacket.CompareTo
         lst.Sort();

         for (int j = 0; j < lst.Count; j++)
            Assert.Equal(j + 1, lst[j].Serial);
      }

      [Fact]
      public void Comparer()
      {
         List<ITcpPacket> lst = new List<ITcpPacket>(_lst.Count);
         for (int j = 0; j < _lst.Count; j++)
            lst.Add(_lst[_lst.Count - 1 - j]);

         // Sort will call the Comparer
         lst.Sort(new TcpPacketComparer());

         for (int j = 0; j < lst.Count; j++)
            Assert.Equal(j + 1, lst[j].Serial);
      }
   }
}
