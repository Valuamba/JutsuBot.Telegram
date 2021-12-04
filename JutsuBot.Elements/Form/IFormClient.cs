using CliverBot.Console.DataAccess;
using Jutsu.Telegarm.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

namespace CliverBot.Console.Form
{
    public interface IFormClient
    {
        void SendValidationInfo();
        void NotifyUser();
        Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, 
            bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, 
            bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default);

    }
}
