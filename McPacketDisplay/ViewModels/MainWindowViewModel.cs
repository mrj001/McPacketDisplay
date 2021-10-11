using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace McPacketDisplay.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

      private IFilterTcpPackets _tcpFilter = new FilterTcpPackets();

      public IFilterTcpPackets TcpFilter
      {
         get => _tcpFilter;
         set => this.RaiseAndSetIfChanged(ref _tcpFilter, value);
      }
   }
}
