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
        public string Value { get; set; }

        public string ChangePropertyTextAlias { get; set; }
        public string AddPropertyValueTextAlias { get; set; }
        public string PlaceholderAlias { get; set; }
        public string AddPropertyCommandAlias { get; set; }
        public string ChangePropertyCommandAlias { get; set; }

        public int FormId { get; set; }
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
