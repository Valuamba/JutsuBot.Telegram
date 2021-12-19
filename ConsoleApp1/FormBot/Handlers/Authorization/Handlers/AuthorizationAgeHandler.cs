using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuForms.Server.FormBot;
using JutsuForms.Server.FormBot.Handlers;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using JutsuForms.Server.TgBotFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationAgeHandler : AuthorizationMessageHandler, IReplyKeyboardButtonHandler<BotExampleContext>
    {
        public AuthorizationAgeHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
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

                default: return false;
            }
        }
    }
}
