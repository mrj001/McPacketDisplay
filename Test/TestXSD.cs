using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using Xunit;

namespace Test
{
   public class TestXSD
   {
      [Fact]
      public void Compiles()
      {
         XmlSchemaSet schemas = new XmlSchemaSet();

         Assembly assy = AppDomain.CurrentDomain.GetAssemblies().
                  Where<Assembly>(a => !a.IsDynamic && a.Location.EndsWith("McPacketDisplay.dll")).
                  First<Assembly>();

         using (Stream xsd = assy.GetManifestResourceStream("McPacketDisplay.Resources.packets.xsd"))
            schemas.Add(XmlSchema.Read(xsd, null));

         // If this schema is faulty, this will throw an XmlSchemaException and fail 
         // the test.
         schemas.Compile();
      }
   }
}
