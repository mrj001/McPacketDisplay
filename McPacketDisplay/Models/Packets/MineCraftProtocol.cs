using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections;

namespace McPacketDisplay.Models.Packets
{
   public class MineCraftProtocol : IMineCraftProtocol
   {
      private string _minecraftVersion = "Beta 1.7.3";

      private int _protocolVersion = 14;

      private List<IMineCraftPacketDefinition> _definitions = new List<IMineCraftPacketDefinition>();

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

      public IMineCraftPacketDefinition this[int index]
      {
         get
         {
            return _definitions[index];
         }
      }

      public int Count { get => _definitions.Count; }

      public IEnumerator<IMineCraftPacketDefinition> GetEnumerator()
      {
         return _definitions.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _definitions.GetEnumerator();
      }
   }
}