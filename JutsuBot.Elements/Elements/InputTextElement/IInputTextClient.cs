using CliverBot.Console.Elements.FormInput;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;

namespace CliverBot.Console.Elements.InputTextElement
{
    public interface IInputTextClient<TContext>
        where TContext : IUpdateContext
    {
        Task<Message> SendInformationMessage(TContext context, string propertyName, InputTextLocalizationSettings localizationSettings, IReplyMarkup replyMarkup);

        Task<Message> SendValidationInfo(long chatId, string errorMessageAlias);

        Task StorePropertyInCache<TModel>(TContext context, object value, InputTextLocalizationSettings localizationSettings, string propertyName)
            where TModel : new();
    }
}
