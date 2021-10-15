using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using McPacketDisplay.ViewModels;

namespace McPacketDisplay.Views
{
   internal class DialogService : IDialogService
   {
      private readonly Window _window;

      public DialogService(Window window)
      {
         _window = window;
      }

      public void GetFileNameFromUser(Action<string> callback)
      {
         FileDialogFilter flt = new FileDialogFilter();
         flt.Name = "Dump Files";
         flt.Extensions = new List<string>() { "pcap", "pcapng" };

         OpenFileDialog dlg = new OpenFileDialog();
         dlg.AllowMultiple = false;
         dlg.Filters = new List<FileDialogFilter>() { flt };
         dlg.Title = "Select Dump File";

         dlg.ShowAsync(_window).ContinueWith((task) =>
         {
            string[] files = task.Result;
            string file;
            if (files is null || files.Length == 0)
               file = String.Empty;
            else
               file = files[0];
            callback(file);
         });
      }
   }
}