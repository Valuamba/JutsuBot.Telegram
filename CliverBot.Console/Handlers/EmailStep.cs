using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace CliverBot.Console.Handlers
{
    public class EmailStep : IUpdateHandler<BotExampleContext>, IStep<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Message.Text.Contains("@"))
            {
                context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData += context.Update.Message.Text;
                context.UserState.CurrentState.Step = 0;
                await next(context, cancellationToken);
            }
            else
            {
                await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), "Некорректный email, повторите попытку.");
                context.UserState.CurrentState.Step++;
            }
        }

        public async Task SendStepInformationAsync(BotExampleContext context, CancellationToken cancellationToken)
        {
            await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), "Введите Email:");
            context.UserState.CurrentState.Step++;
        }
    }
}
