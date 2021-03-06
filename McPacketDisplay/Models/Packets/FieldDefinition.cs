using System;
using System.Xml;

namespace McPacketDisplay.Models.Packets
{
   public class FieldDefinition : IFieldDefinition
   {
      public FieldDefinition(XmlNode node)
      {
         if (node.Name != "field")
            throw new ArgumentException($"{nameof(node)} must be a <field> node.");

         XmlNode nameNode = node.FirstChild!;
         Name = nameNode.InnerText;

         XmlNode typeNode = nameNode.NextSibling!;
         FieldType = ParseTypeNode(typeNode);

         if (FieldType == FieldDataType.ByteArray || FieldType == FieldDataType.ShortArray)
         {
            XmlNode? lengthNode = typeNode.NextSibling;
            if (lengthNode is null)
               throw new ArgumentException($"Field {Name} contains an array type, but is missing it's length node.");

            XmlNode fieldNode = lengthNode.FirstChild!;
            ArrayLengthField = fieldNode.InnerText;

            XmlNode multiplierNode = fieldNode.NextSibling!;
            Multiplier = int.Parse(multiplierNode.InnerText);
         }
         else
         {
            if (typeNode.NextSibling is not null)
               throw new ArgumentException($"Field {Name} has a {(typeNode.NextSibling as XmlElement)?.Name ?? "extra"} node without an array type.");

            ArrayLengthField = string.Empty;
            Multiplier = 1;
         }
      }

      private static FieldDataType ParseTypeNode(XmlNode node)
      {
         switch(node.InnerText)
         {
            case "byte":
               return FieldDataType.Byte;

            case "byte[]":
               return FieldDataType.ByteArray;

            case "short":
               return FieldDataType.Short;

            case "short[]":
               return FieldDataType.ShortArray;

            case "int":
               return FieldDataType.Integer;

            case "long":
               return FieldDataType.Long;

            case "float":
               return FieldDataType.Float;

            case "double":
               return FieldDataType.Double;

            case "string8":
               return FieldDataType.String8;

            case "string16":
               return FieldDataType.String16;

            case "bool":
               return FieldDataType.Bool;

            case "metadata":
               return FieldDataType.Metadata;

            case "itemstack":
               return FieldDataType.ItemStack;

            default:
               throw new InvalidCastException($"Unable to interpret FieldDataType: {node.InnerText}");
         }
      }

      public string Name { get; }

      public FieldDataType FieldType { get; }

      public string ArrayLengthField { get; }

      public int Multiplier { get; }
   }
}
