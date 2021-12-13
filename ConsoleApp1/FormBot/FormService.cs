using JutsuBot.Elements.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using CliverBot.Console.DataAccess;
using JutsuBot.Elements.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using JutsuForms.Server.TgBotFramework.Helpers;

namespace JutsuForms.Server.FormBot
{
    public class FormService
    {
        private readonly TrackedMessageRepository _messageRepository;
        private readonly TelegramBotClient _client;
        private readonly ApplicationDbContext _dbContext;

        public FormService(TrackedMessageRepository messageRepository, TelegramBotClient client, ApplicationDbContext context)
        {
            _messageRepository = messageRepository;
            _client = client;
            _dbContext = context;
        }

        public async Task EnterToForm(long userId, string stage)
        {
            var entityForm = await _dbContext.Forms.AddAsync(new FormModel() { UserId = userId, ChatId = userId });
            stage = stage.AddParameter("formId", entityForm.Entity.FormId);
        }

        public async Task HandleInputMessage(long chatId, int messageId, int formId, string messageText)
        {
            await AddMessageToForm(chatId, messageId, formId, messageText, MessageType.BeDeleted);
        }

        //Нужно будет добавить проверку, если какое-то сообщение было создано > 48h.
        //Если сообщение бот не смог удалить, то что мне делать с ним в базе?
        public async Task DeleteUtilityMessages(int formId, CancellationToken cancellationToken)
        {
            var form = await _dbContext.Forms.Include(f => f.FormUtilityMessages).SingleOrDefaultAsync(f => f.FormId == formId);
            var utilityMessages = form.FormUtilityMessages;

            foreach(var utilityMessage in utilityMessages)
            {
                await _client.DeleteMessageAsync(form.ChatId, utilityMessage.MessageId, cancellationToken);
            }

            _dbContext.TrackedMessages.RemoveRange(utilityMessages);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SendValidationErrorMessageAsync(long chatId, int formId, string messageText)
        {
            var message = await _client.SendTextMessageAsync(chatId, messageText);
            await AddMessageToForm(chatId, message.MessageId, formId, messageText, MessageType.ValidationError);
        }

        public async Task SendFormStepInformationMessageAsync(long chatId, int formId, string messageText)
        {
            var message = await _client.SendTextMessageAsync(chatId, messageText);
            await AddMessageToForm(chatId, message.MessageId, formId, messageText, MessageType.FormStepInformationMessage);
        }

        private async Task AddMessageToForm(long chatId, int messageId, int formId, string messageText, MessageType messageType)
        {
            var form = await _dbContext.Forms.Include(f => f.FormUtilityMessages).SingleOrDefaultAsync(f => f.FormId == formId);

            form.FormUtilityMessages.Add(new TrackedMessage()
            {
                ChatId = chatId,
                MessageId = messageId,
                MessageText = messageText,
                MessageType = messageType,
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
