using ConsoleApp1;
using ConsoleApp1.FormBot.Extensions;
using JutsuForms.Server.TgBotFramework.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TgBotFramework;
using TgBotFramework.Interfaces;
using System.Linq;

namespace JutsuForms.Server.FormBot.Handlers.Authorization
{
    public abstract class AuthorizationBaseHandler : ICallbackButtonHandler<BotExampleContext>
    {
        protected FormService FormService { get; }
        protected FormContext FormContext { get; set; }
        protected FormHandlerContext FormHandlerContext { get; set; }

        public AuthorizationBaseHandler(FormHandlerContext formHandlerContext, FormContext formContext, FormService formService)
        {
            FormHandlerContext = formHandlerContext;
            FormContext = formContext;
            FormService = formService;
        }

        public async Task<bool> HandleCallbackButton(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if(On.CallbackQuery(context, out CallbackQuery callbackQuery))
            {
                if(callbackQuery.Data.IsCallbackCommand(AuthorizationConstants.CHANGE_FIELD_CALLBACK_PATTERN))
                {
                    var changedPropertyName = callbackQuery.Data.GetParameter<string>("name");
                    var formId = callbackQuery.Data.GetParameter<int>("formId");
                    context.UserState.CurrentState.CacheData = context.UserState.CurrentState.CacheData
                        .RemoveProperty(changedPropertyName)
                        .RemoveProperty(FormHandlerContext.FieldName);

                    var changedFormContext = FormContext.FormHandlersContext.FirstOrDefault(f => f.FieldName == changedPropertyName);

                    context.UserState.CurrentState.Step = changedFormContext.Step;
                    await FormService.DeleteUtilityMessages(formId, cancellationToken);

                    await context.MooveToRoot(cancellationToken);

                    return true;
                }
            }

            return false;
        }
    }
}
