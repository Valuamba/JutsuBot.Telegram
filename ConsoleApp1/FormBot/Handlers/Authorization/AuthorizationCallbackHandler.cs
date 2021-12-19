using ConsoleApp1;
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

namespace JutsuForms.Server.FormBot.Handlers.Authorization
{
    //single or multiple select
    public class AuthorizationCallbackHandler : AuthorizationBaseHandler//, INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>, IReplyKeyboardButtonHandler<BotExampleContext>
    {
        public AuthorizationCallbackHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
        }
    }
}
