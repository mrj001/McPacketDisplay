using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
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

         // When the MineCraftProtocol property changes, update the MineCraftPacket Filter.
         this.WhenAnyValue(x => x.MineCraftProtocol)
             .Select(protocol => new MineCraftPacketFilter(protocol))
             .ToProperty(this, x => x.MineCraftPacketFilter, out _mineCraftPacketFilter);

         // An observer to fire whenever the TCP Packet Filter changes
         var obsTcpPacketFilter = TcpFilter.WhenAnyPropertyChanged()
                  .StartWith(new IFilterTcpPackets[] { TcpFilter })
                  .Select(filter => BuildTcpPacketFilter(filter));

         // When the FileName property changes, get a new RawTcpPackets value.
         this.WhenAnyValue(x => x.FileName)
             .Subscribe(filename => ReadFile());

         var obsRawTcpPackets = _rawTcpPackets.Connect();
         obsRawTcpPackets.Bind(out _obsRawTcpPackets).Subscribe();

         // When Raw Tcp Packets changes, update the Filtered TCP Packets
         obsRawTcpPackets
                  .Filter(obsTcpPacketFilter)
                  .Sort(new TcpPacketComparer())
                  .Bind(out _filteredTcpPackets)
                  .DisposeMany()
                  .Subscribe();

         // When Filtered TCP Packets change, update the Raw MineCraft Packets.
         // TODO: can we daisy-chain collections? Raw MineCraft Packets should be the
         //       output from another pipeline starting with _filteredTcpPackets.
         ((INotifyCollectionChanged)_filteredTcpPackets).CollectionChanged += ((s, e) => GetPackets());

         var obsRawMineCraft = _rawMineCraftPackets.Connect();
         obsRawMineCraft.Bind(out _obsRawMineCraftPackets)
                  .Subscribe();

         obsRawMineCraft.ObserveOn(RxApp.MainThreadScheduler)
                  .Subscribe((a) => MineCraftPacketFilter.UpdateFilterPacketCounts(this.RawMineCraftPackets));

         var obsMineCraftPacketFilter = MineCraftPacketFilter
                  .WhenAnyPropertyChanged(new string[] { nameof(MineCraftPacketFilter.Serial) })
                  .StartWith(new MineCraftPacketFilter[] { this.MineCraftPacketFilter })
                  .Select(filter => BuildMineCraftPacketFilter(filter));

         obsRawMineCraft.Filter(obsMineCraftPacketFilter)
                  .Sort(MineCraftPacketComparer.Comparer)
                  .Bind(out _filteredMineCraftPackets)
                  .DisposeMany()
                  .Subscribe();
      }

      private Func<ITcpPacket, bool> BuildTcpPacketFilter(IFilterTcpPackets? filter)
      {
         if (filter is null) return packet => true;
         return packet => filter.PassPacket(packet);
      }

      private Func<IMineCraftPacket, bool> BuildMineCraftPacketFilter(MineCraftPacketFilter? filter)
      {
         if (filter is null) return packet => false;
         return packet => filter.Pass(packet);
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
      private readonly SourceList<IMineCraftPacket> _rawMineCraftPackets = new SourceList<IMineCraftPacket>();
      private readonly ReadOnlyObservableCollection<IMineCraftPacket> _obsRawMineCraftPackets;

      public ReadOnlyObservableCollection<IMineCraftPacket> RawMineCraftPackets
      {
         get => _obsRawMineCraftPackets;
      }

      private void GetPackets()
      {
         _rawMineCraftPackets.Edit((x) =>
         {
            x.Clear();
            IEnumerable<IMineCraftPacket> packets = McPacketDisplay.Models.Packets.MineCraftPackets.GetPackets(MineCraftProtocol, FilteredTcpPackets);
            x.AddRange(packets);
         });
      }
      #endregion

      #region MineCraft Packet Filter
      private readonly ObservableAsPropertyHelper<MineCraftPacketFilter> _mineCraftPacketFilter;

      public MineCraftPacketFilter MineCraftPacketFilter
      {
         get => _mineCraftPacketFilter.Value;
      }

      private void UpdateMineCraftPacketFilter()
      {
         MineCraftPacketFilter.UpdateFilterPacketCounts(this.RawMineCraftPackets);
      }
      #endregion

      #region Filtered MineCraft Packets
      private readonly ReadOnlyObservableCollection<IMineCraftPacket> _filteredMineCraftPackets;

      public ReadOnlyObservableCollection<IMineCraftPacket> MineCraftPackets
      {
         get => _filteredMineCraftPackets;
      }
      #endregion

      #region RawTcpPackets
      private readonly SourceList<ITcpPacket> _rawTcpPackets = new SourceList<ITcpPacket>();
      private readonly ReadOnlyObservableCollection<ITcpPacket> _obsRawTcpPackets;

      public ReadOnlyObservableCollection<ITcpPacket> RawTcpPackets
      {
         get => _obsRawTcpPackets;
      }

      private void ReadFile()
      {
         _rawTcpPackets.Edit((x) =>
         {
            x.Clear();
            IEnumerable<ITcpPacket> packets = TcpPacketList.GetList(this.FileName);
            foreach (ITcpPacket packet in packets)
               x.Add(packet);
         });
      }
      #endregion

      #region FilteredTcpPackets
      private readonly ReadOnlyObservableCollection<ITcpPacket> _filteredTcpPackets;

      public ReadOnlyObservableCollection<ITcpPacket> FilteredTcpPackets
      {
         get => _filteredTcpPackets;
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
