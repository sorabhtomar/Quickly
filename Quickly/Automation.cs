using System;
using System.Collections.Generic;
using Quickly.Domain.SchemaModels;

namespace Quickly.Domain
{
    public class Automation
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String BusinessArea { get; set; }
        public String FunctionalArea { get; set; }
        public String Path { get; set; }
        public String Target { get; set; }
        public List<Argument> Arg { get; set; }
    }
}
