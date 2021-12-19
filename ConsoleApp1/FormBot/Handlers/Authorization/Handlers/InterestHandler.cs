using ConsoleApp1;
using ConsoleApp1.FormBot.Extensions;
using ConsoleApp1.FormBot.Models;
using JutsuForms.Server.FormBot;
using JutsuForms.Server.FormBot.Handlers;
using JutsuForms.Server.FormBot.Handlers.Authorization;
using JutsuForms.Server.FormBot.Models;
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

namespace JutsuForms.Server.FormBot.Handlers.Authorization.Handlers
{
    public class InterestHandler : AuthorizationCallbackHandler, INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>, IReplyKeyboardButtonHandler<BotExampleContext>
    {
        public InterestHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
           : base(formHandlerContext, formContext, formService)
        {
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.CallbackQuery(context, out CallbackQuery callback))
            {
                if(callback.Data.IsCallbackCommand(AuthorizationConstants.CHOOSE_INTEREST_TYPE_CALLBACK_PATTERN))
                {
                    var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
                    var interest = callback.Data.GetCallbackParameter<InterestType>("interest");

                    context.UserState.CurrentState.CacheData = 
                        context.UserState.CurrentState.CacheData.AddItemToArray(interest.ToString(), FormHandlerContext.FieldName);

                    var interestInlineButtons = GetInterestInlineButtons(context, formId);
                    await FormService.EditMessageReplyMarkup(context.Update.GetSenderId(), callback.Message.MessageId, new InlineKeyboardMarkup(interestInlineButtons));

                    await FormService.UpdateTextOfForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
                    {
                        Field = FormHandlerContext.FieldName,
                        FormName = FormContext.FormName,
                        Cache = context.UserState.CurrentState.CacheData
                    });
                }
            }
        }

        public List<List<InlineKeyboardButton>> GetInterestInlineButtons(BotExampleContext context, int formId)
        {
            var interests = context.UserState.CurrentState.CacheData.GetList<InterestType>(FormHandlerContext.FieldName);
            var interestInlineButtons = new List<List<InlineKeyboardButton>>();
            foreach (var interest in Enum.GetValues<InterestType>())
            {
                var interestButtonText = interests is not null && interests.Any(i => i == interest)
                    ? interest.ToString() + " ✅"
                    : interest.ToString();

                interestInlineButtons.Add(new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton(interestButtonText)
                    {
                        CallbackData = string.Format(AuthorizationConstants.CHOOSE_INTEREST_TYPE_CALLBACK_PATTERN, interest, formId)
                    }
                });
            }

            interestInlineButtons.Add(new() { new InlineKeyboardButton("Accept interests") { CallbackData = "accept_interests" } });

            return interestInlineButtons;
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
            FormHandlerContext.NavigationReplyMarkup.ResizeKeyboard = true;

            var interestInlineButtons = GetInterestInlineButtons(context, formId);

            await FormService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, FormHandlerContext.InformationMessage,
                 infoReplyMarkup: FormHandlerContext.NavigationReplyMarkup);
            await FormService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, "Interest type collection:",
                infoReplyMarkup: new InlineKeyboardMarkup(interestInlineButtons));

            await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
            {
                FormName = FormContext.FormName,
                Field = FormHandlerContext.FieldName,
                Placeholder = FormHandlerContext.Placeholder,
                Cache = context.UserState.CurrentState.CacheData
            });

            context.UserState.CurrentState.Step++;
        }

        public async Task<bool> HandleNavigationButtons(BotExampleContext context, int formId, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            switch (context.Update.Message.Text)
            {
                case "Cancel":
                    await FormService.DeleteUtilityMessages(formId, cancellationToken);
                    await FormService.CancelForm(formId);
                    await context.LeaveStage("menu", cancellationToken);
                    return true;

                case "Back":

                    context.UserState.CurrentState.Step -= 3;
                    var previousFormStep = FormContext.FormHandlersContext.Single(f => f.Step == context.UserState.CurrentState.Step);

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

        public override async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.CallbackQuery(context, out CallbackQuery callbackQuery))
            {
                if (callbackQuery.Data.IsCallbackCommand("accept_interests"))
                {
                    var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
                    await FormService.DeleteUtilityMessages(formId, cancellationToken);

                    if (context.UserState.CurrentState.Stage.TryToGetParamter("prevStep", out int nextStep))
                    {
                        context.UserState.CurrentState.Stage = context.UserState.CurrentState.Stage.RemoveStageParameter("prevStep");
                        context.UserState.CurrentState.Step = nextStep;
                        await context.MooveToRoot(cancellationToken);
                    }
                    else
                    {
                        context.UserState.CurrentState.Step++;
                        await next(context);
                    }
                }
                else 
                    return await base.HandleCallbackButton(context, prev, next, cancellationToken);
            }

            return false;
        }

        public async Task<bool> HandleReplyKeyboardButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
                await FormService.HandleInputMessage(context.Update.GetSenderId(), message.MessageId, formId, message.Text);

                return await HandleNavigationButtons(context, formId, prev, next, cancellationToken);
            }

            return false;
        }
    }
}
