using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using McPacketDisplay.ViewModels;
using Moq;
using Xunit;

namespace Test.ViewModels
{
   public class TestMainWindowViewModel
   {
      [Fact]
      public void FileOpen_CanExecute()
      {
         Mock<IDialogService> mockDialogService = new Mock<IDialogService>(MockBehavior.Strict);

         MainWindowViewModel vm = new MainWindowViewModel(mockDialogService.Object);
         ICommand openCommand = vm.OpenCommand;

         Assert.True(openCommand.CanExecute(null));
      }

      [Fact]
      public void FileOpen_Execute_Calls_DialogService()
      {
         bool called = false;
         Mock<IDialogService> mockDialogService = new Mock<IDialogService>(MockBehavior.Strict);
         mockDialogService.Setup(m => m.GetFileNameFromUser(It.IsAny<Action<string>>()))
                          .Callback(() => { called = true; } );

         MainWindowViewModel vm = new MainWindowViewModel(mockDialogService.Object);
         ICommand openCommand = vm.OpenCommand;

         openCommand.Execute(null);

         Assert.True(called);
      }

      [Fact]
      public void Set_FileName()
      {
         Mock<IDialogService> mockDialogService = new Mock<IDialogService>(MockBehavior.Strict);
         MainWindowViewModel vm = new MainWindowViewModel(mockDialogService.Object);
         bool filenameChanged = false;
         bool rawTcpPacketsChanged = false;
         bool filteredTcpPacketsChanged = false;
         bool rawMineCraftPacketsChanged = false;

         vm.PropertyChanged += ((sender, e) => {
            switch(e.PropertyName)
            {
               case nameof(MainWindowViewModel.FileName):
                  filenameChanged = true;
                  break;
            }
         });

         ((INotifyCollectionChanged)vm.RawTcpPackets).CollectionChanged += ((s, e) =>
         {
            rawTcpPacketsChanged = true;
         });

         ((INotifyCollectionChanged)vm.FilteredTcpPackets).CollectionChanged += ((s, e) =>
         {
            filteredTcpPacketsChanged = true;
         });

         ((INotifyCollectionChanged)vm.RawMineCraftPackets).CollectionChanged += ((s, e) =>
         {
            rawMineCraftPacketsChanged = true;
         });

         string expectedFileName = "Files/FourPackets.pcap";

         vm.FileName = expectedFileName;

         Assert.Equal(expectedFileName, vm.FileName);
         Assert.True(filenameChanged);
         Assert.True(rawTcpPacketsChanged);
         Assert.Equal(4, vm.RawTcpPackets.Count());
         Assert.True(filteredTcpPacketsChanged);
         Assert.Equal(4, vm.FilteredTcpPackets.Count());
         Assert.True(rawMineCraftPacketsChanged);
         Assert.Equal(26, vm.RawMineCraftPackets.Count());
      }
   }
}