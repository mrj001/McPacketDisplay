using System;
using System.Globalization;
using McPacketDisplay.Models.Packets;
using McPacketDisplay.Views;
using Xunit;

namespace Test.Views
{
   public class TestPacketSourceConverter
   {
      public static TheoryData<string, PacketSource> Convert_TestData
      {
         get
         {
            var rv = new TheoryData<string, PacketSource>();

            rv.Add("S->C", PacketSource.Server);
            rv.Add("C->S", PacketSource.Client);

            return rv;
         }
      }

      [Theory]
      [MemberData(nameof(Convert_TestData))]
      public void Convert(string expected, PacketSource source)
      {
         PacketSourceConverter conv = new PacketSourceConverter();

         string actual = (string)conv.Convert(source, typeof(string), null, CultureInfo.InvariantCulture);

         Assert.Equal(expected, actual);
      }

      [Fact]
      public void Convert_Throws()
      {
         PacketSourceConverter conv = new PacketSourceConverter();

         Assert.Throws<ApplicationException>(() => conv.Convert(42, typeof(string), null, CultureInfo.InvariantCulture));
      }

      [Fact]
      public void ConvertBack_Throws()
      {
         PacketSourceConverter conv = new PacketSourceConverter();

         Assert.Throws<NotImplementedException>(() => conv.ConvertBack("S->C", typeof(PacketSource), null, CultureInfo.InvariantCulture));
      }


   }
}
