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
    public class AuthorizationConfirmation : IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var authorizationModel = context.UserState.CurrentState.CacheData.Deserialize<AuthorizationModels>();

            context.UserState.FullName = authorizationModel.Name;
            context.UserState.Age = authorizationModel.Age;

            await context.LeaveStage("menu", cancellationToken);
        }
    }
}
