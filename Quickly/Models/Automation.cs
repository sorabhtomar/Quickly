using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Quickly;
using System.Threading.Tasks;
using Windows.Storage;
using System.Xml.Serialization;
using System.IO;

namespace Quickly.Domain.SchemaModels
{ 
    //[DataContract]
    public class Argument
    {
        public String Value { get; set; }
        public bool IsOptional { get; set; }
        public bool AskUser { get; set; }
        public List<Option> Options { get; set; }
        public String AskPhrase { get; set; }
    }

   // [DataContract]
    public class Option
    {
        public string Value { get; set; }
        public string ArgString { get; set; }
    }

   // [DataContract]
    public class Automation
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String BusinessArea { get; set; }
        public String FunctionalArea { get; set; }
        public String Path { get; set; }
        public String Target { get; set; }
        public List<Argument> Arguments { get; set; }
    }

    public class AutomationManager
    {
        public static async Task<List<Automation>> GetAutomationsAsync()
        {
            var automations = new List<Automation>();
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Automations");
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            foreach (StorageFile file in files) {
                automations.Add(await GetAutomationAsync(file.DisplayName));
            }
            return automations;
        }

        private static async Task<Automation>GetAutomationAsync(string displayName)
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Automations");
            StorageFile file= await folder.GetFileAsync(displayName+".xml");
            string text = await FileIO.ReadTextAsync(file);
            var serializer = new XmlSerializer(typeof(Automation));

            TextReader reader = new StringReader(text);

            var result = (Automation)serializer.Deserialize(reader);

            reader.Dispose();
            return result;
        }

        public static List<Automation> GetAutomations()
        {
            var automations = new List<Automation> {
                new Automation { Name = "A1", Description = "This is A1", BusinessArea = "b1", FunctionalArea = "f1", Path = "p1", Target = "t1" },
                new Automation { Name = "A2", Description = "This is A2", BusinessArea = "b2", FunctionalArea = "f2", Path = "p2", Target = "t2" },
                new Automation { Name = "A3", Description = "This is A3", BusinessArea = "b3", FunctionalArea = "f3", Path = "p3", Target = "t3" },
                new Automation { Name = "A4", Description = "This is A4", BusinessArea = "b4", FunctionalArea = "f4", Path = "p4", Target = "t4" }
            };
            return automations;
        }

    }
}
