using System;
using System.Globalization;
using System.Net;
using Avalonia.Data;
using Avalonia.Data.Converters;
using McPacketDisplay.Views;
using Xunit;

namespace Test.Views
{
   public class TestIPAddressConverter
   {
      public static TheoryData<string> Convert_TestData
      {
         get
         {
            TheoryData<string> rv = new TheoryData<string>();

            rv.Add("127.0.0.1");
            rv.Add("192.168.0.21");

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(Convert_TestData))]
      public void Convert_Test(string ip)
      {
         IPAddress ipAddr = IPAddress.Parse(ip);
         IPAddressConverter conv = new IPAddressConverter();

         string actual = (string)conv.Convert(ipAddr, typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(ip, actual);
      }

      [Fact]
      public void Convert_Error()
      {
         IPAddressConverter actual = new IPAddressConverter();

         object rv = actual.Convert("Not An IP Address", typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(typeof(BindingNotification), rv.GetType());
         Assert.Equal(BindingErrorType.DataValidationError, ((BindingNotification)rv).ErrorType);
      }

      public static TheoryData<string> ConvertBack_TestData
      {
         get
         {
            TheoryData<string> rv = new TheoryData<string>();

            rv.Add("127.0.0.1");
            rv.Add("192.168.0.21");

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(ConvertBack_TestData))]
      public void ConvertBack_Test(string ip)
      {
         IPAddress expectedIP = IPAddress.Parse(ip);
         IValueConverter conv = new IPAddressConverter();

         IPAddress actualIP = (IPAddress)conv.ConvertBack(ip, typeof(IPAddress), null, CultureInfo.InvariantCulture);

         Assert.Equal(expectedIP, actualIP);
      }

      [Fact]
      public void ConvertBack_Error()
      {
         IValueConverter conv = new IPAddressConverter();

         object rv = conv.ConvertBack("192.168f.0.21", typeof(IPAddress), null, CultureInfo.InvariantCulture);

         Assert.Equal(typeof(BindingNotification), rv.GetType());
         Assert.Equal(BindingErrorType.DataValidationError, ((BindingNotification)rv).ErrorType);
      }
   }
}

