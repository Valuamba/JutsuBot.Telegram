using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBotFramework;

namespace Jutsu.Telegarm.Bot.Models
{
    public class Client<TBot> : IClient
        where TBot : BaseBot
    {

        public Client(TBot bot)
        {
        }

        public virtual Task<Message> SendTextMessageAsync(ChatId chatId, string text)
        {
            return null;
        }
    }
}
