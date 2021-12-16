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
using ConsoleApp1.FormBot.Extensions;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using JutsuForms.Server.FormBot.Handlers.Authorization;

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

        public async Task<string> EnterToForm(long userId, string stage)
        {
            var message = await _client.SendTextMessageAsync(userId, "<b>Authorization form</b>\r\n\r\n", Telegram.Bot.Types.Enums.ParseMode.Html);
            var entityForm = await _dbContext.Forms.AddAsync(new FormModel() 
            { 
                UserId = userId, 
                ChatId = userId ,
                FormInformationMessage = new TrackedMessage()
                {
                    ChatId = userId,
                    MessageId = message.MessageId,
                    MessageText = message.Text,
                    MessageType = MessageType.MainFormMessage,
                }
            });

            await _dbContext.SaveChangesAsync();
            return stage.AddParameter("formId", entityForm.Entity.FormId);
        }

        public async Task HandleInputMessage(long chatId, int messageId, int formId, string messageText)
        {
            await AddMessageToForm(chatId, messageId, formId, messageText, MessageType.BeDeleted);
        }

        public async Task CancelForm(int formId)
        {
            var form = await _dbContext.Forms.Include(f => f.FormInformationMessage).SingleOrDefaultAsync(f => f.FormId == formId);
            await _client.DeleteMessageAsync(form.ChatId, form.FormInformationMessage.MessageId);
        }

        public async Task UpdateForm(int formId, string cache, FormStepMetadata formStepMetadata)
        {
            var form = await _dbContext.Forms.Include(f => f.FormInformationMessage).SingleOrDefaultAsync(f => f.FormId == formId);

            var editedMessage = GetMainFormMessage(formStepMetadata);
            var inlineKeyboard = GetInlineKeyboardMarkupForStep(form.ChatId, formId, cache);

            await _client.EditMessageTextAsync(form.ChatId, form.FormInformationMessage.MessageId, editedMessage, Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: inlineKeyboard);
        }

        //Нужно будет добавить проверку, если какое-то сообщение было создано > 48h.
        //Если сообщение бот не смог удалить, то что мне делать с ним в базе?
        public async Task DeleteUtilityMessages(int formId, CancellationToken cancellationToken)
        {
            var form = await _dbContext.Forms.Include(f => f.FormUtilityMessages).SingleOrDefaultAsync(f => f.FormId == formId);
            var utilityMessages = form.FormUtilityMessages.OrderByDescending(k => k.MessageId);

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

        public async Task SendFormStepInformationMessageAsync(long chatId, int formId, string messageText, FormStepMetadata formStepMetadata, IReplyMarkup infoReplyMarkup)
        {
            var form = await _dbContext.Forms.Include(f => f.FormInformationMessage).Include(f => f.FormUtilityMessages).SingleOrDefaultAsync(f => f.FormId == formId);

            var editedMessage = GetMainFormMessage(formStepMetadata);
            var inlineKeyboard = GetInlineKeyboardMarkupForStep(form.ChatId, formId, formStepMetadata.Cache);

            await _client.EditMessageTextAsync(form.ChatId, form.FormInformationMessage.MessageId, editedMessage, Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: inlineKeyboard);
            var message = await _client.SendTextMessageAsync(chatId, messageText, replyMarkup: infoReplyMarkup);

            form.FormUtilityMessages.Add(new TrackedMessage()
            {
                ChatId = chatId,
                MessageId = message.MessageId,
                MessageText = messageText,
                MessageType = MessageType.FormStepInformationMessage,
            });
            await _dbContext.SaveChangesAsync();
        }

        public InlineKeyboardMarkup GetInlineKeyboardMarkupForStep(long userId, int formId, string cache)
        {
            var properties = cache.GetPropertiesNamesWithValue();

            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            foreach(var property in properties)
            {
                buttons.Add(new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"Change {property.ToLower()}", 
                        string.Format(AuthorizationConstants.CHANGE_FIELD_CALLBACK_PATTERN, property, userId, formId))
                });
            }

            return new InlineKeyboardMarkup(buttons);
        }

        private string GetMainFormMessage(FormStepMetadata formStepMetadata)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"<b>{formStepMetadata.FormName}</b>");
            sb.AppendLine();

            if (formStepMetadata.Cache != null)
            {
                sb.AppendLine(formStepMetadata.Cache.JsonToTelegramFormattedString().ToString().Trim());
            }
            if (formStepMetadata.Field != null)
            {
                sb.AppendLine($"<b>{formStepMetadata.Field}</b>: {formStepMetadata.Placeholder}");
            }

            return sb.ToString();
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
