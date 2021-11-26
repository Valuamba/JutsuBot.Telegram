using Jutsu.Telegarm.Bot.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jutsu.Telegarm.Bot.Models
{
    public class StageContext : IStageContext
    {
        public string Stage { get; set; }
        public string Parameters { get; set; }
    }
}
