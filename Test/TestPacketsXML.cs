using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Xunit;

namespace Test
{
   public class TestPacketsXML
   {
      [Fact]
      public void SchemaCompliant()
      {
         XmlDocument doc = new XmlDocument();

         Assembly assy = AppDomain.CurrentDomain.GetAssemblies().
                  Where<Assembly>(a => !a.IsDynamic && a.Location.EndsWith("McPacketDisplay.dll")).
                  First<Assembly>();

         using (Stream xsd = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xsd"))
            doc.Schemas.Add(XmlSchema.Read(xsd, null));

         // Test that the packets.xml document complies with the XSD.
         // If it does not comply an XmlSchemaValidationException will be thrown and 
         // the test will fail.
         using (Stream xml = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xml"))
         using (XmlReader rdr = XmlReader.Create(xml))
         {
            doc.Load(rdr);
            doc.Validate(null);
         }
      }
   }
}
