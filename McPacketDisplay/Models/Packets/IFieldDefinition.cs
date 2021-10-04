using System;

namespace McPacketDisplay.Models.Packets
{
   public interface IFieldDefinition
   {
      /// <summary>
      /// Gets the Name of the Field.
      /// </summary>
      /// <value>The Name of the Field.</value>
      string Name { get; }

      /// <summary>
      /// Gets the data type of the Field.
      /// </summary>
      /// <value>The FieldDataType value of the Field.</value>
      FieldDataType FieldType { get; }
   }
}