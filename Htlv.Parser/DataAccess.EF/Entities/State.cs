using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser.DataAccess.EF.Entities
{
    public class State
    {
        public int StateId { get; set; }
        public string CacheData { get; set; }
        public string Stage { get; set; }
        public long? Step { get; set; }
        public int? MessageId { get; set; }

        public long? CurrentStateUserId { get; set; }
        public User UserForCurrentState { get; set; }

        public long? PrevStateUserId { get; set; }
        public User UserForPrevState { get; set; }

        public long? MessageStateUserId { get; set; }
        public User UserForMessageState { get; set; }
    }
}
