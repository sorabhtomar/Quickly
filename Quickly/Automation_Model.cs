using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

}
