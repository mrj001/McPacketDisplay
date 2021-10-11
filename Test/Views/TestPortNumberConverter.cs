using System;
using System.Globalization;
using System.Net;
using Avalonia.Data;
using Avalonia.Data.Converters;
using McPacketDisplay.Views;
using Xunit;

namespace Test.Views
{
   public class TestPortNumberConverter
   {
      public static TheoryData<ushort> Convert_TestData
      {
         get
         {
            TheoryData<ushort> rv = new TheoryData<ushort>();

            rv.Add(65535);
            rv.Add(25565);

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(Convert_TestData))]
      public void Convert_Test(ushort port)
      {
         IValueConverter conv = new PortNumberConverter();

         string actual = (string)conv.Convert(port, typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(port.ToString(), actual);
      }

      [Fact]
      public void Convert_Error()
      {
         IValueConverter conv = new PortNumberConverter();

         object actual = conv.Convert(65536, typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(typeof(BindingNotification), actual.GetType());
         Assert.Equal(BindingErrorType.DataValidationError, ((BindingNotification)actual).ErrorType);
         Assert.Equal(typeof(InvalidCastException), ((BindingNotification)actual).Error.GetType());
      }

      public static TheoryData<string> ConvertBack_TestData
      {
         get
         {
            TheoryData<string> rv = new TheoryData<string>();

            rv.Add("25565");
            rv.Add("42976");

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(ConvertBack_TestData))]
      public void ConvertBack(string port)
      {
         ushort expectedPort = ushort.Parse(port);
         IValueConverter conv = new PortNumberConverter();

         object actual = conv.ConvertBack(port, typeof(ushort), null, CultureInfo.InvariantCulture);

         Assert.True(actual is ushort);
         Assert.Equal(expectedPort, (ushort)actual);
      }

      [Fact]
      public void ConvertBack_Error()
      {
         IValueConverter conv = new PortNumberConverter();

         object actual = conv.ConvertBack("Not a Port", typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(typeof(BindingNotification), actual.GetType());
         Assert.Equal(BindingErrorType.DataValidationError, ((BindingNotification)actual).ErrorType);
         Assert.Equal(typeof(FormatException), ((BindingNotification)actual).Error.GetType());
      }

      [Fact]
      public void ConvertBack_Error_On_Zero_Port()
      {
         IValueConverter conv = new PortNumberConverter();

         object actual = conv.ConvertBack("0", typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(typeof(BindingNotification), actual.GetType());
         Assert.Equal(BindingErrorType.DataValidationError, ((BindingNotification)actual).ErrorType);
         Assert.Equal(typeof(ArgumentOutOfRangeException), ((BindingNotification)actual).Error.GetType());
      }

      [Fact]
      public void ConvertBack_Error_Overflow()
      {
         IValueConverter conv = new PortNumberConverter();

         object actual = conv.ConvertBack("65536", typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(typeof(BindingNotification), actual.GetType());
         Assert.Equal(BindingErrorType.DataValidationError, ((BindingNotification)actual).ErrorType);
         Assert.Equal(typeof(OverflowException), ((BindingNotification)actual).Error.GetType());
      }
   }
}
