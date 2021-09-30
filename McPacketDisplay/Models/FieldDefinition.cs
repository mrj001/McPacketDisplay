using System;
using System.Xml;

namespace McPacketDisplay.Models
{
   public class FieldDefinition
   {
      public enum FieldDataType
      {
            Byte,
            ByteArray,
            Short,
            ShortArray,
            Integer,
            Long,
            Float,
            Double,
            String8,
            String16,
            Bool,
            Metadata
      }

      public FieldDefinition(XmlNode node)
      {
         if (node.Name != "field")
            throw new ArgumentException($"{nameof(node)} must be a <field> node.");

         XmlNode nameNode = node.FirstChild!;
         Name = nameNode.InnerText;

         XmlNode typeNode = nameNode.NextSibling!;
         FieldType = ParseTypeNode(typeNode);
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

            default:
               throw new InvalidCastException($"Unable to interpret FieldDataType: {node.InnerText}");
         }
      }

      public string Name { get; }

      public FieldDataType FieldType { get; }

      // TODO Add code to parse Field values from a byte stream
   }
}
