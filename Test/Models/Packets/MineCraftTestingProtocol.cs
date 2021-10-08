using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using McPacketDisplay.Models.Packets;

namespace Test.Models.Packets
{
   internal static class MineCraftTestingProtocol
   {
      private static IMineCraftProtocol _protocol;

      static MineCraftTestingProtocol()
      {
         Assembly assy = AppDomain.CurrentDomain.GetAssemblies().
                  Where<Assembly>(a => !a.IsDynamic && a.Location.EndsWith("McPacketDisplay.dll")).
                  First<Assembly>();

         XmlDocument doc = new XmlDocument();
         using (Stream xml = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xml"))
         using (XmlReader rdr = XmlReader.Create(xml))
            doc.Load(rdr);

         _protocol = new McPacketDisplay.Models.Packets.MineCraftProtocol(doc.FirstChild.NextSibling);
      }

      public static IMineCraftProtocol GetProtocol()
      {
         return _protocol;
      }
   }
}
