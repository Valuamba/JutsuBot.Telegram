using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Jutsu.Telegarm.Bot.Models.Interfaces
{
    public interface IStageContext
    {
        string Stage { get; set; }
        string Parameters { get; set; }
        public StageHandleType UpdateType { get; set; }
        public long? SenderId { get; set; }
        public long? ChatId { get; set; }
        public int? MessageId { get; set; }

    }
}
