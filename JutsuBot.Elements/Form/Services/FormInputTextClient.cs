using CliverBot.Console.DataAccess;
using CliverBot.Console.DataAccess.Entities;
using CliverBot.Console.DataAccess.Repositories;
using CliverBot.Console.Elements.FormInput;
using CliverBot.Console.Elements.InputTextElement;
using JutsuBot.Elements;
using JutsuBot.Elements.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Form.Services
{
    public class FormInputTextClient<TBot, TContext> : InputTextClient<TBot, TContext>
        where TBot : BaseBot
        where TContext : IUpdateContext
    {
        private readonly FormRepository _formRepository;
        private readonly TrackedMessageRepository _trackedMessageRepository;
        private readonly FormPropertyMetadataRepository _formPropertyRepository;

        public FormInputTextClient(FormRepository formRepository, 
            MessageLocalizationRepository messageLocalization, 
            FormPropertyMetadataRepository formPropertyRepository,
            TrackedMessageRepository trackedMessageRepository,
            TBot bot) : base(bot, messageLocalization)
        {
            _formRepository = formRepository;
            _formPropertyRepository = formPropertyRepository;
            _trackedMessageRepository = trackedMessageRepository;
        }

        //
        public override async Task<Message> SendInformationMessage(TContext context, string propertyName, InputTextLocalizationSettings localizationSettings, IReplyMarkup replyMarkup)
        {
            //add message
            var chatId = context.Update.GetSenderId();
            var message = await base.SendInformationMessage(context, propertyName, localizationSettings, replyMarkup);

            int formId = Convert.ToInt32(context.UserState.CurrentState.Stage.Parse()["form_id"]);

            var formModel = _formRepository.GetFormById(formId);
            if (formModel is null)
            {
                throw new Exception($"The form with Id {formId} doesn't exist.");
            }

            //стоит ли отдельно проверять что форма есть
            await _trackedMessageRepository.AddMessage(new() { ChatId = chatId, MessageId = message.MessageId, MessageType = MessageType.BeDeleted });

            //change or add property status
            await _formPropertyRepository.ChangePropertyStatus(formId, propertyName, PropertyStatus.Writing, null);

            var messageText = GetInformationMessage(formModel.FormProperties);
            await Client.EditMessageTextAsync(chatId, formModel.FormInformationMessage.MessageId, messageText);

            return message;
        }

        private string GetInformationMessage(List<FormPropertyMetadata> formProperties)
        {
            StringBuilder messageBuilder = new();

            //messageBuilder.AppendLine(formModel.FormName);
            foreach (var prop in formProperties)
            {
                var value = prop.PropertyStatus == PropertyStatus.Writing ? MessageLocalization.GetMessage(prop.PlaceholderAlias) : prop.Value;
                messageBuilder.AppendLine($"{MessageLocalization.GetMessage(prop.PropertyName)}: {value}");
            }

            return messageBuilder.ToString();
        }

        private async Task DeleteUtilitMessages(List<TrackedMessage> trackedMessages)
        {
            foreach(var trackedMessage in trackedMessages)
            {
                await Client.DeleteMessageAsync(trackedMessage.ChatId, trackedMessage.MessageId);
            }
            await _trackedMessageRepository.DeleteMessages(trackedMessages);
        }

        public override async Task StorePropertyInCache<TModel>(TContext context, object value, InputTextLocalizationSettings localizationSettings, string propertyName)
        {
            var chatId = context.Update.GetSenderId();

            int formId = Convert.ToInt32(context.UserState.CurrentState.Stage.Parse()["form_id"]);

            var message = context.Update.Message;
            var formModel = _formRepository.GetFormById(formId);

            var utilitMessage = new TrackedMessage() { ChatId = chatId, MessageId = message.MessageId, MessageType = MessageType.BeDeleted };
            await _trackedMessageRepository.AddMessage(utilitMessage);

            formModel.FormUtilityMessages.Add(utilitMessage);

            await _formPropertyRepository.ChangePropertyStatus(formId, propertyName, PropertyStatus.Added, message.Text);

            var messageText = GetInformationMessage(formModel.FormProperties);
            await Client.EditMessageTextAsync(chatId, formModel.FormInformationMessage.MessageId, messageText);

            await DeleteUtilitMessages(formModel.FormUtilityMessages);
        }
    }
}
