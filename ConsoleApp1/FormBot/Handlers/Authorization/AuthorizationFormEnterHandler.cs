using ConsoleApp1;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace JutsuForms.Server.FormBot.Handlers.Authorization
{
    public class AuthorizationFormEnterHandler : IUpdateHandler<BotExampleContext>
    {
        private readonly FormService _formService;

        public AuthorizationFormEnterHandler(FormService formService)
        {
            _formService = formService;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> prev, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            context.UserState.CurrentState.Stage = await _formService.EnterToForm(context.Update.GetSenderId(), context.UserState.CurrentState.Stage);
            context.UserState.CurrentState.Step++;
            await next(context, cancellationToken);
        }
    }
}
