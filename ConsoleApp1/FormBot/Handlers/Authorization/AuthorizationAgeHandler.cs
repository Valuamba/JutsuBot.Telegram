using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuForms.Server.FormBot;
using JutsuForms.Server.TgBotFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationAgeHandler : INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>
    {
        private readonly FormService _formService;

        public AuthorizationAgeHandler(FormService formService)
        {
            _formService = formService;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
                await _formService.HandleInputMessage(context.Update.GetSenderId(), message.MessageId, formId, message.Text);

                if (new Regex("\\d+").IsMatch(context.Update.Message.Text))
                {
                    var age = Convert.ToInt32(context.Update.Message.Text);
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.AddProperty<AuthorizationModels>(age, nameof(AuthorizationModels.Age));
                    context.UserState.CurrentState.Step++;

                    await _formService.DeleteUtilityMessages(formId, cancellationToken);
                    await next(context);
                }
                else
                {
                    await _formService.SendValidationErrorMessageAsync(context.Update.GetSenderId(), formId, "You should write number.");
                }
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
            await _formService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, "Write your age.");
            context.UserState.CurrentState.Step++;
        }
    }
}
