using CliverBot.Console.DataAccess.Entities;
using CliverBot.Console.DataAccess.Repositories;
using CliverBot.Console.Elements.FormInput;
using CliverBot.Console.Elements.InputTextElement;
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
    public class FormInputTextClient<TBot, TContext> : InputTextClient<TBot>
        where TBot : BaseBot
        where TContext : IUpdateContext
    {
        private readonly FormRepository _formRepository;

        public FormInputTextClient(FormRepository formRepository, TBot bot) : base(bot)
        {
            _formRepository = formRepository;
        }

        public override async Task<Message> SendInformationMessage(TContext context, string propertyName, InputTextLocalizationSettings localizationSettings, IReplyMarkup replyMarkup)
        {
            var chatId = context.Update.GetSenderId();
            var message = await base.SendInformationMessage(chatId, notifyMessage, replyMarkup);

            var formModel = _formRepository.GetFormById(context.UserState.CurrentState.Stage.GetFormById());

            formModel.FormUtilityMessages.Add(new() { ChatId = chatId, MessageId = message.MessageId, MessageType = DataAccess.MessageType.BeDeleted });
            var property = formModel.FormProperties.SingleOrDefault(p => p.PropertyName == propertyName);
            if(property is not null)
            {
                property.PropertyStatus = PropertyStatus.Writing;
                property.Value = null;
                //ef change
            }
            else
            {
                FormPropertyMetadata newProperty = new();

                newProperty.PropertyName = propertyName;
                newProperty.PropertyStatus = PropertyStatus.Writing;
                formModel.FormProperties.Add(newProperty);
                //ef add
            }

            StringBuilder messageBuilder = new();

            messageBuilder.AppendLine(formModel.FormName);
            foreach(var prop in formModel.FormProperties)
            {
                messageBuilder.AppendLine($"{prop}")
            }

            await Client.EditMessageTextAsync(formModel.FormInformationMessage.MessageId, $"{inputModelContext.Name} : {inputModelContext.FillingPlaceholder}")

            return message;
        }

        public override async Task StorePropertyInCache<TModel>(string cacheData, object value, InputTextLocalizationSettings localizationSettings, string propertyName)
        {
            base.StorePropertyInCache<TModel>(out cacheData, value, localizationSettings, propertyName);

            var formModel = _formRepository.GetFormById(context.GetFormById());

            var messageText = formModel.FormInformationMessage.MessageText;
            messageText += $"{localizationSettings.Name}: {value}\r\n";

            await Client.EditMessageTextAsync(formModel.FormInformationMessage.MessageId, messageText)
        }
    }
}
