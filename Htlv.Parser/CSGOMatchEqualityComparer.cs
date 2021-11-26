using Htlv.Parser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser
{
    public class CSGOMatchEqualityComparer : IEqualityComparer<CSGOMatch>
    {
        public bool Equals(CSGOMatch x, CSGOMatch y)
        {
            if (x.FirstTeam == y.FirstTeam
                && x.SecondTeam == y.SecondTeam
                && x.MatchMeta == y.MatchMeta
                && x.MatchEvent == y.MatchEvent
                && x.MatchTime == y.MatchTime)
            {
                return true;
            }
            else return false;
        }

        public int GetHashCode([DisallowNull] CSGOMatch obj)
        {
            unchecked
            {
                int result = obj.FirstTeam?.GetHashCode() ?? 0;
                result = (result * 397) ^ (obj.SecondTeam?.GetHashCode() ?? 0);
                result = (result * 397) ^ (obj.MatchEvent?.GetHashCode() ?? 0);
                result = (result * 397) ^ (obj.MatchMeta?.GetHashCode() ?? 0);
                result = (result * 397) ^ (obj.MatchTime.GetHashCode());
                return result;
            }
        }
    }
}
