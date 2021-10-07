using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using McPacketDisplay.Models;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftPacket : IMineCraftPacket
   {
      private readonly PacketID _packetID;

      private readonly string _name;

      private readonly PacketSource _source;

      private readonly List<IField> _lstFields;

      protected MineCraftPacket(PacketID packetID, IMineCraftPacketDefinition definition, Stream strm)
      {
         _packetID = packetID;
         _name = definition.Name;
         _source = definition.From;

         _lstFields = new List<IField>();
         for (int j = 0; j < definition.Count; j++)
         {
            IFieldDefinition fieldDefinition = definition[j];
            IField field = Field.GetField(fieldDefinition, strm);
            _lstFields.Add(field);
         }
      }

      protected MineCraftPacket(PacketID packetID, string name)
      {
         _packetID = packetID;
         _name = name;
         _lstFields = new List<IField>();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="protocol"></param>
      /// <param name="strm"></param>
      /// <returns></returns>
      public static IMineCraftPacket GetPacket(IMineCraftProtocol protocol, Stream strm)
      {
         int n = strm.ReadByte();
         if (n < 0)
            throw new EndOfStreamException();
         PacketID packetID = new PacketID(n);

         int j = 0;
         int jul = protocol.Count;
         while (j < jul && protocol[j].ID != packetID)
            j++;
         if (j == jul)
            return new MineCraftPacketUnknown(packetID);
         MineCraftPacketDefinition definition = protocol[j];

         switch(packetID)
         {
            // TODO add Packet IDs with specific sub-classes.
            default:
               return new MineCraftPacket(packetID, definition, strm);
         }
      }

      /// <inheritdoc />
      public PacketID ID { get => _packetID; }

      /// <inheritdoc />
      public string Name { get => _name; }

      public PacketSource From { get => _source; }

      /// <inheritdoc />
      public IField this[int index] { get => _lstFields[index]; }

      /// <inheritdoc />
      public int Count { get => _lstFields.Count; }

      public IEnumerator<IField> GetEnumerator()
      {
         return _lstFields.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _lstFields.GetEnumerator();
      }
   }

   /// <summary>
   /// Represents a MineCraft Packet which cannot be identified.
   /// </summary>
   public class MineCraftPacketUnknown : MineCraftPacket
   {
      internal MineCraftPacketUnknown(PacketID packetID) : base(packetID, "Unknown Packet")
      {
      }
   }
}