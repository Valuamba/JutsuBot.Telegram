using Jutsu.Telegarm.Bot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace Htlv.Parser.Pagination
{
    public class NextPrevPagination : IPagination
    {
        public const string ChangeTo = "change_to/";

        public int MaxElementsCount { get; set; }

        public IEnumerable<InlineKeyboardButton> Pagination(int count, int page = 1)
        {
            List<InlineKeyboardButton> controls = new(3);

            if (page == 1 && count <= MaxElementsCount)
            {
                return null;
            }
            else
            {
                if (page == 1)
                {
                    controls.Add(InlineKeyboardButton.WithCallbackData(">", $"{ChangeTo}{page + 1}"));
                }
                else
                {
                    controls.Add(InlineKeyboardButton.WithCallbackData("<", $"{ChangeTo}{page - 1}"));

                    var c = count / (double)(page * MaxElementsCount);

                    if (c > 1)
                    {
                        controls.Add(InlineKeyboardButton.WithCallbackData(">", $"{ChangeTo}{page + 1}"));
                    }
                }
            }

            return controls;
        }

        public IQueryable<T> BuildButtonsList<T>(IQueryable<T> data, int page = 1)
        {
            return data
                  .Skip((page - 1) * MaxElementsCount)
                  .Take(MaxElementsCount);
        }

        public CallbackUpdateDelegate<TContext> HandlerPaginationCallback<T, TContext>(Func<int, StepDelegate<TContext>> notifyMethod)
           where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
                if (context.Update.IsCallbackCommand(NextPrevPagination.ChangeTo))
                {
                    int page = Convert.ToInt32(context.Update.TrimCallbackCommand(NextPrevPagination.ChangeTo));

                    context.UserState.CurrentState.Step--;
                    await notifyMethod(page)(prev, next, context, cancellationToken);

                    return true;
                }

                return false;
            };
        }
    }
}
