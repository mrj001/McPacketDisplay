using System;
using System.Globalization;
using System.Net;
using Avalonia.Data;
using Avalonia.Data.Converters;


namespace McPacketDisplay.Views
{
   public class IPAddressConverter : IValueConverter
   {

      public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is IPAddress)
            return value.ToString();
         else
            return new BindingNotification(new InvalidCastException(), BindingErrorType.DataValidationError);
      }

      public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         try
         {
            return IPAddress.Parse((string)value);
         }
         catch (Exception ex)
         {
            return new BindingNotification(ex, BindingErrorType.DataValidationError);
         }
      }
   }
}