using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline
{
    public class UseWhenMiddleware<TContext> : ICallbackButtonHandler<TContext> where TContext : IUpdateContext
    {
        private readonly Predicate<TContext> _predicate;

        private readonly UpdateDelegate<TContext> _branch;

        public UseWhenMiddleware(Predicate<TContext> predicate, UpdateDelegate<TContext> branch)
        {
            _predicate = predicate;
            _branch = branch;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> prev, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            if (_predicate(context))
            {
                await _branch(context, cancellationToken).ConfigureAwait(false);
            }

            await next(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
