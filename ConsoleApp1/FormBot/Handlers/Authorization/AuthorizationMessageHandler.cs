using ConsoleApp1;
using ConsoleApp1.FormBot.Extensions;
using JutsuForms.Server.FormBot.Handlers.Authorization.Extensions;
using JutsuForms.Server.FormBot.Models;
using JutsuForms.Server.TgBotFramework.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.Interfaces;
using TgBotFramework.WrapperExtensions;

namespace JutsuForms.Server.FormBot.Handlers.Authorization
{
    public abstract class AuthorizationMessageHandler : AuthorizationBaseHandler, INotify<BotExampleContext>, IUpdateHandler<BotExampleContext>, IReplyKeyboardButtonHandler<BotExampleContext>
    {
        public AuthorizationMessageHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
            : base(formHandlerContext, formContext, formService)
        {
            ChangeReplyKeyboardMarkup = new string[][]
            {
                new [] { "Leave"},
                new [] { "Cancel"},
            };
        }

        protected ReplyKeyboardMarkup ChangeReplyKeyboardMarkup { get; set; }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");

                if (!await TryValidateInput(context, formId))
                    return;

                AddPropertyToCache(context);
                await PrepareFormToNextStep(context, formId, cancellationToken);
                await MooveToNextStep(context, next, cancellationToken);
            }
        }

        public async Task NotifyStep(BotExampleContext context, CancellationToken cancellationToken)
        {
            var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");

            if (context.UserState.CurrentState.Stage.DoesParameterExist(AuthorizationConstants.CHANGE_FORM_INPUT))
            {
                ChangeReplyKeyboardMarkup.ResizeKeyboard = true;
                await FormService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, FormHandlerContext.InformationMessage,
                    infoReplyMarkup: ChangeReplyKeyboardMarkup);
            }
            else
            {
                FormHandlerContext.NavigationReplyMarkup.ResizeKeyboard = true;
                await FormService.SendFormStepInformationMessageAsync(context.Update.GetSenderId(), formId, FormHandlerContext.InformationMessage,
                    infoReplyMarkup: FormHandlerContext.NavigationReplyMarkup);
            }

            await UpdateCurrentForm(context, formId);
            context.UserState.CurrentState.Step++;
        }

        public async Task<bool> HandleChangeNavigationButtons(BotExampleContext context, int formId, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (context.UserState.CurrentState.Stage.TryToGetParamter("prevStep", out int nextStep))
            {
                switch (context.Update.Message.Text)
                {
                    case "Leave":
                        context.UserState.CurrentState.Stage = context.UserState.CurrentState.Stage.RemoveStageParameter("prevStep");
                        context.UserState.CurrentState.Step = nextStep;
                        await context.MooveToRoot(cancellationToken);
                        return true;

                    case "Cancel":
                        await FormService.DeleteUtilityMessages(formId, cancellationToken);
                        await FormService.CancelForm(formId);
                        await context.LeaveStage("menu", cancellationToken);
                        return true;

                    default: return false;
                }
            }

            return false;
        }

        public virtual Task<bool> HandleNavigationButtons(BotExampleContext context, int formId, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public async Task<bool> HandleReplyKeyboardButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (On.Message(context, out Message message))
            {
                var formId = context.UserState.CurrentState.Stage.GetParameter<int>("formId");
                await FormService.HandleInputMessage(context.Update.GetSenderId(), message.MessageId, formId, message.Text);

                return await HandleChangeNavigationButtons(context, formId, prev, next, cancellationToken)
                    || await HandleNavigationButtons(context, formId, prev, next, cancellationToken);
            }

            return false;
        }

        private async Task<bool> TryValidateInput(BotExampleContext context, int formId)
        {
            if (FormHandlerContext.ValidationHandler != null)
            {
                foreach (var validationHandler in FormHandlerContext.ValidationHandler)
                {
                    if (!validationHandler.UpdatePredicate(context))
                    {
                        await FormService.SendValidationErrorMessageAsync(context.Update.GetSenderId(), formId, validationHandler.ErrorMessageAlias);
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task PrepareFormToNextStep(BotExampleContext context, int formId, CancellationToken cancellationToken)
        {
            await FormService.DeleteUtilityMessages(formId, cancellationToken);
            await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, new FormStepMetadata()
            {
                FormName = FormContext.FormName,
                Cache = context.UserState.CurrentState.CacheData
            });
        }

        private void AddPropertyToCache(BotExampleContext context)
        {
            var value = TConverter.ChangeType(FormHandlerContext.FieldType, context.Update.Message.Text);
            string cache = context.UserState.CurrentState.CacheData;
            AuthorizationCacheHelper.AddProperty(ref cache, new PropertyModel()
            {
                Order = FormHandlerContext.Step,
                PropertyName = FormHandlerContext.FieldName,
                Value = value
            });
            context.UserState.CurrentState.CacheData = cache;
        }

        private async Task MooveToNextStep(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
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

        private async Task UpdateCurrentForm(BotExampleContext context, int formId)
        {
            await FormService.UpdateForm(formId, context.UserState.CurrentState.CacheData, 
                new FormStepMetadata()
                {
                    FormName = FormContext.FormName,
                    Field = FormHandlerContext.FieldName,
                    Placeholder = FormHandlerContext.Placeholder,
                    Cache = context.UserState.CurrentState.CacheData
                }); 
        }
    }
}
