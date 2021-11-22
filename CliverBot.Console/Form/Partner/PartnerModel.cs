using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.Form.Partner
{
    public class PartnerModel
    {
        public long Id { get; set; }
        public string BusinessName { get; set; }
        public long INN { get; set; }
        public string Town { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Mall { get; set; }
        public int? CountOfUnitsOnAutumnAndWinter { get; set; }
        public int? CountOfUnitsOnSpring { get; set; }
        public int? CountOfUnitsOnSummer { get; set; }
        public string MobilePhone { get; set; }
        public string EMail { get; set; }
        public long? ManagerId { get; set; }
        public DateTime NoteDateTime { get; set; }
    }
}
