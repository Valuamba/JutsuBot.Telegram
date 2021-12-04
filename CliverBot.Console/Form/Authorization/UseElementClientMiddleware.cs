using CliverBot.Console.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;

namespace CliverBot.Console.Form.Authorization
{
    public class UseElementClientMiddleware<TContext, TBot> : IUpdateHandler<TContext> 
        where TContext : IUpdateContext
        where TBot : BaseBot
    {
        private readonly ElementClient<TBot> _elementClient;

        public UseElementClientMiddleware(ElementClient<TBot> elementClient)
        {
            _elementClient = elementClient;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            context.CustomClient = _elementClient;

            await next(context, cancellationToken);
        }
    }
}
