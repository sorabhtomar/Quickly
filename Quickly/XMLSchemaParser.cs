using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Quickly.Domain;
using Windows.Storage.Streams;
using Quickly.Domain.SchemaModels;

namespace Quickly.Engine
{
    public class XMLSchemaParser
    {
        public static Automation ParseAutomationFile(string xml)
        {

            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Automation));
                //FileInfo fi = new FileInfo(Path.Combine(fileLocation, fileName));
                //using (FileStream fs = fi.Open(FileMode.Open))

                using (TextReader reader = new StringReader(xml))
                {
                    object obj = deserializer.Deserialize(reader);
                    return (Automation)obj;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

