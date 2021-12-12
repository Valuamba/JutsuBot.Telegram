using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationValidation : IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var authorizationModel = context.UserState.CurrentState.CacheData.Deserialize<AuthorizationModels>();

            if (authorizationModel.Age != 0 && !string.IsNullOrWhiteSpace(authorizationModel.Name))
            {
                context.UserState.CurrentState.Step++;
                await next(context, cancellationToken);
            }
        }
    }
}
