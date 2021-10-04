using System;
using System.Collections.Generic;
using System.Xml;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftPacketDefinition
   {
      private List<FieldDefinition> _fields;

      /// <summary>
      /// Constructs a new MineCraftPacketDefinition
      /// </summary>
      /// <param name="node">An XmlNode as specified by the PacketType in packets.xsd.</param>
      public MineCraftPacketDefinition(XmlNode node)
      {
         if (node.Name != "packet")
            throw new ArgumentException($"{nameof(node)} must be a <packet> node.");

         XmlNode nameNode = node.FirstChild!;
         Name = nameNode.InnerText;

         XmlNode fromNode = nameNode.NextSibling!;
         switch(fromNode.InnerText)
         {
            case "server":
               From = PacketSource.Server;
               break;

            case "client":
               From = PacketSource.Client;
               break;

            default:
               throw new InvalidCastException($"{fromNode.InnerText} cannot be converted to a source.");
         }

         XmlNode idNode = fromNode.NextSibling!;
         ID = new PacketID(idNode);

         _fields = new List<FieldDefinition>();
         XmlNode fieldsNode = idNode.NextSibling!;
         XmlNode fieldNode = fieldsNode.FirstChild!;
         while (fieldNode is not null)
         {
            _fields.Add(new FieldDefinition(fieldNode));
            fieldNode = fieldNode.NextSibling!;
         }
      }

      /// <summary>
      /// Gets the PacketID for the MineCraft Packet defined by this object.
      /// </summary>
      public PacketID ID { get; }

      /// <summary>
      /// Gets the Source (sender) of the MineCraft Packet defined by this object.
      /// </summary>
      public PacketSource From { get; }

      /// <summary>
      /// Gets the Name of the MineCraft Packet defined by this object.
      /// </summary>
      public string Name { get; }

      /// <summary>
      /// Gets the FieldDefinition at the given index.
      /// </summary>
      /// <param name="index">The index of the FieldDefinition to return</param>
      /// <returns></returns>
      public FieldDefinition this[int index]
      {
         get
         {
            return _fields[index];
         }
      }


      // TODO add method to parse the defined MineCraft Packet from a BinaryReader

   }
}
