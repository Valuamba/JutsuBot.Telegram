using CliverBot.Console.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess.Entities
{
    public class FormPropertyMetadata
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
        public PropertyStatus PropertyStatus { get; set; }
        public object Value { get; set; }

        public FormModel Form { get; set; }
    }

    public enum PropertyStatus
    {
        Skipped,
        Added,
        Changed,
        Writing
    }
}
