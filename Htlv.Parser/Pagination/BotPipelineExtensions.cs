using Htlv.Parser.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;
using Telegram.Bot.Types.ReplyMarkups;
using Jutsu.Telegarm.Bot.Extensions;
using Telegram.Bot;
using TgBotFramework.WrapperExtensions;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;

namespace Htlv.Parser.Pagination
{
    public static class BotPipelineExtensions
    {
        public static ILinkedStateMachine<TContext> AddSelector<T, TContext>(
            this ILinkedStateMachine<TContext> pipeline,
            Action<SelectorInfo<T, TContext>> selectorConfigurator)
            where TContext : IUpdateContext
        {
            SelectorInfo<T, TContext> selectorInfo = new();
            selectorConfigurator(selectorInfo);

            PaginationInfo paginationInfo = new();
            selectorInfo.PaginationConfigurator(paginationInfo);

            var updateHandler = selectorInfo.HandleSelectedItem;

            CallbackUpdateDelegate<TContext> callbackUpdateDelegate = paginationInfo.Pagination.HandlerPaginationCallback<T, TContext>(
                    notifyMethod: (page) =>
                        GetNotifyMethod(
                            notifyText: selectorInfo.InformationText,
                            pagination: paginationInfo.Pagination,
                            dataConfigurator: selectorInfo.DataConfigurator,
                            inlineButtonTextTemplate: selectorInfo.InlineButtonTextTemplate,
                            callbackDataTemplate: selectorInfo.CallbackDataTemplate,
                            page)
                        );

            var notifyDelegate = GetNotifyMethod(
                            notifyText: selectorInfo.InformationText,
                            pagination: paginationInfo.Pagination,
                            dataConfigurator: selectorInfo.DataConfigurator,
                            inlineButtonTextTemplate: selectorInfo.InlineButtonTextTemplate,
                            callbackDataTemplate: selectorInfo.CallbackDataTemplate);

            pipeline.Step(
                callbackButtonHandler: callbackUpdateDelegate,
                replyKeyboardButtonHandler: null,
                updateHandler: updateHandler,
                stepHandler: notifyDelegate,
                extendedNextDelegate: selectorInfo.ExtendedNextDelegate,
                extendedPrevDelegate:selectorInfo.ExtendedPrevDelegate);

            return pipeline;
        }

        public static StepDelegate<TContext> GetNotifyMethod<T, TContext>(
           Func<TContext, Task<string>> notifyText,
           IPagination pagination,
           Func<TContext, Task<IQueryable<T>>> dataConfigurator,
           Func<T, string> inlineButtonTextTemplate,
           Func<T, string> callbackDataTemplate,
           int page = 1)
          where TContext : IUpdateContext
        {
            return async (prev, next, context, cancellationToken) =>
            {
                var data = await dataConfigurator(context);

                var count = data.Count();
                var pageElements = await pagination.BuildButtonsList(data, page).ToListAsync();

                List<List<InlineKeyboardButton>> buttons = new();
                for (int i = 0; i < pageElements.Count; i++)
                {
                    buttons.Add(
                        new List<InlineKeyboardButton>()
                        {
                            InlineKeyboardButton.WithCallbackData(
                                inlineButtonTextTemplate(pageElements[i]),
                                callbackDataTemplate(pageElements[i]))
                        });
                }

                var pagingButtons = pagination.Pagination(count, page)?.ToList();

                buttons.Add(pagingButtons);

                var markup = new InlineKeyboardMarkup(buttons);

                //???
                context.UserState.CurrentState.Step++;
                if (context.Update.IsCallbackCommand(NextPrevPagination.ChangeTo))
                {
                    await context.Client.EditMessageTextAsync(context.Update.GetSenderId(),
                       context.Update.CallbackQuery.Message.MessageId,
                       await notifyText(context),
                       parseMode: ParseMode.Html,
                       replyMarkup: markup);
                }
                else
                {
                    await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), await notifyText(context), replyMarkup: markup);
                }
            };
        }
    }
}
