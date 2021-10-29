using System;
using System.ComponentModel;
using System.Net;
using McPacketDisplay.Models;

namespace McPacketDisplay.ViewModels
{
   /// <summary>
   /// This interface is used to filter which TCP Packets will be processed for display.
   /// </summary>
   public interface IFilterTcpPackets : INotifyPropertyChanged
   {
      /// <summary>
      /// Gets or sets the Server IP Address.
      /// </summary>
      /// <remarks>
      /// Packets pass this filter if either their Source Address or their
      /// Destination Address matches the Server.
      /// </remarks>
      IPAddress ServerAddress { get; set; }

      /// <summary>
      /// Gets or sets whether or not Packets are filtered by the Server IP Address.
      /// </summary>
      bool ApplyServerAddressFilter { get; set; }

      /// <summary>
      /// Gets or sets the Server Port.
      /// </summary>
      ushort ServerPort { get; set; }

      /// <summary>
      /// Gets or sets whether or not Packets are filtered by the Server Port.
      /// </summary>
      bool ApplyServerPortFilter { get; set; }

      /// <summary>
      /// Gets or sets the Client IP Address of the filter.
      /// </summary>
      IPAddress ClientAddress { get; set; }

      /// <summary>
      /// Gets or sets whether or not Packets are filtered by the Client Address.
      /// </summary>
      bool ApplyClientAddressFilter { get; set; }

      /// <summary>
      /// Gets or sets the Client Port.
      /// </summary>
      ushort ClientPort { get; set; }

      /// <summary>
      /// Gets or sets whether or not Packets are filtered by the Client Port.
      /// </summary>
      bool ApplyClientPortFilter { get; set; }

      /// <summary>
      /// Determines whether or not the given Packet passes the Filter.
      /// </summary>
      /// <param name="packet">The TCP Packet to check.</param>
      /// <returns>True if the Packet passes the filter; false otherwise.</returns>
      bool PassPacket(ITcpPacket packet);
   }
}
