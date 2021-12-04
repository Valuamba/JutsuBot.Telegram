using CliverBot.Console.DataAccess.Repositories;
using CliverBot.Console.Elements.FormInput;
using CliverBot.Console.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;

namespace CliverBot.Console.Elements.InputTextElement
{
    public class InputTextClient<TBot> : IInputTextClient
        where TBot : BaseBot
    {
        protected TelegramBotClient Client { get; }

        public InputTextClient(TBot bot, MessageLocalizationRepository messageLocalization)
        {
            Client = bot.Client;
        }

        public virtual Task<Message> SendInformationMessage(long chatId, string notifyMessage, IReplyMarkup replyMarkup)
        {
            return Client.SendTextMessageAsync(chatId, notifyMessage, replyMarkup: replyMarkup);
        }

        public virtual Task<Message> SendValidationInfo(long chatId, string errorMessage)
        {
            return Client.SendTextMessageAsync(chatId, errorMessage);
        }

        public virtual void StorePropertyInCache<TModel>(out string cacheData, object value, InputTextLocalizationSettings localizationSettings, string propertyName)
            where TModel : new()
        {
            cacheData = null;
            cacheData = cacheData.AddProperty<TModel>(value, propertyName);
        }
    }
}
