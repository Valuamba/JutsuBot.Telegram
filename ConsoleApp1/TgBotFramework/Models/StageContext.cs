using Jutsu.Telegarm.Bot.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Jutsu.Telegarm.Bot.Models
{
    public class StageContext : IStageContext
    {
        public string Stage { get; set; }
        public string Parameters { get; set; }
        public UpdateType UpdateType { get; set; }
        public long SenderId { get; set; }
        public long ChatId { get; set; }
        public int MessageId { get; set; }
        StageHandleType IStageContext.UpdateType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        long? IStageContext.SenderId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        long? IStageContext.ChatId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        int? IStageContext.MessageId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
