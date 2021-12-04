using CliverBot.Console.DataAccess;
using Jutsu.Telegarm.Bot.Models;
using JutsuBot.Elements.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;

namespace CliverBot.Console.Form.Authorization
{
    public class AuthClient<TBot> : Client<TBot>
        where TBot : BaseBot
    {
        private readonly TrackedMessageRepository _messageRepository;

        public AuthClient(TrackedMessageRepository messageRepository, TBot bot) : base(bot)
        {
            _messageRepository = messageRepository;
        }

        public override async Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<MessageEntity> entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            var message = await base.SendTextMessageAsync(chatId, text, parseMode, entities, disableWebPagePreview, disableNotification, replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
            await _messageRepository.AddMessage(new TrackedMessage()
            {
                ChatId = chatId.Identifier.Value,
                MessageId = message.MessageId,
                MessageType = DataAccess.MessageType.BeDeleted
            });

            return message;
        }
    }
}
