using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess.Entities
{
    public class MessageLocalization
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
    }
}
