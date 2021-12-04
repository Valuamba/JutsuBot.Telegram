using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.Elements.FormInput
{
    public class InputTextLocalizationSettings : ElementLocalizationSettings
    {
        public string ChangePropertyTextAlias { get; set; }
        public string AddPropertyValueTextAlias { get; set; }
        public string PlaceholderAlias { get; set; }
        public string AddPropertyCommandAlias { get; set; }
        public string ChangePropertyCommandAlias { get; set; }
    }
}
