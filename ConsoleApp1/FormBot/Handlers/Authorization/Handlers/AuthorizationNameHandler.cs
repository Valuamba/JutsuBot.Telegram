using ConsoleApp1;
using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuForms.Server.FormBot;
using JutsuForms.Server.FormBot.Handlers;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using JutsuForms.Server.TgBotFramework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace JutsuForms.Server.FormBot.Handlers.Authorization.Handlers
{
    public class AuthorizationNameHandler : AuthorizationMessageHandler
    {
        public AuthorizationNameHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
        }

        public override async Task<bool> HandleNavigationButtons(BotExampleContext context, int formId, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            switch (context.Update.Message.Text)
            {
                case "Cancel":
                    await FormService.DeleteUtilityMessages(formId, cancellationToken);
                    await FormService.CancelForm(formId);
                    await context.LeaveStage("menu", cancellationToken);
                    return true;

                case "Back":

                    var previousFormStep = FormContext.FormHandlersContext.Single(f => f.Step == FormHandlerContext.Step);

                    context.UserState.CurrentState.Step -= 3;
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.RemoveProperty(previousFormStep.FieldName);
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.RemoveProperty(FormHandlerContext.FieldName);
                    await FormService.DeleteUtilityMessages(formId, cancellationToken);
                    await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
                    {
                        FormName = FormContext.FormName,
                        Cache = context.UserState.CurrentState.CacheData
                    });
                    await prev(context, cancellationToken);
                    return true;
            }

            return false;
        }
    }
}
