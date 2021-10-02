using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using PacketDotNet;

namespace McPacketDisplay.ViewModels
{
   public class FilterTcpPackets : IFilterTcpPackets, INotifyPropertyChanged
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
         _applyServerPortFilter = false;

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
         set
         {
            if (value?.Equals(_serverAddress) ?? _serverAddress is null)
               return;

            _serverAddress = value!;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public bool ApplyServerAddressFilter
      {
         get => _applyServerAddressFilter;
         set
         {
            if (_applyServerAddressFilter == value)
               return;

            _applyServerAddressFilter = value;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public ushort ServerPort
      {
         get => _serverPort;
         set
         {
            if (_serverPort == value)
               return;

            _serverPort = value;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public bool ApplyServerPortFilter
      {
         get => _applyServerPortFilter;
         set
         {
            if (_applyServerPortFilter == value)
               return;

            _applyServerPortFilter = value;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public IPAddress ClientAddress
      {
         get => _clientAddress;
         set
         {
            if (value?.Equals(_clientAddress) ?? _clientAddress is null)
               return;

            _clientAddress = value!;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public bool ApplyClientAddressFilter
      {
         get => _applyClientAddressFilter;
         set
         {
            if (_applyClientAddressFilter == value)
               return;

            _applyClientAddressFilter = value;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public ushort ClientPort
      {
         get => _clientPort;
         set
         {
            if (_clientPort == value)
               return;

            _clientPort = value;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public bool ApplyClientPortFilter
      {
         get => _applyClientPortFilter;
         set
         {
            if (_applyClientPortFilter == value)
               return;

            _applyClientPortFilter = value;
            OnPropertyChanged();
         }
      }

      /// <inheritdoc />
      public bool PassPacket(TcpPacket packet)
      {
         IPAddress src, dest;

         if (packet.ParentPacket is IPv4Packet)
         {
            src = ((IPv4Packet)packet.ParentPacket).SourceAddress;
            dest = ((IPv4Packet)packet.ParentPacket).DestinationAddress;
         }
         else
         {
            src = ((IPv6Packet)packet.ParentPacket).SourceAddress;
            dest = ((IPv6Packet)packet.ParentPacket).DestinationAddress;
         }

         if (!CheckAddressFilter(_applyServerAddressFilter, _serverAddress, src, dest,
            _applyServerPortFilter, _serverPort, packet.SourcePort, packet.DestinationPort))
            return false;

         return CheckAddressFilter(_applyClientAddressFilter, _clientAddress, src, dest,
            _applyClientPortFilter, _clientPort, packet.SourcePort, packet.DestinationPort);
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

      #region INotifyPropertyChanged
      public event PropertyChangedEventHandler? PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
      #endregion
   }
}
