using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline
{
    public class UseMiddleware<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {

        private readonly UpdateDelegate<TContext> _branch;

        public UseMiddleware(UpdateDelegate<TContext> branch)
        {
            _branch = branch;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            await _branch(context, cancellationToken).ConfigureAwait(false);
            await next(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
