using System;

namespace McPacketDisplay.ViewModels
{
   public interface IDialogService
   {
      void GetFileNameFromUser(Action<string> callback);
   }
}