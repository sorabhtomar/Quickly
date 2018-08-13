using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace Quickly.Models
{
    public class Collection
    {
        public List<command> commands;
        public List<URL> URLs;
        public static async Task<Collection> GetCollectionAsync()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("Collection.xml");
            string text = await FileIO.ReadTextAsync(file);
            var serializer = new XmlSerializer(typeof(Collection));

            TextReader reader = new StringReader(text);

            var result = (Collection)serializer.Deserialize(reader);

            reader.Dispose();
            return result;
        }
    }

    public class command
    {
        public String key { get; set; }
        public String value { get; set; }
    }
    public class URL
    {
        public String key { get; set; }
        public String value { get; set; }
    }

}
