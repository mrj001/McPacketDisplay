using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using McPacketDisplay.Models;
using McPacketDisplay.Models.Packets;
using PacketDotNet;
using ReactiveUI;

namespace McPacketDisplay.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
      private readonly IDialogService _dialogService;

      public MainWindowViewModel(IDialogService dialogService)
      {
         _dialogService = dialogService;

         OpenCommand = ReactiveCommand.Create(() => FileOpen());

         // When the FileName property changes, get a new RawTcpPackets value.
         var obsRawTcpPackets = this.WhenAnyValue(x => x.FileName)
                                    .Select(filename => ReadFile())
                                    .ToProperty(this, x => x.RawTcpPackets, out _rawTcpPackets);

         var obsFilteredTcpPackets = this.WhenAny(x => x.RawTcpPackets, 
                                                  b => b.Sender.RawTcpPackets.Where(c => TcpFilter.PassPacket(c)))
                                         .ToProperty(this, x => x.FilteredTcpPackets, out _filteredTcpPackets);

         var obsMineCraftPackets = this.WhenAny(x => x.FilteredTcpPackets,
                                                y => y.Sender.GetPackets())
                                       .ToProperty(this, x => x.MineCraftPackets, out _mineCraftPackets);
      }

      private IFilterTcpPackets _tcpFilter = new FilterTcpPackets();

      public IFilterTcpPackets TcpFilter
      {
         get => _tcpFilter;
         set => this.RaiseAndSetIfChanged(ref _tcpFilter, value);
      }

      #region FileName
      private string _filename = String.Empty;

      public string FileName
      {
         get => _filename;
         set => this.RaiseAndSetIfChanged(ref _filename, value);
      }
      #endregion

      private IMineCraftProtocol _protocol = MineCraftProtocols.GetProtocol();

      public IMineCraftProtocol MineCraftProtocol { get => _protocol; }


      #region MineCraftPackets
      private readonly ObservableAsPropertyHelper<IEnumerable<IMineCraftPacket>> _mineCraftPackets;

      public IEnumerable<IMineCraftPacket> MineCraftPackets
      {
         get => _mineCraftPackets.Value;
      }

      private IEnumerable<IMineCraftPacket> GetPackets()
      {
         return McPacketDisplay.Models.Packets.MineCraftPackets.GetPackets(MineCraftProtocol, FilteredTcpPackets);
      }
      #endregion

      #region RawTcpPackets
      private readonly ObservableAsPropertyHelper<IEnumerable<TcpPacket>> _rawTcpPackets;

      public IEnumerable<TcpPacket> RawTcpPackets
      {
         get => _rawTcpPackets.Value;
      }

      private IEnumerable<TcpPacket> ReadFile()
      {
         return TcpPacketList.GetList(this.FileName);
      }
      #endregion

      #region FilteredTcpPackets
      private readonly ObservableAsPropertyHelper<IEnumerable<TcpPacket>> _filteredTcpPackets;

      public IEnumerable<TcpPacket> FilteredTcpPackets
      {
         get => _filteredTcpPackets.Value;
      }
      #endregion

      #region Commands
      public ReactiveCommand<Unit, Unit> OpenCommand { get; }

      private void FileOpen()
      {
         _dialogService.GetFileNameFromUser((file) =>
         {
            if (string.IsNullOrEmpty(file))
               return;

            FileName = file;
         });
      }
      #endregion
   }
}
