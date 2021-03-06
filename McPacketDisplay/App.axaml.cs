using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using McPacketDisplay.ViewModels;
using McPacketDisplay.Views;

namespace McPacketDisplay
{
   public class App : Application
   {
      public override void Initialize()
      {
         AvaloniaXamlLoader.Load(this);
      }

      public override void OnFrameworkInitializationCompleted()
      {
         if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
         {
            desktop.MainWindow = new MainWindow();
            IDialogService dialogService = new DialogService(desktop.MainWindow);
            desktop.MainWindow.DataContext = new MainWindowViewModel(dialogService);
         }

         base.OnFrameworkInitializationCompleted();
      }
   }
}