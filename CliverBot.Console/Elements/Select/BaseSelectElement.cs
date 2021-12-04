using CliverBot.Console.Extensions;
using CliverBot.Console.Form.v3.Elements;
using Htlv.Parser.Pagination;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Form.Elements.Select
{
    public abstract class BaseSelectElement<TModel, TItem, TContext> : BaseElement<TContext>, ICallbackButtonHandler<TContext>, IUpdateHandler<TContext>, IStep<TContext> /*: BaseElement<TContext>*/
        where TContext : IUpdateContext
        where TModel : new()
    {
        public Func<TContext, IQueryable<TItem>> ItemsSupplier { get; set; }
        public Func<TContext, TItem, string> CallbackDataTemplate { get; set; }
        public Func<TItem, string> InlineButtonTextTemplate { get; set; }
        public Func<TItem, string> SelectedInlineButtonTextTemplate { get; set; }
        public Func<TContext, object> CallbackConvereter { get; set; }
        public IPagination Pagination { get; set; }

        public List<List<InlineKeyboardButton>> GetInlineButtons(TContext context, int page)
        {
            var items = ItemsSupplier(context);

            var count = items.Count();
            var filteredItems = Pagination?.BuildButtonsList(items, page).ToList() ?? items.ToList();

            var paginationButtons = Pagination?.PaginationButtons(count, page);

            List<List<InlineKeyboardButton>> buttons = new();

            var array = context.UserState.CurrentState.CacheData.GetList<TItem>(PropertyName);

            for (int i = 0; i < filteredItems.Count; i++)
            {
                if (array?.Any(item => item.Equals(filteredItems[i])) ?? false)
                {
                    buttons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData(SelectedInlineButtonTextTemplate(filteredItems[i]), CallbackDataTemplate(context, filteredItems[i]))
                    });
                }
                else
                {
                    buttons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData(InlineButtonTextTemplate(filteredItems[i]), CallbackDataTemplate(context, filteredItems[i]))
                    });
                }
            }

            if (paginationButtons != null)
            {
                buttons.Add(paginationButtons.ToList());
            }

            return buttons;
        }

        public abstract Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken);

        public async Task<bool> HandleCallbackButton(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.IsCallbackCommand(NextPrevPagination.ChangeTo))
            {
                context.UserState.CurrentState.Step--;
                await NotifyStep(context, cancellationToken);

                return true;
            }

            return false;
        }

        //Сделать тип, который будет говорить как мы будет брать состояние из текущего или из с БД по messageId
        public async Task NotifyStep(TContext context, CancellationToken cancellationToken)
        {
            int page = 1;

            if (context.Update.IsCallbackCommand(NextPrevPagination.ChangeTo))
            {
                page = Convert.ToInt32(context.Update.TrimCallbackCommand(NextPrevPagination.ChangeTo));
                var buttons = GetInlineButtons(context, page);
                context.UserState.CurrentState.Step++;

                await context.Client.EditMessageTextAsync(context.Update.GetSenderId(),
                   context.Update.CallbackQuery.Message.MessageId,
                   NotifyMessage,
                   parseMode: ParseMode.Html,
                   replyMarkup: new InlineKeyboardMarkup(buttons));
            }
            else
            {
                var buttons = GetInlineButtons(context, page);
                context.UserState.CurrentState.Step++;

                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), NotifyMessage,
                    replyMarkup: new InlineKeyboardMarkup(buttons));
            }
        }
    }
}
