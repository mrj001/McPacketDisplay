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

      /// <summary>
      /// Gets the name of the field specifying the Array Length.
      /// </summary>
      /// <remarks>If this field is not an array type, this must return a null or empty string.</remarks>
      string ArrayLengthField { get; }

      /// <summary>
      /// Gets the Multiplier for the Array Length.
      /// </summary>
      int Multiplier { get; }
   }
}