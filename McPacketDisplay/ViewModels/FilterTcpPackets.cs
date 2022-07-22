using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using ReactiveUI;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;

namespace McPacketDisplay.ViewModels
{
   public class FilterTcpPackets : ViewModelBase, IFilterTcpPackets
   {
      private IPAddress _serverAddress;
      private bool _applyServerAddressFilter;

      private ushort _serverPort;
      private bool _applyServerPortFilter;

      private IPAddress _clientAddress;
      private bool _applyClientAddressFilter;

      private ushort _clientPort;
      private bool _applyClientPortFilter;

      public FilterTcpPackets()
      {
         _serverAddress = IPAddress.Loopback;
         _applyServerAddressFilter = false;

         _serverPort = 25565;
         _applyServerPortFilter = true;

         _clientAddress = IPAddress.Loopback;
         _applyClientAddressFilter = false;

         _clientPort = ushort.MaxValue;
         _applyClientPortFilter = false;
      }

      #region IFilterTcpPackets
      /// <inheritdoc />
      public IPAddress ServerAddress
      {
         get => _serverAddress;
         set => this.RaiseAndSetIfChanged(ref _serverAddress, value);
      }

      /// <inheritdoc />
      public bool ApplyServerAddressFilter
      {
         get => _applyServerAddressFilter;
         set => this.RaiseAndSetIfChanged(ref _applyServerAddressFilter, value);
      }

      /// <inheritdoc />
      public ushort ServerPort
      {
         get => _serverPort;
         set => this.RaiseAndSetIfChanged(ref _serverPort, value);
      }

      /// <inheritdoc />
      public bool ApplyServerPortFilter
      {
         get => _applyServerPortFilter;
         set => this.RaiseAndSetIfChanged(ref _applyServerPortFilter, value);
      }

      /// <inheritdoc />
      public IPAddress ClientAddress
      {
         get => _clientAddress;
         set => this.RaiseAndSetIfChanged(ref _clientAddress, value);
      }

      /// <inheritdoc />
      public bool ApplyClientAddressFilter
      {
         get => _applyClientAddressFilter;
         set => this.RaiseAndSetIfChanged(ref _applyClientAddressFilter, value);
      }

      /// <inheritdoc />
      public ushort ClientPort
      {
         get => _clientPort;
         set => this.RaiseAndSetIfChanged(ref _clientPort, value);
      }

      /// <inheritdoc />
      public bool ApplyClientPortFilter
      {
         get => _applyClientPortFilter;
         set => this.RaiseAndSetIfChanged(ref _applyClientPortFilter, value);
      }

      /// <inheritdoc />
      public bool PassPacket(ITcpPacket packet)
      {
         IPAddress src = packet.SourceAddress;
         IPAddress dest = packet.DestinationAddress;

         if (!CheckAddressFilter(_applyServerAddressFilter, _serverAddress, src, dest,
            _applyServerPortFilter, _serverPort, packet.SourcePort, packet.DestinationPort))
            return false;

         return CheckAddressFilter(_applyClientAddressFilter, _clientAddress, src, dest,
            _applyClientPortFilter, _clientPort, packet.SourcePort, packet.DestinationPort);
      }

      /// <inheritdoc />
      public PacketSource GetPacketSource(ITcpPacket packet)
      {
         if (_serverPort != packet.SourcePort)
            return PacketSource.Client;

         if (_applyServerAddressFilter && !_serverAddress.Equals(packet.SourceAddress))
            return PacketSource.Client;

         return PacketSource.Server;
      }

      private static bool CheckAddressFilter(bool applyAddressFilter, IPAddress filterAddress,
               IPAddress sourceAddress, IPAddress destinationAddress,
               bool applyPortFilter, ushort filterPort, ushort sourcePort, ushort destinationPort)
      {
         if (applyAddressFilter)
         {
            if (applyPortFilter)
            {
               return ((sourceAddress.Equals(filterAddress) && (sourcePort == filterPort)) ||
                  (destinationAddress.Equals(filterAddress) && (destinationPort == filterPort)));
            }
            else
            {
               return sourceAddress.Equals(filterAddress) || destinationAddress.Equals(filterAddress);
            }
         }
         else
         {
            if (applyPortFilter)
            {
               return sourcePort == filterPort || destinationPort == filterPort;
            }
            else
            {  // No filters are applied; the packet passes.
               return true;
            }
         }
      }

      #endregion
   }
}
