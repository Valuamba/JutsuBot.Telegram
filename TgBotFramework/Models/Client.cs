using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;

namespace Jutsu.Telegarm.Bot.Models
{
    public class Client<TBot> : IClient
        where TBot : BaseBot
    {
        protected TelegramBotClient BotClient { get; }

        public Client(TBot bot)
        {
            BotClient = bot.Client;
        }

        public virtual Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<MessageEntity> entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            return BotClient.SendTextMessageAsync(chatId, text, parseMode, entities, disableWebPagePreview, disableNotification, replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
        }
    }
}
