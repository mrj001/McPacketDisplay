using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace McPacketDisplay.Views
{
   public class PortNumberConverter : IValueConverter
   {

      public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is ushort)
            return value.ToString();
         else
            return new BindingNotification(new InvalidCastException(), BindingErrorType.DataValidationError);
      }

      public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         try
         {
            ushort port = ushort.Parse((string)value);
            if (port == 0)
               return new BindingNotification(new ArgumentOutOfRangeException(), BindingErrorType.DataValidationError);
            return port;
         }
         catch (Exception ex)
         {
            return new BindingNotification(ex, BindingErrorType.DataValidationError);
         }
      }
   }
}