using CliverBot.Console.Form.v3.Elements;
using Htlv.Parser.Pagination;
using JutsuBot.Elements;
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
    //public class SelectSingleElement<TModel, TItem, TContext> : BaseSelectElement<TModel, TItem, TContext>, ICallbackButtonHandler<TContext>, IUpdateHandler<TContext>, IStep<TContext>
    //   where TContext : IUpdateContext
    //   where TModel : new()
    //{
    //    public async override Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
    //    {
    //        if (context.Update.Type == UpdateType.CallbackQuery)
    //        {
    //            var propertyValue = CallbackConvereter(context);
    //            if (propertyValue is not null)
    //            {
    //                context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.AddProperty<TModel>(propertyValue, PropertyName);
    //                await next(context);
    //            }
    //        }
    //    }
    //}
}
