
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Models
{
    public class PropertyModel
    {
        public string PropertyName { get; set; }
        public int Order { get; set; }
        public object Value { get; set; }
    }
}
