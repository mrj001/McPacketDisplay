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

      #region Construction
      protected MineCraftPacket(PacketID packetID, IMineCraftPacketDefinition definition, Stream strm)
      {
         _packetID = packetID;
         _name = definition.Name;
         _source = definition.From;
         _lstFields = new List<IField>(definition.Count);
         GetFields(_lstFields, definition, strm);
      }

      /// <summary>
      /// Reads the set of Fields from the Stream.  Sub-classes may override this to provide for
      /// reading of a variable set of Fields.
      /// </summary>
      /// <param name="fields">The Fields being read are stored in this List.</param>
      /// <param name="definition">The definition of the MineCraft Packet.</param>
      /// <param name="strm">The Stream from which to read.</param>
      /// <returns>A List of IField instances.</returns>
      protected virtual void GetFields(List<IField> fields, IMineCraftPacketDefinition definition, Stream strm)
      {
         for (int j = 0; j < definition.Count; j++)
         {
            IFieldDefinition fieldDefinition = definition[j];
            IField field = GetField(fieldDefinition, strm);
            fields.Add(field);
         }
      }

      /// <summary>
      /// Reads a Field from the Stream.  Sub-classes override this to read fields that require 
      /// special handling.
      /// </summary>
      /// <param name="definition">The Field Definition to read.</param>
      /// <param name="strm">The input stream</param>
      /// <returns>An instance of IField read from the stream.</returns>
      protected virtual IField GetField(IFieldDefinition definition, Stream strm)
      {
         return Field.GetField(definition, strm);
      }

      protected MineCraftPacket(PacketID packetID, string name)
      {
         _packetID = packetID;
         _name = name;
         _lstFields = new List<IField>();
      }
      #endregion

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

         // TODO add Packet IDs with specific sub-classes.
         if (packetID == 0x33)
            return new MineCraftChunkDataPacket(packetID, definition, strm);
         else if (packetID == 0x66)
            return new MineCraftWindowClickPacket(packetID, definition, strm);
         else if (packetID == 0x67)
            return new MineCraftSetSlotPacket(packetID, definition, strm);
         else if (packetID == 0x68)
            return new MineCraftWindowItemsPacket(packetID, definition, strm);
         else
            return new MineCraftPacket(packetID, definition, strm);
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

      /// <inheritdoc />
      public IField? this[string fieldName]
      {
         get
         {
            foreach(IField j in _lstFields)
               if (j.Name == fieldName)
                  return j;

            return null;
         }
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

   public class MineCraftSetSlotPacket : MineCraftPacket
   {
      internal MineCraftSetSlotPacket(PacketID packetID, MineCraftPacketDefinition definition, Stream strm) :
               base(packetID, definition, strm)
      {
         
      }

      protected override IField GetField(IFieldDefinition definition, Stream strm)
      {
         // SMELL: This code is fragile in the event of renaming or re-ordering fields.
         //        Such may occur if we decide to support other versions of the 
         //        MineCraft Protocol.
         if ((short)(this["ItemID"]?.Value ?? (short)0) == -1)
         {
            if (definition.Name == "Count")
               return new ByteField(definition.Name, 0);

            if (definition.Name == "Metadata")
               return new ShortField(definition.Name, 0);
         }

         return base.GetField(definition, strm);
      }
   }

   public class MineCraftWindowItemsPacket : MineCraftPacket
   {
      internal MineCraftWindowItemsPacket(PacketID packetID, MineCraftPacketDefinition definition, Stream strm) : base(packetID, definition, strm)
      {

      }

      protected override IField GetField(IFieldDefinition definition, Stream strm)
      {
         // SMELL: This code is fragile in the event of renaming or re-ordering fields.
         //        Such may occur if we decide to support other versions of the 
         //        MineCraft Protocol.
         if (definition.Name != "Items")
            return base.GetField(definition, strm);

         int itemCount = (int)(short)(this["Count"]?.Value ?? 0);
         return new ItemArrayField("Items", strm, itemCount);
      }

      public int WindowID { get => (int)(sbyte)this["WindowID"]!.Value; }

      public int ItemCount { get => ((ItemStack[])this["Items"]!.Value).Length; }

      public ItemStack[] Items { get => ((ItemStack[])this["Items"]!.Value); }
   }

   public class MineCraftWindowClickPacket : MineCraftPacket
   {
      internal MineCraftWindowClickPacket(PacketID packetID, MineCraftPacketDefinition definition, Stream strm) :
               base(packetID, definition, strm)
      {

      }


      protected override IField GetField(IFieldDefinition definition, Stream strm)
      {
         // SMELL: This code is fragile in the event of renaming or re-ordering fields.
         //        Such may occur if we decide to support other versions of the 
         //        MineCraft Protocol.
         if ((short)(this["ItemID"]?.Value ?? (short)0) == -1)
         {
            if (definition.Name == "Count")
               return new ByteField(definition.Name, 0);

            if (definition.Name == "Metadata")
               return new ShortField(definition.Name, 0);
         }

         return base.GetField(definition, strm);
      }
   }

   public class MineCraftChunkDataPacket : MineCraftPacket
   {
      internal MineCraftChunkDataPacket(PacketID packetID, MineCraftPacketDefinition definition, Stream strm) :
               base(packetID, definition, strm)
      {

      }

      protected override IField GetField(IFieldDefinition definition, Stream strm)
      {
         if (definition.Name != "CompressedData")
            return base.GetField(definition, strm);

         int compressedSize = (int)this["CompressedSize"]!.Value;
         return Field.GetField(definition, strm, compressedSize);
      }
   }
}