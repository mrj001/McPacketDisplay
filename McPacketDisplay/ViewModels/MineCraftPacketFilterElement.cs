using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;
using ReactiveUI;

namespace McPacketDisplay.ViewModels
{
   /// <summary>
   /// Maintains a count of the number of MineCraft Packets in a Stream of a given ID & PacketSource
   /// and whether or not such a MineCraft Packet should pass the filtering.
   /// </summary>
   public class MineCraftPacketFilterElement : ReactiveObject
   {
      private readonly PacketID _id;

      private readonly PacketSource _source;

      private readonly string _name;

      private int _count;

      private bool _pass;

      /// <summary>
      /// Constructs a new instance of MineCraftPacketFilterElement to match the given MineCraft Packet.
      /// </summary>
      /// <param name="packet">The MineCraft Packet which this filter element is to match.</param>
      public MineCraftPacketFilterElement(IMineCraftPacket packet)
      {
         _id = packet.ID;
         _source = packet.From;
         _name = packet.Name;
         _count = 0;
         _pass = true;
      }

      /// <summary>
      /// Constructs a new instance of MineCraftPacketFilterElement to
      /// match MineCraft Packets of the given definition.
      /// </summary>
      /// <param name="definition">The MineCraft Packet Definition to match.</param>
      public MineCraftPacketFilterElement(IMineCraftPacketDefinition definition)
      {
         _id = definition.ID;
         _source = definition.From;
         _name = definition.Name;
         _count = 0;
         _pass = true;
      }

      public PacketID ID { get => _id; }
      public PacketSource Source { get => _source; }
      public string Name { get => _name; }

      /// <summary>
      /// Gets or Sets the number of times the Packet has occurred in a stream
      /// of MineCraft Packets.
      /// </summary>
      public int PacketCount
      {
         get => _count;
         set => this.RaiseAndSetIfChanged(ref _count, value);
      }

      /// <summary>
      /// Gets or sets whether or not a matching MineCraft Packet should pass a filter.
      /// </summary>
      public bool Pass
      {
         get => _pass;
         set => this.RaiseAndSetIfChanged(ref _pass, value);
      }

      /// <summary>
      /// Determines whether or not the given MineCraft Packet matches this item.
      /// </summary>
      /// <param name="packet">The MineCraft Packet to check.</param>
      /// <returns>True if the Packet is a match for this Filter Element.</returns>
      public bool IsMatch(IMineCraftPacket packet)
      {
         return packet.ID == _id && packet.From == _source;
      }
   }
}
