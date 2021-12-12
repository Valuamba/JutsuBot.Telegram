using CliverBot.Console.DataAccess;
using Jutsu.Telegarm.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess
{
    public class State
    {
        public int StateId { get; set; }

        public string CacheData { get; set; }

        public string Stage { get; set; }

        public long? Step { get; set; }

        public StatePriority StatePriority { get; set;}

        public long? UserId { get; set; }

        public int? MessageId { get; set; }
        public TrackedMessage Message { get; set; }

        public long? CurrentStateUserId { get; set; }
        public User UserForCurrentState { get; set; }

        public long? MessageStateUserId { get; set; }
        public User MessageStateUser { get; set; }

    }
}
