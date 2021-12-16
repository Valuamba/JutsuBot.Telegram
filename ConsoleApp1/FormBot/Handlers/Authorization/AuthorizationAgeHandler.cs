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
    public class AuthorizationAgeHandler : AuthorizationBaseHandler, INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>, IReplyKeyboardButtonHandler<BotExampleContext>
    { 
        public AuthorizationAgeHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");

                if (new Regex("\\d+").IsMatch(context.Update.Message.Text))
                {
                    var age = Convert.ToInt32(context.Update.Message.Text);
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData.AddProperty(age, nameof(AuthorizationModels.Age));
                    context.UserState.CurrentState.Step++;

                    await FormService.DeleteUtilityMessages(formId, cancellationToken);
                    await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
                    {
                        FormName = "Authorization form",
                        Cache = context.UserState.CurrentState.CacheData
                    });
                    await next(context);
                }
                else
                {
                    await FormService.SendValidationErrorMessageAsync(context.Update.GetSenderId(), formId, "You should write number.");
                }
            }
        }

        public async Task<bool> HandleReplyKeyboardButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
                await FormService.HandleInputMessage(context.Update.GetSenderId(), message.MessageId, formId, message.Text);

                switch (message.Text)
                {
                    case "Cancel":
                        await FormService.DeleteUtilityMessages(formId, cancellationToken);
                        await FormService.CancelForm(formId);
                        await context.LeaveStage("menu", cancellationToken);
                        return true;
                }
            }

            return false;
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");

            ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>() { new KeyboardButton("Cancel") },
            })
            {
                ResizeKeyboard = true
            };

            await FormService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, "Write your age.",
                new FormStepMetadata()
                {
                    FormName = "Authorization form",
                    Field = "Age", 
                    Placeholder = "...✍️", 
                    Cache = context.UserState.CurrentState.CacheData 
                },
                infoReplyMarkup: replyKeyboard);
            context.UserState.CurrentState.Step++;
        }
    }
}
