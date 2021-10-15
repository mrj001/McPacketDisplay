using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace McPacketDisplay.Models.Packets
{
   public static class MineCraftProtocols
   {
      public static IMineCraftProtocol GetProtocol()
      {
         XmlDocument doc = new XmlDocument();
         Assembly assy = typeof(MineCraftProtocols).Assembly;

         using (Stream xsd = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xsd")!)
            doc.Schemas.Add(XmlSchema.Read(xsd, null)!);

         using (Stream xml = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xml")!)
         using (XmlReader rdr = XmlReader.Create(xml))
         {
            doc.Load(rdr);
            doc.Validate(null);
         }

         return new MineCraftProtocol(doc.FirstChild!.NextSibling!);
      }
   }
}