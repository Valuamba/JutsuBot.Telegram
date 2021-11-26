using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jutsu.Telegarm.Bot.Models.Interfaces
{
    public interface IStageContext
    {
        string Stage { get; set; }
        string Parameters { get; set; }
    }
}
