using ConsoleApp1;
using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuForms.Server.TgBotFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace JutsuForms.Server.FormBot.Handlers
{
    public class AuthorizationEnding : IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var result = context.UserState.CurrentState.Stage.GetParameter<bool>("result");
            if (result)
            {
                var authorizationModel = context.UserState.CurrentState.CacheData.Deserialize<AuthorizationModels>();

                context.UserState.FullName = authorizationModel.Name;
                context.UserState.Age = authorizationModel.Age;

                await context.BotClient.SendTextMessageAsync(context.Update.GetSenderId(), "Authorization was successful.");

                await context.LeaveStage("menu", cancellationToken);
            }
            else
            {
                context.UserState.CurrentState.Step = 0;
                context.UserState.CurrentState.Stage = "visit";
                await context.LeaveStage("menu", cancellationToken);
            }
        }
    }
}
