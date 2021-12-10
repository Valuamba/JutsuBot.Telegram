using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationNameHandler : INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context))
            {
                if (context.Update.Message.Text.Length < 160)
                {
                    context.UserState.CurrentState.CacheData = 
                        context.UserState.CurrentState.CacheData.AddProperty<AuthorizationModels>(context.Update.Message.Text, nameof(AuthorizationModels.Name));
                    context.UserState.CurrentState.Step++;
                    await next(context);
                }
                else
                {
                    Console.WriteLine("The word is too long! Pleace, repeat.");
                }
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("Write your name.");
            context.UserState.CurrentState.Step++;
        }
    }
}
