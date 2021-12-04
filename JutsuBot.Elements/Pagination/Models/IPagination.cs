using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace Htlv.Parser.Pagination
{
    public interface IPagination
    {
        IEnumerable<InlineKeyboardButton> PaginationButtons(int count, int page = 1);

        IQueryable<T> BuildButtonsList<T>(IQueryable<T> data, int page = 1);

        CallbackUpdateDelegate<TContext> HandlerPaginationCallback<T, TContext>(Func<int, StepDelegate<TContext>> notifyMethod) where TContext : IUpdateContext;
    }
}
