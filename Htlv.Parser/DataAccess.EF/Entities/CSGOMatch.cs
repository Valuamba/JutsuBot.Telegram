using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser.Models
{
    public class CSGOMatch
    {
        public int Id { get; set; }
        public DateTime MatchTime { get; set; }
        public string MatchMeta { get; set; }
        public string FirstTeam { get; set; }
        public string SecondTeam { get; set; }
        public string MatchEvent { get; set; }
    }
}
