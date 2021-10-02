using System;
using System.Xml;
using System.Collections.Generic;

namespace McPacketDisplay.Models
{
   public class MineCraftProtocol
   {
      private string _minecraftVersion = "Beta 1.7.3";

      private int _protocolVersion = 14;

      private List<MineCraftPacketDefinition> _definitions = new List<MineCraftPacketDefinition>();

      public MineCraftProtocol(XmlNode protocol)
      {
         XmlNode packetNode = protocol.FirstChild!;
         while (packetNode is not null)
         {
            MineCraftPacketDefinition def = new MineCraftPacketDefinition(packetNode);
            _definitions.Add(def);
            packetNode = packetNode.NextSibling!;
         }
      }

      public string MineCraftVersion { get => _minecraftVersion; }

      public int ProtocolVersion { get => _protocolVersion; }

      public MineCraftPacketDefinition this[int index]
      {
         get
         {
            return _definitions[index];
         }
      }

      public int Count { get => _definitions.Count; }
   }
}