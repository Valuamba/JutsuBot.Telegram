using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CliverBot.Console.Elements.InputTextElement
{
    public interface IInputTextClient
    {
        Task<Message> SendInformationMessage(long chatId, string notifyMessage, IReplyMarkup replyMarkup);

        Task<Message> SendValidationInfo(long chatId, string errorMessage);

        void StorePropertyInCache(string text, string propertyName);

    }
}
