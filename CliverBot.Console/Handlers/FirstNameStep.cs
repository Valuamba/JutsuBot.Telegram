using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Handlers
{
    public class FirstNameStep : TgBotFramework.IUpdateHandler<BotExampleContext>, IStep<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (new Regex(@"^[А-Яа-я]+$").IsMatch(context.Update.Message.Text))
            {
                //TODO: вынести в константу
                if (context.Update.Message.Text.Length > 160)
                {
                    await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), "Имя слишком длинное. Повторите попытку.");

                }
                else
                {
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData += context.Update.Message.Text;
                    context.UserState.CurrentState.Step++;
                    await next(context);
                }
            }
            else
            {
                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), "Некорректное имя, повторите попытку.");
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), "Введите Имя:");
            context.UserState.CurrentState.Step++;
        }
    }
}
