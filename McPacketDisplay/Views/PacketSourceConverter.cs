using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using McPacketDisplay.Models.Packets;

namespace McPacketDisplay.Views
{
   public class PacketSourceConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         switch((PacketSource)value)
         {
            case PacketSource.Server:
               return "S->C";

            case PacketSource.Client:
               return "C->S";

            default:
               throw new ApplicationException();
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}
