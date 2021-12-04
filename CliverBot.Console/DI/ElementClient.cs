﻿using CliverBot.Console.DataAccess;
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

namespace CliverBot.Console.DI
{
    public class ElementClient<TBot> : Client<TBot>
        where TBot : BaseBot
    {
        private readonly MessageRepository _messageRepository;

        public ElementClient(MessageRepository messageRepository, TBot bot) : base(bot)
        {
            _messageRepository = messageRepository;
        }

        public override async Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<MessageEntity> entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
        {
            Message message = await BotClient.SendTextMessageAsync(chatId, text, parseMode, entities, disableWebPagePreview, disableNotification, replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
            _messageRepository.AddMessage(new TrackedMessage()
            {
                ChatId = chatId.Identifier.Value,
                MessageId = message.MessageId,
                MessageType = DataAccess.MessageType.BeDeleted
            });

            return message;
        }
    }
}
