using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using McPacketDisplay.Models;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftPacket : IMineCraftPacket, IList<IField>
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
      /// <returns>An IMineCraftPacket or null if the end of the stream has been reached.</returns>
      public static IMineCraftPacket GetPacket(IMineCraftProtocol protocol, Stream strm)
      {
         int n = strm.ReadByte();
         if (n < 0)
            return null;
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

      #region IList<IField>
      /// <inheritdoc />
      public IField this[int index]
      { 
         get => _lstFields[index];
         set => throw new NotSupportedException();
      }

      public bool IsReadOnly { get => true; }

      public void Add(IField field)
      {
         throw new NotSupportedException();
      }

      public void Clear()
      {
         throw new NotSupportedException();
      }

      /// <inheritdoc />
      public int Count { get => _lstFields.Count; }

      public bool Contains(IField field)
      {
         return _lstFields.Contains(field);
      }

      public void CopyTo(IField[] array, int arrayIndex)
      {
         _lstFields.CopyTo(array, arrayIndex);
      }

      public int IndexOf(IField item)
      {
         return _lstFields.IndexOf(item);
      }

      public void Insert(int index, IField item)
      {
         throw new NotSupportedException();
      }

      public bool Remove(IField item)
      {
         throw new NotSupportedException();
      }

      public void RemoveAt(int index)
      {
         throw new NotSupportedException();
      }

      public IEnumerator<IField> GetEnumerator()
      {
         return _lstFields.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _lstFields.GetEnumerator();
      }
      #endregion
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