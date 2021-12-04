using CliverBot.Console.DataAccess.Repositories;
using CliverBot.Console.Elements.FormInput;
using JutsuBot.Elements;
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

namespace CliverBot.Console.Elements.InputTextElement
{
    public class InputTextClient<TBot, TContext> : IInputTextClient<TContext>
        where TBot : BaseBot
        where TContext : IUpdateContext
    {
        protected TelegramBotClient Client { get; }
        protected MessageLocalizationRepository MessageLocalization { get; }

        public InputTextClient(TBot bot, MessageLocalizationRepository messageLocalization)
        {
            Client = bot.Client;
            MessageLocalization = messageLocalization;
        }

        public virtual async Task<Message> SendInformationMessage(TContext context, string propertyName, InputTextLocalizationSettings localizationSettings, IReplyMarkup replyMarkup)
        {
            var chatId = context.Update.GetSenderId();
            var message = await Client.SendTextMessageAsync(chatId, MessageLocalization.GetMessage(localizationSettings.AddPropertyValueTextAlias), replyMarkup: replyMarkup);
            return message;
        }

        public virtual Task<Message> SendValidationInfo(long chatId, string errorMessageAlias)
        {
            return Client.SendTextMessageAsync(chatId, MessageLocalization.GetMessage(errorMessageAlias));
        }

        public virtual async Task StorePropertyInCache<TModel>(TContext context, object value, InputTextLocalizationSettings localizationSettings, string propertyName)
            where TModel : new()
        {
            context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.AddProperty<TModel>(value, propertyName);
        }
    }
}
