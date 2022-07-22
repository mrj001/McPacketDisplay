using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

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

      private int _position;

      private readonly long _length;

      public NetworkStream(IEnumerable<ITcpPacket> packets)
      {
         long len = 0;
         _packets = packets.GetEnumerator();
         while (_packets.MoveNext())
            len += _packets.Current.PayloadDataLength;
         _length = len;
         _position = 0;
         _packets.Reset();
         _endOfStream = !_packets.MoveNext();
         _packetIndex = 1;  // WireShark numbers TCP packets from 1.
         _indexWithinPacket = 0;
      }

      public override long Position
      {
         get => _position;
         set => throw new NotSupportedException();
      }

      public override long Length
      {
         get => _length;
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

         int rv = _packets.Current[_indexWithinPacket];
         _indexWithinPacket++;
         _position++;

         if (_indexWithinPacket >= _packets.Current.PayloadDataLength)
         {
            do
            {
               _endOfStream = !_packets.MoveNext();
               _packetIndex++;
            } while (!_endOfStream && _packets.Current.PayloadDataLength == 0);
            _indexWithinPacket = 0;
         }

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

      private void ValidateStreamPosition()
      {
         if (_endOfStream)
            throw new InvalidOperationException("Network Stream is past the end.");
      }

      /// <summary>
      /// Gets the current underlying TCP Packet from the stream of TCP Packets
      /// that back this Stream.
      /// </summary>
      public ITcpPacket CurrentTcpPacket
      {
         get
         {
            ValidateStreamPosition();
            return _packets.Current;
         }
      }
   }
}