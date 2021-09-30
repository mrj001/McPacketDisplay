using System;
using System.Globalization;
using System.Xml;

namespace McPacketDisplay.Models
{
   public class PacketID : IEquatable<PacketID>
   {
      private readonly int _id;

      public PacketID(XmlNode node)
      {
         if (node.Name != "id")
            throw new ArgumentException($"{nameof(node)} must be a node named id");

         int n;
         if (!int.TryParse(node.InnerText, NumberStyles.HexNumber, null, out n))
            throw new InvalidCastException($"{node.InnerText} cannot be interpreted.");

         _id = n;
      }

      /// <summary>
      /// Gets the value of the MineCraft Packet ID.
      /// </summary>
      public int ID { get => _id; }

      public override string ToString()
      {
         return $"0x{_id:x2}";
      }

      public override bool Equals(object? obj)
      {
         return Equals(obj as PacketID);
      }

      public override int GetHashCode()
      {
         return _id.GetHashCode();
      }

      public bool Equals(PacketID? other)
      {
         if (other is null)
            return false;

         return _id == other.ID;
      }

      public static bool operator==(PacketID l, PacketID r)
      {
         return l?.Equals(r) ?? r is null;
      }

      public static bool operator !=(PacketID l, PacketID r)
      {
         return !(l == r);
      }
   }
}
