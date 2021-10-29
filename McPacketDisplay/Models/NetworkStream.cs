using System;
using System.Collections.Generic;
using System.IO;

namespace McPacketDisplay.Models
{
   public class NetworkStream : Stream
   {
      private readonly IEnumerator<ITcpPacket> _packets;

      private int _indexWithinPacket;

      // Counts the number of packets in the input IEnumerable.
      // Useful for finding the TCP packet within an input file with WireShark.
      private int _packetIndex;

      private bool _endOfStream;

      public NetworkStream(IEnumerable<ITcpPacket> packets)
      {
         _packets = packets.GetEnumerator();
         _endOfStream = !_packets.MoveNext();
         _packetIndex = 1;  // WireShark numbers TCP packets from 1.
         _indexWithinPacket = 0;
      }

      public override long Position
      {
         get => throw new NotSupportedException();
         set => throw new NotSupportedException();
      }

      public override long Length 
      {
         get => throw new NotSupportedException();
      }

      public override void SetLength(long value)
      {
         throw new NotSupportedException();
      }

      public override bool CanSeek { get => false; }

      public override long Seek(long offset, SeekOrigin origin)
      {
         throw new NotSupportedException();
      }

      public override bool CanRead { get => true; }

      public override int Read(byte[] buffer, int offset, int count)
      {
         int bytesRead = 0;
         int readByte = ReadByte();
         while (bytesRead < count && readByte >= 0)
         {
            buffer[bytesRead + offset] = (byte)readByte;

            bytesRead++;
            readByte = ReadByte();
         }

         return bytesRead;
      }

      public override int ReadByte()
      {
         if (_endOfStream) return -1;

         if (_indexWithinPacket >= _packets.Current.PayloadDataLength)
         {
            do
            {
               _endOfStream = !_packets.MoveNext();
               _packetIndex++;
            } while (!_endOfStream && _packets.Current.PayloadDataLength == 0);
            _indexWithinPacket = 0;
            if (_endOfStream) return -1;
         }

         int rv = _packets.Current[_indexWithinPacket];
         _indexWithinPacket++;
         return rv;
      }

      public override bool CanWrite { get => false; }

      public override void Write(byte[] buffer, int offset, int count)
      {
         throw new NotSupportedException();
      }

      public override void Flush()
      {
         // empty due to being read-only
      }
   }
}