using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuForms.Server.FormBot;
using JutsuForms.Server.FormBot.Handlers;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using JutsuForms.Server.TgBotFramework.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace ConsoleApp1.FormBot.Handlers
{
    public class AuthorizationNameHandler : AuthorizationBaseHandler, INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>, IReplyKeyboardButtonHandler<BotExampleContext>
    {
        public AuthorizationNameHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");

                if (context.Update.Message.Text.Length < 160)
                {
                    context.UserState.CurrentState.CacheData = 
                        context.UserState.CurrentState.CacheData.AddProperty(context.Update.Message.Text, nameof(AuthorizationModels.Name));
                    context.UserState.CurrentState.Step++;

                    await FormService.DeleteUtilityMessages(formId, cancellationToken);
                    await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
                    {
                        FormName = FormContext.FormName,
                        Cache = context.UserState.CurrentState.CacheData
                    });

                    await next(context);
                }
                else
                {
                    await FormService.SendValidationErrorMessageAsync(context.Update.GetSenderId(), formId, "The word is too long! Pleace, repeat.");
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

                    case "Back":
                        context.UserState.CurrentState.Step -= 3;
                        context.UserState.CurrentState.CacheData.RemoveProperty(nameof(AuthorizationModels.Age));
                        context.UserState.CurrentState.CacheData.RemoveProperty(nameof(AuthorizationModels.Name));
                        await FormService.DeleteUtilityMessages(formId, cancellationToken);
                        await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
                        {
                            FormName = FormContext.FormName,
                            Cache = context.UserState.CurrentState.CacheData
                        });
                        await prev(context, cancellationToken);
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
                new List<KeyboardButton>() { new KeyboardButton("Back") },
                new List<KeyboardButton>() { new KeyboardButton("Cancel") },
            })
            {
                ResizeKeyboard = true
            };

            await FormService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, "Write your name.",
                new FormStepMetadata()
                {
                    FormName = FormContext.FormName,
                    Field = FormHandlerContext.FieldName,
                    Placeholder = "...✍️",
                    Cache = context.UserState.CurrentState.CacheData
                },
                infoReplyMarkup: replyKeyboard);
            context.UserState.CurrentState.Step++;
        }
    }
}
