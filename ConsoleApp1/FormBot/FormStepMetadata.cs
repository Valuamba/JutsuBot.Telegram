using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot
{
    public class FormStepMetadata
    {
        public string Field { get; set; }
        public string Placeholder { get; set; }

        public string Cache { get; set; }
        public string FormName { get; set; }

        //public override string ToString()
        //{
        //    return Value is null
        //        ? $"{Field}: {Placeholder}"
        //        : $"{Field}: {Value}";
        //}
    }
}
