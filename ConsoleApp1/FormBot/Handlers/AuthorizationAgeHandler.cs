using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Interfaces;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationAgeHandler : INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context))
            {
                if (new Regex("\\d+").IsMatch(context.Update.Message.Text))
                {
                    var age = Convert.ToInt32(context.Update.Message.Text);
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.AddProperty<AuthorizationModels>(age, nameof(AuthorizationModels.Age));
                    context.UserState.CurrentState.Step++;
                    await next(context);
                }
                else
                {
                    Console.WriteLine("You should write number.");
                }
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("Write your age.");
            context.UserState.CurrentState.Step++;
        }
    }
}
